using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Input;
using OfficeOpenXml;
using MsBox.Avalonia.Enums;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using GeneracionNumeracionAvalonia.Resources;
using GeneracionNumeracionAvalonia.Base;
using System.Reflection;

namespace GeneracionNumeracionAvalonia.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    int serie = 0;
    string filename = string.Empty;
    string path = string.Empty;

    public List<string> Extensions { get; set; } = new List<string>() { AppResources.ArchivoExcel, AppResources.ArchivoCSV };

    [ObservableProperty]
    private int counters = 1;

    [ObservableProperty]
    private int? start = 0;

    [ObservableProperty]
    private int? end = 0;

    [ObservableProperty]
    private int selectedFormat = 0;

    [ObservableProperty]
    private bool isCheckedDefaultFormat = true;


    public ICommand? GenerateCommand { get; set; }
    public ICommand? ShutdownCommand { get; set; }
    public ICommand? VersionCommand { get; set; }


    public MainWindowViewModel()
    {
        CreateCommands();
    }

    void CreateCommands()
    {
        GenerateCommand = new Command(GenerateExecute);
        ShutdownCommand = new Command(ShutdownExecute);
        VersionCommand = new Command(VersionExecute);
    }

    async void VersionExecute(object? parameter)
    {
        var assembly = Assembly.GetExecutingAssembly();
        using var stream = assembly.GetManifestResourceStream("GeneracionNumeracionAvalonia.Version.txt");
        var version = stream is not null
            ? new StreamReader(stream).ReadToEnd().Trim()
            : "??";

        // Mostrar la versión en un MessageBox

        await MessageBoxService
                .GetMessageBoxStandard(
                    AppResources.Aviso,
                    $"Versión: {version}",
                    ButtonEnum.Ok,
                    Icon.Info);
    }

    async void ShutdownExecute(object? parameter)
    {
        var result = await MessageBoxService
               .GetMessageBoxStandard(
                   AppResources.Aviso,
                   AppResources.SalirPregunta,
                   ButtonEnum.YesNo,
                   Icon.Question);

        if (result == ButtonResult.Yes)
        {
            LoggerService?.LogInformation("Quiting App.");
            App.GeneradorApp?.Shutdown();
        }
    }

    async Task SaveFileDialog()
    {
        try
        {
            var mainWindow = App.GeneradorApp?.MainWindow;
            if (mainWindow?.StorageProvider is not { } provider)
            {
                throw new NullReferenceException("StorageProvider is not available. Try again.");
            }

            var xlsxType = new FilePickerFileType(AppResources.ArchivoExcel)
            {
                Patterns = new List<string> { "*.xlsx" },
                MimeTypes = new List<string> { "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" }
            };
            var csvType = new FilePickerFileType(AppResources.ArchivoCSV)
            {
                Patterns = new List<string> { "*.csv" },
                MimeTypes = new List<string> { "text/csv" }
            };

            // El formato seleccionado va primero en la lista
            bool isExcel = SelectedFormat == (int)ExtensionesEnum.Excel;
            var fileTypes = isExcel
                ? new List<FilePickerFileType> { xlsxType, csvType }
                : new List<FilePickerFileType> { csvType, xlsxType };

            var ext = GetExtension();
            var suggestedNameWithoutExt = filename.EndsWith(ext, StringComparison.OrdinalIgnoreCase)
                ? filename[..^ext.Length]
                : filename;

            var file = await provider.SaveFilePickerAsync(new FilePickerSaveOptions()
            {
                Title = AppResources.GuardarNumerador,
                FileTypeChoices = fileTypes,
                DefaultExtension = ext.TrimStart('.'),
                ShowOverwritePrompt = true,
                SuggestedFileName = suggestedNameWithoutExt,
            });

            if (file is null)
                return;

            // TryGetLocalPath() es el método correcto en Avalonia (multiplataforma)
            path = file.TryGetLocalPath() ?? file.Path.LocalPath;

            // Sincronizar SelectedFormat con la extensión que escogió el usuario
            if (path.EndsWith(".xlsx", StringComparison.OrdinalIgnoreCase))
                SelectedFormat = (int)ExtensionesEnum.Excel;
            else if (path.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
                SelectedFormat = (int)ExtensionesEnum.CSV;

            // Asegurar que la extensión está presente en el path
            var finalExt = GetExtension();
            if (!path.EndsWith(finalExt, StringComparison.OrdinalIgnoreCase))
            {
                path += finalExt;
            }
        }
        catch (Exception e)
        {
            System.Diagnostics.Debug.WriteLine($"SaveFileDialog Error: {e.Message}");
            LoggerService?.LogError(e, nameof(SaveFileDialog));
            await MessageBoxService
                .GetMessageBoxStandard(
                    AppResources.Error,
                    $"{AppResources.ErrorRuta}: {e.Message}",
                    ButtonEnum.Ok,
                    Icon.Error);
        }
    }

    async Task Browse()
    {
        try
        {
            serie = (End.Value - Start.Value + 1) / Counters;
            if (Counters != 1)
            {
                filename = $"{Counters} {AppResources.NumeradoresDel} {Start.Value} {AppResources.A} {End.Value}{GetExtension()}";
            }
            else
            {
                filename = $"{Counters} {AppResources.NumeradorDel} {Start.Value} {AppResources.A} {End.Value}{GetExtension()}";
            }

            await SaveFileDialog();
        }
        catch (Exception e)
        {
            System.Diagnostics.Debug.WriteLine(e.Message);
            await MessageBoxService
                .GetMessageBoxStandard(
                    AppResources.Error,
                    AppResources.ErrorRuta,
                    ButtonEnum.Ok,
                    Icon.Error);
            LoggerService?.LogError(e, nameof(Browse));
        }
    }

    async void GenerateExecute(object? parameter)
    {
        try
        {
            if (!await Validation())
                return;

            await Browse();

            // Validar que el usuario seleccionó un archivo
            if (string.IsNullOrEmpty(path))
            {
                LoggerService?.LogInformation("User cancelled file selection.");
                return;
            }

            bool result = false;

            switch (SelectedFormat)
            {
                case (int)ExtensionesEnum.Excel:
                    result = GenerateExcel();
                    break;
                case (int)ExtensionesEnum.CSV:
                    result = GenerateCSV();
                    break;
            }

            if (result)
            {
                await MessageBoxService
                .GetMessageBoxStandard(
                    AppResources.Aviso,
                    AppResources.GuardadoCorrectamente,
                    ButtonEnum.Ok,
                    Icon.Success);

                ResetValues();
            }
        }
        catch (Exception e)
        {
            System.Diagnostics.Debug.WriteLine(e.Message);
            LoggerService?.LogError(e, nameof(GenerateExecute));
            await MessageBoxService
                .GetMessageBoxStandard(
                   AppResources.Error,
                   e.Message,
                   ButtonEnum.Ok,
                   Icon.Error);
        }

    }

    bool GenerateExcel()
    {
        try
        {
            if (string.IsNullOrEmpty(path))
                throw new InvalidOperationException("Path cannot be empty.");

            ExcelPackage.LicenseContext = LicenseContext.Commercial;

            using ExcelPackage excel = new();
            var worksheet = excel.Workbook.Worksheets.Add("Hoja1");

            // Generar encabezados
            string rangeHeaders = GenerateHeaderRow(worksheet);

            // Generar datos
            GenerateDataRows(worksheet, rangeHeaders);

            // Guardar archivo Excel
            FileInfo excelFile = new(path);
            excel.SaveAs(excelFile);

            return true;
        }
        catch (Exception e)
        {
            System.Diagnostics.Debug.WriteLine(e.Message);
            LoggerService?.LogError(e, nameof(GenerateExcel));
            throw;
        }
    }

    bool GenerateCSV()
    {
        try
        {
            if (string.IsNullOrEmpty(path))
                throw new InvalidOperationException("Path cannot be empty.");

            var headers = string.Join("; ", GenerateHeaderCounters());

            var numbers = GenerateNumbers();

            using StreamWriter writer = new(path);

            writer.WriteLine(headers);

            foreach (var row in numbers)
            {
                writer.WriteLine(string.Join("; ", row));
            }

            return true;
        }
        catch (Exception e)
        {
            System.Diagnostics.Debug.WriteLine(e.Message);
            LoggerService?.LogError(e, nameof(GenerateCSV));
            throw;
        }
    }

    string GenerateHeaderRow(ExcelWorksheet worksheet)
    {
        List<string[]> headerRow = new();
        string[] cabecera = GenerateHeaderCounters();

        headerRow.Add(cabecera);
        string rangeCabecera = "A1:";
        rangeCabecera = GetRangeHeaders(cabecera, rangeCabecera);

        worksheet.Cells[rangeCabecera].LoadFromArrays(headerRow);

        return rangeCabecera;
    }

    string[] GenerateHeaderCounters()
    {
        string[] cabecera = new string[Counters];

        for (int j = 1; j <= Counters; j++)
        {
            cabecera[j - 1] = AppResources.Numerador + j;
        }

        return cabecera;
    }

    void GenerateDataRows(ExcelWorksheet worksheet, string rangeCabecera)
    {
        List<string[]> datos = GenerateNumbers();
        string rangoDatos = rangeCabecera.Replace("1", "2");
        worksheet.Cells[rangoDatos].LoadFromArrays(datos);
    }

    string GetRangeHeaders(string[] cabecera, string headerRange)
    {
        if (cabecera.Length == 1)
        {
            headerRange += "A1";
        }
        if (cabecera.Length > 1)
        {
            var letraColumna = GetColumnNameFromIndex(Counters);
            headerRange += letraColumna + "1";
        }

        return headerRange;
    }

    List<string[]> GenerateNumbers()
    {
        List<string[]> datos = new List<string[]>();

        for (int i = Start.Value; i <= Start.Value + serie - 1; i++)
        {
            var linea = NewLine(i);
            datos.Add(linea);
        }

        return datos;
    }

    static string GetColumnNameFromIndex(int column)
    {
        column--;
        string col = Convert.ToString((char)('A' + (column % 26)));
        while (column >= 26)
        {
            column = (column / 26) - 1;
            col = Convert.ToString((char)('A' + (column % 26))) + col;
        }
        return col;
    }

    void ResetValues()
    {
        Start = 0;
        End = 0;
        path = string.Empty;
        Counters = 1;
        SelectedFormat = 0;
    }

    async Task<bool> Validation()
    {
        if (!Start.HasValue)
        {
            await MessageBoxService
               .GetMessageBoxStandard(
                   AppResources.Error,
                   AppResources.ErrorFinMenorInicio,
                   ButtonEnum.Ok,
                   Icon.Error);
            LoggerService?.LogInformation(AppResources.ErrorFinMenorInicio);
            return false;
        }
        if (!End.HasValue || End <= 0)
        {
            await MessageBoxService
               .GetMessageBoxStandard(
                   AppResources.Error,
                   AppResources.ErrorFinMenor0,
                   ButtonEnum.Ok,
                   Icon.Error);
            LoggerService?.LogInformation(AppResources.ErrorFinMenor0);
            return false;
        }
        if (Start.Value >= End.Value)
        {
            await MessageBoxService
               .GetMessageBoxStandard(
                   AppResources.Error,
                   AppResources.ErrorFinMenorInicio,
                   ButtonEnum.Ok,
                   Icon.Error);
            LoggerService?.LogInformation(AppResources.ErrorFinMenorInicio);
            return false;
        }
        if (Counters <= 0)
        {
            await MessageBoxService
               .GetMessageBoxStandard(
                   AppResources.Error,
                   AppResources.ErrorNumeradores0,
                   ButtonEnum.Ok,
                   Icon.Error);
            LoggerService?.LogInformation(AppResources.ErrorNumeradores0);
            return false;
        }
        if (((End.Value - Start.Value + 1) % Counters) != 0)
        {
            var result = await MessageBoxService
               .GetMessageBoxStandard(
                   AppResources.Aviso,
                   AppResources.DescuadreRegistroNumeradores,
                   ButtonEnum.YesNo,
                   Icon.Warning);
            LoggerService?.LogInformation(AppResources.DescuadreRegistroNumeradores);
            if (result == ButtonResult.No)
                return false;
        }
        return true;
    }

    string[] NewLine(int registro)
    {
        string[] linea = new string[Counters];

        for (int i = 0; i < Counters; i++)
        {
            linea[i] = IsCheckedDefaultFormat ? (registro + (serie * i)).ToString("D4") : registro.ToString();
        }
        return linea;
    }

    string GetExtension()
    {
        return SelectedFormat switch
        {
            (int)ExtensionesEnum.Excel => AppResources.xlsx,
            (int)ExtensionesEnum.CSV => AppResources.csv,
            _ => AppResources.xlsx,
        };
    }
}
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using GeneracionNumeracionAvalonia.Services;
using GeneracionNumeracionAvalonia.ViewModels;
using GeneracionNumeracionAvalonia.Views;

namespace GeneracionNumeracionAvalonia;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        var messagebox = new MessageBoxService();
        var mainViewModel = new MainWindowViewModel(messagebox);

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                DataContext = mainViewModel,
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}
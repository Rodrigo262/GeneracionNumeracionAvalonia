using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using GeneracionNumeracionAvalonia.Services;
using GeneracionNumeracionAvalonia.ViewModels;
using GeneracionNumeracionAvalonia.Views;
using Microsoft.Extensions.DependencyInjection;

namespace GeneracionNumeracionAvalonia;

public partial class App : Application
{
    public static IServiceProvider? ServiceProvider { get; set; }
    public static IClassicDesktopStyleApplicationLifetime? GeneradorApp { get; set; }
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }
    public override void RegisterServices()
    {
        base.RegisterServices();
        ConfigureDesktopServices();
    }

    public static void ConfigureDesktopServices()
    {
        IServiceCollection serviceCollection = new ServiceCollection();

        // Services
        serviceCollection.AddSingleton<IMessageBox, MessageBoxService>();
        serviceCollection.AddSingleton<ILogger, Logger>();
        ServiceProvider = serviceCollection.BuildServiceProvider();
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            GeneradorApp = desktop;
            desktop.MainWindow = new MainWindow
            {
                DataContext = new MainWindowViewModel(),
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}
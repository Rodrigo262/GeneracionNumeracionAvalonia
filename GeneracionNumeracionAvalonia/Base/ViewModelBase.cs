using CommunityToolkit.Mvvm.ComponentModel;
using GeneracionNumeracionAvalonia.Services;
using Microsoft.Extensions.DependencyInjection;

namespace GeneracionNumeracionAvalonia.Base;

public class ViewModelBase : ObservableObject
{
    public IMessageBox MessageBoxService;
    public ILogger LoggerService;

    public ViewModelBase()
    {
        MessageBoxService = App.ServiceProvider.GetService<IMessageBox>();
        LoggerService = App.ServiceProvider.GetService<ILogger>();
    }
}
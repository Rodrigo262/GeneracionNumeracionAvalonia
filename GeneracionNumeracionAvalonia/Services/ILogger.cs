using System;
using System.IO;
using Serilog;

namespace GeneracionNumeracionAvalonia.Services
{
    public interface ILogger
    {
        void LogInformation(string message);
        void LogError(Exception ex, string message);
    }

    public class Logger : ILogger
    {
        public Logger()
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .WriteTo.File("/Users/rodrigo/Documents/Rodrigo/Git/GeneracionNumeracionAvaloniaDesktop/myapp.txt", shared: false)
                .CreateLogger();
        }

        public void LogInformation(string message)
        {
            Log.Information(message);
        }

        public void LogError(Exception ex, string message)
        {
            Log.Error(ex, message);
        }
    }
}


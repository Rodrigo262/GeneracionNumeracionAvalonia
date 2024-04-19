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
            string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string appName = "GeneradorNumeracion";
            string logFileName = "GeneradorNumeracion.log";

            string logFilePath = Path.Combine(documentsPath, appName, logFileName);
            File.Delete(logFilePath);

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .WriteTo.File(logFilePath, shared: false)
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


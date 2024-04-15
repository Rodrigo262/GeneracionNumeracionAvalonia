using System.Threading.Tasks;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;

namespace GeneracionNumeracionAvalonia.Services
{
    public interface IMessageBox
    {
        Task<ButtonResult> GetMessageBoxStandard(string title, string message, ButtonEnum buttons, Icon icon);
    }

    public class MessageBoxService : IMessageBox
    {
        public async Task<ButtonResult> GetMessageBoxStandard(string title, string message, ButtonEnum buttons, Icon icon)
        {
            return await MessageBoxManager
               .GetMessageBoxStandard(
                   title,
                   message,
                   buttons,
                   icon).
               ShowAsync();
        }
    }
}
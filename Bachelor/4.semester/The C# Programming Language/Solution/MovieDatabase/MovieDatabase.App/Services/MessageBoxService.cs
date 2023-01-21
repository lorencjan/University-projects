using System.Windows;

namespace MovieDatabase.App.Services
{
    public class MessageBoxService
    {
        public MessageBoxResult Show(string messageBoxText, string caption, MessageBoxButton button, MessageBoxImage image)
        {
            return MessageBox.Show(messageBoxText, caption, button, image);
        }
    }
}

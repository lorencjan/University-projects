using System.Windows;
using System.Windows.Controls;
using BooksWpf.ViewModels;

namespace BooksWpf.Views
{
    public abstract class UserControlBase : UserControl
    {
        protected UserControlBase()
        {
            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is ViewModelBase viewModel)
            {
                viewModel.Load();
            }
        }
    }
}

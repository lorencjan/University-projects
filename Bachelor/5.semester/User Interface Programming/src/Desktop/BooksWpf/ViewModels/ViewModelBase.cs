using System.ComponentModel;
using System.Runtime.CompilerServices;
using BooksWpf.Services;
namespace BooksWpf.ViewModels
{
    public class ViewModelBase : INotifyPropertyChanged
    {
               protected readonly MessageBoxService _messageBoxService;

        public ViewModelBase()
        {
            _messageBoxService = new MessageBoxService();
        }

        public virtual void Load()
        {}


        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        { 
            if (Equals(storage, value)) 
                return false; 
            storage = value; 
            OnPropertyChanged(propertyName); 
            return true;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BooksWpf.Commands
{
    /// <summary>
    ///     https://johnthiriet.com/mvvm-going-async-with-async-command/
    /// </summary>
    public interface IAsyncCommand : ICommand
    {
        Task ExecuteAsync();
        bool CanExecute();
    }

    /// <summary>
    ///     https://johnthiriet.com/mvvm-going-async-with-async-command/
    /// </summary>
    public interface IAsyncCommand<T> : ICommand
    {
        Task ExecuteAsync(T parameter);
        bool CanExecute(T parameter);
    }
}
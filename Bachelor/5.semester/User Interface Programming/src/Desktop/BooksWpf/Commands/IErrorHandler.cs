using System;

namespace BooksWpf.Commands
{
    /// <summary>
    ///     https://johnthiriet.com/mvvm-going-async-with-async-command/
    /// </summary>
    public interface IErrorHandler
    {
        void HandleError(Exception ex);
    }
}
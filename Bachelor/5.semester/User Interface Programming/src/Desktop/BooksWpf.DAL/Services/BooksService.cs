using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using BooksService.Client;
using BooksService.Common;
using BooksWpf.DAL.Providers;

namespace BooksWpf.DAL.Services
{
    public class BookService
    {
        private readonly BooksClient _booksClient;

        public BookService() => _booksClient = new ClientProvider().GetBooksClient();


        public async Task<List<BookLight>> GetBooksLight(BookFilter filter = null, SortBooksBy? sortBy = null, SortOrdering? sortOrdering = null)
        {
            try
            {
                return await _booksClient.GetBooksLightAsync(filter?.Name, filter?.Year, filter?.Isbn, filter?.Author, filter?.Genre, sortBy, sortOrdering);
            }
            catch (Exception e)
            {
                Trace.WriteLine(e, "Failed to load books light via BookClient");
                return new List<BookLight>();
            }
        }

        public async Task<Book> GetById(int id)
        {
            try
            {
                return await _booksClient.GetByIdAsync(id, BookIncludeFeatures.All);
            }
            catch (Exception e)
            {
                Trace.WriteLine(e, "Failed to load book by id via BookClient");
                return new Book();
            }
        }

        public async Task<int> Create(Book book)
        {
            try
            {
                return await _booksClient.CreateAsync(book);
            }
            catch (Exception e)
            {
                Trace.WriteLine(e, "Failed to create book via BookClient");
                return 0;
            }
        }

        public async Task Update(Book book)
        {
            try
            {
                await _booksClient.UpdateAsync(book);
            }
            catch (Exception e)
            {
                Trace.WriteLine(e, "Failed to update book via BookClient");
            }
        }
        public async Task Delete(int id)
        {
            try
            {
                await _booksClient.DeleteAsync(id);
            }
            catch (Exception e)
            {
                Trace.WriteLine(e, "Failed to delete book via BookClient");
            }
        }

        public async Task DeleteRating(int id)
        {
            try
            {
                await _booksClient.DeleteRatingAsync(id);
            }
            catch (Exception e)
            {
                Trace.WriteLine(e, "Failed to delete book rating via BookClient");
            }
        }

        public async Task<List<Genre>> GetGenres()
        {
            try
            {
                return await _booksClient.GetGenresAsync();
            }
            catch (Exception e)
            {
                Trace.WriteLine(e, "Failed to delete book rating via BookClient");
                return new List<Genre>();
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BooksService.Client;
using BooksService.Common;
using BooksWeb.DAL.Attributes;
using Microsoft.Extensions.Logging;

namespace BooksWeb.DAL.Services
{
    [RegisterService]
    public class BooksService
    {
        private readonly IBooksClient _booksClient;
        private readonly ILogger<BooksService> _logger;

        public BooksService(IBooksClient booksClient, ILogger<BooksService> logger)
        {
            _booksClient = booksClient;
            _logger = logger;
        }

        public async Task<int> CreateRating(int number, string text, int userId, int bookId)
        {
            try
            {
                return await _booksClient.CreateRatingAsync(number, text, userId, bookId);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to create book rating via BookClient");
                return -1;
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
                _logger.LogError(e, "Failed to delete book rating via BookClient");
            }
        }

        public async Task<List<Book>> GetBooks(BookFilter filter = null, SortBooksBy? sortBy = null, SortOrdering? sortOrder = null, BookIncludeFeatures? includes = null)
        {
            try
            {
                return await _booksClient.GetBooksAsync(filter?.Name, filter?.Year, filter?.Isbn, filter?.Author, filter?.Genre, sortBy, sortOrder, includes);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to load books via BookClient");
                return new List<Book>();
            }
        }

        public async Task<List<BookLight>> GetBooksLight(BookFilter filter = null, SortBooksBy? sortBy = null, SortOrdering? sortOrdering = null)
        {
            try
            {
                return await _booksClient.GetBooksLightAsync(filter?.Name, filter?.Year, filter?.Isbn, filter?.Author, filter?.Genre, sortBy, sortOrdering);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to load books light via BookClient");
                return new List<BookLight>();
            }
        }

        public async Task<Book> GetById(int id, BookIncludeFeatures? includeFeatures = null)
        {
            try
            {
                return await _booksClient.GetByIdAsync(id, includeFeatures);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to load book by id via BookClient");
                return new Book();
            }
        }

        public async Task UpdateRating(int ratingId, int number, string text)
        {
            try
            {
                await _booksClient.UpdateRatingAsync(ratingId, number, text);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to update book rating via BookClient");
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
                _logger.LogError(e, "Failed to load genres via BookClient");
                return new List<Genre>();
            }
        }
    }
}
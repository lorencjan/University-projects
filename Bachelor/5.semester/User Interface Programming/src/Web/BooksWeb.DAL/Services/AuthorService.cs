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
    public class AuthorsService
    {
        private readonly IAuthorsClient _authorsClient;
        private readonly ILogger<AuthorsService> _logger;

        public AuthorsService(IAuthorsClient authorsClient, ILogger<AuthorsService> logger)
        {
            _authorsClient = authorsClient;
            _logger = logger;
        }

        public async Task<int> CreateRating(int number, string text, int userId, int authorId)
        {
            try
            {
                return await _authorsClient.CreateRatingAsync(number, text, userId, authorId);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to create author rating via AuthorClient");
                return -1;
            }
        }

        public async Task DeleteRating(int id)
        {
            try
            {
                await _authorsClient.DeleteRatingAsync(id);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to delete author rating via AuthorClient");
            }
        }

        public async Task<List<Author>> GetAuthors(AuthorFilter filter = null, SortAuthorsBy? sortBy = null, SortOrdering? sortOrder = null, AuthorIncludeFeatures? includes = null)
        {
            try
            {
                return await _authorsClient.GetAuthorsAsync(filter?.Name, filter?.Country, sortBy, sortOrder, includes);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to load authors via AuthorClient");
                return new List<Author>();
            }
        }

        public async Task<List<AuthorLight>> GetAuthorsLight(AuthorFilter filter = null, SortAuthorsBy? sortBy = null, SortOrdering? sortOrdering = null)
        {
            try
            {
                return await _authorsClient.GetAuthorsLightAsync(filter?.Name, filter?.Country, sortBy, sortOrdering);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to load authors light via AuthorClient");
                return new List<AuthorLight>();
            }
        }

        public async Task<Author> GetById(int id, AuthorIncludeFeatures? includeFeatures = null)
        {
            try
            {
                return await _authorsClient.GetByIdAsync(id, includeFeatures);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to load author by id via AuthorClient");
                return new Author();
            }
        }

        public async Task UpdateRating(int ratingId, int number, string text)
        {
            try
            {
                await _authorsClient.UpdateRatingAsync(ratingId, number, text);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to update author rating via AuthorsClient");
            }
        }
    }
}
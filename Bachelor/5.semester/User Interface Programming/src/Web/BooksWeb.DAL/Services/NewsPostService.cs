using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BooksService.Client;
using BooksWeb.DAL.Attributes;
using Microsoft.Extensions.Logging;

namespace BooksWeb.DAL.Services
{
    [RegisterService]
    public class NewsPostsService
    {
        private readonly INewsPostsClient _newsClient;
        private readonly ILogger<NewsPostsService> _logger;

        public NewsPostsService(INewsPostsClient newsClient, ILogger<NewsPostsService> logger)
        {
            _newsClient = newsClient;
            _logger = logger;
        }

        public async Task<NewsPost> GetById(int id)
        {
            try
            {
                return await _newsClient.GetByIdAsync(id);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to get newspost by id via NewsClient");
                return new NewsPost();
            }
        }

        public async Task<List<NewsPost>> GetNewsPosts(int? count = null)
        {
            try
            {
                return await _newsClient.GetNewsPostsAsync(count);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to get newsposts via NewsClient");
                return new List<NewsPost>();
            }
        }
    }
}
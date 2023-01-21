using System;
using System.Net.Http;
using BooksService.Client;

namespace BooksWpf.DAL.Providers
{
    public class ClientProvider
    {
        private readonly Uri _uri;

        public ClientProvider(string url = "http://localhost:12345/") => _uri = new Uri(url);
        
        public AuthorsClient GetAuthorsClient() => new AuthorsClient(new HttpClient { BaseAddress = _uri });
        public BooksClient GetBooksClient() => new BooksClient(new HttpClient { BaseAddress = _uri });
        public UsersClient GetUsersClient() => new UsersClient(new HttpClient { BaseAddress = _uri });
        public NewsPostsClient GetNewsPostsClient() => new NewsPostsClient(new HttpClient { BaseAddress = _uri });
    }
}

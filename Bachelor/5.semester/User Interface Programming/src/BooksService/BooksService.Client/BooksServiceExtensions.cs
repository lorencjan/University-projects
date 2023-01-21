using System;
using Microsoft.Extensions.DependencyInjection;
using Polly;

namespace BooksService.Client
{
    public static class BooksServiceExtensions
    {
        /// <summary>Registers BooksService clients to the service collection</summary>
        /// <param name="serviceCollection">DI service collection</param>
        /// <param name="uri">Uri where the service runs</param>
        public static void AddBooksService(this IServiceCollection serviceCollection, string uri)
        {
            serviceCollection.AddHttpClient<IAuthorsClient, AuthorsClient>(client => client.BaseAddress = new Uri(uri))
                .AddTransientHttpErrorPolicy(builder => builder.WaitAndRetryAsync(3, i => TimeSpan.FromMilliseconds(i * 500)));
            serviceCollection.AddHttpClient<IBooksClient, BooksClient>(client => client.BaseAddress = new Uri(uri))
                .AddTransientHttpErrorPolicy(builder => builder.WaitAndRetryAsync(3, i => TimeSpan.FromMilliseconds(i * 500)));
            serviceCollection.AddHttpClient<INewsPostsClient, NewsPostsClient>(client => client.BaseAddress = new Uri(uri))
                .AddTransientHttpErrorPolicy(builder => builder.WaitAndRetryAsync(3, i => TimeSpan.FromMilliseconds(i * 500)));
            serviceCollection.AddHttpClient<IUsersClient, UsersClient>(client => client.BaseAddress = new Uri(uri))
                .AddTransientHttpErrorPolicy(builder => builder.WaitAndRetryAsync(3, i => TimeSpan.FromMilliseconds(i * 500)));
        }
    }
}

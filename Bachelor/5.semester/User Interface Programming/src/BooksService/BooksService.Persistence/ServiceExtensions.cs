using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BooksService.Persistence
{
    public static class ServiceExtensions
    {
        public static void AddBooksDb(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<BooksDbContext>(opts =>
                opts.UseSqlServer(configuration.GetConnectionString("BooksDbContext")));
        }
    }
}
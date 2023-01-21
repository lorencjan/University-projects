using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace RockFests.DAL.StaticServices
{
    public static class Extensions
    {
        public static void AddRockFestsDb(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<RockFestsDbContext>(opts =>
                opts.UseSqlServer(configuration.GetConnectionString("RockFestsDb")));
        }
    }
}

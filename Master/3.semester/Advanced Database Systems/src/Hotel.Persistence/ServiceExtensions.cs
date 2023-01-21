using Hotel.Command.Persistence.Sql;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Hotel.Command.Persistence
{
    public static class ServiceExtensions
    {
        public static void AddHotelDb(this IServiceCollection services, IConfiguration configuration) =>
            services.AddDbContext<HotelContext>(x => x.UseSqlServer(configuration.GetConnectionString("HotelContext")));

        public static void AddHotelMigrationStartupFilter(this IServiceCollection services) {
            services.AddTransient<IStartupFilter, HotelMigrationStartupFilter>();
        }
    }
}
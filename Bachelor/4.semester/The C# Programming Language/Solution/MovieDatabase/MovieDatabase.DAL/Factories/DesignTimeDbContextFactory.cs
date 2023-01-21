using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace MovieDatabase.DAL.Factories
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<MovieDatabaseDbContext>
    {
        /// <summary>Connects to the database with conneciton string from appsettings.json</summary>
        public MovieDatabaseDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<MovieDatabaseDbContext>();
            string pathToSln = Directory.GetParent(Directory.GetCurrentDirectory()).FullName;
            string pathToAppSettings = pathToSln + "\\MovieDatabase.DAL";
            var config = new ConfigurationBuilder()
                .SetBasePath(pathToAppSettings)
                .AddJsonFile("appsettings.json")
                .Build();

            builder.UseSqlServer(config.GetConnectionString("DbConnection"));

            return new MovieDatabaseDbContext(builder.Options);
        }
    }
}

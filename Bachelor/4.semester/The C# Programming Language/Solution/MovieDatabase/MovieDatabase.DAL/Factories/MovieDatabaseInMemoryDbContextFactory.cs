using Microsoft.EntityFrameworkCore;

namespace MovieDatabase.DAL.Factories
{
    public class MovieDatabaseInMemoryDbContextFactory : IDbContextFactory
    {
        public DbContextOptions<MovieDatabaseDbContext> CreateDbContextOptions()
        {
            var contextOptionsBuilder = new DbContextOptionsBuilder<MovieDatabaseDbContext>();
            contextOptionsBuilder.UseInMemoryDatabase("MovieDatabase");
            return contextOptionsBuilder.Options;
        }

        public MovieDatabaseDbContext CreateDbContext()
        {
           return new MovieDatabaseDbContext(CreateDbContextOptions());           
        }
    }
}

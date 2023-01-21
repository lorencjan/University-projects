using System;
using MovieDatabase.DAL.Factories;

namespace MovieDatabase.DAL.Tests
{
    public class MovieDatabaseDbContextTestsFixture : IDisposable
    {
        public MovieDatabaseDbContext MovieDatabaseDbContextSUT { get; set; }

        public MovieDatabaseDbContextTestsFixture()
        {
            MovieDatabaseDbContextSUT = CreateMovieDatabaseDbContext();
        }

        public MovieDatabaseDbContext CreateMovieDatabaseDbContext()
        {
            return new MovieDatabaseInMemoryDbContextFactory().CreateDbContext();
        }
        
        public void Dispose()
        {
            MovieDatabaseDbContextSUT?.Dispose();
        }
    }
}
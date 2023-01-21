namespace MovieDatabase.DAL.Factories
{
    public class MovieDatabaseDbContextFactory : IDbContextFactory
    {
        public MovieDatabaseDbContext CreateDbContext()
        {
            return new MovieDatabaseDbContext();
        }
    }
}

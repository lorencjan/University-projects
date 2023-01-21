namespace MovieDatabase.DAL.Factories
{
    public interface IDbContextFactory
    {
        MovieDatabaseDbContext CreateDbContext();
    }
}

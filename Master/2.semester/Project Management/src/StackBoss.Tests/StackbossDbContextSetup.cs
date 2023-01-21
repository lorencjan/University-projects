using StackBoss.Web.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StackBoss.Tests
{
    public class StackbossDbContextSetup
    {
        public InMemoryDbContextFactory DbContextFactory { get; }

        public StackbossDbContextSetup(string testDbName) => DbContextFactory = new InMemoryDbContextFactory(testDbName);

        public void PrepareDatabase()
        {
            using var dbx = DbContextFactory.CreateDbContext();
            dbx.Database.EnsureCreated();
        }

        public void TearDownDatabase()
        {
            using var dbx = DbContextFactory.CreateDbContext();
            dbx.Database.EnsureDeleted();
        }

        public void Dispose()
        {
            TearDownDatabase();
        }
    }
}

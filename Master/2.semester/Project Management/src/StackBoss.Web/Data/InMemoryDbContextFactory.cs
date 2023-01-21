using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StackBoss.Web.Data
{
    public class InMemoryDbContextFactory
    {
         private readonly string _testDbName;
        public InMemoryDbContextFactory(string testDbName) => _testDbName = testDbName;

        public ApplicationDbContext CreateDbContext()
        {
            var dbContextOptionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            dbContextOptionsBuilder.UseInMemoryDatabase(_testDbName);
            dbContextOptionsBuilder.EnableSensitiveDataLogging();
            return new ApplicationDbContext(dbContextOptionsBuilder.Options);
        }
    }
}

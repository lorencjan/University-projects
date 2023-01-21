using System;
using BooksService.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using NUnit.Framework;

namespace BooksService.Specification
{
    public abstract class TestFixture
    {
        public IServiceProvider ServiceProvider { get; set; }

        [OneTimeSetUp]
        public void BaseSetup()
        {
            ServiceProvider = Host.CreateDefaultBuilder()
                .ConfigureServices((context, collection) => {
                    new Startup(context.Configuration).ConfigureServices(collection);
                    collection.Replace(ServiceDescriptor.Transient(provider => {
                        var dbContextOptions = new DbContextOptionsBuilder<BooksDbContext>();
                        dbContextOptions.UseInMemoryDatabase("InMemory database");
                        return new BooksDbContext(dbContextOptions.Options);
                    }));
                })
                .Build()
                .Services;
        }

        [SetUp]
        public void Setup() => ClearInMemoryDatabase();

        protected void ClearInMemoryDatabase()
        {
            var context = ServiceProvider.GetRequiredService<BooksDbContext>();
            context.Database.EnsureDeleted();
        }

        protected IMediator GetMediator() => ServiceProvider.GetRequiredService<IMediator>();
        protected BooksDbContext GetContext() => ServiceProvider.GetRequiredService<BooksDbContext>();
    }
}

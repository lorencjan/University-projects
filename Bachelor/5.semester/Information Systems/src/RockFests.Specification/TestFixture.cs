using System;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using NUnit.Framework;
using RockFests.BL.Mapping;
using RockFests.DAL;

namespace RockFests.Specification
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
                        var dbContextOptions = new DbContextOptionsBuilder<RockFestsDbContext>();
                        dbContextOptions.UseInMemoryDatabase("InMemory database");
                        return new RockFestsDbContext(dbContextOptions.Options);
                    }));
                })
                .Build()
                .Services;

            Mapper.Reset();
        }

        [SetUp]
        public void Setup()
        {
            ClearInMemoryDatabase();
            MapperConfig.SetMapper();
        }

        [TearDown]
        public void TearDown() => Mapper.Reset();

        protected void ClearInMemoryDatabase()
        {
            var context = ServiceProvider.GetRequiredService<RockFestsDbContext>();
            context.Database.EnsureDeleted();
        }

        protected RockFestsDbContext GetContext() => ServiceProvider.GetRequiredService<RockFestsDbContext>();
    }
}
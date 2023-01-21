using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using RockFests.BL.Model;
using RockFests.BL.Repositories;
using RockFests.DAL.Entities;
using RockFests.DAL.Types;

namespace RockFests.Specification.RepositoryTests
{
    public class FestivalRepositoryTests : TestFixture
    {
        private FestivalRepository _festivalRepository;

        private List<Festival> Festivals() => new List<Festival>
        {
            new Festival
            {
                Id = 1,
                Name = "Test1",
                StartDate = DateTime.Today.AddDays(1),
                Stages = new List<Stage>(),
                Tickets = new List<Ticket>()
            },
            new Festival
            {
                Id = 2,
                Name = "Test2",
                StartDate = DateTime.Today.AddDays(2),
                Stages = new List<Stage>(),
                Tickets = new List<Ticket>()
            },
            new Festival
            {
                Id = 3,
                Name = "Test3",
                StartDate = DateTime.Today.AddDays(3),
                Stages = new List<Stage>(),
                Tickets = new List<Ticket>()
            }
        };

        [SetUp]
        public void FestivalSetup() => _festivalRepository = ServiceProvider.GetRequiredService<FestivalRepository>();

        [Test]
        public async Task Successfully_create_festival()
        {
            var id = await _festivalRepository.Add(new FestivalDto{Name = "Test"});

            var festivals = await GetContext().Festivals.ToListAsync();

            id.Should().Be(1);
            festivals.Should().HaveCount(1);
            festivals.Single().Should().BeEquivalentTo(new Festival
            {
                Id = 1,
                Name = "Test"
            });
        }

        [Test]
        public async Task Successfully_update_festival()
        {
            var dbContext = GetContext();
            await dbContext.Festivals.AddAsync(new Festival{ Name = "Test"});
            await dbContext.SaveChangesAsync();

            var festivalDto = new FestivalDto { Id = 1, Name = "NewValue" };
            await _festivalRepository.Update(festivalDto);

            var festival = await GetContext().Festivals.SingleAsync(x => x.Id == 1);
            festival.Should().BeEquivalentTo(new Festival
            {
                Id = 1,
                Name = "NewValue"
            });
        }

        [Test]
        public async Task Get_existing_festival()
        {
            var dbContext = GetContext();
            await dbContext.Festivals.AddAsync(Festivals().First());
            await dbContext.SaveChangesAsync();

            var festivalDto = await _festivalRepository.GetById(1);

            festivalDto.Should().BeEquivalentTo(Mapper.Map<FestivalDto>(Festivals().First()));
        }

        [Test]
        public async Task Get_not_existing_should_return_null()
        {
            var festivalDto = await _festivalRepository.GetById(1);

            festivalDto.Should().Be(null);
        }

        [Test]
        public async Task Get_all_festivals()
        {
            var festivalDtos = Festivals().AsQueryable().ProjectTo<FestivalDto>().ToList();
            var dbContext = GetContext();
            await dbContext.Festivals.AddRangeAsync(Festivals());
            await dbContext.SaveChangesAsync();

            var allFestivals = await _festivalRepository.GetAll();

            allFestivals.Should().HaveCount(3);
            allFestivals.ForEach(x => festivalDtos.Should().ContainEquivalentOf(x));
        }

        [Test]
        public async Task Get_all_festivals_by_filter()
        {
            var festivalDtos = Festivals().AsQueryable().ProjectTo<FestivalDto>().ToList();
            var dbContext = GetContext();
            await dbContext.Festivals.AddRangeAsync(Festivals());
            await dbContext.SaveChangesAsync();

            var filteredFestivals = await _festivalRepository.GetAll("test1");

            filteredFestivals.Should().HaveCount(1);
            filteredFestivals.Should().BeEquivalentTo(festivalDtos.First());
        }

        [Test]
        public async Task Get_all_festivals_light()
        {
            var festivalLightDtos = Festivals().AsQueryable().ProjectTo<FestivalLightDto>().ToList();
            var dbContext = GetContext();
            await dbContext.Festivals.AddRangeAsync(Festivals());
            await dbContext.SaveChangesAsync();

            var allFestivals = await _festivalRepository.GetAllLight();

            allFestivals.Should().HaveCount(3);
            allFestivals.ForEach(x => festivalLightDtos.Should().ContainEquivalentOf(x));
        }

        [Test]
        public async Task Get_all_festivals_light_past_included()
        {
            var festivalLightDtos = Festivals().AsQueryable().ProjectTo<FestivalLightDto>().ToList();
            festivalLightDtos.Add(new FestivalLightDto{Id = 4, Date = new DateTimeInterval(DateTime.Today.AddDays(-2), DateTime.Today.AddDays(-1)) });
            var dbContext = GetContext();
            await dbContext.Festivals.AddRangeAsync(Festivals());
            await dbContext.Festivals.AddAsync(new Festival{Id = 4, StartDate = DateTime.Today.AddDays(-2), EndDate = DateTime.Today.AddDays(-1)});
            await dbContext.SaveChangesAsync();

            var allFestivals = await _festivalRepository.GetAllLight(includePast: true);

            allFestivals.Should().HaveCount(4);
            allFestivals.ForEach(x => festivalLightDtos.Should().ContainEquivalentOf(x));
        }

        [Test]
        public async Task Get_all_festivals_light_by_filter()
        {
            var festivalLightDtos = Festivals().AsQueryable().ProjectTo<FestivalLightDto>().ToList();
            var dbContext = GetContext();
            await dbContext.Festivals.AddRangeAsync(Festivals());
            await dbContext.SaveChangesAsync();

            var filteredFestivals = await _festivalRepository.GetAllLight("st2");

            filteredFestivals.Should().HaveCount(1);
            filteredFestivals.Should().BeEquivalentTo(festivalLightDtos[1]);
        }

        [Test]
        public async Task Get_closest_festival()
        {
            var dbContext = GetContext();
            await dbContext.Festivals.AddRangeAsync(Festivals());
            await dbContext.SaveChangesAsync();

            var closestFestival = await _festivalRepository.GetClosestFestival();

            closestFestival.Should().BeEquivalentTo(Mapper.Map<FestivalDto>(Festivals().First()));
        }

        [Test]
        public async Task Get_closest_users_festival()
        {
            var dbContext = GetContext();
            await dbContext.Festivals.AddRangeAsync(Festivals());
            await dbContext.Users.AddAsync(new User());
            await dbContext.Tickets.AddRangeAsync(new Ticket{UserId = 1, FestivalId = 2}, new Ticket{UserId = 1, FestivalId = 3});
            await dbContext.SaveChangesAsync();

            var closestUsersFestival = await _festivalRepository.GetClosestUsersFestival(1);

            closestUsersFestival.Should().BeEquivalentTo(Mapper.Map<FestivalDto>(Festivals()[1]));
        }

        [Test]
        public async Task Successfully_delete_festival()
        {
            var dbContext = GetContext();
            var festival = await dbContext.Festivals.AddAsync(new Festival{Name = "Test"});
            await dbContext.SaveChangesAsync();

            (await dbContext.Festivals.CountAsync()).Should().Be(1);
            await _festivalRepository.Delete(festival.Entity.Id);

            (await dbContext.Festivals.CountAsync()).Should().Be(0);
        }

        [Test]
        public void Delete_not_existing_should_do_nothing()
            => Assert.DoesNotThrowAsync(async () => await _festivalRepository.Delete(5));
    }
}

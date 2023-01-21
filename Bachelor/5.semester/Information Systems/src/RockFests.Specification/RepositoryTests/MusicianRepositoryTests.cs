using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper.QueryableExtensions;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using RockFests.BL.Model;
using RockFests.BL.Repositories;
using RockFests.DAL.Entities;
using RockFests.DAL.Seeds;

namespace RockFests.Specification.RepositoryTests
{
    public class MusicianRepositoryTests : TestFixture
    {
        private MusicianRepository _musicianRepository;

        private List<Musician> Musicians() => new List<Musician>
        {
            new Musician
            {
                Id = 1,
                FirstName = "Test1",
                LastName = "Testovic1",
                Performances = new List<Performance>(),
                Bands = new List<BandMusician>(),
                Ratings = new List<MusicianRating> {new MusicianRating
                {
                    Id = 1,
                    MusicianId = 1,
                    Musician = new Musician{Id = 1, FirstName = "Test1", LastName = "Testovic1" },
                    Number = 8
                }}
            },
            new Musician
            {
                Id = 2,
                FirstName = "Test2",
                LastName = "Testovic2",
                Performances = new List<Performance>(),
                Bands = new List<BandMusician>(),
                Ratings = new List<MusicianRating>
                {
                    new MusicianRating{Id = 2,MusicianId = 2, Number = 8, Musician = new Musician{Id = 2, FirstName = "Test2", LastName = "Testovic2" },},
                    new MusicianRating{Id = 3,MusicianId = 2, Number = 9, Musician = new Musician{Id = 2, FirstName = "Test2", LastName = "Testovic2" },}
                }
            },
            new Musician
            {
                Id = 3,
                FirstName = "Test3",
                LastName = "Testovic3",
                Performances = new List<Performance>(),
                Bands = new List<BandMusician>(),
                Ratings = new List<MusicianRating>()
            }
        };

        [SetUp]
        public void MusicianSetup() => _musicianRepository = ServiceProvider.GetRequiredService<MusicianRepository>();

        [Test]
        public async Task Successfully_create_musician()
        {
            var id = await _musicianRepository.Add(new MusicianDto{FirstName = "Test"});

            var musicians = await GetContext().Musicians.ToListAsync();

            id.Should().Be(1);
            musicians.Should().HaveCount(1);
            musicians.Single().Should().BeEquivalentTo(new Musician
            {
                Id = 1,
                FirstName = "Test",
                Photo = new byte[0]
            });
        }

        [Test]
        public async Task Successfully_update_musician()
        {
            var dbContext = GetContext();
            await dbContext.Musicians.AddAsync(new Musician{ FirstName = "Test"});
            await dbContext.SaveChangesAsync();

            var musicianDto = new MusicianDto { Id = 1, FirstName = "NewValue" };
            await _musicianRepository.Update(musicianDto);

            var musician = await GetContext().Musicians.SingleAsync(x => x.Id == 1);
            musician.Should().BeEquivalentTo(new Musician
            {
                Id = 1,
                FirstName = "NewValue",
                Photo = new byte[0]
            });
        }

        [Test]
        public async Task Get_existing_musician()
        {
            var dbContext = GetContext();
            await dbContext.Musicians.AddAsync(new Musician
            {
                FirstName = "Test",
                Photo = SeedImages.bruce_dickinson
            });
            await dbContext.SaveChangesAsync();

            var musicianDto = await _musicianRepository.GetById(1);

            musicianDto.Should().BeEquivalentTo(new MusicianDto
            {
                Id = 1,
                FirstName = "Test",
                Photo = SeedImages.bruce_dickinson,
                Performances = new List<PerformanceDto>(),
                Bands = new List<BandLightDto>(),
                Ratings = new List<RatingDto>(),
            });
        }

        [Test]
        public async Task Get_not_existing_should_return_null()
        {
            var musicianDto = await _musicianRepository.GetById(1);

            musicianDto.Should().Be(null);
        }

        [Test]
        public async Task Get_all_musicians()
        {
            var musiciansDto = Musicians().AsQueryable().ProjectTo<MusicianDto>().ToList();
            var dbContext = GetContext();
            await dbContext.Musicians.AddRangeAsync(Musicians());
            await dbContext.SaveChangesAsync();

            var allMusicians = await _musicianRepository.GetAll();

            allMusicians.Should().HaveCount(3);
            allMusicians.ForEach(x => musiciansDto.Should().ContainEquivalentOf(x));
        }

        [Test]
        public async Task Get_all_musicians_by_filter()
        {
            var musicianDtos = Musicians().AsQueryable().ProjectTo<MusicianDto>().ToList();
            var dbContext = GetContext();
            await dbContext.Musicians.AddRangeAsync(Musicians());
            await dbContext.SaveChangesAsync();

            var filteredMusicians = await _musicianRepository.GetAll("test1");

            filteredMusicians.Should().HaveCount(1);
            filteredMusicians.Should().BeEquivalentTo(musicianDtos.First());
        }

        [Test]
        public async Task Get_all_musicians_light()
        {
            var musicianLightDtos = Musicians().AsQueryable().ProjectTo<MusicianLightDto>().ToList();
            var dbContext = GetContext();
            await dbContext.Musicians.AddRangeAsync(Musicians());
            await dbContext.SaveChangesAsync();

            var allMusicians = await _musicianRepository.GetAllLight();

            allMusicians.Should().HaveCount(3);
            allMusicians.ForEach(x => musicianLightDtos.Should().ContainEquivalentOf(x));
        }

        [Test]
        public async Task Get_all_musicians_light_by_filter()
        {
            var musicianLightDtos = Musicians().AsQueryable().ProjectTo<MusicianLightDto>().ToList();
            var dbContext = GetContext();
            await dbContext.Musicians.AddRangeAsync(Musicians());
            await dbContext.SaveChangesAsync();

            var filteredMusicians = await _musicianRepository.GetAllLight("ovic2");

            filteredMusicians.Should().HaveCount(1);
            filteredMusicians.Should().BeEquivalentTo(musicianLightDtos[1]);
        }

        [Test]
        public async Task Successfully_delete_musician()
        {
            var dbContext = GetContext();
            var musician = await dbContext.Musicians.AddAsync(new Musician{FirstName = "Test"});
            await dbContext.SaveChangesAsync();

            (await dbContext.Musicians.CountAsync()).Should().Be(1);
            await _musicianRepository.Delete(musician.Entity.Id);

            (await dbContext.Musicians.CountAsync()).Should().Be(0);
        }

        [Test]
        public void Delete_not_existing_should_do_nothing()
            => Assert.DoesNotThrowAsync(async () => await _musicianRepository.Delete(5));
    }
}

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
    public class BandRepositoryTests : TestFixture
    {
        private BandRepository _bandRepository;

        private List<Band> Bands() => new List<Band>
        {
            new Band
            {
                Id = 1,
                Name = "Test1",
                Performances = new List<Performance>(),
                Members = new List<BandMusician>(),
                Ratings = new List<BandRating> {new BandRating{Id = 1, BandId = 1, Number = 5, Band = new Band{Id = 1, Name = "Test1"}}}
            },
            new Band
            {
                Id = 2,
                Name = "Test2",
                Performances = new List<Performance>(),
                Members = new List<BandMusician>(),
                Ratings = new List<BandRating>
                {
                    new BandRating{Id = 2,BandId = 2, Number = 5, Band = new Band{Id = 1, Name = "Test2"}},
                    new BandRating{Id = 3,BandId = 2, Number = 10, Band = new Band{Id = 1, Name = "Test2"}}
                }
            },
            new Band
            {
                Id = 3,
                Name = "Test3",
                Performances = new List<Performance>(),
                Members = new List<BandMusician>(),
                Ratings = new List<BandRating>()
            }
        };

        [SetUp]
        public void BandSetup() => _bandRepository = ServiceProvider.GetRequiredService<BandRepository>();

        [Test]
        public async Task Successfully_create_band()
        {
            var id = await _bandRepository.Add(new BandDto{Name = "Test"});

            var bands = await GetContext().Bands.ToListAsync();

            id.Should().Be(1);
            bands.Should().HaveCount(1);
            bands.Single().Should().BeEquivalentTo(new Band
            {
                Id = 1,
                Name = "Test",
                Photo = new byte[0]
            });
        }

        [Test]
        public async Task Successfully_update_band()
        {
            var dbContext = GetContext();
            await dbContext.Bands.AddAsync(new Band{ Name = "Test"});
            await dbContext.SaveChangesAsync();

            var bandDto = new BandDto { Id = 1, Name = "NewValue" };
            await _bandRepository.Update(bandDto);

            var band = await GetContext().Bands.SingleAsync(x => x.Id == 1);
            band.Should().BeEquivalentTo(new Band
            {
                Id = 1,
                Name = "NewValue",
                Photo = new byte[0]
            });
        }

        [Test]
        public async Task Get_existing_band()
        {
            var dbContext = GetContext();
            await dbContext.Bands.AddAsync(new Band
            {
                Name = "Test",
                Photo = SeedImages.metallica_logo
            });
            await dbContext.SaveChangesAsync();

            var bandDto = await _bandRepository.GetById(1);

            bandDto.Should().BeEquivalentTo(new BandDto
            {
                Id = 1,
                Name = "Test",
                Photo = SeedImages.metallica_logo,
                Performances = new List<PerformanceDto>(),
                Members = new List<MusicianLightDto>(),
                Ratings = new List<RatingDto>(),
            });
        }

        [Test]
        public async Task Get_not_existing_should_return_null()
        {
            var bandDto = await _bandRepository.GetById(1);

            bandDto.Should().Be(null);
        }

        [Test]
        public async Task Get_all_bands()
        {
            var bandDtos = Bands().AsQueryable().ProjectTo<BandDto>().ToList();
            var dbContext = GetContext();
            await dbContext.Bands.AddRangeAsync(Bands());
            await dbContext.SaveChangesAsync();

            var allBands = await _bandRepository.GetAll();

            allBands.Should().HaveCount(3);
            allBands.ForEach(x => bandDtos.Should().ContainEquivalentOf(x));
        }

        [Test]
        public async Task Get_all_bands_by_filter()
        {
            var bandDtos = Bands().AsQueryable().ProjectTo<BandDto>().ToList();
            var dbContext = GetContext();
            await dbContext.Bands.AddRangeAsync(Bands());
            await dbContext.SaveChangesAsync();

            var filteredBands = await _bandRepository.GetAll("test1");

            filteredBands.Should().HaveCount(1);
            filteredBands.Should().BeEquivalentTo(bandDtos.First());
        }

        [Test]
        public async Task Get_all_bands_light()
        {
            var bandLightDtos = Bands().AsQueryable().ProjectTo<BandLightDto>().ToList();
            var dbContext = GetContext();
            await dbContext.Bands.AddRangeAsync(Bands());
            await dbContext.SaveChangesAsync();

            var allBands = await _bandRepository.GetAllLight();

            allBands.Should().HaveCount(3);
            allBands.ForEach(x => bandLightDtos.Should().ContainEquivalentOf(x));
        }

        [Test]
        public async Task Get_all_bands_light_by_filter()
        {
            var bandLightDtos = Bands().AsQueryable().ProjectTo<BandLightDto>().ToList();
            var dbContext = GetContext();
            await dbContext.Bands.AddRangeAsync(Bands());
            await dbContext.SaveChangesAsync();

            var filteredBands = await _bandRepository.GetAllLight("st2");

            filteredBands.Should().HaveCount(1);
            filteredBands.Should().BeEquivalentTo(bandLightDtos[1]);
        }

        [Test]
        public async Task Successfully_delete_band()
        {
            var dbContext = GetContext();
            var band = await dbContext.Bands.AddAsync(new Band{Name = "Test"});
            await dbContext.SaveChangesAsync();

            (await dbContext.Bands.CountAsync()).Should().Be(1);
            await _bandRepository.Delete(band.Entity.Id);

            (await dbContext.Bands.CountAsync()).Should().Be(0);
        }

        [Test]
        public void Delete_not_existing_should_do_nothing()
            => Assert.DoesNotThrowAsync(async () => await _bandRepository.Delete(5));

        [Test]
        public async Task Add_musician_to_band()
        {
            var dbContext = GetContext();
            var bandId = (await dbContext.Bands.AddAsync(new Band { Name = "Test" })).Entity.Id;
            var musicianId = (await dbContext.Musicians.AddAsync(new Musician { FirstName = "Test", LastName = "Testovic"})).Entity.Id;
            await dbContext.SaveChangesAsync();

            await _bandRepository.AddMusician(bandId, musicianId);

            var bandMusician = await GetContext().BandMusicians.SingleAsync();
            bandMusician.BandId.Should().Be(bandId);
            bandMusician.MusicianId.Should().Be(musicianId);
        }

        [Test]
        public async Task Add_multiple_musicians_to_band()
        {
            var dbContext = GetContext();
            var bandId = (await dbContext.Bands.AddAsync(new Band { Name = "Test" })).Entity.Id;
            var musicianId1 = (await dbContext.Musicians.AddAsync(new Musician { FirstName = "Test1", LastName = "Testovic1" })).Entity.Id;
            var musicianId2 = (await dbContext.Musicians.AddAsync(new Musician { FirstName = "Test2", LastName = "Testovic2" })).Entity.Id;
            var musicianId3 = (await dbContext.Musicians.AddAsync(new Musician { FirstName = "Test3", LastName = "Testovic3" })).Entity.Id;
            await dbContext.SaveChangesAsync();

            await _bandRepository.AddMusicians(bandId, new []{ musicianId1, musicianId2, musicianId3  });

            var bandMusicians = await GetContext().BandMusicians.ToListAsync();
            bandMusicians.Should().HaveCount(3);
            bandMusicians.Should().ContainEquivalentOf(new BandMusician{BandId = bandId, MusicianId = musicianId1});
            bandMusicians.Should().ContainEquivalentOf(new BandMusician{BandId = bandId, MusicianId = musicianId2});
            bandMusicians.Should().ContainEquivalentOf(new BandMusician{BandId = bandId, MusicianId = musicianId3});
        }

        [Test]
        public async Task Remove_musician_from_band()
        {
            var dbContext = GetContext();
            await dbContext.BandMusicians.AddAsync(new BandMusician{BandId = 1, MusicianId = 1});
            await dbContext.SaveChangesAsync();

            (await dbContext.BandMusicians.CountAsync()).Should().Be(1);
            await _bandRepository.DeleteMusician(1, 1);

            (await dbContext.BandMusicians.CountAsync()).Should().Be(0);
        }

        [Test]
        public void Removing_not_existing_musician_from_band()
            => Assert.DoesNotThrowAsync(async () => await _bandRepository.DeleteMusician(1,3));
    }
}

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

namespace RockFests.Specification.RepositoryTests
{
    public class BandRatingRepositoryTests : TestFixture
    {
        private BandRatingRepository _bandRatingRepository;

        private List<BandRating> BandRatings() => new List<BandRating>
        {
            new BandRating
            {
                Id = 1,
                BandId = 1,
                Number = 5,
                Text = "Test rating 1",
                Band = new Band{Name = "Test"},
                UserName = "User"
            },
            new BandRating
            {
                Id = 2,
                BandId = 1,
                Number = 7,
                Text = "Test rating 2",
                UserName = "User 2"
            },
            new BandRating
            {
                Id = 3,
                BandId = 1,
                Number = 8,
                Text = "Test rating 3",
                UserName = "User 3"
            }
        };

        [SetUp]
        public void BandRatingSetup() => _bandRatingRepository = ServiceProvider.GetRequiredService<BandRatingRepository>();

        [Test]
        public async Task Successfully_create_band_rating()
        {
            var id = await _bandRatingRepository.Add(new RatingDto{Text = "Test"});

            var ratings = await GetContext().BandRatings.ToListAsync();

            id.Should().Be(1);
            ratings.Should().HaveCount(1);
            ratings.Single().Should().BeEquivalentTo(new BandRating
            {
                Id = 1,
                Text = "Test"
            });
        }

        [Test]
        public async Task Successfully_update_band_rating()
        {
            var dbContext = GetContext();
            await dbContext.BandRatings.AddAsync(new BandRating{ Text = "Test"});
            await dbContext.SaveChangesAsync();

            var ratingDto = new RatingDto { Id = 1, Text = "NewValue" };
            await _bandRatingRepository.Update(ratingDto);

            var bandRating = await GetContext().BandRatings.SingleAsync(x => x.Id == 1);
            bandRating.Should().BeEquivalentTo(new BandRating
            {
                Id = 1,
                Text = "NewValue"
            });
        }

        [Test]
        public async Task Get_existing_band_rating()
        {
            var dbContext = GetContext();
            await dbContext.BandRatings.AddAsync(new BandRating
            {
                Text = "Test", 
                BandId = 1, 
                Band = new Band{Name = "Test Maiden"}
            });
            await dbContext.SaveChangesAsync();

            var rating = await _bandRatingRepository.GetById(1);

            rating.Should().BeEquivalentTo(new RatingDto
            {
                Id = 1,
                Text = "Test",
                InterpretId = 1,
                InterpretName = "Test Maiden"
            });
        }

        [Test]
        public async Task Get_not_existing_should_return_null()
        {
            var ratingDto = await _bandRatingRepository.GetById(1);

            ratingDto.Should().Be(null);
        }

        [Test]
        public async Task Get_all_band_ratings()
        {
            var ratingDtos = BandRatings().AsQueryable().ProjectTo<RatingDto>().ToList();
            ratingDtos.ForEach(x => x.InterpretName = "Test");
            var dbContext = GetContext();
            await dbContext.BandRatings.AddRangeAsync(BandRatings());
            await dbContext.SaveChangesAsync();

            var allRatings = await _bandRatingRepository.GetAll();

            allRatings.Should().HaveCount(3);
            allRatings.ForEach(x => ratingDtos.Should().ContainEquivalentOf(x));
        }

        [Test]
        public async Task Successfully_delete_band_rating()
        {
            var dbContext = GetContext();
            var bandRating = await dbContext.BandRatings.AddAsync(new BandRating{Text = "Test"});
            await dbContext.SaveChangesAsync();

            (await dbContext.BandRatings.CountAsync()).Should().Be(1);
            await _bandRatingRepository.Delete(bandRating.Entity.Id);

            (await dbContext.BandRatings.CountAsync()).Should().Be(0);
        }

        [Test]
        public void Delete_not_existing_should_do_nothing()
            => Assert.DoesNotThrowAsync(async () => await _bandRatingRepository.Delete(5));
    }
}

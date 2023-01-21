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
    public class MusicianRatingRepositoryTests : TestFixture
    {
        private MusicianRatingRepository _musicianRatingRepository;

        private List<MusicianRating> MusicianRatings() => new List<MusicianRating>
        {
            new MusicianRating
            {
                Id = 1,
                MusicianId = 1,
                Number = 5,
                Text = "Test rating 1",
                Musician = new Musician{FirstName = "Test", LastName = "Testovic"},
                UserName = "User"
            },
            new MusicianRating
            {
                Id = 2,
                MusicianId = 1,
                Number = 7,
                Text = "Test rating 2",
                UserName = "User 2"
            },
            new MusicianRating
            {
                Id = 3,
                MusicianId = 1,
                Number = 8,
                Text = "Test rating 3",
                UserName = "User 3"
            }
        };

        [SetUp]
        public void MusicianRatingSetup() => _musicianRatingRepository = ServiceProvider.GetRequiredService<MusicianRatingRepository>();

        [Test]
        public async Task Successfully_create_musician_rating()
        {
            var id = await _musicianRatingRepository.Add(new RatingDto{Text = "Test"});

            var ratings = await GetContext().MusicianRatings.ToListAsync();

            id.Should().Be(1);
            ratings.Should().HaveCount(1);
            ratings.Single().Should().BeEquivalentTo(new MusicianRating
            {
                Id = 1,
                Text = "Test"
            });
        }

        [Test]
        public async Task Successfully_update_musician_rating()
        {
            var dbContext = GetContext();
            await dbContext.MusicianRatings.AddAsync(new MusicianRating{ Text = "Test"});
            await dbContext.SaveChangesAsync();

            var ratingDto = new RatingDto { Id = 1, Text = "NewValue" };
            await _musicianRatingRepository.Update(ratingDto);

            var bandRating = await GetContext().MusicianRatings.SingleAsync(x => x.Id == 1);
            bandRating.Should().BeEquivalentTo(new MusicianRating
            {
                Id = 1,
                Text = "NewValue"
            });
        }

        [Test]
        public async Task Get_existing_musician_rating()
        {
            var dbContext = GetContext();
            await dbContext.MusicianRatings.AddAsync(new MusicianRating
            {
                Number = 5,
                Text = "Test", 
                MusicianId = 1, 
                Musician = new Musician{FirstName = "Test", LastName = "Testovic"}
            });
            await dbContext.SaveChangesAsync();

            var rating = await _musicianRatingRepository.GetById(1);

            rating.Should().BeEquivalentTo(new RatingDto
            {
                Id = 1,
                Text = "Test",
                InterpretId = 1,
                InterpretName = "Test Testovic",
                Number = 5
            });
        }

        [Test]
        public async Task Get_not_existing_should_return_null()
        {
            var ratingDto = await _musicianRatingRepository.GetById(1);

            ratingDto.Should().Be(null);
        }

        [Test]
        public async Task Get_all_musician_ratings()
        {
            var ratingDtos = MusicianRatings().AsQueryable().ProjectTo<RatingDto>().ToList();
            ratingDtos.ForEach(x => x.InterpretName = "Test Testovic");
            var dbContext = GetContext();
            await dbContext.MusicianRatings.AddRangeAsync(MusicianRatings());
            await dbContext.SaveChangesAsync();

            var allRatings = await _musicianRatingRepository.GetAll();

            allRatings.Should().HaveCount(3);
            allRatings.ForEach(x => ratingDtos.Should().ContainEquivalentOf(x));
        }

        [Test]
        public async Task Successfully_delete_musician_rating()
        {
            var dbContext = GetContext();
            var musicianRating = await dbContext.MusicianRatings.AddAsync(new MusicianRating{Text = "Test"});
            await dbContext.SaveChangesAsync();

            (await dbContext.MusicianRatings.CountAsync()).Should().Be(1);
            await _musicianRatingRepository.Delete(musicianRating.Entity.Id);

            (await dbContext.MusicianRatings.CountAsync()).Should().Be(0);
        }

        [Test]
        public void Delete_not_existing_should_do_nothing()
            => Assert.DoesNotThrowAsync(async () => await _musicianRatingRepository.Delete(5));
    }
}

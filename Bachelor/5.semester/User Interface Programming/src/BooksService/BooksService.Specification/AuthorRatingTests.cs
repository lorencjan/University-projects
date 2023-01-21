using System;
using System.Threading.Tasks;
using BooksService.Application.Ratings;
using BooksService.Domain.Entities;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace BooksService.Specification
{
    public class AuthorRatingTests : TestFixture
    {
        [SetUp]
        public async Task RatingTestsSetup()
        {
            var ctx = GetContext();
            await ctx.Users.AddAsync(new User());
            await ctx.Authors.AddAsync(new Author());
            await ctx.SaveChangesAsync();
        }
        
        private AuthorRating CreateAuthorRating() => new AuthorRating
        {
            Number = 5,
            Text = "Test",
            AuthorId = 1,
            UserId = 1
        };

        [Test]
        public async Task Successfully_create_author_rating()
        {
            var rating = CreateAuthorRating();

            rating.Id = await GetMediator().Send(new CreateAuthorRating
            {
                Number = rating.Number,
                Text = rating.Text,
                UserId = rating.UserId,
                AuthorId = rating.AuthorId
            });
            var created = await GetContext().AuthorRatings.SingleAsync();

            rating.Id.Should().Be(1);
            created.Date.Should().BeCloseTo(DateTime.Now, new TimeSpan(0, 1, 0));
            created.Date = new DateTime();
            created.Should().BeEquivalentTo(rating);
        }

        [Test]
        public async Task Successfully_update_author_rating()
        {
            var ctx = GetContext();
            await ctx.AddAsync(CreateAuthorRating());
            await ctx.SaveChangesAsync();

            var rating = await GetContext().AuthorRatings.AsNoTracking().SingleAsync();
            rating.Number = 7;
            rating.Text = "New text";
            await GetMediator().Send(new UpdateAuthorRating
            {
                Id = rating.Id,
                Number = rating.Number,
                Text = rating.Text
            });
            var updated = await GetContext().AuthorRatings.SingleAsync();

            updated.Should().BeEquivalentTo(rating);
        }

        [Test]
        public async Task Successfully_delete_author_rating()
        {
            var ctx = GetContext();
            var rating = await ctx.AuthorRatings.AddAsync(CreateAuthorRating());
            await ctx.SaveChangesAsync();

            (await ctx.AuthorRatings.CountAsync()).Should().Be(1);
            await GetMediator().Send(new DeleteAuthorRating { Id = rating.Entity.Id });

            (await ctx.AuthorRatings.CountAsync()).Should().Be(0);
        }

        [Test]
        public void Delete_not_existing_should_do_nothing()
            => Assert.DoesNotThrowAsync(async () => await GetMediator().Send(new DeleteAuthorRating { Id = 1 }));
    }
}

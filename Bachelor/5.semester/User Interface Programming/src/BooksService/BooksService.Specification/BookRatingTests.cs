using System;
using System.Threading.Tasks;
using BooksService.Application.Ratings;
using BooksService.Domain.Entities;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace BooksService.Specification
{
    public class BookRatingTests : TestFixture
    {
        [SetUp]
        public async Task RatingTestsSetup()
        {
            var ctx = GetContext();
            await ctx.Users.AddAsync(new User());
            await ctx.Books.AddAsync(new Book());
            await ctx.SaveChangesAsync();
        }

        private BookRating CreateBookRating() => new BookRating
        {
            Number = 5,
            Text = "Test",
            BookId = 1,
            UserId = 1
        };

        [Test]
        public async Task Successfully_create_book_rating()
        {
            var rating = CreateBookRating();

            rating.Id = await GetMediator().Send(new CreateBookRating
            {
                Number = rating.Number,
                Text = rating.Text,
                UserId = rating.UserId,
                BookId = rating.BookId
            });
            var created = await GetContext().BookRatings.SingleAsync();

            rating.Id.Should().Be(1);
            created.Date.Should().BeCloseTo(DateTime.Now, new TimeSpan(0, 1, 0));
            created.Date = new DateTime();
            created.Should().BeEquivalentTo(rating);
        }

        [Test]
        public async Task Successfully_update_book_rating()
        {
            var ctx = GetContext();
            await ctx.AddAsync(CreateBookRating());
            await ctx.SaveChangesAsync();

            var rating = await GetContext().BookRatings.AsNoTracking().SingleAsync();
            rating.Number = 7;
            rating.Text = "New text";
            await GetMediator().Send(new UpdateBookRating
            {
                Id = rating.Id,
                Number = rating.Number,
                Text = rating.Text
            });
            var updated = await GetContext().BookRatings.SingleAsync();

            updated.Should().BeEquivalentTo(rating);
        }

        [Test]
        public async Task Successfully_delete_book_rating()
        {
            var ctx = GetContext();
            var rating = await ctx.BookRatings.AddAsync(CreateBookRating());
            await ctx.SaveChangesAsync();

            (await ctx.BookRatings.CountAsync()).Should().Be(1);
            await GetMediator().Send(new DeleteBookRating { Id = rating.Entity.Id });

            (await ctx.BookRatings.CountAsync()).Should().Be(0);
        }

        [Test]
        public void Delete_not_existing_should_do_nothing()
            => Assert.DoesNotThrowAsync(async () => await GetMediator().Send(new DeleteBookRating { Id = 1 }));
    }
}

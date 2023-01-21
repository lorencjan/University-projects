using System.Threading.Tasks;
using BooksService.Application.Favorites;
using BooksService.Domain.Entities;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace BooksService.Specification
{
    public class BookFavoriteTests : TestFixture
    {
        [SetUp]
        public async Task BookFavoriteTestsSetup()
        {
            var ctx = GetContext();
            await ctx.Users.AddAsync(new User());
            await ctx.Books.AddAsync(new Book());
            await ctx.SaveChangesAsync();
        }

        [Test]
        public async Task Successfully_create_add_book_to_favorites()
        {
            await GetMediator().Send(new AddFavoriteBook { UserId = 1, BookId = 1 });
            
            var created = await GetContext().UsersFavoriteBooks.SingleAsync();

            created.BookId.Should().Be(1);
            created.UserId.Should().Be(1);
        }

        [Test]
        public async Task Adding_book_who_is_already_in_favorites_should_do_nothing()
        {
            var ctx = GetContext();
            await ctx.UsersFavoriteBooks.AddAsync(new UserFavoriteBook { UserId = 1, BookId = 1 });
            await ctx.SaveChangesAsync();

            (await ctx.UsersFavoriteBooks.CountAsync()).Should().Be(1);
            Assert.DoesNotThrowAsync(async () => await GetMediator().Send(new AddFavoriteBook { UserId = 1, BookId = 1 }));

            (await ctx.UsersFavoriteBooks.CountAsync()).Should().Be(1);
        }

        [Test]
        public async Task Successfully_remove_book_from_favorites_rating()
        {
            var ctx = GetContext();
            await ctx.UsersFavoriteBooks.AddAsync(new UserFavoriteBook { UserId = 1, BookId = 1 });
            await ctx.SaveChangesAsync();

            (await ctx.UsersFavoriteBooks.CountAsync()).Should().Be(1);
            await GetMediator().Send(new RemoveFavoriteBook { UserId = 1, BookId = 1});

            (await ctx.UsersFavoriteBooks.CountAsync()).Should().Be(0);
        }

        [Test]
        public void Delete_not_existing_should_do_nothing()
            => Assert.DoesNotThrowAsync(async () => await GetMediator().Send(new RemoveFavoriteBook { UserId = 1, BookId = 1 }));
    }
}

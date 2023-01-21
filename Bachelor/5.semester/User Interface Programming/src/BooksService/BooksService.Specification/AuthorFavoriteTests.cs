using System.Threading.Tasks;
using BooksService.Application.Favorites;
using BooksService.Domain.Entities;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace BooksService.Specification
{
    public class AuthorFavoriteTests : TestFixture
    {
        [SetUp]
        public async Task AuthorFavoriteTestsSetup()
        {
            var ctx = GetContext();
            await ctx.Users.AddAsync(new User());
            await ctx.Authors.AddAsync(new Author());
            await ctx.SaveChangesAsync();
        }

        [Test]
        public async Task Successfully_create_add_author_to_favorites()
        {
            await GetMediator().Send(new AddFavoriteAuthor { UserId = 1, AuthorId = 1 });
            
            var created = await GetContext().UsersFavoriteAuthors.SingleAsync();

            created.AuthorId.Should().Be(1);
            created.UserId.Should().Be(1);
        }

        [Test]
        public async Task Adding_author_who_is_already_in_favorites_should_do_nothing()
        {
            var ctx = GetContext();
            await ctx.UsersFavoriteAuthors.AddAsync(new UserFavoriteAuthor { UserId = 1, AuthorId = 1 });
            await ctx.SaveChangesAsync();

            (await ctx.UsersFavoriteAuthors.CountAsync()).Should().Be(1);
            Assert.DoesNotThrowAsync(async () => await GetMediator().Send(new AddFavoriteAuthor { UserId = 1, AuthorId = 1 }));

            (await ctx.UsersFavoriteAuthors.CountAsync()).Should().Be(1);
        }

        [Test]
        public async Task Successfully_remove_author_from_favorites_rating()
        {
            var ctx = GetContext();
            await ctx.UsersFavoriteAuthors.AddAsync(new UserFavoriteAuthor { UserId = 1, AuthorId = 1 });
            await ctx.SaveChangesAsync();

            (await ctx.UsersFavoriteAuthors.CountAsync()).Should().Be(1);
            await GetMediator().Send(new RemoveFavoriteAuthor { UserId = 1, AuthorId = 1});

            (await ctx.UsersFavoriteAuthors.CountAsync()).Should().Be(0);
        }

        [Test]
        public void Delete_not_existing_should_do_nothing()
            => Assert.DoesNotThrowAsync(async () => await GetMediator().Send(new RemoveFavoriteAuthor { UserId = 1, AuthorId = 1 }));
    }
}

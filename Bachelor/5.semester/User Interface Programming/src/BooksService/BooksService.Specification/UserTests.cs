using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BooksService.Application.Users;
using BooksService.Application.Users.Services;
using BooksService.Common;
using BooksService.Domain;
using BooksService.Domain.Entities;
using BooksService.Persistence;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace BooksService.Specification
{
    public class UserTests : TestFixture
    {
        private User CreateUser(int id = 1, string login = "login", string password = "password") => new User
        {
            Id = id,
            Login = login,
            Password = password
        };

        [Test]
        public async Task Successfully_create_user()
        {
            var user = CreateUser();

            var id = await GetMediator().Send(new CreateUser { Login = user.Login, Password = user.Password });
            var created = await GetContext().Users.SingleAsync();

            id.Should().Be(1);
            created.Login.Should().Be(user.Login);
            Encryption.VerifyPasswords(created.Password, user.Password).Should().BeTrue();
        }

        [Test]
        public async Task Successfully_change_password_to_user()
        {
            var ctx = GetContext();
            await ctx.Users.AddAsync(CreateUser());
            await ctx.SaveChangesAsync();

            var user = await GetContext().Users.AsNoTracking().SingleAsync();
            user.Password = "NewPassword";
            await GetMediator().Send(new ChangePassword { UserId = user.Id, NewPassword = user.Password });

            var updated = await GetContext().Users.SingleAsync();
            updated.Id.Should().Be(user.Id);
            updated.Login.Should().Be(user.Login);
            Encryption.VerifyPasswords(updated.Password, user.Password).Should().BeTrue();
        }

        [TestCase("login", "password", true)]
        [TestCase("login", "wrongPassword", false)]
        [TestCase("wrongLogin", "password", false)]
        public async Task Recognize_valid_and_invalid_users_credentials(string login, string password, bool result)
        {
            var user = CreateUser();
            await GetMediator().Send(new CreateUser { Login = user.Login, Password = user.Password });

            var response = await GetMediator().Send(new VerifyCredentials { Login = login, Password = password });

            response.Should().Be(result);
        }

        [Test]
        public async Task Get_existing_user()
        {
            var user = CreateUser();
            var ctx = GetContext();
            await ctx.Users.AddAsync(user);
            await ctx.SaveChangesAsync();

            var response = await GetMediator().Send(new GetUserById { Id = 1 });

            response.Should().BeEquivalentTo(user);
        }

        [Test]
        public async Task Get_not_existing_should_return_null()
        {
            var user = await GetMediator().Send(new GetUserById { Id = 1 });

            user.Should().Be(null);
        }

        [TestCase(1, UserIncludeFeatures.BookRatings)]
        [TestCase(2, UserIncludeFeatures.AuthorRatings)]
        [TestCase(3, UserIncludeFeatures.Feedbacks)]
        [TestCase(4, UserIncludeFeatures.Feedbacks | UserIncludeFeatures.AuthorRatings)]
        [TestCase(5, null)]
        public async Task Get_user_with_included_related_entities(int testRun, UserIncludeFeatures? includes)
        {
            var user = CreateUser();
            var ctx = GetContext();
            await ctx.Users.AddAsync(user);
            await ctx.Authors.AddRangeAsync(new Author());
            await ctx.Books.AddRangeAsync(new Book());
            await ctx.AuthorRatings.AddAsync(new AuthorRating { AuthorId = 1, UserId = 1 });
            await ctx.BookRatings.AddAsync(new BookRating { BookId = 1, UserId = 1 });
            await ctx.UsersFavoriteAuthors.AddAsync(new UserFavoriteAuthor { AuthorId = 1, UserId = 1 });
            await ctx.UsersFavoriteBooks.AddAsync(new UserFavoriteBook { BookId = 1, UserId = 1 });
            await ctx.Feedbacks.AddAsync(new Feedback { Text = "Test", UserId = 1 });
            await ctx.SaveChangesAsync();

            user = await GetMediator().Send(new GetUserById { Id = 1, IncludeFeatures = includes });

            switch (testRun)
            {
                case 1:
                    user.BookRatings.Should().HaveCount(1);
                    user.BookRatings.Single().Should().NotBeNull();

                    user.AuthorRatings.Should().BeNull();
                    user.FavoriteAuthors.Should().BeNull();
                    user.FavoriteBooks.Should().BeNull();
                    user.Feedbacks.Should().BeNull();
                    break;
                case 2:
                    user.AuthorRatings.Should().HaveCount(1);
                    user.AuthorRatings.Single().Should().NotBeNull();

                    user.BookRatings.Should().BeNull();
                    user.FavoriteAuthors.Should().BeNull();
                    user.FavoriteBooks.Should().BeNull();
                    user.Feedbacks.Should().BeNull();
                    break;
                case 3:
                    user.Feedbacks.Should().HaveCount(1);
                    user.Feedbacks.Single().Should().NotBeNull();

                    user.AuthorRatings.Should().BeNull();
                    user.BookRatings.Should().BeNull();
                    user.FavoriteBooks.Should().BeNull();
                    user.FavoriteAuthors.Should().BeNull();
                    break;
                case 4:
                    user.AuthorRatings.Should().HaveCount(1);
                    user.AuthorRatings.Single().Should().NotBeNull();
                    user.Feedbacks.Should().HaveCount(1);
                    user.Feedbacks.Single().Should().NotBeNull();

                    user.BookRatings.Should().BeNull();
                    user.FavoriteAuthors.Should().BeNull();
                    break;
                case 5:
                    user.BookRatings.Should().BeNull();
                    user.AuthorRatings.Should().BeNull();
                    user.FavoriteBooks.Should().BeNull();
                    user.FavoriteAuthors.Should().BeNull();
                    user.Feedbacks.Should().BeNull();
                    break;
            }
        }

        [Test]
        public async Task Get_all_users()
        {
            var user1 = CreateUser();
            var user2 = CreateUser(2, "Test2");
            var user3 = CreateUser(3, "Test3");
            var ctx = GetContext();
            await ctx.Users.AddRangeAsync(user1, user2, user3);
            await ctx.SaveChangesAsync();

            var users = await GetMediator().Send(new GetUsers());

            users.Should().HaveCount(3);
            users.Should().ContainEquivalentOf(user1);
            users.Should().ContainEquivalentOf(user2);
            users.Should().ContainEquivalentOf(user3);
        }

        [TestCase(1, SortOrdering.Ascending, SortUsersBy.Login)]
        [TestCase(2, SortOrdering.Descending, SortUsersBy.Feedbacks)]
        [TestCase(3, SortOrdering.Ascending, SortUsersBy.BookRatings)]
        [TestCase(4, SortOrdering.Descending, SortUsersBy.AuthorRatings)]
        public async Task Get_sorted_users(int testRun, SortOrdering order, SortUsersBy sort)
        {
            var ctx = GetContext();
            await ctx.Users.AddRangeAsync(CreateUser(1, "abc"), CreateUser(2, "xyz"), CreateUser(3, "mno"));
            await ctx.BookRatings.AddRangeAsync(
                new BookRating{UserId = 1},new BookRating{UserId = 1},new BookRating{UserId = 2},new BookRating{UserId = 2},
                new BookRating{UserId = 2},new BookRating{UserId = 2},new BookRating{UserId = 3});
            await ctx.AuthorRatings.AddRangeAsync(
                new AuthorRating { UserId = 1 }, new AuthorRating { UserId = 2 }, new AuthorRating { UserId = 2 }, new AuthorRating { UserId = 3 },
                new AuthorRating { UserId = 3 }, new AuthorRating { UserId = 3 }, new AuthorRating { UserId = 3 });
            await ctx.Feedbacks.AddRangeAsync(
                new Feedback { UserId = 1 }, new Feedback { UserId = 1 }, new Feedback { UserId = 1 }, new Feedback { UserId = 2 },
                new Feedback { UserId = 3 }, new Feedback { UserId = 3 }, new Feedback { UserId = 3 });
            await ctx.SaveChangesAsync();

            var users = await GetMediator().Send(new GetUsers { SortOrdering = order, SortUsersBy = sort });
            
            users.Should().HaveCount(3);
            switch (testRun)
            {
                case 1: users.Select(x => x.Login).Should().BeInAscendingOrder(); break;
                case 2: users.Select(x => x.Feedbacks.Count).Should().BeInDescendingOrder(); break;
                case 3: users.Select(x => x.BookRatings.Count).Should().BeInAscendingOrder(); break;
                case 4: users.Select(x => x.AuthorRatings.Count).Should().BeInDescendingOrder(); break;
            }
        }

        [Test]
        public async Task Get_users_with_includes()
        {
            var ctx = GetContext();
            await ctx.Users.AddAsync(CreateUser(1, "abc"));
            await ctx.BookRatings.AddRangeAsync(new BookRating { UserId = 1 });
            await ctx.SaveChangesAsync();

            var users = await GetMediator().Send(new GetUsers { IncludeFeatures = UserIncludeFeatures.BookRatings});

            users.Should().HaveCount(1);
            users.Single().BookRatings.Should().NotBeNull();
            users.Single().BookRatings.Should().HaveCount(1);
        }

        [TestCase(true)]
        [TestCase(false)]
        public async Task Get_all_users_light(bool withFilters)
        {
            var ctx = GetContext();
            await ctx.Users.AddRangeAsync(CreateUser(1, "aaa"), CreateUser(2, "abc"), CreateUser(3, "bcd"));
            await ctx.BookRatings.AddRangeAsync(
                new BookRating { UserId = 1 }, new BookRating { UserId = 1 }, new BookRating { UserId = 2 }, new BookRating { UserId = 2 },
                new BookRating { UserId = 2 }, new BookRating { UserId = 2 }, new BookRating { UserId = 3 });
            await ctx.AuthorRatings.AddRangeAsync(
                new AuthorRating { UserId = 1 }, new AuthorRating { UserId = 2 }, new AuthorRating { UserId = 2 }, new AuthorRating { UserId = 3 },
                new AuthorRating { UserId = 3 }, new AuthorRating { UserId = 3 }, new AuthorRating { UserId = 3 });
            await ctx.Feedbacks.AddRangeAsync(
                new Feedback { UserId = 1 }, new Feedback { UserId = 1 }, new Feedback { UserId = 1 }, new Feedback { UserId = 2 },
                new Feedback { UserId = 3 }, new Feedback { UserId = 3 }, new Feedback { UserId = 3 });
            await ctx.SaveChangesAsync();

            var request = withFilters
                ? new GetUsersLight{LoginFilter = "a", SortOrdering = SortOrdering.Descending, SortUsersBy = SortUsersBy.Login}
                : new GetUsersLight();
            var users = await GetMediator().Send(request);

            users.Should().HaveCount(withFilters ? 2 : 3);
            users.Should().ContainEquivalentOf(new UserLight
            {
                Id = 1,
                Login = "aaa",
                AuthorRatingsCount = 1,
                BookRatingsCount = 2,
                FeedbacksCount = 3
            });
            users.Should().ContainEquivalentOf(new UserLight
            {
                Id = 2,
                Login = "abc",
                AuthorRatingsCount = 2,
                BookRatingsCount = 4,
                FeedbacksCount = 1
            });

            if (withFilters)
            {
                users.Select(x => x.Login).Should().BeInDescendingOrder();
                return;
            }

            users.Should().ContainEquivalentOf(new UserLight
            {
                Id = 3,
                Login = "bcd",
                AuthorRatingsCount = 4,
                BookRatingsCount = 1,
                FeedbacksCount = 3
            });
        }

        [Test]
        public async Task Successfully_get_all_favorite_authors()
        {
            var ctx = GetContext();
            await ctx.Users.AddAsync(CreateUser());
            await ctx.Authors.AddRangeAsync(new Author{Id = 1, FirstName = "a", LastName = "b", Books = new List<AuthorBook>{new AuthorBook{AuthorId = 1, BookId = 1}}, Ratings = new List<AuthorRating>{new AuthorRating{AuthorId = 1, Number = 8}}}, new Author{ Id = 2});
            await ctx.Books.AddAsync(new Book{Id = 1, Name = "Book"});
            await ctx.UsersFavoriteAuthors.AddAsync(new UserFavoriteAuthor{UserId = 1, AuthorId = 1});
            await ctx.SaveChangesAsync();

            var favorites = await GetMediator().Send(new GetFavoriteAuthors {UserId = 1});

            favorites.Should().HaveCount(1);
            favorites.Single().BookCount.Should().Be(1);
            favorites.Single().Rating.Should().Be(8);
        }

        [Test]
        public async Task Successfully_get_all_favorite_books()
        {
            var ctx = GetContext();
            await ctx.Users.AddAsync(CreateUser());
            await ctx.Books.AddRangeAsync(new Book { Id = 1, Name = "a", Authors = new List<AuthorBook> { new AuthorBook { AuthorId = 1, BookId = 1 } }, Genres = new List<GenreBook>{new GenreBook{BookId = 1, GenreId = 1}}, Ratings = new List<BookRating> { new BookRating { BookId = 1, Number = 8 } } }, new Book { Id = 2 });
            await ctx.Authors.AddAsync(new Author { Id = 1, FirstName = "a", LastName = "b"});
            await ctx.Genres.AddAsync(new Genre { Id = 1, Name = "Fantasy" });
            await ctx.UsersFavoriteBooks.AddAsync(new UserFavoriteBook { UserId = 1, BookId = 1 });
            await ctx.SaveChangesAsync();

            var favorites = await GetMediator().Send(new GetFavoriteBooks { UserId = 1 });

            favorites.Should().HaveCount(1);
            favorites.Single().Author.Should().Be("a b");
            favorites.Single().Genres.Should().Be("Fantasy");
            favorites.Single().Rating.Should().Be(8);
        }

        [Test]
        public async Task Successfully_delete_user()
        {
            var ctx = GetContext();
            var user = await ctx.Users.AddAsync(CreateUser());
            await ctx.SaveChangesAsync();

            (await ctx.Users.CountAsync()).Should().Be(1);
            await GetMediator().Send(new DeleteUser { Id = user.Entity.Id });

            (await ctx.Users.CountAsync()).Should().Be(0);
        }

        [Test]
        public void Delete_not_existing_should_do_nothing()
            => Assert.DoesNotThrowAsync(async () => await GetMediator().Send(new DeleteUser { Id = 1 }));
    }
}

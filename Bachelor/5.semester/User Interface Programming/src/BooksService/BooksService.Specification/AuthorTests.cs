using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BooksService.Application.Authors;
using BooksService.Common;
using BooksService.Domain;
using BooksService.Domain.Entities;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace BooksService.Specification
{
    public class AuthorTests : TestFixture
    {
        private Author CreateAuthor(int id = 1, string firstName = "Test", string lastName = "Testovic") => new Author
        {
            Id = id,
            FirstName = firstName,
            LastName = lastName,
            Photo = new byte[]{1,2,3,4,5},
            Country = "TestLand",
            BirthDate = new DateTime(2000, 1, 1),
            Biography = "Something"
        };

        [Test]
        public async Task Successfully_create_author()
        {
            var author = CreateAuthor();

            var id = await GetMediator().Send(new CreateAuthor { Author = author });
            var created = await GetContext().Authors.SingleAsync();

            id.Should().Be(1);
            created.Should().BeEquivalentTo(author);
        }

        [Test]
        public async Task Successfully_update_author()
        {
            var author = CreateAuthor();
            var ctx = GetContext();
            await ctx.Authors.AddAsync(author);
            await ctx.SaveChangesAsync();

            author = await GetContext().Authors.SingleAsync();
            author.FirstName = "NewFirstName";
            author.LastName = "NewLastName";
            author.Photo = new byte[]{6,7,8,9};
            author.BirthDate = new DateTime(1990, 6, 30);
            await GetMediator().Send(new UpdateAuthor { Author = author });
            
            var updated = await GetContext().Authors.SingleAsync();
            updated.Should().BeEquivalentTo(author);
        }

        [Test]
        public async Task Get_existing_author()
        {
            var author = CreateAuthor();
            var ctx = GetContext();
            await ctx.Authors.AddAsync(author);
            await ctx.SaveChangesAsync();

            var response = await GetMediator().Send(new GetAuthorById { Id = 1 });

            response.Should().BeEquivalentTo(author);
        }

        [Test]
        public async Task Get_not_existing_should_return_null()
        {
            var author = await GetMediator().Send(new GetAuthorById { Id = 1 });

            author.Should().Be(null);
        }

        [TestCase(1, AuthorIncludeFeatures.JoinBooks)]
        [TestCase(2, AuthorIncludeFeatures.Books)]
        [TestCase(3, AuthorIncludeFeatures.Ratings)]
        [TestCase(4, AuthorIncludeFeatures.Ratings | AuthorIncludeFeatures.Books)]
        [TestCase(5, null)]
        public async Task Get_author_with_included_related_entities(int testRun, AuthorIncludeFeatures? includes)
        {
            var author = CreateAuthor();
            author.Ratings = new List<AuthorRating> { new AuthorRating { Number = 10, UserId = 1 } };
            var ctx = GetContext();
            await ctx.Authors.AddAsync(author);
            await ctx.Books.AddRangeAsync(new Book(), new Book());
            await ctx.AuthorBooks.AddRangeAsync(new AuthorBook { AuthorId = 1, BookId = 1 }, new AuthorBook { AuthorId = 1, BookId = 2 });
            await ctx.Users.AddRangeAsync(new User(), new User());
            await ctx.UsersFavoriteAuthors.AddRangeAsync(new UserFavoriteAuthor { AuthorId = 1, UserId = 1 }, new UserFavoriteAuthor { AuthorId = 1, UserId = 2 });
            await ctx.SaveChangesAsync();

            author = await GetMediator().Send(new GetAuthorById { Id = 1, IncludeFeatures = includes});

            switch (testRun)
            {
                case 1:
                    author.Books.Should().HaveCount(2);
                    author.Books[0].Should().NotBeNull();
                    author.Books[1].Should().NotBeNull();

                    author.Ratings.Should().BeNull();
                    author.FavoredBy.Should().BeNull();
                    break;
                case 2:
                    author.Books.Should().HaveCount(2);
                    author.Books[0].Should().NotBeNull();
                    author.Books[0].Book.Should().NotBeNull();
                    author.Books[1].Should().NotBeNull();
                    author.Books[1].Book.Should().NotBeNull();

                    author.Ratings.Should().BeNull();
                    author.FavoredBy.Should().BeNull();
                    break;
                case 3:
                    author.Books.Should().BeNull();

                    author.Ratings.Should().HaveCount(1);
                    author.Ratings.Single().Should().NotBeNull();
                    author.Ratings.Single().Number.Should().Be(10);

                    author.FavoredBy.Should().BeNull();
                    break;
                case 4:
                    author.Books.Should().HaveCount(2);
                    author.Books[0].Should().NotBeNull();
                    author.Books[0].Book.Should().NotBeNull();
                    author.Books[1].Should().NotBeNull();
                    author.Books[1].Book.Should().NotBeNull();

                    author.Ratings.Should().HaveCount(1);
                    author.Ratings.Single().Should().NotBeNull();
                    author.Ratings.Single().Number.Should().Be(10);

                    author.FavoredBy.Should().BeNull();
                    break;
                case 5:
                    author.Books.Should().BeNull();
                    author.Ratings.Should().BeNull();
                    author.FavoredBy.Should().BeNull();
                    break;
            }
        }

        [Test]
        public async Task Get_all_authors()
        {
            var author1 = CreateAuthor();
            var author2 = CreateAuthor(2, "Test2", "Testovic2");
            var author3 = CreateAuthor(3, "Test3", "Testovic3");
            var ctx = GetContext();
            await ctx.Authors.AddRangeAsync(author1, author2, author3);
            await ctx.SaveChangesAsync();

            var authors = await GetMediator().Send(new GetAuthors());

            authors.Should().HaveCount(3);
            authors.Should().ContainEquivalentOf(author1);
            authors.Should().ContainEquivalentOf(author2);
            authors.Should().ContainEquivalentOf(author3);
        }

        [Test]
        public async Task Get_authors_with_includes()
        {
            var ctx = GetContext();
            await ctx.Authors.AddAsync(CreateAuthor());
            await ctx.Books.AddAsync(new Book());
            await ctx.AuthorBooks.AddAsync(new AuthorBook{AuthorId = 1, BookId = 1});
            await ctx.SaveChangesAsync();

            var authors = await GetMediator().Send(new GetAuthors {IncludeFeatures = AuthorIncludeFeatures.Books});

            authors.Should().HaveCount(1);
            authors.Single().Books.Should().HaveCount(1);
            authors.Single().Books.Single().Book.Should().NotBeNull();
        }

        [TestCase(1, "stou", null)]
        [TestCase(2, null, "tesTom")]
        [TestCase(3, "oVic3", "ie")]
        public async Task Get_filtered_authors(int testRun, string name, string country)
        {
            var author1 = CreateAuthor(1, "Testous", "Testovic");
            var author2 = CreateAuthor(2, "Test2", "Testovic2");
            author2.Country = "Testomanie";
            var author3 = CreateAuthor(3, "Test3", "Testovic3");
            author3.Country = "Testomanie";
            var ctx = GetContext();
            await ctx.Authors.AddRangeAsync(author1, author2, author3);
            await ctx.SaveChangesAsync();

            var authors = await GetMediator().Send(new GetAuthors { Filter = new AuthorFilter {Country = country, Name = name}});

            switch (testRun)
            {
                case 1:
                    authors.Should().HaveCount(1);
                    authors.Single().Should().BeEquivalentTo(author1);
                    break;
                case 2:
                    authors.Should().HaveCount(2);
                    authors.Should().ContainEquivalentOf(author2);
                    authors.Should().ContainEquivalentOf(author3);
                    break;
                case 3:
                    authors.Should().HaveCount(1);
                    authors.Single().Should().BeEquivalentTo(author3);
                    break;
            }
        }

        [TestCase(1, SortOrdering.Ascending, SortAuthorsBy.FirstName)]
        [TestCase(2, SortOrdering.Descending, SortAuthorsBy.LastName)]
        [TestCase(3, SortOrdering.Ascending, SortAuthorsBy.BirthDate)]
        [TestCase(4, SortOrdering.Descending, SortAuthorsBy.Country)]
        [TestCase(5, SortOrdering.Ascending, SortAuthorsBy.Rating)]
        [TestCase(6, SortOrdering.Descending, SortAuthorsBy.NumberOfBooks)]
        public async Task Get_sorted_authors(int testRun, SortOrdering order, SortAuthorsBy sort)
        {
            var author1 = CreateAuthor(1, "abc", "xyz");
            author1.BirthDate = new DateTime(1999,1,1);
            author1.Ratings = new List<AuthorRating>{new AuthorRating{Number = 10}};
            var author2 = CreateAuthor(2, "xyz", "mno");
            author2.Country = "LandOfTest";
            author2.Ratings = new List<AuthorRating> { new AuthorRating { Number = 8 } };
            var author3 = CreateAuthor(3, "mno", "abc");
            author3.BirthDate = new DateTime(2001, 1, 1);
            author3.Country = "PlaceOfTest";
            author3.Ratings = new List<AuthorRating> { new AuthorRating { Number = 10 }, new AuthorRating { Number = 8 } };
            var ctx = GetContext();
            await ctx.Authors.AddRangeAsync(author1, author2, author3);
            await ctx.Books.AddRangeAsync(new Book(), new Book(), new Book());
            await ctx.AuthorBooks.AddRangeAsync(
                new AuthorBook {AuthorId = 1, BookId = 1},
                new AuthorBook {AuthorId = 2, BookId = 1},
                new AuthorBook {AuthorId = 2, BookId = 2},
                new AuthorBook {AuthorId = 2, BookId = 3},
                new AuthorBook {AuthorId = 3, BookId = 2},
                new AuthorBook {AuthorId = 3, BookId = 3});
            await ctx.SaveChangesAsync();

            var authors = await GetMediator().Send(new GetAuthors { SortOrdering = order, SortAuthorsBy = sort });

            authors.Should().HaveCount(3);
            switch (testRun)
            {
                case 1: authors.Select(x => x.FirstName).Should().BeInAscendingOrder(); break;
                case 2: authors.Select(x => x.LastName).Should().BeInDescendingOrder(); break;
                case 3: authors.Select(x => x.BirthDate).Should().BeInAscendingOrder(); break;
                case 4: authors.Select(x => x.Country).Should().BeInDescendingOrder(); break;
                case 5: authors.Select(x => x.Ratings.Average(y => y.Number)).Should().BeInAscendingOrder(); break;
                case 6: authors.Select(x => x.Books.Count).Should().BeInDescendingOrder(); break;
            }
        }

        [TestCase(true)]
        [TestCase(false)]
        public async Task Get_all_authors_light(bool withFilters)
        {
            var author1 = CreateAuthor(1, "abc", "xyz");
            author1.Ratings = new List<AuthorRating> { new AuthorRating { Number = 10, UserId = 1} };
            var author2 = CreateAuthor(2, "xyz", "mno");
            author2.Ratings = new List<AuthorRating> { new AuthorRating { Number = 8, UserId = 1 } };
            var author3 = CreateAuthor(3, "mno", "abc");
            author3.Ratings = new List<AuthorRating> { new AuthorRating { Number = 10, UserId = 1 }, new AuthorRating { Number = 7, UserId = 1 } };
            var ctx = GetContext();
            await ctx.Authors.AddRangeAsync(author1, author2, author3);
            await ctx.Books.AddRangeAsync(new Book(), new Book(), new Book());
            await ctx.Users.AddAsync(new User());
            await ctx.AuthorBooks.AddRangeAsync(
                new AuthorBook { AuthorId = 1, BookId = 1 },
                new AuthorBook { AuthorId = 2, BookId = 1 },
                new AuthorBook { AuthorId = 2, BookId = 2 },
                new AuthorBook { AuthorId = 2, BookId = 3 },
                new AuthorBook { AuthorId = 3, BookId = 2 },
                new AuthorBook { AuthorId = 3, BookId = 3 });
            await ctx.SaveChangesAsync();

            var request = withFilters
                ? new GetAuthorsLight {Filter = new AuthorFilter{Name = "xyz"}, SortOrdering = SortOrdering.Ascending, SortAuthorsBy = SortAuthorsBy.LastName}
                : new GetAuthorsLight();
            var authors = await GetMediator().Send(request);

            authors.Should().HaveCount(withFilters ? 2 :3);
            authors.Should().ContainEquivalentOf(new AuthorLight
            {
                Id = 1,
                FirstName = "abc",
                LastName = "xyz",
                Country = "TestLand",
                Photo = new byte[] { 1, 2, 3, 4, 5 },
                BirthDate = new DateTime(2000, 1, 1),
                BookCount = 1,
                Rating = 10
            });
            authors.Should().ContainEquivalentOf(new AuthorLight
            {
                Id = 2,
                FirstName = "xyz",
                LastName = "mno",
                Country = "TestLand",
                Photo = new byte[] { 1, 2, 3, 4, 5 },
                BirthDate = new DateTime(2000, 1, 1),
                BookCount = 3,
                Rating = 8
            });

            if (withFilters)
            {
                authors.Select(x => x.LastName).Should().BeInAscendingOrder();
                return;
            }

            authors.Should().ContainEquivalentOf(new AuthorLight
            {
                Id = 3,
                FirstName = "mno",
                LastName = "abc",
                Country = "TestLand",
                Photo = new byte[] { 1, 2, 3, 4, 5 },
                BirthDate = new DateTime(2000, 1, 1),
                BookCount = 2,
                Rating = 8.5
            });
        }

        [Test]
        public async Task Successfully_delete_author()
        {
            var ctx = GetContext();
            var author = await ctx.Authors.AddAsync(CreateAuthor());
            await ctx.SaveChangesAsync();

            (await ctx.Authors.CountAsync()).Should().Be(1);
            await GetMediator().Send(new DeleteAuthor { Id = author.Entity.Id });

            (await ctx.Authors.CountAsync()).Should().Be(0);
        }

        [Test]
        public void Delete_not_existing_should_do_nothing()
            => Assert.DoesNotThrowAsync(async () => await GetMediator().Send(new DeleteAuthor { Id = 1 }));
    }
}

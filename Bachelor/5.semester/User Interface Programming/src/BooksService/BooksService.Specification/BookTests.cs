using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BooksService.Application.Books;
using BooksService.Common;
using BooksService.Domain;
using BooksService.Domain.Entities;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace BooksService.Specification
{
    public class BookTests : TestFixture
    {
        private Book CreateBook(int id = 1, string name = "Test", int year = 2000, string isbn = "12345-15-78", int pages = 400)
            => new Book
            {
                Id = id,
                Name = name,
                Photo = new byte[] { 1, 2, 3, 4, 5 },
                Year = year,
                Pages = pages,
                Isbn = isbn,
                Description = "Something"
            };

        [Test]
        public async Task Successfully_create_book()
        {
            var book = CreateBook();

            var id = await GetMediator().Send(new CreateBook { Book = book });
            var created = await GetContext().Books.SingleAsync();

            id.Should().Be(1);
            created.Should().BeEquivalentTo(book);
        }

        [Test]
        public async Task Successfully_update_book()
        {
            var book = CreateBook();
            var ctx = GetContext();
            await ctx.Books.AddAsync(book);
            await ctx.SaveChangesAsync();

            book = await GetContext().Books.AsNoTracking().SingleAsync();
            book.Name = "NewName";
            book.Isbn = "789456-123";
            book.Photo = new byte[] { 6, 7, 8, 9 };
            book.Pages = 450;
            await GetMediator().Send(new UpdateBook { Book = book });

            var updated = await GetContext().Books.Include(x => x.Authors).Include(x => x.Genres).SingleAsync();
            updated.Should().BeEquivalentTo(book);
        }

        [Test]
        public async Task Successfully_change_books_author()
        {
            var book = CreateBook();
            var author1 = new Author { Id = 1, FirstName = "test1" };
            var author2 = new Author { Id = 2, FirstName = "test2" };
            var author3 = new Author { Id = 3, FirstName = "test3" };
            var ctx = GetContext();
            await ctx.Books.AddAsync(book);
            await ctx.Authors.AddRangeAsync(author1, author2, author3);
            await ctx.SaveChangesAsync();
            await ctx.AuthorBooks.AddRangeAsync(
                new AuthorBook{AuthorId = 1, BookId = 1},
                new AuthorBook{AuthorId = 2, BookId = 1});
            await ctx.SaveChangesAsync();

            book = await GetContext().Books.Include(x => x.Authors).AsNoTracking().SingleAsync();
            book.Authors.RemoveAll(x => x.AuthorId == 2);
            book.Authors.Add(new AuthorBook { AuthorId = 3, BookId = 1 });
            await GetMediator().Send(new UpdateBook { Book = book });

            book = await GetContext().Books.Include(x => x.Authors).AsNoTracking().SingleAsync();
            book.Authors.Should().HaveCount(2);
            book.Authors[0].AuthorId.Should().Be(1);
            book.Authors[1].AuthorId.Should().Be(3);
        }

        [Test]
        public async Task Successfully_change_books_genre()
        {
            var book = CreateBook();
            var genre1 = new Genre { Id = 1, Name = "test1" };
            var genre2 = new Genre { Id = 2, Name = "test2" };
            var genre3 = new Genre { Id = 3, Name = "test3" };
            var ctx = GetContext();
            await ctx.Books.AddAsync(book);
            await ctx.Genres.AddRangeAsync(genre1, genre2, genre3);
            await ctx.Authors.AddRangeAsync(new Author { Id = 1, FirstName = "test1" });
            await ctx.SaveChangesAsync();
            await ctx.GenreBooks.AddRangeAsync(
                new GenreBook { GenreId = 1, BookId = 1 },
                new GenreBook { GenreId = 2, BookId = 1 });
            await ctx.SaveChangesAsync();

            book = await GetContext().Books.Include(x => x.Genres).AsNoTracking().SingleAsync();
            book.Genres.RemoveAll(x => x.GenreId == 1);
            book.Genres.Add(new GenreBook { GenreId = 3, BookId = 1 });
            await GetMediator().Send(new UpdateBook { Book = book });

            book = await GetContext().Books.Include(x => x.Genres).AsNoTracking().SingleAsync();
            book.Genres.Should().HaveCount(2);
            book.Genres[0].GenreId.Should().Be(2);
            book.Genres[1].GenreId.Should().Be(3);
        }

        [Test]
        public async Task Nulling_genre_or_author_should_remove_all()
        {
            var book = CreateBook();
            var genre1 = new Genre { Id = 1, Name = "test1" };
            var genre2 = new Genre { Id = 2, Name = "test2" };
            var ctx = GetContext();
            await ctx.Books.AddAsync(book);
            await ctx.Genres.AddRangeAsync(genre1, genre2);
            await ctx.SaveChangesAsync();
            await ctx.GenreBooks.AddRangeAsync(
                new GenreBook { GenreId = 1, BookId = 1 },
                new GenreBook { GenreId = 2, BookId = 1 });
            await ctx.AuthorBooks.AddRangeAsync(new AuthorBook { AuthorId = 1, BookId = 1 });
            await ctx.SaveChangesAsync();

            book = await ctx.Books.Include(x => x.Authors).Include(x => x.Genres).AsNoTracking().SingleAsync();
            book.Authors.Should().HaveCount(1);
            book.Genres.Should().HaveCount(2);
            book.Authors = null;
            book.Genres = null;
            await GetMediator().Send(new UpdateBook {Book = book});

            book = await ctx.Books.Include(x => x.Authors).Include(x => x.Genres).AsNoTracking().SingleAsync();
            book.Authors.Should().HaveCount(0);
            book.Genres.Should().HaveCount(0);
        }

        [Test]
        public async Task Get_existing_book()
        {
            var book = CreateBook();
            var ctx = GetContext();
            await ctx.Books.AddAsync(book);
            await ctx.SaveChangesAsync();

            var response = await GetMediator().Send(new GetBookById { Id = 1 });

            response.Should().BeEquivalentTo(book);
        }

        [Test]
        public async Task Get_not_existing_should_return_null()
        {
            var book = await GetMediator().Send(new GetBookById { Id = 1 });

            book.Should().Be(null);
        }

        [TestCase(1, BookIncludeFeatures.All)]
        [TestCase(2, BookIncludeFeatures.Ratings)]
        [TestCase(3, BookIncludeFeatures.Authors)]
        [TestCase(4, BookIncludeFeatures.Genres)]
        [TestCase(5, BookIncludeFeatures.Ratings | BookIncludeFeatures.Authors)]
        [TestCase(6, null)]
        public async Task Get_book_with_included_related_entities(int testRun, BookIncludeFeatures? includes)
        {
            var book = CreateBook();
            book.Ratings = new List<BookRating> { new BookRating { Number = 10, UserId = 1 } };
            var ctx = GetContext();
            await ctx.Books.AddAsync(book);
            await ctx.Authors.AddRangeAsync(new Author(), new Author());
            await ctx.AuthorBooks.AddRangeAsync(new AuthorBook { AuthorId = 1, BookId = 1 }, new AuthorBook { AuthorId = 2, BookId = 1 });
            await ctx.Users.AddRangeAsync(new User(), new User());
            await ctx.UsersFavoriteBooks.AddRangeAsync(new UserFavoriteBook { BookId = 1, UserId = 1 }, new UserFavoriteBook { BookId = 1, UserId = 2 });
            await ctx.Genres.AddAsync(new Genre());
            await ctx.GenreBooks.AddRangeAsync(new GenreBook{GenreId = 1, BookId = 1});
            await ctx.SaveChangesAsync();

            book = await GetMediator().Send(new GetBookById { Id = 1, IncludeFeatures = includes });

            switch (testRun)
            {
                case 1:
                    book.Ratings.Should().HaveCount(1);
                    book.Ratings.Single().Should().NotBeNull();
                    book.Ratings.Single().Number.Should().Be(10);

                    book.Authors.Should().HaveCount(2);
                    book.Authors[0].Should().NotBeNull();
                    book.Authors[0].Book.Should().NotBeNull();
                    book.Authors[1].Should().NotBeNull();
                    book.Authors[1].Book.Should().NotBeNull();

                    book.Genres.Should().HaveCount(1);
                    book.Genres.Single().Should().NotBeNull();

                    book.FavoredBy.Should().BeNull();
                    break;
                case 2:
                    book.Ratings.Should().HaveCount(1);
                    book.Ratings.Single().Should().NotBeNull();
                    book.Ratings.Single().Number.Should().Be(10);

                    book.Authors.Should().BeNull();
                    book.FavoredBy.Should().BeNull();
                    book.Genres.Should().BeNull();
                    break;
                case 3:
                    book.Ratings.Should().BeNull();

                    book.Authors.Should().HaveCount(2);
                    book.Authors[0].Should().NotBeNull();
                    book.Authors[0].Book.Should().NotBeNull();
                    book.Authors[1].Should().NotBeNull();
                    book.Authors[1].Book.Should().NotBeNull();

                    book.FavoredBy.Should().BeNull();
                    book.Genres.Should().BeNull();
                    break;
                case 4:
                    book.Ratings.Should().BeNull();
                    book.Authors.Should().BeNull();
                    book.FavoredBy.Should().BeNull();

                    book.Genres.Should().HaveCount(1);
                    book.Genres.Single().Should().NotBeNull();
                    break;
                case 5:
                    book.Ratings.Should().HaveCount(1);
                    book.Ratings.Single().Should().NotBeNull();
                    book.Ratings.Single().Number.Should().Be(10);

                    book.Authors.Should().HaveCount(2);
                    book.Authors[0].Should().NotBeNull();
                    book.Authors[0].Book.Should().NotBeNull();
                    book.Authors[1].Should().NotBeNull();
                    book.Authors[1].Book.Should().NotBeNull();

                    book.Genres.Should().BeNull();
                    break;
                case 6:
                    book.Ratings.Should().BeNull();
                    book.Authors.Should().BeNull();
                    book.FavoredBy.Should().BeNull();
                    book.Genres.Should().BeNull();
                    break;
            }
        }

        [Test]
        public async Task Get_all_books()
        {
            var book1 = CreateBook();
            var book2 = CreateBook(2, "Test2");
            var book3 = CreateBook(3, "Test3");
            var ctx = GetContext();
            await ctx.Books.AddRangeAsync(book1, book2, book3);
            await ctx.SaveChangesAsync();

            var books = await GetMediator().Send(new GetBooks());

            books.Should().HaveCount(3);
            books.Should().ContainEquivalentOf(book1);
            books.Should().ContainEquivalentOf(book2);
            books.Should().ContainEquivalentOf(book3);
        }

        [TestCase(1, "rAgon", null, null, null, null)]
        [TestCase(2, null, 2000, null, null, null)]
        [TestCase(3, null, null, "45", null, null)]
        [TestCase(4, null, null, null, "bc m", null)]
        [TestCase(5, null, null, null, null, "fanta")]
        [TestCase(6, null, 2000, null, null, "vel")]
        public async Task Get_filtered_books(int testRun, string name, int? year, string isbn, string author, string genre)
        {
            var book1 = CreateBook(1, "Eragon", 2000, "1234");
            var book2 = CreateBook(2, "Eldest", 2000, "2345");
            var book3 = CreateBook(3, "Brisinger", 2001, "4567");
            var author1 = new Author {Id = 1, FirstName = "abc", LastName = "mno"};
            var author2 = new Author {Id = 2, FirstName = "mno", LastName = "xyz"};
            var genre1 = new Genre {Id = 1, Name = "Fantasy"};
            var genre2 = new Genre {Id = 2, Name = "Novel"};
            var ctx = GetContext();
            await ctx.Books.AddRangeAsync(book1, book2, book3);
            await ctx.Authors.AddRangeAsync(author1, author2);
            await ctx.AuthorBooks.AddRangeAsync(
                new AuthorBook{AuthorId = 1, BookId = 1},
                new AuthorBook{AuthorId = 1, BookId = 2},
                new AuthorBook{AuthorId = 2, BookId = 3});
            await ctx.Genres.AddRangeAsync(genre1, genre2);
            await ctx.GenreBooks.AddRangeAsync(
                new GenreBook { GenreId = 1, BookId = 1 },
                new GenreBook { GenreId = 1, BookId = 2 },
                new GenreBook { GenreId = 2, BookId = 2 },
                new GenreBook { GenreId = 2, BookId = 3 });
            await ctx.SaveChangesAsync();

            var books = await GetMediator().Send(new GetBooks { Filter = new BookFilter
            {
                Name = name,
                Year = year,
                Isbn = isbn,
                Author = author,
                Genre = genre
            } });

            foreach (var book in books)
            {
                book.Authors = null;  //both will be included due to the filtering
                book.Genres = null;   //not what we're testing neither what what we have in mock data
            }
            book1.Authors = null; 
            book1.Genres = null;
            book2.Authors = null;
            book2.Genres = null;
            book3.Authors = null;
            book3.Genres = null;

            switch (testRun)
            {
                case 1:
                    books.Should().HaveCount(1);
                    books.Single().Should().BeEquivalentTo(book1);
                    break;
                case 2:
                case 4:
                case 5:
                    books.Should().HaveCount(2);
                    books.Should().ContainEquivalentOf(book1);
                    books.Should().ContainEquivalentOf(book2);
                    break;
                case 3:
                    books.Should().HaveCount(2);
                    books.Should().ContainEquivalentOf(book2);
                    books.Should().ContainEquivalentOf(book3);
                    break;
                case 6:
                    books.Should().HaveCount(1);
                    books.Single().Should().BeEquivalentTo(book2);
                    break;
            }
        }

        [TestCase(1, SortOrdering.Ascending, SortBooksBy.Name)]
        [TestCase(2, SortOrdering.Descending, SortBooksBy.Year)]
        [TestCase(3, SortOrdering.Ascending, SortBooksBy.Pages)]
        [TestCase(4, SortOrdering.Descending, SortBooksBy.Rating)]
        public async Task Get_sorted_books(int testRun, SortOrdering order, SortBooksBy sort)
        {
            var book1 = CreateBook(1, "abc", 2001, "", 500);
            book1.Ratings = new List<BookRating> { new BookRating { Number = 10 } };
            var book2 = CreateBook(2, "xyz", 2000, "", 150);
            var book3 = CreateBook(3, "mno", 2002, "", 300);
            book3.Ratings = new List<BookRating> { new BookRating { Number = 10 }, new BookRating { Number = 8 } };
            var ctx = GetContext();
            await ctx.Books.AddRangeAsync(book1, book2, book3);
            await ctx.SaveChangesAsync();

            var books = await GetMediator().Send(new GetBooks { SortOrdering = order, SortBooksBy = sort, IncludeFeatures = BookIncludeFeatures.Ratings});
            
            books.Should().HaveCount(3);
            switch (testRun)
            {
                case 1: books.Select(x => x.Name).Should().BeInAscendingOrder(); break;
                case 2: books.Select(x => x.Year).Should().BeInDescendingOrder(); break;
                case 3: books.Select(x => x.Pages).Should().BeInAscendingOrder(); break;
                case 4: books.Select(x => x.Ratings.Count == 0 ? 0 : x.Ratings.Average(y => y.Number)).Should().BeInDescendingOrder(); break;
            }
        }

        [TestCase(true)]
        [TestCase(false)]
        public async Task Get_all_books_light(bool withFilters)
        {
            var book1 = CreateBook(1, "aaa", 2001, "111", 500);
            book1.Ratings = new List<BookRating> { new BookRating { Number = 10, UserId = 1 } };
            var book2 = CreateBook(2, "bbb", 2000, "222", 150);
            var book3 = CreateBook(3, "ccc", 2002, "333", 300);
            book3.Ratings = new List<BookRating> { new BookRating { Number = 10, UserId = 1 }, new BookRating { Number = 5, UserId = 1 } };
            var author1 = new Author { Id = 1, FirstName = "abc", LastName = "mno" };
            var author2 = new Author { Id = 2, FirstName = "mno", LastName = "xyz" };
            var genre1 = new Genre { Id = 1, Name = "Fantasy" };
            var genre2 = new Genre { Id = 2, Name = "Novel" };
            var ctx = GetContext();
            await ctx.Books.AddRangeAsync(book1, book2, book3);
            await ctx.Authors.AddRangeAsync(author1, author2);
            await ctx.Users.AddAsync(new User());
            await ctx.AuthorBooks.AddRangeAsync(
                new AuthorBook { AuthorId = 1, BookId = 1 },
                new AuthorBook { AuthorId = 1, BookId = 2 },
                new AuthorBook { AuthorId = 2, BookId = 2 },
                new AuthorBook { AuthorId = 2, BookId = 3 });
            await ctx.Genres.AddRangeAsync(genre1, genre2);
            await ctx.GenreBooks.AddRangeAsync(
                new GenreBook { GenreId = 1, BookId = 1 },
                new GenreBook { GenreId = 1, BookId = 2 },
                new GenreBook { GenreId = 2, BookId = 2 },
                new GenreBook { GenreId = 2, BookId = 3 });
            await ctx.SaveChangesAsync();

            var request = withFilters 
                ? new GetBooksLight { Filter = new BookFilter{Genre = "Fantasy"}, SortOrdering = SortOrdering.Descending, SortBooksBy = SortBooksBy.Name} 
                : new GetBooksLight();
            var books = await GetMediator().Send(request);

            books.Should().HaveCount(withFilters ? 2 : 3);
            books.Should().ContainEquivalentOf(new BookLight
            {
                Id = 1,
                Name = "aaa",
                Photo = new byte[] { 1, 2, 3, 4, 5 },
                Year = 2001,
                Isbn = "111",
                Pages = 500,
                Author = "abc mno",
                Genres = "Fantasy",
                Rating = 10
            });
            books.Should().ContainEquivalentOf(new BookLight
            {
                Id = 2,
                Name = "bbb",
                Photo = new byte[] { 1, 2, 3, 4, 5 },
                Year = 2000,
                Isbn = "222",
                Pages = 150,
                Author = "abc mno, mno xyz",
                Genres = "Fantasy, Novel",
                Rating = 0
            });

            if (withFilters)
            {
                books.Select(x => x.Name).Should().BeInDescendingOrder();
                return;
            }

            books.Should().ContainEquivalentOf(new BookLight
            {
                Id = 3,
                Name = "ccc",
                Photo = new byte[] { 1, 2, 3, 4, 5 },
                Year = 2002,
                Isbn = "333",
                Pages = 300,
                Author = "mno xyz",
                Genres = "Novel",
                Rating = 7.5
            });
        }

        [Test]
        public async Task Successfully_delete_book()
        {
            var ctx = GetContext();
            var book = await ctx.Books.AddAsync(CreateBook());
            await ctx.SaveChangesAsync();

            (await ctx.Books.CountAsync()).Should().Be(1);
            await GetMediator().Send(new DeleteBook { Id = book.Entity.Id });

            (await ctx.Books.CountAsync()).Should().Be(0);
        }

        [Test]
        public void Delete_not_existing_should_do_nothing()
            => Assert.DoesNotThrowAsync(async () => await GetMediator().Send(new DeleteBook { Id = 1 }));

        [Test]
        public async Task Get_genres()
        {
            var ctx = GetContext();
            var genres1 = new List<Genre> {new Genre { Id = 1, Name = "genre 1" }, new Genre {Id = 2, Name = "genre 2"}};
            await ctx.Genres.AddRangeAsync(genres1);
            await ctx.SaveChangesAsync();

            var genres2 = await GetMediator().Send(new GetGenres());

            genres2.Should().BeEquivalentTo(genres1);
        }
    }
}

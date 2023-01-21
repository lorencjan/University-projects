using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BooksService.Domain.Entities;
using BooksService.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BooksService.Application.Books
{
    public class UpdateBook : IRequest
    {
        public Book Book { get; set; }
    }

    public class UpdateBookHandler : AsyncRequestHandler<UpdateBook>
    {
        private readonly BooksDbContext _booksDbContext;

        public UpdateBookHandler(BooksDbContext booksDbContext) => _booksDbContext = booksDbContext;

        protected override async Task Handle(UpdateBook request, CancellationToken cancellationToken)
        {
            var book = request.Book;

            var toUpdate = await _booksDbContext.Books
                .Include(x => x.Authors)
                .Include(x => x.Genres)
                .SingleAsync(x => x.Id == book.Id, cancellationToken);
            
            toUpdate.Name = book.Name;
            toUpdate.Photo = book.Photo;
            toUpdate.Year = book.Year;
            toUpdate.Isbn = book.Isbn;
            toUpdate.Pages = book.Pages;
            toUpdate.Description = book.Description;

            //synchronize collections as with M:N the tables are not affected by changed collections
            book.Authors ??= new List<AuthorBook>();
            var authorsToRemove = toUpdate.Authors.Where(inDb => book.Authors.All(update => update.AuthorId != inDb.AuthorId));
            var authorsToAdd = book.Authors.Where(update => toUpdate.Authors.All(inDb => update.AuthorId != inDb.AuthorId));
            _booksDbContext.AuthorBooks.RemoveRange(authorsToRemove);
            await _booksDbContext.AuthorBooks.AddRangeAsync(authorsToAdd, cancellationToken);

            book.Genres ??= new List<GenreBook>();
            var genresToRemove = toUpdate.Genres.Where(inDb => book.Genres.All(update => update.GenreId != inDb.GenreId));
            var genresToAdd = book.Genres.Where(update => toUpdate.Genres.All(inDb => update.GenreId != inDb.GenreId));
            _booksDbContext.GenreBooks.RemoveRange(genresToRemove);
            await _booksDbContext.GenreBooks.AddRangeAsync(genresToAdd, cancellationToken);

            await _booksDbContext.SaveChangesAsync(cancellationToken);
        }
    }
}

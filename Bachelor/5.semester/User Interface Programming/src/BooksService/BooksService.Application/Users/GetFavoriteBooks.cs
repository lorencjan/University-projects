using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BooksService.Domain;
using BooksService.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BooksService.Application.Users
{
    public class GetFavoriteBooks : IRequest<List<BookLight>>
    {
        public int UserId { get; set; }
    }

    public class GetFavoriteBooksHandler : IRequestHandler<GetFavoriteBooks, List<BookLight>>
    {
        private readonly BooksDbContext _booksDbContext;

        public GetFavoriteBooksHandler(BooksDbContext booksDbContext) => _booksDbContext = booksDbContext;

        public async Task<List<BookLight>> Handle(GetFavoriteBooks request, CancellationToken cancellationToken)
        {
            var books = await _booksDbContext.UsersFavoriteBooks
                .Where(x => x.UserId == request.UserId)
                .Include(x => x.Book).ThenInclude(x => x.Genres).ThenInclude(x => x.Genre)
                .Include(x => x.Book).ThenInclude(x => x.Authors).ThenInclude(x => x.Author)
                .Include(x => x.Book).ThenInclude(x => x.Ratings)
                .Select(x => x.Book)
                .ToListAsync(cancellationToken);

            return books.Select(x => new BookLight
            {
                Id = x.Id,
                Name = x.Name,
                Photo = x.Photo,
                Author = string.Join(", ", x.Authors.Select(a => $"{a.Author.FirstName} {a.Author.LastName}").ToList()),
                Year = x.Year,
                Isbn = x.Isbn,
                Pages = x.Pages,
                Genres = string.Join(", ", x.Genres.Select(g => g.Genre.Name).ToList()),
                Rating = x.Ratings.Count == 0 ? 0 : x.Ratings.Average(r => r.Number)
            }).ToList();
        }
    }
}

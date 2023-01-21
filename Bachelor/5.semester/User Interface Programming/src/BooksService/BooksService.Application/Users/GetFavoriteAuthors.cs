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
    public class GetFavoriteAuthors : IRequest<List<AuthorLight>>
    {
        public int UserId { get; set; }
    }

    public class GetFavoriteAuthorsHandler : IRequestHandler<GetFavoriteAuthors, List<AuthorLight>>
    {
        private readonly BooksDbContext _booksDbContext;

        public GetFavoriteAuthorsHandler(BooksDbContext booksDbContext) => _booksDbContext = booksDbContext;

        public async Task<List<AuthorLight>> Handle(GetFavoriteAuthors request, CancellationToken cancellationToken)
        {
            var authors = await _booksDbContext.UsersFavoriteAuthors
                .Where(x => x.UserId == request.UserId)
                .Include(x => x.Author).ThenInclude(x => x.Books).ThenInclude(x => x.Book)
                .Include(x => x.Author).ThenInclude(x => x.Ratings)
                .Select(x => x.Author)
                .ToListAsync(cancellationToken);

            return authors.Select(x => new AuthorLight
            {
                Id = x.Id,
                FirstName = x.FirstName,
                LastName = x.LastName,
                Photo = x.Photo,
                Country = x.Country,
                BirthDate = x.BirthDate,
                BookCount = x.Books.Count,
                Rating = x.Ratings.Count == 0 ? 0 : x.Ratings.Average(r => r.Number)
            }).ToList();
        }
    }
}

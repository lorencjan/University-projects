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
    public class GetGenres : IRequest<List<Genre>>
    {}

    public class GetGenresHandler : IRequestHandler<GetGenres, List<Genre>>
    {
        private readonly BooksDbContext _booksDbContext;

        public GetGenresHandler(BooksDbContext booksDbContext) => _booksDbContext = booksDbContext;

        public async Task<List<Genre>> Handle(GetGenres request, CancellationToken cancellationToken)
            => await _booksDbContext.Genres
                .OrderBy(x => x)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
    }
}

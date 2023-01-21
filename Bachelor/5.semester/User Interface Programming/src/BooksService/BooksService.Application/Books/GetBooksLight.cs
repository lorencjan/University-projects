using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BooksService.Application.Books.Services;
using BooksService.Common;
using BooksService.Domain;
using BooksService.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BooksService.Application.Books
{
    public class GetBooksLight : IRequest<List<BookLight>>
    {
        public BookFilter Filter { get; set; }
        public SortBooksBy? SortBooksBy { get; set; }
        public SortOrdering? SortOrdering { get; set; }
    }

    public class GetBooksLightHandler : IRequestHandler<GetBooksLight, List<BookLight>>
    {
        private readonly BooksDbContext _booksDbContext;

        public GetBooksLightHandler(BooksDbContext booksDbContext) => _booksDbContext = booksDbContext;

        public async Task<List<BookLight>> Handle(GetBooksLight request, CancellationToken cancellationToken)
        {
            var query = await BooksQueryBuilder
                .Build(_booksDbContext, BookIncludeFeatures.All, request.Filter, request.SortBooksBy, request.SortOrdering)
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            if (!string.IsNullOrWhiteSpace(request.Filter?.Author))
            {
                query = query.Where(x => x.Authors.Any(a => $"{a.Author.FirstName} {a.Author.LastName}".IndexOf(request.Filter.Author, StringComparison.InvariantCultureIgnoreCase) != -1)).ToList();
            }

            return query.Select(x => new BookLight
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

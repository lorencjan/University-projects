using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BooksService.Application.Authors.Services;
using BooksService.Common;
using BooksService.Domain;
using BooksService.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BooksService.Application.Authors
{
    public class GetAuthorsLight : IRequest<List<AuthorLight>>
    {
        public AuthorFilter Filter { get; set; }
        public SortAuthorsBy? SortAuthorsBy { get; set; }
        public SortOrdering? SortOrdering { get; set; }
    }

    public class GetAuthorsLightHandler : IRequestHandler<GetAuthorsLight, List<AuthorLight>>
    {
        private readonly BooksDbContext _booksDbContext;

        public GetAuthorsLightHandler(BooksDbContext booksDbContext) => _booksDbContext = booksDbContext;

        public async Task<List<AuthorLight>> Handle(GetAuthorsLight request, CancellationToken cancellationToken)
        {
            var query = await AuthorsQueryBuilder
                .Build(_booksDbContext, AuthorIncludeFeatures.JoinBooks | AuthorIncludeFeatures.Ratings, request.Filter, request.SortAuthorsBy, request.SortOrdering)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
            if (!string.IsNullOrWhiteSpace(request.Filter?.Name))
            {
                query = query.Where(x => $"{x.FirstName} {x.LastName}".IndexOf(request.Filter.Name, StringComparison.InvariantCultureIgnoreCase) != -1).ToList();
            }
            return query.Select(x => new AuthorLight
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

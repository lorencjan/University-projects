using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BooksService.Application.Authors.Services;
using BooksService.Common;
using BooksService.Domain.Entities;
using BooksService.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BooksService.Application.Authors
{
    public class GetAuthors : IRequest<List<Author>>
    {
        public AuthorFilter Filter { get; set; }
        public AuthorIncludeFeatures? IncludeFeatures { get; set; }
        public SortAuthorsBy? SortAuthorsBy { get; set; }
        public SortOrdering? SortOrdering { get; set; }
    }

    public class GetAuthorsHandler : IRequestHandler<GetAuthors, List<Author>>
    {
        private readonly BooksDbContext _booksDbContext;

        public GetAuthorsHandler(BooksDbContext booksDbContext) => _booksDbContext = booksDbContext;

        public async Task<List<Author>> Handle(GetAuthors request, CancellationToken cancellationToken)
        {
            var authors = await AuthorsQueryBuilder
                .Build(_booksDbContext, request.IncludeFeatures, request.Filter, request.SortAuthorsBy, request.SortOrdering)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
            if (!string.IsNullOrWhiteSpace(request.Filter?.Name))
            {
                authors = authors.Where(x => $"{x.FirstName} {x.LastName}".IndexOf(request.Filter.Name, StringComparison.InvariantCultureIgnoreCase) != -1).ToList();
            }
            return authors;
        }
    }
}

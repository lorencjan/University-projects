using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BooksService.Application.Books.Services;
using BooksService.Common;
using BooksService.Domain.Entities;
using BooksService.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BooksService.Application.Books
{
    public class GetBooks : IRequest<List<Book>>
    {
        public BookFilter Filter { get; set; }
        public BookIncludeFeatures? IncludeFeatures { get; set; }
        public SortBooksBy? SortBooksBy { get; set; }
        public SortOrdering? SortOrdering { get; set; }
    }

    public class GetBooksHandler : IRequestHandler<GetBooks, List<Book>>
    {
        private readonly BooksDbContext _booksDbContext;

        public GetBooksHandler(BooksDbContext booksDbContext) => _booksDbContext = booksDbContext;

        public async Task<List<Book>> Handle(GetBooks request, CancellationToken cancellationToken)
        {
            var books = await BooksQueryBuilder
                .Build(_booksDbContext, request.IncludeFeatures, request.Filter, request.SortBooksBy, request.SortOrdering)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
            if (!string.IsNullOrWhiteSpace(request.Filter?.Author))
            {
                books = books.Where(x => x.Authors.Any(a => $"{a.Author.FirstName} {a.Author.LastName}".IndexOf(request.Filter.Author, StringComparison.InvariantCultureIgnoreCase) != -1)).ToList();
            }
            return books;
        }
    }
}

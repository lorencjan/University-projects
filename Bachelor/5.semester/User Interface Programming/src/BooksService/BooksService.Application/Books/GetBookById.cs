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
    public class GetBookById : IRequest<Book>
    {
        public int Id { get; set; }
        public BookIncludeFeatures? IncludeFeatures { get; set; }
    }

    public class GetBookByIdHandler : IRequestHandler<GetBookById, Book>
    {
        private readonly BooksDbContext _booksDbContext;

        public GetBookByIdHandler(BooksDbContext booksDbContext) => _booksDbContext = booksDbContext;

        public async Task<Book> Handle(GetBookById request, CancellationToken cancellationToken)
            => await BooksQueryBuilder
                .Build(_booksDbContext, request.IncludeFeatures)
                .AsNoTracking()
                .SingleOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
    }
}

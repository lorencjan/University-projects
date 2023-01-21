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
    public class GetAuthorById : IRequest<Author>
    {
        public int Id { get; set; }
        public AuthorIncludeFeatures? IncludeFeatures { get; set; }
    }

    public class GetAuthorByIdHandler : IRequestHandler<GetAuthorById, Author>
    {
        private readonly BooksDbContext _booksDbContext;

        public GetAuthorByIdHandler(BooksDbContext booksDbContext) => _booksDbContext = booksDbContext;

        public async Task<Author> Handle(GetAuthorById request, CancellationToken cancellationToken)
            => await AuthorsQueryBuilder
                .Build(_booksDbContext, request.IncludeFeatures)
                .AsNoTracking()
                .SingleOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
    }
}

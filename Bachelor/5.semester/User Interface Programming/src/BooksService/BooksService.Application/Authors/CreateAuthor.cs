using System.Threading;
using System.Threading.Tasks;
using BooksService.Domain.Entities;
using BooksService.Persistence;
using MediatR;

namespace BooksService.Application.Authors
{
    public class CreateAuthor : IRequest<int>
    {
        public Author Author { get; set; }
    }

    public class CreateAuthorHandler : IRequestHandler<CreateAuthor, int>
    {
        private readonly BooksDbContext _booksDbContext;

        public CreateAuthorHandler(BooksDbContext booksDbContext) => _booksDbContext = booksDbContext;

        public async Task<int> Handle(CreateAuthor request, CancellationToken cancellationToken)
        {
            var book = await _booksDbContext.Authors.AddAsync(request.Author, cancellationToken);
            await _booksDbContext.SaveChangesAsync(cancellationToken);
            return book.Entity.Id;
        }
    }
}
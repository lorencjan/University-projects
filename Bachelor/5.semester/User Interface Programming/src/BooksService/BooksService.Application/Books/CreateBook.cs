using System.Threading;
using System.Threading.Tasks;
using BooksService.Domain.Entities;
using BooksService.Persistence;
using MediatR;

namespace BooksService.Application.Books
{
    public class CreateBook : IRequest<int>
    {
        public Book Book { get; set; }
    }

    public class CreateBookHandler : IRequestHandler<CreateBook, int>
    {
        private readonly BooksDbContext _booksDbContext;

        public CreateBookHandler(BooksDbContext booksDbContext) => _booksDbContext = booksDbContext;

        public async Task<int> Handle(CreateBook request, CancellationToken cancellationToken)
        {
            var book = await _booksDbContext.Books.AddAsync(request.Book, cancellationToken);
            await _booksDbContext.SaveChangesAsync(cancellationToken);
            return book.Entity.Id;
        }
    }
}
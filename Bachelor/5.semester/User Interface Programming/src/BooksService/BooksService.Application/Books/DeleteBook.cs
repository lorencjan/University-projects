using System.Threading;
using System.Threading.Tasks;
using BooksService.Persistence;
using MediatR;

namespace BooksService.Application.Books
{
    public class DeleteBook : IRequest
    {
        public int Id { get; set; }
    }

    public class DeleteBookHandler : AsyncRequestHandler<DeleteBook>
    {
        private readonly BooksDbContext _booksDbContext;
        
        public DeleteBookHandler(BooksDbContext booksDbContext) => _booksDbContext = booksDbContext;

        protected override async Task Handle(DeleteBook request, CancellationToken cancellationToken)
        {
            var book = await _booksDbContext.Books.FindAsync(request.Id);
            if (book == null)
            {
                return;
            }

            _booksDbContext.Books.Remove(book);
            await _booksDbContext.SaveChangesAsync(cancellationToken);
        }
    }
}

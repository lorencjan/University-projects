using System.Threading;
using System.Threading.Tasks;
using BooksService.Persistence;
using MediatR;

namespace BooksService.Application.Authors
{
    public class DeleteAuthor : IRequest
    {
        public int Id { get; set; }
    }

    public class DeleteAuthorHandler : AsyncRequestHandler<DeleteAuthor>
    {
        private readonly BooksDbContext _booksDbContext;
        
        public DeleteAuthorHandler(BooksDbContext booksDbContext) => _booksDbContext = booksDbContext;

        protected override async Task Handle(DeleteAuthor request, CancellationToken cancellationToken)
        {
            var author = await _booksDbContext.Authors.FindAsync(request.Id);
            if (author == null)
            {
                return;
            }

            _booksDbContext.Authors.Remove(author);
            await _booksDbContext.SaveChangesAsync(cancellationToken);
        }
    }
}

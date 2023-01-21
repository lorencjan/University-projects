using System.Threading;
using System.Threading.Tasks;
using BooksService.Persistence;
using MediatR;

namespace BooksService.Application.Favorites
{
    public class RemoveFavoriteBook : IRequest
    {
        public int UserId { get; set; }
        public int BookId { get; set; }
    }

    public class RemoveFavoriteBookHandler : AsyncRequestHandler<RemoveFavoriteBook>
    {
        private readonly BooksDbContext _booksDbContext;
        
        public RemoveFavoriteBookHandler(BooksDbContext booksDbContext) =>  _booksDbContext = booksDbContext;

        protected override async Task Handle(RemoveFavoriteBook request, CancellationToken cancellationToken)
        {
            var favorite = await _booksDbContext.UsersFavoriteBooks.FindAsync(request.UserId, request.BookId);
            if (favorite == null)
            {
                return;
            }

            _booksDbContext.UsersFavoriteBooks.Remove(favorite);
            await _booksDbContext.SaveChangesAsync(cancellationToken);
        }
    }
}

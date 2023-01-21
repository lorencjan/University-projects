using System.Threading;
using System.Threading.Tasks;
using BooksService.Persistence;
using MediatR;

namespace BooksService.Application.Favorites
{
    public class RemoveFavoriteAuthor : IRequest
    {
        public int UserId { get; set; }
        public int AuthorId { get; set; }
    }

    public class RemoveFavoriteAuthorHandler : AsyncRequestHandler<RemoveFavoriteAuthor>
    {
        private readonly BooksDbContext _booksDbContext;
        
        public RemoveFavoriteAuthorHandler(BooksDbContext booksDbContext) =>  _booksDbContext = booksDbContext;

        protected override async Task Handle(RemoveFavoriteAuthor request, CancellationToken cancellationToken)
        {
            var favorite = await _booksDbContext.UsersFavoriteAuthors.FindAsync(request.UserId, request.AuthorId);
            if (favorite == null)
            {
                return;
            }

            _booksDbContext.UsersFavoriteAuthors.Remove(favorite);
            await _booksDbContext.SaveChangesAsync(cancellationToken);
        }
    }
}

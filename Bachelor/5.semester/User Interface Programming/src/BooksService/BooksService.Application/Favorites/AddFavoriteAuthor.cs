using System.Threading;
using System.Threading.Tasks;
using BooksService.Domain.Entities;
using BooksService.Persistence;
using MediatR;

namespace BooksService.Application.Favorites
{
    public class AddFavoriteAuthor : IRequest
    {
        public int UserId { get; set; }
        public int AuthorId { get; set; }
    }

    public class AddFavoriteAuthorHandler : AsyncRequestHandler<AddFavoriteAuthor>
    {
        private readonly BooksDbContext _booksDbContext;
        
        public AddFavoriteAuthorHandler(BooksDbContext booksDbContext) =>  _booksDbContext = booksDbContext;

        protected override async Task Handle(AddFavoriteAuthor request, CancellationToken cancellationToken)
        {
            var favorite = await _booksDbContext.UsersFavoriteAuthors.FindAsync(request.UserId, request.AuthorId);
            if (favorite != null)
            {
                return;
            }

            await _booksDbContext.UsersFavoriteAuthors.AddAsync(new UserFavoriteAuthor
            {
                UserId = request.UserId,
                AuthorId = request.AuthorId
            },
            cancellationToken);
            await _booksDbContext.SaveChangesAsync(cancellationToken);
        }
    }
}

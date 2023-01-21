using System.Threading;
using System.Threading.Tasks;
using BooksService.Domain.Entities;
using BooksService.Persistence;
using MediatR;

namespace BooksService.Application.Favorites
{
    public class AddFavoriteBook : IRequest
    {
        public int UserId { get; set; }
        public int BookId { get; set; }
    }

    public class AddFavoriteBookHandler : AsyncRequestHandler<AddFavoriteBook>
    {
        private readonly BooksDbContext _booksDbContext;
        
        public AddFavoriteBookHandler(BooksDbContext booksDbContext) =>  _booksDbContext = booksDbContext;

        protected override async Task Handle(AddFavoriteBook request, CancellationToken cancellationToken)
        {
            var favorite = await _booksDbContext.UsersFavoriteBooks.FindAsync(request.UserId, request.BookId);
            if (favorite != null)
            {
                return;
            }

            await _booksDbContext.UsersFavoriteBooks.AddAsync(new UserFavoriteBook
            {
                UserId = request.UserId,
                BookId = request.BookId
            },
            cancellationToken);
            await _booksDbContext.SaveChangesAsync(cancellationToken);
        }
    }
}

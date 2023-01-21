using System.Threading;
using System.Threading.Tasks;
using BooksService.Persistence;
using MediatR;

namespace BooksService.Application.Ratings
{
    public class DeleteBookRating : IRequest
    {
        public int Id { get; set; }
    }

    public class DeleteBookRatingHandler : AsyncRequestHandler<DeleteBookRating>
    {
        private readonly BooksDbContext _booksDbContext;

        public DeleteBookRatingHandler(BooksDbContext booksDbContext) => _booksDbContext = booksDbContext;

        protected override async Task Handle(DeleteBookRating request, CancellationToken cancellationToken)
        {
            var rating = await _booksDbContext.BookRatings.FindAsync(request.Id);
            if (rating == null)
            {
                return;
            }

            _booksDbContext.BookRatings.Remove(rating);
            await _booksDbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
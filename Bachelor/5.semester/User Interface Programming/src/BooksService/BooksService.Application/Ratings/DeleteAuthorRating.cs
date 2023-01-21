using System.Threading;
using System.Threading.Tasks;
using BooksService.Persistence;
using MediatR;

namespace BooksService.Application.Ratings
{
    public class DeleteAuthorRating : IRequest
    {
        public int Id { get; set; }
    }

    public class DeleteAuthorRatingHandler : AsyncRequestHandler<DeleteAuthorRating>
    {
        private readonly BooksDbContext _booksDbContext;

        public DeleteAuthorRatingHandler(BooksDbContext booksDbContext) => _booksDbContext = booksDbContext;

        protected override async Task Handle(DeleteAuthorRating request, CancellationToken cancellationToken)
        {
            var rating = await _booksDbContext.AuthorRatings.FindAsync(request.Id);
            if (rating == null)
            {
                return;
            }

            _booksDbContext.AuthorRatings.Remove(rating);
            await _booksDbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
using System.Threading;
using System.Threading.Tasks;
using BooksService.Persistence;
using MediatR;

namespace BooksService.Application.Ratings
{
    public class UpdateBookRating : IRequest
    {
        public int Id { get; set; }
        public int Number { get; set; }
        public string Text { get; set; }
    }

    public class UpdateBookRatingHandler : AsyncRequestHandler<UpdateBookRating>
    {
        private readonly BooksDbContext _booksDbContext;

        public UpdateBookRatingHandler(BooksDbContext booksDbContext) => _booksDbContext = booksDbContext;

        protected override async Task Handle(UpdateBookRating request, CancellationToken cancellationToken)
        {
            var toUpdate = await _booksDbContext.BookRatings.FindAsync(request.Id);

            toUpdate.Number = request.Number;
            toUpdate.Text = request.Text;

            await _booksDbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
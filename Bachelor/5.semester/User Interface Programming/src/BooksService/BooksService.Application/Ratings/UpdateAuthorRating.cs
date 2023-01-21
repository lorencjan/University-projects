using System.Threading;
using System.Threading.Tasks;
using BooksService.Persistence;
using MediatR;

namespace BooksService.Application.Ratings
{
    public class UpdateAuthorRating : IRequest
    {
        public int Id { get; set; }
        public int Number { get; set; }
        public string Text { get; set; }
    }

    public class UpdateAuthorRatingHandler : AsyncRequestHandler<UpdateAuthorRating>
    {
        private readonly BooksDbContext _booksDbContext;

        public UpdateAuthorRatingHandler(BooksDbContext booksDbContext) => _booksDbContext = booksDbContext;

        protected override async Task Handle(UpdateAuthorRating request, CancellationToken cancellationToken)
        {
            var toUpdate = await _booksDbContext.AuthorRatings.FindAsync(request.Id);

            toUpdate.Number = request.Number;
            toUpdate.Text = request.Text;

            await _booksDbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
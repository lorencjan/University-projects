using System.Threading;
using System.Threading.Tasks;
using BooksService.Persistence;
using MediatR;

namespace BooksService.Application.Feedbacks
{
    public class DeleteFeedback : IRequest
    {
        public int Id { get; set; }
    }

    public class DeleteFeedbackHandler : AsyncRequestHandler<DeleteFeedback>
    {
        private readonly BooksDbContext _booksDbContext;

        public DeleteFeedbackHandler(BooksDbContext booksDbContext) => _booksDbContext = booksDbContext;

        protected override async Task Handle(DeleteFeedback request, CancellationToken cancellationToken)
        {
            var feedback = await _booksDbContext.Feedbacks.FindAsync(request.Id);
            if (feedback == null)
            {
                return;
            }

            _booksDbContext.Feedbacks.Remove(feedback);
            await _booksDbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
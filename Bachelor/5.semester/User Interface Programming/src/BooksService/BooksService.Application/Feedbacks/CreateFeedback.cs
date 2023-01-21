using System;
using System.Threading;
using System.Threading.Tasks;
using BooksService.Domain.Entities;
using BooksService.Persistence;
using MediatR;

namespace BooksService.Application.Feedbacks
{
    public class CreateFeedback : IRequest<int>
    {
        public string Text { get; set; }
        public int UserId { get; set; }
    }

    public class CreateFeedbackHandler : IRequestHandler<CreateFeedback, int>
    {
        private readonly BooksDbContext _booksDbContext;

        public CreateFeedbackHandler(BooksDbContext booksDbContext) => _booksDbContext = booksDbContext;

        public async Task<int> Handle(CreateFeedback request, CancellationToken cancellationToken)
        {
            var created = await _booksDbContext.Feedbacks.AddAsync(new Feedback
            {
                Text = request.Text,
                UserId = request.UserId,
                Date = DateTime.Now
            }, cancellationToken);
            await _booksDbContext.SaveChangesAsync(cancellationToken);
            return created.Entity.Id;
        }
    }
}
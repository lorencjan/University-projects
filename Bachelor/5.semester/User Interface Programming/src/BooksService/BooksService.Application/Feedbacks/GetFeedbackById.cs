using System.Threading;
using System.Threading.Tasks;
using BooksService.Domain.Entities;
using BooksService.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BooksService.Application.Feedbacks
{
    public class GetFeedbackById : IRequest<Feedback>
    {
        public int Id { get; set; }
    }

    public class GetFeedbackByIdHandler : IRequestHandler<GetFeedbackById, Feedback>
    {
        private readonly BooksDbContext _booksDbContext;

        public GetFeedbackByIdHandler(BooksDbContext booksDbContext) => _booksDbContext = booksDbContext;

        public async Task<Feedback> Handle(GetFeedbackById request, CancellationToken cancellationToken)
            => await _booksDbContext.Feedbacks
                .Include(x => x.User)
                .AsNoTracking()
                .SingleOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
    }
}

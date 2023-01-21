using System.Threading;
using System.Threading.Tasks;
using BooksService.Application.Feedbacks;
using BooksService.Persistence;
using MediatR;

namespace BooksService.Application.NewsPosts
{
    public class DeleteNewsPost : IRequest
    {
        public int Id { get; set; }
    }

    public class DeleteNewsPostHandler : AsyncRequestHandler<DeleteNewsPost>
    {
        private readonly BooksDbContext _booksDbContext;

        public DeleteNewsPostHandler(BooksDbContext booksDbContext) => _booksDbContext = booksDbContext;

        protected override async Task Handle(DeleteNewsPost request, CancellationToken cancellationToken)
        {
            var newsPost = await _booksDbContext.NewsPosts.FindAsync(request.Id);
            if (newsPost == null)
            {
                return;
            }

            _booksDbContext.NewsPosts.Remove(newsPost);
            await _booksDbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
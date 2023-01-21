using System.Threading;
using System.Threading.Tasks;
using BooksService.Domain.Entities;
using BooksService.Persistence;
using MediatR;

namespace BooksService.Application.NewsPosts
{
    public class UpdateNewsPost : IRequest
    {
        public NewsPost NewsPost { get; set; }
    }

    public class UpdateNewsPostHandler : AsyncRequestHandler<UpdateNewsPost>
    {
        private readonly BooksDbContext _booksDbContext;

        public UpdateNewsPostHandler(BooksDbContext booksDbContext) => _booksDbContext = booksDbContext;

        protected override async Task Handle(UpdateNewsPost request, CancellationToken cancellationToken)
        {
            var update = request.NewsPost;

            var newsPost = await _booksDbContext.NewsPosts.FindAsync(update.Id);
            newsPost.Header = update.Header;
            newsPost.Text = update.Text;
            newsPost.Date = update.Date;
            await _booksDbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
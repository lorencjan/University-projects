using System;
using System.Threading;
using System.Threading.Tasks;
using BooksService.Domain.Entities;
using BooksService.Persistence;
using MediatR;

namespace BooksService.Application.NewsPosts
{
    public class CreateNewsPost : IRequest<int>
    {
        public string Header { get; set; }
        public string Text { get; set; }
    }

    public class CreateNewsPostHandler : IRequestHandler<CreateNewsPost, int>
    {
        private readonly BooksDbContext _booksDbContext;

        public CreateNewsPostHandler(BooksDbContext booksDbContext) => _booksDbContext = booksDbContext;

        public async Task<int> Handle(CreateNewsPost request, CancellationToken cancellationToken)
        {
            var created = await _booksDbContext.NewsPosts.AddAsync(new NewsPost
            {
                Header = request.Header,
                Text = request.Text,
                Date = DateTime.Now
            }, cancellationToken);
            await _booksDbContext.SaveChangesAsync(cancellationToken);
            return created.Entity.Id;
        }
    }
}
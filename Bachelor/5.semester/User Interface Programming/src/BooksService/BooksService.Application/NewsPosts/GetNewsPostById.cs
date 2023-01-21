using System.Threading;
using System.Threading.Tasks;
using BooksService.Domain.Entities;
using BooksService.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BooksService.Application.NewsPosts
{
    public class GetNewsPostById : IRequest<NewsPost>
    {
        public int Id { get; set; }
    }

    public class GetNewsPostByIdHandler : IRequestHandler<GetNewsPostById, NewsPost>
    {
        private readonly BooksDbContext _booksDbContext;

        public GetNewsPostByIdHandler(BooksDbContext booksDbContext) => _booksDbContext = booksDbContext;

        public async Task<NewsPost> Handle(GetNewsPostById request, CancellationToken cancellationToken)
            => await _booksDbContext.NewsPosts.AsNoTracking().SingleOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
    }
}

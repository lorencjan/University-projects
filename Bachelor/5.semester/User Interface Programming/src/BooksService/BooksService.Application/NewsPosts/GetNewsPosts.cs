using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BooksService.Application.NewsPosts.Services;
using BooksService.Common;
using BooksService.Domain.Entities;
using BooksService.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BooksService.Application.NewsPosts
{
    public class GetNewsPosts : IRequest<List<NewsPost>>
    {
        /// <summary>Latest N posts</summary>
        public int? Count { get; set; }
        public NewsPostFilter Filter { get; set; }
        public SortNewsPostsBy? SortNewsPostsBy { get; set; }
        public SortOrdering? SortOrdering { get; set; }
    }

    public class GetNewsPostsHandler : IRequestHandler<GetNewsPosts, List<NewsPost>>
    {
        private readonly BooksDbContext _booksDbContext;

        public GetNewsPostsHandler(BooksDbContext booksDbContext) => _booksDbContext = booksDbContext;

        public async Task<List<NewsPost>> Handle(GetNewsPosts request, CancellationToken cancellationToken)
        {
            var query = NewsPostsQueryBuilder.Build(_booksDbContext, request.Filter, request.SortNewsPostsBy, request.SortOrdering);

            if (request.Count.HasValue)
                query = query.Take(request.Count.Value);
            
            return await query.AsNoTracking().ToListAsync(cancellationToken);
        }
    }
}

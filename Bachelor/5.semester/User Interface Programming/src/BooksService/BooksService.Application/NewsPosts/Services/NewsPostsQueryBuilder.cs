using System;
using System.Linq;
using BooksService.Common;
using BooksService.Domain.Entities;
using BooksService.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BooksService.Application.NewsPosts.Services
{
    public static class NewsPostsQueryBuilder
    {
        public static IQueryable<NewsPost> Build(BooksDbContext context, NewsPostFilter filter = null, SortNewsPostsBy? sortBy = null, SortOrdering? sortOrdering = null)
        {
            var query = context.NewsPosts.AsQueryable();
            AddFilter(ref query, filter);
            AddSort(ref query, sortBy, sortOrdering);
            return query;
        }

        private static void AddFilter(ref IQueryable<NewsPost> query, NewsPostFilter filter)
        {
            if (filter == null)
                return;
            
            if (!string.IsNullOrWhiteSpace(filter.Header))
                query = query.Where(x => EF.Functions.Like(x.Header, $"%{filter.Header}%"));
        }

        private static void AddSort(ref IQueryable<NewsPost> query, SortNewsPostsBy? sort, SortOrdering? order)
        {
            sort ??= SortNewsPostsBy.Date;
            order ??= SortOrdering.Descending;
            query = sort switch
            {
                SortNewsPostsBy.Header => order == SortOrdering.Ascending ? query.OrderBy(x => x.Header) : query.OrderByDescending(x => x.Header),
                SortNewsPostsBy.Date => order == SortOrdering.Ascending ? query.OrderBy(x => x.Date) : query.OrderByDescending(x => x.Date),
                _ => throw new ArgumentOutOfRangeException(nameof(sort), sort, null)
            };
        }
    }
}

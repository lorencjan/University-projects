using System;
using System.Linq;
using BooksService.Common;
using BooksService.Domain.Entities;
using BooksService.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BooksService.Application.Books.Services
{
    public static class BooksQueryBuilder
    {
        public static IQueryable<Book> Build(BooksDbContext context, BookIncludeFeatures? includes = null, BookFilter filter = null, SortBooksBy? sortBy = null, SortOrdering? sortOrdering = null)
        {
            var query = context.Books.AsQueryable();
            IncludeFeatures(ref query, includes);
            AddFilter(ref query, filter);
            AddSort(ref query, sortBy, sortOrdering);
            return query;
        }

        private static void IncludeFeatures(ref IQueryable<Book> query, BookIncludeFeatures? includes)
        {
            if (!includes.HasValue)
                return;

            if (includes.Value.HasFlag(BookIncludeFeatures.Authors))
                query = query.Include(x => x.Authors).ThenInclude(x => x.Author);
            if (includes.Value.HasFlag(BookIncludeFeatures.Genres))
                query = query.Include(x => x.Genres).ThenInclude(x => x.Genre);
            if (includes.Value.HasFlag(BookIncludeFeatures.Ratings))
                query = query.Include(x => x.Ratings).ThenInclude(x => x.User);
        }

        private static void AddFilter(ref IQueryable<Book> query, BookFilter filter)
        {
            if (filter == null)
                return;

            if(filter.Year.HasValue)
                query = query.Where(x => x.Year == filter.Year.Value);

            if (!string.IsNullOrWhiteSpace(filter.Name))
                query = query.Where(x => EF.Functions.Like(x.Name, $"%{filter.Name}%"));
            if (!string.IsNullOrWhiteSpace(filter.Isbn))
                query = query.Where(x => EF.Functions.Like(x.Isbn, $"%{filter.Isbn}%"));

            if (!string.IsNullOrWhiteSpace(filter.Genre))
            {
                query = query
                    .Include(x => x.Genres)
                    .ThenInclude(x => x.Genre)
                    .Where(x => x.Genres.Any(g => EF.Functions.Like(g.Genre.Name, $"%{filter.Genre}%")));
            }

            // we want to filter the author by both first and lastname -> cannot be converted to sql
            // ... so just get it and than filtr this on client side
            if (!string.IsNullOrWhiteSpace(filter.Author))
            {
                query = query.Include(x => x.Authors).ThenInclude(x => x.Author);
            }
        }

        private static void AddSort(ref IQueryable<Book> query, SortBooksBy? sort, SortOrdering? order)
        {
            sort ??= SortBooksBy.Name;
            order ??= SortOrdering.Ascending;
            query = sort switch
            {
                SortBooksBy.Name => order == SortOrdering.Ascending ? query.OrderBy(x => x.Name) : query.OrderByDescending(x => x.Name),
                SortBooksBy.Year => order == SortOrdering.Ascending ? query.OrderBy(x => x.Year) : query.OrderByDescending(x => x.Year),
                SortBooksBy.Pages => order == SortOrdering.Ascending ? query.OrderBy(x => x.Pages) : query.OrderByDescending(x => x.Pages),
                SortBooksBy.Rating => order == SortOrdering.Ascending
                    ? query.Include(x => x.Ratings).OrderBy(x => x.Ratings.Count == 0 ? 0 : x.Ratings.Average(r => r.Number))
                    : query.Include(x => x.Ratings).OrderByDescending(x => x.Ratings.Count == 0 ? 0 : x.Ratings.Average(r => r.Number)),
                _ => throw new ArgumentOutOfRangeException(nameof(sort), sort, null)
            };
        }
    }
}

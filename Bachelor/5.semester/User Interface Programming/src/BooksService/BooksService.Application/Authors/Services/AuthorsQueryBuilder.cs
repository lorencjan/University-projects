using System;
using System.Linq;
using BooksService.Common;
using BooksService.Domain.Entities;
using BooksService.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BooksService.Application.Authors.Services
{
    public static class AuthorsQueryBuilder
    {
        public static IQueryable<Author> Build(BooksDbContext context, AuthorIncludeFeatures? includes = null, AuthorFilter filter = null, SortAuthorsBy? sortBy = null, SortOrdering? sortOrdering = null)
        {
            var query = context.Authors.AsQueryable();
            IncludeFeatures(ref query, includes);
            AddFilter(ref query, filter);
            AddSort(ref query, sortBy, sortOrdering);
            return query;
        }

        private static void IncludeFeatures(ref IQueryable<Author> query, AuthorIncludeFeatures? includes)
        {
            if (!includes.HasValue)
                return;

            if (includes.Value.HasFlag(AuthorIncludeFeatures.Books))
                query = query.Include(x => x.Books).ThenInclude(x => x.Book);
            else if (includes.Value.HasFlag(AuthorIncludeFeatures.JoinBooks))
                query = query.Include(x => x.Books);
            if (includes.Value.HasFlag(AuthorIncludeFeatures.Ratings))
                query = query.Include(x => x.Ratings).ThenInclude(x => x.User);
        }

        private static void AddFilter(ref IQueryable<Author> query, AuthorFilter filter)
        {
            if (filter == null)
                return;

            if (!string.IsNullOrWhiteSpace(filter.Country))
                query = query.Where(x => EF.Functions.Like(x.Country, $"%{filter.Country}%"));
        }

        private static void AddSort(ref IQueryable<Author> query, SortAuthorsBy? sort, SortOrdering? order)
        {
            sort ??= SortAuthorsBy.FirstName;
            order ??= SortOrdering.Ascending;
            query = sort switch
            {
                SortAuthorsBy.FirstName => order == SortOrdering.Ascending ? query.OrderBy(x => x.FirstName).ThenBy(x => x.LastName) : query.OrderByDescending(x => x.FirstName).ThenByDescending(x => x.LastName),
                SortAuthorsBy.LastName => order == SortOrdering.Ascending ? query.OrderBy(x => x.LastName).ThenBy(x => x.FirstName) : query.OrderByDescending(x => x.LastName).ThenByDescending(x => x.FirstName),
                SortAuthorsBy.Country => order == SortOrdering.Ascending ? query.OrderBy(x => x.Country) : query.OrderByDescending(x => x.Country),
                SortAuthorsBy.BirthDate => order == SortOrdering.Ascending ? query.OrderBy(x => x.BirthDate) : query.OrderByDescending(x => x.BirthDate),
                SortAuthorsBy.NumberOfBooks => order == SortOrdering.Ascending ? query.Include(x => x.Books).OrderBy(x => x.Books.Count) : query.Include(x => x.Books).OrderByDescending(x => x.Books.Count),
                SortAuthorsBy.Rating => order == SortOrdering.Ascending
                    ? query.Include(x => x.Ratings).OrderBy(x => x.Ratings.Count == 0 ? 0 : x.Ratings.Average(r => r.Number))
                    : query.Include(x => x.Ratings).OrderByDescending(x => x.Ratings.Count == 0 ? 0 : x.Ratings.Average(r => r.Number)),
                _ => throw new ArgumentOutOfRangeException(nameof(sort), sort, null)
            };
        }
    }
}

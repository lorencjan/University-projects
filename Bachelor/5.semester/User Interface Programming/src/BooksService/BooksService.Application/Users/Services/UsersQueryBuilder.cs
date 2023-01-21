using System;
using System.Linq;
using BooksService.Common;
using BooksService.Domain.Entities;
using BooksService.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BooksService.Application.Users.Services
{
    public static class UsersQueryBuilder
    {
        public static IQueryable<User> Build(BooksDbContext context, UserIncludeFeatures? includes = null, string filter = null, SortUsersBy? sortBy = null, SortOrdering? sortOrdering = null)
        {
            var query = context.Users.AsQueryable();
            IncludeFeatures(ref query, includes);
            AddFilter(ref query, filter);
            AddSort(ref query, sortBy, sortOrdering);
            return query;
        }

        private static void IncludeFeatures(ref IQueryable<User> query, UserIncludeFeatures? includes)
        {
            if (!includes.HasValue)
                return;

            if (includes.Value.HasFlag(UserIncludeFeatures.AuthorRatings))
                query = query.Include(x => x.AuthorRatings);
            if (includes.Value.HasFlag(UserIncludeFeatures.BookRatings))
                query = query.Include(x => x.BookRatings);
            if (includes.Value.HasFlag(UserIncludeFeatures.Feedbacks))
                query = query.Include(x => x.Feedbacks);
        }

        private static void AddFilter(ref IQueryable<User> query, string filter)
        {
            if (!string.IsNullOrWhiteSpace(filter))
                query = query.Where(x => EF.Functions.Like(x.Login, $"%{filter}%"));
        }

        private static void AddSort(ref IQueryable<User> query, SortUsersBy? sort, SortOrdering? order)
        {
            sort ??= SortUsersBy.Login;
            order ??= SortOrdering.Ascending;
            query = sort switch
            {
                SortUsersBy.Login => order == SortOrdering.Ascending ? query.OrderBy(x => x.Login) : query.OrderByDescending(x => x.Login),
                SortUsersBy.Feedbacks => order == SortOrdering.Ascending ? query.Include(x => x.Feedbacks).OrderBy(x => x.Feedbacks.Count) : query.Include(x => x.Feedbacks).OrderByDescending(x => x.Feedbacks.Count),
                SortUsersBy.BookRatings => order == SortOrdering.Ascending ? query.Include(x => x.BookRatings).OrderBy(x => x.BookRatings.Count) : query.Include(x => x.BookRatings).OrderByDescending(x => x.BookRatings.Count),
                SortUsersBy.AuthorRatings => order == SortOrdering.Ascending ? query.Include(x => x.AuthorRatings).OrderBy(x => x.AuthorRatings.Count) : query.Include(x => x.AuthorRatings).OrderByDescending(x => x.AuthorRatings.Count),
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}

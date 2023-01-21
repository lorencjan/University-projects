using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BooksService.Application.Users.Services;
using BooksService.Common;
using BooksService.Domain;
using BooksService.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BooksService.Application.Users
{
    public class GetUsersLight : IRequest<List<UserLight>>
    {
        public string LoginFilter { get; set; }
        public SortUsersBy? SortUsersBy { get; set; }
        public SortOrdering? SortOrdering { get; set; }
    }

    public class GetUsersLightHandler : IRequestHandler<GetUsersLight, List<UserLight>>
    {
        private readonly BooksDbContext _booksDbContext;

        public GetUsersLightHandler(BooksDbContext booksDbContext) => _booksDbContext = booksDbContext;

        public async Task<List<UserLight>> Handle(GetUsersLight request, CancellationToken cancellationToken)
        {
            var includes = UserIncludeFeatures.AuthorRatings | UserIncludeFeatures.BookRatings | UserIncludeFeatures.Feedbacks;
            var query = await UsersQueryBuilder
                .Build(_booksDbContext, includes, request.LoginFilter, request.SortUsersBy, request.SortOrdering)
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            return query.Select(x => new UserLight
            {
                Id = x.Id,
                Login = x.Login,
                BookRatingsCount = x.BookRatings.Count,
                AuthorRatingsCount = x.AuthorRatings.Count,
                FeedbacksCount = x.Feedbacks.Count
            }).ToList();
        }
    }
}

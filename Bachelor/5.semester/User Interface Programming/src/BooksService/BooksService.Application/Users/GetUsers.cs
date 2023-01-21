using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BooksService.Application.Users.Services;
using BooksService.Common;
using BooksService.Domain.Entities;
using BooksService.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BooksService.Application.Users
{
    public class GetUsers : IRequest<List<User>>
    {
        public string LoginFilter { get; set; }
        public SortUsersBy? SortUsersBy { get; set; }
        public SortOrdering? SortOrdering { get; set; }
        public UserIncludeFeatures? IncludeFeatures { get; set; }
    }

    public class GetUsersHandler : IRequestHandler<GetUsers, List<User>>
    {
        private readonly BooksDbContext _booksDbContext;

        public GetUsersHandler(BooksDbContext booksDbContext) => _booksDbContext = booksDbContext;

        public async Task<List<User>> Handle(GetUsers request, CancellationToken cancellationToken)
            => await UsersQueryBuilder
                .Build(_booksDbContext, request.IncludeFeatures, request.LoginFilter, request.SortUsersBy, request.SortOrdering)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
    }
}

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
    public class GetUserById : IRequest<User>
    {
        public int Id { get; set; }
        public UserIncludeFeatures? IncludeFeatures { get; set; }
    }

    public class GetUserByIdHandler : IRequestHandler<GetUserById, User>
    {
        private readonly BooksDbContext _booksDbContext;

        public GetUserByIdHandler(BooksDbContext booksDbContext) => _booksDbContext = booksDbContext;

        public async Task<User> Handle(GetUserById request, CancellationToken cancellationToken)
            => await UsersQueryBuilder
                .Build(_booksDbContext, request.IncludeFeatures)
                .AsNoTracking()
                .SingleOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
    }
}

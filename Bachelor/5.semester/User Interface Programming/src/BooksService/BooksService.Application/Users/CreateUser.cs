using System.Threading;
using System.Threading.Tasks;
using BooksService.Domain.Entities;
using BooksService.Persistence;
using MediatR;

namespace BooksService.Application.Users
{
    public class CreateUser : IRequest<int>
    {
        public string Login { get; set; }
        public string Password { get; set; }
    }

    public class CreateUserHandler : IRequestHandler<CreateUser, int>
    {
        private readonly BooksDbContext _booksDbContext;

        public CreateUserHandler(BooksDbContext booksDbContext) => _booksDbContext = booksDbContext;

        public async Task<int> Handle(CreateUser request, CancellationToken cancellationToken)
        {
           var user = await _booksDbContext.Users.AddAsync(new User
            {
                Login = request.Login,
                Password = Encryption.HashPassword(request.Password)
            }, cancellationToken);
            await _booksDbContext.SaveChangesAsync(cancellationToken);

            return user.Entity.Id;
        }
    }
}
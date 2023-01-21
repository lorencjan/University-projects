using System.Threading;
using System.Threading.Tasks;
using BooksService.Persistence;
using MediatR;

namespace BooksService.Application.Users
{
    public class ChangePassword : IRequest
    {
        public int UserId { get; set; }
        public string NewPassword { get; set; }
    }

    public class ChangePasswordHandler : AsyncRequestHandler<ChangePassword>
    {
        private readonly BooksDbContext _booksDbContext;
        
        public ChangePasswordHandler(BooksDbContext booksDbContext) =>  _booksDbContext = booksDbContext;

        protected override async Task Handle(ChangePassword request, CancellationToken cancellationToken)
        {
            var user = await _booksDbContext.Users.FindAsync(request.UserId);
            user.Password = Encryption.HashPassword(request.NewPassword);
            await _booksDbContext.SaveChangesAsync(cancellationToken);
        }
    }
}

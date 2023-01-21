using System.Threading;
using System.Threading.Tasks;
using BooksService.Persistence;
using MediatR;

namespace BooksService.Application.Users
{
    public class DeleteUser : IRequest
    {
        public int Id { get; set; }
    }

    public class DeleteUserHandler : AsyncRequestHandler<DeleteUser>
    {
        private readonly BooksDbContext _booksDbContext;
        
        public DeleteUserHandler(BooksDbContext booksDbContext) =>  _booksDbContext = booksDbContext;

        protected override async Task Handle(DeleteUser request, CancellationToken cancellationToken)
        {
            var user = await _booksDbContext.Users.FindAsync(request.Id);
            if (user == null)
            {
                return;
            }

            _booksDbContext.Users.Remove(user);
            await _booksDbContext.SaveChangesAsync(cancellationToken);
        }
    }
}

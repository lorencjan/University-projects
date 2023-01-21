using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BooksService.Application.Users.Services;
using BooksService.Persistence;
using MediatR;

namespace BooksService.Application.Users
{
    public class VerifyCredentials : IRequest<bool>
    {
        public string Login { get; set; }
        public string Password { get; set; }
    }

    public class VerifyCredentialsHandler : IRequestHandler<VerifyCredentials, bool>
    {
        private readonly IMediator _mediator;
        
        public VerifyCredentialsHandler(IMediator mediator) => _mediator = mediator;

        public async Task<bool> Handle(VerifyCredentials request, CancellationToken cancellationToken)
        {
            var users = await _mediator.Send(new GetUsers { LoginFilter = request.Login }, cancellationToken);
            var user = users.SingleOrDefault(x => x.Login == request.Login); //through the filter can pass partial matches
            return user != null && Encryption.VerifyPasswords(user.Password, request.Password);
        }
    }
}

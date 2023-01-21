using FluentValidation;
using Hotel.Command.Application.Clients.Dtos;

namespace Hotel.Command.Application.Clients.Validators;

public class ClientValidator : AbstractValidator<UpdateClientDto>
{
    public ClientValidator()
    {
        CascadeMode = CascadeMode.Stop;
        
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.LastName).NotEmpty();
    }
}
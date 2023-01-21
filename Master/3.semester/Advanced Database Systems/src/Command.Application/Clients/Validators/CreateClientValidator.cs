using FluentValidation;
using Hotel.Command.Application.Clients.Dtos;
using Hotel.Command.Persistence.Sql;
using Microsoft.EntityFrameworkCore;

namespace Hotel.Command.Application.Clients.Validators;

public class CreateClientValidator : AbstractValidator<CreateClient>
{
    public CreateClientValidator(IValidator<UpdateClientDto> validator, HotelContext ctx)
    {
        CascadeMode = CascadeMode.Stop;

        RuleFor(x => x.Client).SetValidator(validator);
        RuleFor(x => x.Client.Email).NotEmpty();
        RuleFor(x => x.Client.IdentityCardNumber).NotEmpty();
        RuleFor(x => x.Client)
            .MustAsync(async (client, cancellationToken) =>
                           !await ctx.Clients.AnyAsync(
                               x => x.Email == client.Email || x.IdentityCardNumber.ToLower() == client.IdentityCardNumber.ToLower(),
                               cancellationToken))
            .WithMessage(_ => "Client with same e-mail or identity number already exist in the database.");
    }
}
using FluentValidation;
using Hotel.Command.Application.Clients.Dtos;
using Hotel.Command.Persistence.Sql;
using Microsoft.EntityFrameworkCore;

namespace Hotel.Command.Application.Clients.Validators;

public class UpdateClientValidator : AbstractValidator<UpdateClient>
{
    public UpdateClientValidator(IValidator<UpdateClientDto> validator, HotelContext ctx)
    {
        CascadeMode = CascadeMode.Stop;

        RuleFor(x => x.Client).SetValidator(validator);

        RuleFor(x => x.ClientId)
            .GreaterThan(0)
            .MustAsync(async (clientId, cancellationToken) => await ctx.Clients.AnyAsync(x => x.Id == clientId, cancellationToken))
            .WithMessage(x => $"Client with id {x.ClientId} doesn't exist.");

    }
}
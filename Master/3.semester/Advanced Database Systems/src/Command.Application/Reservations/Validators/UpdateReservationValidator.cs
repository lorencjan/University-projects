using FluentValidation;
using Hotel.Command.Persistence.Sql;
using Microsoft.EntityFrameworkCore;

namespace Hotel.Command.Application.Reservations.Validators;

public class UpdateReservationValidator : AbstractValidator<UpdateReservation>
{
    public UpdateReservationValidator(HotelContext ctx)
    {
        CascadeMode = CascadeMode.Stop;
        
        RuleFor(x => x.ReservationId)
            .GreaterThan(0)
            .MustAsync(async (reservationId, cancellationToken) => await ctx.Reservations.AnyAsync(x => x.Id == reservationId, cancellationToken))
            .WithMessage(x => $"Reservation with id {x.ReservationId} doesn't exist.");
    }
}
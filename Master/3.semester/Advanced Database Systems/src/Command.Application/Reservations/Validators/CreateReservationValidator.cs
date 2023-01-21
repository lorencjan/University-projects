using FluentValidation;
using Hotel.Command.Persistence.Sql;
using Microsoft.EntityFrameworkCore;

namespace Hotel.Command.Application.Reservations.Validators;

public class CreateReservationValidator : AbstractValidator<CreateReservation>
{
    public CreateReservationValidator(HotelContext ctx)
    {
        CascadeMode = CascadeMode.Stop;

        RuleFor(x => x.Reservation.ClientId).GreaterThan(0);
        RuleFor(x => x.Reservation.Rooms)
            .NotEmpty()
            .MustAsync(
                async (rooms, cancellationToken) =>
                {
                    foreach (var room in rooms)
                        if (!await ctx.Rooms.AnyAsync(x => x.Id == room.RoomId, cancellationToken))
                            return false;

                    return true;
                })
            .WithMessage(_ => "One of specified rooms doesn't exist.");
    }
}
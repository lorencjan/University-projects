using FluentValidation;
using Hotel.Command.Persistence.Sql;
using Microsoft.EntityFrameworkCore;

namespace Hotel.Command.Application.Rooms.Validators;

public class UpdateRoomValidator : AbstractValidator<UpdateRoom>
{
    public UpdateRoomValidator(HotelContext ctx)
    {
        CascadeMode = CascadeMode.Stop;
        
        RuleFor(x => x.RoomId)
            .GreaterThan(0)
            .MustAsync(async (roomId, cancellationToken) => await ctx.Rooms.AnyAsync(x => x.Id == roomId, cancellationToken))
            .WithMessage(x => $"Room with id {x.RoomId} doesn't exist.");
    }
}
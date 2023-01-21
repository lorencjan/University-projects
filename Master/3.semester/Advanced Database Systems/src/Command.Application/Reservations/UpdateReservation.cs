using System.Threading;
using System.Threading.Tasks;
using Hotel.Command.Application.Reservations.Dtos;
using Hotel.Command.Persistence.Sql;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Hotel.Command.Application.Reservations;

public class UpdateReservation : IRequest
{
    public int ReservationId { get; set; }
    public ReservationUpdateDto Reservation { get; set; }
}

public class UpdateReservationHandler : AsyncRequestHandler<UpdateReservation>
{
    private readonly HotelContext _dbContext;
    private readonly ILogger<UpdateReservationHandler> _logger;

    public UpdateReservationHandler(HotelContext dbContext, ILogger<UpdateReservationHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    protected override async Task Handle(UpdateReservation request, CancellationToken cancellationToken)
    {
        var toUpdate = await _dbContext.Reservations.SingleAsync(x => x.Id == request.ReservationId, cancellationToken);
        toUpdate.Paid = request.Reservation.Paid;
        toUpdate.State = request.Reservation.State;

        await _dbContext.SaveChangesAsync(cancellationToken);
        _logger.LogInformation($"Reservation {toUpdate.Id} updated.");
    }
}
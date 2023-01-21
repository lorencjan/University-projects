using System.Threading;
using System.Threading.Tasks;
using Hotel.Command.Persistence.Sql;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Hotel.Command.Application.Reservations;

public class DeleteReservation : IRequest
{
    public int Id { get; set; }
}

public class DeleteReservationHandler : AsyncRequestHandler<DeleteReservation>
{
    private readonly HotelContext _dbContext;
    private readonly ILogger<DeleteReservationHandler> _logger;

    public DeleteReservationHandler(HotelContext dbContext, ILogger<DeleteReservationHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    protected override async Task Handle(DeleteReservation request, CancellationToken cancellationToken)
    {
        var reservation = await _dbContext.Reservations.FindAsync(request.Id);
        if (reservation is null)
            return;

        _dbContext.Remove(reservation);
        await _dbContext.SaveChangesAsync(cancellationToken);
        _logger.LogInformation($"Reservation {reservation.Id} was deleted.");
    }
}
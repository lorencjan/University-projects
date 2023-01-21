using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Hotel.Command.Application.Reservations.Dtos;
using Hotel.Command.Persistence.Sql;
using Hotel.Command.Persistence.Sql.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Hotel.Command.Application.Reservations;

public class CreateReservation : IRequest<int>
{
    public ReservationDto Reservation { get; set; }
}

public class CreateReservationHandler : IRequestHandler<CreateReservation, int>
{
    private readonly IMapper _mapper;
    private readonly HotelContext _dbContext;
    private readonly ILogger<CreateReservationHandler> _logger;

    public CreateReservationHandler(IMapper mapper, HotelContext dbContext, ILogger<CreateReservationHandler> logger)
    {
        _mapper = mapper;
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<int> Handle(CreateReservation request, CancellationToken cancellationToken)
    {
        var reservation = _mapper.Map<Reservation>(request.Reservation);

        await _dbContext.Reservations.AddAsync(reservation, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation($"Reservation for client {reservation.ClientId} was created with id {reservation.Id}.");
        return reservation.Id;
    }
}
using System.Threading;
using System.Threading.Tasks;
using Hotel.Command.Application.Rooms.Dtos;
using Hotel.Command.Persistence.Sql;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Hotel.Command.Application.Rooms;

public class UpdateRoom : IRequest
{
    public int RoomId { get; set; }
    public RoomUpdateDto Room { get; set; }
}

public class UpdateClientHandler : AsyncRequestHandler<UpdateRoom>
{
    private readonly HotelContext _dbContext;
    private readonly ILogger<UpdateClientHandler> _logger;

    public UpdateClientHandler(HotelContext dbContext, ILogger<UpdateClientHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    protected override async Task Handle(UpdateRoom request, CancellationToken cancellationToken)
    {
        var toUpdate = await _dbContext.Rooms.SingleAsync(x => x.Id == request.RoomId, cancellationToken);
        toUpdate.IsCleaned = request.Room.IsCleaned;

        await _dbContext.SaveChangesAsync(cancellationToken);
        _logger.LogInformation($"Room {toUpdate.Id} updated.");
    }
}
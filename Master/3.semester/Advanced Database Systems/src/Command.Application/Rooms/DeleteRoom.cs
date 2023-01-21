using System.Threading;
using System.Threading.Tasks;
using Hotel.Command.Persistence.Sql;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Hotel.Command.Application.Rooms;

public class DeleteRoom : IRequest
{
    public int Id { get; set; }
}

public class DeleteRoomHandler : AsyncRequestHandler<DeleteRoom>
{
    private readonly HotelContext _dbContext;
    private readonly ILogger<DeleteRoomHandler> _logger;

    public DeleteRoomHandler(HotelContext dbContext, ILogger<DeleteRoomHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    protected override async Task Handle(DeleteRoom request, CancellationToken cancellationToken)
    {
        var room = await _dbContext.Rooms.FindAsync(request.Id);
        if (room is null)
            return;

        _dbContext.Remove(room);
        await _dbContext.SaveChangesAsync(cancellationToken);
        _logger.LogInformation($"Room {room.Id} was deleted.");
    }
}
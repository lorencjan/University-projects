using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Hotel.Command.Application.Rooms.Dtos;
using Hotel.Command.Persistence.Sql;
using Hotel.Command.Persistence.Sql.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Hotel.Command.Application.Rooms;

public class CreateRoom : IRequest<int>
{
    public RoomDto Room { get; set; }
}

public class CreateRoomHandler : IRequestHandler<CreateRoom, int>
{
    private readonly IMapper _mapper;
    private readonly HotelContext _dbContext;
    private readonly ILogger<CreateRoomHandler> _logger;

    public CreateRoomHandler(IMapper mapper, HotelContext dbContext, ILogger<CreateRoomHandler> logger)
    {
        _mapper = mapper;
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<int> Handle(CreateRoom request, CancellationToken cancellationToken)
    {
        var room = _mapper.Map<Room>(request.Room);

        await _dbContext.Rooms.AddAsync(room, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation($"Room {room.RoomNumber} was created with id {room.Id}.");
        return room.Id;
    }
}
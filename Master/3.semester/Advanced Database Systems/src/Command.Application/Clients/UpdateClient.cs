using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Hotel.Command.Application.Clients.Dtos;
using Hotel.Command.Persistence.Sql;
using Hotel.Command.Persistence.Sql.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Hotel.Command.Application.Clients;

public class UpdateClient : IRequest
{
    public int ClientId { get; set; }
    public UpdateClientDto Client { get; set; }
}

public class UpdateClientHandler : AsyncRequestHandler<UpdateClient>
{
    private readonly IMapper _mapper;
    private readonly HotelContext _dbContext;
    private readonly ILogger<UpdateClientHandler> _logger;

    public UpdateClientHandler(IMapper mapper, HotelContext dbContext, ILogger<UpdateClientHandler> logger)
    {
        _mapper = mapper;
        _dbContext = dbContext;
        _logger = logger;
    }

    protected override async Task Handle(UpdateClient request, CancellationToken cancellationToken)
    {
        var client = _mapper.Map<Client>(request.Client);
        var toUpdate = await _dbContext.Clients.SingleAsync(x => x.Id == request.ClientId, cancellationToken);
        toUpdate.Name = client.Name;
        toUpdate.LastName = client.LastName;
        toUpdate.Street = client.Street;
        toUpdate.HouseNumber = client.HouseNumber;
        toUpdate.City = client.City;
        toUpdate.Zip = client.Zip;
        toUpdate.Country = client.Country;
        toUpdate.Sex = client.Sex;

        await _dbContext.SaveChangesAsync(cancellationToken);
        _logger.LogInformation($"Client {toUpdate.Id} updated.");
    }
}
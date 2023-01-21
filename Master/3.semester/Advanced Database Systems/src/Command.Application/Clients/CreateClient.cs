using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Hotel.Command.Application.Clients.Dtos;
using Hotel.Command.Persistence.Sql;
using Hotel.Command.Persistence.Sql.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Hotel.Command.Application.Clients;

public class CreateClient : IRequest<int>
{
    public ClientDto Client { get; set; }
}

public class CreateClientHandler : IRequestHandler<CreateClient, int>
{
    private readonly IMapper _mapper;
    private readonly HotelContext _dbContext;
    private readonly ILogger<CreateClientHandler> _logger;

    public CreateClientHandler(IMapper mapper, HotelContext dbContext, ILogger<CreateClientHandler> logger)
    {
        _mapper = mapper;
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<int> Handle(CreateClient request, CancellationToken cancellationToken)
    {
        var client = _mapper.Map<Client>(request.Client);

        await _dbContext.Clients.AddAsync(client, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation($"Client {client.Name} {client.LastName} was created with id {client.Id}.");
        return client.Id;
    }
}
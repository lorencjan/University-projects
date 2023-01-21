using System.Threading;
using System.Threading.Tasks;
using Hotel.Command.Persistence.Sql;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Hotel.Command.Application.Clients;

public class DeleteClient : IRequest
{
    public int Id { get; set; }
}

public class DeleteClientHandler : AsyncRequestHandler<DeleteClient>
{
    private readonly HotelContext _dbContext;
    private readonly ILogger<DeleteClientHandler> _logger;

    public DeleteClientHandler(HotelContext dbContext, ILogger<DeleteClientHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    protected override async Task Handle(DeleteClient request, CancellationToken cancellationToken)
    {
        var client = await _dbContext.Clients.FindAsync(request.Id);
        if (client is null)
            return;

        _dbContext.Remove(client);
        await _dbContext.SaveChangesAsync(cancellationToken);
        _logger.LogInformation($"Client {client.Id} {client.Name} {client.LastName} was deleted.");
    }
}
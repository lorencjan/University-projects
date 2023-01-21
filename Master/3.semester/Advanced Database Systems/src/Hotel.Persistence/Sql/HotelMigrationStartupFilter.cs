using System;
using System.Linq;
using Hotel.Command.Persistence.Sql.Seed;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Hotel.Command.Persistence.Sql;

internal class HotelMigrationStartupFilter : IStartupFilter
{
    private readonly IServiceScopeFactory _scopeFactory;

    public HotelMigrationStartupFilter(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
    {
        using var scope = _scopeFactory.CreateScope();
        using var ctx = scope.ServiceProvider.GetRequiredService<HotelContext>();
        ctx.Database.Migrate();

        if (ctx.Clients.Any()) // there are already data in the db -> no seed
            return next;

        ctx.Clients.AddRange(ClientsSeed.Data);
        ctx.Rooms.AddRange(RoomsSeed.Data);
        ctx.Reservations.AddRange(ReservationsSeed.Data);
        ctx.SaveChanges();

        return next;
    }
}
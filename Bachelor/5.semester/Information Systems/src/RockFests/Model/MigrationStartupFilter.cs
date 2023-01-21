using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace RockFests.Model
{
    internal class MigrationStartupFilter<TContext> : IStartupFilter where TContext : DbContext
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public MigrationStartupFilter(IServiceScopeFactory scopeFactory)
            => _scopeFactory = scopeFactory;

        public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
        {
            using var scope = _scopeFactory.CreateScope();
            using var ctx = scope.ServiceProvider.GetRequiredService<TContext>();
            ctx.Database.Migrate();
            return next;
        }
    }
}

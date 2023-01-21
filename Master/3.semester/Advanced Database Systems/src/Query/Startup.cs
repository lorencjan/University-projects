using Hotel.Command.Persistence.Cassandra;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Hotel.Query;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddApiVersioning(options =>
        {
            options.ReportApiVersions = true;
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.DefaultApiVersion = new ApiVersion(1, 0);
        });
        services.AddVersionedApiExplorer(options => options.SubstituteApiVersionInUrl = true);

        services.AddControllers();

        services.AddSwaggerGen();
        services.AddOpenApiDocument((settings, _) =>
        {
            settings.Title = "Query";
            settings.Version = "v1";
            settings.SchemaType = NJsonSchema.SchemaType.OpenApi3;
        });

        //Creates new connection to DB and applies migrations
        using (HotelContextCassandra db = new HotelContextCassandra())
        {
            db.CassandraPseudoMigrationTables();
            //db.CassandraPseudoMigrationSecondaryIndexes();
            db.CassandraPseudoMigrationMaterializedViews();
        }
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
            app.UseDeveloperExceptionPage();

        app.UseRouting();
        app.UseAuthorization();

        app.UseEndpoints(endpoints => endpoints.MapControllers());
        app.UseOpenApi();
        app.UseSwaggerUi3();
    }
}
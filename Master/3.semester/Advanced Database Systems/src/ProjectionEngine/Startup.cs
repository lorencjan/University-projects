using Hotel.Command.Persistence;
using Hotel.ProjectionEngine.Application;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Hotel.ProjectionEngine;

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
        services.AddHotelDb(Configuration);
        services.AddTransient<ProjectRepository>();
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
            settings.Title = "Projection Engine";
            settings.Version = "v1";
            settings.SchemaType = NJsonSchema.SchemaType.OpenApi3;
        });
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
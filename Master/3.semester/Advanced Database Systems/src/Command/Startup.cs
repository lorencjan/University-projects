using AutoMapper;
using FluentValidation.AspNetCore;
using Hotel.Command.Application;
using Hotel.Command.Application.Clients;
using Hotel.Command.Application.Clients.Validators;
using Hotel.Command.Persistence;
using Hotel.ProjectionEngine.Client;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Hotel.Command;

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
        services.AddProjectionEngine(Configuration);
        services.AddHotelDb(Configuration);
        services.AddApiVersioning(options =>
        {
            options.ReportApiVersions = true;
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.DefaultApiVersion = new ApiVersion(1, 0);
        });
        services.AddVersionedApiExplorer(options => options.SubstituteApiVersionInUrl = true);

        services.AddHotelMigrationStartupFilter();

        services
            .AddControllers(options => options.Filters.Add<ValidationExceptionFilterAttribute>())
            .AddFluentValidation(
                configuration =>
                {
                    configuration.RegisterValidatorsFromAssemblyContaining<ClientValidator>();
                    configuration.AutomaticValidationEnabled = false;
                });

        services.AddSwaggerGen();
        services.AddOpenApiDocument((settings, _) =>
        {
            settings.Title = "Command";
            settings.Version = "v1";
            settings.SchemaType = NJsonSchema.SchemaType.OpenApi3;
        });

        services.AddMediatR(typeof(CreateClient));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        
        var mapperConfig = new MapperConfiguration(mc => mc.AddProfile<MapperProfile>());
        services.AddSingleton(mapperConfig.CreateMapper());
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
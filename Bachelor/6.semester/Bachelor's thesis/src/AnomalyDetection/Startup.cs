using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using YSoft.Rqa.AnomalyDetection.Data.Model.Graylog;
using YSoft.Rqa.AnomalyDetection.Data.Services;

namespace YSoft.Rqa.AnomalyDetection
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration) {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services) {
            services.AddApiVersioning(
                options =>
                {
                    options.ReportApiVersions = true;
                    options.AssumeDefaultVersionWhenUnspecified = true;
                    options.DefaultApiVersion = new ApiVersion(1, 0);
                });

            services.AddVersionedApiExplorer(options => options.SubstituteApiVersionInUrl = true);
            services.AddHealthChecks();
            services.AddControllers();
            services.AddSwaggerGen();
            services.AddOpenApiDocument(
                (settings, provider) =>
                {
                    settings.Title = "AnomalyDetection";
                    settings.Version = "v1";
                });

            services.AddTransient<GraylogProvider>();
            services.AddTransient<CsvHandler>();
            services.Configure<GraylogConfiguration>(Configuration.GetSection("GraylogConfiguration"));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(
                endpoints =>
                {
                    endpoints.MapControllers();
                    endpoints.MapHealthChecks(
                        "/healthcheck",
                        new HealthCheckOptions {Predicate = _ => true, ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse});
                });

            app.UseOpenApi();
            app.UseSwaggerUi3();
        }
    }
}
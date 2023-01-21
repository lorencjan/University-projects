using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using DotVVM.Framework.Routing;
using Microsoft.Extensions.Logging;
using RockFests.BL.Mapping;
using RockFests.DAL;
using RockFests.DAL.StaticServices;
using RockFests.Model;

namespace RockFests
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration) => Configuration = configuration;

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDataProtection();
            services.AddAuthorization();
            services.AddWebEncoders();
            services.AddHttpContextAccessor();
            services.AddDotVVM<DotvvmStartup>();

            MapperConfig.SetMapper();
            services.AddRockFestsDb(Configuration);
            services.AddTransient<IStartupFilter, MigrationStartupFilter<RockFestsDbContext>>();

            services.AddAuthorization();
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/Authentication/SignIn";
                    options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
                    options.SlidingExpiration = true;
                });

            services.AddServicesWithAttribute();
            services.AddViewModels();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            app.UseAuthentication();
            app.UseStatusCodePages(async context =>
            {
                await Task.CompletedTask;
                if (context.HttpContext.Response.StatusCode == 404)
                {
                    context.HttpContext.Response.Redirect("/404");
                }
            });

            // use DotVVM
            var dotvvmConfiguration = app.UseDotVVM<DotvvmStartup>(env.ContentRootPath, modifyConfiguration: configuration =>
                configuration.Runtime.GlobalFilters.Add(new ExceptionFilter(loggerFactory.CreateLogger<ExceptionFilter>())));
            dotvvmConfiguration.AssertConfigurationIsValid();
            
            // use static files
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(env.ContentRootPath)
            });

            app.UseHttpsRedirection();
        }
    }
}

using System;
using Gelf.Extensions.Logging;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace YSoft.Rqa.AnomalyDetection
{
    public class Program
    {
        public static void Main(string[] args) {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureLogging(
                    (context, builder) =>
                    {
                        builder.ClearProviders();
                        builder.AddConsole();
                        builder.AddGelf(options => options.AdditionalFields.Add("Hostname", Environment.MachineName));
                        builder.AddConfiguration(context.Configuration.GetSection("Logging"));
                    })
                .ConfigureWebHostDefaults(
                    webBuilder =>
                    {
                        webBuilder.UseKestrel();
                        webBuilder.UseStartup<Startup>();
                    });
    }
}
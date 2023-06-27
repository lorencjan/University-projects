using Quartz;
using WapApi;
using WebApi.Application.DataProviders;
using WebApi.Application.Jobs;
using WebApi.Application.Options;
using WebApi.Application.Services;

var builder = WebApplication.CreateBuilder(args);

// services
builder.Services.AddCors(opts => opts.AddDefaultPolicy(p => p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<DatasetUrls>(builder.Configuration.GetSection("DatasetUrls"));
builder.Services.Configure<DatasetPaths>(builder.Configuration.GetSection("DatasetPaths"));

builder.Services.AddTransient<AccidentsProvider>();
builder.Services.AddTransient<SchoolsProvider>();
builder.Services.AddTransient<SportGroundsProvider>();
builder.Services.AddTransient<RefreshService>();
builder.Services.AddHostedService<StartupService>();

builder.Services.AddQuartz(q =>
{
    q.UseMicrosoftDependencyInjectionJobFactory();

    var jobKey = new JobKey("RefreshJob");
    q.AddJob<RefreshJob>(opts => opts.WithIdentity(jobKey));

    q.AddTrigger(
        opts => opts
            .ForJob(jobKey)
            .WithIdentity("RefreshJob-trigger")
            .WithSchedule(CronScheduleBuilder.DailyAtHourAndMinute(0, 0))); // everyday midnight
});
builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

var app = builder.Build();

// configuration
app.UseCors();
app.MapControllers();
app.UseSwagger();
app.UseSwaggerUI();

app.Run();

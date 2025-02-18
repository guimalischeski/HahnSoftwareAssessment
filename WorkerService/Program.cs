using Domain.Interfaces;
using Hangfire;
using HangfireJob;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Persistence;
using Persistence.Repositories;

var builder = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((context, config) =>
    {
        config.SetBasePath(AppContext.BaseDirectory);
        config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
    })
    .ConfigureServices((hostContext, services) =>
    {
        var configuration = hostContext.Configuration;

        // Database Configuration
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        // Configure Hangfire Storage
        services.AddHangfire(config =>
            config.UseSqlServerStorage(configuration.GetConnectionString("DefaultConnection")));

        // Start Hangfire Server
        services.AddHangfireServer();

        // Register Repository & Job
        services.AddScoped<ICatFactRepository, CatFactRepository>();
        services.AddScoped<ICatFactUpsertJob, CatFactUpsertJob>();

        // Register Hangfire Job Manager
        services.AddSingleton<IRecurringJobManager, RecurringJobManager>();
    });

var host = builder.Build();

// Retrieve Job Interval from Config (before `using` block)
var configuration = host.Services.GetRequiredService<IConfiguration>();
var jobInterval = configuration["HangfireSettings:CatFactJobInterval"] ?? "Hourly";

using (var scope = host.Services.CreateScope())
{
    var serviceProvider = scope.ServiceProvider;
    var recurringJobManager = serviceProvider.GetRequiredService<IRecurringJobManager>();
    var job = serviceProvider.GetRequiredService<ICatFactUpsertJob>();

    string cronExpression = jobInterval switch
    {
        "Minutely" => Cron.Minutely(),
        "Hourly" => Cron.Hourly(),
        "Daily" => Cron.Daily(),
        "Weekly" => Cron.Weekly(),
        _ => Cron.Hourly()
    };

    // Use IRecurringJobManager instead of static RecurringJob.AddOrUpdate
    recurringJobManager.AddOrUpdate("FetchCatFact", () => job.Execute(), cronExpression);
}

await host.RunAsync();

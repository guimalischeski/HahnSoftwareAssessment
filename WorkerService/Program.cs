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

        // Database
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        // Hangfire
        services.AddHangfire(config =>
            config.UseSqlServerStorage(configuration.GetConnectionString("DefaultConnection")));
        services.AddHangfireServer();

        // Register Repository & Job
        services.AddScoped<ICatFactRepository, CatFactRepository>();
        services.AddSingleton<ICatFactUpsertJob, CatFactUpsertJob>();  // Register via interface

        // Retrieve the Hangfire Job interval setting
        var jobInterval = configuration["HangfireSettings:CatFactJobInterval"] ?? "Hourly";

        // Set up Hangfire job schedule dynamically
        var cronExpression = jobInterval switch
        {
            "Hourly" => Cron.Hourly(),   // Every hour
            "Daily" => Cron.Daily(),     // Every day at midnight
            "Weekly" => Cron.Weekly(),   // Every Sunday at midnight
            _ => Cron.Hourly(),          // Default to hourly if unknown value
        };

        // Schedule the job
        RecurringJob.AddOrUpdate<ICatFactUpsertJob>(
            "FetchCatFact",
            job => job.Execute(),
            cronExpression  // Dynamically set schedule
        );
    })
    .Build();

await builder.RunAsync();

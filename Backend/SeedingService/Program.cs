using Database;
using FileStorage;
using SeedingService;
using ServiceDefaults;

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();
builder.Services.AddHostedService<Worker>();

builder.Services.AddOpenTelemetry()
    .WithTracing(tracing => tracing.AddSource(Worker.ActivitySourceName));

builder.AddNpgsqlDbContext<AppDbContext>("postgresdb");
builder.Services.AddSingleton<IFileStorage, LocalFileStorage>();

var host = builder.Build();
host.Run();
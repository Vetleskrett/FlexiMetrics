using MigrationService;
using Database;

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();
builder.Services.AddHostedService<Worker>();

builder.Services.AddOpenTelemetry()
    .WithTracing(tracing => tracing.AddSource(Worker.ActivitySourceName));

builder.AddNpgsqlDbContext<AppDbContext>("postgresdb", _ => { }, options =>
{
    if (builder.Environment.IsDevelopment())
    {
        options.UseAsyncSeeding(async (dbContext, _, cancellationToken) =>
        {
            await dbContext.SeedDatabaseAsync();
        });
        options.UseSeeding((dbContext, _) =>
        {
            dbContext.SeedDatabaseAsync().Wait();
        });
    }
});

var host = builder.Build();
host.Run();
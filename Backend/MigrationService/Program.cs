using MigrationService;
using Database;
using FileStorage;

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();
builder.Services.AddHostedService<Worker>();

builder.Services.AddOpenTelemetry()
    .WithTracing(tracing => tracing.AddSource(Worker.ActivitySourceName));

builder.AddNpgsqlDbContext<AppDbContext>("postgresdb", _ => { }, options =>
{


    if (builder.Environment.IsDevelopment())
    {
        var fileStorage = new LocalFileStorage();

        options.UseAsyncSeeding(async (dbContext, _, cancellationToken) =>
        {
            await dbContext.SeedDatabaseAsync(fileStorage);
        });
        options.UseSeeding((dbContext, _) =>
        {
            dbContext.SeedDatabaseAsync(fileStorage).Wait();
        });
    }
});

var host = builder.Build();
host.Run();
var builder = DistributedApplication.CreateBuilder(args);

var postgresdb = builder.AddPostgres("postgres")
    .WithDataVolume()
    .WithPgAdmin()
    .AddDatabase("postgresdb");

var migrationService = builder.AddProject<Projects.MigrationService>("migrationservice")
    .WithReference(postgresdb)
    .WaitFor(postgresdb);

var seedingService = builder.AddProject<Projects.SeedingService>("seedingservice")
    .WithReference(postgresdb)
    .WaitFor(postgresdb)
    .WaitForCompletion(migrationService);

var api = builder.AddProject<Projects.Api>("api")
    .WithReference(postgresdb)
    .WaitForCompletion(migrationService)
    .WaitForCompletion(seedingService);

#if DEBUG

builder.AddNpmApp("frontend", "../../Frontend", "dev")
    .WithReference(api)
    .WaitFor(api)
    .WithHttpEndpoint(5173, isProxied: false);

#else

builder.AddNpmApp("frontend", "../../Frontend", "start")
    .WithReference(api)
    .WaitFor(api)
    .WithHttpEndpoint(4173, isProxied: false);

#endif

builder.Build().Run();

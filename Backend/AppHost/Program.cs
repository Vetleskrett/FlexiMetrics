var builder = DistributedApplication.CreateBuilder(args);

var postgresdb = builder.AddPostgres("postgres")
    .WithDataVolume()
    .WithPgAdmin()
    .AddDatabase("postgresdb");

var migrationService = builder.AddProject<Projects.MigrationService>("migrationservice")
    .WithReference(postgresdb)
    .WaitFor(postgresdb);

var api = builder.AddProject<Projects.Api>("api")
    .WithReference(postgresdb)
    .WaitForCompletion(migrationService);

builder.AddNpmApp("frontend", "../../Frontend", "dev")
    .WithReference(api)
    .WaitFor(api)
    .WithHttpEndpoint(5173, isProxied: false);

builder.Build().Run();

var builder = DistributedApplication.CreateBuilder(args);

var postgresdb = builder.AddPostgres("postgres")
    .WithDataVolume()
    .AddDatabase("postgresdb");

builder.AddProject<Projects.Api>("api")
    .WithReference(postgresdb);

builder.AddProject<Projects.MigrationService>("migrationservice")
    .WithReference(postgresdb);

builder.Build().Run();

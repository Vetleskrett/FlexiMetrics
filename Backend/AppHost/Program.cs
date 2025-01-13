var builder = DistributedApplication.CreateBuilder(args);

var postgresdb = builder.AddPostgres("postgres")
    .WithDataVolume()
    .WithPgAdmin()
    .AddDatabase("postgresdb");

var migrationService = builder.AddProject<Projects.MigrationService>("migrationservice")
    .WithReference(postgresdb)
    .WaitFor(postgresdb);

builder.AddProject<Projects.Api>("api")
    .WithReference(postgresdb)
    .WaitForCompletion(migrationService);

builder.Build().Run();

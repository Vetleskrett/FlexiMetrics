using Api.Courses;
using FluentValidation;
using Database;
using Api.Teams;
using Api.Assignments;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.AddNpgsqlDbContext<AppDbContext>("postgresdb");

builder.Services.AddScoped<ICourseService, CourseService>();
builder.Services.AddScoped<ITeamService, TeamService>();
builder.Services.AddScoped<IAssignmentService, AssignmentService>();

builder.Services.AddValidatorsFromAssemblyContaining<Program>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowCors",
        builder => builder
            .WithOrigins("http://localhost:5173", "https://localhost:5173")
            .SetIsOriginAllowedToAllowWildcardSubdomains()
            .AllowAnyMethod()
            .AllowAnyHeader());
});

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

var app = builder.Build();

app.MapDefaultEndpoints();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapCourseEndpoints();
app.MapTeamEndpoints();
app.MapAssignmentEndpoints();
app.UseCors("AllowCors");

app.Run();

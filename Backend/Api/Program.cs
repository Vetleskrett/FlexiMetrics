using Api.Courses;
using FluentValidation;
using Database;
using Api.Teams;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.AddNpgsqlDbContext<AppDbContext>("postgresdb");

builder.Services.AddScoped<ICourseService, CourseService>();
builder.Services.AddScoped<ITeamService, TeamService>();

builder.Services.AddValidatorsFromAssemblyContaining<Program>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

app.Run();

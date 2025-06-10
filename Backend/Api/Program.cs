using Api.Analyses;
using Api.Analyzers;
using Api.AssignmentFields;
using Api.Assignments;
using Api.Courses;
using Api.Deliveries;
using Api.Feedbacks;
using Api.Progress;
using Api.Students;
using Api.Teachers;
using Api.Teams;
using Container;
using Container.Consumers;
using Database;
using FileStorage;
using FluentValidation;
using MassTransit;
using ServiceDefaults;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddLogging();

builder.AddServiceDefaults();

builder.Services.AddSingleton<IFileStorage, LocalFileStorage>();
builder.Services.AddContainer();

builder.AddNpgsqlDbContext<AppDbContext>("postgresdb");

builder.Services.AddMassTransit(options =>
{
    options.AddConsumer<BuildAnalyzerConsumer>();
    options.AddConsumer<RunAnalyzerConsumer>();
    options.AddConsumer<CancelAnalyzerConsumer>();
    options.AddConsumer<DeleteAnalyzerConsumer>();
    options.UsingInMemory((context, config) =>
    {
        config.ConfigureEndpoints(context);
    });
});

builder.Services.AddScoped<ICourseService, CourseService>();
builder.Services.AddScoped<ITeacherService, TeacherService>();
builder.Services.AddScoped<IStudentService, StudentService>();
builder.Services.AddScoped<ITeamService, TeamService>();
builder.Services.AddScoped<IAssignmentService, AssignmentService>();
builder.Services.AddScoped<IAssignmentFieldService, AssignmentFieldService>();
builder.Services.AddScoped<IDeliveryService, DeliveryService>();
builder.Services.AddScoped<IProgressService, ProgressService>();
builder.Services.AddScoped<IFeedbackService, FeedbackService>();
builder.Services.AddScoped<IAnalyzerService, AnalyzerService>();
builder.Services.AddScoped<IAnalysisService, AnalysisService>();

builder.Services.AddValidatorsFromAssemblyContaining<Program>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowCors",
        builder => builder
            .WithOrigins
            (
                "http://localhost:5173",
                "https://localhost:5173",
                "http://localhost:4173",
                "https://localhost:4173"
            )
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

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("AllowCors");

app.MapCourseEndpoints();
app.MapTeacherEndpoints();
app.MapStudentEndpoints();
app.MapTeamEndpoints();
app.MapAssignmentEndpoints();
app.MapAssignmentFieldEndpoints();
app.MapDeliveryEndpoints();
app.MapProgressEndpoints();
app.MapFeedbackEndpoints();
app.MapAnalyzerEndpoints();
app.MapAnalysisEndpoints();

app.Run();

public partial class Program { }
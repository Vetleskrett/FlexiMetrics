using Api.Courses;
using FluentValidation;
using Database;
using Api.Teams;
using Api.Assignments;
using System.Text.Json.Serialization;
using Api.Teachers;
using Api.Students;
using Api.AssignmentFields;
using Api.Deliveries;
using Api.Feedbacks;
using FileStorage;
using Api.Analyzers;
using Container;
using Api.Analyses;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using MassTransit;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddLogging();

builder.AddServiceDefaults();

builder.Services.AddSingleton<IFileStorage, LocalFileStorage>();
builder.Services.AddContainer();

builder.AddNpgsqlDbContext<AppDbContext>("postgresdb");

builder.Services.AddMassTransit(options =>
{
    options.AddConsumer<RunAnalyzerConsumer>();
    options.AddConsumer<AnalyzerStatusUpdateConsumer>();
    options.UsingInMemory((context, config) =>
    {
        config.ConfigureEndpoints(context);
    });
});
builder.Services.AddSingleton<IAnalyzerStatusUpdateReader, AnalyzerStatusUpdateReader>();

builder.Services.AddScoped<ICourseService, CourseService>();
builder.Services.AddScoped<ITeacherService, TeacherService>();
builder.Services.AddScoped<IStudentService, StudentService>();
builder.Services.AddScoped<ITeamService, TeamService>();
builder.Services.AddScoped<IAssignmentService, AssignmentService>();
builder.Services.AddScoped<IAssignmentFieldService, AssignmentFieldService>();
builder.Services.AddScoped<IDeliveryService, DeliveryService>();
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

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(jwtOptions =>
{
    if (builder.Environment.IsDevelopment())
    {
        jwtOptions.RequireHttpsMetadata = false;
    }

    jwtOptions.Authority = builder.Configuration["Feide:Issuer"];
    jwtOptions.Audience = builder.Configuration["Feide:ClientId"];
});

builder.Services.AddAuthorizationBuilder()
    .SetDefaultPolicy
    (
        new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme)
            .RequireAuthenticatedUser()
            .Build()
    );

var app = builder.Build();

await app.InitializeContainer();

app.MapDefaultEndpoints();

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("AllowCors");

app.UseAuthentication();
app.UseAuthorization();

app.MapCourseEndpoints();
app.MapTeacherEndpoints();
app.MapStudentEndpoints();
app.MapTeamEndpoints();
app.MapAssignmentEndpoints();
app.MapAssignmentFieldEndpoints();
app.MapDeliveryEndpoints();
app.MapFeedbackEndpoints();
app.MapAnalyzerEndpoints();
app.MapAnalysisEndpoints();

app.Run();

public partial class Program { }
using Api.Analyses;
using Api.Analyzers;
using Api.AssignmentFields;
using Api.Assignments;
using Api.Authorization;
using Api.Courses;
using Api.Deliveries;
using Api.Feedbacks;
using Api.Progress;
using Api.Students;
using Api.Teachers;
using Api.Teams;
using Container;
using Database;
using FileStorage;
using FluentValidation;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
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
    options.AddConsumer<RunAnalyzerConsumer>();
    options.AddConsumer<CancelAnalyzerConsumer>();
    options.AddConsumer<DeleteAnalyzerConsumer>();
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
builder.Services.AddScoped<IProgressService, ProgressService>();
builder.Services.AddScoped<IFeedbackService, FeedbackService>();
builder.Services.AddScoped<IAnalyzerService, AnalyzerService>();
builder.Services.AddScoped<IAnalysisService, AnalysisService>();

builder.Services.AddValidatorsFromAssemblyContaining<Program>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    var oauth2Scheme = new OpenApiSecurityScheme
    {
        Name = "oAuth2",
        Type = SecuritySchemeType.OAuth2,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Flows = new OpenApiOAuthFlows
        {
            AuthorizationCode = new OpenApiOAuthFlow
            {
                AuthorizationUrl = new Uri("https://auth.dataporten.no/oauth/authorization"),
                TokenUrl = new Uri("https://auth.dataporten.no/oauth/token"),
                Scopes = new Dictionary<string, string>
                {
                    { "openid", "OpenID Connect" },
                    { "profile", "User profile information" },
                    { "email", "User email address" }
                }
            }
        },
        Extensions = new Dictionary<string, IOpenApiExtension>
        {
            { "x-tokenName", new OpenApiString("id_token") }
        },
    };

    options.AddSecurityDefinition("oauth2", oauth2Scheme);

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "oauth2"
                }
            },
            Array.Empty<string>()
        }
    });
});

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
)
.AddPolicy("Admin", policy => policy.Requirements.Add(new UserRequirement("admin")))
.AddPolicy("Teacher", policy => policy.Requirements.Add(new UserRequirement("teacher")))
.AddPolicy("TeacherId", policy => policy.Requirements.Add(new UserRequirement("teacherId")))
.AddPolicy("StudentId", policy => policy.Requirements.Add(new UserRequirement("studentId")))
.AddPolicy("Course", policy => policy.Requirements.Add(new UserRequirement("course")))
.AddPolicy("TeacherInCourse", policy => policy.Requirements.Add(new UserRequirement("teacherCourse")))
.AddPolicy("TeacherForTeam", policy => policy.Requirements.Add(new UserRequirement("teacherTeam")))
.AddPolicy("StudentInTeam", policy => policy.Requirements.Add(new UserRequirement("studentTeam")))
.AddPolicy("StudentInCourse", policy => policy.Requirements.Add(new UserRequirement("studentCourse")))
.AddPolicy("InDelivery", policy => policy.Requirements.Add(new UserRequirement("delivery")))
.AddPolicy("InDeliveryField", policy => policy.Requirements.Add(new UserRequirement("deliveryField")))
.AddPolicy("Feedback", policy => policy.Requirements.Add(new UserRequirement("feedback")))
.AddPolicy("TeacherForFeedback", policy => policy.Requirements.Add(new UserRequirement("feedbackTeacher")))
.AddPolicy("TeacherForAssignmentOrStudent", policy => policy.Requirements.Add(new UserRequirement("assignmentTeacherOrStudent")))
.AddPolicy("TeacherForAssignmentOrTeam", policy => policy.Requirements.Add(new UserRequirement("assignmentTeacherOrTeam")))
.AddPolicy("TeacherForAssignment", policy => policy.Requirements.Add(new UserRequirement("assignmentTeacher")))
.AddPolicy("TeacherForAssignmentField", policy => policy.Requirements.Add(new UserRequirement("assignmentFieldTeacher")))
.AddPolicy("Assignment", policy => policy.Requirements.Add(new UserRequirement("assignment")))
.AddPolicy("TeacherForAnalyzer", policy => policy.Requirements.Add(new UserRequirement("analyzer")))
.AddPolicy("TeacherForAnalysis", policy => policy.Requirements.Add(new UserRequirement("analysis")));

builder.Services.AddScoped<IAuthorizationHandler, UserAuthorizationHandler>();

var app = builder.Build();

app.MapDefaultEndpoints();

app.UseAuthentication();
app.UseAuthorization();

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.OAuthClientId(builder.Configuration["Feide:ClientId"]);
    options.OAuthScopes("openid", "profile", "email");
    options.OAuthUsePkce();
    options.EnablePersistAuthorization();
});

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
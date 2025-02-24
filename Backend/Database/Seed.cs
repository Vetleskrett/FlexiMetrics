using Bogus;
using Database.Models;
using FileStorage;
using Microsoft.EntityFrameworkCore;
using System.Net.Mime;
using System.Text;
using System.Text.Json;

namespace Database;

public static class Seed
{
    private const int SEED = 123;

    public static async Task SeedDatabaseAsync(this DbContext dbContext, IFileStorage fileStorage)
    {
        fileStorage.DeleteAll();
        await dbContext.Set<Course>().ExecuteDeleteAsync();
        await dbContext.Set<User>().ExecuteDeleteAsync();

        var userFaker = new Faker<User>()
            .UseSeed(SEED)
            .RuleFor(x => x.Id, f => f.Random.Guid())
            .RuleFor(x => x.Email, f => f.Person.Email)
            .RuleFor(x => x.Name, f => f.Person.FullName); ;

        var students = userFaker
            .RuleFor(x => x.Role, Role.Student)
            .Generate(100);

        var teachers = userFaker
            .RuleFor(x => x.Role, Role.Teacher)
            .Generate(10);

        dbContext.AddRange(students);
        dbContext.AddRange(teachers);

        var courses = new Faker<Course>()
            .UseSeed(SEED)
            .RuleFor(x => x.Id, f => f.Random.Guid())
            .RuleFor(x => x.Code, f => "TDT" + f.Random.Number(1000, 9999))
            .RuleFor(x => x.Name, f => f.PickRandom(COURSES))
            .RuleFor(x => x.Year, 2025)
            .RuleFor(x => x.Semester, Semester.Spring)
            .Generate(10);

        dbContext.AddRange(courses);

        var courseTeacherFaker = new Faker<CourseTeacher>()
            .UseSeed(SEED);

        var courseTeachers = courses.Select(course =>
        {
            return courseTeacherFaker
                .RuleFor(x => x.CourseId, course.Id)
                .RuleFor(x => x.TeacherId, f => f.PickRandom(teachers).Id)
                .GenerateForever()
                .DistinctBy(x => x.TeacherId)
                .Take(3);
        })
        .SelectMany(x => x)
        .ToList();

        dbContext.AddRange(courseTeachers);

        var courseStudentFaker = new Faker<CourseStudent>()
            .UseSeed(SEED);

        var courseStudents = courses.Select(course =>
        {
            return courseStudentFaker
                .RuleFor(x => x.CourseId, course.Id)
                .RuleFor(x => x.StudentId, f => f.PickRandom(students).Id)
                .GenerateForever()
                .DistinctBy(x => x.StudentId)
                .Take(30);
        })
        .SelectMany(x => x)
        .ToList();

        dbContext.AddRange(courseStudents);

        var teamFaker = new Faker<Team>()
            .UseSeed(SEED)
            .RuleFor(x => x.Id, f => f.Random.Guid());

        var teams = courses.Select(course =>
        {
            var studentsInCourse = courseStudents
                .Where(x => x.CourseId == course.Id)
                .Select(x => students.First(s => s.Id == x.StudentId));

            return studentsInCourse.Chunk(3).Select((studentsInTeam, index) =>
            {
                return teamFaker
                    .RuleFor(x => x.TeamNr, index + 1)
                    .RuleFor(x => x.CourseId, course.Id)
                    .RuleFor(x => x.Students, studentsInTeam.ToList())
                    .Generate();
            });
        })
        .SelectMany(x => x)
        .ToList();

        dbContext.AddRange(teams);

        var assignmentFaker = new Faker<Assignment>()
            .UseSeed(SEED)
            .RuleFor(x => x.Id, f => f.Random.Guid());

        var assignments = courses.Select(course =>
        {
            return assignmentFaker
                .RuleFor(x => x.Name, f => f.PickRandom(ASSIGNMENTS))
                .RuleFor(x => x.DueDate, f =>
                {
                    var start = DateTime.UtcNow - TimeSpan.FromDays(30);
                    var end = DateTime.UtcNow + TimeSpan.FromDays(30);
                    return f.Date.Between(start, end);
                })
                .RuleFor(x => x.Published, f => f.Random.Float() > 0.1)
                .RuleFor(x => x.CollaborationType, f => f.Random.Enum<CollaborationType>())
                .RuleFor(x => x.Mandatory, f => f.Random.Bool())
                .RuleFor(x => x.GradingType, f => f.Random.Enum<GradingType>())
                .RuleFor(x => x.MaxPoints, (f, a) => a.GradingType == GradingType.PointsGrading ? 10 * f.Random.Int(1, 10) : null)
                .RuleFor(x => x.Description, f => f.Lorem.Paragraphs(2))
                .RuleFor(x => x.CourseId, course.Id)
                .GenerateBetween(2, 5);
        })
        .SelectMany(x => x)
        .ToList();

        dbContext.AddRange(assignments);

        var assignmentFieldFaker = new Faker<AssignmentField>()
            .UseSeed(SEED)
            .RuleFor(x => x.Id, f => f.Random.Guid());

        var assignmentFields = assignments.Select(assignment =>
        {
            return assignmentFieldFaker
                .RuleFor(x => x.Type, f => f.Random.Enum<AssignmentDataType>())
                .RuleFor(f => f.SubType, (f, field) =>
                {
                    return field.Type == AssignmentDataType.List ?
                        f.Random.Enum<AssignmentDataType>(exclude: [AssignmentDataType.List, AssignmentDataType.File]) :
                        null;
                })
                .RuleFor(x => x.Name, f =>
                {
                    var name = string.Join(" ", f.Lorem.Words());
                    return string.Concat(name.ToUpper().AsSpan(0, 1), name.AsSpan(1));
                })
                .RuleFor(x => x.AssignmentId, assignment.Id)
                .GenerateBetween(3, 6);
        })
        .SelectMany(x => x)
        .ToList();

        dbContext.AddRange(assignmentFields);

        var deliveryFaker = new Faker<Delivery>()
            .UseSeed(SEED)
            .RuleFor(x => x.Id, f => f.Random.Guid());

        var deliveries = courses.Select(course =>
        {
            var studentsInCourse = courseStudents
                .Where(x => x.CourseId == course.Id)
                .Select(x => students.First(s => s.Id == x.StudentId))
                .ToList();

            var teamsInCourse = teams
                .Where(x => x.CourseId == course.Id)
                .ToList();

            var assignmentsInCourse = assignments
                .Where(a => a.CourseId == course.Id && a.Published)
                .ToList();

            return assignmentsInCourse
                .Take(assignmentsInCourse.Count - 1)
                .Select(assignment =>
                {
                    var isIndividual = assignment.CollaborationType == CollaborationType.Individual;

                    if (isIndividual)
                    {
                        return studentsInCourse.Select(student =>
                        {
                            return deliveryFaker
                                .RuleFor(x => x.AssignmentId, assignment.Id)
                                .RuleFor(x => x.StudentId, student.Id)
                                .RuleFor(x => x.TeamId, f => null)
                                .Generate();
                        })
                        .ToList();
                    }
                    else
                    {
                        return teamsInCourse.Select(team =>
                        {
                            return deliveryFaker
                                .RuleFor(x => x.AssignmentId, assignment.Id)
                                .RuleFor(x => x.TeamId, team.Id)
                                .RuleFor(x => x.StudentId, f => null)
                                .Generate();
                        })
                        .ToList();
                    }
                })
                .SelectMany(x => x);
        })
        .SelectMany(x => x)
        .ToList();

        dbContext.AddRange(deliveries);

        var deliveryFieldFaker = new Faker<DeliveryField>()
            .UseSeed(SEED)
            .RuleFor(x => x.Id, f => f.Random.Guid());

        static object GetValue(AssignmentDataType type, Faker f)
        {
            return type switch
            {
                AssignmentDataType.ShortText => f.Lorem.Sentence(3),
                AssignmentDataType.LongText => f.Lorem.Paragraph(),
                AssignmentDataType.Integer => f.Random.Int(0, 100),
                AssignmentDataType.Float => f.Random.Double(0, 100),
                AssignmentDataType.Boolean => f.Random.Bool(),
                AssignmentDataType.URL => f.Internet.Url(),
                AssignmentDataType.JSON => JsonSerializer.Serialize(new
                {
                    Value = f.Random.Int(),
                    Collection = f.Random.WordsArray(2, 5),
                    SubObject = new
                    {
                        Value = f.Random.Float()
                    }
                }),
                _ => f.Lorem.Sentence()
            };
        }

        var deliveryFields = deliveries.Select(delivery =>
        {
            var deliveryId = delivery.Id;
            var assignmentId = delivery.AssignmentId;
            var courseId = assignments.First(a => a.Id == assignmentId).CourseId;

            var fieldsInAssignment = assignmentFields
                .Where(f => f.AssignmentId == delivery.AssignmentId)
                .ToList();

            return fieldsInAssignment.Select(field =>
            {
                return deliveryFieldFaker
                    .RuleFor(f => f.DeliveryId, delivery.Id)
                    .RuleFor(f => f.AssignmentFieldId, field.Id)
                    .RuleFor(f => f.Value, (f, deliveryField) =>
                    {
                        if (field.Type == AssignmentDataType.File)
                        {
                            var stream = new MemoryStream(Encoding.UTF8.GetBytes(f.Lorem.Paragraph(10)));
                            fileStorage.WriteDeliveryField
                            (
                                courseId,
                                assignmentId,
                                deliveryId,
                                deliveryField.Id,
                                stream
                            );

                            return new FileMetadata
                            {
                                FileName = f.Lorem.Word() + ".txt",
                                ContentType = MediaTypeNames.Text.Plain,
                            };
                        }

                        if (field.Type == AssignmentDataType.List)
                        {
                            return Enumerable.Range(0, f.Random.Int(1, 4))
                                .Select(_ => GetValue(field.SubType!.Value, f))
                                .ToList();
                        }

                        return GetValue(field.Type, f);
                    })
                    .Generate();
            });
        })
        .SelectMany(x => x)
        .ToList();

        dbContext.AddRange(deliveryFields);

        var feedbackFaker = new Faker<Feedback>()
            .UseSeed(SEED + 0)
            .RuleFor(x => x.Id, f => f.Random.Guid())
            .RuleFor(x => x.Comment, f => f.Lorem.Paragraphs(2));

        var approvalFeedbackFaker = new Faker<ApprovalFeedback>()
            .UseSeed(SEED + 1)
            .RuleFor(x => x.Id, f => f.Random.Guid())
            .RuleFor(x => x.Comment, f => f.Lorem.Paragraphs(2))
            .RuleFor(x => x.IsApproved, f => f.Random.Bool());

        var letterFeedbackFaker = new Faker<LetterFeedback>()
            .UseSeed(SEED + 2)
            .RuleFor(x => x.Id, f => f.Random.Guid())
            .RuleFor(x => x.Comment, f => f.Lorem.Paragraphs(2))
            .RuleFor(x => x.LetterGrade, f => f.Random.Enum<LetterGrade>());

        var pointsFeedbackFaker = new Faker<PointsFeedback>()
            .UseSeed(SEED + 3)
            .RuleFor(x => x.Id, f => f.Random.Guid())
            .RuleFor(x => x.Comment, f => f.Lorem.Paragraphs(2));

        var feedbacks = courses.Select(course =>
        {
            var studentsInCourse = courseStudents
                .Where(x => x.CourseId == course.Id)
                .Select(x => students.First(s => s.Id == x.StudentId))
                .ToList();

            var teamsInCourse = teams
                .Where(x => x.CourseId == course.Id)
                .ToList();

            var assignmentsInCourse = assignments
                .Where(a => a.CourseId == course.Id && a.Published)
                .ToList();

            return assignmentsInCourse
                .Where(a => a.DueDate < DateTime.UtcNow)
                .Select(assignment =>
                {
                    var isIndividual = assignment.CollaborationType == CollaborationType.Individual;
                    var ids = isIndividual ?
                        studentsInCourse.Select(s => s.Id).ToList() :
                        teamsInCourse.Select(s => s.Id).ToList();

                    return ids
                        .Take(ids.Count - 3)
                        .Select(id =>
                        {
                            return assignment.GradingType switch
                            {
                                GradingType.ApprovalGrading => approvalFeedbackFaker
                                    .RuleFor(x => x.AssignmentId, assignment.Id)
                                    .RuleFor(x => x.StudentId, isIndividual ? id : null)
                                    .RuleFor(x => x.TeamId, isIndividual ? null : id)
                                    .Generate(),

                                GradingType.LetterGrading => letterFeedbackFaker
                                    .RuleFor(x => x.AssignmentId, assignment.Id)
                                    .RuleFor(x => x.StudentId, isIndividual ? id : null)
                                    .RuleFor(x => x.TeamId, isIndividual ? null : id)
                                    .Generate(),

                                GradingType.PointsGrading => pointsFeedbackFaker
                                    .RuleFor(x => x.AssignmentId, assignment.Id)
                                    .RuleFor(x => x.StudentId, isIndividual ? id : null)
                                    .RuleFor(x => x.TeamId, isIndividual ? null : id)
                                    .RuleFor(x => x.Points, f => f.Random.Int(0, assignment.MaxPoints!.Value))
                                    .Generate(),

                                _ => feedbackFaker
                                    .RuleFor(x => x.AssignmentId, assignment.Id)
                                    .RuleFor(x => x.StudentId, isIndividual ? id : null)
                                    .RuleFor(x => x.TeamId, isIndividual ? null : id)
                                    .Generate()
                            };
                        });
                })
                .SelectMany(x => x);
        })
        .SelectMany(x => x)
        .ToList();

        dbContext.AddRange(feedbacks);

        await dbContext.SaveChangesAsync();
    }

    private static readonly string[] COURSES =
    [
        "Algorithms",
        "Databases",
        "Networks",
        "Data Structures",
        "Operating Systems",
        "Computer Architecture",
        "Software Engineering",
        "Artificial Intelligence",
        "Machine Learning",
        "Natural Language Processing",
        "Computer Vision",
        "Data Mining",
        "Cybersecurity",
        "Cryptography",
        "Cloud Computing",
        "Web Development",
        "Mobile Application Development",
        "Human-Computer Interaction",
        "Programming Languages",
        "Distributed Systems",
        "Parallel Computing",
        "Internet of Things",
        "Big Data Analytics",
        "Bioinformatics",
        "Quantum Computing",
        "Game Development",
        "Digital Signal Processing",
        "Embedded Systems",
        "Compiler Design",
        "Theory of Computation",
        "Information Retrieval",
        "Virtual Reality",
        "Augmented Reality",
    ];

    private static readonly string[] ASSIGNMENTS =
    [
        "To do app",
        "Database normalization",
        "HTTP server",
        "Chat application",
        "File compression tool",
        "Simple compiler",
        "Network packet analyzer",
        "Search engine implementation",
        "Sorting algorithm visualization",
        "Weather app with API integration",
        "Multiplayer game server",
        "Blockchain ledger simulation",
        "Text editor with syntax highlighting",
        "Image processing tool",
        "Spam email classifier",
        "Recommendation system",
        "Social media feed generator",
        "Mobile app for fitness tracking",
        "Encryption and decryption tool",
        "RESTful API development",
        "Online shopping cart system",
        "Cloud storage manager",
        "Virtual assistant chatbot",
        "URL shortening service",
        "File transfer protocol implementation",
        "Distributed hash table",
        "Personal finance tracker",
        "Memory allocation simulator",
        "Digital clock with alarms",
        "Language parser and grammar checker",
        "Machine learning model deployment",
        "Voice recognition app",
        "Data visualization dashboard",
        "Real-time traffic monitoring system",
        "Peer-to-peer file sharing network",
        "Content delivery network simulation",
        "Face detection and recognition",
        "Genetic algorithm for optimization",
        "SQL query optimizer",
        "Compiler for a toy programming language",
        "Code versioning tool",
        "Game engine prototype",
        "Pathfinding algorithm implementation",
        "IoT home automation system",
        "Virtual reality scene creator",
        "Augmented reality object placement",
        "Secure password manager",
        "Event scheduling system",
        "Digital signature generator",
        "AI chatbot for customer support",
        "Movie ticket booking system",
        "Multithreaded file downloader",
        "Cloud resource monitoring tool",
        "Interactive learning app",
        "Speech-to-text converter",
        "Handwriting recognition tool",
        "Real-time collaborative text editor",
        "Predictive text input system",
        "Simulation of a bank transaction system",
        "Sensor data aggregator for IoT devices",
        "News aggregator website",
        "Compiler error message generator",
        "Traffic light simulation",
        "Online voting system",
        "Pixel art creator",
        "Distributed database system",
        "Real-time video streaming app",
        "Web scraping tool",
        "Quiz platform with live leaderboard",
        "Music recommendation app",
        "AI-based grammar corrector",
        "Data clustering visualizer",
        "CPU scheduling simulator",
        "Virtual file system",
        "Dependency injection framework",
        "Blockchain-based voting system",
        "Autonomous vehicle route planner",
        "Fitness app with wearable integration",
        "Custom game level designer",
        "Secure messaging app",
        "Remote desktop application",
        "Electronic health record system",
        "Cache simulation tool",
        "Graph traversal visualizer",
        "Stock market prediction model",
        "Multi-factor authentication system",
        "User activity log analyzer",
        "Weather forecasting using machine learning",
        "API gateway implementation",
        "Simple IoT device emulator",
        "Data pipeline for ETL operations",
        "Interactive debugger for coding assignments",
        "Serverless app deployment pipeline",
        "Smart contract development for Ethereum",
        "AI-driven news summarizer",
        "Custom web browser prototype",
        "Augmented reality treasure hunt game",
        "Emotion detection from text",
        "Real-time earthquake alert system",
        "Crypto wallet simulator",
        "Dynamic DNS service implementation",
    ];
}

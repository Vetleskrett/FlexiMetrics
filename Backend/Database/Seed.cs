using Bogus;
using Database.Models;
using Microsoft.EntityFrameworkCore;

namespace Database;

public static class Seed
{
    private const int SEED = 123;

    public static async Task SeedDatabaseAsync(this DbContext dbContext)
    {
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

        await AddRangeIfNotExists
        (
            students.Union(teachers),
            user => dbContext.Set<User>().AnyAsync(x => x.Id == user.Id),
            user => dbContext.Set<User>().Add(user)
        );

        var courses = new Faker<Course>()
            .UseSeed(SEED)
            .RuleFor(x => x.Id, f => f.Random.Guid())
            .RuleFor(x => x.Code, f => "TDT" + f.Random.Number(1000, 9999))
            .RuleFor(x => x.Name, f => f.PickRandom(COURSES))
            .RuleFor(x => x.Year, 2025)
            .RuleFor(x => x.Semester, Semester.Spring)
            .Generate(10);

        await AddRangeIfNotExists
        (
            courses,
            course => dbContext.Set<Course>().AnyAsync(x => x.Id == course.Id),
            course => dbContext.Set<Course>().Add(course)
        );

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

        await AddRangeIfNotExists
        (
            courseTeachers,
            courseTeacher => dbContext.Set<CourseTeacher>()
                .AnyAsync(x => x.CourseId == courseTeacher.CourseId && x.TeacherId == courseTeacher.TeacherId),
            courseTeacher => dbContext.Set<CourseTeacher>().Add(courseTeacher)
        );

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

        await AddRangeIfNotExists
        (
            courseStudents,
            courseStudent => dbContext.Set<CourseStudent>()
                .AnyAsync(x => x.CourseId == courseStudent.CourseId && x.StudentId == courseStudent.StudentId),
            courseStudent => dbContext.Set<CourseStudent>().Add(courseStudent)
        );

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

        await AddRangeIfNotExists
        (
            teams,
            team => dbContext.Set<Team>().AnyAsync(x => x.Id == team.Id),
            team => dbContext.Set<Team>().Add(team)
        );

        var assignmentFaker = new Faker<Assignment>()
            .UseSeed(SEED)
            .RuleFor(x => x.Id, f => f.Random.Guid());

        var assignments = courses.Select(course =>
        {
            return assignmentFaker
                .RuleFor(x => x.Name, f => f.PickRandom(ASSIGNMENTS))
                .RuleFor(x => x.DueDate, f => f.Date.Between(new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc), new DateTime(2025, 6, 1, 0, 0, 0, DateTimeKind.Utc)))
                .RuleFor(x => x.Published, f => f.PickRandom(true, true, true, false))
                .RuleFor(x => x.CollaborationType, f => f.Random.Enum<CollaborationType>())
                .RuleFor(x => x.Mandatory, f => f.Random.Bool())
                .RuleFor(x => x.GradingFormat, f =>
                {
                    var type = f.Random.Enum<GradingType>();
                    return new GradingFormat
                    {
                        GradingType = type,
                        MaxPoints = type == GradingType.PointsGrading ? 10 * f.Random.Int(1, 10) : null,
                    };
                })
                .RuleFor(x => x.Description, f => f.Lorem.Paragraphs(2))
                .RuleFor(x => x.CourseId, course.Id)
                .GenerateBetween(2, 5);
        })
        .SelectMany(x => x)
        .ToList();

        await AddRangeIfNotExists
        (
            assignments,
            assignment => dbContext.Set<Assignment>().AnyAsync(x => x.Id == assignment.Id),
            assignment => dbContext.Set<Assignment>().Add(assignment)
        );

        var assignmentFieldFaker = new Faker<AssignmentField>()
            .UseSeed(SEED)
            .RuleFor(x => x.Id, f => f.Random.Guid());

        var assignmentFields = assignments.Select(assignment =>
        {
            return assignmentFieldFaker
                .RuleFor(x => x.Type, f => f.Random.Enum<AssignmentDataType>())
                .RuleFor(x => x.Name, f => f.Lorem.Word())
                .RuleFor(x => x.AssignmentId, assignment.Id)
                .GenerateBetween(3, 6);
        })
        .SelectMany(x => x)
        .ToList();

        await AddRangeIfNotExists
        (
            assignmentFields,
            field => dbContext.Set<AssignmentField>().AnyAsync(x => x.Id == field.Id),
            field => dbContext.Set<AssignmentField>().Add(field)
        );

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
                .Take(assignmentsInCourse.Count * 3 / 4)
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

        await AddRangeIfNotExists
        (
            deliveries,
            delivery => dbContext.Set<Delivery>().AnyAsync(x => x.Id == delivery.Id),
            delivery => dbContext.Set<Delivery>().Add(delivery)
        );

        var deliveryFieldFaker = new Faker<DeliveryField>()
            .UseSeed(SEED)
            .RuleFor(x => x.Id, f => f.Random.Guid());

        var deliveryFields = deliveries.Select(delivery =>
        {
            var fieldsInAssignment = assignmentFields
                .Where(f => f.AssignmentId == delivery.AssignmentId)
                .ToList();

            return fieldsInAssignment.Select(field =>
            {
                return deliveryFieldFaker
                    .RuleFor(f => f.DeliveryId, delivery.Id)
                    .RuleFor(f => f.AssignmentFieldId, field.Id)
                    .RuleFor(f => f.Value, f =>
                    {
                        return field.Type switch
                        {
                            AssignmentDataType.String => f.Lorem.Sentence(),
                            AssignmentDataType.Integer => f.Random.Int(0, 100),
                            AssignmentDataType.Double => f.Random.Double(0, 100),
                            AssignmentDataType.Boolean => f.Random.Bool(),
                            _ => f.Lorem.Sentence()
                        };
                    })
                    .Generate();
            });
        })
        .SelectMany(x => x)
        .ToList();

        await AddRangeIfNotExists
        (
            deliveryFields,
            field => dbContext.Set<DeliveryField>().AnyAsync(x => x.Id == field.Id),
            field => dbContext.Set<DeliveryField>().Add(field)
        );

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

        var feedbacks = deliveries
            .Take(deliveries.Count * 3 / 4)
            .Select(delivery =>
            {
                var assignment = assignments.First(a => a.Id == delivery.AssignmentId);
                return assignment.GradingFormat.GradingType switch
                {
                    GradingType.ApprovalGrading => approvalFeedbackFaker
                        .RuleFor(x => x.DeliveryId, delivery.Id)
                        .Generate(),

                    GradingType.LetterGrading => letterFeedbackFaker
                        .RuleFor(x => x.DeliveryId, delivery.Id)
                        .Generate(),

                    GradingType.PointsGrading => pointsFeedbackFaker
                        .RuleFor(x => x.DeliveryId, delivery.Id)
                        .RuleFor(x => x.Points, f => f.Random.Int(0, assignment.GradingFormat.MaxPoints!.Value))
                        .Generate(),

                    _ => feedbackFaker
                        .RuleFor(x => x.DeliveryId, delivery.Id)
                        .Generate()
                };
            })
            .ToList();

        await AddRangeIfNotExists
        (
            feedbacks,
            feedback => dbContext.Set<Feedback>().AnyAsync(x => x.Id == feedback.Id),
            feedback => dbContext.Set<Feedback>().Add(feedback)
        );

        await dbContext.SaveChangesAsync();
    }

    private static async Task AddRangeIfNotExists<T>(IEnumerable<T> items, Func<T, Task<bool>> containsItem, Action<T> addItem)
    {
        foreach (var item in items)
        {
            var contains = await containsItem(item);
            if (!contains)
            {
                addItem(item);
            }
        }
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

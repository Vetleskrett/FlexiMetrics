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
            .RuleFor(x => x.Role, f => Role.Student)
            .Generate(100);

        var teachers = userFaker
            .RuleFor(x => x.Role, f => Role.Teacher)
            .Generate(10);

        await AddRangeIfNotExists(dbContext, students);
        await AddRangeIfNotExists(dbContext, teachers);

        var courses = new Faker<Course>()
            .UseSeed(SEED)
            .RuleFor(x => x.Id, f => f.Random.Guid())
            .RuleFor(x => x.Code, f => "TDT" + f.Random.Number(1000, 9999))
            .RuleFor(x => x.Name, f => f.PickRandom(COURSES))
            .RuleFor(x => x.Year, f => 2025)
            .RuleFor(x => x.Semester, f => Semester.Spring)
            .RuleFor(x => x.Teachers, f => f.Random.ListItems(teachers, 3))
            .RuleFor(x => x.Students, f => f.Random.ListItems(students, 30))
            .Generate(10);

        await AddRangeIfNotExists(dbContext, courses);

        var teamFaker = new Faker<Team>()
            .UseSeed(SEED)
            .RuleFor(x => x.Id, f => f.Random.Guid());

        var teams = courses.Select(course =>
        {
            return course.Students!.Chunk(3).Select((students, index) =>
            {
                return teamFaker
                    .RuleFor(x => x.TeamNr, f => index + 1)
                    .RuleFor(x => x.CourseId, f => course.Id)
                    .RuleFor(x => x.Students, f => students.ToList())
                    .Generate();
            });
        })
        .SelectMany(x => x);

        await AddRangeIfNotExists(dbContext, teams);

        var assignmentFaker = new Faker<Assignment>()
            .UseSeed(SEED)
            .RuleFor(x => x.Id, f => f.Random.Guid());

        var assignments = courses.Select(course =>
        {
            return assignmentFaker
                .RuleFor(x => x.Name, f => f.PickRandom(ASSIGNMENTS))
                .RuleFor(x => x.DueDate, f => f.Date.Between(new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 6, 1, 0, 0, 0, DateTimeKind.Utc)))
                .RuleFor(x => x.Published, f => f.Random.Bool())
                .RuleFor(x => x.CollaborationType, f => f.Random.Enum<CollaborationType>())
                .RuleFor(x => x.CourseId, f => course.Id)
                .GenerateBetween(2, 5);
        })
        .SelectMany(x => x).ToList();

        var assignmentFieldFaker = new Faker<AssignmentField>()
            .UseSeed(SEED)
            .RuleFor(x => x.Id, f => f.Random.Guid());

        var assignmentFields = assignments.Select(assignment =>
        {
            var fields = assignmentFieldFaker
                .RuleFor(x => x.Type, f => f.Random.Enum<AssignmentDataType>())
                .RuleFor(x => x.Name, f => f.System.FileName().Split(".")[0])
                .RuleFor(x => x.AssignmentId, f => assignment.Id)
                .GenerateBetween(2, 5);
            assignment.Fields = fields;
            return fields;
        })
        .SelectMany(x => x);
        await AddRangeIfNotExists(dbContext, assignments);
        await AddRangeIfNotExists(dbContext, assignmentFields);

        await dbContext.SaveChangesAsync();
    }

    private static async Task AddRangeIfNotExists<T>(DbContext dbContext, IEnumerable<T> items) where T : class, IModel
    {
        var set = dbContext.Set<T>();

        foreach (var item in items)
        {
            var contains = await set.Where(x => x.Id == item.Id).AnyAsync();
            if (!contains)
            {
                set.Add(item);
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

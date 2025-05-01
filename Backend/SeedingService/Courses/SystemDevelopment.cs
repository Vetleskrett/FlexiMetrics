using Database.Models;
using FileStorage;
using Microsoft.EntityFrameworkCore;

namespace SeedingService.Courses;

public static class SystemDevelopment
{
    public static async Task Seed(DbContext dbContext, IFileStorage fileStorage, List<User> teachers, List<User> students)
    {
        var NUM_TEAMS = 5;
        var STUDENTS_PER_TEAM = (int)Math.Ceiling(students.Count / (float)NUM_TEAMS);
        var teamStudents = students.Chunk(STUDENTS_PER_TEAM);

        var course = new Course
        {
            Id = Guid.NewGuid(),
            Name = "System Development",
            Code = "TDT1005",
            Year = 2025,
            Semester = Semester.Spring,
        };
        dbContext.Add(course);
        dbContext.AddRange(teachers.Select(t => new CourseTeacher
        {
            CourseId = course.Id,
            TeacherId = t.Id,
        }));
        dbContext.AddRange(students.Select(s => new CourseStudent
        {
            CourseId = course.Id,
            StudentId = s.Id,
        }));

        var teams = teamStudents.Select((students, i) => new Team
        {
            Id = Guid.NewGuid(),
            CourseId = course.Id,
            TeamNr = i + 1,
            Students = students.ToList()
        }).ToList();
        dbContext.AddRange(teams);

        var assignment = new Assignment
        {
            Id = Guid.NewGuid(),
            CourseId = course.Id,
            Name = "Project",
            DueDate = DateTime.UtcNow + TimeSpan.FromDays(7),
            Published = true,
            CollaborationType = CollaborationType.Teams,
            Mandatory = true,
            GradingType = GradingType.LetterGrading,
            MaxPoints = null,
            Description = "In this assignment, you should develop a web-based Campus Event Management System that helps streamline the organization and participation of events within the university. The system should support multiple user roles, allowing event organizers to create and manage events, students to browse and register for them, and administrators to oversee the overall platform activity. Your solution should include features such as event approval workflows, an interactive calendar, RSVP and attendance tracking, and a notification system to keep users informed. You are also encouraged to implement additional functionality such as QR code check-ins or user feedback on events. The project will be developed collaboratively in teams following the agile methodology. Your deliverables will include the application, technical documentation, and a final presentation demonstrating your system and development process."
        };
        dbContext.Add(assignment);

        var assignmentField = new AssignmentField
        {
            Id = Guid.NewGuid(),
            AssignmentId = assignment.Id,
            Type = AssignmentDataType.URL,
            Name = "GitHub Repository",
            Min = null,
            Max = null,
            Regex = "\\.git$",
            SubType = null,
        };
        dbContext.Add(assignmentField);

        foreach (var team in teams)
        {
            var deliveryId = Guid.NewGuid();
            dbContext.Add(new Delivery
            {
                Id = deliveryId,
                AssignmentId = assignment.Id,
                TeamId = team.Id,
                StudentId = null,
                Fields =
                [
                    new DeliveryField
                    {
                        Id = Guid.NewGuid(),
                        DeliveryId = deliveryId,
                        AssignmentFieldId = assignmentField.Id,
                        Value = "https://github.com/Vetleskrett/FlexiMetrics.git"
                    }
                ]
            });
        }

        var analyzer = new Analyzer
        {
            Id = Guid.NewGuid(),
            AssignmentId = assignment.Id,
            Name = "Git Analyzer",
            Requirements = "GitPython\nmatplotlib",
            AptPackages = "git\ncloc",
            FileName = "git.py",
            State = AnalyzerState.Building,
        };
        dbContext.Add(analyzer);

        await dbContext.SaveChangesAsync();

        var script = File.OpenRead("Analyzers/git.py");
        await fileStorage.WriteAnalyzerScript(course.Id, assignment.Id, analyzer.Id, script);
    }
}

using Database.Models;
using FileStorage;
using Microsoft.EntityFrameworkCore;

namespace SeedingService.Courses;

public static class WebDevelopment
{
    private record DeliveryRecord(string Title, string Description, string HomePage, string[] ThreePages);

    public static async Task Seed(DbContext dbContext, IFileStorage fileStorage, List<User> teachers, List<User> students)
    {
        var NUM_TEAMS = 5;
        var STUDENTS_PER_TEAM = (int)Math.Ceiling(students.Count / (float)NUM_TEAMS);
        var teamStudents = students.Chunk(STUDENTS_PER_TEAM);

        var course = new Course
        {
            Id = Guid.NewGuid(),
            Name = "Web Development",
            Code = "TDT1001",
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
            Name = "Web application",
            DueDate = DateTime.UtcNow + TimeSpan.FromDays(7),
            Published = true,
            CollaborationType = CollaborationType.Teams,
            Mandatory = true,
            GradingType = GradingType.PointsGrading,
            MaxPoints = 100,
            Description = "In this assignment, you will design and implement a fully functional web application using React. You are free to choose the topic of your app, but your project must include at least 3 interactive features, a home page and at least 3 other unique pages. The main focus of this assignment is accessibility, good user experience (UX), and to follow best practices. Creativity is encouraged—build something useful, engaging, or unique!"
        };
        dbContext.Add(assignment);

        var fieldTitle = new AssignmentField
        {
            Id = Guid.NewGuid(),
            AssignmentId = assignment.Id,
            Type = AssignmentDataType.ShortText,
            Name = "Project Title",
            Min = null,
            Max = null,
            Regex = null,
            SubType = null,
        };
        var fieldDescription = new AssignmentField
        {
            Id = Guid.NewGuid(),
            AssignmentId = assignment.Id,
            Type = AssignmentDataType.LongText,
            Name = "Description",
            Min = null,
            Max = null,
            Regex = null,
            SubType = null,
        };
        var fieldHomePage = new AssignmentField
        {
            Id = Guid.NewGuid(),
            AssignmentId = assignment.Id,
            Type = AssignmentDataType.URL,
            Name = "Home Page",
            Min = null,
            Max = null,
            Regex = null,
            SubType = null,
        };
        var fieldThreePages = new AssignmentField
        {
            Id = Guid.NewGuid(),
            AssignmentId = assignment.Id,
            Type = AssignmentDataType.List,
            Name = "Three Pages",
            Min = 3,
            Max = 3,
            Regex = null,
            SubType = AssignmentDataType.URL,
        };
        dbContext.AddRange(fieldTitle, fieldDescription, fieldHomePage, fieldThreePages);

        DeliveryRecord[] deliveries =
        [
            new
            (
                "NTNU Website",
                "Website for NTNU",
                "https://www.ntnu.no",
                [
                    "https://www.ntnu.no/studier",
                    "https://www.ntnu.no/forskning",
                    "https://www.ntnu.no/student/trondheim"
                ]
            ),
            new
            (
                "UiO Website",
                "Website for UiO",
                "https://www.uio.no",
                [
                    "https://www.uio.no/studier",
                    "https://www.uio.no/forskning",
                    "https://www.uio.no/livet-rundt-studiene"
                ]
            ),
            new
            (
                "UiS Website",
                "Website for UiS",
                "https://www.uis.no/nb",
                [
                    "https://www.uis.no/nb/studier",
                    "https://www.uis.no/nb/forskning/forsking",
                    "https://www.uis.no/nb/studier/bli-student-ved-uis"
                ]
            ),
            new
            (
                "UiT Website",
                "Website for UiT",
                "https://uit.no/startsida",
                [
                    "https://uit.no/utdanning",
                    "https://uit.no/forskning",
                    "https://uit.no/ub/skriveogreferere"
                ]
            ),
            new
            (
                "UiB Website",
                "Website for UiB",
                "https://www.uib.no/",
                [
                    "https://www4.uib.no/studier",
                    "https://www.uib.no/forskning",
                    "https://www.uib.no/innovasjon"
                ]
            )
        ];

        foreach (var (delivery, team) in deliveries.Zip(teams))
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
                        AssignmentFieldId = fieldTitle.Id,
                        Value = delivery.Title
                    },
                    new DeliveryField
                    {
                        Id = Guid.NewGuid(),
                        DeliveryId = deliveryId,
                        AssignmentFieldId = fieldDescription.Id,
                        Value = delivery.Description
                    },
                    new DeliveryField
                    {
                        Id = Guid.NewGuid(),
                        DeliveryId = deliveryId,
                        AssignmentFieldId = fieldHomePage.Id,
                        Value = delivery.HomePage
                    },
                    new DeliveryField
                    {
                        Id = Guid.NewGuid(),
                        DeliveryId = deliveryId,
                        AssignmentFieldId = fieldThreePages.Id,
                        Value = delivery.ThreePages
                    }
                ]
            });
        }

        var analyzer = new Analyzer
        {
            Id = Guid.NewGuid(),
            AssignmentId = assignment.Id,
            Name = "Lighthouse Analyzer",
            Requirements = "",
            AptPackages = "npm\nchromium",
            FileName = "lighthouse.py",
            State = AnalyzerState.Standby,
        };
        dbContext.Add(analyzer);

        await dbContext.SaveChangesAsync();

        var script = File.OpenRead("Scripts/lighthouse.py");
        await fileStorage.WriteAnalyzerScript(course.Id, assignment.Id, analyzer.Id, script);
    }
}

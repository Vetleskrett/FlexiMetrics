using Database.Models;
using FileStorage;
using Microsoft.EntityFrameworkCore;
using SharpCompress.Common;
using SharpCompress.Writers;

namespace SeedingService.Courses;

public static class CppIntroductoryProgramming
{
    public static async Task Seed(DbContext dbContext, IFileStorage fileStorage, List<User> teachers, List<User> students)
    {
        var course = new Course
        {
            Id = Guid.NewGuid(),
            Name = "C++ introductory programming",
            Code = "TDT1003",
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

        var assignment = new Assignment
        {
            Id = Guid.NewGuid(),
            CourseId = course.Id,
            Name = "HTTP Server",
            DueDate = DateTime.UtcNow + TimeSpan.FromDays(7),
            Published = true,
            CollaborationType = CollaborationType.Individual,
            Mandatory = true,
            GradingType = GradingType.LetterGrading,
            MaxPoints = null,
            Description = "In this assignment, you will implement a simple Linux HTTP server from scratch using C++. The server should handle basic TCP connections, parse HTTP requests, and return appropriate HTTP responses. Your server must support the following endpoints:\r\n\r\nPOST ‘/api/items’\r\nAccepts a JSON body in the format { \"name\": \"Example 1\" } and stores the item in an in-memory list.\r\n\r\nGET ‘/api/items’\r\nReturns a JSON array of all stored items, e.g.,\r\n[ { \"name\": \"Example 1\" }, { \"name\": \"Example 2\" } ]\r\n\r\nYou are expected to manage the connection lifecycle, correctly interpret HTTP headers and bodies, and ensure responses comply with HTTP standards. This assignment focuses on applying core programming concepts such as input/output, memory management, and basic data structures while introducing you to foundational networking principles.\r\n"
        };
        dbContext.Add(assignment);

        var assignmentField = new AssignmentField
        {
            Id = Guid.NewGuid(),
            AssignmentId = assignment.Id,
            Type = AssignmentDataType.File,
            Name = "Source Code (.tar.gz)",
            Min = null,
            Max = null,
            Regex = null,
            SubType = null,
        };
        dbContext.Add(assignmentField);

        var tarStream = new MemoryStream();
        using (var writer = WriterFactory.Open(tarStream, ArchiveType.Tar, CompressionType.GZip))
        {
            foreach (var path in Directory.GetFiles("Deliveries/CppIntroductoryProgramming"))
            {
                var fileStream = File.OpenRead(path);
                writer.Write(Path.GetFileName(path), fileStream, DateTime.UtcNow);
            }
        }

        foreach (var student in students)
        {
            var deliveryId = Guid.NewGuid();

            var deliveryField = new DeliveryField
            {
                Id = Guid.NewGuid(),
                DeliveryId = deliveryId,
                AssignmentFieldId = assignmentField.Id,
                Value = new FileMetadata
                {
                    FileName = "src.tar.gz",
                    ContentType = "application/gzip"
                }
            };

            var delivery = new Delivery
            {
                Id = deliveryId,
                AssignmentId = assignment.Id,
                TeamId = null,
                StudentId = student.Id,
                Fields = [deliveryField]
            };
            dbContext.Add(delivery);

            tarStream.Position = 0;
            await fileStorage.WriteDeliveryField(course.Id, assignment.Id, delivery.Id, deliveryField.Id, tarStream);
        }

        var httpAnalyzer = new Analyzer
        {
            Id = Guid.NewGuid(),
            AssignmentId = assignment.Id,
            Name = "HTTP Analyzer",
            Requirements = "requests",
            AptPackages = "g++",
            FileName = "http.py",
            State = AnalyzerState.Building,
        };
        var codeAnalyzer = new Analyzer
        {
            Id = Guid.NewGuid(),
            AssignmentId = assignment.Id,
            Name = "Static Code Analyzer",
            Requirements = "pathvalidate",
            AptPackages = "cppcheck",
            FileName = "cppcheck.py",
            State = AnalyzerState.Building,
        };
        dbContext.AddRange(httpAnalyzer, codeAnalyzer);

        await dbContext.SaveChangesAsync();

        var httpScript = File.OpenRead("Analyzers/http.py");
        await fileStorage.WriteAnalyzerScript(course.Id, assignment.Id, httpAnalyzer.Id, httpScript);

        var codeScript = File.OpenRead("Analyzers/cppcheck.py");
        await fileStorage.WriteAnalyzerScript(course.Id, assignment.Id, codeAnalyzer.Id, codeScript);
    }
}

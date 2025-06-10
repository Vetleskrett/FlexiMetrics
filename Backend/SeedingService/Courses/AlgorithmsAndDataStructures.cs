using Database.Models;
using FileStorage;
using Microsoft.EntityFrameworkCore;

namespace SeedingService.Courses;

public static class AlgorithmsAndDataStructures
{
    public static async Task Seed(DbContext dbContext, IFileStorage fileStorage, List<User> teachers, List<User> students)
    {
        var course = new Course
        {
            Id = Guid.NewGuid(),
            Name = "Algorithms and Data Structures",
            Code = "TDT1002",
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
            Name = "Compression",
            DueDate = DateTime.UtcNow - TimeSpan.FromDays(7),
            Published = true,
            CollaborationType = CollaborationType.Individual,
            Mandatory = true,
            GradingType = GradingType.ApprovalGrading,
            MaxPoints = null,
            Description = "In this assignment, you will implement both a compression and decompression algorithm in Python. You are encouraged to research existing algorithms (e.g., Huffman coding, Run-Length Encoding, LZ77) and choose or adapt one to optimize your implementation. You are not allowed to use any external libraries.\r\n\r\nThe compression script should take an input file path and an output file path as command-line arguments and save a compressed version of the input file to the output file path.\r\n\r\nThe decompression script should take an input file path and an output file path as command-line arguments and save a decompressed version of the input file to the output file path.\r\n\r\nYour solution will be evaluated based on data compression ratio, execution speed and of course correctness."
        };
        dbContext.Add(assignment);

        var fieldCompression = new AssignmentField
        {
            Id = Guid.NewGuid(),
            AssignmentId = assignment.Id,
            Type = AssignmentDataType.File,
            Name = "Compression",
            Min = null,
            Max = null,
            Regex = null,
            SubType = null,
        };
        var fieldDecompressionn = new AssignmentField
        {
            Id = Guid.NewGuid(),
            AssignmentId = assignment.Id,
            Type = AssignmentDataType.File,
            Name = "Decompression",
            Min = null,
            Max = null,
            Regex = null,
            SubType = null,
        };
        dbContext.AddRange(fieldCompression, fieldDecompressionn);

        foreach (var student in students)
        {
            var deliveryId = Guid.NewGuid();

            var deliveryFieldCompression = new DeliveryField
            {
                Id = Guid.NewGuid(),
                DeliveryId = deliveryId,
                AssignmentFieldId = fieldCompression.Id,
                Value = new FileMetadata
                {
                    FileName = "compression.py",
                    ContentType = "text/plain"
                }
            };

            var deliveryFieldDecompression = new DeliveryField
            {
                Id = Guid.NewGuid(),
                DeliveryId = deliveryId,
                AssignmentFieldId = fieldDecompressionn.Id,
                Value = new FileMetadata
                {
                    FileName = "decompression.py",
                    ContentType = "text/plain"
                }
            };

            var delivery = new Delivery
            {
                Id = deliveryId,
                AssignmentId = assignment.Id,
                TeamId = null,
                StudentId = student.Id,
                Fields = [deliveryFieldCompression, deliveryFieldDecompression]
            };
            dbContext.Add(delivery);

            var compressionScript = File.OpenRead("Deliveries/AlgorithmsAndDataStructures/compression.py");
            await fileStorage.WriteDeliveryField(course.Id, assignment.Id, delivery.Id, deliveryFieldCompression.Id, compressionScript);

            var decompressionScript = File.OpenRead("Deliveries/AlgorithmsAndDataStructures/decompression.py");
            await fileStorage.WriteDeliveryField(course.Id, assignment.Id, delivery.Id, deliveryFieldDecompression.Id, decompressionScript);
        }

        var analyzer = new Analyzer
        {
            Id = Guid.NewGuid(),
            AssignmentId = assignment.Id,
            Name = "Compression Analyzer",
            Requirements = "Faker",
            AptPackages = "",
            FileName = "compression.py",
            State = AnalyzerState.Building,
        };
        dbContext.Add(analyzer);

        await dbContext.SaveChangesAsync();

        var script = File.OpenRead("Analyzers/compression.py");
        await fileStorage.WriteAnalyzerScript(course.Id, assignment.Id, analyzer.Id, script);
    }
}

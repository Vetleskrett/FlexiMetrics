using Database.Models;
using FileStorage;
using Microsoft.EntityFrameworkCore;

namespace SeedingService.Courses;

public static class Databases
{
    public static async Task Seed(DbContext dbContext, IFileStorage fileStorage, List<User> teachers, List<User> students)
    {
        var course = new Course
        {
            Id = Guid.NewGuid(),
            Name = "Databases",
            Code = "TDT1004",
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
            Name = "Normalization",
            DueDate = DateTime.UtcNow + TimeSpan.FromDays(7),
            Published = true,
            CollaborationType = CollaborationType.Individual,
            Mandatory = false,
            GradingType = GradingType.NoGrading,
            MaxPoints = null,
            Description = "In this assignment, you will apply database normalization principles to design and implement a relational schema for a vacation-house rental system. Your objective is to create a SQL file that defines the necessary tables, data types, and relationships to support the business logic of the system while ensuring the design adheres to at least the Third Normal Form (3NF). Your database should model the following core entities:\r\n\r\nHouse:\r\nEach house represents a vacation property available for rent. A house must have a unique ID, an address, a description, the number of rooms, maximum occupancy, and rental price per night. Every house is owned by a landlord.\r\n\r\nLandlord:\r\nLandlords are the owners of one or more houses. Each landlord has a unique ID, name, contact information (email, phone), and optionally a bank account for receiving payments.\r\n\r\nTenant:\r\nTenants are users who rent vacation houses. Each tenant has a unique ID, name, contact information, and may have multiple bookings over time.\r\n\r\nBooking:\r\nRepresents a booking of a house by a tenant. Each rental must include a unique rental ID, start and end dates, the ID of the house being rented, and the tenant's ID. A house can only be rented to one tenant at a time, there should be no overlapping bookings for the same house."
        };
        dbContext.Add(assignment);

        var fieldSql = new AssignmentField
        {
            Id = Guid.NewGuid(),
            AssignmentId = assignment.Id,
            Type = AssignmentDataType.File,
            Name = "SQL File",
            Min = null,
            Max = null,
            Regex = null,
            SubType = null,
        };
        var field1NF = new AssignmentField
        {
            Id = Guid.NewGuid(),
            AssignmentId = assignment.Id,
            Type = AssignmentDataType.LongText,
            Name = "Explain how you meet the criteria of 1NF",
            Min = null,
            Max = null,
            Regex = null,
            SubType = null,
        };
        var field2NF = new AssignmentField
        {
            Id = Guid.NewGuid(),
            AssignmentId = assignment.Id,
            Type = AssignmentDataType.LongText,
            Name = "Explain how you meet the criteria of 2NF",
            Min = null,
            Max = null,
            Regex = null,
            SubType = null,
        };
        var field3NF = new AssignmentField
        {
            Id = Guid.NewGuid(),
            AssignmentId = assignment.Id,
            Type = AssignmentDataType.LongText,
            Name = "Explain how you meet the criteria of 3NF",
            Min = null,
            Max = null,
            Regex = null,
            SubType = null,
        };
        dbContext.AddRange(fieldSql, field1NF, field2NF, field3NF);

        foreach (var student in students)
        {
            var deliveryId = Guid.NewGuid();

            var deliveryFieldSql = new DeliveryField
            {
                Id = Guid.NewGuid(),
                DeliveryId = deliveryId,
                AssignmentFieldId = fieldSql.Id,
                Value = new FileMetadata
                {
                    FileName = "script.sql",
                    ContentType = "text/plain"
                }
            };

            var delivery = new Delivery
            {
                Id = deliveryId,
                AssignmentId = assignment.Id,
                TeamId = null,
                StudentId = student.Id,
                Fields =
                [
                    deliveryFieldSql,
                    new DeliveryField
                    {
                        Id = Guid.NewGuid(),
                        DeliveryId = deliveryId,
                        AssignmentFieldId = field1NF.Id,
                        Value = "a single cell must not hold more than one value (atomicity)\r\nthere must be a primary key for identification\r\nno duplicated rows or columns\r\neach column must have only one value for each row in the table"
                    },
                    new DeliveryField
                    {
                        Id = Guid.NewGuid(),
                        DeliveryId = deliveryId,
                        AssignmentFieldId = field2NF.Id,
                        Value = "it’s already in 1NF\r\nhas no partial dependency. That is, all non-key attributes are fully dependent on a primary key."
                    },
                    new DeliveryField
                    {
                        Id = Guid.NewGuid(),
                        DeliveryId = deliveryId,
                        AssignmentFieldId = field3NF.Id,
                        Value = "be in 2NF\r\nhave no transitive partial dependency."
                    }
                ]
            };

            dbContext.Add(delivery);

            var sqlScript = File.OpenRead("Deliveries/Databases/script.sql");
            await fileStorage.WriteDeliveryField(course.Id, assignment.Id, delivery.Id, deliveryFieldSql.Id, sqlScript);
        }

        var analyzer = new Analyzer
        {
            Id = Guid.NewGuid(),
            AssignmentId = assignment.Id,
            Name = "SQL Analyzer",
            Requirements = "eralchemy\neralchemy[graphviz]",
            AptPackages = "graphviz",
            FileName = "sql.py",
            State = AnalyzerState.Building,
        };
        dbContext.Add(analyzer);

        await dbContext.SaveChangesAsync();

        var script = File.OpenRead("Analyzers/sql.py");
        await fileStorage.WriteAnalyzerScript(course.Id, assignment.Id, analyzer.Id, script);
    }
}

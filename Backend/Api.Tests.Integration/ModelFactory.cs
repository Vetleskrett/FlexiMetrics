using Database.Models;

namespace Api.Tests.Integration;

public static class ModelFactory
{
    public static Course GetValidCourse()
    {
        return new Course
        {
            Id = Guid.NewGuid(),
            Code = "TDT1001",
            Name = "Webutvikling",
            Year = 2025,
            Semester = Semester.Spring
        };
    }

    public static User GetValidStudent(string email = "student@ntnu.no")
    {
        return new User
        {
            Id = Guid.NewGuid(),
            Name = "Student Studentsen",
            Email = email,
            Role = Role.Student,
        };
    }

    public static User GetValidTeacher(string email = "teacher@ntnu.no")
    {
        return new User
        {
            Id = Guid.NewGuid(),
            Name = "Teacher Teachersen",
            Email = email,
            Role = Role.Teacher,
        };
    }

    public static CourseTeacher GetValidCourseTeacher(Guid courseId, Guid teacherId)
    {
        return new CourseTeacher
        {
            CourseId = courseId,
            TeacherId = teacherId
        };
    }

    public static CourseStudent GetValidCourseStudent(Guid courseId, Guid studentId)
    {
        return new CourseStudent
        {
            CourseId = courseId,
            StudentId = studentId
        };
    }

    public static Team GetValidTeam(Guid courseId, int teamNr = 1)
    {
        return new Team
        {
            Id = Guid.NewGuid(),
            CourseId = courseId,
            TeamNr = teamNr,
            Students = []
        };
    }

    public static Assignment GetValidAssignment(Guid courseId, bool published = true, TimeSpan offset = new())
    {
        return new Assignment
        {
            Id = Guid.NewGuid(),
            Name = "Frontend project",
            DueDate = new DateTime(2025, 5, 15, 0, 0, 0, DateTimeKind.Utc).Add(offset),
            Published = published,
            CollaborationType = CollaborationType.Individual,
            Mandatory = true,
            GradingFormat = new GradingFormat { GradingType = GradingType.LetterGrading, MaxPoints = null },
            Description = "Create a frontend project with svelte",
            CourseId = courseId,
        };
    }

    public static AssignmentField GetValidAssignmentField(Guid assignmentId)
    {
        return new AssignmentField
        {
            Id = Guid.NewGuid(),
            AssignmentId = assignmentId,
            Type = AssignmentDataType.String,
            Name = "Project title"
        };
    }

    public static Delivery GetValidStudentDelivery(Guid assignmentId, Guid studentId)
    {
        return new Delivery
        {
            Id = Guid.NewGuid(),
            AssignmentId = assignmentId,
            StudentId = studentId,
            TeamId = null,
            Fields = []
        };
    }

    public static Delivery GetValidTeamDelivery(Guid assignmentId, Guid teamId)
    {
        return new Delivery
        {
            Id = Guid.NewGuid(),
            AssignmentId = assignmentId,
            StudentId = null,
            TeamId = teamId,
            Fields = []
        };
    }

    public static DeliveryField GetValidDeliveryField(Guid deliveryId, AssignmentField assignmentField)
    {
        return new DeliveryField
        {
            Id = Guid.NewGuid(),
            AssignmentFieldId = assignmentField.Id,
            DeliveryId = deliveryId,
            Value = assignmentField.Type switch
            {
                AssignmentDataType.String => "My Frontend Project",
                AssignmentDataType.Integer => 16,
                AssignmentDataType.Double => 5.7,
                AssignmentDataType.Boolean => true,
                _ => throw new ArgumentException(),
            }
        };
    }

    public static Feedback GetValidFeedback(Guid deliveryId)
    {
        return new Feedback
        {
            Id = Guid.NewGuid(),
            Comment = "Looks good to me",
            DeliveryId = deliveryId
        };
    }
}
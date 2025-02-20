using Database;
using Database.Models;

namespace Api.Tests.Integration;

public class ModelFactory
{
    private readonly AppDbContext _dbContext;

    public ModelFactory(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Course CreateCourse(string code = "TDT1001")
    {
        var course = new Course
        {
            Id = Guid.NewGuid(),
            Code = code,
            Name = "Webutvikling",
            Year = 2025,
            Semester = Semester.Spring
        };
        _dbContext.Courses.Add(course);
        return course;
    }

    public List<Course> CreateCourses(int count)
    {
        return Enumerable.Range(1, count)
            .Select(i => CreateCourse("TDT1" + i.ToString().PadLeft(3, '0')))
            .ToList();
    }

    public User CreateStudent(string email = "student@ntnu.no")
    {
        var student = new User
        {
            Id = Guid.NewGuid(),
            Name = "Student Studentsen",
            Email = email,
            Role = Role.Student,
        };
        _dbContext.Users.Add(student);
        return student;
    }

    public List<User> CreateStudents(int count)
    {
        return Enumerable.Range(1, count)
            .Select(i => CreateStudent($"student{i}@ntnu.no"))
            .ToList();
    }

    public User CreateTeacher(string email = "teacher@ntnu.no")
    {
        var teacher = new User
        {
            Id = Guid.NewGuid(),
            Name = "Teacher Teachersen",
            Email = email,
            Role = Role.Teacher,
        };
        _dbContext.Users.Add(teacher);
        return teacher;
    }

    public List<User> CreateTeachers(int count)
    {
        return Enumerable.Range(1, count)
            .Select(i => CreateTeacher($"teacher{i}@ntnu.no"))
            .ToList();
    }

    public CourseTeacher CreateCourseTeacher(Guid courseId, Guid teacherId)
    {
        var courseTeacher = new CourseTeacher
        {
            CourseId = courseId,
            TeacherId = teacherId
        };
        _dbContext.CourseTeachers.Add(courseTeacher);
        return courseTeacher;
    }

    public List<User> CreateCourseTeachers(Guid courseId, int count)
    {
        var teachers = CreateTeachers(count);

        _dbContext.CourseTeachers.AddRange(teachers.Select(teacher => new CourseTeacher
        {
            CourseId = courseId,
            TeacherId = teacher.Id
        }));

        return teachers;
    }

    public CourseStudent CreateCourseStudent(Guid courseId, Guid studentId)
    {
        var courseStudent = new CourseStudent
        {
            CourseId = courseId,
            StudentId = studentId
        };
        _dbContext.CourseStudents.Add(courseStudent);
        return courseStudent;
    }

    public List<User> CreateCourseStudents(Guid courseId, int count)
    {
        var students = CreateStudents(count);

        _dbContext.CourseStudents.AddRange(students.Select(student => new CourseStudent
        {
            CourseId = courseId,
            StudentId = student.Id
        }));

        return students;
    }

    public Team CreateTeam(Guid courseId, int teamNr = 1, List<User>? students = null)
    {
        var team = new Team
        {
            Id = Guid.NewGuid(),
            CourseId = courseId,
            TeamNr = teamNr,
            Students = students ?? []
        };
        _dbContext.Teams.Add(team);
        return team;
    }

    public List<Team> CreateTeams(Guid courseId, int count)
    {
        return Enumerable.Range(1, count)
            .Select(i => CreateTeam(courseId, i))
            .ToList();
    }

    public List<Team> CreateTeamsWithStudents(Guid courseId, List<User> students, int count)
    {
        var numStudentsPerTeam = (int)Math.Ceiling(students.Count / (float)count);
        var teams = students.Chunk(numStudentsPerTeam).Select((members, i) =>
        {
            return CreateTeam(courseId, i + 1, members.ToList());
        });
        return teams.ToList();
    }

    public Assignment CreateAssignment
    (
        Guid courseId,
        bool published = true,
        TimeSpan offset = new(),
        CollaborationType collaboration = CollaborationType.Individual,
        GradingType gradingType = GradingType.LetterGrading,
        int? maxPoints = null
    )
    {
        var assignment = new Assignment
        {
            Id = Guid.NewGuid(),
            Name = "Frontend project",
            DueDate = DateTime.UtcNow.AddDays(1).Add(offset),
            Published = published,
            CollaborationType = collaboration,
            Mandatory = true,
            GradingType = gradingType,
            MaxPoints = maxPoints,
            Description = "Create a frontend project with svelte",
            CourseId = courseId,
        };
        _dbContext.Assignments.Add(assignment);
        return assignment;
    }

    public List<Assignment> CreateAssignments(Guid courseId, int count, bool published = true)
    {
        return Enumerable.Range(1, count)
            .Select(i => CreateAssignment(courseId, published, TimeSpan.FromDays(i)))
            .ToList();
    }

    public AssignmentField CreateAssignmentField(Guid assignmentId, AssignmentDataType dataType = AssignmentDataType.ShortText)
    {
        var assignmentField = new AssignmentField
        {
            Id = Guid.NewGuid(),
            AssignmentId = assignmentId,
            Type = dataType,
            Name = "Project title",
            Min = null,
            Max = null,
            Regex = null
        };
        _dbContext.AssignmentFields.Add(assignmentField);
        return assignmentField;
    }

    public List<AssignmentField> CreateAssignmentFields(Guid assignmentId, int count)
    {
        return Enumerable.Range(1, count)
            .Select(i => CreateAssignmentField(assignmentId))
            .ToList();
    }

    public Delivery CreateStudentDelivery(Guid assignmentId, Guid studentId)
    {
        var delivery = new Delivery
        {
            Id = Guid.NewGuid(),
            AssignmentId = assignmentId,
            StudentId = studentId,
            TeamId = null,
            Fields = []
        };
        _dbContext.Deliveries.Add(delivery);
        return delivery;
    }

    public Delivery CreateStudentDeliveryWithFields(Guid assignmentId, List<AssignmentField> assignmentFields, Guid studentId)
    {
        var delivery = CreateStudentDelivery(assignmentId, studentId);

        var deliveryFields = assignmentFields
                .Select(assignmentField => CreateDeliveryField(delivery.Id, assignmentField))
                .ToList();
        _dbContext.DeliveryFields.AddRange(deliveryFields);

        return delivery;
    }

    public Delivery CreateTeamDelivery(Guid assignmentId, Guid teamId)
    {
        var delivery = new Delivery
        {
            Id = Guid.NewGuid(),
            AssignmentId = assignmentId,
            StudentId = null,
            TeamId = teamId,
            Fields = []
        };
        _dbContext.Deliveries.Add(delivery);
        return delivery;
    }

    public Delivery CreateTeamDeliveryWithFields(Guid assignmentId, List<AssignmentField> assignmentFields, Guid teamId)
    {
        var delivery = CreateTeamDelivery(assignmentId, teamId);

        var deliveryFields = assignmentFields
                .Select(assignmentField => CreateDeliveryField(delivery.Id, assignmentField))
                .ToList();
        _dbContext.DeliveryFields.AddRange(deliveryFields);

        return delivery;
    }

    public DeliveryField CreateDeliveryField(Guid deliveryId, AssignmentField assignmentField)
    {
        var deliveryField = new DeliveryField
        {
            Id = Guid.NewGuid(),
            AssignmentFieldId = assignmentField.Id,
            DeliveryId = deliveryId,
            Value = assignmentField.Type switch
            {
                AssignmentDataType.ShortText => "My Frontend Project",
                AssignmentDataType.Integer => 16,
                AssignmentDataType.Float => 5.7,
                AssignmentDataType.Boolean => true,
                _ => throw new ArgumentException(),
            }
        };
        _dbContext.DeliveryFields.Add(deliveryField);
        return deliveryField;
    }

    public List<Delivery> CreateStudentDeliveries(Guid assignmentId, List<User> students)
    {
        var deliveries = students
            .Select(student => CreateStudentDelivery(assignmentId, student.Id))
            .ToList();
        _dbContext.Deliveries.AddRange(deliveries);
        return deliveries;
    }

    public List<Delivery> CreateStudentDeliveriesWithFields(Guid assignmentId, List<AssignmentField> assignmentFields, List<User> students)
    {
        var deliveries = students
            .Select(student => CreateStudentDelivery(assignmentId, student.Id))
            .ToList();
        _dbContext.Deliveries.AddRange(deliveries);

        var deliveryFields = deliveries.Select(delivery =>
        {
            delivery.Fields = assignmentFields
                .Select(assignmentField => CreateDeliveryField(delivery.Id, assignmentField))
                .ToList();

            return delivery.Fields;
        })
        .SelectMany(x => x);
        _dbContext.DeliveryFields.AddRange(deliveryFields);

        return deliveries;
    }

    public List<Delivery> CreateTeamDeliveries(Guid assignmentId, List<Team> teams)
    {
        var deliveries = teams
            .Select(team => CreateTeamDelivery(assignmentId, team.Id))
            .ToList();
        _dbContext.Deliveries.AddRange(deliveries);
        return deliveries;
    }

    public List<Delivery> CreateTeamDeliveriesWithFields(Guid assignmentId, List<AssignmentField> assignmentFields, List<Team> teams)
    {
        var deliveries = teams
            .Select(team => CreateTeamDelivery(assignmentId, team.Id))
            .ToList();
        _dbContext.Deliveries.AddRange(deliveries);

        var deliveryFields = deliveries.Select(delivery =>
        {
            delivery.Fields = assignmentFields
                .Select(assignmentField => CreateDeliveryField(delivery.Id, assignmentField))
                .ToList();

            return delivery.Fields;
        })
        .SelectMany(x => x);
        _dbContext.DeliveryFields.AddRange(deliveryFields);

        return deliveries;
    }

    public Feedback CreateFeedback(Guid assignmentId, Guid? studentId, Guid? teamId)
    {
        var feedback = new Feedback
        {
            Id = Guid.NewGuid(),
            Comment = "Looks good to me",
            AssignmentId = assignmentId,
            StudentId = studentId,
            TeamId = teamId,
        };
        _dbContext.Feedbacks.Add(feedback);
        return feedback;
    }

    public List<Feedback> CreateFeedbacks(Guid assignmentId, List<User> students)
    {
        return students
            .Select(student => CreateFeedback(assignmentId, student.Id, null))
            .ToList();
    }

    public List<Feedback> CreateFeedbacks(Guid assignmentId, List<Team> teams)
    {
        return teams
            .Select(team => CreateFeedback(assignmentId, null, team.Id))
            .ToList();
    }

    public ApprovalFeedback CreateApprovalFeedback(Guid assignmentId, Guid? studentId, Guid? teamId)
    {
        var feedback = new ApprovalFeedback
        {
            Id = Guid.NewGuid(),
            Comment = "Looks good to me",
            AssignmentId = assignmentId,
            StudentId = studentId,
            TeamId = teamId,
            IsApproved = true
        };
        _dbContext.Feedbacks.Add(feedback);
        return feedback;
    }

    public List<ApprovalFeedback> CreateApprovalFeedbacks(Guid assignmentId, List<User> students)
    {
        return students
            .Select(student => CreateApprovalFeedback(assignmentId, student.Id, null))
            .ToList();
    }

    public List<ApprovalFeedback> CreateApprovalFeedbacks(Guid assignmentId, List<Team> teams)
    {
        return teams
            .Select(team => CreateApprovalFeedback(assignmentId, null, team.Id))
            .ToList();
    }

    public LetterFeedback CreateLetterFeedback(Guid assignmentId, Guid? studentId, Guid? teamId)
    {
        var feedback = new LetterFeedback
        {
            Id = Guid.NewGuid(),
            Comment = "Looks good to me",
            AssignmentId = assignmentId,
            StudentId = studentId,
            TeamId = teamId,
            LetterGrade = LetterGrade.C
        };
        _dbContext.Feedbacks.Add(feedback);
        return feedback;
    }

    public List<LetterFeedback> CreateLetterFeedbacks(Guid assignmentId, List<User> students)
    {
        return students
            .Select(student => CreateLetterFeedback(assignmentId, student.Id, null))
            .ToList();
    }

    public List<LetterFeedback> CreateLetterFeedbacks(Guid assignmentId, List<Team> teams)
    {
        return teams
            .Select(team => CreateLetterFeedback(assignmentId, null, team.Id))
            .ToList();
    }

    public PointsFeedback CreatePointsFeedback(Guid assignmentId, Guid? studentId, Guid? teamId)
    {
        var feedback = new PointsFeedback
        {
            Id = Guid.NewGuid(),
            Comment = "Looks good to me",
            AssignmentId = assignmentId,
            StudentId = studentId,
            TeamId = teamId,
            Points = 75
        };
        _dbContext.Feedbacks.Add(feedback);
        return feedback;
    }

    public List<PointsFeedback> CreatePointsFeedbacks(Guid assignmentId, List<User> students)
    {
        return students
            .Select(student => CreatePointsFeedback(assignmentId, student.Id, null))
            .ToList();
    }

    public List<PointsFeedback> CreatePointsFeedbacks(Guid assignmentId, List<Team> teams)
    {
        return teams
            .Select(team => CreatePointsFeedback(assignmentId, null, team.Id))
            .ToList();
    }
}
using Database;
using Database.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Api.Authorization;

public class UserRequirement : IAuthorizationRequirement
{
    public string Policy { get; }
    public UserRequirement(string policy) { Policy = policy; }
}

public class UserAuthorizationHandler : AuthorizationHandler<UserRequirement>
{
    private readonly AppDbContext _dbContext;
    private readonly ILogger<UserAuthorizationHandler> _logger;

    public UserAuthorizationHandler(AppDbContext dbContext, ILogger<UserAuthorizationHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, UserRequirement requirement)
    {
        // For development purposes
        // context.Succeed(requirement);
        // return;

        var claimedId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        _logger.LogInformation("Finding User");
        if (string.IsNullOrEmpty(claimedId) || !(context.Resource is HttpContext httpContext) || !Guid.TryParse(claimedId, out Guid userId))
        {
            context.Fail();
            return;
        }
        _logger.LogInformation("User found: " + userId);


        switch (requirement.Policy)
        {
            case "admin":
                //TODO: Find solution to endpoints like GetAll... 
                break;
            case "course":
                var courseId = Guid.Parse(httpContext.Request.RouteValues["courseId"]?.ToString());
                if (!await InCourse(courseId, userId))
                {
                    context.Fail();
                }
                break;
            case "teacher":
                if (!await IsTeacher(userId))
                {
                    context.Fail();
                }
                break;
            case "teacherId":
                var teacherId = Guid.Parse(httpContext.Request.RouteValues["teacherId"]?.ToString());
                _logger.LogInformation("Checking if " + userId + " is " + teacherId);
                if (!IsClaimedId(teacherId, userId))
                {
                    context.Fail();
                    _logger.LogInformation("User is not the same");
                }
                break;
            case "studentId":
                var studentId = Guid.Parse(httpContext.Request.RouteValues["studentId"]?.ToString());
                _logger.LogInformation("Checking if " + userId + " is " + studentId);
                if (!IsClaimedId(studentId, userId))
                {
                    context.Fail();
                    _logger.LogInformation("User is not the same");
                }
                break;
            case "teacherCourse":
                courseId = Guid.Parse(httpContext.Request.RouteValues["courseId"]?.ToString());
                if (!await IsTeacherInCourse(courseId, userId))
                {
                    context.Fail();
                    _logger.LogInformation("User is not the same");
                }
                break;
            case "teacherTeam":
                var teamId = Guid.Parse(httpContext.Request.RouteValues["teamId"]?.ToString());
                if (!await IsTeacherForTeam(teamId, userId))
                {
                    context.Fail();
                }
                break;
            case "studentTeam":
                teamId = Guid.Parse(httpContext.Request.RouteValues["teamId"]?.ToString());
                if (!await IsStudentInTeam(teamId, userId))
                {
                    context.Fail();
                }
                break;
            case "studentCourse":
                courseId = Guid.Parse(httpContext.Request.RouteValues["courseId"]?.ToString());
                if (!await IsStudentInCourse(courseId, userId))
                {
                    context.Fail();
                }
                break;
            case "delivery":
                var deliveryId = Guid.Parse(httpContext.Request.RouteValues["deliveryId"]?.ToString());
                if (!(await IsStudentDelivery(deliveryId, userId) || await IsTeacherForDelivery(deliveryId, userId)))
                {
                    context.Fail();
                }
                break;
            case "deliveryField":
                var deliveryFieldId = Guid.Parse(httpContext.Request.RouteValues["deliveryFieldId"]?.ToString());
                if (!await InDeliveryField(deliveryFieldId, userId))
                {
                    context.Fail();
                }
                break;
            case "feedback":
                var feedbackId = Guid.Parse(httpContext.Request.RouteValues["feedbackId"]?.ToString());
                if (!(await IsTeacherForFeedback(feedbackId, userId) || await FeedbackForStudent(feedbackId, userId)))
                {
                    context.Fail();
                }
                break;
            case "feedbackTeacher":
                feedbackId = Guid.Parse(httpContext.Request.RouteValues["feedbackId"]?.ToString());
                if (!await IsTeacherForFeedback(feedbackId, userId))
                {
                    context.Fail();
                }
                break;
            case "assignmentTeacherOrStudent":
                var assignmentId = Guid.Parse(httpContext.Request.RouteValues["assignmentId"]?.ToString());
                studentId = Guid.Parse(httpContext.Request.RouteValues["studentId"]?.ToString());
                if (!(await IsTeacherForAssignment(assignmentId, userId) || IsClaimedId(studentId, userId)))
                {
                    context.Fail();
                }
                break;

            case "assignmentTeacherOrTeam":
                assignmentId = Guid.Parse(httpContext.Request.RouteValues["assignmentId"]?.ToString());
                teamId = Guid.Parse(httpContext.Request.RouteValues["teamId"]?.ToString());
                if (!(await IsTeacherForAssignment(assignmentId, userId) || await IsStudentInTeam(teamId, userId)))
                {
                    context.Fail();
                }
                break;
            case "assignmentTeacher":
                assignmentId = Guid.Parse(httpContext.Request.RouteValues["assignmentId"]?.ToString());
                if (!await IsTeacherForAssignment(assignmentId, userId))
                {
                    context.Fail();
                }
                break;
            case "assignmentFieldTeacher":
                var assignmentFieldId = Guid.Parse(httpContext.Request.RouteValues["assignmentFieldId"]?.ToString());
                if (!await IsTeacherForAssignmentField(assignmentFieldId, userId))
                {
                    context.Fail();
                }
                break;
            case "assignment":
                assignmentId = Guid.Parse(httpContext.Request.RouteValues["assignmentId"]?.ToString());
                if (!(await IsTeacherForAssignment(assignmentId, userId) || await IsStudentForAssignment(assignmentId, userId)))
                {
                    context.Fail();
                }
                break;
            case "analyzer":
                var analyzerId = Guid.Parse(httpContext.Request.RouteValues["analyzerId"]?.ToString());
                if (!await IsTeacherForAnalyzer(analyzerId, userId))
                {
                    context.Fail();
                }
                break;
            case "analysis":
                var analysisId = Guid.Parse(httpContext.Request.RouteValues["analysisId"]?.ToString());
                if (!await IsTeacherForAnalyzer(analysisId, userId))
                {
                    context.Fail();
                }
                break;
            default:
                context.Fail();
                break;
        }

        context.Succeed(requirement);
    }

    private async Task<bool> IsTeacher(Guid userId)
    {
        var teacher = await _dbContext.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == userId && u.Role == Role.Teacher);
        return teacher != null;
    }

    private async Task<bool> InCourse(Guid courseId, Guid userId)
    {
        var course = await _dbContext.Courses.AsNoTracking().Include(c => c.CourseTeachers!).Include(c => c.CourseStudents!).FirstOrDefaultAsync(c => c.Id == courseId);

        if (course == null)
        {
            return false;
        }
        if (course.CourseTeachers != null && course.CourseTeachers.Any(t => t.TeacherId == userId))
        {
            return true;
        }
        if (course.CourseStudents != null && course.CourseStudents.Any(t => t.StudentId == userId))
        {
            return true;
        }
        return false;
    }

    private async Task<bool> IsTeacherInCourse(Guid courseId, Guid userId)
    {
        var course = await _dbContext.Courses.AsNoTracking().Include(c => c.CourseTeachers!).FirstOrDefaultAsync(c => c.Id == courseId);

        if (course == null || course.CourseTeachers == null || !course.CourseTeachers.Any(t => t.TeacherId == userId))
        {
            return false;
        }
        return true;
    }

    private async Task<bool> IsTeacherForTeam(Guid teamId, Guid userId)
    {
        var team = await _dbContext.Teams.AsNoTracking().Include(t => t.Course!).ThenInclude(c => c.CourseTeachers).FirstOrDefaultAsync(c => c.Id == teamId);

        if (team == null || team.Course == null || team.Course.CourseTeachers == null || !team.Course.CourseTeachers.Any(t => t.TeacherId == userId))
        {
            return false;
        }
        return true;
    }

    private bool IsClaimedId(Guid claimedId, Guid userId)
    {
        return claimedId.Equals(userId);
    }


    private async Task<bool> IsStudentInCourse(Guid courseId, Guid userId)
    {
        var course = await _dbContext.Courses.AsNoTracking().Include(c => c.CourseStudents!).FirstOrDefaultAsync(c => c.Id == courseId);

        if (course == null || course.CourseStudents == null || !course.CourseStudents.Any(t => t.StudentId == userId))
        {
            return false;
        }
        return true;
    }
    private async Task<bool> IsStudentInTeam(Guid teamId, Guid userId)
    {
        var team = await _dbContext.Teams.AsNoTracking().FirstOrDefaultAsync(t => t.Id == teamId);

        if (team == null || !team.Students.Any(s => s.Id == userId))
        {
            return false;
        }
        return true;
    }

    private async Task<bool> IsStudentDelivery(Guid deliveryId, Guid userId)
    {
        var delivery = await _dbContext.Deliveries.AsNoTracking().Include(d => d.Team).ThenInclude(t => t!.Students).FirstOrDefaultAsync(t => t.Id == deliveryId);

        if (delivery == null || delivery.StudentId != userId || delivery.Team == null || !delivery.Team.Students.Any(s => s.Id == userId))
        {
            return false;
        }
        return true;
    }

    private async Task<bool> IsTeacherForDelivery(Guid deliveryId, Guid userId)
    {
        var delivery = await _dbContext.Deliveries.AsNoTracking().Include(d => d.Assignment).ThenInclude(a => a!.Course).ThenInclude(c => c!.CourseTeachers).FirstOrDefaultAsync(t => t.Id == deliveryId);

        if (delivery == null
            || delivery.Assignment == null
            || delivery.Assignment.Course == null
            || delivery.Assignment.Course.CourseTeachers == null
            || !delivery.Assignment.Course.CourseTeachers.Any(t => t.TeacherId == userId))
        {
            return false;
        }
        return true;
    }


    private async Task<bool> InDeliveryField(Guid deliveryId, Guid userId)
    {
        var deliveryField = await _dbContext.DeliveryFields.AsNoTracking()
            .Include(d => d.Delivery)
            .ThenInclude(d => d!.Team)
            .ThenInclude(t => t!.Students)
            .Include(d => d.Delivery)
            .ThenInclude(d => d!.Assignment)
            .ThenInclude(a => a!.Course)
            .ThenInclude(c => c.CourseTeachers).FirstOrDefaultAsync(t => t.Id == deliveryId);
        if (deliveryField == null || deliveryField.Delivery == null)
        {
            return false;
        }

        if (deliveryField.Delivery.StudentId == userId ||
            (deliveryField.Delivery.Team != null && deliveryField.Delivery.Team.Students.Any(s => s.Id == userId)))
        {
            return true;
        }

        if (deliveryField.Delivery.Assignment != null
            && deliveryField.Delivery.Assignment.Course != null
            && deliveryField.Delivery.Assignment.Course.CourseTeachers != null
            && deliveryField.Delivery.Assignment.Course.CourseTeachers.Any(t => t.TeacherId == userId))
        {
            return true;
        }
        return false;
    }


    private async Task<bool> IsTeacherForAssignment(Guid assignmentId, Guid userId)
    {
        var assignment = await _dbContext.Assignments.AsNoTracking()
            .Include(a => a!.Course)
            .ThenInclude(c => c!.CourseTeachers)
            .FirstOrDefaultAsync(f => f.Id == assignmentId);

        if (assignment == null ||
            assignment.Course == null ||
            assignment.Course.CourseTeachers == null ||
            !assignment.Course.CourseTeachers.Any(t => t.TeacherId == userId))
        {
            return false;
        }
        return true;
    }

    private async Task<bool> IsTeacherForAssignmentField(Guid assignmentId, Guid userId)
    {
        var assignmentField = await _dbContext.AssignmentFields.AsNoTracking()
            .Include(af => af!.Assignment)
            .ThenInclude(a => a!.Course)
            .ThenInclude(c => c!.CourseTeachers)
            .FirstOrDefaultAsync(f => f.Id == assignmentId);

        if (assignmentField == null ||
            assignmentField.Assignment == null ||
            assignmentField.Assignment.Course == null ||
            assignmentField.Assignment.Course.CourseTeachers == null ||
            !assignmentField.Assignment.Course.CourseTeachers.Any(t => t.TeacherId == userId))
        {
            return false;
        }
        return true;
    }

    private async Task<bool> IsStudentForAssignment(Guid assignmentId, Guid userId)
    {
        var assignment = await _dbContext.Assignments.AsNoTracking()
            .Include(a => a!.Course)
            .ThenInclude(c => c!.CourseStudents)
            .FirstOrDefaultAsync(f => f.Id == assignmentId);

        if (assignment == null ||
            assignment.Course == null ||
            assignment.Course.CourseStudents == null ||
            !assignment.Course.CourseStudents.Any(t => t.StudentId == userId))
        {
            return false;
        }
        return true;
    }

    private async Task<bool> FeedbackForStudent(Guid feedbackId, Guid userId)
    {
        var feedback = await _dbContext.Feedbacks.AsNoTracking()
            .Include(f => f.Team)
            .ThenInclude(t => t!.Students)
            .FirstOrDefaultAsync(f => f.Id == feedbackId);

        if (feedback == null)
        {
            return false;
        }
        if (feedback.StudentId == userId)
        {
            return true;
        }
        if (feedback.Team != null && feedback.Team != null && feedback.Team.Students.Any(t => t.Id == userId))
        {
            return true;
        }
        return false;
    }

    private async Task<bool> IsTeacherForFeedback(Guid feedbackId, Guid userId)
    {
        var feedback = await _dbContext.Feedbacks.AsNoTracking()
            .Include(f => f.Assignment)
            .ThenInclude(a => a!.Course)
            .ThenInclude(c => c!.CourseTeachers)
            .FirstOrDefaultAsync(f => f.Id == feedbackId);

        if (feedback == null ||
            feedback.Assignment == null ||
            feedback.Assignment.Course == null ||
            feedback.Assignment.Course.CourseTeachers == null ||
            !feedback.Assignment.Course.CourseTeachers.Any(t => t.TeacherId == userId))
        {
            return false;
        }
        return true;
    }
    private async Task<bool> IsTeacherForAnalyzer(Guid analyzerId, Guid userId)
    {
        var analyzer = await _dbContext.Analyzers.AsNoTracking()
            .Include(a => a.Assignment)
            .ThenInclude(a => a!.Course)
            .ThenInclude(c => c!.CourseTeachers)
            .FirstOrDefaultAsync(a => a.Id == analyzerId);

        if (analyzer == null ||
            analyzer.Assignment == null ||
            analyzer.Assignment.Course == null ||
            analyzer.Assignment.Course.CourseTeachers == null ||
            !analyzer.Assignment.Course.CourseTeachers.Any(t => t.TeacherId == userId))
        {
            return false;
        }
        return true;
    }

    private async Task<bool> IsTeacherForAnalysis(Guid analyzerId, Guid userId)
    {
        var analysis = await _dbContext.Analyses.AsNoTracking()
            .Include(a => a.Analyzer)
            .ThenInclude(a => a!.Assignment)
            .ThenInclude(a => a!.Course)
            .ThenInclude(c => c!.CourseTeachers)
            .FirstOrDefaultAsync(a => a.Id == analyzerId);

        if (analysis == null ||
            analysis.Analyzer == null ||
            analysis.Analyzer.Assignment == null ||
            analysis.Analyzer.Assignment.Course == null ||
            analysis.Analyzer.Assignment.Course.CourseTeachers == null ||
            !analysis.Analyzer.Assignment.Course.CourseTeachers.Any(t => t.TeacherId == userId))
        {
            return false;
        }
        return true;
    }
}

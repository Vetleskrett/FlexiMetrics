using Database.Models;

namespace Api.Assignments.Contracts
{
    public static class AssignmentMapping
    {
        public static Assignment MapToAssignment(this CreateAssignmentRequest request)
        {
            return new Assignment
            {
                Id = Guid.NewGuid(),
                CourseId = request.CourseId,
                DueDate = request.DueDate,
                Name = request.Name,
            };
        }

        public static Assignment MapToAssignment(this UpdateAssignmentRequest request, Guid id)
        {
            return new Assignment
            {
                Id = id,
                CourseId = request.CourseId,
                DueDate = request.DueDate,
                Name = request.Name,
            };
        }

        public static AssignmentResponse MapToResponse(this Assignment assignment)
        {
            return new AssignmentResponse
            {
                Id = assignment.Id,
                Name = assignment.Name,
                DueDate = assignment.DueDate,
                CourseId = assignment.CourseId,
            };
        }

        public static IEnumerable<AssignmentResponse> MapToResponse(this IEnumerable<Assignment> assignments)
        {
            return assignments.Select(assignment => assignment.MapToResponse());
        }


        public static AssignmentResponse MapToResponse(this Assignment assignment, IEnumerable<AssignmentVariable> assignmentVariables)
        {
            return new AssignmentResponse
            {
                Id = assignment.Id,
                Name = assignment.Name,
                DueDate = assignment.DueDate,
                CourseId = assignment.CourseId,
                Variables = assignmentVariables.MapToResponse(),
            };
        }

        public static AssignmentVariable MapToAssignmentVariable(this AssignmentVariableContract request)
        {
            return new AssignmentVariable
            {
                Id = Guid.NewGuid(),
                Type = request.Type,
                Name = request.Name
            };
        }

        public static IEnumerable<AssignmentVariable> MapToAssignmentVariable(this IEnumerable<AssignmentVariableContract> variables)
        {
            return variables.Select(variable => variable.MapToAssignmentVariable());
        }

        public static AssignmentVariableContract MapToResponse(this AssignmentVariable variable)
        {
            return new AssignmentVariableContract
            {
                Name = variable.Name,
                Type = variable.Type,
            };
        }

        public static IEnumerable<AssignmentVariableContract> MapToResponse(this IEnumerable<AssignmentVariable> variables)
        {
            return variables.Select(variable => variable.MapToResponse());
        }
    }
}

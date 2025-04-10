using Database.Models;

namespace Container.Models;

#pragma warning disable IDE1006 // Naming Styles

public class AssignmentEntryDTO
{
    public required StudentDTO? student { get; init; }
    public required TeamDTO? team { get; init; }
    public required DeliveryDTO? delivery { get; init; }

    public static AssignmentEntryDTO Create(User? student, Team? team, Delivery? delivery)
    {
        return new AssignmentEntryDTO
        {
            student = student is not null ? StudentDTO.MapFrom(student) : null,
            team = team is not null ? TeamDTO.MapFrom(team) : null,
            delivery = delivery is not null ? DeliveryDTO.MapFrom(delivery) : null
        };
    }
}

public class DeliveryDTO
{
    public required Dictionary<string, object> fields { get; init; }

    public static DeliveryDTO MapFrom(Delivery delivery)
    {
        return new DeliveryDTO
        {
            fields = delivery.Fields!.ToDictionary
            (
                d => d.AssignmentField!.Name,
                d => d.Value
            )
        };
    }
}

public class StudentDTO
{
    public required Guid student_id { get; init; }
    public required string name { get; init; }
    public required string email { get; init; }

    public static StudentDTO MapFrom(User student)
    {
        return new StudentDTO
        {
            student_id = student.Id,
            name = student.Name,
            email = student.Email
        };
    }
}

public class TeamDTO
{
    public required Guid team_id { get; init; }
    public required int team_nr { get; init; }
    public required List<StudentDTO> students { get; init; }

    public static TeamDTO MapFrom(Team team)
    {
        return new TeamDTO
        {
            team_id = team.Id,
            team_nr = team.TeamNr,
            students = team.Students.Select(StudentDTO.MapFrom).ToList()
        };
    }
}

#pragma warning restore IDE1006 // Naming Styles
using Database.Models;

namespace Container.Models;

public class AssignmentEntry
{
    public required User? Student { get; init; }
    public required Team? Team { get; init; }
    public required Delivery? Delivery { get; init; }

    public Guid Id => Student is not null ? Student.Id : Team!.Id;
}

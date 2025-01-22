using System.ComponentModel.DataAnnotations;

namespace Database.Models;

public abstract class Delivery
{
    [Key]
    public required Guid Id { get; set; }
    public required Guid AssignmentId { get; set; }
    public Assignment? Assignment { get; set; }
    public List<DeliveryField>? Fields { get; set; }
}

public class StudentDelivery : Delivery
{
    public required Guid StudentId { get; set; }
    public User? Student { get; set; }
}

public class TeamDelivery : Delivery
{
    public required Guid TeamId { get; set; }
    public Team? Team { get; set; }
}
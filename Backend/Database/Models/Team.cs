using System.ComponentModel.DataAnnotations;

namespace Database.Models;

public class Team : IModel
{
    [Key]
    public required Guid Id { get; set; }
    public required int TeamNr { get; set; }
    public required Guid CourseId { get; set; }
    public Course? Course { get; set; }
    public required List<User> Students { get; set; }
}

using System.ComponentModel.DataAnnotations;

namespace Database.Models;

public class Course
{
    [Key]
    public required Guid Id { get; set; }
    public required string Title { get; set; }
    public required string Code { get; set; }
    public List<User>? Teachers { get; set; }
    public List<User>? Students { get; set; }
    public List<Team>? Teams { get; set; }
}

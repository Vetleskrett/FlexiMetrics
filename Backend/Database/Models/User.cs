using System.ComponentModel.DataAnnotations;

namespace Database.Models;

public class User : IModel
{
    [Key]
    public required Guid Id { get; set; }
    public required string Email { get; set; }
    public required string Name { get; set; }
    public required Role Role { get; set; }
}
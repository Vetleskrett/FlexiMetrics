using System.ComponentModel.DataAnnotations;

namespace Database.Models
{
    public abstract class IEntity
    {
        [Key]
        public required Guid Id { get; set; }
    }
}

using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Database.Models
{
    public class Team : IEntity
    {
        // Maybe this should be string to make it more flexible?
        public required int TeamId { get; set; }
        public required Guid CourseId { get; set; }
    }
}

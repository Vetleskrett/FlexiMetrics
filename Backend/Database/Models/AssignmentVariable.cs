using System.ComponentModel.DataAnnotations;

namespace Database.Models
{

    // TODO: FIND ALL VARIABLE TYPES, THIS IS A DRAFT
    public enum VariableType
    {
       STRING,
       BOOLEAN,
       INTEGER,
       DOUBLE,
       FILE
    }
    public class AssignmentVariable
    {
        [Key]
        public required Guid Id { get; set; }
        public required VariableType Type { get; set; }
        public required string Name { get; set; }
        public Guid AssignmentId { get; set; }
    }
}

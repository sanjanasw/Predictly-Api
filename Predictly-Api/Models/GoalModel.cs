using Predictly_Api.Enums;
using System.ComponentModel.DataAnnotations;

namespace Predictly_Api.Models
{
    public class GoalModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }

        [Required]
        public int SubjectId { get; set; }

        [Required]
        public Results Goal { get; set; }

    }
}

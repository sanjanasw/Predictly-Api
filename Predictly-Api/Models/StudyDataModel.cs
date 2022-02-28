using Predictly_Api.Enums;
using System.ComponentModel.DataAnnotations;

namespace Predictly_Api.Models
{
    public class StudyDataModel
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string UserId { get; set; }
        [Required]
        public SubHours Commitment { get; set; }
        [Required]
        public bool ClassStatus { get; set; }
        [Required]
        public double AvgMarks { get; set; }
    }
}

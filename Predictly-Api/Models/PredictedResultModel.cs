using Predictly_Api.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace Predictly_Api.Models
{
    public class PredictedResultModel
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string UserId { get; set; }
        [Required]
        public int SubjectId { get; set; }
        [Required]
        public double A  { get; set; }
        [Required]
        public double B { get; set; }
        [Required]
        public double C { get; set; }
        [Required]
        public double S { get; set; }
        [Required]
        public double W { get; set; }
        public DateTime UpdatedOn { get; set; } = DateTime.Now;
    }
}

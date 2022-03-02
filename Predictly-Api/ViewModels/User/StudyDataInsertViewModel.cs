using System.ComponentModel.DataAnnotations;
using Predictly_Api.Enums;

namespace Predictly_Api.ViewModels.User
{
    public class StudyDataInsertViewModel
    {
        [Required(ErrorMessage = "SubjectId is required")]
        public int SubjectId { get; set; }

        [Required(ErrorMessage = "Commitment is required")]
        public SubHours Commitment { get; set; }

        [Required(ErrorMessage = "Class status is required")]
        public bool ClassStatus { get; set; }

        [Required(ErrorMessage = "Average marks is required")]
        public double AvgMarks { get; set; }
    }
}

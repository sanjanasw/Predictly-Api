using Predictly_Api.Enums;
using System.ComponentModel.DataAnnotations;

namespace Predictly_Api.ViewModels.User
{
    public class StudyDataUpdateViewModel
    {
        public int Id { get; set; }
        public int SubjectId { get; set; }
        public SubHours Commitment { get; set; }
        public bool ClassStatus { get; set; }
        public double AvgMarks { get; set; }
    }
}

using Predictly_Api.Enums;
using System.ComponentModel.DataAnnotations;

namespace Predictly_Api.Models
{
    public class StudyDataModel
    {
        [Key]
        public string UserId { get; set; }

        public SubHours Sub1Hours { get; set; }

        public bool Sub1Class { get; set; }

        public float Sub1AvgMarks { get; set; }

        public SubHours Sub2Hours { get; set; }

        public bool Sub2Class { get; set; }

        public float Sub2AvgMarks { get; set; }

        public SubHours Sub3Hours { get; set; }

        public bool Sub3Class { get; set; }

        public float Sub3AvgMarks { get; set; }

        public SubHours Sub4Hours { get; set; }

        public bool Sub4Class { get; set; }

        public float Sub4AvgMarks { get; set; }

        public SubHours Sub5Hours { get; set; }

        public bool Sub5Class { get; set; }

        public float Sub5AvgMarks { get; set; }

        public SubHours Sub6Hours { get; set; }

        public bool Sub6Class { get; set; }

        public float Sub6AvgMarks { get; set; }

        public SubHours Sub7Hours { get; set; }

        public bool Sub7Class { get; set; }

        public float Sub7AvgMarks { get; set; }

        public SubHours Sub8Hours { get; set; }

        public bool Sub8Class { get; set; }

        public float Sub8AvgMarks { get; set; }

        public SubHours Sub9Hours { get; set; }

        public bool Sub9Class { get; set; }

        public float Sub9AvgMarks { get; set; }
    }
}

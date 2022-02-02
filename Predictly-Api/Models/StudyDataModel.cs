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

        public double Sub1AvgMarks { get; set; }

        public SubHours Sub2Hours { get; set; }

        public bool Sub2Class { get; set; }

        public double Sub2AvgMarks { get; set; }

        public SubHours Sub3Hours { get; set; }

        public bool Sub3Class { get; set; }

        public double Sub3AvgMarks { get; set; }

        public SubHours Sub4Hours { get; set; }

        public bool Sub4Class { get; set; }

        public double Sub4AvgMarks { get; set; }

        public SubHours Sub5Hours { get; set; }

        public bool Sub5Class { get; set; }

        public double Sub5AvgMarks { get; set; }

        public SubHours Sub6Hours { get; set; }

        public bool Sub6Class { get; set; }

        public double Sub6AvgMarks { get; set; }

        public SubHours Sub7Hours { get; set; }

        public bool Sub7Class { get; set; }

        public double Sub7AvgMarks { get; set; }

        public SubHours Sub8Hours { get; set; }

        public bool Sub8Class { get; set; }

        public double Sub8AvgMarks { get; set; }

        public SubHours Sub9Hours { get; set; }

        public bool Sub9Class { get; set; }

        public double Sub9AvgMarks { get; set; }
    }
}

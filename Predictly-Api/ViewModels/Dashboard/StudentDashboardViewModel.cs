using Predictly_Api.Enums;
using System.Collections.Generic;

namespace Predictly_Api.ViewModels.Dashboard
{
    public class StudentDashboardViewModel
    {
        public List<PredictedResultViewModel> PredictedResult { get; set; }
    }

    public class PredictedResultViewModel
    {
        public string Subject { get; set; }
        public string Goal { get; set; }
        public double A { get; set; }
        public double B { get; set; }
        public double C { get; set; }
        public double S { get; set; }
        public double W { get; set; }
    }
}

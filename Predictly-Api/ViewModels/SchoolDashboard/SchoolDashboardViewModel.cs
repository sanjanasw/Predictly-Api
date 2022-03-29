using System.Collections.Generic;

namespace Predictly_Api.ViewModels.SchoolDashboard
{
    public class SchoolDashboardViewModel
    {
        public List<SchoolDashboardResultsPredictionDataViewModel> ResultPrediction { get; set; }
        public SchoolGenderDistributionViewModel GenderDistribution { get; set; }
        public List<ClassStatusViewModel> ClassStatus { get; set; }
    }
}

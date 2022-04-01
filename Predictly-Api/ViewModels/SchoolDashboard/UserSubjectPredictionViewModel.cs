using Predictly_Api.Enums;

namespace Predictly_Api.ViewModels.SchoolDashboard
{
    public class UserSubjectPredictionViewModel
    {
        public string UserId { get; set; }
        public int SubjectId { get; set; }
        public Results PredictedResult { get; set; }
    }
}

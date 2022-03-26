using Predictly_Api.Enums;

namespace Predictly_Api.ViewModels.SchoolDashboard
{
    public class UserSubjectPrediction
    {
        public string UserId { get; set; }
        public int SubjectId { get; set; }
        public Results PredictedResult { get; set; }
    }
}

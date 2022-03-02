using Predictly_Api.Enums;
using Predictly_Api.ViewModels.Dashboard;

namespace Predictly_Api.ViewModels.User
{
    public class CurrentStatusViewModel
    {
        public int Id { get; set; }
        public string Subject { get; set; }
        public int SubjectId { get; set; }
        public int BucketType { get; set; }
        public SubHours Commitment { get; set; }
        public bool ClassStatus { get; set; }
        public double AvgMarks { get; set; }

    }
}

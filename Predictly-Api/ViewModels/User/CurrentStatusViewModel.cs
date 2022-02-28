using Predictly_Api.Enums;
using Predictly_Api.ViewModels.Dashboard;

namespace Predictly_Api.ViewModels.User
{
    public class CurrentStatusViewModel
    {
        public string Subject { get; set; }
        public SubHours Commitment { get; set; } 
        public ResultViewModel PredictedResult { get; set; }

    }
}

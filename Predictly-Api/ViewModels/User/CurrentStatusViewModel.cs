using Predictly_Api.Enums;

namespace Predictly_Api.ViewModels.User
{
    public class CurrentStatusViewModel
    {
        public string Subject { get; set; }
        public SubHours CurrentHours { get; set; } 
        public Results PredictedResult { get; set; }

    }
}

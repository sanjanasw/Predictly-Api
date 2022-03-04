using Predictly_Api.Enums;

namespace Predictly_Api.ViewModels.Goal
{
    public class GoalViewModel
    {
        public int? Id { get; set; }

        public int SubjectId { get; set; }

        public string Subject { get; set; }

        public Results? Goal { get; set; }
    }
}

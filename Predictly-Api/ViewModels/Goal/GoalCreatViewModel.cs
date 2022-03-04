using System.ComponentModel.DataAnnotations;
using Predictly_Api.Enums;

namespace Predictly_Api.ViewModels.Goal
{
    public class GoalCreateViewModel
    {

        [Required(ErrorMessage = "Subject id is required")]
        public int SubjectId { get; set; }

        [Required(ErrorMessage = "Goal is required")]
        public Results Goal { get; set; }
    }
}

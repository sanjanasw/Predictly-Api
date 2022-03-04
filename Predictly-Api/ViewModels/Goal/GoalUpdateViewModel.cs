using System.ComponentModel.DataAnnotations;
using Predictly_Api.Enums;

namespace Predictly_Api.ViewModels.Goal
{
    public class GoalUpdateViewModel
    {
        [Required(ErrorMessage = "Goal id is required")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Goal is required")]
        public Results Goal { get; set; }
    }
}

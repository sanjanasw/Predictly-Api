using System.ComponentModel.DataAnnotations;

namespace VMS_API.ViewModels.Authentication
{
    public class ChangePasswordViewModel
    {
        [Required(ErrorMessage = "Current Password is Required")]
        public string CurrentPassword { get; set; }

        [Required(ErrorMessage = "New Password is Required")]
        public string NewPassword { get; set; }
    }
}

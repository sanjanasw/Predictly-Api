using System.ComponentModel.DataAnnotations;

namespace Predictly_Api.ViewModels.Authentication
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Username is required")]
        public string UserName { get; set; }

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }
    }
}

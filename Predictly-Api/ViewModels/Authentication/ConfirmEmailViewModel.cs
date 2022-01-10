using System.ComponentModel.DataAnnotations;

namespace Predictly_Api.ViewModels.Authentication
{
    public class ConfirmEmailViewModel
    {
        [Required(ErrorMessage = "User Id is Required")]
        public string Userid { get; set; }

        [Required(ErrorMessage = "Token is Required")]
        public string Token { get; set; }
    }
}

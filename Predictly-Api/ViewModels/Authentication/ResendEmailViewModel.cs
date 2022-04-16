using System.ComponentModel.DataAnnotations;

namespace Predictly_Api.ViewModels.Authentication
{ 
    public class ResendEmailViewModel
    {
        [Required(ErrorMessage = "Email is Required")]
        public string Email { get; set; }
    }
}

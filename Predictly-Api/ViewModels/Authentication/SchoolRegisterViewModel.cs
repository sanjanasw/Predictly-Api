using Predictly_Api.Enums;
using System.ComponentModel.DataAnnotations;

namespace Predictly_Api.ViewModels.Authentication
{
    public class SchoolRegisterViewModel
    {
        [Required(ErrorMessage = "User info is required")]
        public SchoolUserInfoViewModel UserInfo { get; set; }

        [Required(ErrorMessage = "School info is required")]
        public SchoolInfoViewModel SchoolInfo { get; set; }
    }

    public class SchoolUserInfoViewModel
    {
        [Required(ErrorMessage = "First Name is required")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last Name is required")]
        public string LastName { get; set; }

        [EmailAddress]
        [Required(ErrorMessage =  "Email is required")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Gender is required")]
        public Genders Gender { get; set; }

        [Required(ErrorMessage = "UserName is required")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }
    }

    public class SchoolInfoViewModel
    {
        [Required(ErrorMessage = "School name is required")]
        public string Name { get; set; }

        [Required(ErrorMessage = "School address is required")]
        public string Address { get; set; }
    }
}

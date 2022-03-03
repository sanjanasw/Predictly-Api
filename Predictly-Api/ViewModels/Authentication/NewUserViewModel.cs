using System.ComponentModel.DataAnnotations;
using Predictly_Api.Enums;

namespace Predictly_Api.ViewModels.Authentication
{
    public class NewUserViewModel
    {
        [Required(ErrorMessage = "First Name is required")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last Name is required")]
        public string LastName { get; set; }

        [EmailAddress]
        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Gender is required")]
        public Genders Gender { get; set; }

        [Required(ErrorMessage = "UserName is required")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Role is required")]
        public UserRoles Role { get; set; }

        [Required(ErrorMessage = "School Id is required")]
        public int SchoolId { get; set; }


    }
}

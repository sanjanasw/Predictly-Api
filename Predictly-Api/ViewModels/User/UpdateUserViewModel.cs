using System;
using System.ComponentModel.DataAnnotations;
using Predictly_Api.Enums;

namespace Predictly_Api.ViewModels.User
{
    public class UpdateUserViewModel
    {
        [Required(ErrorMessage = "NIC is required")]
        public string Id { get; set; }

        [Required(ErrorMessage = "NIC is required")]
        public string NIC { get; set; }

        [Required(ErrorMessage = "UserName is required")]
        public string Username { get; set; }

        [Required(ErrorMessage = "First Name is required")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last Name is required")]
        public string LastName { get; set; }

        [EmailAddress]
        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Phone Number is required")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Gender is required")]
        public Genders Gender { get; set; }

        [Required(ErrorMessage = "Birthday is required")]
        public DateTime DOB { get; set; }

        public string Role { get; set; }
    }
}

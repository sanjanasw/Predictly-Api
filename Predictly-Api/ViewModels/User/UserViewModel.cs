using System;
using System.ComponentModel.DataAnnotations;
using Predictly_Api.Enums;
using Predictly_Api.Models;

namespace Predictly_Api.ViewModels.User
{
    public class UserViewModel
    {
        [Required(ErrorMessage = "Id is required")]
        public string Id { get; set; }

        [Required(ErrorMessage = "UserName is required")]
        public string Username { get; set; }

        [Required(ErrorMessage = "First Name is required")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last Name is required")]
        public string LastName { get; set; }

        [EmailAddress]
        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; }
        public Genders Gender { get; set; }
        public int SchoolId { get; set; }
        public int OLYear { get; set; } = 0;
        public string Role { get; set; }

        public StudyDataModel StudyData { get; set; } = null;
    }
}

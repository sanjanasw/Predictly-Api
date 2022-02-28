using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Predictly_Api.Enums;
using Predictly_Api.Models;

namespace Predictly_Api.ViewModels.Authentication
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "User Info is required")]
        public UserInfo UserInfo { get; set; }

        [Required(ErrorMessage = "Study Data is required")]
        public List<StudyDataModel> StudyData { get; set; }

    }

    public class UserInfo
    {
        [Required(ErrorMessage = "First Name is required")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last Name is required")]
        public string LastName { get; set; }

        [EmailAddress]
        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; }

        [Required(ErrorMessage = "OL year required")]
        public int OLYear { get; set; }

        [Required(ErrorMessage = "Gender is required")]
        public Genders Gender { get; set; }

        [Required(ErrorMessage = "School is required")]
        public int SchoolId { get; set; }

        public int BSub1 { get; set; }

        public int BSub2 { get; set; }

        public int BSub3 { get; set; }

        [Required(ErrorMessage = "Fathers Education Level is required")]
        public EducationLevels FathersEduLevel { get; set; }

        [Required(ErrorMessage = "Mothers Education Level is required")]
        public EducationLevels MothersEduLevel { get; set; }

        [Required(ErrorMessage = "UserName is required")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }
    }
}

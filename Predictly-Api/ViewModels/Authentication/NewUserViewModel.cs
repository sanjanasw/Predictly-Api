using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Predictly_Api.Enums;
using Predictly_Api.Models;

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


    }
}

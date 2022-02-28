using System;
using System.ComponentModel.DataAnnotations;
using Predictly_Api.Enums;
using Predictly_Api.Models;

namespace Predictly_Api.ViewModels.User
{
    public class UserViewModel
    {
        public string Id { get; set; }
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public Genders Gender { get; set; }
        public int SchoolId { get; set; }
        public int OLYear { get; set; }
        public string Role { get; set; }
    }
}

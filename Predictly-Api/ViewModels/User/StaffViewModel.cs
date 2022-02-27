using Predictly_Api.Enums;

namespace Predictly_Api.ViewModels.User
{
    public class StaffViewModel
    {
        public string Id { get; set; }
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public Genders Gender { get; set; }
        public bool isActive { get; set; }
        public int SchoolId { get; set; }
        public string Role { get; set; }
    }
}

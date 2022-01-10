using System.ComponentModel.DataAnnotations;

namespace Predictly_Api.Models
{
    public class SchoolModel
    {
        [Key]
        public int Id { get; set; }

        public string StaffUserId { get; set; }

        public string Name { get; set; }

        public string Address { get; set; }

    }
}

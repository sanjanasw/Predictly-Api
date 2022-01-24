using System.ComponentModel.DataAnnotations;

namespace Predictly_Api.Models
{
    public class SubjectModel
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        public int BucketType { get; set; } = 0;
    }
}

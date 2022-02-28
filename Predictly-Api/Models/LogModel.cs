using System;
using System.ComponentModel.DataAnnotations;

namespace Predictly_Api.Models
{
    public class LogModel
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Message { get; set; }
        [Required]
        public string MessageTemplate { get; set; }
        [Required]
        public string Level { get; set; }
        [Required]
        public DateTime TimeStamp { get; set; }
        public string Exception { get; set; }
        [Required]
        public string Properties { get; set; }
    }
}

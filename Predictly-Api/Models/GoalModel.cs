using Predictly_Api.Enums;
using System.ComponentModel.DataAnnotations;

namespace Predictly_Api.Models
{
    public class GoalModel
    {
        [Key]
        public string UserId { get; set; }
        public Results? Sub1Goal { get; set; } = null;
        public Results? Sub2Goal { get; set; } = null;
        public Results? Sub3Goal { get; set; } = null;
        public Results? Sub4Goal { get; set; } = null;
        public Results? Sub5Goal { get; set; } = null;
        public Results? Sub6Goal { get; set; } = null;
        public Results? Sub7Goal { get; set; } = null;
        public Results? Sub8Goal { get; set; } = null;
        public Results? Sub9Goal { get; set; } = null;

    }
}

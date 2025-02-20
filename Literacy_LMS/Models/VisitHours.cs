using System;
using System.ComponentModel.DataAnnotations;

namespace Literacy_LMS.Models
{
    public class VisitHours
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public TimeSpan OpeningTime { get; set; }

        [Required]
        public TimeSpan ClosingTime { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace Literacy_LMS.Models
{
    public class Student
    {
        [Key]
        public int Id { get; set; } // Primary Key

        [Required]
        [StringLength(50)]
        public string IDNumber { get; set; } // Student ID Number

        [Required]
        [StringLength(100)]
        public string FullName { get; set; } // Student Full Name

        [Required]
        [ForeignKey("IdentityUser")]
        public string UserId { get; set; } // Foreign Key to AspNetUsers table

        public IdentityUser User { get; set; } // Navigation Property
    }
}

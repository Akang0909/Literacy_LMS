using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Literacy_LMS.Models
{
    public class Payment
    {
        [Key]
        public int PaymentId { get; set; } // Primary Key

        [Required]
        [ForeignKey("Students")]
        public string IDNumber { get; set; } // Foreign Key to Student Table



        [Required]
        [ForeignKey("Book")]
        public int BookID { get; set; } // Foreign Key to Book Table (nullable if general payment)


        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal AmountPaid { get; set; } // Payment Amount

        [Required]
        [StringLength(50)]
        public string PaymentMethod { get; set; } // Payment Method (Cash, GCash, etc.)

        [Required]
        public DateTime PaymentDate { get; set; } = DateTime.Now; // Payment Date

        [StringLength(255)]
        public string Remarks { get; set; } // Additional info (e.g., "Lost Book Fee", "Overdue Fine")
    }
}

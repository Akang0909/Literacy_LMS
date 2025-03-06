using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class IssueRequest
{
    [Key]
    public int RequestID { get; set; }

    [ForeignKey("Book")]
    public int BookID { get; set; }

    [ForeignKey("Students")]
    public string IDNumber { get; set; }  // Student ID

    public string Textbook { get; set; }  // Book Name
    public int NumberOfCopies { get; set; }
    public string Status { get; set; } = "Pending";
    public DateTime RequestDate { get; set; } = DateTime.Now;

    public DateTime DueDate { get; set; }
    public DateTime? ReturnDate { get; set; }  // Nullable for books not yet returned

    [Column(TypeName = "decimal(10,2)")]
    public decimal? OverdueAmount { get; set; }  // Nullable, calculated when book is returned

    public string PaymentStatus { get; set; } = ""; // New field

    public int RenewCount { get; set; } = 1;

    public string RenewStatus { get; set; } = "";

}

using System.ComponentModel.DataAnnotations;

public class BorrowedBook
{
    [Key]
    public int Id { get; set; }  // Primary Key

    [Required]
    public string UserId { get; set; }  // Foreign Key linking to Student

    [Required]
    public int BookId { get; set; }  // Foreign Key linking to Books table

    [Required]
    public string BookName { get; set; }  // Book name

    [Required]
    public DateTime IssueDate { get; set; }  // When it was borrowed

    [Required]
    public DateTime DueDate { get; set; }  // When it’s due

    public DateTime? ReturnDate { get; set; }  // When it was returned (nullable)
}

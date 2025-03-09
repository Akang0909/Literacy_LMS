namespace Literacy_LMS.Models
{
    public class StudentRequestViewModel
    {
        public string IDNumber { get; set; }
        public string StudentName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public int? BookID { get; set; }
        public string Textbook { get; set; }
        public string Status { get; set; }
        public DateTime? RequestDate { get; set; }
        public DateTime? DueDate { get; set; }
        public decimal? OverdueAmount { get; set; }
        public string PaymentStatus { get; set; }
    }
}

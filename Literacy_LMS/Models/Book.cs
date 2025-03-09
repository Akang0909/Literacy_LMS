namespace Literacy_LMS.Models
{
    public class Book
    {
        public int BookID { get; set; }
        public string BookSection { get; set; }
        public string Subject { get; set; }
        public string Textbook { get; set; }
        public int CopyrightYear { get; set; }
        public int? Volume { get; set; } // Nullable
        public int NumberOfCopies { get; set; }
        public string Author { get; set; }
        public string ISBN { get; set; }
        public string BookStatus { get; set; }

        public bool IsArchived { get; set; } = false;

    }
}

using System.ComponentModel.DataAnnotations.Schema;

namespace Literacy_LMS.Models
{
    [NotMapped] // Prevents EF Core from mapping this class to a database table
    public class EmailViewModel
    {
        public string ToEmail { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
    }
}

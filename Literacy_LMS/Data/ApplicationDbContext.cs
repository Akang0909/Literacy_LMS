using Literacy_LMS.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Literacy_LMS.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Book> Books { get; set; }
        public DbSet<VisitHours> VisitHours { get; set; }

        public DbSet<Student> Students { get; set; }

        public DbSet<IssueRequest> IssueRequests { get; set; }

        public DbSet<BorrowedBook> BorrowedBooks { get; set; }


        public DbSet<Payment> Payments { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}

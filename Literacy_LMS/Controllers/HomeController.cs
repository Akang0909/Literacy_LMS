using System.Diagnostics;
using Literacy_LMS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Literacy_LMS.Data;
using Microsoft.IdentityModel.Tokens;
using iTextSharp.text.pdf;
using iTextSharp.text;
using Literacy_LMS.Helpers;


namespace Literacy_LMS.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }



        [Authorize(Roles = "Admin")]
        public class AdminController : Controller
        {
            public IActionResult Index()
            {
                return View();
            }
        }

        [Authorize(Roles = "Librarian")]
    public IActionResult LibrarianDashboard()
    {
        return View("Librarian/Dashboard"); // Ensure this view exists in Views/Home/Librarian/
    }

    [Authorize(Roles = "Student")]
    public IActionResult StudentDashboard()
    {
        return View("Students/Dashboard"); // Ensure this view exists in Views/Home/Students/
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Dashboard()
        {
            return View();
        }


        //testing emails api
        public async Task<ActionResult> TestEmail()
        {
            await EmailService.SendEmailAsync("mr.ven09@gmail.com", "📢 Test Email", "This is a test email from your Library System.");
            return Content("Test email sent!");
        }



        ///new code working
        public IActionResult Previous()
        {
            // Get the issue requests along with the related Book data
            var previousBooks = _context.IssueRequests
                .Where(ir => ir.Status == "Return")  // Fetch books that are returned
                .Join(_context.Books, ir => ir.BookID, b => b.BookID, (ir, b) => new IssueRequest
                {
                    RequestID = ir.RequestID,
                    BookID = ir.BookID,
                    IDNumber = ir.IDNumber,
                    Status = ir.Status,
                    RequestDate = ir.RequestDate,
                    DueDate = ir.DueDate,
                    Textbook = b.Textbook,  // Adding Textbook from the Book table
                    NumberOfCopies = b.NumberOfCopies, // Adding NumberOfCopies from the Book table
                    ReturnDate = ir.ReturnDate
                })
                .ToList();

            // Pass the data directly to the view (no need for ViewModel now)
            return View(previousBooks);
        }

        ///end of new code working




        public IActionResult Messages()
        {
            return View();
        }


        //PAYMENT
        public IActionResult Payment()
        {
            return View();
        }


        public IActionResult ProcessPayment([FromBody] Payment model)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                return Json(new { success = false, message = "Invalid input.", errors });
            }

            var student = _context.Students.FirstOrDefault(s => s.IDNumber == model.IDNumber);
            if (student == null)
            {
                return Json(new { success = false, message = "Student not found." });
            }

            var issueRequest = _context.IssueRequests
                .FirstOrDefault(r => r.IDNumber == model.IDNumber && r.BookID == model.BookID);

            if (issueRequest == null)
            {
                return Json(new { success = false, message = "Issue request not found for this student and book." });
            }

            Console.WriteLine($"Before Update: ID: {issueRequest.RequestID}, PaymentStatus: {issueRequest.PaymentStatus}, OverdueAmount: {issueRequest.OverdueAmount}");

            if (issueRequest.OverdueAmount > 0)
            {
                decimal remainingAmount = issueRequest.OverdueAmount ?? 0;

                if (model.AmountPaid >= remainingAmount)
                {
                    issueRequest.PaymentStatus = "Paid";
                    issueRequest.OverdueAmount = 0;  // Overdue is fully paid
                }
                else
                {
                    issueRequest.OverdueAmount = remainingAmount - model.AmountPaid; // Reduce the remaining balance
                    issueRequest.PaymentStatus = "Partial"; // Mark as partially paid
                }


                _context.Entry(issueRequest).State = EntityState.Modified;
            }
            else
            {
                return Json(new { success = false, message = "No overdue amount to pay." });
            }

            // Save the payment record
            var payment = new Payment
            {
                IDNumber = model.IDNumber,
                BookID = model.BookID,
                AmountPaid = model.AmountPaid,
                PaymentMethod = model.PaymentMethod,
                PaymentDate = DateTime.Now,
                Remarks = model.Remarks ?? (issueRequest.OverdueAmount == 0 ? "Paid in full" : "Partial payment")
            };

            try
            {
                _context.Payments.Add(payment);
                _context.SaveChanges();

                Console.WriteLine($"Payment saved successfully. Updated OverdueAmount: {issueRequest.OverdueAmount}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving payment: {ex.Message}");
                return Json(new { success = false, message = "An error occurred while saving the payment: " + ex.Message });
            }

            return Json(new { success = true, message = "Payment processed successfully!" });
        }







        //END OF PAYMENT

        ///////////////Transactions/////////////////
        public async Task<IActionResult> Transactions(string search, string statusFilter)
        {
            var payments = _context.Payments.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                payments = payments.Where(p => p.IDNumber.Contains(search) || p.PaymentMethod.Contains(search));
            }

            if (!string.IsNullOrEmpty(statusFilter))
            {
                payments = payments.Where(p => p.Remarks == statusFilter);
            }

            return View(await payments.ToListAsync());
        }

        [HttpGet]
        public IActionResult GetReceipt(int id)
        {
            var payment = _context.Payments.FirstOrDefault(m => m.PaymentId == id);

            if (payment == null)
            {
                return NotFound();
            }

            return PartialView("_ReceiptPartial", payment);
        }

        //pdf
        public ActionResult ExportToPDF()
        {
            var payments = _context.Payments.ToList(); // Fetch all data

            // Create a memory stream for PDF
            MemoryStream stream = new MemoryStream();
            Document document = new Document(PageSize.A4, 10, 10, 10, 10);
            PdfWriter writer = PdfWriter.GetInstance(document, stream);

            document.Open();

            // Add Title
            Font titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 18);
            Paragraph title = new Paragraph("Payment Transactions", titleFont);
            title.Alignment = Element.ALIGN_CENTER;
            document.Add(title);
            document.Add(new Paragraph("\n")); // Line break

            // Create Table
            PdfPTable table = new PdfPTable(7); // 7 columns for your table
            table.WidthPercentage = 100;
            table.SetWidths(new float[] { 15, 20, 15, 15, 15, 15, 20 });

            // Add Table Headers
            string[] headers = { "Payment ID", "Student ID", "Book ID", "Amount Paid", "Payment Method", "Date", "Remarks" };
            foreach (var header in headers)
            {
                PdfPCell cell = new PdfPCell(new Phrase(header, FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12)))
                {
                    BackgroundColor = new BaseColor(200, 200, 200),
                    HorizontalAlignment = Element.ALIGN_CENTER
                };
                table.AddCell(cell);
            }

            // Add Data Rows
            foreach (var payment in payments)
            {
                table.AddCell(payment.PaymentId.ToString());
                table.AddCell(payment.IDNumber);
                table.AddCell(payment.BookID == 0 ? "N/A" : payment.BookID.ToString());
                table.AddCell(payment.AmountPaid.ToString("C"));
                table.AddCell(payment.PaymentMethod);
                table.AddCell(payment.PaymentDate.ToShortDateString());
                table.AddCell(payment.Remarks);
            }

            document.Add(table);
            document.Close();

            // Return the PDF as a file download
            return File(stream.ToArray(), "application/pdf", "Payment_Transactions.pdf");
        }


        //////////////Transactions///////////////////

        public IActionResult Details()
        {
            return View();
        }

        public IActionResult Edit()
        {
            return View();
        }


        public IActionResult ManageStudents()
        {
            return View();
        }

        public IActionResult RecievedMessages()
        {
            return View();
        }
        public IActionResult IssueRequest()
        {
            decimal finePerDay = 5; // Set your fine per day here

            var issueRequests = _context.IssueRequests
                .Join(_context.Books, ir => ir.BookID, b => b.BookID, (ir, b) => new IssueRequest
                {
                    RequestID = ir.RequestID,
                    BookID = ir.BookID,
                    IDNumber = ir.IDNumber,
                    Status = ir.Status,
                    RequestDate = ir.RequestDate,
                    DueDate = ir.DueDate,
                    ReturnDate = ir.ReturnDate,
                    Textbook = b.Textbook,
                    NumberOfCopies = b.NumberOfCopies,
                    OverdueAmount = (ir.ReturnDate.HasValue && ir.ReturnDate > ir.DueDate)
                        ? (ir.ReturnDate.Value - ir.DueDate).Days * finePerDay
                        : 0 // No fine if returned on time
                })
                .Take(1000)
                .ToList();

            return View(issueRequests);
        }






        [HttpPost]
        public IActionResult AcceptRequest(int requestId)
        {
            // Fetch the request from the database
            var request = _context.IssueRequests.FirstOrDefault(r => r.RequestID == requestId);

            if (request == null)
            {
                return NotFound(); // Handle error, request not found
            }

            // Fetch the corresponding book for this request
            var book = _context.Books.FirstOrDefault(b => b.BookID == request.BookID);

            if (book == null)
            {
                return NotFound(); // Handle error, book not found
            }

            // Check if there are available copies to deduct
            if (book.NumberOfCopies <= 0)
            {
                return Json(new { success = false, message = "Not enough copies available" });
            }

            // Deduct 1 copy from the book's number of copies
            book.NumberOfCopies -= 1;

            // Calculate the DueDate (3 days from the RequestDate)
            request.DueDate = request.RequestDate.AddDays(3);

            // Update the request status to "Accepted"
            request.Status = "Accepted";

            // Save the changes to the database
            _context.SaveChanges();

            // Return the updated list of accepted requests
            var acceptedRequests = _context.IssueRequests
                .Where(r => r.Status == "Accepted")
                .ToList();

            return Json(new { success = true, message = "Request Accepted and book copy deducted", data = acceptedRequests });
        }


        [HttpPost]
        public IActionResult RejectRequest(int requestId)
        {
            // Fetch the request from the database
            var request = _context.IssueRequests.FirstOrDefault(r => r.RequestID == requestId);
            if (request == null)
            {
                // Handle error, request not found
                return NotFound();
            }

            // Update the status to Rejected
            request.Status = "Rejected";

            // Save the changes to the database
            _context.SaveChanges();

            // Return the updated list of requests excluding Rejected ones
            var remainingRequests = _context.IssueRequests
                .Where(r => r.Status != "Accepted" && r.Status != "Rejected")
                .ToList();

            return Json(new { success = true, message = "Request Rejected", data = remainingRequests });
        }

        //Renewal of book//

        public IActionResult RenewRequest()
        {
            var renewRequests = _context.IssueRequests
                .Where(r => r.Status == "Accepted" && r.RenewStatus == "Pending")
                .ToList();

            return View(renewRequests);
        }


        [HttpPost]
        public IActionResult RenewRequest(int requestId)
        {
            var request = _context.IssueRequests.FirstOrDefault(r => r.RequestID == requestId);

            if (request == null)
            {
                return Json(new { success = false, message = "Request not found." });
            }

            if (request.RenewCount <= 0)
            {
                return Json(new { success = false, message = "Renewal limit reached." });
            }

            // request.RenewCount--;
            request.RenewStatus = "Pending";
            _context.SaveChanges();

            return Json(new { success = true, message = "Renewal successful. New Due Date: " + DateTime.Now.AddDays(3).ToString("yyyy-MM-dd") });
        }

        [HttpPost]
        public IActionResult RejectRenewRequest(int requestId)
        {
            var request = _context.IssueRequests.FirstOrDefault(r => r.RequestID == requestId);

            if (request == null)
            {
                return Json(new { success = false, message = "Request not found." });
            }

            // Mark request as "Rejected" or any other logic needed
            request.RenewStatus = "Rejected";
            _context.SaveChanges();

            return Json(new { success = true, message = "Renewal request rejected successfully." });
        }


        //new

        [HttpPost]
        public IActionResult AcceptRenewRequest(int requestId)
        {
            var request = _context.IssueRequests.FirstOrDefault(r => r.RequestID == requestId);

            if (request == null)
            {
                return Json(new { success = false, message = "Request not found." });
            }

            if (request.RenewCount <= 0)
            {
                return Json(new { success = false, message = "Renewal limit reached." });
            }

            request.RenewCount--;
            request.RenewStatus = "Accepted";
            request.DueDate = DateTime.Now.AddDays(3); // Extend due date
            _context.SaveChanges();

            return Json(new { success = true, message = "Renewal request accepted. New Due Date: " + request.DueDate.ToString("yyyy-MM-dd") });
        }



        //end of the renewal of book





        ////////////////////////////////////////////////////////////////////////////////////////////////////////////

        // Return book

        [HttpPost]
        public IActionResult ReturnBook(int requestId)
        {
            var request = _context.IssueRequests.FirstOrDefault(r => r.RequestID == requestId);

            if (request == null)
            {
                return Json(new { success = false, message = "Request not found." });
            }

            // Find the book being returned using the BookID (or equivalent field in the IssueRequests table)
            var book = _context.Books.FirstOrDefault(b => b.BookID == request.BookID); // Make sure BookID exists

            if (book == null)
            {
                return Json(new { success = false, message = "Book not found." });
            }

            // Increment the NumberOfCopies of the book by 1
            book.NumberOfCopies++;

            // Update the status of the request and the return date
            request.Status = "Return"; // Updating status to 'Return'
            request.ReturnDate = DateTime.Now; // Setting the ReturnDate to the current date

            // Save changes to the database
            _context.SaveChanges();

            return Json(new { success = true, message = "Return successful. Number of copies updated." });
        }







        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        //end of the return book



        public IActionResult ArchivedBooks()
        {
            var archivedBooks = _context.Books.Where(b => b.IsArchived).ToList();
            return View(archivedBooks);
        }


        //WORK ISSUED BOOKS
       

        public IActionResult IssuedBooks()
        {
            var issuedBooks = _context.IssueRequests
                .Where(ir => ir.Status == "Return")  // Fetch books that are returned
                .Join(_context.Books, ir => ir.BookID, b => b.BookID, (ir, b) => new
                {
                    IssueRequest = ir,
                    Book = b
                })
                .ToList();

            foreach (var entry in issuedBooks)
            {
                var issueRequest = entry.IssueRequest;
                var book = entry.Book;

                if (issueRequest.ReturnDate.HasValue && issueRequest.ReturnDate > issueRequest.DueDate)
                {
                    int overdueDays = (issueRequest.ReturnDate.Value - issueRequest.DueDate).Days;
                    decimal overdueAmount = overdueDays * 10; // Assuming fine is ₱10 per day

                    // Update the OverdueAmount in the database
                    issueRequest.OverdueAmount = overdueAmount;
                    _context.Entry(issueRequest).State = EntityState.Modified;
                }
                else
                {
                    issueRequest.OverdueAmount = 0; // No overdue, set amount to 0
                    _context.Entry(issueRequest).State = EntityState.Modified;
                }
            }

            _context.SaveChanges(); // Persist changes

            // Pass the updated data to the view
            var viewModel = issuedBooks.Select(entry => new IssueRequest
            {
                RequestID = entry.IssueRequest.RequestID,
                BookID = entry.IssueRequest.BookID,
                IDNumber = entry.IssueRequest.IDNumber,
                Status = entry.IssueRequest.Status,
                RequestDate = entry.IssueRequest.RequestDate,
                DueDate = entry.IssueRequest.DueDate,
                Textbook = entry.Book.Textbook,
                NumberOfCopies = entry.Book.NumberOfCopies,
                ReturnDate = entry.IssueRequest.ReturnDate,
                OverdueAmount = entry.IssueRequest.OverdueAmount,
                 PaymentStatus = entry.IssueRequest.PaymentStatus
            }).ToList();

            return View(viewModel);
        }


        //WORK ISSUED BOOKS  

        public IActionResult ReturnRequest()
        {
            return View();
        }

        public IActionResult Add()
        {
            //var book = new Book();
            //ViewData["Title"] = "Add Book";
            return View();
        }
        public IActionResult Register()
        {
            return View();
        }
        public IActionResult Index()
        {
            return View();
        }


        public IActionResult Privacy()
        {
            return View();
        }

        

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

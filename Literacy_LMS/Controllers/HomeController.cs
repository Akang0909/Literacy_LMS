﻿using System.Diagnostics;
using Literacy_LMS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Literacy_LMS.Data;


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



        public IActionResult Messages()
        {
            return View();
        }

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
            // Get the issue requests along with the related Book data
            var issueRequests = _context.IssueRequests
                .Join(_context.Books, ir => ir.BookID, b => b.BookID, (ir, b) => new IssueRequest
                {
                    RequestID = ir.RequestID,
                    BookID = ir.BookID,
                    IDNumber = ir.IDNumber,
                    Status = ir.Status,
                    RequestDate = ir.RequestDate,
                    DueDate = ir.DueDate,  // Ensure that DueDate is included
                    Textbook = b.Textbook,
                    NumberOfCopies = b.NumberOfCopies
                })
                .Take(1000)
                .ToList();

            // Pass the data directly to the view (no need for a ViewModel now)
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

            request.Status = "Return"; // Updating status to 'Return'
            request.ReturnDate = DateTime.Now; // Setting the ReturnDate to the current date
            _context.SaveChanges();

            return Json(new { success = true, message = "Return successful." });
        }






        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        //end of the return book



        public IActionResult ArchivedBooks()
        {
            var archivedBooks = _context.Books.Where(b => b.IsArchived).ToList();
            return View(archivedBooks);
        }

        public IActionResult IssuedBooks()
        {
            var issuedBooks = _context.IssueRequests
                .Where(b => b.ReturnDate == null)  // Only show books that are not returned
                .OrderByDescending(b => b.RequestDate)
                .ToList();

            return View(issuedBooks);
        }


        

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

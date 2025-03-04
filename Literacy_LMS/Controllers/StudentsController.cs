using Literacy_LMS.Data;
using Literacy_LMS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using X.PagedList;
using X.PagedList.Extensions;

[Authorize(Roles = "Student")]
public class StudentsController : Controller
{
    private readonly ApplicationDbContext _context;

    public StudentsController(ApplicationDbContext context)
    {
        _context = context;
    }


    //this is for showing the dashboard 
    [Authorize(Roles = "Student")]
    public IActionResult Dashboard()
    {
        // Get the logged-in user's Id from AspNetUsers
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized(); // If no user is found, prevent access
        }

        // Query the Students table where UserId matches the logged-in user's Id
        var student = _context.Students
            .FirstOrDefault(s => s.UserId == userId);

        if (student == null)
        {
            ViewBag.StudentIDNumber = "Not Found"; // Handle case where student is not found
        }
        else
        {
            ViewBag.StudentIDNumber = student.IDNumber;
        }

        // Get Phone Number from AspNetUsers table
        var user = _context.Users
            .FirstOrDefault(u => u.Id == userId);

        if (user == null)
        {
            ViewBag.PhoneNumber = "Not Found";
        }
        else
        {
            ViewBag.PhoneNumber = user.PhoneNumber;
        }

        return View();
    }





    public IActionResult Details(int id)
    {
        var book = _context.Books.FirstOrDefault(b => b.BookID == id);
        if (book == null)
        {
            return NotFound(); // Or handle the case where the book is not found
        }
        return View(book); // Passing the book to the view
    }

    //public IActionResult Currently()
    //{
    //    return View();
    //}

    public IActionResult Previous()
    {
        
        return View();
    }




    public IActionResult Books(string search, int? page)
    {
        int pageSize = 23; // Number of books per page
        int pageNumber = page ?? 1; // Default to page 1 if none is specified

        var books = _context.Books.AsQueryable(); // Start with all books

        // Apply search filter if a search query is provided
        if (!string.IsNullOrEmpty(search))
        {
            books = books.Where(b =>
                b.Textbook.Contains(search) ||
                b.BookSection.Contains(search));
        }

        var pagedBooks = books.OrderBy(b => b.BookID) // Order books
                              .ToPagedList(pageNumber, pageSize); // Paginate

        return View("Books", pagedBooks);
    }


    //new risk
    [HttpPost]
    public JsonResult RequestBook(int bookId, string textbook)
    {
        // Get the logged-in user's ID (UserId from AspNetUsers)
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userId))
        {
            return Json(new { success = false, message = "User not authenticated" });
        }

        // Retrieve the student from the Students table based on the UserId
        var student = _context.Students.FirstOrDefault(s => s.UserId == userId);

        if (student == null)
        {
            return Json(new { success = false, message = "Student not found" });
        }

        // Use the student's IDNumber
        var issueRequest = new IssueRequest
        {
            BookID = bookId,
            IDNumber = student.IDNumber, // Now using the actual student ID from the Students table
            Textbook = textbook,
            NumberOfCopies = 1,
            Status = "Pending",
            RequestDate = DateTime.Now
        };

        _context.IssueRequests.Add(issueRequest);
        _context.SaveChanges();

        return Json(new { success = true });
    }

    //new
    public IActionResult Currently(string searchTerm)
    {
        var issueRequests = _context.IssueRequests
            .Join(_context.Books, ir => ir.BookID, b => b.BookID, (ir, b) => new IssueRequest
            {
                RequestID = ir.RequestID,
                BookID = ir.BookID,
                IDNumber = ir.IDNumber,
                Status = ir.Status,
                RequestDate = ir.RequestDate,
                DueDate = ir.DueDate,
                Textbook = b.Textbook,
                NumberOfCopies = b.NumberOfCopies
            })
            // Filter to get only accepted requests
            .Where(ir => ir.Status == "Accepted")
            .Take(1000)
            .ToList();

        return View(issueRequests);
    }






}

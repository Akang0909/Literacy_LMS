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


    [Authorize(Roles = "Student")]
    public IActionResult Dashboard()
    {
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
}

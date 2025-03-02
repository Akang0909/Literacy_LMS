using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Literacy_LMS.Data;
using Literacy_LMS.Models;
using X.PagedList;
using X.PagedList.Extensions;
using Microsoft.CodeAnalysis.Scripting;

namespace Literacy_LMS.Controllers
{
    public class BookController : Controller
    {
        private readonly ApplicationDbContext _context;
        private const int DefaultPageSize = 10; // Default items per page

        public BookController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Book/Add
        public IActionResult Add()
        {
            ViewData["Title"] = "Add Book";
            return View(new Book());
        }

        public IActionResult Details(int id)
        {
            var book = _context.Books.FirstOrDefault(b => b.BookID == id);
            if (book == null)
            {
                return NotFound();
            }
            return View(book);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var book = _context.Books.FirstOrDefault(b => b.BookID == id);
            if (book == null)
            {
                return NotFound();
            }
            return View(book);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Book book)
        {
            if (ModelState.IsValid)
            {
                var existingBook = _context.Books.Find(book.BookID);
                if (existingBook != null)
                {
                    existingBook.BookSection = book.BookSection;
                    existingBook.Subject = book.Subject;
                    existingBook.Textbook = book.Textbook;
                    existingBook.CopyrightYear = book.CopyrightYear;
                    existingBook.Volume = book.Volume;
                    existingBook.NumberOfCopies = book.NumberOfCopies;
                    existingBook.Author = book.Author;
                    existingBook.ISBN = book.ISBN;
                    existingBook.BookStatus = book.BookStatus;

                    _context.SaveChanges();

                    TempData["SuccessMessage"] = "Changes saved successfully!";
                }

                return RedirectToAction("Edit", new { id = book.BookID });
            }

            return View(book);
        }




        // POST: Book/Add
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Add(Book book)
        {
            if (ModelState.IsValid)
            {
                _context.Books.Add(book);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index)); // Redirect to book list
            }
            return View(book);
        }

        // Book List with Pagination
        public IActionResult Index(string search, string sectionFilter, string statusFilter, int page = 1, int pageSize = DefaultPageSize)
        {
            ViewData["Title"] = "Book List";

            var booksQuery = GetFilteredBooks(search, sectionFilter, statusFilter);

            // Convert to PagedList
            var pagedBooks = booksQuery.ToPagedList(page, pageSize);

            return View("Book", pagedBooks);
        }

        // AJAX-based Book Filtering with Pagination
        public IActionResult FilterBooks(string search, string sectionFilter, string statusFilter, int page = 1, int pageSize = DefaultPageSize)
        {
            var booksQuery = GetFilteredBooks(search, sectionFilter, statusFilter);

            // Convert to PagedList
            var pagedBooks = booksQuery.ToPagedList(page, pageSize);

            return PartialView("_BookTablePartial", pagedBooks);
        }

        // GET: Book List (with pagination)
        // GET: Book List (with pagination)
        public IActionResult Book(int page = 1, int pageSize = DefaultPageSize)
        {
            var books = _context.Books
                .OrderBy(b => b.BookID) // Change 'BookId' to the actual primary key field
                .ToPagedList(page, pageSize);

            return View("Book", books);
        }


        // Extracted Filtering Logic
        private IQueryable<Book> GetFilteredBooks(string search, string sectionFilter, string statusFilter)
        {
            var books = _context.Books.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                books = books.Where(b =>
                    b.Textbook.Contains(search) ||
                    b.BookSection.Contains(search));
            }

            if (!string.IsNullOrEmpty(sectionFilter))
            {
                books = books.Where(b => b.BookSection == sectionFilter);
            }

            if (!string.IsNullOrEmpty(statusFilter))
            {
                books = books.Where(b => b.BookStatus == statusFilter);
            }

            return books;
        }



        //students view 
   


      
    }
}



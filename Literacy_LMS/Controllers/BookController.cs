﻿using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Literacy_LMS.Data;
using Literacy_LMS.Models;
using X.PagedList;
using X.PagedList.Extensions;
using Microsoft.CodeAnalysis.Scripting;
using System.Security.Claims;

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
                // Save the book to the database
                _context.Books.Add(book);
                _context.SaveChanges();

                // Return a JSON response
                return Json(new { success = true, message = "Book added successfully!" });
            }

            // If model validation fails, return error
            return Json(new { success = false, message = "Failed to add book. Please check the input fields." });
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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RequestBook(int bookID, string bookName)
        {
            try
            {
                // Retrieve the logged-in user's IDNumber from claims
                var idNumber = User.FindFirstValue("IDNumber");

                if (string.IsNullOrEmpty(idNumber))
                {
                    return Json(new { success = false, message = "User not authenticated or IDNumber not found" });
                }

                // Validate input
                if (bookID == 0 || string.IsNullOrEmpty(bookName))
                {
                    return Json(new { success = false, message = "Invalid input data" });
                }

                // Create a new book request
                var request = new IssueRequest
                {
                    BookID = bookID,
                    IDNumber = idNumber,  // Use the IDNumber from the logged-in user
                    RequestDate = DateTime.Now,
                    Status = "Requested",
                    Textbook = bookName, // Assuming this is a string
                    NumberOfCopies = 1 // Adjust based on your logic
                };

                // Add to the database
                _context.IssueRequests.Add(request);
                _context.SaveChanges();

                return Json(new { success = true, message = "Request sent successfully" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }


        public IActionResult Archive(int id)
        {
            var book = _context.Books.FirstOrDefault(b => b.BookID == id);
            if (book == null)
            {
                return NotFound();
            }

            book.IsArchived = true;
            _context.SaveChanges();

            return RedirectToAction("Index"); // Redirect to the main book list
        }


    }
}



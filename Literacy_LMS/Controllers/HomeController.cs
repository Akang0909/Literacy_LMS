using System.Diagnostics;
using Literacy_LMS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;


namespace Literacy_LMS.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        [Authorize]
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
            return View();
        }

        public IActionResult RenewRequest()
        {
            return View();
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

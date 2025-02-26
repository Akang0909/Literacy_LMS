using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Authorize] // Requires login to access
public class DashboardController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}

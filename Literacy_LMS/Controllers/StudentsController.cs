using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Authorize(Roles = "Student")]
public class StudentsController : Controller
{
    public IActionResult Dashboard()
    {
        return View();
    }
}

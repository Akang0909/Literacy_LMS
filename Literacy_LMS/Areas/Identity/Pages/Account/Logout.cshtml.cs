using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Literacy_LMS.Areas.Identity.Pages.Account
{
    public class LogoutModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;

        public LogoutModel(SignInManager<IdentityUser> signInManager)
        {
            _signInManager = signInManager;
        }

        public async Task<IActionResult> OnPost()
        {
            // ✅ Clear session
            HttpContext.Session.Clear();
            Response.Cookies.Delete(".AspNetCore.Session"); // Delete session cookie
            Response.Cookies.Delete(".AspNetCore.Identity.Application"); // Delete authentication cookie

            // ✅ Sign out the user
            await _signInManager.SignOutAsync();

            // ✅ Redirect to Index (landing page)
            return RedirectToAction("Index", "Home");
        }
    }
}

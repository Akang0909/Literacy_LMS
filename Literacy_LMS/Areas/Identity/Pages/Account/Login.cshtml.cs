﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace Literacy_LMS.Areas.Identity.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<LoginModel> _logger;

        public LoginModel(SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager, ILogger<LoginModel> logger)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public string ReturnUrl { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [Display(Name = "Remember me?")]
            public bool RememberMe { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            returnUrl ??= Url.Content("~/");

            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            ReturnUrl = returnUrl;
        }

        //public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        //{
        //    returnUrl ??= Url.Content("~");

        //    ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

        //    if (ModelState.IsValid)
        //    {
        //        var user = await _userManager.FindByEmailAsync(Input.Email);
        //        if (user != null)
        //        {
        //            var result = await _signInManager.PasswordSignInAsync(user, Input.Password, Input.RememberMe, lockoutOnFailure: false);
        //            if (result.Succeeded)
        //            {
        //                _logger.LogInformation("User logged in.");

        //                var roles = await _userManager.GetRolesAsync(user);
        //                if (roles.Contains("Admin"))
        //                {
        //                    return LocalRedirect("~/Home/Dashboard");
        //                }
        //                else if (roles.Contains("Librarian"))
        //                {
        //                    return LocalRedirect("~/Admin/Dashboard");
        //                }
        //                else if (roles.Contains("Student"))
        //                {
        //                    return LocalRedirect("~/Students/Dashboard");
        //                }
        //                else
        //                {
        //                    return LocalRedirect("~/Home/Index");
        //                }
        //            }
        //            if (result.RequiresTwoFactor)
        //            {
        //                return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = Input.RememberMe });
        //            }
        //            if (result.IsLockedOut)
        //            {
        //                _logger.LogWarning("User account locked out.");
        //                return RedirectToPage("./Lockout");
        //            }
        //        }
        //        ModelState.AddModelError(string.Empty, "Invalid login attempt.");
        //    }
        //    return Page();
        //}

        //new
        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(Input.Email);
                if (user != null)
                {
                    var result = await _signInManager.PasswordSignInAsync(user, Input.Password, Input.RememberMe, lockoutOnFailure: false);
                    if (result.Succeeded)
                    {
                        _logger.LogInformation("User logged in.");

                        var roles = await _userManager.GetRolesAsync(user);
                        if (roles.Contains("Admin"))
                        {
                            return RedirectToAction("Dashboard", "Home"); // Ensure this matches your controller and action
                        }
                        else if (roles.Contains("Librarian"))
                        {
                            return RedirectToAction("Dashboard", "Admin");
                        }
                        else if (roles.Contains("Student"))
                        {
                            return RedirectToAction("Dashboard", "Students");
                        }
                        else
                        {
                            return RedirectToAction("Index", "Home");
                        }

                    }
                    if (result.RequiresTwoFactor)
                    {
                        // Redirect to 2FA page with the appropriate ReturnUrl and RememberMe state
                        return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = Input.RememberMe });
                    }
                    if (result.IsLockedOut)
                    {
                        _logger.LogWarning("User account locked out.");
                        return RedirectToPage("./Lockout");
                    }
                }
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            }
            return Page();
        }

    }
}
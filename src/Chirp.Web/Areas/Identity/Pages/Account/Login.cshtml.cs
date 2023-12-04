// Licensed to the .NET Foundation under one or more agreements.
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

using Chirp.Models;

namespace Chirp.Web.Areas.Identity.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly SignInManager<Author> _signInManager;
        private readonly UserManager<Author> _userManager;
        private readonly ILogger<LoginModel> _logger;

        public LoginModel(UserManager<Author> userManager, SignInManager<Author> signInManager, ILogger<LoginModel> logger)
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
            public string UserName { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [Display(Name = "Remember me?")]
            public bool RememberMe { get; set; }
        }

        public async Task<IActionResult> OnGetExternalLoginCallback(string returnUrl = null, string remoteError = null)
        {
            _logger.LogInformation("OnGetExternalLoginCallback reached");
            returnUrl = returnUrl ?? Url.Content("~/");
            if (remoteError != null)
            {
                ErrorMessage = $"Error from external provider: {remoteError}";
                return RedirectToPage("./Login", new { ReturnUrl = returnUrl });
            }

            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                _logger.LogInformation("info == null redirect to ./Login");
                return RedirectToPage("./Login", new { ReturnUrl = returnUrl });
            }

            // Here you can log the external login info details
            _logger.LogInformation($"External login info: Provider={info.ProviderDisplayName}, ProviderKey={info.ProviderKey}");

            // Rest of your external login handling
            return Page();
        }


        public async Task OnGetAsync(string returnUrl = null)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            returnUrl ??= Url.Content("~/");

            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            _logger.LogInformation("Loading external authentication schemes.");

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            ModelState.Clear();

            returnUrl ??= Url.Content("~/");

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            _logger.LogInformation("OnPostAsync reached after github login");

            if (ModelState.IsValid)
            {
                // 01. Locate the User and confirm its not NULL and its not 
                var user = await _userManager.FindByNameAsync(Input.UserName);

                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "User not found. Ya sure you registered your account?");
                    return Page();
                }
                else if(user.IsForgotten == true)
                {
                    ModelState.AddModelError(string.Empty, "This user has been 'Forgotten'. Cannot log in with this User.");
                    return Page();
                }

                var result = await _signInManager.PasswordSignInAsync(Input.UserName, Input.Password, Input.RememberMe, lockoutOnFailure: false);
                _logger.LogInformation($"Login attempt for user {Input.UserName} resulted in: {result}");   // [TODO] Remove


                _logger.LogInformation($"Logged in User 'ForgetMe' Status: {user.IsForgotten}");   // [TODO] Remove


                if (result.Succeeded)
                {
                    return RedirectToPage("/Public");
                }
                else if (result.RequiresTwoFactor) // [TODO] Consider removing - only adds unnecessary clutter.
                {
                    return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, Input.RememberMe });
                }
                else if (result.IsLockedOut)    // [TODO] Consider removing - only adds unnecessary clutter.
                {
                    return RedirectToPage("./Lockout");
                }
                else if (result.IsNotAllowed)
                {
                    string userNotAllowed_msg = "User is not allowed";

                    if (!await _userManager.IsEmailConfirmedAsync(user))
                    {
                        userNotAllowed_msg += " - Email is NOT confirmed.";
                    }

                    ModelState.AddModelError(string.Empty, userNotAllowed_msg);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return Page();
                }
            } 
            else if(!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "File: 'Public.cshtml.cs' - Method: 'OnPostDislikeOrLike()' - Message: ModelState was Invalid";
                return RedirectToPage("/Error");
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using ASPNETCoreIdentityDemo.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ASPNETCoreIdentityDemo.Controllers
{
    [Route("account")]
    public class AccountController : Controller
    {
        public UserManager<IdentityUser> UserManager { get; }
        public SignInManager<IdentityUser> SignInManager { get; }

        public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }



        [HttpGet]
        [Route("register")]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new IdentityUser
                {
                    UserName = model.Email,
                    Email = model.Email,


                };
                // create user 
                var result = await UserManager.CreateAsync(user, model.Password);
                //if user is created immidiatl
                if (result.Succeeded)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Index", "Home");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View(model);
        }

        [HttpGet]
        [Route("login")]
        public IActionResult Login(string? ReturnUrl = null)
        {
            ViewData["ReturnUrl"] = ReturnUrl;
            return View();
        }
        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login(LoginViewModel model, string? ReturnUrl)
        {
            if (ModelState.IsValid)
            {
                var result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure
                     : false);
                if (result.Succeeded)
                {
                    // Check if the ReturnUrl is not null and is a local URL
                    if (!string.IsNullOrEmpty(ReturnUrl) && Url.IsLocalUrl(ReturnUrl))
                    {
                        return Redirect(ReturnUrl);
                    }
                    else
                    {
                        // Redirect to default page
                        return RedirectToAction("Index", "Home");
                    }
                }
                if (result.RequiresTwoFactor)
                {
                    // Handle two-factor authentication case
                }
                if (result.IsLockedOut)
                {
                    // Handle lockout scenario
                }
                else
                {
                    // Handle failure
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return View(model);
                }
            }
            return View(model);
        }

        [HttpPost]
        [Route("/logout")]
        public async Task<IActionResult> Logout()
        {
            await SignInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> IsEmailAvailable(string Email)
        {

            var user = await UserManager.FindByEmailAsync(Email);
            if(user == null){
                return Json(true);
            }
            else{
                return Json($"Email {Email} is already in use");
            }
        }

    }
}
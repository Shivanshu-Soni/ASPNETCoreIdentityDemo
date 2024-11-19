# Redirecting to the ReturnUrl after login is a common use case in web applications. It enhances user experience by returning users to the originally requested page after they authenticate. This is useful in scenarios where users attempt to access restricted areas of your site and are redirected to the login page.

What happens when we try to access a URL that requires authentication? By default, ASP.NET Core redirects to the login URL with the ReturnUrl as the query string parameter. The URL that we were trying to access will be the value of the ReturnUrl query string parameter. The query string parameter name ReturnUrl is fixed. You cannot change this parameter name.

Example to Understand Redirect to ReturnUrl After Login in ASP.NET Core:
Let us understand this with an example. Here, we are going to use two Authorization Attributes. They are as follows:

Authorize Attribute: The [Authorize] attribute in ASP.NET Core specifies that only authenticated users can access a particular action method or controller. That means the [Authorize] attribute specifies that the associated action method or controller requires the user to be authenticated.

## AllowAnonymous Attribute: The [AllowAnonymous] attribute in ASP.NET Core specifies that a particular action method or controller should allow anonymous access, even when the [Authorize] attribute is applied to the controller or action methods. In other words, it permits unauthenticated users to access the decorated resource, bypassing any authentication and authorization checks that might be in place at higher levels.
Please check the following article to learn more about Authorization Filter in ASP.NET Core:

https://dotnettutorials.net/lesson/authorization-filters-in-asp-net-core-mvc/

Modifying the Home Controller:
To understand this better, please modify the Home Controller as follows. Here, you can see we have two action methods decorated with Authorize Attribute and another two action methods decorated with AllowAnonymous Attribute. Authenticated and anonymous users can access the Index and NonSecureMethod action methods as these two methods are decorated with AllowAnonymous Attribute. On the other hand, authenticated users can only access the Privacy and SecureMethod methods as these two methods are decorated with Authorize Attribute.

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace ASPNETCoreIdentityDemo.Controllers
{
public class HomeController : Controller
{
[AllowAnonymous]
public IActionResult Index()
{
return View();
}
[Authorize]
public IActionResult Privacy()
{
return View();
}
[AllowAnonymous]
public IActionResult NonSecureMethod()
{
return View();
}
[Authorize]
public IActionResult SecureMethod()
{
return View();
}
}
}
We already have the Index and Privacy view files in our application, which were, by default, created when we created the project using the Model-View-Controller Project template. So, let us create the other two view files. So, add the following two view files within the Home/Views folder of your application:

NonSecureMethod.cshtml
@{
ViewData["Title"] = "NonSecureMethod";
}

<h1>This is a Non-Secure Method</h1>
SecureMethod.cshtml
@{
    ViewData["Title"] = "SecureMethod";
}
<h1>This is a Secure Method</h1>
Link to All the Action Methods in the Layout File:
Next, we need to add the link for all these action methods to be visible in the menus. So, open _Layout.cshtml file and then copy and paste the following code.

@using Microsoft.AspNetCore.Identity
@inject SignInManager<IdentityUser> SignInManager

<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>@ViewData["Title"] - ASPNETCoreIdentityDemo</title>
    <link rel="stylesheet" href="~/lib/bootstrap/dist/css/bootstrap.min.css" />
    <link rel="stylesheet" href="~/css/site.css" asp-append-version="true" />
    <link rel="stylesheet" href="~/ASPNETCoreIdentityDemo.styles.css" asp-append-version="true" />
</head>
<body>
    <header>
        <nav class="navbar navbar-expand-sm navbar-toggleable-sm navbar-light bg-white border-bottom box-shadow mb-3">
            <div class="container-fluid">
                <a class="navbar-brand" asp-controller="Home" asp-action="Index">Home</a>
                <button class="navbar-toggler" type="button" data-bs-toggle="collapse" data-bs-target=".navbar-collapse" aria-controls="navbarSupportedContent"
                        aria-expanded="false" aria-label="Toggle navigation">
                    <span class="navbar-toggler-icon"></span>
                </button>
                <div class="navbar-collapse collapse d-sm-inline-flex justify-content-between">
                    <ul class="navbar-nav flex-grow-1">
                        <li class="nav-item">
                            <a class="nav-link text-dark" asp-controller="Home" asp-action="Privacy">Privacy</a>
                        </li>
                        <li class="nav-item">
                            <a class="nav-link text-dark" asp-controller="Home" asp-action="SecureMethod">Secure</a>
                        </li>
                        <li class="nav-item">
                            <a class="nav-link text-dark" asp-controller="Home" asp-action="NonSecureMethod">Non Secure</a>
                        </li>
                    </ul>
                    <ul class="navbar-nav ml-auto">
                        @*If the user is signed-in display Logout link*@
                        @if (SignInManager.IsSignedIn(User))
                        {
                            <li class="nav-item">
                                <form method="post" asp-controller="account" asp-action="logout">
                                    <button type="submit" style="width:auto"
                                            class="nav-link btn btn-link py-0">
                                        Logout @User?.Identity?.Name
                                    </button>
                                </form>
                            </li>
                        }
                        else
                        {
                            <li class="nav-item">
                                <a class="nav-link" asp-controller="account" asp-action="register">
                                    Register
                                </a>
                            </li>
                            <li class="nav-item">
                                <a class="nav-link" asp-controller="account" asp-action="login">
                                    Login
                                </a>
                            </li>
                        }
                    </ul>
                </div>
            </div>
        </nav>
    </header>
    <div class="container">
        <main role="main" class="pb-3">
            @RenderBody()
        </main>
    </div>
    <footer class="border-top footer text-muted">
        <div class="container">
            &copy; 2023 - ASPNETCoreIdentityDemo - <a asp-area="" asp-controller="Home" asp-action="Privacy">Privacy</a>
        </div>
    </footer>
    <script src="~/lib/jquery/dist/jquery.min.js"></script>
    <script src="~/lib/bootstrap/dist/js/bootstrap.bundle.min.js"></script>
    <script src="~/js/site.js" asp-append-version="true"></script>
    @await RenderSectionAsync("Scripts", required: false)
</body>
</html>

How to Configure the Login URL for the Unauthenticated User to be Redirected in ASP.NET Core Identity?
## In ASP.NET Core Identity, we can configure the login URL to which an unauthenticated user should be redirected when the user trying to access a page for which authentication is required. This is done in the Program.cs file of your ASP.NET Core project, where you set up the middleware and services for ASP.NET Core Identity.

## So, in the Program.cs class file, find the part where the Identity services are configured and add the following code. Here, we need to configure the login URL using the ApplicationCookie options. Then, we need to specify the login path using the LoginPath property. This path is where users will be redirected if they attempt to access a resource that requires authentication but is not authenticated.

// Configure the Application Cookie settings
builder.Services.ConfigureApplicationCookie(options =>
{
// If the LoginPath isn't set, ASP.NET Core defaults the path to /Account/Login.
options.LoginPath = "/Account/Login"; // Set your login path here
});
Note: The point that you need to remember is the default value for LoginPath is the Account Controller Login Action method. If you want to redirect the user to the Account Controller Login Action method, then it is optional to set the LoginPath. But if your path is other than the Account Controller Login Action method, then it is mandatory to set the Login path for unauthenticated users.

Testing the Application:
With the above changes in place, now run the application, and without login, click on the Secure link from the menu as shown in the below image:

How to Redirect to ReturnUrl After Login in ASP.NET Core

Once you click on the Secure link, as the user is not yet logged in, it will open the following login page. That means redirecting to the login page is working as expected. Here, provide a valid Email ID and Password and click on the Login button as shown in the below image:

How to Redirect to ReturnUrl After Login in ASP.NET Core

Now, once you click on the Login button, if credentials are valid, then it will log in the user and redirect you to the Home page as shown in the below image:

How to Redirect to ReturnUrl After Login in ASP.NET Core

Instead of navigating to the Home Page, we want to redirect the user to the Secure page they initially requested. Let’s proceed and see how to implement this in our ASP.NET Core Application.

How do you redirect to ReturnUrl after logging in?
The ReturnUrl query string parameter indicates where to send the user after successfully logging in, especially if the user attempts to access a resource or page that requires authentication.

Capture the ReturnUrl:
When a user tries to access a protected page, redirect the user to the login action method and pass the original URL as the query string parameter. The good news is that, in ASP.NET Core, the ReturnUrl mechanism works automatically with the built-in authentication middleware, especially when using Identity systems. This is useful for redirecting users back to their original destination after being prompted to log in.

The ASP.NET Core Framework is responsible for appending the original URL or Redirect URL while navigating the user to the Login Page. What we all need to do is, we need to have a parameter in the Login action method to capture the query string parameter value. The parameter name in the Login action method to capture the query string value must be ReturnUrl. Within the login action method, after successful login, we need to check this ReturnUrl parameter value, and if it is not null or empty and is a local URL, then we need to redirect the user to that URL.

HttpGet Login Action Method:
So, modify the HttpGet version of your Login action method of Account Controller as follows. Here, we make the ReturnUrl parameter optional by initializing it with the null value. Then, store that ReturnUrl in the ViewData, which will be used as a hidden field in the login view so that we can post this value to the Login HTTP Post action method.

[HttpGet]
public IActionResult Login(string? ReturnUrl = null)
{
ViewData["ReturnUrl"] = ReturnUrl;
return View();
}
Modifying Login Page:
In the Login page (in our example Login.cshtml view), ensure the login form includes the ReturnUrl as a hidden field. This will ensure that the ReturnUrl is sent back to the server when the user submits the form. So, modify the Login.cshtml file as follows:

@model LoginViewModel
@{
ViewBag.Title = "User Login";
}

<h1>User Login</h1>
<div class="row">
    <div class="col-md-12">
        <form method="post">
            <input type="hidden" name="ReturnUrl" value="@ViewData["ReturnUrl"]" />
            <div asp-validation-summary="All" class="text-danger"></div>
            <div class="form-group">
                <label asp-for="Email"></label>
                <input asp-for="Email" class="form-control" />
                <span asp-validation-for="Email" class="text-danger"></span>
            </div>
            <div class="form-group">
                <label asp-for="Password"></label>
                <input asp-for="Password" class="form-control" />
                <span asp-validation-for="Password" class="text-danger"></span>
            </div>
            <div class="form-group">
                <div class="checkbox">
                    <label asp-for="RememberMe">
                        <input asp-for="RememberMe" />
                        @Html.DisplayNameFor(m => m.RememberMe)
                    </label>
                </div>
            </div>
            <button type="submit" class="btn btn-primary">Login</button>
        </form>
    </div>
</div>
HttpPost Login Action Method:
Once the user submits their credentials, validate them, and upon successful validation, if a valid ReturnUrl is present, redirect the user to that URL. If no ReturnUrl is present, redirect the user to a default page, like the homepage or dashboard. So, modify the HTTP Post version of the Login Action method of the Account Controller as follows:

[HttpPost]
[AllowAnonymous]
public async Task<IActionResult> Login(LoginViewModel model, string? ReturnUrl)
{
if (ModelState.IsValid)
{
var result = await signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);
if (result.Succeeded)
{
// Handle successful login
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
// If we got this far, something failed, redisplay form
return View(model);
}
With the above changes in place, run the application, and it should work as expected. Now run the application, and click on the Secure link from the menu as shown in the below image:

Example to Understand Redirect to ReturnUrl After Login in ASP.NET Core

Once you click on the Secure link, it will redirect to the following login page. Here, provide a valid Email ID and Password and click on the Login button as shown in the below image:

Example to Understand Redirect to ReturnUrl After Login in ASP.NET Core

Now, once you click on the Login button, if the credentials are valid, then it will log in the user and redirect back to the Secure page as shown in the below image:

Example to Understand Redirect to ReturnUrl After Login in ASP.NET Core

What is the Use of the IsLocalUrl Method in ASP.NET Core?
The IsLocalUrl method in ASP.NET Core determines whether the given URL is local. This method is useful when redirecting a user after an operation like login, as it helps prevent open redirect attacks. To test this, modify the hidden ReturnUrl field using the browser developer tool and set the value to a different website URL.

An Open Redirect Attack occurs when an application redirects users to an external, potentially malicious URL. By using the IsLocalUrl method, you can ensure that your application only redirects to URLs within your application, thereby avoiding such security risks.

#### check Task/List
- item1 
- item 2
    - itmem 3
- [ ] taks1 
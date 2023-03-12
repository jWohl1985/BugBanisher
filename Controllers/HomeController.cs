using BugBanisher.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace BugBanisher.Controllers;

public class HomeController : Controller
{
    private readonly SignInManager<AppUser> _signInManager;

    public HomeController(SignInManager<AppUser> signInManager)
    {
        _signInManager = signInManager;
    }

    public IActionResult Index()
    {
        if (!_signInManager.IsSignedIn(User))
            return Redirect("/Identity/Account/Login");

        return View();
    }
}
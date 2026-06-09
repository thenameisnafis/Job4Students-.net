using Job4Students.Models;
using Job4Students.Repos;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Job4Students.Web.Controllers;

public class AuthController(AuthRepo authRepo) : Controller
{
    [HttpGet]
    [AllowAnonymous]
    public IActionResult Login()
    {
        return View(new LoginModel());
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Login(LoginModel model)
    {
        if (ModelState.IsValid == false)
        {
            return View(model);
        }

        var result = authRepo.Login(model);

        if (result.IsSuccess == false || result.Data == null)
        {
            ViewBag.Error = result.Message;
            return View(model);
        }

        var claims = new List<Claim>()
        {
            new Claim(ClaimTypes.Name, result.Data.FullName),
            new Claim(ClaimTypes.Role, result.Data.Role),
            new Claim(ClaimTypes.Email, result.Data.Email),
            new Claim("Email", result.Data.Email),
            new Claim("UserId", result.Data.Id.ToString())
        };

        var identity = new ClaimsIdentity(claims, "JobAuth");
        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync("JobAuth", principal);

        if (result.Data.Role == "student")
        {
            return RedirectToAction("Index", "StudentDashboard");
        }

        if (result.Data.Role == "employer")
        {
            return RedirectToAction("Index", "EmployerDashboard");
        }

        if (result.Data.Role == "admin")
        {
            return RedirectToAction("Index", "Admin");
        }

        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Register()
    {
        return View(new RegisterModel());
    }

    [HttpPost]
    [AllowAnonymous]
    public IActionResult RegisterStudent(RegisterModel model)
    {
        if (ModelState.IsValid == false)
        {
            return View("Register");
        }

        var result = authRepo.RegisterStudent(model);

        if (result.IsSuccess)
        {
            TempData["Success"] = result.Message;
            return RedirectToAction(nameof(Login));
        }

        TempData["Error"] = result.Message;
        return View("Register");
    }

    [HttpPost]
    [AllowAnonymous]
    public IActionResult RegisterEmployer(RegisterModel model)
    {
        if (ModelState.IsValid == false)
        {
            return View("Register");
        }

        var result = authRepo.RegisterEmployer(model);

        if (result.IsSuccess)
        {
            TempData["Success"] = result.Message;
            return RedirectToAction(nameof(Login));
        }

        TempData["Error"] = result.Message;
        return View("Register");
    }

    [Authorize]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync("JobAuth");
        return RedirectToAction(nameof(Login));
    }

    [AllowAnonymous]
    public IActionResult Denied()
    {
        return Content("Access Denied");
    }
}

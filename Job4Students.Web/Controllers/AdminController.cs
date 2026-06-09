using Job4Students.Models;
using Job4Students.Repos;
using Job4Students.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Job4Students.Web.Controllers;

[Authorize(Roles = "admin")]
public class AdminController(AdminRepo adminRepo, JobRepo jobRepo, ApplicationRepo applicationRepo, CurrentUserHelper currentUserHelper) : Controller
{
    public IActionResult Index()
    {
        var result = adminRepo.GetDashboard();
        return View(result);
    }

    [HttpGet]
    public IActionResult Profile()
    {
        var profile = adminRepo.GetProfile(currentUserHelper.UserId);
        if (profile == null)
        {
            TempData["Error"] = "Admin profile not found.";
            return RedirectToAction("Index");
        }

        return View(profile);
    }

    [HttpPost]
    public IActionResult ChangeCredentials(AccountCredentialsModel model)
    {
        var result = adminRepo.ChangeCredentials(currentUserHelper.UserId, model);

        if (result.IsSuccess)
        {
            TempData["Success"] = result.Message;
        }
        else
        {
            TempData["Error"] = result.Message;
        }

        return RedirectToAction(nameof(Profile));
    }

    public IActionResult Users(string? role, string? status)
    {
        ViewBag.Role = role;
        ViewBag.Status = status;

        var users = adminRepo.GetUsers(role, status);
        return View(users);
    }

    [HttpPost]
    public IActionResult UpdateUserStatus(int id, string status)
    {
        var result = adminRepo.UpdateUserStatus(id, status);

        if (result.IsSuccess)
        {
            TempData["Success"] = result.Message;
        }
        else
        {
            TempData["Error"] = result.Message;
        }

        return RedirectToAction(nameof(Users));
    }

    [HttpPost]
    public IActionResult DeleteUser(int id)
    {
        var result = adminRepo.DeleteUser(id);

        if (result.IsSuccess)
        {
            TempData["Success"] = result.Message;
        }
        else
        {
            TempData["Error"] = result.Message;
        }

        return RedirectToAction(nameof(Users));
    }

    public IActionResult Jobs()
    {
        var jobs = jobRepo.GetAllJobsForAdmin();
        return View(jobs);
    }

    [HttpPost]
    public IActionResult UpdateJobStatus(int id, string status)
    {
        var result = jobRepo.ChangeStatus(id, status);

        if (result.IsSuccess)
        {
            TempData["Success"] = result.Message;
        }
        else
        {
            TempData["Error"] = result.Message;
        }

        return RedirectToAction(nameof(Jobs));
    }

    public IActionResult Applications()
    {
        var applications = applicationRepo.GetAllApplications();
        return View(applications);
    }
}

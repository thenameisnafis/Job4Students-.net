using Job4Students.Models;
using Job4Students.Repos;
using Job4Students.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Job4Students.Web.Controllers;

[Authorize(Roles = "employer")]
public class EmployerDashboardController(EmployerRepo employerRepo, ApplicationRepo applicationRepo, JobRepo jobRepo, CurrentUserHelper currentUserHelper) : Controller
{
    public IActionResult Index()
    {
        ViewBag.Jobs = jobRepo.GetEmployerJobs(currentUserHelper.UserId);
        var dashboard = employerRepo.GetDashboard(currentUserHelper.UserId);
        return View(dashboard);
    }

    public IActionResult Applications()
    {
        var applications = applicationRepo.GetEmployerApplications(currentUserHelper.UserId);
        return View(applications);
    }

    [HttpPost]
    public IActionResult UpdateApplicationStatus(int id, string status)
    {
        var result = applicationRepo.UpdateStatus(id, currentUserHelper.UserId, status);

        if (result.IsSuccess)
        {
            TempData["Success"] = result.Message;
        }
        else
        {
            TempData["Error"] = result.Message;
        }

        return RedirectToAction(nameof(Applications));
    }

    [HttpGet]
    public IActionResult Profile()
    {
        var profile = employerRepo.GetProfile(currentUserHelper.UserId);
        if (profile == null)
        {
            TempData["Error"] = "Employer profile not found.";
            return RedirectToAction("Index");
        }

        return View(profile);
    }

    [HttpPost]
    public IActionResult Profile(EmployerProfileModel model)
    {
        var result = employerRepo.UpdateProfile(currentUserHelper.UserId, model);

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

    [HttpPost]
    public IActionResult ChangeCredentials(AccountCredentialsModel model)
    {
        var result = employerRepo.ChangeCredentials(currentUserHelper.UserId, model);

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
}

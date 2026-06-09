using Job4Students.Models;
using Job4Students.Repos;
using Job4Students.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Job4Students.Web.Controllers;

[Authorize(Roles = "student")]
public class StudentDashboardController(StudentRepo studentRepo, ApplicationRepo applicationRepo, JobRepo jobRepo, CurrentUserHelper currentUserHelper) : BaseController
{
    public IActionResult Index()
    {
        var dashboard = studentRepo.GetDashboard(currentUserHelper.UserId);
        return View(dashboard);
    }

    public IActionResult MyApplications()
    {
        var applications = applicationRepo.GetStudentApplications(currentUserHelper.UserId);
        return View(applications);
    }

    [HttpGet]
    public IActionResult Apply(int jobId)
    {
        var job = jobRepo.GetDetails(jobId);
        if (job == null)
        {
            TempData["Error"] = "Job not found.";
            return RedirectToAction("Index", "Jobs");
        }

        ViewBag.Job = job;
        return View(new ApplicationCreateModel { JobId = jobId });
    }

    [HttpPost]
    public IActionResult Apply(ApplicationCreateModel model, IFormFile? cvFile)
    {
        var job = jobRepo.GetDetails(model.JobId);
        if (job == null)
        {
            TempData["Error"] = "Job not found.";
            return RedirectToAction("Index", "Jobs");
        }

        ViewBag.Job = job;

        if (string.IsNullOrWhiteSpace(model.CoverLetter))
        {
            ModelState.AddModelError(nameof(model.CoverLetter), "Cover letter is required.");
        }

        if (cvFile == null || cvFile.Length == 0)
        {
            ModelState.AddModelError("cvFile", "CV upload is required.");
        }

        if (ModelState.IsValid == false)
        {
            return View(model);
        }

        model.CvFilePath = SaveFile(cvFile, "application_cv", FileUploadOptions.CvExtensions, FileUploadOptions.MaxCvBytes);
        if (string.IsNullOrWhiteSpace(model.CvFilePath))
        {
            ModelState.AddModelError("cvFile", "CV must be PDF, DOC, or DOCX and max 5 MB.");
            return View(model);
        }

        var result = applicationRepo.Apply(currentUserHelper.UserId, model);

        if (result.IsSuccess)
        {
            TempData["Success"] = result.Message;
            return RedirectToAction(nameof(MyApplications));
        }

        TempData["Error"] = result.Message;
        return RedirectToAction("Details", "Jobs", new { id = model.JobId });
    }

    [HttpPost]
    public IActionResult Withdraw(int id)
    {
        var result = applicationRepo.Withdraw(id, currentUserHelper.UserId);

        if (result.IsSuccess)
        {
            TempData["Success"] = result.Message;
        }
        else
        {
            TempData["Error"] = result.Message;
        }

        return RedirectToAction(nameof(MyApplications));
    }

    [HttpGet]
    public IActionResult Profile()
    {
        var profile = studentRepo.GetProfile(currentUserHelper.UserId);
        if (profile == null)
        {
            TempData["Error"] = "Student profile not found.";
            return RedirectToAction("Index");
        }

        return View(profile);
    }

    [HttpPost]
    public IActionResult Profile(StudentProfileModel model, IFormFile? cvFile)
    {
        model.CvFilePath = SaveFile(cvFile, "cv", FileUploadOptions.CvExtensions, FileUploadOptions.MaxCvBytes);
        var result = studentRepo.UpdateProfile(currentUserHelper.UserId, model);

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
        var result = studentRepo.ChangeCredentials(currentUserHelper.UserId, model);

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

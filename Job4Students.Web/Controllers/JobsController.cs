using Job4Students.Models;
using Job4Students.Repos;
using Job4Students.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Job4Students.Web.Controllers;

public class JobsController(JobRepo jobRepo, CurrentUserHelper currentUserHelper) : BaseController
{
    [AllowAnonymous]
    public IActionResult Index(JobFilterModel filter)
    {
        ViewBag.Filter = filter;

        var jobs = User.IsInRole("employer") && currentUserHelper.UserId > 0
            ? jobRepo.GetEmployerJobs(currentUserHelper.UserId)
            : jobRepo.GetOpenJobs(filter);

        return View(jobs);
    }

    [AllowAnonymous]
    public IActionResult Details(int id)
    {
        var job = jobRepo.GetDetails(id);

        if (job == null)
        {
            TempData["Error"] = "Job not found.";
            return RedirectToAction("Index");
        }

        return View(job);
    }

    [HttpGet]
    [Authorize(Roles = "employer")]
    public IActionResult Create()
    {
        return View(new JobCreateModel());
    }

    [HttpPost]
    [Authorize(Roles = "employer")]
    public IActionResult Create(JobCreateModel model, List<IFormFile>? images)
    {
        if (ModelState.IsValid == false)
        {
            return View(model);
        }

        if (images != null)
        {
            foreach (var image in images)
            {
                var path = SaveFile(image, "jobs", FileUploadOptions.ImageExtensions, FileUploadOptions.MaxImageBytes);
                if (path != null)
                {
                    model.ImagePaths.Add(path);
                }
            }
        }

        var result = jobRepo.Create(currentUserHelper.UserId, model);

        if (result.IsSuccess)
        {
            TempData["Success"] = result.Message;
            return RedirectToAction("Index", "EmployerDashboard");
        }

        ViewBag.ErrorMessage = result.Message;
        return View(model);
    }

    [HttpGet]
    [Authorize(Roles = "employer")]
    public IActionResult Edit(int id)
    {
        var job = jobRepo.GetDetails(id);
        if (job == null)
        {
            TempData["Error"] = "Job not found.";
            return RedirectToAction("Index", "EmployerDashboard");
        }

        if (job.EmployerUserId != currentUserHelper.UserId)
        {
            TempData["Error"] = "You are not allowed to edit this job.";
            return RedirectToAction("Index", "Home");
        }

        var model = new JobUpdateModel();
        model.Id = job.Id;
        model.Title = job.Title;
        model.Description = job.Description;
        model.JobType = job.JobType;
        model.Industry = job.Industry;
        model.Location = job.Location;
        model.CompanyName = job.CompanyName;
        model.Salary = job.Salary;
        model.Requirements = job.Requirements;
        model.ApplicationDeadline = job.ApplicationDeadline;
        model.Status = job.Status;

        return View(model);
    }

    [HttpPost]
    [Authorize(Roles = "employer")]
    public IActionResult Edit(int id, JobUpdateModel model)
    {
        if (ModelState.IsValid == false)
        {
            return View(model);
        }

        var result = jobRepo.Update(id, currentUserHelper.UserId, model);

        if (result.IsSuccess)
        {
            TempData["Success"] = result.Message;
            return RedirectToAction("Index", "EmployerDashboard");
        }

        ViewBag.ErrorMessage = result.Message;
        return View(model);
    }

    [HttpPost]
    [Authorize(Roles = "employer")]
    public IActionResult Delete(int id)
    {
        var result = jobRepo.Delete(id, currentUserHelper.UserId);

        if (result.IsSuccess)
        {
            TempData["Success"] = result.Message;
        }
        else
        {
            TempData["Error"] = result.Message;
        }

        return RedirectToAction("Index", "EmployerDashboard");
    }
}

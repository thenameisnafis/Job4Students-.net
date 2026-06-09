using Job4Students.Repos;
using Job4Students.Models;
using Microsoft.AspNetCore.Mvc;

namespace Job4Students.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ApplicationController(ApplicationRepo repo) : ControllerBase
{
    [HttpPost("Apply/{studentUserId}")]
    public IActionResult Apply(int studentUserId, ApplicationCreateModel application)
    {
        var result = repo.Apply(studentUserId, application);

        if (!result.IsSuccess)
            return BadRequest(result.Message);

        return Ok(result);
    }

    [HttpGet("Student/{studentUserId}")]
    public IActionResult StudentApplications(int studentUserId)
    {
        var applications = repo.GetStudentApplications(studentUserId);
        return Ok(applications);
    }

    [HttpGet("Employer/{employerUserId}")]
    public IActionResult EmployerApplications(int employerUserId)
    {
        var applications = repo.GetEmployerApplications(employerUserId);
        return Ok(applications);
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        var applications = repo.GetAllApplications();
        return Ok(applications);
    }

    [HttpPatch("UpdateStatus/{id}/Employer/{employerUserId}/{status}")]
    public IActionResult UpdateStatus(int id, int employerUserId, string status)
    {
        var result = repo.UpdateStatus(id, employerUserId, status);

        if (!result.IsSuccess)
            return BadRequest(result.Message);

        return Ok(result);
    }

    [HttpPatch("Withdraw/{id}/Student/{studentUserId}")]
    public IActionResult Withdraw(int id, int studentUserId)
    {
        var result = repo.Withdraw(id, studentUserId);

        if (!result.IsSuccess)
            return BadRequest(result.Message);

        return Ok(result);
    }
}

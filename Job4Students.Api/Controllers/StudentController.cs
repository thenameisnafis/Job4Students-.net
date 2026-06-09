using Job4Students.Repos;
using Job4Students.Models;
using Microsoft.AspNetCore.Mvc;

namespace Job4Students.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class StudentController(StudentRepo studentRepo, AuthRepo authRepo) : ControllerBase
{
    [HttpPost("Register")]
    public IActionResult Register(RegisterModel student)
    {
        var result = authRepo.RegisterStudent(student);

        if (!result.IsSuccess)
            return BadRequest(result.Message);

        return Ok(result);
    }

    [HttpGet("Dashboard/{userId}")]
    public IActionResult Dashboard(int userId)
    {
        var dashboard = studentRepo.GetDashboard(userId);
        return Ok(dashboard);
    }

    [HttpGet("Profile/{userId}")]
    public IActionResult Profile(int userId)
    {
        var profile = studentRepo.GetProfile(userId);

        if (profile is null)
            return NotFound();

        return Ok(profile);
    }

    [HttpPut("UpdateProfile/{userId}")]
    public IActionResult UpdateProfile(int userId, StudentProfileModel profile)
    {
        var result = studentRepo.UpdateProfile(userId, profile);

        if (!result.IsSuccess)
            return BadRequest(result.Message);

        return Ok(result);
    }

    [HttpPut("ChangeCredentials/{userId}")]
    public IActionResult ChangeCredentials(int userId, AccountCredentialsModel credentials)
    {
        var result = studentRepo.ChangeCredentials(userId, credentials);

        if (!result.IsSuccess)
            return BadRequest(result.Message);

        return Ok(result);
    }
}

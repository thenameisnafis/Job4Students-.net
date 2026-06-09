using Job4Students.Repos;
using Job4Students.Models;
using Microsoft.AspNetCore.Mvc;

namespace Job4Students.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class JobController(JobRepo repo) : ControllerBase
{
    [HttpGet]
    public IActionResult GetAll([FromQuery] JobFilterModel filter)
    {
        var jobs = repo.GetOpenJobs(filter);
        return Ok(jobs);
    }

    [HttpGet("GetJobById/{id}")]
    public IActionResult GetJobById(int id)
    {
        var job = repo.GetDetails(id);

        if (job is null)
            return NotFound();

        return Ok(job);
    }

    [HttpPost("Create/{employerUserId}")]
    public IActionResult PostJob(int employerUserId, JobCreateModel job)
    {
        var result = repo.Create(employerUserId, job);

        if (!result.IsSuccess)
            return BadRequest(result.Message);

        return Ok(result);
    }

    [HttpPut("Update/{id}/Employer/{employerUserId}")]
    public IActionResult Update(int id, int employerUserId, JobUpdateModel job)
    {
        var result = repo.Update(id, employerUserId, job);

        if (!result.IsSuccess)
            return BadRequest(result.Message);

        return Ok(result);
    }

    [HttpDelete("Delete/{id}/Employer/{employerUserId}")]
    public IActionResult Delete(int id, int employerUserId)
    {
        var result = repo.Delete(id, employerUserId);

        if (!result.IsSuccess)
            return BadRequest(result.Message);

        return Ok(result);
    }

    [HttpPatch("ChangeStatus/{id}/{status}")]
    public IActionResult ChangeStatus(int id, string status)
    {
        var result = repo.ChangeStatus(id, status);

        if (!result.IsSuccess)
            return BadRequest(result.Message);

        return Ok(result);
    }
}

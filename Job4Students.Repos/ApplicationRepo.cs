using Job4Students.Data;
using Job4Students.Shared;
using Job4Students.Models;
using Job4Students.Entities;
using Microsoft.EntityFrameworkCore;

namespace Job4Students.Repos;

public class ApplicationRepo(JobStudentDbContext context, CurrentUserHelper currentUserHelper)
{

    public Result<int> Apply(int studentUserId, ApplicationCreateModel model)
    {
        try
        {
            Student? student = context.Students
                .Include(s => s.AppUser)
                .FirstOrDefault(s => s.AppUserId == studentUserId);

            if (student == null)
            {
                return Result<int>.Failure(ErrorMessages.Unauthorized);
            }

            if (student.AppUser.Status != "active")
            {
                return Result<int>.Failure(ErrorMessages.AccountInactive);
            }

            Job? job = context.Jobs
                .Include(j => j.Employer)
                .FirstOrDefault(j => j.Id == model.JobId && !j.IsDeleted);

            if (job == null)
            {
                return Result<int>.Failure(ErrorMessages.NotFound);
            }

            if (job.Status != "open" || job.ApplicationDeadline.Date < DateTime.Today)
            {
                return Result<int>.Failure(ErrorMessages.JobClosed);
            }

            bool exists = context.Applications.Any(a => a.StudentId == student.Id && a.JobId == model.JobId);
            if (exists)
            {
                return Result<int>.Failure(ErrorMessages.DuplicateApplication);
            }

            if (string.IsNullOrWhiteSpace(model.CoverLetter))
            {
                return Result<int>.Failure("Cover letter is required.");
            }

            if (string.IsNullOrWhiteSpace(model.CvFilePath))
            {
                return Result<int>.Failure("CV upload is required.");
            }

            Application application = new Application();
            application.StudentId = student.Id;
            application.EmployerId = job.EmployerId;
            application.JobId = job.Id;
            application.ExpectedStartDate = model.ExpectedStartDate;
            application.CoverLetter = model.CoverLetter;
            application.CvFilePath = model.CvFilePath;
            application.Status = "pending";

            context.Applications.Add(application);
            context.SaveChanges();

            return Result<int>.Success(application.Id, SuccessMessages.Applied);
        }
        catch (Exception ex)
        {
            return Result<int>.Failure(ex.Message);
        }
    }

    public List<ApplicationListItemModel> GetStudentApplications(int studentUserId)
    {
        var applications = GetApplicationQuery()
            .Where(a => a.Student.AppUserId == studentUserId)
            .OrderByDescending(a => a.ApplicationDate)
            .ToList();

        return ConvertToListModel(applications);
    }

    public List<ApplicationListItemModel> GetEmployerApplications(int employerUserId)
    {
        var applications = GetApplicationQuery()
            .Where(a => a.Employer.AppUserId == employerUserId)
            .OrderByDescending(a => a.ApplicationDate)
            .ToList();

        return ConvertToListModel(applications);
    }

    public List<ApplicationListItemModel> GetAllApplications()
    {
        var applications = GetApplicationQuery()
            .OrderByDescending(a => a.ApplicationDate)
            .ToList();

        return ConvertToListModel(applications);
    }

    public Result UpdateStatus(int applicationId, int employerUserId, string status)
    {
        try
        {
            Application? application = context.Applications
                .Include(a => a.Employer)
                .FirstOrDefault(a => a.Id == applicationId);

            if (application == null)
            {
                return Result.Failure(ErrorMessages.NotFound);
            }

            if (application.Employer.AppUserId != employerUserId)
            {
                return Result.Failure(ErrorMessages.Unauthorized);
            }

            application.Status = status;
            context.SaveChanges();

            return Result.Success(SuccessMessages.Updated);
        }
        catch (Exception ex)
        {
            return Result.Failure(ex.Message);
        }
    }

    public Result Withdraw(int applicationId, int studentUserId)
    {
        try
        {
            Application? application = context.Applications
                .Include(a => a.Student)
                .FirstOrDefault(a => a.Id == applicationId);

            if (application == null)
            {
                return Result.Failure(ErrorMessages.NotFound);
            }

            if (application.Student.AppUserId != studentUserId)
            {
                return Result.Failure(ErrorMessages.Unauthorized);
            }

            if (application.Status != "pending" && application.Status != "approved")
            {
                return Result.Failure("Only pending or approved applications can be withdrawn.");
            }

            application.Status = "cancelled";
            context.SaveChanges();

            return Result.Success(SuccessMessages.Updated);
        }
        catch (Exception ex)
        {
            return Result.Failure(ex.Message);
        }
    }

    private IQueryable<Application> GetApplicationQuery()
    {
        return context.Applications
            .Include(a => a.Job)
            .Include(a => a.Student).ThenInclude(s => s.AppUser)
            .Include(a => a.Employer).ThenInclude(e => e.AppUser);
    }

    private List<ApplicationListItemModel> ConvertToListModel(List<Application> applications)
    {
        List<ApplicationListItemModel> list = new List<ApplicationListItemModel>();

        foreach (Application application in applications)
        {
            ApplicationListItemModel model = new ApplicationListItemModel();
            model.Id = application.Id;
            model.JobId = application.JobId;
            model.JobTitle = application.Job.Title;
            model.CompanyName = application.Job.CompanyName ?? application.Employer.CompanyName;
            model.Location = application.Job.Location;
            model.Salary = application.Job.Salary;
            model.JobType = application.Job.JobType;
            model.Industry = application.Job.Industry;
            model.StudentName = application.Student.AppUser.FullName;
            model.StudentEmail = application.Student.AppUser.Email;
            model.EmployerName = application.Employer.AppUser.FullName;
            model.EmployerEmail = application.Employer.AppUser.Email;
            model.ApplicationDate = application.ApplicationDate;
            model.ExpectedStartDate = application.ExpectedStartDate;
            model.Status = application.Status;
            model.CoverLetter = application.CoverLetter;
            model.CvFilePath = application.CvFilePath;

            list.Add(model);
        }

        return list;
    }
}

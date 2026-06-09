using Job4Students.Data;
using Job4Students.Entities;
using Job4Students.Shared;
using Job4Students.Models;
using Microsoft.EntityFrameworkCore;

namespace Job4Students.Repos;

public class EmployerRepo(JobStudentDbContext context, CurrentUserHelper currentUserHelper)
{

    public EmployerDashboardModel GetDashboard(int employerUserId)
    {
        EmployerDashboardModel model = new EmployerDashboardModel();

        var jobs = context.Jobs.Where(j => !j.IsDeleted && j.Employer.AppUserId == employerUserId);
        var applications = context.Applications.Where(a => a.Employer.AppUserId == employerUserId);

        model.TotalJobs = jobs.Count();
        model.OpenJobs = jobs.Count(j => j.Status == "open");
        model.TotalApplications = applications.Count();
        model.PendingApplications = applications.Count(a => a.Status == "pending");

        return model;
    }

    public EmployerProfileModel? GetProfile(int employerUserId)
    {
        Employer? employer = context.Employers
            .Include(e => e.AppUser)
            .FirstOrDefault(e => e.AppUserId == employerUserId);

        if (employer == null)
        {
            return null;
        }

        EmployerProfileModel model = new EmployerProfileModel();
        model.UserId = employer.AppUserId;
        model.EmployerId = employer.Id;
        model.FullName = employer.AppUser.FullName;
        model.Email = employer.AppUser.Email;
        model.Phone = employer.AppUser.Phone;
        model.Address = employer.AppUser.Address;
        model.CompanyName = employer.CompanyName;
        model.CompanyFounded = employer.CompanyFounded;
        model.CompanyDescription = employer.CompanyDescription;
        model.IsVerified = employer.IsVerified;

        return model;
    }

    public Result UpdateProfile(int employerUserId, EmployerProfileModel model)
    {
        try
        {
            Employer? employer = context.Employers
                .Include(e => e.AppUser)
                .FirstOrDefault(e => e.AppUserId == employerUserId);

            if (employer == null)
            {
                return Result.Failure(ErrorMessages.NotFound);
            }

            employer.AppUser.FullName = model.FullName;
            employer.AppUser.Phone = model.Phone;
            employer.AppUser.Address = model.Address;
            employer.CompanyName = model.CompanyName;
            employer.CompanyFounded = model.CompanyFounded;
            employer.CompanyDescription = model.CompanyDescription;

            context.SaveChanges();
            return Result.Success(SuccessMessages.Updated);
        }
        catch (Exception ex)
        {
            return Result.Failure(ex.Message);
        }
    }

    public Result ChangeCredentials(int employerUserId, AccountCredentialsModel model)
    {
        try
        {
            var user = context.AppUsers.FirstOrDefault(u => u.Id == employerUserId && u.Role == "employer");
            if (user == null)
            {
                return Result.Failure(ErrorMessages.NotFound);
            }

            string normalizedEmail = model.Email.Trim().ToLowerInvariant();
            bool emailTaken = context.AppUsers.Any(u => u.Id != employerUserId && u.Email.ToLower() == normalizedEmail);
            if (emailTaken)
            {
                return Result.Failure(ErrorMessages.DuplicateAccount);
            }

            user.Email = model.Email.Trim();

            if (!string.IsNullOrWhiteSpace(model.NewPassword))
            {
                user.PasswordHash = model.NewPassword;
            }

            context.SaveChanges();
            return Result.Success(SuccessMessages.Updated);
        }
        catch (Exception ex)
        {
            return Result.Failure(ex.Message);
        }
    }
}

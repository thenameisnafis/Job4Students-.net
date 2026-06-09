using Job4Students.Data;
using Job4Students.Shared;
using Job4Students.Models;
using Job4Students.Entities;
using Microsoft.EntityFrameworkCore;

namespace Job4Students.Repos;

public class AdminRepo(JobStudentDbContext context, CurrentUserHelper currentUserHelper)
{

    public AdminDashboardModel GetDashboard()
    {
        AdminDashboardModel model = new AdminDashboardModel();
        model.TotalUsers = context.AppUsers.Count();
        model.TotalJobs = context.Jobs.Count(j => !j.IsDeleted);
        model.TotalApplications = context.Applications.Count();
        model.PendingApplications = context.Applications.Count(a => a.Status == "pending");
        return model;
    }

    public AdminProfileModel? GetProfile(int adminUserId)
    {
        string defaultPasswordHash = "admin12345";
        AppUser? user = context.AppUsers.FirstOrDefault(u => u.Id == adminUserId && u.Role == "admin");

        if (user == null)
        {
            return null;
        }

        AdminProfileModel model = new AdminProfileModel();
        model.UserId = user.Id;
        model.Username = user.Username;
        model.Email = user.Email;
        model.PasswordDisplay = user.PasswordHash == defaultPasswordHash ? "admin12345" : "Password changed";

        return model;
    }

    public List<UserSummaryModel> GetUsers(string? role = null, string? status = null)
    {
        var query = context.AppUsers.AsQueryable();

        if (!string.IsNullOrWhiteSpace(role))
        {
            query = query.Where(u => u.Role == role);
        }

        if (!string.IsNullOrWhiteSpace(status))
        {
            query = query.Where(u => u.Status == status);
        }

        var users = query.OrderByDescending(u => u.CreatedAt).ToList();
        List<UserSummaryModel> list = new List<UserSummaryModel>();

        foreach (AppUser user in users)
        {
            UserSummaryModel model = new UserSummaryModel();
            model.Id = user.Id;
            model.Username = user.Username;
            model.Email = user.Email;
            model.FullName = user.FullName;
            model.Role = user.Role;
            model.Status = user.Status;
            model.Phone = user.Phone;
            model.Address = user.Address;
            model.CreatedAt = user.CreatedAt;
            list.Add(model);
        }

        return list;
    }

    public Result ChangeCredentials(int adminUserId, AccountCredentialsModel model)
    {
        try
        {
            AppUser? user = context.AppUsers.FirstOrDefault(u => u.Id == adminUserId && u.Role == "admin");
            if (user == null)
            {
                return Result.Failure(ErrorMessages.NotFound);
            }

            string normalizedEmail = model.Email.Trim().ToLowerInvariant();
            bool emailTaken = context.AppUsers.Any(u => u.Id != adminUserId && u.Email.ToLower() == normalizedEmail);
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

    public Result UpdateUserStatus(int userId, string status)
    {
        try
        {
            AppUser? user = context.AppUsers.Find(userId);
            if (user == null)
            {
                return Result.Failure(ErrorMessages.NotFound);
            }

            if (user.Role == "admin")
            {
                user.Status = "active";
                context.SaveChanges();
                return Result.Failure("Admin account must remain active.");
            }

            user.Status = status;
            context.SaveChanges();

            return Result.Success(SuccessMessages.Updated);
        }
        catch (Exception ex)
        {
            return Result.Failure(ex.Message);
        }
    }

    public Result DeleteUser(int userId)
    {
        try
        {
            AppUser? user = context.AppUsers
                .Include(u => u.Student)
                .Include(u => u.Employer)
                .FirstOrDefault(u => u.Id == userId);

            if (user == null)
            {
                return Result.Failure(ErrorMessages.NotFound);
            }

            if (user.Role == "admin")
            {
                return Result.Failure("Admin users cannot be deleted.");
            }

            if (user.Student != null)
            {
                List<Application> studentApplications = context.Applications
                    .Where(a => a.StudentId == user.Student.Id)
                    .ToList();

                context.Applications.RemoveRange(studentApplications);
                context.Students.Remove(user.Student);
            }

            if (user.Employer != null)
            {
                List<int> employerJobIds = context.Jobs
                    .Where(j => j.EmployerId == user.Employer.Id)
                    .Select(j => j.Id)
                    .ToList();

                List<Application> employerApplications = context.Applications
                    .Where(a => a.EmployerId == user.Employer.Id || employerJobIds.Contains(a.JobId))
                    .ToList();

                List<JobImage> employerJobImages = context.JobImages
                    .Where(i => employerJobIds.Contains(i.JobId))
                    .ToList();

                List<Job> employerJobs = context.Jobs
                    .Where(j => j.EmployerId == user.Employer.Id)
                    .ToList();

                context.Applications.RemoveRange(employerApplications);
                context.JobImages.RemoveRange(employerJobImages);
                context.Jobs.RemoveRange(employerJobs);
                context.Employers.Remove(user.Employer);
            }

            context.AppUsers.Remove(user);
            context.SaveChanges();

            return Result.Success(SuccessMessages.Deleted);
        }
        catch (DbUpdateException)
        {
            return Result.Failure("This user could not be deleted because some related records still exist.");
        }
        catch (Exception ex)
        {
            return Result.Failure(ex.Message);
        }
    }
}

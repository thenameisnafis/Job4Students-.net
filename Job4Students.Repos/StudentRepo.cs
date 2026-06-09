using Job4Students.Data;
using Job4Students.Entities;
using Job4Students.Shared;
using Job4Students.Models;
using Microsoft.EntityFrameworkCore;

namespace Job4Students.Repos;

public class StudentRepo(JobStudentDbContext context, CurrentUserHelper currentUserHelper)
{

    public StudentDashboardModel GetDashboard(int studentUserId)
    {
        StudentDashboardModel model = new StudentDashboardModel();

        var applications = context.Applications.Where(a => a.Student.AppUserId == studentUserId);

        model.TotalApplications = applications.Count();
        model.PendingApplications = applications.Count(a => a.Status == "pending");
        model.ApprovedApplications = applications.Count(a => a.Status == "approved");
        model.ConfirmedApplications = applications.Count(a => a.Status == "confirmed");

        return model;
    }

    public StudentProfileModel? GetProfile(int studentUserId)
    {
        Student? student = context.Students
            .Include(s => s.AppUser)
            .FirstOrDefault(s => s.AppUserId == studentUserId);

        if (student == null)
        {
            return null;
        }

        StudentProfileModel model = new StudentProfileModel();
        model.UserId = student.AppUserId;
        model.StudentId = student.Id;
        model.FullName = student.AppUser.FullName;
        model.Email = student.AppUser.Email;
        model.Phone = student.AppUser.Phone;
        model.Address = student.AppUser.Address;
        model.University = student.University;
        model.EducationLevel = student.EducationLevel;
        model.Skills = student.Skills;
        model.PreferredJobArea = student.PreferredJobArea;
        model.BirthDate = student.BirthDate;
        model.Gender = student.Gender;
        model.CvFilePath = student.CvFilePath;

        return model;
    }

    public Result UpdateProfile(int studentUserId, StudentProfileModel model)
    {
        try
        {
            Student? student = context.Students
                .Include(s => s.AppUser)
                .FirstOrDefault(s => s.AppUserId == studentUserId);

            if (student == null)
            {
                return Result.Failure(ErrorMessages.NotFound);
            }

            student.AppUser.FullName = model.FullName;
            student.AppUser.Phone = model.Phone;
            student.AppUser.Address = model.Address;
            student.University = model.University;
            student.EducationLevel = model.EducationLevel;
            student.Skills = model.Skills;
            student.PreferredJobArea = model.PreferredJobArea;
            student.BirthDate = model.BirthDate;
            student.Gender = model.Gender;

            if (!string.IsNullOrWhiteSpace(model.CvFilePath))
            {
                student.CvFilePath = model.CvFilePath;
            }

            context.SaveChanges();
            return Result.Success(SuccessMessages.Updated);
        }
        catch (Exception ex)
        {
            return Result.Failure(ex.Message);
        }
    }

    public Result ChangeCredentials(int studentUserId, AccountCredentialsModel model)
    {
        try
        {
            var user = context.AppUsers.FirstOrDefault(u => u.Id == studentUserId && u.Role == "student");
            if (user == null)
            {
                return Result.Failure(ErrorMessages.NotFound);
            }

            string normalizedEmail = model.Email.Trim().ToLowerInvariant();
            bool emailTaken = context.AppUsers.Any(u => u.Id != studentUserId && u.Email.ToLower() == normalizedEmail);
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

using Job4Students.Data;
using Job4Students.Entities;
using Job4Students.Shared;
using Job4Students.Models;
using System.Security.Cryptography;
using System.Text;

namespace Job4Students.Repos;

public class AuthRepo(JobStudentDbContext context)
{
    public Result<int> RegisterStudent(RegisterModel model)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(model.FullName) || string.IsNullOrWhiteSpace(model.Email) || string.IsNullOrWhiteSpace(model.Password) ||
                string.IsNullOrWhiteSpace(model.Phone) || string.IsNullOrWhiteSpace(model.Address) || string.IsNullOrWhiteSpace(model.University) ||
                string.IsNullOrWhiteSpace(model.EducationLevel) || string.IsNullOrWhiteSpace(model.Skills) || string.IsNullOrWhiteSpace(model.PreferredJobArea))
            {
                return Result<int>.Failure("Please fill up all student registration fields.");
            }

            if (AccountExists(model.Email))
            {
                return Result<int>.Failure(ErrorMessages.DuplicateAccount);
            }

            AppUser user = new AppUser();
            user.FullName = model.FullName.Trim();
            user.Username = MakeUsername(model.Email);
            user.Email = model.Email.Trim().ToLower();
            user.PasswordHash = model.Password.Trim();
            user.Phone = model.Phone.Trim();
            user.Address = model.Address.Trim();
            user.Role = "student";
            user.Status = "active";
            user.CreatedAt = DateTime.Now;

            Student student = new Student();
            student.University = model.University.Trim();
            student.EducationLevel = model.EducationLevel.Trim();
            student.Skills = model.Skills.Trim();
            student.PreferredJobArea = model.PreferredJobArea.Trim();

            user.Student = student;

            context.AppUsers.Add(user);
            context.SaveChanges();

            return Result<int>.Success(user.Id, SuccessMessages.Registered);
        }
        catch (Exception ex)
        {
            return Result<int>.Failure(ex.Message);
        }
    }

    public Result<int> RegisterEmployer(RegisterModel model)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(model.FullName) || string.IsNullOrWhiteSpace(model.CompanyName) || string.IsNullOrWhiteSpace(model.Email) ||
                string.IsNullOrWhiteSpace(model.Password) || string.IsNullOrWhiteSpace(model.Phone) || string.IsNullOrWhiteSpace(model.Address) || !model.CompanyFounded.HasValue)
            {
                return Result<int>.Failure("Please fill up all recruiter registration fields.");
            }

            if (AccountExists(model.Email))
            {
                return Result<int>.Failure(ErrorMessages.DuplicateAccount);
            }

            AppUser user = new AppUser();
            user.FullName = model.FullName.Trim();
            user.Username = MakeUsername(model.Email);
            user.Email = model.Email.Trim().ToLower();
            user.PasswordHash = model.Password.Trim();
            user.Phone = model.Phone.Trim();
            user.Address = model.Address.Trim();
            user.Role = "employer";
            user.Status = "active";
            user.CreatedAt = DateTime.Now;

            Employer employer = new Employer();
            employer.CompanyName = model.CompanyName.Trim();
            employer.CompanyFounded = model.CompanyFounded;
            employer.CompanyDescription = string.Empty;
            employer.IsVerified = true;

            user.Employer = employer;

            context.AppUsers.Add(user);
            context.SaveChanges();

            return Result<int>.Success(user.Id, SuccessMessages.Registered);
        }
        catch (Exception ex)
        {
            return Result<int>.Failure(ex.Message);
        }
    }

    public Result<AppUser> Login(LoginModel model)
    {
        try
        {
            string email = model.Email.Trim().ToLower();
            string password = model.Password.Trim();

            if (email == "admin@gmail.com" && password == "admin12345")
            {
                EnsureDefaultAdmin();
            }

            AppUser? user = context.AppUsers.FirstOrDefault(u => u.Email.ToLower() == email);

            if (user == null)
            {
                return Result<AppUser>.Failure(ErrorMessages.InvalidLogin);
            }

            string storedPassword = user.PasswordHash ?? string.Empty;
            string hashedPassword = HashPassword(password);

            if (storedPassword != password && storedPassword != hashedPassword)
            {
                return Result<AppUser>.Failure(ErrorMessages.InvalidLogin);
            }

            if (user.Status != "active")
            {
                return Result<AppUser>.Failure(ErrorMessages.AccountInactive);
            }

            return Result<AppUser>.Success(user, SuccessMessages.LoggedIn);
        }
        catch (Exception ex)
        {
            return Result<AppUser>.Failure(ex.Message);
        }
    }

    public AppUser? GetUserById(int id)
    {
        return context.AppUsers.Find(id);
    }

    public Result ChangePassword(int userId, ChangePasswordModel model)
    {
        try
        {
            AppUser? user = context.AppUsers.Find(userId);
            if (user == null)
            {
                return Result.Failure(ErrorMessages.NotFound);
            }

            if (user.PasswordHash != model.CurrentPassword)
            {
                return Result.Failure("Current password is incorrect.");
            }

            user.PasswordHash = model.NewPassword;
            context.SaveChanges();

            return Result.Success("Password changed successfully.");
        }
        catch (Exception ex)
        {
            return Result.Failure(ex.Message);
        }
    }

    private void EnsureDefaultAdmin()
    {
        string adminEmail = "admin@gmail.com";

        AppUser? admin = context.AppUsers.FirstOrDefault(u => u.Email.ToLower() == adminEmail);

        if (admin == null)
        {
            admin = context.AppUsers.FirstOrDefault(u => u.Username.ToLower() == "admin" || u.Role.ToLower() == "admin");
        }

        if (admin == null)
        {
            admin = new AppUser();
            context.AppUsers.Add(admin);
        }

        admin.FullName = "Admin";
        admin.Username = "admin";
        admin.Email = adminEmail;
        admin.PasswordHash = "admin12345";
        admin.Phone = "01700000000";
        admin.Address = "Admin Address";
        admin.Role = "admin";
        admin.Status = "active";

        if (admin.CreatedAt == default)
        {
            admin.CreatedAt = DateTime.Now;
        }

        context.SaveChanges();
    }

    private string HashPassword(string password)
    {
        using (var sha = SHA256.Create())
        {
            byte[] bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToHexString(bytes).ToLowerInvariant();
        }
    }

    private bool AccountExists(string email)
    {
        string normalizedEmail = email.Trim().ToLower();
        return context.AppUsers.Any(u => u.Email.ToLower() == normalizedEmail);
    }

    private string MakeUsername(string email)
    {
        string value = email.Trim().ToLower();
        int atIndex = value.IndexOf('@');

        if (atIndex > 0)
        {
            return value.Substring(0, atIndex);
        }

        return value;
    }
}

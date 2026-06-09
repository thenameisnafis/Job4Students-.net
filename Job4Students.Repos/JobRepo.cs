using Job4Students.Data;
using Job4Students.Entities;
using Job4Students.Shared;
using Job4Students.Models;
using Microsoft.EntityFrameworkCore;

namespace Job4Students.Repos;

public class JobRepo(JobStudentDbContext context, CurrentUserHelper currentUserHelper)
{

    public List<JobListItemModel> GetOpenJobs(JobFilterModel? filter = null)
    {
        var query = context.Jobs
            .Include(j => j.Employer).ThenInclude(e => e.AppUser)
            .Include(j => j.Images)
            .Include(j => j.Applications)
            .Where(j => !j.IsDeleted && j.Status == "open" && j.ApplicationDeadline.Date >= DateTime.Today);

        query = ApplyFilter(query, filter);

        var jobs = query.OrderByDescending(j => j.CreatedAt).ToList();
        return ConvertToListModel(jobs);
    }

    public List<JobListItemModel> GetEmployerJobs(int employerUserId)
    {
        var jobs = context.Jobs
            .Include(j => j.Employer).ThenInclude(e => e.AppUser)
            .Include(j => j.Images)
            .Include(j => j.Applications)
            .Where(j => !j.IsDeleted && j.Employer.AppUserId == employerUserId)
            .OrderByDescending(j => j.CreatedAt)
            .ToList();

        return ConvertToListModel(jobs);
    }

    public List<JobListItemModel> GetAllJobsForAdmin()
    {
        var jobs = context.Jobs
            .Include(j => j.Employer).ThenInclude(e => e.AppUser)
            .Include(j => j.Images)
            .Include(j => j.Applications)
            .Where(j => !j.IsDeleted)
            .OrderByDescending(j => j.CreatedAt)
            .ToList();

        return ConvertToListModel(jobs);
    }

    public JobDetailsModel? GetDetails(int id)
    {
        Job? job = context.Jobs
            .Include(j => j.Employer).ThenInclude(e => e.AppUser)
            .Include(j => j.Images)
            .Include(j => j.Applications)
            .FirstOrDefault(j => !j.IsDeleted && j.Id == id);

        if (job == null)
        {
            return null;
        }

        JobDetailsModel model = new JobDetailsModel();
        model.Id = job.Id;
        model.EmployerId = job.EmployerId;
        model.EmployerUserId = job.Employer.AppUserId;
        model.Title = job.Title;
        model.Description = job.Description;
        model.CompanyName = job.CompanyName ?? job.Employer.CompanyName;
        model.RecruiterName = job.Employer.AppUser.FullName;
        model.RecruiterEmail = job.Employer.AppUser.Email;
        model.JobType = job.JobType;
        model.Industry = job.Industry;
        model.Location = job.Location;
        model.Salary = job.Salary;
        model.Requirements = job.Requirements;
        model.ApplicationDeadline = job.ApplicationDeadline;
        model.Status = job.Status;
        model.CreatedAt = job.CreatedAt;
        model.ApplicationCount = job.Applications.Count;

        JobImage? primaryImage = job.Images.FirstOrDefault(i => i.IsPrimary);
        if (primaryImage != null)
        {
            model.PrimaryImagePath = primaryImage.ImagePath;
        }

        model.ImagePaths = job.Images.Select(i => i.ImagePath).ToList();

        return model;
    }

    public Result<int> Create(int employerUserId, JobCreateModel model)
    {
        try
        {
            Employer? employer = context.Employers
                .Include(e => e.AppUser)
                .FirstOrDefault(e => e.AppUserId == employerUserId);

            if (employer == null)
            {
                return Result<int>.Failure(ErrorMessages.Unauthorized);
            }

            Job job = new Job();
            job.EmployerId = employer.Id;
            job.Title = model.Title;
            job.Description = model.Description;
            job.JobType = model.JobType;
            job.Industry = model.Industry;
            job.Location = model.Location;
            job.CompanyName = string.IsNullOrWhiteSpace(model.CompanyName) ? employer.CompanyName : model.CompanyName;
            job.Salary = model.Salary;
            job.Requirements = model.Requirements;
            job.ApplicationDeadline = model.ApplicationDeadline;
            job.Status = "open";

            for (int i = 0; i < model.ImagePaths.Count; i++)
            {
                JobImage image = new JobImage();
                image.ImagePath = model.ImagePaths[i];
                image.IsPrimary = i == 0;
                job.Images.Add(image);
            }

            context.Jobs.Add(job);
            context.SaveChanges();

            return Result<int>.Success(job.Id, SuccessMessages.Saved);
        }
        catch (Exception ex)
        {
            return Result<int>.Failure(ex.Message);
        }
    }

    public Result Update(int jobId, int employerUserId, JobUpdateModel model)
    {
        try
        {
            Job? job = context.Jobs
                .Include(j => j.Employer)
                .FirstOrDefault(j => j.Id == jobId && !j.IsDeleted);

            if (job == null)
            {
                return Result.Failure(ErrorMessages.NotFound);
            }

            if (job.Employer.AppUserId != employerUserId)
            {
                return Result.Failure(ErrorMessages.Unauthorized);
            }

            job.Title = model.Title;
            job.Description = model.Description;
            job.JobType = model.JobType;
            job.Industry = model.Industry;
            job.Location = model.Location;
            job.CompanyName = model.CompanyName;
            job.Salary = model.Salary;
            job.Requirements = model.Requirements;
            job.ApplicationDeadline = model.ApplicationDeadline;
            job.Status = model.Status;
            job.UpdatedAt = DateTime.Now;

            context.SaveChanges();
            return Result.Success(SuccessMessages.Updated);
        }
        catch (Exception ex)
        {
            return Result.Failure(ex.Message);
        }
    }

    public Result Delete(int jobId, int employerUserId)
    {
        try
        {
            Job? job = context.Jobs
                .Include(j => j.Employer)
                .FirstOrDefault(j => j.Id == jobId && !j.IsDeleted);

            if (job == null)
            {
                return Result.Failure(ErrorMessages.NotFound);
            }

            if (job.Employer.AppUserId != employerUserId)
            {
                return Result.Failure(ErrorMessages.Unauthorized);
            }

            job.IsDeleted = true;
            job.Status = "closed";
            job.UpdatedAt = DateTime.Now;

            context.SaveChanges();
            return Result.Success(SuccessMessages.Deleted);
        }
        catch (Exception ex)
        {
            return Result.Failure(ex.Message);
        }
    }

    public Result ChangeStatus(int jobId, string status)
    {
        try
        {
            Job? job = context.Jobs.FirstOrDefault(j => j.Id == jobId && !j.IsDeleted);
            if (job == null)
            {
                return Result.Failure(ErrorMessages.NotFound);
            }

            job.Status = status;
            job.UpdatedAt = DateTime.Now;

            context.SaveChanges();
            return Result.Success(SuccessMessages.Updated);
        }
        catch (Exception ex)
        {
            return Result.Failure(ex.Message);
        }
    }

    private IQueryable<Job> ApplyFilter(IQueryable<Job> query, JobFilterModel? filter)
    {
        if (filter == null)
        {
            return query;
        }

        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            string search = filter.Search.Trim().ToLower();
            query = query.Where(j => j.Title.ToLower().Contains(search) ||
                                     j.Description.ToLower().Contains(search) ||
                                     (j.CompanyName ?? j.Employer.CompanyName).ToLower().Contains(search));
        }

        if (!string.IsNullOrWhiteSpace(filter.JobType))
        {
            query = query.Where(j => j.JobType == filter.JobType);
        }

        if (!string.IsNullOrWhiteSpace(filter.Industry))
        {
            query = query.Where(j => j.Industry != null && j.Industry.Contains(filter.Industry));
        }

        if (!string.IsNullOrWhiteSpace(filter.Location))
        {
            query = query.Where(j => j.Location != null && j.Location.Contains(filter.Location));
        }

        if (filter.MinSalary.HasValue)
        {
            query = query.Where(j => j.Salary >= filter.MinSalary.Value);
        }

        if (filter.MaxSalary.HasValue)
        {
            query = query.Where(j => j.Salary <= filter.MaxSalary.Value);
        }

        return query;
    }

    private List<JobListItemModel> ConvertToListModel(List<Job> jobs)
    {
        List<JobListItemModel> list = new List<JobListItemModel>();

        foreach (Job job in jobs)
        {
            JobListItemModel model = new JobListItemModel();
            model.Id = job.Id;
            model.Title = job.Title;
            model.CompanyName = job.CompanyName ?? job.Employer.CompanyName;
            model.RecruiterName = job.Employer.AppUser.FullName;
            model.JobType = job.JobType;
            model.Industry = job.Industry;
            model.Location = job.Location;
            model.Salary = job.Salary;
            model.ApplicationDeadline = job.ApplicationDeadline;
            model.Status = job.Status;
            model.ApplicationCount = job.Applications.Count;

            JobImage? primaryImage = job.Images.FirstOrDefault(i => i.IsPrimary);
            if (primaryImage != null)
            {
                model.PrimaryImagePath = primaryImage.ImagePath;
            }

            list.Add(model);
        }

        return list;
    }
}

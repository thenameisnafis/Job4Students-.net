using System.ComponentModel.DataAnnotations;

namespace Job4Students.Models;

public class JobFilterModel
{
    public string? Search { get; set; }
    public string? JobType { get; set; }
    public string? Industry { get; set; }
    public string? Location { get; set; }
    public decimal? MinSalary { get; set; }
    public decimal? MaxSalary { get; set; }
}

public class JobCreateModel
{
    [Required, MaxLength(180)] public string Title { get; set; } = string.Empty;
    [Required, MaxLength(4000)] public string Description { get; set; } = string.Empty;
    [Required, MaxLength(40)] public string JobType { get; set; } = "part-time";
    [MaxLength(120)] public string? Industry { get; set; }
    [MaxLength(160)] public string? Location { get; set; }
    [MaxLength(180)] public string? CompanyName { get; set; }
    public decimal? Salary { get; set; }
    [MaxLength(4000)] public string? Requirements { get; set; }
    [Required] public DateTime ApplicationDeadline { get; set; } = DateTime.Today.AddDays(30);
    public List<string> ImagePaths { get; set; } = new List<string>();
}

public class JobUpdateModel
{
    public int Id { get; set; }
    [Required, MaxLength(180)] public string Title { get; set; } = string.Empty;
    [Required, MaxLength(4000)] public string Description { get; set; } = string.Empty;
    [Required, MaxLength(40)] public string JobType { get; set; } = "part-time";
    [MaxLength(120)] public string? Industry { get; set; }
    [MaxLength(160)] public string? Location { get; set; }
    [MaxLength(180)] public string? CompanyName { get; set; }
    public decimal? Salary { get; set; }
    [MaxLength(4000)] public string? Requirements { get; set; }
    [Required] public DateTime ApplicationDeadline { get; set; } = DateTime.Today.AddDays(30);
    public string Status { get; set; } = "open";
}

public class JobListItemModel
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string CompanyName { get; set; } = string.Empty;
    public string? RecruiterName { get; set; }
    public string JobType { get; set; } = string.Empty;
    public string? Industry { get; set; }
    public string? Location { get; set; }
    public decimal? Salary { get; set; }
    public DateTime ApplicationDeadline { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? PrimaryImagePath { get; set; }
    public int ApplicationCount { get; set; }
}

public class JobDetailsModel
{
    public int Id { get; set; }
    public int EmployerId { get; set; }
    public int EmployerUserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string CompanyName { get; set; } = string.Empty;
    public string? RecruiterName { get; set; }
    public string? RecruiterEmail { get; set; }
    public string JobType { get; set; } = string.Empty;
    public string? Industry { get; set; }
    public string? Location { get; set; }
    public decimal? Salary { get; set; }
    public string? Requirements { get; set; }
    public DateTime ApplicationDeadline { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public string? PrimaryImagePath { get; set; }
    public int ApplicationCount { get; set; }
    public List<string> ImagePaths { get; set; } = new List<string>();
}

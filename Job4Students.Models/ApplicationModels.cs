using System.ComponentModel.DataAnnotations;

namespace Job4Students.Models;

public class ApplicationCreateModel
{
    [Required] public int JobId { get; set; }
    public DateTime? ExpectedStartDate { get; set; }
    [Required, MaxLength(4000)] public string CoverLetter { get; set; } = string.Empty;
    public string? CvFilePath { get; set; }
}

public class ApplicationListItemModel
{
    public int Id { get; set; }
    public int JobId { get; set; }
    public string JobTitle { get; set; } = string.Empty;
    public string CompanyName { get; set; } = string.Empty;
    public string? Location { get; set; }
    public decimal? Salary { get; set; }
    public string JobType { get; set; } = string.Empty;
    public string? Industry { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public string StudentEmail { get; set; } = string.Empty;
    public string EmployerName { get; set; } = string.Empty;
    public string EmployerEmail { get; set; } = string.Empty;
    public DateTime ApplicationDate { get; set; }
    public DateTime? ExpectedStartDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? CoverLetter { get; set; }
    public string? CvFilePath { get; set; }
}

public class ApplicationStatusUpdateModel
{
    [Required] public int ApplicationId { get; set; }
    [Required] public string Status { get; set; } = string.Empty;
}

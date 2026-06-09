using System.ComponentModel.DataAnnotations;

namespace Job4Students.Models;

public class StudentProfileModel
{
    public int UserId { get; set; }
    public int StudentId { get; set; }
    [Required, MaxLength(120)] public string FullName { get; set; } = string.Empty;
    [EmailAddress, MaxLength(160)] public string Email { get; set; } = string.Empty;
    [MaxLength(40)] public string? Phone { get; set; }
    [MaxLength(300)] public string? Address { get; set; }
    [MaxLength(160)] public string? University { get; set; }
    [MaxLength(80)] public string? EducationLevel { get; set; }
    [MaxLength(700)] public string? Skills { get; set; }
    [MaxLength(160)] public string? PreferredJobArea { get; set; }
    public DateTime? BirthDate { get; set; }
    [MaxLength(40)] public string? Gender { get; set; }
    public string? CvFilePath { get; set; }
}

public class EmployerProfileModel
{
    public int UserId { get; set; }
    public int EmployerId { get; set; }
    [Required, MaxLength(120)] public string FullName { get; set; } = string.Empty;
    [EmailAddress, MaxLength(160)] public string Email { get; set; } = string.Empty;
    [Required, MaxLength(180)] public string CompanyName { get; set; } = string.Empty;
    [MaxLength(40)] public string? Phone { get; set; }
    [MaxLength(300)] public string? Address { get; set; }
    public DateTime? CompanyFounded { get; set; }
    [MaxLength(1200)] public string? CompanyDescription { get; set; }
    public bool IsVerified { get; set; }
}

public class AdminProfileModel
{
    public int UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordDisplay { get; set; } = string.Empty;
}

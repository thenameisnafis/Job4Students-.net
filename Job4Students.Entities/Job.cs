using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Job4Students.Entities;

[Table("Jobs")]
public class Job
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public int EmployerId { get; set; }
    public Employer Employer { get; set; } = null!;

    [Required, MaxLength(180)]
    public string Title { get; set; } = string.Empty;

    [Required, MaxLength(4000)]
    public string Description { get; set; } = string.Empty;

    [Required, MaxLength(40)]
    public string JobType { get; set; } = "part-time";

    [MaxLength(120)]
    public string? Industry { get; set; }

    [MaxLength(160)]
    public string? Location { get; set; }

    [MaxLength(180)]
    public string? CompanyName { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal? Salary { get; set; }

    [MaxLength(4000)]
    public string? Requirements { get; set; }

    public DateTime ApplicationDeadline { get; set; }

    [Required, MaxLength(30)]
    public string Status { get; set; } = "open";

    public bool IsDeleted { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime? UpdatedAt { get; set; }

    public ICollection<JobImage> Images { get; set; } = new List<JobImage>();
    public ICollection<Application> Applications { get; set; } = new List<Application>();
}

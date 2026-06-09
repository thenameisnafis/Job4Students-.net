using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Job4Students.Entities;

[Table("Applications")]
public class Application
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public int StudentId { get; set; }
    public Student Student { get; set; } = null!;

    public int EmployerId { get; set; }
    public Employer Employer { get; set; } = null!;

    public int JobId { get; set; }
    public Job Job { get; set; } = null!;

    public DateTime ApplicationDate { get; set; } = DateTime.Now;
    public DateTime? ExpectedStartDate { get; set; }

    [Required, MaxLength(30)]
    public string Status { get; set; } = "pending";

    [MaxLength(4000)]
    public string? CoverLetter { get; set; }

    [MaxLength(300)]
    public string? CvFilePath { get; set; }
}

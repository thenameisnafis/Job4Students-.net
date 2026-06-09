using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Job4Students.Entities;

[Table("Employers")]
public class Employer
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public int AppUserId { get; set; }
    public AppUser AppUser { get; set; } = null!;

    [Required, MaxLength(180)]
    public string CompanyName { get; set; } = string.Empty;

    public DateTime? CompanyFounded { get; set; }

    [MaxLength(1200)]
    public string? CompanyDescription { get; set; }

    public bool IsVerified { get; set; }

    public ICollection<Job> Jobs { get; set; } = new List<Job>();
    public ICollection<Application> Applications { get; set; } = new List<Application>();
}

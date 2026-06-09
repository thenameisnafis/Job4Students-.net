using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Job4Students.Entities;

[Table("Students")]
public class Student
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public int AppUserId { get; set; }
    public AppUser AppUser { get; set; } = null!;

    [MaxLength(160)]
    public string? University { get; set; }

    [MaxLength(80)]
    public string? EducationLevel { get; set; }

    [MaxLength(700)]
    public string? Skills { get; set; }

    [MaxLength(160)]
    public string? PreferredJobArea { get; set; }

    public DateTime? BirthDate { get; set; }

    [MaxLength(40)]
    public string? Gender { get; set; }

    [MaxLength(300)]
    public string? CvFilePath { get; set; }

    public ICollection<Application> Applications { get; set; } = new List<Application>();
}

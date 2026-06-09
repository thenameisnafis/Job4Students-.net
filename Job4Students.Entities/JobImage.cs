using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Job4Students.Entities;

[Table("JobImages")]
public class JobImage
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public int JobId { get; set; }
    public Job Job { get; set; } = null!;

    [Required, MaxLength(300)]
    public string ImagePath { get; set; } = string.Empty;

    public bool IsPrimary { get; set; }
}

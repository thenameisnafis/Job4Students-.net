using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Job4Students.Entities;

[Table("AppUsers")]
public class AppUser
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required, MaxLength(80)]
    public string Username { get; set; } = string.Empty;

    [Required, MaxLength(160), EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string PasswordHash { get; set; } = string.Empty;

    [Required, MaxLength(120)]
    public string FullName { get; set; } = string.Empty;

    [MaxLength(40)]
    public string? Phone { get; set; }

    [MaxLength(300)]
    public string? Address { get; set; }

    [Required, MaxLength(30)]
    public string Role { get; set; } = string.Empty;

    [Required, MaxLength(30)]
    public string Status { get; set; } = "active";

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public Student? Student { get; set; }
    public Employer? Employer { get; set; }
}

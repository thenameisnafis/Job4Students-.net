using Job4Students.Entities;
using Microsoft.EntityFrameworkCore;

namespace Job4Students.Data;

public class JobStudentDbContext(DbContextOptions<JobStudentDbContext> options) : DbContext(options)
{
    public DbSet<AppUser> AppUsers { get; set; }
    public DbSet<Student> Students { get; set; }
    public DbSet<Employer> Employers { get; set; }
    public DbSet<Job> Jobs { get; set; }
    public DbSet<JobImage> JobImages { get; set; }
    public DbSet<Application> Applications { get; set; }
}

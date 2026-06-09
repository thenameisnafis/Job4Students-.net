namespace Job4Students.Models;

public class StudentDashboardModel
{
    public int TotalApplications { get; set; }
    public int PendingApplications { get; set; }
    public int ApprovedApplications { get; set; }
    public int ConfirmedApplications { get; set; }
}

public class EmployerDashboardModel
{
    public int TotalJobs { get; set; }
    public int OpenJobs { get; set; }
    public int TotalApplications { get; set; }
    public int PendingApplications { get; set; }
}

public class AdminDashboardModel
{
    public int TotalUsers { get; set; }
    public int TotalJobs { get; set; }
    public int TotalApplications { get; set; }
    public int PendingApplications { get; set; }
}

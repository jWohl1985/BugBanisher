namespace BugBanisher.Models.ViewModels;

public class DashboardViewModel
{
    public List<Project> ActiveProjects { get; set; } = default!;
    public List<Ticket> OpenTickets { get; set; } = default!;
    public List<Ticket> CompletedTickets { get; set; } = default!;
    public List<AppUser> Employees { get; set; } = default!;
    public List<Notification> Notifications { get; set; } = default!;
    public int AdminCount { get; set; }
    public int ProjectManagerCount { get; set; }
    public int DeveloperCount { get; set; }
    public int MemberCount { get; set; }
}

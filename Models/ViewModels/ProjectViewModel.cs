namespace BugBanisher.Models.ViewModels;

public class ProjectViewModel
{
    public Project Project { get; set; } = default!;
    public AppUser? ProjectManager { get; set; }
    public List<AppUser> Developers { get; set; } = default!;
    public List<AppUser> Members { get; set; } = default!;
    public IEnumerable<Ticket> Tickets { get; set; } = default!;

    public string GetDeveloperTextFormat(Ticket ticket) => ticket.DeveloperId is not null ? "" : "fw-bold text-danger";
    public string GetPriorityBadgeFormat(Ticket ticket) => ticket.Priority!.Id switch
    {
        "veryLow" => "bg-success",
        "low" => "bg-success",
        "medium" => "bg-primary",
        "high" => "bg-warning",
        "veryHigh" => "bg-danger",
        _ => "bg-danger",
    };
    public string GetStatusBadgeFormat(Ticket ticket) => ticket.Status!.Id switch
    {
        "unassigned" => "bg-danger",
        "pending" => "bg-warning",
        "development" => "bg-primary",
        "hold" => "bg-danger",
        "complete" => "bg-success",
        _ => "bg-danger",
    };

}

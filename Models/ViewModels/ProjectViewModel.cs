namespace BugBanisher.Models.ViewModels;

public class ProjectViewModel
{
    public Project Project { get; set; } = default!;
    public AppUser? ProjectManager { get; set; }
    public List<AppUser> Developers { get; set; } = default!;
    public List<AppUser> Members { get; set; } = default!;
    public IEnumerable<Ticket> Tickets { get; set; } = default!;

    public string GetDeveloperTextFormat(Ticket ticket) => ticket.DeveloperId is not null ? "" : "text-danger";
    public string GetPriorityBadgeFormat(Ticket ticket) => ticket.Priority!.Id switch
    {
        "low" => "bg-success",
        "medium" => "bg-primary",
        "high" => "bg-danger",
        _ => "bg-danger",
    };

    public string GetStatusTextFormat(Ticket ticket) => ticket.Status!.Id switch
    {
        "unassigned" => "text-danger",
        "pending" => "text-dark",
        "development" => "text-dark",
        "hold" => "text-danger",
        "complete" => "text-success",
        _ => "text-danger",
    };

}

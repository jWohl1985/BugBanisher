using Microsoft.AspNetCore.Mvc.Rendering;

namespace BugBanisher.Models.ViewModels;

public class TicketListViewModel
{
    public string TicketListType { get; set; } = default!;
    public List<Ticket> Tickets { get; set; } = default!;
    public string PageNumber { get; set; } = default!;
    public string PerPage { get; set; } = default!;
    public string SortBy { get; set; } = default!;


    public SelectList SortByOptions { get; set; } = default!;
    public SelectList PerPageOptions { get; set; } = default!;

    public string GetPriorityFormatting(string priorityId) => priorityId switch
    {
        "low" => "badge bg-success",
        "medium" => "badge bg-info",
        "high" => "badge bg-danger",
        _ => "",
    };

    public string GetStatusFormatting(string statusId) => statusId switch
    {
        "unassigned" => "text-danger",
        "hold" => "text-danger",
        "development" => "text-dark",
        "pending" => "text-warning",
        "complete" => "text-success",
        _ => "",
    };
}

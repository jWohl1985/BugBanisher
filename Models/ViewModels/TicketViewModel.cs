using System.Drawing;

namespace BugBanisher.Models.ViewModels;

public class TicketViewModel
{
	public Ticket Ticket { get; set; } = default!;
	public Project Project { get; set; } = default!;
	public AppUser? ProjectManager { get; set; }
	public AppUser? Developer { get; set; }
	public string NewComment { get; set; } = String.Empty;
	public IFormFile? NewAttachment { get; set; }
	public string? FileDescription { get; set; }

	public string StatusBadgeType => Ticket.Status is null ? "" : Ticket.Status.Id switch
	{
		"unassigned" => "bg-danger",
		"pending" => "bg-warning",
		"development" => "bg-primary",
		"hold" => "bg-danger",
		"complete" => "bg-success",
		_ => "bg-danger"
	};

	public string PriorityBadgeType => Ticket.Priority is null ? "" : Ticket.Priority!.Id switch
	{
		"veryLow" => "bg-success",
		"low" => "bg-success",
		"medium" => "bg-primary",
		"high" => "bg-warning",
		"veryHigh" => "bg-danger",
		_ => "bg-danger",
	};

	
}

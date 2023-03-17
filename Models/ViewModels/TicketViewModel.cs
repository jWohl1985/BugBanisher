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

	public string StatusTextFormat => Ticket.Status is null ? "" : Ticket.Status.Id switch
	{
		"unassigned" => "fw-bold text-danger",
		"pending" => "fw-bold text-danger",
		"development" => "fw-bold text-dark",
		"hold" => "fw-bold text-danger",
		"complete" => "fw-bold text-success",
		_ => "text-danger"
	};

	public string PriorityBadgeType => Ticket.Priority is null ? "" : Ticket.Priority!.Id switch
	{
		"low" => "bg-success",
		"medium" => "bg-primary",
		"high" => "bg-danger",
		_ => "bg-danger",
	};

	
}

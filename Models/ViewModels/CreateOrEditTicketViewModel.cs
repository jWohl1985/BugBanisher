using Microsoft.AspNetCore.Mvc.Rendering;

namespace BugBanisher.Models.ViewModels;

public class CreateOrEditTicketViewModel
{
	public Ticket Ticket { get; set; } = default!;
	public string Title { get; set; } = String.Empty;
	public string Description { get; set; } = String.Empty;

	public SelectList Priorities { get; set; } = default!;
	public SelectList Statuses { get; set; } = default!;
	public SelectList Types { get; set; } = default!;
	public SelectList Developers { get; set; } = default!;

	public string? SelectedPriority { get; set; }
	public string? SelectedStatus { get; set; }
	public string? SelectedType { get; set; }
	public string? SelectedDeveloper { get; set; }
	public bool IsStatusDropdownEnabled { get; set; }
}

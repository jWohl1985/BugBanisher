using Microsoft.AspNetCore.Mvc.Rendering;

namespace BugBanisher.Models.ViewModels;

public class EditEmployeeViewModel
{
    public AppUser Employee { get; set; } = default!;
    public string JobTitle { get; set; } = default!;
    public SelectList AvailableRoles { get; set; } = default!;
    public string? SelectedRole { get; set; }
}

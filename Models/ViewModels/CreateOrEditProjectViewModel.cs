using Microsoft.AspNetCore.Mvc.Rendering;

namespace BugBanisher.Models.ViewModels;

public class CreateOrEditProjectViewModel
{
    public Project Project { get; set; } = default!;
    public IFormFile? Image { get; set; }
    public DateTime Deadline { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string Description { get; set; } = default!;

    public SelectList ProjectManagers { get; set; } = default!;
    public string SelectedManager { get; set; } = default!;
    public MultiSelectList Developers { get; set; } = default!;
    public IEnumerable<string>? SelectedDevelopers { get; set; } = default!;
    public MultiSelectList Members { get; set; } = default!;
    public IEnumerable<string>? SelectedMembers { get; set; } = default!;
}

using Microsoft.AspNetCore.Mvc.Rendering;

namespace BugBanisher.Models.ViewModels;

public class ProjectListViewModel
{
    public string ActiveOrArchived { get; set; } = default!;
    public List<Project> Projects { get; set; } = default!;
    public string PageNumber { get; set; } = default!;
    public string PerPage { get; set; } = default!;
    public string SortBy { get; set; } = default!;


    public SelectList SortByOptions { get; set; } = default!;
    public SelectList PerPageOptions { get; set; } = default!;
}

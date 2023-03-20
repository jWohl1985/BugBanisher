using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace BugBanisher.Models;

public class AppUser : IdentityUser
{
    public int? CompanyId { get; set; }
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public string FullName => $"{FirstName} {LastName}";
    public string JobTitle { get; set; } = default!;
    public string? About { get; set; }

    [NotMapped]
    public IFormFile? ProfilePicture { get; set; }
    public string? PictureExtension { get; set; }
    public byte[]? PictureData { get; set; }

    public Company? Company { get; set; }
    public ICollection<Project> Projects { get; set; } = new HashSet<Project>();
    public ICollection<Notification> Notifications { get; set; } = new HashSet<Notification>();
}

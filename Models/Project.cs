using BugBanisher.Extensions;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BugBanisher.Models;

public class Project
{
    public int Id { get; set; }
    public int CompanyId { get; set; } = default!;
    public string? ProjectManagerId { get; set; }

    public string Name { get; set; } = default!;
    public string Description { get; set; } = default!;
    public bool IsArchived { get; set; }
    public DateTime Created { get; set; }
    public DateTime Deadline { get; set; }

    [NotMapped]
    [DataType(DataType.Upload)]
    [MaxFileSize(1024 * 1024)]
    [AllowedExtensions(new string[] { ".gif", ".jpg", ".png", ".jpeg" })]
    public IFormFile? PictureFile { get; set; }
    public byte[]? PictureData { get; set; }
    public string? PictureExtension { get; set; }


    public Company? Company { get; set; }
    public ICollection<Ticket> Tickets { get; set; } = new HashSet<Ticket>();
    public ICollection<AppUser> Team { get; set; } = new HashSet<AppUser>();
    public ICollection<Notification> Notifications { get; set; } = new HashSet<Notification>();
}

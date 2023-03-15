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
    public IFormFile? PictureFile { get; set; }
    public byte[] PictureData { get; set; } = new byte[0];
    public string PictureExtension { get; set; } = "image/png";


    public Company? Company { get; set; }
    public ICollection<Ticket> Tickets { get; set; } = new HashSet<Ticket>();
    public ICollection<AppUser> Team { get; set; } = new HashSet<AppUser>();
}

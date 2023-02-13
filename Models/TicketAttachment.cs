using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using BugBanisher.Extensions;

namespace BugBanisher.Models;

public class TicketAttachment
{
    public int Id { get; set; }
    public int TicketId { get; set; }
    public string AppUserId { get; set; } = default!;

    public DateTime Created { get; set; }
    public string? Description { get; set; }

    [NotMapped]
    [DataType(DataType.Upload)]
    [DisplayName("Select a file")]
    [MaxFileSize(1024 * 1024)]
    [AllowedExtensions(new string[] { ".css", ".doc", ".gif", ".html", ".jpg", ".js", ".pdf", ".sql", ".tif", ".txt", ".xml", ".zip", ".cs", ".cshtml", ".json" })]
    public IFormFile FormFile { get; set; } = default!;
    public byte[] FileData { get; set; } = default!;
    public string FileName { get; set; } = default!;
    public string FileType { get; set; } = default!;


    public Ticket? Ticket { get; set; }
    public AppUser? Uploader { get; set; }
}

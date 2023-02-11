using System.ComponentModel;

namespace BugBanisher.Models;

public class TicketComment
{
    public int Id { get; set; }
    public int TicketId { get; set; }
    public string AppUserId { get; set; } = default!;

    public DateTimeOffset Created { get; set; }
    public string Comment { get; set; } = default!;


    public Ticket? Ticket { get; set; }
    public AppUser? User { get; set; }
}

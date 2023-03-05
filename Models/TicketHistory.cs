namespace BugBanisher.Models;

public class TicketHistory
{
    public int Id { get; set; }
    public int TicketId { get; set; }
    public string AppUserId { get; set; } = default!;

    public DateTime Created { get; set; }
    public string Description { get; set; } = default!;


    public Ticket? Ticket { get; set; }
    public AppUser? AppUser { get; set; }
}

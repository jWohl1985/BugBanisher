using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Net.Sockets;

namespace BugBanisher.Models;

public class Ticket
{
    public int Id { get; set; }
    public int ProjectId { get; set; }
    public string TicketTypeId { get; set; } = default!;
    public string TicketStatusId { get; set; } = default!;
    public string TicketPriorityId { get; set; } = default!;
    public string CreatorId { get; set; } = default!;
    public string? DeveloperId { get; set; }


    public string Title { get; set; } = default!;
    public string Description { get; set; } = default!;
    public DateTime Created { get; set; }
    public DateTime? Updated { get; set; }
    public bool IsArchived { get; set; }
    public bool IsArchivedByProject { get; set; }

    public Project? Project { get; set; }
    public TicketType? Type { get; set; }
    public TicketStatus? Status { get; set; }
    public TicketPriority? Priority { get; set; }
    public AppUser? Creator { get; set; }
    public AppUser? Developer { get; set; }
    public virtual ICollection<TicketComment> Comments { get; set; } = new HashSet<TicketComment>();
    public virtual ICollection<TicketAttachment> Attachments { get; set; } = new HashSet<TicketAttachment>();
    public virtual ICollection<TicketHistory> History { get; set; } = new HashSet<TicketHistory>();
}


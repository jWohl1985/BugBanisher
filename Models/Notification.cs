using BugBanisher.Models.Enums;

namespace BugBanisher.Models;

public class Notification
{
    public int Id { get; set; }
    public string AppUserId { get; set; } = default!;
    public string? SenderId { get; set; } = default!;
    public int? ProjectId { get; set; }
    public int? TicketId { get; set; }
    public int CompanyId { get; set; }
	public int NotificationTypeId { get; set; } = default!;
	public DateTime Created { get; set; }
    public bool HasBeenSeen { get; set; }
    public string Title { get; set; } = default!;
    public string PreviewText { get; set; } = default!;

    public NotificationType NotificationType => (NotificationType)NotificationTypeId;
    public string TimeSinceCreated
    {
        get
        {
            TimeSpan timeSince = DateTime.Now - Created;

            if (timeSince.Days > 30)
                return "Over a month ago";

            if (timeSince.Days > 1)
                return $"{timeSince.Days} days ago";

            if (timeSince.Days == 1)
                return $"Yesterday";

            if (timeSince.Hours > 1)
                return $"{timeSince.Hours} hours ago";

            if (timeSince.Minutes > 1)
                return $"{timeSince.Minutes} minutes ago";

            return "Just now";
        }
    }

    public AppUser? AppUser { get; set; }
    public Company? Company { get; set; }
    public Ticket? Ticket { get; set; }
    public Project? Project { get; set; }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using BugBanisher.Models;

namespace BugBanisher.Database.Configuration;

public class SeedTicketPriorities : IEntityTypeConfiguration<TicketPriority>
{
    public void Configure(EntityTypeBuilder<TicketPriority> builder)
    {
        TicketPriority[] ticketPriorities = SeedDefaultTicketPriorities();
        builder.HasData(ticketPriorities);
    }

    private TicketPriority[] SeedDefaultTicketPriorities()
    {
        return new TicketPriority[]
        {
            new TicketPriority
            {
                Id = "low",
                Description = "Low"
            },

            new TicketPriority
            {
                Id = "medium",
                Description = "Medium",
            },

            new TicketPriority
            {
                Id = "high",
                Description = "High"
            },
        };
    }
}

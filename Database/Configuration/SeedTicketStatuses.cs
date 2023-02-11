using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using BugBanisher.Models;

namespace BugBanisher.Database.Configuration;

public class SeedTicketStatuses : IEntityTypeConfiguration<TicketStatus>
{
    public void Configure(EntityTypeBuilder<TicketStatus> builder)
    {
        TicketStatus[] ticketStatuses = SeedDefaultTicketStatuses();
        builder.HasData(ticketStatuses);
    }

    private TicketStatus[] SeedDefaultTicketStatuses()
    {
        return new TicketStatus[]
        {
            new TicketStatus
            {
                Id = "unassigned",
                Description = "Unassigned"
            },

            new TicketStatus
            {
                Id = "pending",
                Description = "Pending accept"
            },

            new TicketStatus
            {
                Id = "development",
                Description = "In development"
            },

            new TicketStatus
            {
                Id = "hold",
                Description = "On hold"
            },

            new TicketStatus
            {
                Id = "complete",
                Description = "Complete"
            },
        };
    }
}

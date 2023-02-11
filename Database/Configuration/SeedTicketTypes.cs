using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using BugBanisher.Models;

namespace BugBanisher.Database.Configuration;

public class SeedTicketTypes : IEntityTypeConfiguration<TicketType>
{
    public void Configure(EntityTypeBuilder<TicketType> builder)
    {
        TicketType[] seedTicketTypes = SeedDefaultTicketTypes();
        builder.HasData(seedTicketTypes);
    }

    private TicketType[] SeedDefaultTicketTypes()
    {
        return new TicketType[]
        {
            new TicketType
            {
                Id = "feature",
                Description = "New feature",
            },

            new TicketType
            {
                Id = "bug",
                Description = "Bug fix",
            },

            new TicketType
            {
                Id = "UI",
                Description = "UI change"
            },

            new TicketType
            {
                Id = "other",
                Description = "Other"
            }
        };
    }
}

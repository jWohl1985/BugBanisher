using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using BugBanisher.Models;

namespace BugBanisher.Database.Configuration;

public class SeedCompanies : IEntityTypeConfiguration<Company>
{
    public void Configure(EntityTypeBuilder<Company> builder)
    {
        Company[] seedCompanies = SeedDefaultCompanies();
        builder.HasData(seedCompanies);
    }

    private Company[] SeedDefaultCompanies()
    {
        return new Company[]
        {
             new Company()
             {
                 Id = 1,
                 Name = "JDW, Inc."
             }
        };
    }
}

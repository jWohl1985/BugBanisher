using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using BugBanisher.Models;

namespace BugBanisher.Database.Configuration;

public class SeedProjects : IEntityTypeConfiguration<Project>
{
	public void Configure(EntityTypeBuilder<Project> builder)
	{
		Project[] defaultProjects = SeedDefaultProjects();
		builder.HasData(defaultProjects);
	}

	private Project[] SeedDefaultProjects()
	{
		return new Project[]
		{
			new Project()
			{
				Id = 1,
				CompanyId = 1,
				Name = "WidgetCo Inventory System",
				Description = "Build an enterprise application that can track, manage, and generate reports for WidgetCo's inventory.",
				Created = DateTime.Now,
				Deadline = DateTime.Now + new TimeSpan(days: 180, hours: 0, minutes: 0, seconds: 0),
				PictureData = File.ReadAllBytes("wwwroot/img/ProfilePics/defaultUser.png"),
				PictureExtension = "png",
			}
		};
	}
}

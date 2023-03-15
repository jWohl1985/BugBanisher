using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using BugBanisher.Models;
using BugBanisher.Services.Interfaces;
using BugBanisher.Services;
using System.Drawing;

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
				Created = DateTime.Now - new TimeSpan(days: 22, hours: 0, minutes: 0, seconds: 0),
				Deadline = DateTime.Now + new TimeSpan(days: 180, hours: 0, minutes: 0, seconds: 0),
			},

			new Project()
			{
				Id = 2,
				CompanyId = 1,
				Name = "JW's Diner Website",
				Description = "Design and deploy a simple static website for JW's Diner.",
				Created = DateTime.Now - new TimeSpan(days: 1, hours: 0, minutes: 0, seconds: 0),
				Deadline = DateTime.Now + new TimeSpan(days: 180, hours: 0, minutes: 0, seconds: 0),
			},

			new Project()
			{
				Id = 3,
				CompanyId = 1,
				Name = "Estimate - Existing Website Fix",
				Description = "Our customer had one of our competitors build them a website. It's an ASP.Net Core MVC project, and they are not happy with the result."
				+ " Apparently there are a lot of bugs, missing features, and it crashes often. They want us to give them an estimate to fix it. We need to dive into the code and see" +
				" if this is feasible, or if we're better off starting from scratch. This is our chance to get in with this customer!",
				Created = DateTime.Now - new TimeSpan(days: 3, hours: 0, minutes: 0, seconds: 0),
                Deadline = DateTime.Now + new TimeSpan(days: 20, hours: 0, minutes: 0, seconds: 0),
            },

            new Project()
            {
                Id = 4,
                CompanyId = 1,
                Name = "Project Management Software",
                Description = "The client wants us to build them a web application they can use to manage their projects. It will be an issue tracking application similar to" +
				" the BugBanisher we use. We need to meet with them to determine what their projects entail and the scope of work. This is going to a big project that will keep" +
				" several people tied up for the foreseeable future. Dennis is the project manager. Let's get meetings under way as soon as possible.",
                Created = DateTime.Now - new TimeSpan(days: 60, hours: 0, minutes: 0, seconds: 0),
                Deadline = DateTime.Now + new TimeSpan(days: 305, hours: 0, minutes: 0, seconds: 0),
            },

			// Archived Projects

			new Project()
            {
                Id = 5,
                CompanyId = 1,
                Name = "WPF Meal Planning Application",
                Description = "Create a Windows desktop application for diet and meal planning.",
                Created = DateTime.Now - new TimeSpan(days: 90, hours: 0, minutes: 0, seconds: 0),
                Deadline = DateTime.Now + new TimeSpan(days: 60, hours: 0, minutes: 0, seconds: 0),
				IsArchived = true,
            },

            new Project()
            {
                Id = 6,
                CompanyId = 1,
                Name = "Portfolio Website",
                Description = "Create a portfolio website to start your career in tech.",
                Created = DateTime.Now - new TimeSpan(days: 120, hours: 0, minutes: 0, seconds: 0),
                Deadline = DateTime.Now + new TimeSpan(days: 90, hours: 0, minutes: 0, seconds: 0),
				IsArchived = true,
            },
        };
	}
}

using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using BugBanisher.Models;
using BugBanisher.Models.Enums;
using System.Data;
using System.Net.Sockets;
using System.Threading.Tasks;
using Npgsql;
using BugBanisher.Services.Interfaces;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace BugBanisher.Database;

public static class SeedData
{
	public static string GetConnectionString(IConfiguration configuration)
	{
		string connectionString = configuration.GetConnectionString("DefaultConnection")!;
		var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
		return string.IsNullOrEmpty(databaseUrl) ? connectionString : BuildConnectionString(databaseUrl);
	}

	public static string BuildConnectionString(string databaseUrl)
	{
		var databaseUri = new Uri(databaseUrl);
		var userInfo = databaseUri.UserInfo.Split(':');

		var builder = new NpgsqlConnectionStringBuilder
		{
			Host = databaseUri.Host,
			Port = databaseUri.Port,
			Username = userInfo[0],
			Password = userInfo[1],
			Database = databaseUri.LocalPath.TrimStart('/'),
			SslMode = SslMode.Prefer,
			TrustServerCertificate = true,
		};


		return builder.ToString();
	}

	public static async Task ManageDataAsync(IHost host)
	{
		try
		{
			using var serviceScope = host.Services.CreateScope();
			IServiceProvider serviceProvider = serviceScope.ServiceProvider;

			var dbContextService = serviceProvider.GetRequiredService<BugBanisherContext>();
			var roleManagerService = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
			var userManagerService = serviceProvider.GetRequiredService<UserManager<AppUser>>();

			// Migration: This is the programmatic equivalent to Update-Database
			await dbContextService.Database.MigrateAsync();

			// Seed the demo data
			await SeedRolesAsync(roleManagerService);
			await SeedDemoUsersAsync(userManagerService);
			await SeedDemoProjectsAsync(dbContextService);
			await SeedDemoTicketsAsync(dbContextService);
		}
		catch (Exception ex)
		{
			PrintExceptionMessageToConsole(ex);
		}
	}

	private static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
	{
		await roleManager.CreateAsync(new IdentityRole(Roles.Admin.ToString()));
		await roleManager.CreateAsync(new IdentityRole(Roles.ProjectManager.ToString()));
		await roleManager.CreateAsync(new IdentityRole(Roles.Developer.ToString()));
		await roleManager.CreateAsync(new IdentityRole(Roles.Member.ToString()));
	}

	private static async Task SeedDemoUsersAsync(UserManager<AppUser> userManager)
	{
		AppUser[] demoUsers = new AppUser[]
		{
			new AppUser()
			{
				UserName = "jdwohl@gmail.com",
				Email = "jdwohl@gmail.com",
				FirstName = "Jeremy",
				LastName = "Wohl",
				JobTitle = "President",
				EmailConfirmed = true,
				CompanyId = 1,
			},

			new AppUser()
			{
				UserName = "darrenjohnson@jdwinc.com",
				Email = "darrenjohnson@jdwinc.com",
				FirstName = "Darren",
				LastName = "Johnson",
				JobTitle = "Sr. Project Manager",
				EmailConfirmed = true,
				CompanyId = 1,
			},

			new AppUser()
			{
				UserName = "williamcasey@jdwinc.com",
				Email = "williamcasey@jdwinc.com",
				FirstName = "William",
				LastName = "Casey",
				JobTitle = "Jr. Project Manager",
				EmailConfirmed = true,
				CompanyId = 1,
			},

			new AppUser()
			{
				UserName = "amandagallup@jdwinc.com",
				Email = "amandagallup@jdwinc.com",
				FirstName = "Amanda",
				LastName = "Gallup",
				JobTitle = "Sr. Developer",
				EmailConfirmed = true,
				CompanyId = 1,
			},

			new AppUser()
			{
				UserName = "hermancampos@jdwinc.com",
				Email = "hermancampos@jdwinc.com",
				FirstName = "Herman",
				LastName = "Campos",
				JobTitle = "Developer",
				EmailConfirmed = true,
				CompanyId = 1,
			},

			new AppUser()
			{
				UserName = "shawnblankenship@jdwinc.com",
				Email = "shawnblankenship@jdwinc.com",
				FirstName = "Shawn",
				LastName = "Blankenship",
				JobTitle = "Intern",
				EmailConfirmed = true,
				CompanyId = 1,
			},
		};

		foreach (AppUser demoUser in demoUsers)
		{
			try
			{
				using (FileStream fs = File.OpenRead($"wwwroot/img/ProfilePics/{demoUser.FullName}.jpg"))
				{
					using (var memoryStream = new MemoryStream())
					{
						fs.CopyTo(memoryStream);
						demoUser.PictureData = memoryStream.ToArray();
						demoUser.PictureExtension = "image/jpg";
					}

				}

				AppUser? user = await userManager.FindByEmailAsync(demoUser.Email);
				if (user is null)
				{
					await userManager.CreateAsync(demoUser, "Abc&123!");

					string role = demoUser.FullName switch
					{
						"Jeremy Wohl" => Roles.Admin.ToString(),
						"Darren Johnson" => Roles.ProjectManager.ToString(),
						"William Casey" => Roles.ProjectManager.ToString(),
						"Amanda Gallup" => Roles.Developer.ToString(),
						"Herman Campos" => Roles.Developer.ToString(),
						"Shawn Blankenship" => Roles.Member.ToString(),
						_ => Roles.Member.ToString(),
					};

					await userManager.AddToRoleAsync(demoUser, role);
				}
			}
			catch (Exception ex)
			{
				PrintExceptionMessageToConsole(ex);
				throw;
			}
		}
	}

	private static async Task SeedDemoProjectsAsync(BugBanisherContext context)
	{
		if (context.Projects.Any(p => p.CompanyId == 1))
			return;

		List<AppUser> users = context.Users.ToList();

		AppUser darrenJohnson = users.First(u => u.FullName == "Darren Johnson");
		AppUser williamCasey = users.First(u => u.FullName == "William Casey");
		AppUser amandaGallup = users.First(u => u.FullName == "Amanda Gallup");
		AppUser hermanCampos = users.First(u => u.FullName == "Herman Campos");
		AppUser shawnBlankenship = users.First(u => u.FullName == "Shawn Blankenship");

		Project[] seedProjects = new Project[]
		{
                // Active projects

                new Project()
				{
					CompanyId = 1,
					Name = "WidgetCo Inventory System",
					Description = "Build an enterprise application that can track, manage, and generate reports for WidgetCo's inventory.",
					Created = DateTime.Now - new TimeSpan(days: 22, hours: 0, minutes: 0, seconds: 0),
					Deadline = DateTime.Now + new TimeSpan(days: 180, hours: 0, minutes: 0, seconds: 0),
					ProjectManagerId = darrenJohnson.Id,
					Team = new List<AppUser>
					{
						amandaGallup,
						shawnBlankenship,
					}
				},

				new Project()
				{
					CompanyId = 1,
					Name = "JW's Diner Website",
					Description = "Design and deploy a simple static website for JW's Diner.",
					Created = DateTime.Now - new TimeSpan(days: 1, hours: 0, minutes: 0, seconds: 0),
					Deadline = DateTime.Now + new TimeSpan(days: 180, hours: 0, minutes: 0, seconds: 0),
					ProjectManagerId = williamCasey.Id,
					Team = new List<AppUser>
					{
						hermanCampos,
						shawnBlankenship,
					}
				},

				new Project()
				{
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
					CompanyId = 1,
					Name = "Project Management Software",
					Description = "The client wants us to build them a web application they can use to manage their projects. It will be an issue tracking application similar to" +
					" the BugBanisher we use. We need to meet with them to determine what their projects entail and the scope of work. This is going to a big project that will keep" +
					" several people tied up for the foreseeable future. Dennis is the project manager. Let's get meetings under way as soon as possible.",
					Created = DateTime.Now - new TimeSpan(days: 60, hours: 0, minutes: 0, seconds: 0),
					Deadline = DateTime.Now + new TimeSpan(days: 305, hours: 0, minutes: 0, seconds: 0),
					ProjectManagerId = darrenJohnson.Id,
					Team = new List<AppUser>
					{
						amandaGallup,
						hermanCampos,
						shawnBlankenship,
					}
				},

			    // Archived Projects

			    new Project()
				{
					CompanyId = 1,
					Name = "WPF Meal Planning Application",
					Description = "Create a Windows desktop application for diet and meal planning.",
					Created = DateTime.Now - new TimeSpan(days: 90, hours: 0, minutes: 0, seconds: 0),
					Deadline = DateTime.Now + new TimeSpan(days: 60, hours: 0, minutes: 0, seconds: 0),
					ProjectManagerId = williamCasey.Id,
					IsArchived = true,
					Team = new List<AppUser>
					{
						amandaGallup,
						shawnBlankenship,
					}
				},

				new Project()
				{
					CompanyId = 1,
					Name = "Portfolio Website",
					Description = "Create a portfolio website to start your career in tech.",
					Created = DateTime.Now - new TimeSpan(days: 120, hours: 0, minutes: 0, seconds: 0),
					Deadline = DateTime.Now + new TimeSpan(days: 90, hours: 0, minutes: 0, seconds: 0),
					ProjectManagerId = williamCasey.Id,
					IsArchived = true,
					Team = new List<AppUser>
					{
						hermanCampos,
						shawnBlankenship,
					}
				},
		};

		try
		{
			foreach (Project project in seedProjects)
			{
				using (FileStream fs = File.OpenRead($"wwwroot/img/ProjectPics/{project.Name}.jpg"))
				{
					using (var memoryStream = new MemoryStream())
					{
						fs.CopyTo(memoryStream);
						project.PictureData = memoryStream.ToArray();
						project.PictureExtension = "image/jpg";
					}

				}
				context.Projects.Add(project);
				await context.SaveChangesAsync();
			}
		}
		catch (Exception ex)
		{
			PrintExceptionMessageToConsole(ex);
			throw;
		}
	}

	private static async Task SeedDemoTicketsAsync(BugBanisherContext context)
	{
		if (context.Tickets.Any())
			return;

		List<AppUser> users = context.Users.ToList();

		AppUser darrenJohnson = users.First(u => u.FullName == "Darren Johnson");
		AppUser williamCasey = users.First(u => u.FullName == "William Casey");
		AppUser amandaGallup = users.First(u => u.FullName == "Amanda Gallup");
		AppUser hermanCampos = users.First(u => u.FullName == "Herman Campos");
		AppUser shawnBlankenship = users.First(u => u.FullName == "Shawn Blankenship");

		List<Project> projects = context.Projects.ToList();

		Project widgetCo = projects.First(p => p.Name == "WidgetCo Inventory System");
		Project diner = projects.First(p => p.Name == "JW's Diner Website");
		Project estimate = projects.First(p => p.Name == "Estimate - Existing Website Fix");

		Ticket[] demoTickets = new Ticket[]
		{
			new Ticket()
			{
				ProjectId = widgetCo.Id,
				TicketTypeId = "bug",
				TicketStatusId = "development",
				TicketPriorityId = "high",
				CreatorId = darrenJohnson.Id,
				DeveloperId = amandaGallup.Id,
				Title = "Crash when deleting certain products",
				Description = "The app is crashing when a user tries to delete certain products. We can't figure out what these items have in common, but it's repeatable and" +
				" always happens on the same ones. I attached the list.",
				Created = DateTime.Now - new TimeSpan(days: 1, hours: 0, minutes: 0, seconds: 0),
			},

			new Ticket()
			{
				ProjectId = widgetCo.Id,
				TicketTypeId = "feature",
				TicketStatusId = "development",
				TicketPriorityId = "medium",
				CreatorId = darrenJohnson.Id,
				DeveloperId = amandaGallup.Id,
				Title = "New report column",
				Description = "The customer wants to add a new column to the \"Items in Stock\" report. It should be titled \"Last Cost\" and show the last price the item"
				+ " was purchased at.",
				Created = DateTime.Now - new TimeSpan(days: 3, hours: 0, minutes: 0, seconds: 0),
			},

			new Ticket()
			{
				ProjectId = widgetCo.Id,
				TicketTypeId = "UI",
				TicketStatusId = "pending",
				TicketPriorityId = "low",
				CreatorId = amandaGallup.Id,
				DeveloperId = amandaGallup.Id,
				Title = "UI colors",
				Description = "Just a reminder ticket so I don't forget about it. The UI colors need to be updated per the attached list. I'll get to it eventually but if anyone is looking for something to do"
				+ " this would help me out.",
				Created = DateTime.Now - new TimeSpan(days: 7, hours: 0, minutes: 0, seconds: 0),
			},

			new Ticket()
			{
				ProjectId = widgetCo.Id,
				TicketTypeId = "bug",
				TicketStatusId = "hold",
				TicketPriorityId = "medium",
				CreatorId = darrenJohnson.Id,
				DeveloperId = amandaGallup.Id,
				Title = "Purchasing view occasionally freezing",
				Description = "I haven't been able to reproduce it myself but customer says the purchasing view is occasionally locking up. Can you see if you can reproduce and fix?",
				Created = DateTime.Now - new TimeSpan(days: 2, hours: 0, minutes: 0, seconds: 0),
			},

			new Ticket()
			{
				ProjectId = widgetCo.Id,
				TicketTypeId = "feature",
				TicketStatusId = "complete",
				TicketPriorityId = "medium",
				CreatorId = amandaGallup.Id,
				DeveloperId = amandaGallup.Id,
				Title = "Automatic reorder feature",
				Description = "Customer gave us direction to add a feature. When the quantity of an item drops below 10, they want it to prompt the user to "
				+ "to place a restock order, and generate the order for them if they choose yes.",
				Created = DateTime.Now - new TimeSpan(days: 8, hours: 0, minutes: 0, seconds: 0),
			},

			new Ticket()
			{
				ProjectId = widgetCo.Id,
				TicketTypeId = "other",
				TicketStatusId = "complete",
				TicketPriorityId = "high",
				CreatorId = darrenJohnson.Id,
				DeveloperId = amandaGallup.Id,
				Title = "Database swap",
				Description = "Can we change the database from SQLServer to PostgreSQL. I know, I know. Let me know what this is going to take.",
				Created = DateTime.Now - new TimeSpan(days: 20, hours: 0, minutes: 0, seconds: 0),
				IsArchived = true,
			},
		};

		try
		{
			foreach (Ticket demoTicket in demoTickets)
			{
				context.Tickets.Add(demoTicket);
			}

			await context.SaveChangesAsync();
		}
		catch (Exception ex)
		{
			PrintExceptionMessageToConsole(ex);
			throw;
		}
	}

	private static void PrintExceptionMessageToConsole(Exception exception)
	{
		ConsoleColor previousColor = Console.ForegroundColor;
		Console.ForegroundColor = ConsoleColor.Red;
		Console.WriteLine("*************  ERROR  *************");
		Console.WriteLine(exception.Message);
		Console.ForegroundColor = previousColor;
	}
}

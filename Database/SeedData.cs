using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using BugBanisher.Models;
using BugBanisher.Models.Enums;
using Npgsql;
using BugBanisher.Services.Interfaces;

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
			var ticketService = serviceProvider.GetRequiredService<ITicketService>();
			var ticketHistoryService = serviceProvider.GetRequiredService<ITicketHistoryService>();

			// Migration: This is the programmatic equivalent to Update-Database
			await dbContextService.Database.MigrateAsync();

			// Seed the demo data
			await SeedRolesAsync(roleManagerService);
			await SeedUsers.SeedDemoUsersAsync(userManagerService);
			await SeedProjects.SeedDemoProjectsAsync(dbContextService);
			await SeedTickets.SeedDemoTicketsAsync(dbContextService, ticketService, ticketHistoryService);
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

	private static void PrintExceptionMessageToConsole(Exception exception)
	{
		ConsoleColor previousColor = Console.ForegroundColor;
		Console.ForegroundColor = ConsoleColor.Red;
		Console.WriteLine("*************  ERROR  *************");
		Console.WriteLine(exception.Message);
		Console.ForegroundColor = previousColor;
	}
}

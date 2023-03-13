using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using BugBanisher.Models;
using BugBanisher.Models.Enums;
using System.Data;
using System.Net.Sockets;
using System.Threading.Tasks;
using Npgsql;

namespace BugBanisher.Database;

public class SeedData
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

            // Seed the default data
            await SeedRolesAsync(roleManagerService);
            await SeedDefaultUsersAsync(userManagerService);
        }
        catch (Exception ex)
        {
            ConsoleColor previousColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(ex);
            Console.ForegroundColor = previousColor;
        }
    }

    public static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
    {
        await roleManager.CreateAsync(new IdentityRole(Roles.Admin.ToString()));
        await roleManager.CreateAsync(new IdentityRole(Roles.ProjectManager.ToString()));
        await roleManager.CreateAsync(new IdentityRole(Roles.Developer.ToString()));
        await roleManager.CreateAsync(new IdentityRole(Roles.Member.ToString()));
    }

    public static async Task SeedDefaultUsersAsync(UserManager<AppUser> userManager)
    {
        AppUser defaultAdmin = new()
        {
            UserName = "jdwohl@gmail.com",
            Email = "jdwohl@gmail.com",
            FirstName = "Jeremy",
            LastName = "Wohl",
            JobTitle = "President",
            EmailConfirmed = true,
            CompanyId = 1,
        };

        try
        {
            AppUser? user = await userManager.FindByEmailAsync(defaultAdmin.Email);
            if (user is null)
            {
                await userManager.CreateAsync(defaultAdmin, "Abc&123!");
                await userManager.AddToRoleAsync(defaultAdmin, Roles.Admin.ToString());
            }
        }
        catch (Exception ex)
        {
            ConsoleColor previousColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("*************  ERROR  *************");
            Console.WriteLine("Error seeding default admin.");
            Console.WriteLine(ex.Message);
            Console.WriteLine("***********************************");
            Console.ForegroundColor = previousColor;
            throw;
        }

        AppUser defaultProjectManager = new()
        {
            UserName = "dennisjohnson@mailinator.com",
            Email = "dennisjohnson@mailinator.com",
            FirstName = "Dennis",
            LastName = "Johnson",
            JobTitle = "Project Manager",
            EmailConfirmed = true,
            CompanyId = 1,
        };

        try
        {
            AppUser? user = await userManager.FindByEmailAsync(defaultProjectManager.Email);
            if (user is null)
            {
                await userManager.CreateAsync(defaultProjectManager, "Abc&123!");
                await userManager.AddToRoleAsync(defaultProjectManager, Roles.ProjectManager.ToString());
            }
        }
        catch (Exception ex)
        {
            ConsoleColor previousColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("*************  ERROR  *************");
            Console.WriteLine("Error seeding default project manager.");
            Console.WriteLine(ex.Message);
            Console.WriteLine("***********************************");
            Console.ForegroundColor = previousColor;
            throw;
        }

        AppUser defaultDeveloper = new()
        {
            UserName = "johnsmith@mailinator.com",
            Email = "johnsmith@mailinator.com",
            FirstName = "John",
            LastName = "Smith",
            JobTitle = "Developer",
            EmailConfirmed = true,
            CompanyId = 1,
        };

        try
        {
            AppUser? user = await userManager.FindByEmailAsync(defaultDeveloper.Email);
            if (user is null)
            {
                await userManager.CreateAsync(defaultDeveloper, "Abc&123!");
                await userManager.AddToRoleAsync(defaultDeveloper, Roles.Developer.ToString());
            }
        }
        catch (Exception ex)
        {
            ConsoleColor previousColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("*************  ERROR  *************");
            Console.WriteLine("Error seeding default developer.");
            Console.WriteLine(ex.Message);
            Console.WriteLine("***********************************");
            Console.ForegroundColor = previousColor;
            throw;
        }
    }
}

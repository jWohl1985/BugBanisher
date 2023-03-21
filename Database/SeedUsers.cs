using BugBanisher.Models.Enums;
using BugBanisher.Models;
using Microsoft.AspNetCore.Identity;

namespace BugBanisher.Database;

public static class SeedUsers
{
    public static async Task SeedDemoUsersAsync(UserManager<AppUser> userManager)
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
            catch (Exception)
            {
                throw;
            }
        }
    }
}

using BugBanisher.Models;

namespace BugBanisher.Database;

public static class SeedProjects
{
    public static async Task SeedDemoProjectsAsync(BugBanisherContext context)
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
                    ProjectManagerId = williamCasey.Id,
                    Created = DateTime.Now - new TimeSpan(days: 3, hours: 0, minutes: 0, seconds: 0),
                    Deadline = DateTime.Now + new TimeSpan(days: 20, hours: 0, minutes: 0, seconds: 0),
                    Team = new List<AppUser>
                    {
                        hermanCampos,
                        shawnBlankenship,
                    }
                },

                new Project()
                {
                    CompanyId = 1,
                    Name = "Project Management Software",
                    Description = "The client wants us to build them a web application they can use to manage their projects. It will be an issue tracking application similar to" +
                    " the BugBanisher we use. We need to meet with them to determine what their projects entail and the scope of work. This is going to a big project that will keep" +
                    " several people tied up for the foreseeable future. Darren will be the project manager. Let's get meetings under way as soon as possible.",
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
                    Description = "We've all been working hard, let's create a portfolio website to show off our work and attract more clients!",
                    Created = DateTime.Now - new TimeSpan(days: 120, hours: 0, minutes: 0, seconds: 0),
                    Deadline = DateTime.Now + new TimeSpan(days: 45, hours: 0, minutes: 0, seconds: 0),
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
        catch (Exception)
        {
            throw;
        }
    }
}

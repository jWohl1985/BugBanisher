using BugBanisher.Models;
using BugBanisher.Services.Interfaces;
using System.Net.Sockets;

namespace BugBanisher.Database;

public static class SeedTickets
{
    private static BugBanisherContext _context = default!;
    private static ITicketService _ticketService = default!;
    private static ITicketHistoryService _ticketHistoryService = default!;

    private static List<AppUser> users => _context.Users.ToList();
    private static AppUser jeremyWohl => users.First(u => u.FullName == "Jeremy Wohl");
    private static AppUser darrenJohnson => users.First(u => u.FullName == "Darren Johnson");
    private static AppUser williamCasey => users.First(u => u.FullName == "William Casey");
    private static AppUser amandaGallup => users.First(u => u.FullName == "Amanda Gallup");
    private static AppUser hermanCampos => users.First(u => u.FullName == "Herman Campos");
    private static AppUser shawnBlankenship => users.First(u => u.FullName == "Shawn Blankenship");
    private static List<Project> projects => _context.Projects.ToList();

    // Active projects
    private static Project widgetCo => projects.First(p => p.Name == "WidgetCo Inventory System");
    private static Project diner => projects.First(p => p.Name == "JW's Diner Website");
    private static Project estimate => projects.First(p => p.Name == "Estimate - Existing Website Fix");
    private static Project projectManagement => projects.First(p => p.Name == "Project Management Software");

    // Archived projects
    private static Project wpfDiet => projects.First(p => p.Name == "WPF Meal Planning Application");
    private static Project portfolio => projects.First(p => p.Name == "Portfolio Website");

    public static async Task SeedDemoTicketsAsync(BugBanisherContext context, ITicketService ticketService, ITicketHistoryService ticketHistoryService)
    {
        if (context.Tickets.Any())
            return;

        _context = context;
        _ticketService = ticketService;
        _ticketHistoryService = ticketHistoryService;

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

            new Ticket()
            {
                ProjectId = diner.Id,
                TicketTypeId = "feature",
                TicketStatusId = "development",
                TicketPriorityId = "medium",
                CreatorId = williamCasey.Id,
                DeveloperId = hermanCampos.Id,
                Title = "Menu page",
                Description = "Can we add a link in the nav bar to a new page that shows the current menu? See the attachment.",
                Created = DateTime.Now - new TimeSpan(days: 1, hours: 0, minutes: 0, seconds: 0),
            },

            new Ticket()
            {
                ProjectId = diner.Id,
                TicketTypeId = "other",
                TicketStatusId = "pending",
                TicketPriorityId = "low",
                CreatorId = williamCasey.Id,
                DeveloperId = hermanCampos.Id,
                Title = "New contact info",
                Description = "See the attachment, can we please update the \"Contact Us\" page with this new information?",
                Created = DateTime.Now,
            },

            new Ticket()
            {
                ProjectId = diner.Id,
                TicketTypeId = "bug",
                TicketStatusId = "complete",
                TicketPriorityId = "high",
                CreatorId = williamCasey.Id,
                DeveloperId = hermanCampos.Id,
                Title = "Dead links",
                Description = "Some of the links in the footer are going to the Not Found page, I think it's because we renamed some of the files. Can you update the references please?",
                Created = DateTime.Now - new TimeSpan(days: 1, hours: 0, minutes: 0, seconds: 0),
            },

            new Ticket()
            {
                ProjectId = diner.Id,
                TicketTypeId = "other",
                TicketStatusId = "complete",
                TicketPriorityId = "high",
                CreatorId = hermanCampos.Id,
                DeveloperId = hermanCampos.Id,
                Title = "Presentation meeting",
                Description = "We finished the initial design based on our meetings, we need to present the final look to the client and make sure they're happy with it." +
                " I have a Zoom call with the customer at 9:30am tomorrow. I'll present the layout and examples we came up with and then we should be able to get started" +
                " if they like it. Link to the meeting if you want to join: https://teams.microsoft.com/l/app/not-a-real-link",
                Created = DateTime.Now - new TimeSpan(days: 3, hours: 0, minutes: 0, seconds: 0),
                IsArchived = true,
            },

            new Ticket()
            {
                ProjectId = estimate.Id,
                TicketTypeId = "other",
                TicketStatusId = "complete",
                TicketPriorityId = "high",
                CreatorId = jeremyWohl.Id,
                DeveloperId = jeremyWohl.Id,
                Title = "Assign a PM",
                Description = "This project needs to have a PM assigned. I'll get with Darren and William and see who will have time to take it. There shouldn't be much" +
                " PM work as far as just getting the estimate to them goes.",
                Created = DateTime.Now - new TimeSpan(days: 5, hours: 0, minutes: 0, seconds: 0),
                IsArchived = true,
            },

            new Ticket()
            {
                ProjectId = estimate.Id,
                TicketTypeId = "other",
                TicketStatusId = "development",
                TicketPriorityId = "high",
                CreatorId = jeremyWohl.Id,
                DeveloperId = hermanCampos.Id,
                Title = "Review existing code",
                Description = "Herman, can you please review the existing code and let me know what we're dealing with? Do we need to start over or just fix it up? They have it" +
                " on GitHub: https://www.github.com/not-a-real-link",
                Created = DateTime.Now - new TimeSpan(days: 4, hours: 0, minutes: 0, seconds: 0),
            },

            new Ticket()
            {
                ProjectId = projectManagement.Id,
                TicketTypeId = "bug",
                TicketStatusId = "development",
                TicketPriorityId = "medium",
                CreatorId = darrenJohnson.Id,
                DeveloperId = amandaGallup.Id,
                Title = "Last project missing",
                Description = "On the pages that show the list of current projects, the most recently added project is always missing from the list. Can you investigate and fix please?",
                Created = DateTime.Now - new TimeSpan(days: 1, hours: 0, minutes: 0, seconds: 0),
            },

            new Ticket()
            {
                ProjectId = projectManagement.Id,
                TicketTypeId = "UI",
                TicketStatusId = "development",
                TicketPriorityId = "medium",
                CreatorId = darrenJohnson.Id,
                DeveloperId = hermanCampos.Id,
                Title = "Fix mobile view",
                Description = "The project status page doesn't look good on smaller screens, can we make this more responsive?",
                Created = DateTime.Now - new TimeSpan(days: 1, hours: 0, minutes: 0, seconds: 0),
            },

            new Ticket()
            {
                ProjectId = projectManagement.Id,
                TicketTypeId = "feature",
                TicketPriorityId = "low",
                TicketStatusId = "hold",
                CreatorId = darrenJohnson.Id,
                DeveloperId = amandaGallup.Id,
                Title = "New view - employee workload",
                Description = "The customer wants us to add a view where a manager can see all of the projects and tickets that an employee currently has assigned. Please" +
                " see the attachment for general layout, and let me know if you have any questions. This needs to be done eventually, but it's not a priority at the moment.",
                Created = DateTime.Now - new TimeSpan(days: 3, hours: 0, minutes: 0, seconds: 0),
            },

            new Ticket()
            {
                ProjectId = projectManagement.Id,
                TicketTypeId = "feature",
                TicketPriorityId = "medium",
                TicketStatusId = "complete",
                CreatorId = hermanCampos.Id,
                DeveloperId = hermanCampos.Id,
                Title = "PDF generation",
                Description = "We need to implement the IReportService interface that Amanda created for generating various PDF reports. I'm going to work on doing this using" +
                " the QuestPDF nuget package. Assigning this to myself.",
                Created = DateTime.Now - new TimeSpan(days: 4, hours: 0, minutes: 0, seconds: 0),
            },

            new Ticket()
            {
                ProjectId = projectManagement.Id,
                TicketTypeId = "other",
                TicketPriorityId = "high",
                TicketStatusId = "complete",
                CreatorId = amandaGallup.Id,
                DeveloperId = amandaGallup.Id,
                Title = "Ticket page clarifications",
                Description = "Darren, see my email in the attachment. I'm not sure how they want this page to work. Can we schedule a call with the end user to go over this with them? I can't move" +
                " forward on this part without some clarification. Putting this as a ticket for tracking.",
                Created = DateTime.Now - new TimeSpan(days: 6, hours: 0, minutes: 0, seconds: 0),
                IsArchived = true,
            },

            new Ticket()
            {
                ProjectId = wpfDiet.Id,
                TicketTypeId = "feature",
                TicketPriorityId = "medium",
                TicketStatusId = "complete",
                CreatorId = williamCasey.Id,
                DeveloperId = amandaGallup.Id,
                Title = "Import/export",
                Description = "Last feature to tie this up I think! Talking with the group we think it'd be nice if users could import a list of foods from a file instead of manually entering them. Being able to export to a file would be good too" +
                " so people can share their food lists or back them up. Doesn't have to be anything fancy, just a csv file or something. Can we do this?",
                Created = DateTime.Now - new TimeSpan(days: 32, hours: 0, minutes: 0, seconds: 0),
                IsArchived = true,
                IsArchivedByProject = true,
            },

            new Ticket()
            {
                ProjectId = wpfDiet.Id,
                TicketTypeId = "bug",
                TicketPriorityId = "high",
                TicketStatusId = "complete",
                CreatorId = amandaGallup.Id,
                DeveloperId = amandaGallup.Id,
                Title = "Data validation",
                Description = "Most features are up and running now, but they depend on the user inputting correct information. We need to implement some kind of data validation for" +
                " the add/edit food and add/edit user views.",
                Created = DateTime.Now - new TimeSpan(days: 28, hours: 0, minutes: 0, seconds: 0),
                IsArchived = true,
                IsArchivedByProject = true,
            },

            new Ticket()
            {
                ProjectId = wpfDiet.Id,
                TicketTypeId = "bug",
                TicketPriorityId = "high",
                TicketStatusId = "complete",
                CreatorId = williamCasey.Id,
                DeveloperId = amandaGallup.Id,
                Title = "Calorie calculations, unit tests",
                Description = "I think we found a problem with the calorie calculation for certain users? See what happens when you enter the attached user data. The calories should"
                + " be around 1700 but it gives back 1900. Amanda, can you investigate and add some unit tests to make sure this is always calculating properly?",
                Created = DateTime.Now - new TimeSpan(days: 25, hours: 0, minutes: 0, seconds: 0),
                IsArchived = true,
                IsArchivedByProject = true,
            },

            new Ticket()
            {
                ProjectId = wpfDiet.Id,
                TicketTypeId = "UI",
                TicketPriorityId = "medium",
                TicketStatusId = "complete",
                CreatorId = williamCasey.Id,
                DeveloperId = amandaGallup.Id,
                Title = "Logo placement",
                Description = "Please tweak the placement of the customer's logo as shown in the attachment.",
                Created = DateTime.Now - new TimeSpan(days: 22, hours: 0, minutes: 0, seconds: 0),
                IsArchived = true,
                IsArchivedByProject = true,
            },

            new Ticket()
            {
                ProjectId = portfolio.Id,
                TicketTypeId = "feature",
                TicketPriorityId = "high",
                TicketStatusId = "complete",
                CreatorId = williamCasey.Id,
                DeveloperId = hermanCampos.Id,
                Title = "Blog posts",
                Description = "Site's looking great, but the blog is a little empty, can we write a few posts?",
                Created = DateTime.Now - new TimeSpan(days: 77, hours: 0, minutes: 0, seconds: 0),
                IsArchivedByProject = true,
            },

            new Ticket()
            {
                ProjectId = portfolio.Id,
                TicketTypeId = "feature",
                TicketPriorityId = "high",
                TicketStatusId = "complete",
                CreatorId = williamCasey.Id,
                DeveloperId = hermanCampos.Id,
                Title = "Finish portfolio website",
                Description = "The boss wants us to build a website to showcase our projects. Can we get something up showing off the stuff we've been working on?",
                Created = DateTime.Now - new TimeSpan(days: 90, hours: 0, minutes: 0, seconds: 0),
                IsArchived = true,
                IsArchivedByProject = true,
            },
        };

        try
        {
            foreach (Ticket demoTicket in demoTickets)
            {
                context.Tickets.Add(demoTicket);
            }

            await context.SaveChangesAsync();

            foreach (Ticket demoTicket in demoTickets)
            {
                await ticketHistoryService.AddTicketCreatedEventAsync(demoTicket.Id);
            }
        }
        catch (Exception)
        {
            throw;
        }

        await SeedCommentsAsync();
        //await SeedAttachmentsAsync();
        await SeedHistoryItemsAsync();
    }

    private static async Task SeedCommentsAsync()
    {
        await SeedEstimateProjectCommentsAsync();
    }

    private static async Task SeedHistoryItemsAsync()
    {
        await SeedEstimateProjectHistoryItemsAsync();
    }

    private static async Task SeedEstimateProjectCommentsAsync()
    {
        Ticket assignAPM = _context.Tickets.Where(t => t.Title == "Assign a PM").First();

        TicketComment williamComment = new TicketComment()
        {
            TicketId = assignAPM.Id,
            AppUserId = williamCasey.Id,
            Created = assignAPM.Created + new TimeSpan(days: 1, hours: 0, minutes: 0, seconds: 0),
            Comment = "@Jeremy Wohl I can do this, Darren is buried with the Project Management Software project and all I've got at the moment is JW's Diner.",
        };

        TicketComment jeremyComment = new TicketComment()
        {
            TicketId = assignAPM.Id,
            AppUserId = jeremyWohl.Id,
            Created = assignAPM.Created + new TimeSpan(days: 1, hours: 3, minutes: 0, seconds: 0),
            Comment = "Thanks William, I'll go ahead and assign it and mark this complete.",
        };

        await _ticketService.AddTicketCommentAsync(assignAPM.Id, williamComment);
        await _ticketService.AddTicketCommentAsync(assignAPM.Id, jeremyComment);
    }

    private static async Task SeedEstimateProjectHistoryItemsAsync()
    {
        Ticket assignAPM = _context.Tickets.Where(t => t.Title == "Assign a PM").First();

        TicketHistory statusChange = new TicketHistory()
        {
            TicketId = assignAPM.Id,
            AppUserId = jeremyWohl.Id,
            Created = assignAPM.Created + new TimeSpan(days: 1, hours: 3, minutes: 8, seconds: 0),
            Description = $"{jeremyWohl.FullName} made the following changes:</br><ul>",
        };

        statusChange.Description += $"<li>Changed the ticket status to <strong>Complete</strong></li>";
        statusChange.Description += "</ul>";

        TicketHistory archiveChange = new TicketHistory()
        {
            TicketId = assignAPM.Id,
            AppUserId = jeremyWohl.Id,
            Created = assignAPM.Created + new TimeSpan(days: 1, hours: 3, minutes: 9, seconds: 0),
            Description = "Jeremy Wohl archived the ticket."
        };

        await _ticketHistoryService.AddTicketHistoryItemAsync(statusChange);
        await _ticketHistoryService.AddTicketHistoryItemAsync(archiveChange);

        Ticket reviewExistingCode = _context.Tickets.Where(t => t.Title == "Review existing code").First();

        TicketHistory accepted = new TicketHistory()
        {
            TicketId = reviewExistingCode.Id,
            AppUserId = hermanCampos.Id,
            Created = reviewExistingCode.Created + new TimeSpan(days: 1, hours: 5, minutes: 22, seconds: 0),
            Description = $"{hermanCampos.FullName} made the following changes:</br><ul>",
        };

        accepted.Description += $"<li>Changed the ticket status to <strong>In Development</strong></li>";
        accepted.Description += "</ul>";

        await _ticketHistoryService.AddTicketHistoryItemAsync(accepted);
    }
}

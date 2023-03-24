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
        await SeedAttachmentsAsync();
        await SeedHistoryItemsAsync();
    }

    private static async Task SeedCommentsAsync()
    {
        await SeedEstimateProjectCommentsAsync();
        await SeedDinerProjectCommentsAsync();
        await SeedProjectManagementProjectCommentsAsync();
        await SeedInventoryProjectCommentsAsync();
        await SeedPortfolioProjectCommentsAsync();
        await SeedDietProjectCommentsAsync();
    }

    private static async Task SeedAttachmentsAsync()
    {
        await SeedDinerProjectAttachmentsAsync();
        await SeedProjectManagementProjectAttachmentsAsync();
        await SeedInventoryProjectAttachmentsAsync();
        await SeedDietProjectAttachmentsAsync();
    }

    private static async Task SeedHistoryItemsAsync()
    {
        await SeedEstimateProjectHistoryItemsAsync();
        await SeedDinerProjectHistoryItemsAsync();
        await SeedProjectManagementProjectHistoryItemsAsync();
        await SeedInventoryProjectHistoryItemsAsync();
        await SeedPortfolioProjectHistoryItemsAsync();
        await SeedDietProjectHistoryItemsAsync();
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

    private static async Task SeedDinerProjectCommentsAsync()
    {
        Ticket presentationMeeting = _context.Tickets.Where(t => t.Title == "Presentation meeting").First();
        Ticket menuPage = _context.Tickets.Where(t => t.Title == "Menu page").First();

        TicketComment hermanComment = new TicketComment()
        {
            TicketId = presentationMeeting.Id,
            AppUserId = hermanCampos.Id,
            Created = presentationMeeting.Created + new TimeSpan(days: 1, hours: 0, minutes: 0, seconds: 0),
            Comment = "They liked it! Marking this complete and archiving. Thanks for jumping in Will.",
        };

        TicketComment hermanComment2 = new TicketComment()
        {
            TicketId = menuPage.Id,
            AppUserId = hermanCampos.Id,
            Created = menuPage.Created + new TimeSpan(days: 1, hours: 0, minutes: 0, seconds: 0),
            Comment = "Working on it now...should be easy!",
        };

        await _ticketService.AddTicketCommentAsync(presentationMeeting.Id, hermanComment);
        await _ticketService.AddTicketCommentAsync(menuPage.Id, hermanComment2);
    }

    private static async Task SeedProjectManagementProjectCommentsAsync()
    {
        Ticket ticketPageClarifications = _context.Tickets.Where(t => t.Title == "Ticket page clarifications").First();

        TicketComment darrenComment = new TicketComment()
        {
            TicketId = ticketPageClarifications.Id,
            AppUserId = darrenJohnson.Id,
            Created = ticketPageClarifications.Created + new TimeSpan(days: 0, hours: 6, minutes: 0, seconds: 0),
            Comment = "@Amanda Gallup Does 10:00am tomorrow work?",
        };

        TicketComment amandaComment = new TicketComment()
        {
            TicketId = ticketPageClarifications.Id,
            AppUserId = amandaGallup.Id,
            Created = ticketPageClarifications.Created + new TimeSpan(days: 0, hours: 6, minutes: 24, seconds: 0),
            Comment = "Yes, thanks. Talk to you then.",
        };

        await _ticketService.AddTicketCommentAsync(ticketPageClarifications.Id, darrenComment);
        await _ticketService.AddTicketCommentAsync(ticketPageClarifications.Id, amandaComment);

        Ticket pdfGeneration = _context.Tickets.Where(t => t.Title == "PDF generation").First();

        TicketComment darrenComment2 = new TicketComment()
        {
            TicketId = pdfGeneration.Id,
            AppUserId = darrenJohnson.Id,
            Created = pdfGeneration.Created + new TimeSpan(days: 1, hours: 6, minutes: 0, seconds: 0),
            Comment = "Sounds good!",
        };

        await _ticketService.AddTicketCommentAsync(pdfGeneration.Id, darrenComment2);

        Ticket newView = _context.Tickets.Where(t => t.Title == "New view - employee workload").First();

        TicketComment amandaComment2 = new TicketComment()
        {
            TicketId = newView.Id,
            AppUserId = amandaGallup.Id,
            Created = newView.Created + new TimeSpan(days: 0, hours: 3, minutes: 59, seconds: 0),
            Comment = "@Darren Johnson -- there's no attachment. When we spoke you weren't sure if we actually had it yet. Placing this on hold for now.",
        };

        TicketComment darrenComment3 = new TicketComment()
        {
            TicketId = newView.Id,
            AppUserId = darrenJohnson.Id,
            Created = newView.Created + new TimeSpan(days: 0, hours: 5, minutes: 13, seconds: 0),
            Comment = "@Amanda Gallup Yeah sorry I thought it was in my emails and I created this ticket a little early. I'll let you know when we get it.",
        };

        await _ticketService.AddTicketCommentAsync(newView.Id, amandaComment2);
        await _ticketService.AddTicketCommentAsync(newView.Id, darrenComment3);

        Ticket mobileView = _context.Tickets.Where(t => t.Title == "Fix mobile view").First();

        TicketComment hermanComment = new TicketComment()
        {
            TicketId = mobileView.Id,
            AppUserId = hermanCampos.Id,
            Created = mobileView.Created + new TimeSpan(days: 0, hours: 11, minutes: 28, seconds: 0),
            Comment = "Might be a good one for the intern. @Shawn Blankenship you want to give this one a try? Remember what I taught you about Bootstrap breakpoints?",
        };

        TicketComment seanComment = new TicketComment()
        {
            TicketId = mobileView.Id,
            AppUserId = shawnBlankenship.Id,
            Created = mobileView.Created + new TimeSpan(days: 0, hours: 12, minutes: 13, seconds: 0),
            Comment = "@Herman Campos Sure!",
        };

        await _ticketService.AddTicketCommentAsync(mobileView.Id, hermanComment);
        await _ticketService.AddTicketCommentAsync(mobileView.Id, seanComment);

        Ticket lastProjectMissing = _context.Tickets.Where(t => t.Title == "Last project missing").First();

        TicketComment amandaComment3 = new TicketComment()
        {
            TicketId = lastProjectMissing.Id,
            AppUserId = amandaGallup.Id,
            Created = lastProjectMissing.Created + new TimeSpan(days: 0, hours: 16, minutes: 17, seconds: 0),
            Comment = "We let Shawn code this part and it's proving...interesting...to track down. Still working on it!",
        };

        TicketComment seanComment2 = new TicketComment()
        {
            TicketId = lastProjectMissing.Id,
            AppUserId = shawnBlankenship.Id,
            Created = lastProjectMissing.Created + new TimeSpan(days: 0, hours: 18, minutes: 12, seconds: 0),
            Comment = "Haha sorry Amanda. Let me know if you need help understanding my code...although I'm not sure I understand it either!",
        };

        TicketComment amandaComment4 = new TicketComment()
        {
            TicketId = lastProjectMissing.Id,
            AppUserId = amandaGallup.Id,
            Created = lastProjectMissing.Created + new TimeSpan(days: 0, hours: 20, minutes: 12, seconds: 0),
            Comment = "Whew, ok I fixed it, and cleaned it up. @Shawn Blankenship take a look at the attachment I added on the DRY principle of coding ;)",
        };

        await _ticketService.AddTicketCommentAsync(lastProjectMissing.Id, amandaComment3);
        await _ticketService.AddTicketCommentAsync(lastProjectMissing.Id, seanComment2);
        await _ticketService.AddTicketCommentAsync(lastProjectMissing.Id, amandaComment4);
    }

    private static async Task SeedInventoryProjectCommentsAsync()
    {
        Ticket databaseSwap = _context.Tickets.Where(t => t.Title == "Database swap").First();

        TicketComment amandaComment = new TicketComment()
        {
            TicketId = databaseSwap.Id,
            AppUserId = amandaGallup.Id,
            Created = databaseSwap.Created + new TimeSpan(days: 0, hours: 4, minutes: 17, seconds: 0),
            Comment = "Since we're using dependency injection it shouldn't be that big of a deal at all...just need to change a few lines.",
        };

        await _ticketService.AddTicketCommentAsync(databaseSwap.Id, amandaComment);

        Ticket automaticReorder = _context.Tickets.Where(t => t.Title == "Automatic reorder feature").First();

        TicketComment amandaComment2 = new TicketComment()
        {
            TicketId = automaticReorder.Id,
            AppUserId = amandaGallup.Id,
            Created = automaticReorder.Created + new TimeSpan(days: 0, hours: 0, minutes: 12, seconds: 0),
            Comment = "I think I will need to rework the models a bit to make this work, so it might take a little extra time.",
        };

        TicketComment darrenComment = new TicketComment()
        {
            TicketId = automaticReorder.Id,
            AppUserId = darrenJohnson.Id,
            Created = automaticReorder.Created + new TimeSpan(days: 0, hours: 1, minutes: 38, seconds: 0),
            Comment = "Alright, no problem, thanks for the heads up.",
        };

        await _ticketService.AddTicketCommentAsync(automaticReorder.Id, amandaComment2);
        await _ticketService.AddTicketCommentAsync(automaticReorder.Id, darrenComment);

        Ticket purchasingViewFreeze = _context.Tickets.Where(t => t.Title == "Purchasing view occasionally freezing").First();

        TicketComment amandaComment3 = new TicketComment()
        {
            TicketId = purchasingViewFreeze.Id,
            AppUserId = amandaGallup.Id,
            Created = purchasingViewFreeze.Created + new TimeSpan(days: 0, hours: 11, minutes: 15, seconds: 0),
            Comment = "@Darren Johnson -- I have not been able to reproduce this. The code there is pretty simple and I cannot" +
            " see what the issue could be. Could we try to get more information from them to pin this down? Placing on hold.",
        };

        TicketComment darrenComment2 = new TicketComment()
        {
            TicketId = purchasingViewFreeze.Id,
            AppUserId = darrenJohnson.Id,
            Created = purchasingViewFreeze.Created + new TimeSpan(days: 0, hours: 12, minutes: 9, seconds: 0),
            Comment = "Ok, I couldn't get it to happen either. I'll bring it up in our meeting tomorrow and see if we can find out more.",
        };

        await _ticketService.AddTicketCommentAsync(purchasingViewFreeze.Id, amandaComment3);
        await _ticketService.AddTicketCommentAsync(purchasingViewFreeze.Id, darrenComment2);

        Ticket uiColors = _context.Tickets.Where(t => t.Title == "UI colors").First();

        TicketComment shawnComment = new TicketComment()
        {
            TicketId = uiColors.Id,
            AppUserId = shawnBlankenship.Id,
            Created = uiColors.Created + new TimeSpan(days: 2, hours: 3, minutes: 25, seconds: 0),
            Comment = "@Amanda Gallup I think I can handle this, I'll see if I can sneak it in today.",
        };

        TicketComment amandaComment4 = new TicketComment()
        {
            TicketId = uiColors.Id,
            AppUserId = amandaGallup.Id,
            Created = uiColors.Created + new TimeSpan(days: 2, hours: 3, minutes: 42, seconds: 0),
            Comment = "Thank you!",
        };

        await _ticketService.AddTicketCommentAsync(uiColors.Id, shawnComment);
        await _ticketService.AddTicketCommentAsync(uiColors.Id, amandaComment4);

        Ticket newColumn = _context.Tickets.Where(t => t.Title == "New report column").First();

        TicketComment amandaComment5 = new TicketComment()
        {
            TicketId = newColumn.Id,
            AppUserId = amandaGallup.Id,
            Created = newColumn.Created + new TimeSpan(days: 1, hours: 15, minutes: 59, seconds: 0),
            Comment = "Sounds easy in theory (famous last words!) Working on this now.",
        };

        await _ticketService.AddTicketCommentAsync(newColumn.Id, amandaComment5);
    }

    private static async Task SeedPortfolioProjectCommentsAsync()
    {
        Ticket portfolio = _context.Tickets.Where(t => t.Title == "Finish portfolio website").First();

        TicketComment hermanComment = new TicketComment()
        {
            TicketId = portfolio.Id,
            AppUserId = hermanCampos.Id,
            Created = portfolio.Created + new TimeSpan(days: 4, hours: 2, minutes: 39, seconds: 0),
            Comment = "Here you go. I think it turned out pretty good! https://jdwohl.up.railway.app",
        };

        await _ticketService.AddTicketCommentAsync(portfolio.Id, hermanComment);

        Ticket blog = _context.Tickets.Where(t => t.Title == "Blog posts").First();

        TicketComment hermanComment2 = new TicketComment()
        {
            TicketId = blog.Id,
            AppUserId = hermanCampos.Id,
            Created = blog.Created + new TimeSpan(days: 4, hours: 2, minutes: 39, seconds: 0),
            Comment = "I took some of the boss's book reviews and articles and put them up. https://jdwohl.up.railway.app/Blog",
        };

        await _ticketService.AddTicketCommentAsync(blog.Id, hermanComment2);
    }

    private static async Task SeedDietProjectCommentsAsync()
    {
        Ticket logo = _context.Tickets.Where(t => t.Title == "Logo placement").First();

        TicketComment amandaComment = new TicketComment
        {
            TicketId = logo.Id,
            AppUserId = amandaGallup.Id,
            Created = logo.Created + new TimeSpan(days: 0, hours: 16, minutes: 28, seconds: 0),
            Comment = "All done!",
        };

        await _ticketService.AddTicketCommentAsync(logo.Id, amandaComment);

        Ticket calories = _context.Tickets.Where(t => t.Title == "Calorie calculations, unit tests").First();

        TicketComment amandaComment2 = new TicketComment
        {
            TicketId = calories.Id,
            AppUserId = amandaGallup.Id,
            Created = calories.Created + new TimeSpan(days: 1, hours: 10, minutes: 21, seconds: 0),
            Comment = "Aha, I accidentally used Math.Log instead of Math.Log10. Who named these?! I'm blaming Microsoft! Math.Log should be Math.Ln... Anyway," +
            " I added an xUnit test project that confirms the calorie calculations for a wide range of users. It should catch any more problems.",
        };

        await _ticketService.AddTicketCommentAsync(calories.Id, amandaComment2);


        Ticket dataValidation = _context.Tickets.Where(t => t.Title == "Data validation").First();

        TicketComment amandaComment3 = new TicketComment
        {
            TicketId = dataValidation.Id,
            AppUserId = amandaGallup.Id,
            Created = dataValidation.Created + new TimeSpan(days: 0, hours: 2, minutes: 39, seconds: 0),
            Comment = "@William Casey -- check it out, the program displays errors if you leave a field blank or input wrong information now. You also can't" +
            " save changes until the errors are cleaned up."
        };

        TicketComment williamComment = new TicketComment
        {
            TicketId = dataValidation.Id,
            AppUserId = williamCasey.Id,
            Created = dataValidation.Created + new TimeSpan(days: 0, hours: 3, minutes: 0, seconds: 0),
            Comment = "Cool! That little change makes it feel like a real app now.",
        };

        await _ticketService.AddTicketCommentAsync(dataValidation.Id, amandaComment3);
        await _ticketService.AddTicketCommentAsync(dataValidation.Id, williamComment);

        Ticket importExport = _context.Tickets.Where(t => t.Title == "Import/export").First();

        TicketComment amandaComment4 = new TicketComment
        {
            TicketId = importExport.Id,
            AppUserId = amandaGallup.Id,
            Created = importExport.Created + new TimeSpan(days: 1, hours: 6, minutes: 38, seconds: 0),
            Comment = "This should be pretty simple, I'll see if I can squeeze it in today.",
        };

        await _ticketService.AddTicketCommentAsync(importExport.Id, amandaComment4);
    }

    private static async Task SeedDinerProjectAttachmentsAsync()
    {
        Ticket newContactInfo = _context.Tickets.Where(t => t.Title == "New contact info").First();
        Ticket menuPage = _context.Tickets.Where(t => t.Title == "Menu page").First();

        TicketAttachment exampleAttachment = new TicketAttachment()
        {
            TicketId = newContactInfo.Id,
            AppUserId = williamCasey.Id,
            Created = newContactInfo.Created + new TimeSpan(days: 0, hours: 0, minutes: 2, seconds: 0),
            Description = "Updated info",
            FileName = "Contact info.pdf",
        };

        TicketAttachment exampleAttachment2 = new TicketAttachment()
        {
            TicketId = menuPage.Id,
            AppUserId = williamCasey.Id,
            Created = menuPage.Created + new TimeSpan(days: 0, hours: 1, minutes: 17, seconds: 0),
            Description = "Updated menu",
            FileName = "New menu.pdf",
        };

        using (FileStream fs = File.OpenRead($"wwwroot/ExampleAttachment.pdf"))
        {
            using (var memoryStream = new MemoryStream())
            {
                fs.CopyTo(memoryStream);
                exampleAttachment.FileData = memoryStream.ToArray();
                exampleAttachment.FileType = "application/pdf";
                exampleAttachment2.FileData = memoryStream.ToArray();
                exampleAttachment2.FileType = "application/pdf";
            }
        }

        await _ticketService.AddTicketAttachmentAsync(newContactInfo.Id, exampleAttachment);
        await _ticketService.AddTicketAttachmentAsync(menuPage.Id, exampleAttachment2);
        await _ticketHistoryService.AddAttachmentEventAsync(newContactInfo, exampleAttachment);
        await _ticketHistoryService.AddAttachmentEventAsync(menuPage, exampleAttachment2);
    }

    private static async Task SeedProjectManagementProjectAttachmentsAsync()
    {
        Ticket ticketPageClarifications = _context.Tickets.Where(t => t.Title == "Ticket page clarifications").First();
        Ticket lastProjectMissing = _context.Tickets.Where(t => t.Title == "Last project missing").First();

        TicketAttachment exampleAttachment = new TicketAttachment()
        {
            TicketId = ticketPageClarifications.Id,
            AppUserId = amandaGallup.Id,
            Created = ticketPageClarifications.Created + new TimeSpan(days: 0, hours: 0, minutes: 1, seconds: 0),
            Description = "Questions email",
            FileName = "Email.pdf",
        };

        TicketAttachment exampleAttachment2 = new TicketAttachment()
        {
            TicketId = lastProjectMissing.Id,
            AppUserId = amandaGallup.Id,
            Created = lastProjectMissing.Created + new TimeSpan(days: 0, hours: 20, minutes: 9, seconds: 0),
            Description = "Some reading material for Shawn",
            FileName = "The DRY Principal.pdf",
        };

        using (FileStream fs = File.OpenRead($"wwwroot/ExampleAttachment.pdf"))
        {
            using (var memoryStream = new MemoryStream())
            {
                fs.CopyTo(memoryStream);
                exampleAttachment.FileData = memoryStream.ToArray();
                exampleAttachment.FileType = "application/pdf";
                exampleAttachment2.FileData = memoryStream.ToArray();
                exampleAttachment2.FileType = "application/pdf";
            }
        }

        await _ticketService.AddTicketAttachmentAsync(ticketPageClarifications.Id, exampleAttachment);
        await _ticketService.AddTicketAttachmentAsync(lastProjectMissing.Id, exampleAttachment2);
        await _ticketHistoryService.AddAttachmentEventAsync(ticketPageClarifications, exampleAttachment);
        await _ticketHistoryService.AddAttachmentEventAsync(lastProjectMissing, exampleAttachment2);
    }

    private static async Task SeedInventoryProjectAttachmentsAsync()
    {
        Ticket uiColors = _context.Tickets.Where(t => t.Title == "UI colors").First();
        Ticket crash = _context.Tickets.Where(t => t.Title == "Crash when deleting certain products").First();

        TicketAttachment exampleAttachment = new TicketAttachment()
        {
            TicketId = uiColors.Id,
            AppUserId = amandaGallup.Id,
            Created = uiColors.Created + new TimeSpan(days: 0, hours: 0, minutes: 3, seconds: 0),
            Description = "Replacement colors",
            FileName = "Colors.pdf",
        };

        TicketAttachment exampleAttachment2 = new TicketAttachment()
        {
            TicketId = crash.Id,
            AppUserId = darrenJohnson.Id,
            Created = crash.Created + new TimeSpan(days: 0, hours: 0, minutes: 1, seconds: 0),
            Description = "List of products",
            FileName = "Crashing products.pdf",
        };

        using (FileStream fs = File.OpenRead($"wwwroot/ExampleAttachment.pdf"))
        {
            using (var memoryStream = new MemoryStream())
            {
                fs.CopyTo(memoryStream);
                exampleAttachment.FileData = memoryStream.ToArray();
                exampleAttachment.FileType = "application/pdf";
                exampleAttachment2.FileData = memoryStream.ToArray();
                exampleAttachment2.FileType = "application/pdf";
            }
        }

        await _ticketService.AddTicketAttachmentAsync(uiColors.Id, exampleAttachment);
        await _ticketService.AddTicketAttachmentAsync(crash.Id, exampleAttachment2);
        await _ticketHistoryService.AddAttachmentEventAsync(uiColors, exampleAttachment);
        await _ticketHistoryService.AddAttachmentEventAsync(crash, exampleAttachment2);
    }

    private static async Task SeedDietProjectAttachmentsAsync()
    {
        Ticket logo = _context.Tickets.Where(t => t.Title == "Logo placement").First();
        Ticket calories = _context.Tickets.Where(t => t.Title == "Calorie calculations, unit tests").First();

        TicketAttachment exampleAttachment = new TicketAttachment()
        {
            TicketId = logo.Id,
            AppUserId = williamCasey.Id,
            Created = logo.Created + new TimeSpan(days: 0, hours: 0, minutes: 7, seconds: 0),
            Description = "Logo placement markup",
            FileName = "Logo location.png",
        };

        TicketAttachment exampleAttachment2 = new TicketAttachment()
        {
            TicketId = calories.Id,
            AppUserId = williamCasey.Id,
            Created = calories.Created + new TimeSpan(days: 0, hours: 0, minutes: 1, seconds: 0),
            Description = "User info",
            FileName = "Example.pdf",
        };

        using (FileStream fs = File.OpenRead($"wwwroot/ExampleAttachment.png"))
        {
            using (var memoryStream = new MemoryStream())
            {
                fs.CopyTo(memoryStream);
                exampleAttachment.FileData = memoryStream.ToArray();
                exampleAttachment.FileType = "image/png";
                exampleAttachment2.FileData = memoryStream.ToArray();
                exampleAttachment2.FileType = "application/pdf";
            }
        }

        await _ticketService.AddTicketAttachmentAsync(logo.Id, exampleAttachment);
        await _ticketService.AddTicketAttachmentAsync(calories.Id, exampleAttachment2);
        await _ticketHistoryService.AddAttachmentEventAsync(logo, exampleAttachment);
        await _ticketHistoryService.AddAttachmentEventAsync(calories, exampleAttachment2);
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

    private static async Task SeedDinerProjectHistoryItemsAsync()
    {
        Ticket presentationMeeting = _context.Tickets.Where(t => t.Title == "Presentation meeting").First();
        Ticket deadLinks = _context.Tickets.Where(t => t.Title == "Dead links").First();
        Ticket menuPage = _context.Tickets.Where(t => t.Title == "Menu page").First();

        TicketHistory statusChange = new TicketHistory()
        {
            TicketId = presentationMeeting.Id,
            AppUserId = hermanCampos.Id,
            Created = presentationMeeting.Created + new TimeSpan(days: 1, hours: 4, minutes: 8, seconds: 0),
            Description = $"{hermanCampos.FullName} made the following changes:</br><ul>",
        };

        statusChange.Description += $"<li>Changed the ticket status to <strong>Complete</strong></li>";
        statusChange.Description += "</ul>";

        TicketHistory archiveChange = new TicketHistory()
        {
            TicketId = presentationMeeting.Id,
            AppUserId = hermanCampos.Id,
            Created = presentationMeeting.Created + new TimeSpan(days: 1, hours: 4, minutes: 10, seconds: 0),
            Description = "Herman Campos archived the ticket."
        };

        await _ticketHistoryService.AddTicketHistoryItemAsync(statusChange);
        await _ticketHistoryService.AddTicketHistoryItemAsync(archiveChange);

        TicketHistory statusChange2 = new TicketHistory()
        {
            TicketId = deadLinks.Id,
            AppUserId = hermanCampos.Id,
            Created = deadLinks.Created + new TimeSpan(days: 0, hours: 4, minutes: 8, seconds: 0),
            Description = $"{hermanCampos.FullName} made the following changes:</br><ul>",
        };

        statusChange2.Description += $"<li>Changed the ticket status to <strong>In Development</strong></li>";
        statusChange2.Description += "</ul>";

        TicketHistory statusChange3 = new TicketHistory()
        {
            TicketId = deadLinks.Id,
            AppUserId = hermanCampos.Id,
            Created = deadLinks.Created + new TimeSpan(days: 0, hours: 4, minutes: 38, seconds: 0),
            Description = $"{hermanCampos.FullName} made the following changes:</br><ul>",
        };

        statusChange3.Description += $"<li>Changed the ticket status to <strong>Complete</strong></li>";
        statusChange3.Description += "</ul>";

        await _ticketHistoryService.AddTicketHistoryItemAsync(statusChange2);
        await _ticketHistoryService.AddTicketHistoryItemAsync(statusChange3);

        TicketHistory statusChange4 = new TicketHistory()
        {
            TicketId = menuPage.Id,
            AppUserId = hermanCampos.Id,
            Created = menuPage.Created + new TimeSpan(days: 1, hours: 1, minutes: 38, seconds: 0),
            Description = $"{hermanCampos.FullName} made the following changes:</br><ul>",
        };

        statusChange4.Description += $"<li>Changed the ticket status to <strong>In Development</strong></li>";
        statusChange4.Description += "</ul>";

        await _ticketHistoryService.AddTicketHistoryItemAsync(statusChange4);
    }

    private static async Task SeedProjectManagementProjectHistoryItemsAsync()
    {
        Ticket ticketPageClarifications = _context.Tickets.Where(t => t.Title == "Ticket page clarifications").First();

        TicketHistory statusChange = new TicketHistory()
        {
            TicketId = ticketPageClarifications.Id,
            AppUserId = amandaGallup.Id,
            Created = ticketPageClarifications.Created + new TimeSpan(days: 0, hours: 0, minutes: 2, seconds: 0),
            Description = $"{amandaGallup.FullName} made the following changes:</br><ul>",
        };

        statusChange.Description += $"<li>Changed the ticket status to <strong>In Development</strong></li>";
        statusChange.Description += "</ul>";

        TicketHistory statusChange2 = new TicketHistory()
        {
            TicketId = ticketPageClarifications.Id,
            AppUserId = amandaGallup.Id,
            Created = ticketPageClarifications.Created + new TimeSpan(days: 1, hours: 3, minutes: 2, seconds: 0),
            Description = $"{amandaGallup.FullName} made the following changes:</br><ul>",
        };

        statusChange2.Description += $"<li>Changed the ticket status to <strong>Complete</strong></li>";
        statusChange2.Description += "</ul>";

        TicketHistory archiveChange = new TicketHistory()
        {
            TicketId = ticketPageClarifications.Id,
            AppUserId = amandaGallup.Id,
            Created = ticketPageClarifications.Created + new TimeSpan(days: 1, hours: 6, minutes: 15, seconds: 0),
            Description = "Amanda Gallup archived the ticket."
        };

        await _ticketHistoryService.AddTicketHistoryItemAsync(statusChange);
        await _ticketHistoryService.AddTicketHistoryItemAsync(statusChange2);
        await _ticketHistoryService.AddTicketHistoryItemAsync(archiveChange);

        Ticket pdfGeneration = _context.Tickets.Where(t => t.Title == "PDF generation").First();

        TicketHistory statusChange3 = new TicketHistory()
        {
            TicketId = pdfGeneration.Id,
            AppUserId = hermanCampos.Id,
            Created = pdfGeneration.Created + new TimeSpan(days: 0, hours: 0, minutes: 2, seconds: 0),
            Description = $"{hermanCampos.FullName} made the following changes:</br><ul>",
        };

        statusChange3.Description += $"<li>Changed the ticket status to <strong>In Development</strong></li>";
        statusChange3.Description += "</ul>";

        TicketHistory statusChange4 = new TicketHistory()
        {
            TicketId = pdfGeneration.Id,
            AppUserId = hermanCampos.Id,
            Created = pdfGeneration.Created + new TimeSpan(days: 2, hours: 0, minutes: 0, seconds: 0),
            Description = $"{hermanCampos.FullName} made the following changes:</br><ul>",
        };

        statusChange4.Description += $"<li>Changed the ticket status to <strong>Complete</strong></li>";
        statusChange4.Description += "</ul>";

        await _ticketHistoryService.AddTicketHistoryItemAsync(statusChange3);
        await _ticketHistoryService.AddTicketHistoryItemAsync(statusChange4);

        Ticket newView = _context.Tickets.Where(t => t.Title == "New view - employee workload").First();

        TicketHistory statusChange5 = new TicketHistory()
        {
            TicketId = newView.Id,
            AppUserId = amandaGallup.Id,
            Created = newView.Created + new TimeSpan(days: 0, hours: 3, minutes: 47, seconds: 0),
            Description = $"{amandaGallup.FullName} made the following changes:</br><ul>",
        };

        statusChange5.Description += $"<li>Changed the ticket status to <strong>In Development</strong></li>";
        statusChange5.Description += "</ul>";

        TicketHistory statusChange6 = new TicketHistory()
        {
            TicketId = newView.Id,
            AppUserId = amandaGallup.Id,
            Created = newView.Created + new TimeSpan(days: 0, hours: 3, minutes: 52, seconds: 0),
            Description = $"{amandaGallup.FullName} made the following changes:</br><ul>",
        };

        statusChange6.Description += $"<li>Changed the ticket status to <strong>On Hold</strong></li>";
        statusChange6.Description += "</ul>";

        await _ticketHistoryService.AddTicketHistoryItemAsync(statusChange5);
        await _ticketHistoryService.AddTicketHistoryItemAsync(statusChange6);

        Ticket mobileView = _context.Tickets.Where(t => t.Title == "Fix mobile view").First();

        TicketHistory statusChange7 = new TicketHistory()
        {
            TicketId = mobileView.Id,
            AppUserId = hermanCampos.Id,
            Created = mobileView.Created + new TimeSpan(days: 0, hours: 9, minutes: 37, seconds: 0),
            Description = $"{hermanCampos.FullName} made the following changes:</br><ul>",
        };

        statusChange7.Description += $"<li>Changed the ticket status to <strong>In Development</strong></li>";
        statusChange7.Description += "</ul>";

        await _ticketHistoryService.AddTicketHistoryItemAsync(statusChange7);

        Ticket lastProjectMissing = _context.Tickets.Where(t => t.Title == "Last project missing").First();

        TicketHistory statusChange8 = new TicketHistory()
        {
            TicketId = lastProjectMissing.Id,
            AppUserId = amandaGallup.Id,
            Created = lastProjectMissing.Created + new TimeSpan(days: 0, hours: 4, minutes: 2, seconds: 0),
            Description = $"{amandaGallup.FullName} made the following changes:</br><ul>",
        };

        statusChange8.Description += $"<li>Changed the ticket status to <strong>In Development</strong></li>";
        statusChange8.Description += "</ul>";

        await _ticketHistoryService.AddTicketHistoryItemAsync(statusChange8);
    }

    private static async Task SeedInventoryProjectHistoryItemsAsync()
    {
        Ticket databaseSwap = _context.Tickets.Where(t => t.Title == "Database swap").First();

        TicketHistory statusChange = new TicketHistory()
        {
            TicketId = databaseSwap.Id,
            AppUserId = amandaGallup.Id,
            Created = databaseSwap.Created + new TimeSpan(days: 0, hours: 4, minutes: 23, seconds: 0),
            Description = $"{amandaGallup.FullName} made the following changes:</br><ul>",
        };

        statusChange.Description += $"<li>Changed the ticket status to <strong>In Development</strong></li>";
        statusChange.Description += "</ul>";

        TicketHistory statusChange2 = new TicketHistory()
        {
            TicketId = databaseSwap.Id,
            AppUserId = amandaGallup.Id,
            Created = databaseSwap.Created + new TimeSpan(days: 0, hours: 5, minutes: 3, seconds: 0),
            Description = $"{amandaGallup.FullName} made the following changes:</br><ul>",
        };

        statusChange2.Description += $"<li>Changed the ticket status to <strong>Complete</strong></li>";
        statusChange2.Description += "</ul>";

        TicketHistory archiveChange = new TicketHistory()
        {
            TicketId = databaseSwap.Id,
            AppUserId = amandaGallup.Id,
            Created = databaseSwap.Created + new TimeSpan(days: 1, hours: 2, minutes: 15, seconds: 0),
            Description = "Amanda Gallup archived the ticket."
        };

        await _ticketHistoryService.AddTicketHistoryItemAsync(statusChange);
        await _ticketHistoryService.AddTicketHistoryItemAsync(statusChange2);
        await _ticketHistoryService.AddTicketHistoryItemAsync(archiveChange);

        Ticket automaticReorder = _context.Tickets.Where(t => t.Title == "Automatic reorder feature").First();

        TicketHistory statusChange3 = new TicketHistory()
        {
            TicketId = automaticReorder.Id,
            AppUserId = amandaGallup.Id,
            Created = automaticReorder.Created + new TimeSpan(days: 0, hours: 0, minutes: 13, seconds: 0),
            Description = $"{amandaGallup.FullName} made the following changes:</br><ul>",
        };

        statusChange3.Description += $"<li>Changed the ticket status to <strong>In Development</strong></li>";
        statusChange3.Description += "</ul>";

        TicketHistory statusChange4 = new TicketHistory()
        {
            TicketId = automaticReorder.Id,
            AppUserId = amandaGallup.Id,
            Created = automaticReorder.Created + new TimeSpan(days: 1, hours: 6, minutes: 19, seconds: 0),
            Description = $"{amandaGallup.FullName} made the following changes:</br><ul>",
        };

        statusChange4.Description += $"<li>Changed the ticket status to <strong>Complete</strong></li>";
        statusChange4.Description += "</ul>";

        await _ticketHistoryService.AddTicketHistoryItemAsync(statusChange3);
        await _ticketHistoryService.AddTicketHistoryItemAsync(statusChange4);

        Ticket purchasingViewFreeze = _context.Tickets.Where(t => t.Title == "Purchasing view occasionally freezing").First();

        TicketHistory statusChange5 = new TicketHistory()
        {
            TicketId = purchasingViewFreeze.Id,
            AppUserId = amandaGallup.Id,
            Created = purchasingViewFreeze.Created + new TimeSpan(days: 1, hours: 9, minutes: 35, seconds: 0),
            Description = $"{amandaGallup.FullName} made the following changes:</br><ul>",
        };

        statusChange5.Description += $"<li>Changed the ticket status to <strong>In Development</strong></li>";
        statusChange5.Description += "</ul>";

        TicketHistory statusChange6 = new TicketHistory()
        {
            TicketId = purchasingViewFreeze.Id,
            AppUserId = amandaGallup.Id,
            Created = purchasingViewFreeze.Created + new TimeSpan(days: 1, hours: 11, minutes: 14, seconds: 0),
            Description = $"{amandaGallup.FullName} made the following changes:</br><ul>",
        };

        statusChange6.Description += $"<li>Changed the ticket status to <strong>On Hold</strong></li>";
        statusChange6.Description += "</ul>";

        await _ticketHistoryService.AddTicketHistoryItemAsync(statusChange5);
        await _ticketHistoryService.AddTicketHistoryItemAsync(statusChange6);

        Ticket newColumn = _context.Tickets.Where(t => t.Title == "New report column").First();

        TicketHistory statusChange7 = new TicketHistory()
        {
            TicketId = newColumn.Id,
            AppUserId = amandaGallup.Id,
            Created = newColumn.Created + new TimeSpan(days: 1, hours: 15, minutes: 1, seconds: 0),
            Description = $"{amandaGallup.FullName} made the following changes:</br><ul>",
        };

        statusChange7.Description += $"<li>Changed the ticket status to <strong>In Development</strong></li>";
        statusChange7.Description += "</ul>";

        await _ticketHistoryService.AddTicketHistoryItemAsync(statusChange7);

        Ticket crash = _context.Tickets.Where(t => t.Title == "Crash when deleting certain products").First();

        TicketHistory statusChange8 = new TicketHistory()
        {
            TicketId = crash.Id,
            AppUserId = amandaGallup.Id,
            Created = crash.Created + new TimeSpan(days: 0, hours: 6, minutes: 11, seconds: 0),
            Description = $"{amandaGallup.FullName} made the following changes:</br><ul>",
        };

        statusChange8.Description += $"<li>Changed the ticket status to <strong>In Development</strong></li>";
        statusChange8.Description += "</ul>";

        await _ticketHistoryService.AddTicketHistoryItemAsync(statusChange8);
    }

    private static async Task SeedPortfolioProjectHistoryItemsAsync()
    {
        Ticket portfolio = _context.Tickets.Where(t => t.Title == "Finish portfolio website").First();

        TicketHistory statusChange = new TicketHistory()
        {
            TicketId = portfolio.Id,
            AppUserId = hermanCampos.Id,
            Created = portfolio.Created + new TimeSpan(days: 1, hours: 18, minutes: 23, seconds: 0),
            Description = $"{hermanCampos.FullName} made the following changes:</br><ul>",
        };

        statusChange.Description += $"<li>Changed the ticket status to <strong>In Development</strong></li>";
        statusChange.Description += "</ul>";

        TicketHistory statusChange2 = new TicketHistory()
        {
            TicketId = portfolio.Id,
            AppUserId = hermanCampos.Id,
            Created = portfolio.Created + new TimeSpan(days: 4, hours: 1, minutes: 11, seconds: 0),
            Description = $"{hermanCampos.FullName} made the following changes:</br><ul>",
        };

        statusChange2.Description += $"<li>Changed the ticket status to <strong>Complete</strong></li>";
        statusChange2.Description += "</ul>";

        TicketHistory archiveChange = new TicketHistory()
        {
            TicketId = portfolio.Id,
            AppUserId = hermanCampos.Id,
            Created = portfolio.Created + new TimeSpan(days: 4, hours: 1, minutes: 13, seconds: 0),
            Description = $"{hermanCampos.FullName} archived the ticket."
        };

        await _ticketHistoryService.AddTicketHistoryItemAsync(statusChange);
        await _ticketHistoryService.AddTicketHistoryItemAsync(statusChange2);
        await _ticketHistoryService.AddTicketHistoryItemAsync(archiveChange);

        Ticket blog = _context.Tickets.Where(t => t.Title == "Blog posts").First();

        TicketHistory statusChange3 = new TicketHistory()
        {
            TicketId = blog.Id,
            AppUserId = hermanCampos.Id,
            Created = blog.Created + new TimeSpan(days: 1, hours: 10, minutes: 30, seconds: 0),
            Description = $"{hermanCampos.FullName} made the following changes:</br><ul>",
        };

        statusChange3.Description += $"<li>Changed the ticket status to <strong>In Development</strong></li>";
        statusChange3.Description += "</ul>";

        TicketHistory statusChange4 = new TicketHistory()
        {
            TicketId = blog.Id,
            AppUserId = hermanCampos.Id,
            Created = blog.Created + new TimeSpan(days: 2, hours: 1, minutes: 17, seconds: 0),
            Description = $"{hermanCampos.FullName} made the following changes:</br><ul>",
        };

        statusChange4.Description += $"<li>Changed the ticket status to <strong>Complete</strong></li>";
        statusChange4.Description += "</ul>";

        await _ticketHistoryService.AddTicketHistoryItemAsync(statusChange3);
        await _ticketHistoryService.AddTicketHistoryItemAsync(statusChange4);
    }

    private static async Task SeedDietProjectHistoryItemsAsync()
    {
        Ticket logo = _context.Tickets.Where(t => t.Title == "Logo placement").First();

        TicketHistory statusChange = new TicketHistory()
        {
            TicketId = logo.Id,
            AppUserId = amandaGallup.Id,
            Created = logo.Created + new TimeSpan(days: 0, hours: 3, minutes: 53, seconds: 0),
            Description = $"{amandaGallup.FullName} made the following changes:</br><ul>",
        };

        statusChange.Description += $"<li>Changed the ticket status to <strong>In Development</strong></li>";
        statusChange.Description += "</ul>";

        TicketHistory statusChange2 = new TicketHistory()
        {
            TicketId = logo.Id,
            AppUserId = amandaGallup.Id,
            Created = logo.Created + new TimeSpan(days: 0, hours: 4, minutes: 1, seconds: 0),
            Description = $"{amandaGallup.FullName} made the following changes:</br><ul>",
        };

        statusChange2.Description += $"<li>Changed the ticket status to <strong>Complete</strong></li>";
        statusChange2.Description += "</ul>";

        TicketHistory archiveChange = new TicketHistory()
        {
            TicketId = logo.Id,
            AppUserId = amandaGallup.Id,
            Created = logo.Created + new TimeSpan(days: 0, hours: 4, minutes: 3, seconds: 0),
            Description = $"{amandaGallup.FullName} archived the ticket."
        };

        await _ticketHistoryService.AddTicketHistoryItemAsync(statusChange);
        await _ticketHistoryService.AddTicketHistoryItemAsync(statusChange2);
        await _ticketHistoryService.AddTicketHistoryItemAsync(archiveChange);

        Ticket calories = _context.Tickets.Where(t => t.Title == "Calorie calculations, unit tests").First();

        TicketHistory statusChange3 = new TicketHistory()
        {
            TicketId = calories.Id,
            AppUserId = amandaGallup.Id,
            Created = calories.Created + new TimeSpan(days: 1, hours: 7, minutes: 16, seconds: 0),
            Description = $"{amandaGallup.FullName} made the following changes:</br><ul>",
        };

        statusChange3.Description += $"<li>Changed the ticket status to <strong>In Development</strong></li>";
        statusChange3.Description += "</ul>";

        TicketHistory statusChange4 = new TicketHistory()
        {
            TicketId = calories.Id,
            AppUserId = amandaGallup.Id,
            Created = calories.Created + new TimeSpan(days: 1, hours: 10, minutes: 19, seconds: 0),
            Description = $"{amandaGallup.FullName} made the following changes:</br><ul>",
        };

        statusChange4.Description += $"<li>Changed the ticket status to <strong>Complete</strong></li>";
        statusChange4.Description += "</ul>";

        TicketHistory archiveChange2 = new TicketHistory()
        {
            TicketId = calories.Id,
            AppUserId = amandaGallup.Id,
            Created = calories.Created + new TimeSpan(days: 1, hours: 10, minutes: 37, seconds: 0),
            Description = $"{amandaGallup.FullName} archived the ticket."
        };

        await _ticketHistoryService.AddTicketHistoryItemAsync(statusChange3);
        await _ticketHistoryService.AddTicketHistoryItemAsync(statusChange4);
        await _ticketHistoryService.AddTicketHistoryItemAsync(archiveChange2);

        Ticket dataValidation = _context.Tickets.Where(t => t.Title == "Data validation").First();

        TicketHistory statusChange5 = new TicketHistory()
        {
            TicketId = dataValidation.Id,
            AppUserId = amandaGallup.Id,
            Created = dataValidation.Created + new TimeSpan(days: 0, hours: 0, minutes: 11, seconds: 0),
            Description = $"{amandaGallup.FullName} made the following changes:</br><ul>",
        };

        statusChange5.Description += $"<li>Changed the ticket status to <strong>In Development</strong></li>";
        statusChange5.Description += "</ul>";

        TicketHistory statusChange6 = new TicketHistory()
        {
            TicketId = dataValidation.Id,
            AppUserId = amandaGallup.Id,
            Created = dataValidation.Created + new TimeSpan(days: 0, hours: 2, minutes: 32, seconds: 0),
            Description = $"{amandaGallup.FullName} made the following changes:</br><ul>",
        };

        statusChange6.Description += $"<li>Changed the ticket status to <strong>Complete</strong></li>";
        statusChange6.Description += "</ul>";

        TicketHistory archiveChange3 = new TicketHistory()
        {
            TicketId = dataValidation.Id,
            AppUserId = amandaGallup.Id,
            Created = dataValidation.Created + new TimeSpan(days: 0, hours: 2, minutes: 37, seconds: 0),
            Description = $"{amandaGallup.FullName} archived the ticket."
        };

        await _ticketHistoryService.AddTicketHistoryItemAsync(statusChange5);
        await _ticketHistoryService.AddTicketHistoryItemAsync(statusChange6);
        await _ticketHistoryService.AddTicketHistoryItemAsync(archiveChange3);

        Ticket importExport = _context.Tickets.Where(t => t.Title == "Import/export").First();

        TicketHistory statusChange7 = new TicketHistory()
        {
            TicketId = importExport.Id,
            AppUserId = amandaGallup.Id,
            Created = importExport.Created + new TimeSpan(days: 1, hours: 9, minutes: 22, seconds: 0),
            Description = $"{amandaGallup.FullName} made the following changes:</br><ul>",
        };

        statusChange7.Description += $"<li>Changed the ticket status to <strong>In Development</strong></li>";
        statusChange7.Description += "</ul>";

        TicketHistory statusChange8 = new TicketHistory()
        {
            TicketId = importExport.Id,
            AppUserId = amandaGallup.Id,
            Created = importExport.Created + new TimeSpan(days: 1, hours: 11, minutes: 40, seconds: 0),
            Description = $"{amandaGallup.FullName} made the following changes:</br><ul>",
        };

        statusChange8.Description += $"<li>Changed the ticket status to <strong>Complete</strong></li>";
        statusChange8.Description += "</ul>";

        TicketHistory archiveChange4 = new TicketHistory()
        {
            TicketId = importExport.Id,
            AppUserId = amandaGallup.Id,
            Created = importExport.Created + new TimeSpan(days: 1, hours: 12, minutes: 58, seconds: 0),
            Description = $"{amandaGallup.FullName} archived the ticket."
        };

        await _ticketHistoryService.AddTicketHistoryItemAsync(statusChange7);
        await _ticketHistoryService.AddTicketHistoryItemAsync(statusChange8);
        await _ticketHistoryService.AddTicketHistoryItemAsync(archiveChange4);
    }
}

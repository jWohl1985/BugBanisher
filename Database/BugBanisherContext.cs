﻿using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using BugBanisher.Database.Configuration;
using BugBanisher.Models;

namespace BugBanisher.Database;

public class BugBanisherContext : IdentityDbContext<AppUser>
{
    public BugBanisherContext(DbContextOptions<BugBanisherContext> options) : base(options)
    {

    }

    public DbSet<Company> Companies { get; set; } = default!;
    public DbSet<Project> Projects { get; set; } = default!;
    public DbSet<Ticket> Tickets { get; set; } = default!;
    public DbSet<TicketPriority> TicketPriorities { get; set; } = default!;
    public DbSet<TicketType> TicketTypes { get; set; } = default!;
    public DbSet<TicketStatus> TicketStatuses { get; set; } = default!;
    public DbSet<Notification> Notifications { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfiguration(new SeedCompanies());
        modelBuilder.ApplyConfiguration(new SeedProjects());
        modelBuilder.ApplyConfiguration(new SeedTicketPriorities());
        modelBuilder.ApplyConfiguration(new SeedTicketTypes());
        modelBuilder.ApplyConfiguration(new SeedTicketStatuses());

        modelBuilder.Entity<TicketAttachment>()
            .HasOne<Ticket>(ta => ta.Ticket)
            .WithMany(t => t.Attachments)
            .HasForeignKey(ta => ta.TicketId)
            .OnDelete(DeleteBehavior.Restrict);

		modelBuilder.Entity<TicketComment>()
			.HasOne<Ticket>(ta => ta.Ticket)
			.WithMany(t => t.Comments)
			.HasForeignKey(ta => ta.TicketId)
			.OnDelete(DeleteBehavior.Restrict);

		modelBuilder.Entity<TicketHistory>()
			.HasOne<Ticket>(ta => ta.Ticket)
			.WithMany(t => t.History)
			.HasForeignKey(ta => ta.TicketId)
			.OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Project>()
            .HasMany(p => p.Team) // AppUsers
            .WithMany(t => t.Projects)
            .UsingEntity(j => j.ToTable("ProjectUsers"));
    }
}

using Bug_Ticketing_System;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

public class BugTicketingSystemContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
{
    public BugTicketingSystemContext(DbContextOptions<BugTicketingSystemContext> options)
        : base(options) { }

    public DbSet<Bug> Bugs { get; set; }
    public DbSet<Project> Projects { get; set; }
    public DbSet<Attachment> Attachments { get; set; }
    public DbSet<BugAssignee> BugAssignees { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<BugAssignee>()
            .HasKey(ba => new { ba.BugId, ba.UserId });

        modelBuilder.Entity<Bug>()
            .HasOne(b => b.Reporter)
            .WithMany(u => u.ReportedBugs)
            .HasForeignKey(b => b.ReporterId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Project>()
            .HasMany(p => p.Bugs)
            .WithOne(b => b.Project)
            .HasForeignKey(b => b.ProjectId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Attachment>()
            .HasOne(a => a.UploadedByUser)
            .WithMany(u => u.Attachments)
            .HasForeignKey(a => a.UploadedBy)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();
    }
}
﻿using Capstone.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace Capstone.DataAccess
{
    public class CapstoneContext : DbContext
    {
        public CapstoneContext(DbContextOptions<CapstoneContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Attachment>().HasKey(sc => new { sc.AttachmentId });

            modelBuilder.Entity<Attachment>()
                .HasOne(tc => tc.User)
                .WithMany(wi => wi.Attachments)
                .HasForeignKey(tc => tc.CreateBy)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Attachment>()
                .HasOne(sc => sc.TaskComment)
                .WithMany(s => s.Attachments)
                .HasForeignKey(sc => sc.CommentId);

            modelBuilder.Entity<Attachment>()
                .HasOne(sc => sc.Ticket)
                .WithMany(s => s.Attachments)
                .HasForeignKey(sc => sc.TicketId);

            modelBuilder.Entity<TicketHistory>().HasKey(sc => new { sc.HistoryId });

            modelBuilder.Entity<TicketHistory>()
                .HasOne(tc => tc.Ticket)
                .WithMany(wi => wi.TaskHistories)
                .HasForeignKey(tc => tc.TicketId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TicketComment>().HasKey(sc => new { sc.CommentId });

            modelBuilder.Entity<TicketComment>()
                .HasOne(tc => tc.Ticket)
                .WithMany(wi => wi.TaskComments)
                .HasForeignKey(tc => tc.TaskId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ProjectMember>().HasKey(sc => new { sc.MemberId });

            modelBuilder.Entity<ProjectMember>()
                .HasOne(sc => sc.Project)
                .WithMany(s => s.ProjectMembers)
                .HasForeignKey(sc => sc.ProjectId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ProjectMember>()
                .HasOne(sc => sc.Users)
                .WithMany(s => s.ProjectMember)
                .HasForeignKey(sc => sc.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ProjectMember>()
                .HasOne(sc => sc.Role)
                .WithMany(s => s.ProjectMember)
                .HasForeignKey(sc => sc.RoleId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TicketType>().HasKey(sc => new { sc.TypeId });

            modelBuilder.Entity<TicketType>()
                .HasMany(sc => sc.Tickets)
                .WithOne(s => s.TicketType)
                .HasForeignKey(sc => sc.TypeId);

            modelBuilder.Entity<Ticket>().HasKey(sc => new { sc.TicketId });

            modelBuilder.Entity<Ticket>()
                .HasOne(sc => sc.PriorityLevel)
                .WithMany(s => s.Tickets)
                .HasForeignKey(sc => sc.PriorityId);

            modelBuilder.Entity<Ticket>()
                .HasOne(sc => sc.Status)
                .WithMany(s => s.Tickets)
                .HasForeignKey(sc => sc.TicketId);
            
            modelBuilder.Entity<Ticket>()
                .HasOne(sc => sc.ProjectMember)
                .WithMany(s => s.Tickets)
                .HasForeignKey(sc => sc.CreateBy);
            
            modelBuilder.Entity<Ticket>()
                .HasOne(sc => sc.ProjectMember)
                .WithMany(s => s.Tickets)
                .HasForeignKey(sc => sc.AssignTo);

            modelBuilder.Entity<TicketHistory>().HasKey(sc => new { sc.HistoryId });

            modelBuilder.Entity<TicketHistory>()
                .HasOne(sc => sc.Ticket)
                .WithMany(s => s.TaskHistories)
                .HasForeignKey(sc => sc.TicketId);

            modelBuilder.Entity<Notification>().HasKey(sc => new { sc.NotificationId });

            modelBuilder.Entity<Notification>()
                .HasOne(sc => sc.User)
                .WithMany(s => s.Notifications)
                .HasForeignKey(sc => sc.RecerverId);

            modelBuilder.Entity<Board>().HasKey(sc => new { sc.BoardId });

            modelBuilder.Entity<Board>()
                .HasOne(sc => sc.Project)
                .WithOne(s => s.Board)
                .HasForeignKey<Project>(sc => sc.ProjectId);

            modelBuilder.Entity<Interation>().HasKey(sc => new { sc.InterationId });

            modelBuilder.Entity<Interation>()
                .HasOne(sc => sc.Board)
                .WithMany(s => s.Interations)
                .HasForeignKey(sc => sc.BoardId);

            modelBuilder.Entity<Interation>()
                .HasMany(sc => sc.Tickets)
                .WithOne(s => s.Interation)
                .HasForeignKey(sc => sc.InterationId);

            modelBuilder.Entity<SchemaPermission>().HasKey(sc => new { sc.RoleId, sc.PermissionId, sc.SchemaId });

            modelBuilder.Entity<SchemaPermission>()
                .HasOne(sc => sc.Schema)
                .WithMany(s => s.SchemaPermissions)
                .HasForeignKey(sc => sc.SchemaId);

            modelBuilder.Entity<SchemaPermission>()
                .HasOne(sc => sc.Permission)
                .WithMany(s => s.SchemaPermissions)
                .HasForeignKey(sc => sc.PermissionId);

            modelBuilder.Entity<SchemaPermission>()
                .HasOne(sc => sc.Role)
                .WithMany(s => s.SchemaPermissions)
                .HasForeignKey(sc => sc.RoleId);
            
            modelBuilder.Entity<Status>().HasKey(sc => new { sc.StatusId});
            
            modelBuilder.Entity<Status>()
                .HasMany(sc => sc.Boards)
                .WithOne(s => s.Status)
                .HasForeignKey(sc => sc.StatusId)
                .OnDelete(DeleteBehavior.NoAction);

			modelBuilder.Entity<Status>()
                .HasMany(sc => sc.Interations)
                .WithOne(s => s.Status)
                .HasForeignKey(sc => sc.StatusId)
				.OnDelete(DeleteBehavior.NoAction);

			modelBuilder.Entity<Status>()
				.HasMany(sc => sc.Projects)
				.WithOne(s => s.Status)
				.HasForeignKey(sc => sc.StatusId)
				.OnDelete(DeleteBehavior.NoAction);
            
            modelBuilder.Entity<Status>()
                .HasMany(sc => sc.Users)
                .WithOne(s => s.Status)
                .HasForeignKey(sc => sc.StatusId)
                .OnDelete(DeleteBehavior.NoAction);
            
            modelBuilder.Entity<Status>()
                .HasMany(sc => sc.Tickets)
                .WithOne(s => s.Status)
                .HasForeignKey(sc => sc.StatusId)
                .OnDelete(DeleteBehavior.NoAction);
		}

		public DbSet<Attachment>? Attachments { get; set; }
        public DbSet<Board>? Boards { get; set; }
        public DbSet<Notification>? Notifications { get; set; }
        public DbSet<Permission>? Permissions { get; set; }
        public DbSet<Schema>? PermissionSchemas { get; set; }
        public DbSet<PriorityLevel>? PriorityLevels { get; set; }
        public DbSet<Project>? Projects { get; set; }
        public DbSet<Interation>? Interations { get; set; }
        public DbSet<ProjectMember>? ProjectMembers { get; set; }
        public DbSet<SchemaPermission>? SchemaPermissions { get; set; }
        public DbSet<Role>? Roles { get; set; }
        public DbSet<Ticket>? Tickets { get; set; }
        public DbSet<Schema>? Schemas { get; set; }
        public DbSet<TicketComment>? TicketComments { get; set; }
        public DbSet<TicketHistory>? TicketHistories { get; set; }
        public DbSet<Status>? Status { get; set; }
        public DbSet<TicketType>? TicketTypes { get; set; }
        public DbSet<User>? Users { get; set; }
    }
}
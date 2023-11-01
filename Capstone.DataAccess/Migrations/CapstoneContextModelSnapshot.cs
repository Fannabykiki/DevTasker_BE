﻿// <auto-generated />
using System;
using Capstone.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Capstone.DataAccess.Migrations
{
    [DbContext(typeof(CapstoneContext))]
    partial class CapstoneContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.15")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("Capstone.DataAccess.Entities.Attachment", b =>
                {
                    b.Property<Guid>("AttachmentId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("CommentId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreateAt")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("CreateBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("DeleteAt")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("TicketId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("AttachmentId");

                    b.HasIndex("CommentId");

                    b.HasIndex("CreateBy");

                    b.HasIndex("TicketId");

                    b.ToTable("Attachments");
                });

            modelBuilder.Entity("Capstone.DataAccess.Entities.Board", b =>
                {
                    b.Property<Guid>("BoardId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreateAt")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("DeleteAt")
                        .HasColumnType("datetime2");

                    b.Property<Guid?>("ProjectId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("StatusId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("UpdateAt")
                        .HasColumnType("datetime2");

                    b.HasKey("BoardId");

                    b.HasIndex("StatusId");

                    b.ToTable("Boards");
                });

            modelBuilder.Entity("Capstone.DataAccess.Entities.Interation", b =>
                {
                    b.Property<Guid>("InterationId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("BoardId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("EndDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("InterationName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("StartDate")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("StatusId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("InterationId");

                    b.HasIndex("BoardId");

                    b.HasIndex("StatusId");

                    b.ToTable("Interations");
                });

            modelBuilder.Entity("Capstone.DataAccess.Entities.Notification", b =>
                {
                    b.Property<Guid>("NotificationId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreateAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsRead")
                        .HasColumnType("bit");

                    b.Property<Guid>("RecerverId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("TargetUrl")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Title")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("NotificationId");

                    b.HasIndex("RecerverId");

                    b.ToTable("Notifications");
                });

            modelBuilder.Entity("Capstone.DataAccess.Entities.Permission", b =>
                {
                    b.Property<Guid>("PermissionId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("PermissionId");

                    b.ToTable("Permissions");
                });

            modelBuilder.Entity("Capstone.DataAccess.Entities.PriorityLevel", b =>
                {
                    b.Property<Guid>("LevelId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("Level")
                        .HasColumnType("int");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("LevelId");

                    b.ToTable("PriorityLevels");
                });

            modelBuilder.Entity("Capstone.DataAccess.Entities.Project", b =>
                {
                    b.Property<Guid>("ProjectId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreateAt")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("CreateBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("DeleteAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("EndDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("ExpireAt")
                        .HasColumnType("datetime2");

                    b.Property<bool>("PrivacyStatus")
                        .HasColumnType("bit");

                    b.Property<string>("ProjectName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("SchemasId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("StartDate")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("StatusId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("ProjectId");

                    b.HasIndex("SchemasId");

                    b.HasIndex("StatusId");

                    b.ToTable("Projects");
                });

            modelBuilder.Entity("Capstone.DataAccess.Entities.ProjectMember", b =>
                {
                    b.Property<Guid>("MemberId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("IsOwner")
                        .HasColumnType("bit");

                    b.Property<Guid>("ProjectId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("RoleId")
                        .IsRequired()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("MemberId");

                    b.HasIndex("ProjectId");

                    b.HasIndex("RoleId");

                    b.HasIndex("UserId");

                    b.ToTable("ProjectMembers");
                });

            modelBuilder.Entity("Capstone.DataAccess.Entities.Role", b =>
                {
                    b.Property<Guid>("RoleId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RoleName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("RoleId");

                    b.ToTable("Roles");
                });

            modelBuilder.Entity("Capstone.DataAccess.Entities.Schema", b =>
                {
                    b.Property<Guid>("SchemaId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("SchemaName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("SchemaId");

                    b.ToTable("Schema");
                });

            modelBuilder.Entity("Capstone.DataAccess.Entities.SchemaPermission", b =>
                {
                    b.Property<Guid?>("RoleId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("PermissionId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("SchemaId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("RoleId", "PermissionId", "SchemaId");

                    b.HasIndex("PermissionId");

                    b.HasIndex("SchemaId");

                    b.ToTable("SchemaPermissions");
                });

            modelBuilder.Entity("Capstone.DataAccess.Entities.Status", b =>
                {
                    b.Property<Guid>("StatusId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("StatusId");

                    b.ToTable("Status");
                });

            modelBuilder.Entity("Capstone.DataAccess.Entities.Ticket", b =>
                {
                    b.Property<Guid>("TicketId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("AssignTo")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("CreateBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreateTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("Decription")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("DeleteTime")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("DueDate")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("InterationId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("PrevId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("PriorityId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("StartDate")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("StatusId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("TypeId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("TicketId");

                    b.HasIndex("AssignTo");

                    b.HasIndex("InterationId");

                    b.HasIndex("PriorityId");

                    b.HasIndex("StatusId");

                    b.HasIndex("TypeId");

                    b.ToTable("Tickets");
                });

            modelBuilder.Entity("Capstone.DataAccess.Entities.TicketComment", b =>
                {
                    b.Property<Guid>("CommentId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("AttachmentId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Content")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreateAt")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("CreateBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("DeleteAt")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("TaskId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("UpdateAt")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("CommentId");

                    b.HasIndex("TaskId");

                    b.HasIndex("UserId");

                    b.ToTable("TicketComments");
                });

            modelBuilder.Entity("Capstone.DataAccess.Entities.TicketHistory", b =>
                {
                    b.Property<Guid>("HistoryId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("ChangeAt")
                        .HasColumnType("datetime2");

                    b.Property<int>("CurrentStatus")
                        .HasColumnType("int");

                    b.Property<int?>("PreviousStatus")
                        .HasColumnType("int");

                    b.Property<Guid>("TicketId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("HistoryId");

                    b.HasIndex("TicketId");

                    b.ToTable("TicketHistories");
                });

            modelBuilder.Entity("Capstone.DataAccess.Entities.TicketType", b =>
                {
                    b.Property<Guid>("TypeId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("TypeId");

                    b.ToTable("TicketTypes");
                });

            modelBuilder.Entity("Capstone.DataAccess.Entities.User", b =>
                {
                    b.Property<Guid>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("AccessToken")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Address")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Avatar")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("Dob")
                        .HasColumnType("datetime2");

                    b.Property<string>("Email")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Fullname")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("Gender")
                        .HasColumnType("int");

                    b.Property<bool>("IsAdmin")
                        .HasColumnType("bit");

                    b.Property<bool>("IsFirstTime")
                        .HasColumnType("bit");

                    b.Property<DateTime>("JoinedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("PassResetToken")
                        .HasColumnType("nvarchar(max)");

                    b.Property<byte[]>("PasswordHash")
                        .HasColumnType("varbinary(max)");

                    b.Property<byte[]>("PasswordSalt")
                        .HasColumnType("varbinary(max)");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RefreshToken")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("ResetTokenExpires")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("StatusId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("TokenCreated")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("TokenExpires")
                        .HasColumnType("datetime2");

                    b.Property<string>("UserName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("VerificationToken")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("VerifiedAt")
                        .HasColumnType("datetime2");

                    b.HasKey("UserId");

                    b.HasIndex("StatusId");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("StatusTicketHistory", b =>
                {
                    b.Property<Guid>("TaskHistoriesHistoryId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("TaskStatusStatusId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("TaskHistoriesHistoryId", "TaskStatusStatusId");

                    b.HasIndex("TaskStatusStatusId");

                    b.ToTable("StatusTicketHistory");
                });

            modelBuilder.Entity("Capstone.DataAccess.Entities.Attachment", b =>
                {
                    b.HasOne("Capstone.DataAccess.Entities.TicketComment", "TaskComment")
                        .WithMany("Attachments")
                        .HasForeignKey("CommentId");

                    b.HasOne("Capstone.DataAccess.Entities.User", "User")
                        .WithMany("Attachments")
                        .HasForeignKey("CreateBy")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("Capstone.DataAccess.Entities.Ticket", "Ticket")
                        .WithMany("Attachments")
                        .HasForeignKey("TicketId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("TaskComment");

                    b.Navigation("Ticket");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Capstone.DataAccess.Entities.Board", b =>
                {
                    b.HasOne("Capstone.DataAccess.Entities.Status", "Status")
                        .WithMany("Boards")
                        .HasForeignKey("StatusId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("Status");
                });

            modelBuilder.Entity("Capstone.DataAccess.Entities.Interation", b =>
                {
                    b.HasOne("Capstone.DataAccess.Entities.Board", "Board")
                        .WithMany("Interations")
                        .HasForeignKey("BoardId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Capstone.DataAccess.Entities.Status", "Status")
                        .WithMany("Interations")
                        .HasForeignKey("StatusId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("Board");

                    b.Navigation("Status");
                });

            modelBuilder.Entity("Capstone.DataAccess.Entities.Notification", b =>
                {
                    b.HasOne("Capstone.DataAccess.Entities.User", "User")
                        .WithMany("Notifications")
                        .HasForeignKey("RecerverId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("Capstone.DataAccess.Entities.Project", b =>
                {
                    b.HasOne("Capstone.DataAccess.Entities.Board", "Board")
                        .WithOne("Project")
                        .HasForeignKey("Capstone.DataAccess.Entities.Project", "ProjectId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Capstone.DataAccess.Entities.Schema", "Schemas")
                        .WithMany()
                        .HasForeignKey("SchemasId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Capstone.DataAccess.Entities.Status", "Status")
                        .WithMany("Projects")
                        .HasForeignKey("StatusId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("Board");

                    b.Navigation("Schemas");

                    b.Navigation("Status");
                });

            modelBuilder.Entity("Capstone.DataAccess.Entities.ProjectMember", b =>
                {
                    b.HasOne("Capstone.DataAccess.Entities.Project", "Project")
                        .WithMany("ProjectMembers")
                        .HasForeignKey("ProjectId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("Capstone.DataAccess.Entities.Role", "Role")
                        .WithMany("ProjectMember")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("Capstone.DataAccess.Entities.User", "Users")
                        .WithMany("ProjectMember")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Project");

                    b.Navigation("Role");

                    b.Navigation("Users");
                });

            modelBuilder.Entity("Capstone.DataAccess.Entities.SchemaPermission", b =>
                {
                    b.HasOne("Capstone.DataAccess.Entities.Permission", "Permission")
                        .WithMany("SchemaPermissions")
                        .HasForeignKey("PermissionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Capstone.DataAccess.Entities.Role", "Role")
                        .WithMany("SchemaPermissions")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Capstone.DataAccess.Entities.Schema", "Schema")
                        .WithMany("SchemaPermissions")
                        .HasForeignKey("SchemaId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Permission");

                    b.Navigation("Role");

                    b.Navigation("Schema");
                });

            modelBuilder.Entity("Capstone.DataAccess.Entities.Ticket", b =>
                {
                    b.HasOne("Capstone.DataAccess.Entities.ProjectMember", "ProjectMember")
                        .WithMany("Tickets")
                        .HasForeignKey("AssignTo")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Capstone.DataAccess.Entities.Interation", "Interation")
                        .WithMany("Tickets")
                        .HasForeignKey("InterationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Capstone.DataAccess.Entities.PriorityLevel", "PriorityLevel")
                        .WithMany("Tickets")
                        .HasForeignKey("PriorityId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Capstone.DataAccess.Entities.Status", "Status")
                        .WithMany("Tickets")
                        .HasForeignKey("StatusId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("Capstone.DataAccess.Entities.TicketType", "TicketType")
                        .WithMany("Tickets")
                        .HasForeignKey("TypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Interation");

                    b.Navigation("PriorityLevel");

                    b.Navigation("ProjectMember");

                    b.Navigation("Status");

                    b.Navigation("TicketType");
                });

            modelBuilder.Entity("Capstone.DataAccess.Entities.TicketComment", b =>
                {
                    b.HasOne("Capstone.DataAccess.Entities.Ticket", "Ticket")
                        .WithMany("TaskComments")
                        .HasForeignKey("TaskId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("Capstone.DataAccess.Entities.User", "User")
                        .WithMany("TaskComments")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Ticket");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Capstone.DataAccess.Entities.TicketHistory", b =>
                {
                    b.HasOne("Capstone.DataAccess.Entities.Ticket", "Ticket")
                        .WithMany("TaskHistories")
                        .HasForeignKey("TicketId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Ticket");
                });

            modelBuilder.Entity("Capstone.DataAccess.Entities.User", b =>
                {
                    b.HasOne("Capstone.DataAccess.Entities.Status", "Status")
                        .WithMany("Users")
                        .HasForeignKey("StatusId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("Status");
                });

            modelBuilder.Entity("StatusTicketHistory", b =>
                {
                    b.HasOne("Capstone.DataAccess.Entities.TicketHistory", null)
                        .WithMany()
                        .HasForeignKey("TaskHistoriesHistoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Capstone.DataAccess.Entities.Status", null)
                        .WithMany()
                        .HasForeignKey("TaskStatusStatusId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Capstone.DataAccess.Entities.Board", b =>
                {
                    b.Navigation("Interations");

                    b.Navigation("Project")
                        .IsRequired();
                });

            modelBuilder.Entity("Capstone.DataAccess.Entities.Interation", b =>
                {
                    b.Navigation("Tickets");
                });

            modelBuilder.Entity("Capstone.DataAccess.Entities.Permission", b =>
                {
                    b.Navigation("SchemaPermissions");
                });

            modelBuilder.Entity("Capstone.DataAccess.Entities.PriorityLevel", b =>
                {
                    b.Navigation("Tickets");
                });

            modelBuilder.Entity("Capstone.DataAccess.Entities.Project", b =>
                {
                    b.Navigation("ProjectMembers");
                });

            modelBuilder.Entity("Capstone.DataAccess.Entities.ProjectMember", b =>
                {
                    b.Navigation("Tickets");
                });

            modelBuilder.Entity("Capstone.DataAccess.Entities.Role", b =>
                {
                    b.Navigation("ProjectMember");

                    b.Navigation("SchemaPermissions");
                });

            modelBuilder.Entity("Capstone.DataAccess.Entities.Schema", b =>
                {
                    b.Navigation("SchemaPermissions");
                });

            modelBuilder.Entity("Capstone.DataAccess.Entities.Status", b =>
                {
                    b.Navigation("Boards");

                    b.Navigation("Interations");

                    b.Navigation("Projects");

                    b.Navigation("Tickets");

                    b.Navigation("Users");
                });

            modelBuilder.Entity("Capstone.DataAccess.Entities.Ticket", b =>
                {
                    b.Navigation("Attachments");

                    b.Navigation("TaskComments");

                    b.Navigation("TaskHistories");
                });

            modelBuilder.Entity("Capstone.DataAccess.Entities.TicketComment", b =>
                {
                    b.Navigation("Attachments");
                });

            modelBuilder.Entity("Capstone.DataAccess.Entities.TicketType", b =>
                {
                    b.Navigation("Tickets");
                });

            modelBuilder.Entity("Capstone.DataAccess.Entities.User", b =>
                {
                    b.Navigation("Attachments");

                    b.Navigation("Notifications");

                    b.Navigation("ProjectMember");

                    b.Navigation("TaskComments");
                });
#pragma warning restore 612, 618
        }
    }
}

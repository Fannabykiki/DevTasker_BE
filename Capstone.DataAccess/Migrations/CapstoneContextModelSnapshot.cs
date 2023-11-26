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

                    b.Property<DateTime>("CreateAt")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("CreateBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("DeleteAt")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("ExprireTime")
                        .HasColumnType("datetime2");

                    b.Property<bool?>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<Guid?>("TaskId")
                        .IsRequired()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("AttachmentId");

                    b.HasIndex("CreateBy");

                    b.HasIndex("TaskId");

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

            modelBuilder.Entity("Capstone.DataAccess.Entities.BoardStatus", b =>
                {
                    b.Property<Guid>("BoardStatusId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("BoardId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int?>("Order")
                        .HasColumnType("int");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("BoardStatusId");

                    b.HasIndex("BoardId");

                    b.ToTable("BoardStatus");
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

            modelBuilder.Entity("Capstone.DataAccess.Entities.Invitation", b =>
                {
                    b.Property<Guid>("InvitationId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreateAt")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("CreateBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("InviteTo")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid?>("ProjectId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("ProjectName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("StatusId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("InvitationId");

                    b.HasIndex("CreateBy");

                    b.HasIndex("StatusId");

                    b.ToTable("Invitations");
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
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("EndDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("ExpireAt")
                        .HasColumnType("datetime2");

                    b.Property<bool?>("IsDelete")
                        .HasColumnType("bit");

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

                    b.Property<Guid?>("StatusId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("MemberId");

                    b.HasIndex("ProjectId");

                    b.HasIndex("RoleId");

                    b.HasIndex("StatusId");

                    b.HasIndex("UserId");

                    b.ToTable("ProjectMembers");
                });

            modelBuilder.Entity("Capstone.DataAccess.Entities.Role", b =>
                {
                    b.Property<Guid>("RoleId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("DeleteAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("ExprireTime")
                        .HasColumnType("datetime2");

                    b.Property<bool?>("IsDelete")
                        .HasColumnType("bit");

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

                    b.Property<DateTime?>("DeleteAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("ExprireTime")
                        .HasColumnType("datetime2");

                    b.Property<bool?>("IsDelete")
                        .HasColumnType("bit");

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

            modelBuilder.Entity("Capstone.DataAccess.Entities.Task", b =>
                {
                    b.Property<Guid>("TaskId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("AssignTo")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("CreateBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreateTime")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("DeleteAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("DueDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("ExprireTime")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("InterationId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool?>("IsDelete")
                        .HasColumnType("bit");

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

                    b.HasKey("TaskId");

                    b.HasIndex("AssignTo");

                    b.HasIndex("InterationId");

                    b.HasIndex("PriorityId");

                    b.HasIndex("StatusId");

                    b.HasIndex("TypeId");

                    b.ToTable("Tasks");
                });

            modelBuilder.Entity("Capstone.DataAccess.Entities.TaskComment", b =>
                {
                    b.Property<Guid>("CommentId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Content")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreateAt")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("CreateBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("DeleteAt")
                        .HasColumnType("datetime2");

                    b.Property<Guid?>("ReplyTo")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("TaskId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("UpdateAt")
                        .HasColumnType("datetime2");

                    b.HasKey("CommentId");

                    b.HasIndex("CreateBy");

                    b.HasIndex("TaskId");

                    b.ToTable("TaskComments");
                });

            modelBuilder.Entity("Capstone.DataAccess.Entities.TaskHistory", b =>
                {
                    b.Property<Guid>("HistoryId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("ChangeAt")
                        .HasColumnType("datetime2");

                    b.Property<Guid?>("ChangeBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("CurrentStatusId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("PreviousStatusId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("StatusId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("TaskId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("HistoryId");

                    b.HasIndex("ChangeBy");

                    b.HasIndex("CurrentStatusId");

                    b.HasIndex("StatusId");

                    b.HasIndex("TaskId");

                    b.ToTable("TaskHistories");
                });

            modelBuilder.Entity("Capstone.DataAccess.Entities.TaskType", b =>
                {
                    b.Property<Guid>("TypeId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("TypeId");

                    b.ToTable("TaskTypes");
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

            modelBuilder.Entity("Capstone.DataAccess.Entities.Attachment", b =>
                {
                    b.HasOne("Capstone.DataAccess.Entities.ProjectMember", "ProjectMember")
                        .WithMany("Attachments")
                        .HasForeignKey("CreateBy")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("Capstone.DataAccess.Entities.Task", "Task")
                        .WithMany("Attachments")
                        .HasForeignKey("TaskId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ProjectMember");

                    b.Navigation("Task");
                });

            modelBuilder.Entity("Capstone.DataAccess.Entities.Board", b =>
                {
                    b.HasOne("Capstone.DataAccess.Entities.Status", null)
                        .WithMany("Boards")
                        .HasForeignKey("StatusId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Capstone.DataAccess.Entities.BoardStatus", b =>
                {
                    b.HasOne("Capstone.DataAccess.Entities.Board", "Board")
                        .WithMany("Status")
                        .HasForeignKey("BoardId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("Board");
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

            modelBuilder.Entity("Capstone.DataAccess.Entities.Invitation", b =>
                {
                    b.HasOne("Capstone.DataAccess.Entities.ProjectMember", "ProjectMember")
                        .WithMany("Invitations")
                        .HasForeignKey("CreateBy")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("Capstone.DataAccess.Entities.Status", "Status")
                        .WithMany("Invitations")
                        .HasForeignKey("StatusId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("ProjectMember");

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
                        .WithMany("Project")
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

                    b.HasOne("Capstone.DataAccess.Entities.Status", "Status")
                        .WithMany("ProjectMembers")
                        .HasForeignKey("StatusId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("Capstone.DataAccess.Entities.User", "Users")
                        .WithMany("ProjectMember")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Project");

                    b.Navigation("Role");

                    b.Navigation("Status");

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

            modelBuilder.Entity("Capstone.DataAccess.Entities.Task", b =>
                {
                    b.HasOne("Capstone.DataAccess.Entities.ProjectMember", "ProjectMember")
                        .WithMany("Tasks")
                        .HasForeignKey("AssignTo")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Capstone.DataAccess.Entities.Interation", "Interation")
                        .WithMany("Tasks")
                        .HasForeignKey("InterationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Capstone.DataAccess.Entities.PriorityLevel", "PriorityLevel")
                        .WithMany("Tasks")
                        .HasForeignKey("PriorityId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Capstone.DataAccess.Entities.BoardStatus", "Status")
                        .WithMany("Tasks")
                        .HasForeignKey("StatusId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("Capstone.DataAccess.Entities.TaskType", "TicketType")
                        .WithMany("Tasks")
                        .HasForeignKey("TypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Interation");

                    b.Navigation("PriorityLevel");

                    b.Navigation("ProjectMember");

                    b.Navigation("Status");

                    b.Navigation("TicketType");
                });

            modelBuilder.Entity("Capstone.DataAccess.Entities.TaskComment", b =>
                {
                    b.HasOne("Capstone.DataAccess.Entities.ProjectMember", "ProjectMember")
                        .WithMany("TaskComments")
                        .HasForeignKey("CreateBy")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("Capstone.DataAccess.Entities.Task", "Task")
                        .WithMany("TaskComments")
                        .HasForeignKey("TaskId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.Navigation("ProjectMember");

                    b.Navigation("Task");
                });

            modelBuilder.Entity("Capstone.DataAccess.Entities.TaskHistory", b =>
                {
                    b.HasOne("Capstone.DataAccess.Entities.ProjectMember", "ProjectMember")
                        .WithMany("TaskHistories")
                        .HasForeignKey("ChangeBy")
                        .OnDelete(DeleteBehavior.NoAction);

                    b.HasOne("Capstone.DataAccess.Entities.BoardStatus", "BoardStatus")
                        .WithMany("TaskHistory")
                        .HasForeignKey("CurrentStatusId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("Capstone.DataAccess.Entities.Status", null)
                        .WithMany("TaskHistories")
                        .HasForeignKey("StatusId");

                    b.HasOne("Capstone.DataAccess.Entities.Task", "Task")
                        .WithMany("TaskHistories")
                        .HasForeignKey("TaskId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("BoardStatus");

                    b.Navigation("ProjectMember");

                    b.Navigation("Task");
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

            modelBuilder.Entity("Capstone.DataAccess.Entities.Board", b =>
                {
                    b.Navigation("Interations");

                    b.Navigation("Project")
                        .IsRequired();

                    b.Navigation("Status");
                });

            modelBuilder.Entity("Capstone.DataAccess.Entities.BoardStatus", b =>
                {
                    b.Navigation("TaskHistory");

                    b.Navigation("Tasks");
                });

            modelBuilder.Entity("Capstone.DataAccess.Entities.Interation", b =>
                {
                    b.Navigation("Tasks");
                });

            modelBuilder.Entity("Capstone.DataAccess.Entities.Permission", b =>
                {
                    b.Navigation("SchemaPermissions");
                });

            modelBuilder.Entity("Capstone.DataAccess.Entities.PriorityLevel", b =>
                {
                    b.Navigation("Tasks");
                });

            modelBuilder.Entity("Capstone.DataAccess.Entities.Project", b =>
                {
                    b.Navigation("ProjectMembers");
                });

            modelBuilder.Entity("Capstone.DataAccess.Entities.ProjectMember", b =>
                {
                    b.Navigation("Attachments");

                    b.Navigation("Invitations");

                    b.Navigation("TaskComments");

                    b.Navigation("TaskHistories");

                    b.Navigation("Tasks");
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

                    b.Navigation("Invitations");

                    b.Navigation("Project");

                    b.Navigation("ProjectMembers");

                    b.Navigation("TaskHistories");

                    b.Navigation("Users");
                });

            modelBuilder.Entity("Capstone.DataAccess.Entities.Task", b =>
                {
                    b.Navigation("Attachments");

                    b.Navigation("TaskComments");

                    b.Navigation("TaskHistories");
                });

            modelBuilder.Entity("Capstone.DataAccess.Entities.TaskType", b =>
                {
                    b.Navigation("Tasks");
                });

            modelBuilder.Entity("Capstone.DataAccess.Entities.User", b =>
                {
                    b.Navigation("Notifications");

                    b.Navigation("ProjectMember");
                });
#pragma warning restore 612, 618
        }
    }
}

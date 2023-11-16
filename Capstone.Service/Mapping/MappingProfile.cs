﻿using AutoMapper;
using Capstone.Common.DTOs.Comments;
using Capstone.Common.DTOs.Iteration;
using Capstone.Common.DTOs.PermissionSchema;
using Capstone.Common.DTOs.Project;
using Capstone.Common.DTOs.Role;
using Capstone.Common.DTOs.Task;
using Capstone.Common.DTOs.TaskPriority;
using Capstone.Common.DTOs.User;
using Capstone.DataAccess.Entities;

namespace Capstone.Service.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<User, UserViewModel>();
        CreateMap<Project, GetAllProjectViewModel>();
        CreateMap<Project, GetAllProjectResponse>();
        CreateMap<Project, GetProjectUsedResponse>()
            .ForMember(dest => dest.ProjectStatus, opt => opt.MapFrom(src => src.Status.Title)); ;
		CreateMap<ProjectMember, ViewMemberProject>()
			.ForPath(dest => dest.Email, opt => opt.MapFrom(src => src.Users.Email))
			.ForPath(dest => dest.Fullname, opt => opt.MapFrom(src => src.Users.Fullname))
			.ForPath(dest => dest.RoleName, opt => opt.MapFrom(src => src.Role.RoleName))
			.ForPath(dest => dest.UserName, opt => opt.MapFrom(src => src.Users.UserName));
        CreateMap<Schema, GetAllPermissionSchemaResponse>();
        CreateMap<Schema, GetSchemaResponse>();
        CreateMap<Role, GetRoleResponse>();
        CreateMap<TaskComment, GetCommentResponse>()
            .ForPath(dest => dest.User.UserId, opt => opt.MapFrom(src => src.ProjectMember.UserId))
            .ForPath(dest => dest.User.Fullname, opt => opt.MapFrom(src => src.ProjectMember.Users.Fullname))
            .ForPath(dest => dest.User.Email, opt => opt.MapFrom(src => src.ProjectMember.Users.Email))
            .ForPath(dest => dest.User.IsFirstTime, opt => opt.MapFrom(src => src.ProjectMember.Users.IsFirstTime))
            .ForPath(dest => dest.User.IsAdmin, opt => opt.MapFrom(src => src.ProjectMember.Users.IsAdmin))
            .ForPath(dest => dest.User.Status, opt => opt.MapFrom(src => src.ProjectMember.Users.Status.Title));
        CreateMap<User, UserResponse>()
		   .ForPath(dest => dest.StatusName, opt => opt.MapFrom(src => src.Status.Title));
		CreateMap<ProjectMember, GetAllProjectViewModel>()
			.ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => src.Project.EndDate))
			.ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.Project.StartDate))
			.ForMember(dest => dest.ProjectId, opt => opt.MapFrom(src => src.Project.ProjectId))
			// .ForMember(dest => dest.ProjectStatus, opt => opt.MapFrom(src => src.Project.ProjectStatus))
			.ForMember(dest => dest.CreateBy, opt => opt.MapFrom(src => src.Project.CreateBy))
			.ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Project.Description))
			.ForMember(dest => dest.PrivacyStatus, opt => opt.MapFrom(src => src.Project.PrivacyStatus))
			.ForMember(dest => dest.CreateAt, opt => opt.MapFrom(src => src.Project.CreateAt))
			.ForMember(dest => dest.DeleteAt, opt => opt.MapFrom(src => src.Project.DeleteAt))
			.ForMember(dest => dest.ExpireAt, opt => opt.MapFrom(src => src.Project.ExpireAt))
			.ForMember(dest => dest.ProjectName, opt => opt.MapFrom(src => src.Project.ProjectName));
		CreateMap<BoardStatus, StatusTaskViewModel>();
		CreateMap<TaskType, TaskTypeViewModel>();
		CreateMap<PriorityLevel, GetAllTaskPriority>();
		CreateMap<Interation, InterationViewModel>()
			.ForMember(dest => dest.StatusName, opt => opt.MapFrom(src => src.Status.Title));
	}
}
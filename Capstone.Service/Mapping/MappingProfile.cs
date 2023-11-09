using AutoMapper;
using Capstone.Common.DTOs.Comments;
using Capstone.Common.DTOs.PermissionSchema;
using Capstone.Common.DTOs.Project;
using Capstone.Common.DTOs.Role;
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
        CreateMap<Project, GetProjectUseRoleResponse>()
            .ForMember(dest => dest.ProjectStatus, opt => opt.MapFrom(src => src.Status.Title)); ;
        CreateMap<ProjectMember, ViewMemberProject>();
        CreateMap<Schema, GetAllPermissionSchemaResponse>();
        CreateMap<Schema, GetSchemaResponse>();
        CreateMap<Role, GetRoleResponse>();
        CreateMap<TaskComment, GetCommentResponse>()
            .ForMember(dest => dest.CreateByUser, opt => opt.MapFrom(src => src.User));
		CreateMap<User, UserResponse>()
		   .ForMember(dest => dest.StatusName, opt => opt.MapFrom(src => src.Status.Title));
		CreateMap<ProjectMember, GetAllProjectViewModel>()
		   .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => src.Project.EndDate))
			.ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.Project.StartDate))
			 .ForMember(dest => dest.ProjectId, opt => opt.MapFrom(src => src.Project.ProjectId))
			  .ForMember(dest => dest.ProjectStatus, opt => opt.MapFrom(src => src.Project.Status.Title))
			   .ForMember(dest => dest.CreateBy, opt => opt.MapFrom(src => src.Project.CreateBy))
				.ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Project.Description))
				 .ForMember(dest => dest.PrivacyStatus, opt => opt.MapFrom(src => src.Project.PrivacyStatus))
				  .ForMember(dest => dest.CreateAt, opt => opt.MapFrom(src => src.Project.CreateAt))
				   .ForMember(dest => dest.DeleteAt, opt => opt.MapFrom(src => src.Project.DeleteAt))
					.ForMember(dest => dest.ExpireAt, opt => opt.MapFrom(src => src.Project.ExpireAt))
					 .ForMember(dest => dest.ProjectName, opt => opt.MapFrom(src => src.Project.ProjectName))
					  .ForMember(dest => dest.ProjectStatus, opt => opt.MapFrom(src => src.Project.Status.Title));

	}
}
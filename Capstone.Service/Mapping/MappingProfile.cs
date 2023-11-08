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
        CreateMap<Role, GetRoleResponse>();
        CreateMap<TaskComment, GetCommentResponse>()
            .ForMember(dest => dest.CreateByUser, opt => opt.MapFrom(src => src.User));
		CreateMap<User, UserResponse>()
		   .ForMember(dest => dest.StatusName, opt => opt.MapFrom(src => src.Status.Title));
	}
}
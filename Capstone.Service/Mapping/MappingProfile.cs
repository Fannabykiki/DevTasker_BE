using AutoMapper;
using Capstone.Common.DTOs.Comments;
using Capstone.Common.DTOs.PermissionSchema;
using Capstone.Common.DTOs.Project;
using Capstone.Common.DTOs.User;
using Capstone.DataAccess.Entities;

namespace Capstone.Service.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<User, UserViewModel>();
        CreateMap<Project, GetAllProjectViewModel>();
        CreateMap<ProjectMember, ViewMemberProject>();
        CreateMap<Schema, GetAllPermissionSchemaResponse>();
        CreateMap<TicketComment, GetCommentResponse>()
            .ForMember(dest => dest.CreateByUser, opt => opt.MapFrom(src => src.User)); ;
    }
}
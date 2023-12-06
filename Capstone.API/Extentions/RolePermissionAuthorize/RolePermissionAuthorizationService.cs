using Capstone.Service.ProjectService;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Capstone.API.Extentions.RolePermissionAuthorize
{
    public class RolePermissionAuthorizationService : IAuthorizationService
    {
        private readonly IProjectService _projectService;
        public RolePermissionAuthorizationService(IProjectService projectService)
        {
            _projectService = projectService;
        }
        public Task<AuthorizationResult> AuthorizeAsync(ClaimsPrincipal user, object? resource, IEnumerable<IAuthorizationRequirement> requirements)
        {
            throw new NotImplementedException();
        }

        public async Task<AuthorizationResult> AuthorizeAsync(ClaimsPrincipal user, object? resource, string policyName)
        {
            try
            {
                var userId = user.GetCurrentLoginUserIdFromClaims();
                var tmp = resource as RolePermissionResource;
                foreach (var projectId in tmp.ListProjectId)
                {
                    if (!projectId.HasValue) continue;
                    var lstPermission = await _projectService.GetPermissionAuthorizeByUserId(projectId.Value, userId);
                    if (lstPermission.ToList().Any(x => tmp.ListPermissionAuthorized.Contains(x.Name)))
                    {
                        return AuthorizationResult.Success();
                    }
                }
                return AuthorizationResult.Failed();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
    public class RolePermissionResource
    {
        public List<Guid?> ListProjectId { get; set; }
        public List<string> ListPermissionAuthorized { get; set; }
    }
}

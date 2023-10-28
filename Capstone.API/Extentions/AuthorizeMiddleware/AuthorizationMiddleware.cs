using Capstone.Service.ProjectService;
using Capstone.Service.UserService;

namespace Capstone.API.Extentions.AuthorizeMiddleware
{
    public class AuthorizationMiddleware : IMiddleware
    {
        private readonly IProjectService _projectService;
        public AuthorizationMiddleware(IProjectService projectService)
        {
            _projectService = projectService;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var userIdClaim = context.User.FindFirst("userId")?.Value;
            var projectIdClaim = context.User.FindFirst("projectId")?.Value;

            var isUserIdValid = Guid.TryParse(userIdClaim, out Guid userId);
            var isprojectIdValid = Guid.TryParse(userIdClaim, out Guid projectIdId);

            if (!isUserIdValid || !isprojectIdValid)
            {
                return;
            }

            var permissions = await _projectService.GetPermissionByUserId(projectIdId, userId);
            context.Items["Permissions"] = permissions;

            await next(context);
        }

    }
}

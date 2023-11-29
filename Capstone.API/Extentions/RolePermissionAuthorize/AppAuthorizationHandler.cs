using Microsoft.AspNetCore.Authorization;

namespace Capstone.API.Extentions.RolePermissionAuthorize
{
    public class AppAuthorizationHandler : IAuthorizationHandler
    {
        public Task HandleAsync(AuthorizationHandlerContext context)
        {
            if(context.Resource is HttpContext)
            {
                var hcontext = context.Resource as HttpContext;
                if(hcontext != null)
                {
                    //var res = hcontext.GetRouteValue("ProjectId");
                    var res = hcontext.Request.QueryString;
                }
            }
            var requirements = context.Requirements.ToList();
            foreach ( var requirement in requirements )
            {
                if (requirement is PermissionRoleRequirement)
                {
                    var claims = context.User;
                    context.Succeed(requirement);
                }
                
            }
            
            return Task.CompletedTask;
            
        }
    }
}

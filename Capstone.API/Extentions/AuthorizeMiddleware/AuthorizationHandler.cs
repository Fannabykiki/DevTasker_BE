using Microsoft.AspNetCore.Authorization;

namespace Capstone.API.Extentions.AuthorizeMiddleware
{
	public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
	{

		protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
		{
			if (!requirement.Permissions.Contains("CanView"))
			{
				context.Fail();
				return Task.CompletedTask;
			}

			context.Succeed(requirement);
			return Task.CompletedTask;
		}
	}
}

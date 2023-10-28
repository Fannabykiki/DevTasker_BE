using Microsoft.AspNetCore.Authorization;

namespace Capstone.API.Extentions.AuthorizeMiddleware
{
	public class PermissionRequirement : IAuthorizationRequirement
	{
		public List<string> Permissions { get; }

		public PermissionRequirement(List<string> permissions)
		{
			Permissions = permissions;
		}
	}
}

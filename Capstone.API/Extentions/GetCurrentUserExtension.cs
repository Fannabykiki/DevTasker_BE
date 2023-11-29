using Capstone.Common.DTOs.User;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Capstone.API.Extentions
{
	public static class GetCurrentUserExtension
	{
		public static Guid GetCurrentLoginUserId(this ControllerBase controller)
		{
			if (controller.HttpContext.User.Identity is ClaimsIdentity identity)
			{
				var userIdString = identity?.FindFirst("UserId")?.Value;

				if (string.IsNullOrWhiteSpace(userIdString))
				{
					return Guid.Empty;
				}

				var isUserIdValid = Guid.TryParse(userIdString, out Guid userId);

				if (!isUserIdValid)
				{
					return Guid.Empty;
				}

				return userId;
			}
			return Guid.Empty;
		}
		public static bool GetIsAdmin(this ClaimsPrincipal user)
		{
            if (user.Identity is ClaimsIdentity identity)
            {
                var userIsAdmin = identity?.FindFirst("IsAdmin")?.Value;

                if (string.IsNullOrWhiteSpace(userIsAdmin))
                {
                    return false;
                }

                var isAdminParsed = bool.TryParse(userIsAdmin, out bool isAdmin);

                if (!isAdminParsed)
                {
                    return false;
                }

                return isAdmin;
            }
            return false;
        }
        public static Guid GetCurrentLoginUserIdFromClaims(this ClaimsPrincipal user)
        {
            if (user.Identity is ClaimsIdentity identity)
            {
                var userIdString = identity?.FindFirst("UserId")?.Value;

                if (string.IsNullOrWhiteSpace(userIdString))
                {
                    return Guid.Empty;
                }

                var isUserIdValid = Guid.TryParse(userIdString, out Guid userId);

                if (!isUserIdValid)
                {
                    return Guid.Empty;
                }

                return userId;
            }
            return Guid.Empty;
        }
        public static string SayHello(this string currentString, string name)
		{
			return $"Hello : {name}";
		}

		public static string SayHello(this string currentString)
		{
			return $"Hello : {currentString ?? "Anomynous"}";
		}
	}
}

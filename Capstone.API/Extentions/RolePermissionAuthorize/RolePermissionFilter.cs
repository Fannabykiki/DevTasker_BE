using Capstone.Service.ProjectService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.CodeAnalysis;
using System.Net;

namespace Capstone.API.Extentions.RolePermissionAuthorize
{
    public class RolePermissionFilter :ActionFilterAttribute, IAsyncActionFilter
    {
        public string PropertyName { get; set; }
        public string PropertyType { get; set; }
        public string[] Permissions { get; set; }
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var _projectService = context.HttpContext.RequestServices.GetService<IProjectService>();
            //var contet
            Guid? projectId = await GetProjectIdFromPath(context);
            //var t = context.ActionArguments;
            //foreach(var obj in t.Values)
            //{
            //    var info = obj.GetType().GetProperty(PropertyName);
            //    if (info != null) { projectId = (Guid) info.GetValue(obj, null);}
            //}
            projectId = new Guid("4CB31FA7-2F7B-42D6-80CF-A4545A5FE69B");
            
            var userId = ((ControllerBase)context.Controller).GetCurrentLoginUserId();
            var lsTPermission =  await _projectService.GetPermissionByUserId(projectId.Value, userId);
            var isAuthorize = lsTPermission.Any(x => Permissions.Contains(x.Name));
            if (!isAuthorize)
            {
                throw new BadHttpRequestException("Not authorize",403);
            }
            //var uri = context.HttpContext.Request.Path;
            await  next();
        }
        public async Task<Guid?> GetProjectIdFromPath(ActionExecutingContext context)
        {

            if(PropertyType == "Project")
            {
                var tmp = context.ActionArguments;
                foreach(var key in tmp.Keys)
                {
                    var obj = tmp[key];
                    if(obj is Guid && key == PropertyName)
                    {
                        return (Guid)obj;
                    }
                    var info = obj.GetType().GetProperty(PropertyName);
                    if (info != null)
                    {
                        return (Guid)info.GetValue(obj, null);
                    }
                }
                
            }
            return null;
        }
    }
}

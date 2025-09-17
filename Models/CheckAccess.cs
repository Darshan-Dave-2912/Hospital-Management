using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Admin.Models
{
    public class CheckAccess : ActionFilterAttribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext filterContext)
        {
            if (filterContext.HttpContext.Session.GetString("UserId") == null)
                filterContext.Result = new RedirectResult("~/User/Login");
        }

        public override void OnResultExecuting(ResultExecutingContext context)
        {
            if (context.HttpContext.Session.GetString("UserId") == null)
            {
                context.HttpContext.Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
                context.HttpContext.Response.Headers["Expires"] = "-1";
                context.HttpContext.Response.Headers["Pragma"] = "no-cache";
                base.OnResultExecuting(context);
            }
        }
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace AdminApp.Controllers
{
    [Authorize]
    public class ProtectedController : Controller
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var sessions = context.HttpContext.Session.GetString("Token");
            if (sessions == null)
            {
                context.Result = new RedirectToActionResult("Index", "Login", null);
            }
            base.OnActionExecuting(context);
        }
    }
}
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace sistema_biblioteca.Controllers;

public abstract class AuthenticatedController : Controller
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var controllerName = ControllerContext.ActionDescriptor.ControllerName;
        var actionName = ControllerContext.ActionDescriptor.ActionName;

        if (controllerName == "Auth" || (controllerName == "Home" && actionName == "Error"))
        {
            base.OnActionExecuting(context);
            return;
        }

        var role = HttpContext.Session.GetString("Role");
        if (string.IsNullOrWhiteSpace(role))
        {
            context.Result = RedirectToAction("Login", "Auth");
            return;
        }

        base.OnActionExecuting(context);
    }
}

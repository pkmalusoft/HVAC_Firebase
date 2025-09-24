using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
namespace HVAC.DAL
{
    public class SessionExpireFilterAttribute : ActionFilterAttribute
    {

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            HttpContext ctx = HttpContext.Current;
            
            // Check if session is available
            if (ctx.Session == null)
            {
                filterContext.Result = new RedirectResult("~/Home/Home");
                return;
            }
            
            // Check for required session variables
            if (HttpContext.Current.Session["UserID"] == null || 
                HttpContext.Current.Session["UserRoleID"] == null ||
                HttpContext.Current.Session["CurrentBranchID"] == null)
            {
                // Clear any existing session data
                ctx.Session.Clear();
                ctx.Session.Abandon();
                
                // Check if it's an AJAX request
                if (filterContext.HttpContext.Request.IsAjaxRequest())
                {
                    filterContext.HttpContext.Response.ClearContent();
                    filterContext.HttpContext.Items["AjaxPermissionDenied"] = true;
                    filterContext.Result = new JsonResult
                    {
                        Data = new { status = "error", message = "Session expired" },
                        JsonRequestBehavior = JsonRequestBehavior.AllowGet
                    };
                }
                else
                {
                    filterContext.Result = new RedirectResult("~/Home/Home");
                }
                return;
            }
            
            base.OnActionExecuting(filterContext);
        }
    }
}
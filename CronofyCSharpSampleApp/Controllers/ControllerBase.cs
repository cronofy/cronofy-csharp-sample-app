using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Cronofy;

namespace CronofyCSharpSampleApp.Controllers
{
    public class ControllerBase : Controller
    {
		protected override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			var cronofyCookie = Request.Cookies.Get(CronofyHelper.CookieName);

            if (cronofyCookie == null)
            {
                filterContext.Result = new RedirectResult("/login");
            } 
            else if(!CronofyHelper.LoadUser(cronofyCookie.Value, false))
			{
                Response.Cookies.Remove(CronofyHelper.CookieName);
                filterContext.Result = new RedirectResult("/login");
			}

			base.OnActionExecuting(filterContext);
		}

		protected override void OnException(ExceptionContext filterContext)
		{
			var ex = filterContext.Exception;

			if (ex is CronofyResponseException || ex is CredentialsInvalidError)
			{
				Response.Cookies.Remove(CronofyHelper.CookieName);
				filterContext.Result = new RedirectResult("/login");
                filterContext.ExceptionHandled = true;
			}

			base.OnException(filterContext);
		}
    }
}

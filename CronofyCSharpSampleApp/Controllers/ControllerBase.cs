using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CronofyCSharpSampleApp.Controllers
{
    public class ControllerBase : Controller
    {
		protected override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			var cronofyCookie = Request.Cookies.Get(CronofyHelper.CookieName);

			if (cronofyCookie == null || !CronofyHelper.LoadUser(cronofyCookie.Value))
			{
				filterContext.Result = new RedirectResult("/login");
			}

			base.OnActionExecuting(filterContext);
		}
    }
}

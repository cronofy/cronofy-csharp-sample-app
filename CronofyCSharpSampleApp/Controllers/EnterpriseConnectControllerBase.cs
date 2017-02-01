using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Cronofy;

namespace CronofyCSharpSampleApp.Controllers
{
    public class EnterpriseConnectControllerBase : Controller
    {
        protected HttpCookie uidCookie;

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            uidCookie = Request.Cookies.Get(CronofyHelper.EnterpriseConnectCookieName);

            if (uidCookie == null)
            {
                filterContext.Result = new RedirectResult("/login/enterpriseconnect");
            }
            else if (!CronofyHelper.LoadUser(uidCookie.Value, true))
            {
                Response.Cookies.Remove(CronofyHelper.EnterpriseConnectCookieName);
                filterContext.Result = new RedirectResult("/login/enterpriseconnect");
            }

            base.OnActionExecuting(filterContext);
        }

        protected override void OnException(ExceptionContext filterContext)
        {
            var ex = filterContext.Exception;

            if (ex is CronofyResponseException || ex is CredentialsInvalidError)
            {
                Response.Cookies.Remove(CronofyHelper.EnterpriseConnectCookieName);
                filterContext.Result = new RedirectResult("/login/enterpriseconnect");
                filterContext.ExceptionHandled = true;
            }

            base.OnException(filterContext);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CronofyCSharpSampleApp.Controllers
{
    public class LoginController : Controller
    {
        public ActionResult Index()
        {
            ViewData["authUrl"] = CronofyHelper.GetAuthUrl();

            return View ();
        }

        public ActionResult EnterpriseConnect()
        {
            ViewData["authUrl"] = CronofyHelper.GetEnterpriseConnectAuthUrl();

            return View("EnterpriseConnect");
        }
    }
}

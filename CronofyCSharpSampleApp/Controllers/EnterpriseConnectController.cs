using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CronofyCSharpSampleApp.Controllers
{
    public class EnterpriseConnectController : EnterpriseConnectControllerBase
    {
        public ActionResult Index()
        {
            ViewData["resources"] = CronofyHelper.GetResources();

            return View ();
        }

		public ActionResult Login()
		{
			return View("Login");
		}
    }
}

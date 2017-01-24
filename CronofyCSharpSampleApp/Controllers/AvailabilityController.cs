using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CronofyCSharpSampleApp.Controllers
{
    public class AvailabilityController : ControllerBase
    {
        public ActionResult Index()
        {
            return View ();
        }

        public ActionResult AccountId(string code)
        {
            try
            {
                var token = CronofyHelper.GetAccountIdOAuthToken(code);
                ViewData["AccountId"] = token.AccountId;
                ViewData["AuthUrl"] = CronofyHelper.GetAccountIdAuthUrl();
            }
            catch
            {
                return new RedirectResult(CronofyHelper.GetAccountIdAuthUrl());
            }

            return View();
        }
    }
}

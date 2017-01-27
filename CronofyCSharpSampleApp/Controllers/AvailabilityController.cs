using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CronofyCSharpSampleApp.Models;

namespace CronofyCSharpSampleApp.Controllers
{
    public class AvailabilityController : ControllerBase
    {
        public ActionResult Index()
        {
            return View(new Availability
            {
                AuthUrl = CronofyHelper.GetAccountIdAuthUrl(),

                RequiredParticipants = "all",
                Duration = 60
            });
        }

        public ActionResult ViewAvailability(Availability resource)
        {
            resource.AuthUrl = CronofyHelper.GetAccountIdAuthUrl();

            if (ModelState.IsValid)
            {
                resource.AvailablePeriods = CronofyHelper.Availability(resource);

                return View("Index", resource);
            }

            return View("Index", resource);
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

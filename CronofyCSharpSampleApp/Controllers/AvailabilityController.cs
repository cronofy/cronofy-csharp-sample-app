using System;
using System.Web.Mvc;
using Cronofy;
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

                AccountId1 = CronofyHelper.GetAccount().Id,
                RequiredParticipants = "all",
                Duration = 60,
                Start = DateTime.Now.Date.AddDays(1),
                End = DateTime.Now.Date.AddDays(2),
            });
        }

        public ActionResult ViewAvailability(Availability resource)
        {
            resource.AuthUrl = CronofyHelper.GetAccountIdAuthUrl();

            if (ModelState.IsValid)
            {
                try
                {
                    resource.AvailablePeriods = CronofyHelper.Availability(resource);
                }
                catch (CronofyResponseException ex)
                {
                    resource.SetError(ex);
                }

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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CronofyCSharpSampleApp.Controllers
{
    public class CalendarsController : ControllerBase
    {
        public ActionResult New(string id)
        {
			ViewData["profileName"] = CronofyHelper.GetProfiles().First(x => x.Id == id).Name;

			var calendar = new Models.Calendar
			{
				ProfileId = id,
			};

            return View("New", calendar);
        }

		public ActionResult Create(Models.Calendar calendar)
		{
			if (ModelState.IsValid)
			{
				CronofyHelper.CreateCalendar(calendar.ProfileId, calendar.Name);

				return new RedirectResult("/profiles");
			}

			ViewData["profileName"] = CronofyHelper.GetProfiles().First(x => x.Id == calendar.ProfileId).Name;

			return View("New", calendar);
		}
    }
}

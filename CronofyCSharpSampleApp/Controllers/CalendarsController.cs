using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CronofyCSharpSampleApp.Controllers
{
    public class CalendarsController : ControllerBase
    {
		public ActionResult Show(string id)
		{
			ViewData["calendarId"] = id;
			ViewData["calendarName"] = CronofyHelper.GetCalendars().First(x => x.CalendarId == id).Name;
			ViewData["events"] = CronofyHelper.ReadEventsForCalendar(id);

			return View("Show");
		}

        public ActionResult New([Bind(Prefix = "id")] string profileId)
        {
			ViewData["profileName"] = CronofyHelper.GetProfiles().First(x => x.Id == profileId).Name;

			var calendar = new Models.Calendar
			{
				ProfileId = profileId,
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

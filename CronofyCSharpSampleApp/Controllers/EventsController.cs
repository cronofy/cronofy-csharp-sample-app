using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CronofyCSharpSampleApp.Controllers
{
    public class EventsController : ControllerBase
    {
		public ActionResult Show(string id)
        {
			var shownEvent = CronofyHelper.ReadEvents().First(x => x.EventUid == id);

			ViewData["calendarName"] = CronofyHelper.GetCalendars().First(x => x.CalendarId == shownEvent.CalendarId).Name;

			return View("Show", shownEvent);
        }
    }
}

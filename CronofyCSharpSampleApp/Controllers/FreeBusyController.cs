using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CronofyCSharpSampleApp.Controllers
{
    public class FreeBusyController : ControllerBase
    {
        public ActionResult Index()
        {
            var calendars = CronofyHelper.GetCalendars();
            var freeBusy = CronofyHelper.GetFreeBusy();

            var model = freeBusy
                .GroupBy(x => x.CalendarId)
                .ToDictionary(x => calendars.First(cal => cal.CalendarId == x.Key), x => x.ToList());

            return View ("Index", model);
        }
    }
}

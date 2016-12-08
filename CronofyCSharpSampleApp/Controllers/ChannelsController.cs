using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CronofyCSharpSampleApp.Controllers
{
    public class ChannelsController : ControllerBase
    {
        public ActionResult Index()
        {
            return View("Index", CronofyHelper.GetChannels());
        }

		public ActionResult Show(string id)
		{
			var channel = CronofyHelper.GetChannels().First(x => x.Id == id);

			ViewData["Data"] = DatabaseHandler.Many<ChannelData>($"SELECT * FROM ChannelData WHERE ChannelId='{channel.Id}'");

			return View("Show", channel);
		}

		public ActionResult New()
		{
			var calendars = CronofyHelper.GetCalendars();

			var channel = new Models.Channel
			{
				CalendarIds = calendars.Select(x => x.CalendarId).ToArray(),
			};

			ViewData["domain"] = ConfigurationManager.AppSettings["domain"];
			ViewData["calendars"] = calendars;
			
			return View("New", channel);
		}

		public ActionResult Create(Models.Channel channel)
		{
			if (ModelState.IsValid)
			{
				var url = ConfigurationManager.AppSettings["domain"] + "/push/" + channel.Path;

				CronofyHelper.CreateChannel(url, channel.OnlyManaged, channel.CalendarIds);

				return new RedirectResult("/channels");
			}

			ViewData["domain"] = ConfigurationManager.AppSettings["domain"];
			ViewData["calendars"] = CronofyHelper.GetCalendars();

			return View("New", channel);
		}
    }
}

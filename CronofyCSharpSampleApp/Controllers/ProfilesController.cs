using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Cronofy;

namespace CronofyCSharpSampleApp.Controllers
{
    public class ProfilesController : ControllerBase
    {
        public ActionResult Index()
		{
			var profiles = new Dictionary<Cronofy.Profile, Cronofy.Calendar[]>();
			var calendars = CronofyHelper.GetCalendars();

			foreach (var profile in CronofyHelper.GetProfiles())
			{
				profiles.Add(profile, calendars.Where(x => x.Profile.ProfileId == profile.Id).ToArray());
			}

            return View(profiles);
        }
    }
}

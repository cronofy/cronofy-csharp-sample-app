using System;
using System.Collections.Generic;
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
    }
}

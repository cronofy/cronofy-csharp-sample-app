using System.Web.Mvc;
using System.Configuration;
using Cronofy;
using System;

namespace CronofyCSharpSampleApp.Controllers
{
	public class HomeController : ControllerBase
	{
		public ActionResult Index()
		{
			return View();
		}
	}
}

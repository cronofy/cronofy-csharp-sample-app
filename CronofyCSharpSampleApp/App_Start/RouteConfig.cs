using System.Web.Mvc;
using System.Web.Routing;

namespace CronofyCSharpSampleApp
{
	public class RouteConfig
	{
		public static void RegisterRoutes(RouteCollection routes)
		{
			routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                            name: "ServiceAccountCalendarsNewEvent",
                            url: "serviceaccountusers/show/{userId}/calendar/{calendarId}/newevent",
                            defaults: new { controller = "ServiceAccountUsers", action = "NewEvent" });

            routes.MapRoute(
                            name: "ServiceAccountCalendarsCreateEvent",
                            url: "serviceaccountusers/show/{userId}/calendar/{calendarId}/createevent",
                            defaults: new { controller = "ServiceAccountUsers", action = "CreateEvent" });
            
            routes.MapRoute(
                            name: "ServiceAccountCalendars",
                            url: "serviceaccountusers/show/{userId}/calendar/{calendarId}",
                            defaults: new { controller = "ServiceAccountUsers", action = "Calendar" });

			routes.MapRoute(
				name: "Default",
				url: "{controller}/{action}/{id}",
				defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
			);
		}
	}
}

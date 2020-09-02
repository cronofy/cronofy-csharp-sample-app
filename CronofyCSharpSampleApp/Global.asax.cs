using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Net;

namespace CronofyCSharpSampleApp
{
    public class Global : HttpApplication
    {
        protected void Application_Start()
        {
            ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }
    }
}

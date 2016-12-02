using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Cronofy;

namespace CronofyCSharpSampleApp.Controllers
{
    public class OAuthController : Controller
    {
        public ActionResult Index()
        {
			var token = CronofyHelper.GetOAuthToken(Request.QueryString["code"]);
			CronofyHelper.SetToken(token);

			var account = CronofyHelper.GetAccount();

			var recordCount = Convert.ToInt32(DatabaseHandler.Scalar($"SELECT COUNT(*) FROM Users WHERE CronofyUID='{account.Id}'"));

			if (recordCount == 0)
			{
				DatabaseHandler.ExecuteNonQuery($"INSERT INTO Users(CronofyUID, AccessToken, RefreshToken) VALUES('{account.Id}', '{token.AccessToken}', '{token.RefreshToken}')");
			}
			else 
			{
				DatabaseHandler.ExecuteNonQuery($"UPDATE Users SET AccessToken='{token.AccessToken}', RefreshToken='{token.RefreshToken}' WHERE CronofyUID='{account.Id}'");
			}

			Response.SetCookie(new HttpCookie(CronofyHelper.CookieName, account.Id));

			return new RedirectResult("/");
        }
    }
}

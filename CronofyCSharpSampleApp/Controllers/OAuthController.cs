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

			var recordCount = Convert.ToInt32(DatabaseHandler.Scalar($"SELECT COUNT(*) FROM Users WHERE CronofyUID='{account.Id}' AND ServiceAccount=0"));

			if (recordCount == 0)
			{
				DatabaseHandler.ExecuteNonQuery($"INSERT INTO Users(CronofyUID, AccessToken, RefreshToken, ServiceAccount) VALUES('{account.Id}', '{token.AccessToken}', '{token.RefreshToken}', 0)");
			}
			else 
			{
				DatabaseHandler.ExecuteNonQuery($"UPDATE Users SET AccessToken='{token.AccessToken}', RefreshToken='{token.RefreshToken}' WHERE CronofyUID='{account.Id}'");
			}

			Response.SetCookie(new HttpCookie(CronofyHelper.CookieName, account.Id));

			return new RedirectResult("/");
        }

		public ActionResult EnterpriseConnect()
		{
			var token = CronofyHelper.GetEnterpriseConnectOAuthToken(Request.QueryString["code"]);
			CronofyHelper.SetToken(token);

			var userInfo = CronofyHelper.GetUserInfo();

			var recordCount = Convert.ToInt32(DatabaseHandler.Scalar($"SELECT COUNT(*) FROM Users WHERE CronofyUID='{userInfo.Sub}' AND ServiceAccount=1"));

			if (recordCount == 0)
			{
				DatabaseHandler.ExecuteNonQuery($"INSERT INTO Users(CronofyUID, AccessToken, RefreshToken, ServiceAccount) VALUES('{userInfo.Sub}', '{token.AccessToken}', '{token.RefreshToken}', 1)");
			}
			else
			{
				DatabaseHandler.ExecuteNonQuery($"UPDATE Users SET AccessToken='{token.AccessToken}', RefreshToken='{token.RefreshToken}' WHERE CronofyUID='{userInfo.Sub}'");
			}

			Response.SetCookie(new HttpCookie(CronofyHelper.EnterpriseConnectCookieName, userInfo.Sub));

			return new RedirectResult("/enterpriseconnect");
		}
    }
}

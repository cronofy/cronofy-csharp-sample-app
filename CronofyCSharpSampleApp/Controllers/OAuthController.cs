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
			CronofyHelper.SetToken(token, false);

			var account = CronofyHelper.GetAccount();

			var recordCount = Convert.ToInt32(DatabaseHandler.Scalar($"SELECT COUNT(*) FROM UserCredentials WHERE CronofyUID='{account.Id}' AND ServiceAccount=0"));

			if (recordCount == 0)
			{
				DatabaseHandler.ExecuteNonQuery($"INSERT INTO UserCredentials(CronofyUID, AccessToken, RefreshToken, ServiceAccount) VALUES('{account.Id}', '{token.AccessToken}', '{token.RefreshToken}', 0)");
                LogHelper.Log($"Create user credentials record - accountId=`{account.Id}`");
			}
			else 
			{
				DatabaseHandler.ExecuteNonQuery($"UPDATE UserCredentials SET AccessToken='{token.AccessToken}', RefreshToken='{token.RefreshToken}' WHERE CronofyUID='{account.Id}'");
                LogHelper.Log($"Update user credentials record - accountId=`{account.Id}`");
			}

			Response.SetCookie(new HttpCookie(CronofyHelper.CookieName, account.Id));

			return new RedirectResult("/");
        }

		public ActionResult EnterpriseConnect()
		{
			var token = CronofyHelper.GetEnterpriseConnectOAuthToken(Request.QueryString["code"]);
			CronofyHelper.SetToken(token, true);

			var userInfo = CronofyHelper.GetEnterpriseConnectUserInfo();

			var recordCount = Convert.ToInt32(DatabaseHandler.Scalar($"SELECT COUNT(*) FROM UserCredentials WHERE CronofyUID='{userInfo.Sub}' AND ServiceAccount=1"));

			if (recordCount == 0)
			{
				DatabaseHandler.ExecuteNonQuery($"INSERT INTO UserCredentials(CronofyUID, AccessToken, RefreshToken, ServiceAccount) VALUES('{userInfo.Sub}', '{token.AccessToken}', '{token.RefreshToken}', 1)");
                LogHelper.Log($"Create enterprise connect user credentials record - sub=`{userInfo.Sub}`");
			}
			else
			{
				DatabaseHandler.ExecuteNonQuery($"UPDATE UserCredentials SET AccessToken='{token.AccessToken}', RefreshToken='{token.RefreshToken}' WHERE CronofyUID='{userInfo.Sub}'");
                LogHelper.Log($"Update enterprise connect user credentials record - sub=`{userInfo.Sub}`");
			}

			Response.SetCookie(new HttpCookie(CronofyHelper.EnterpriseConnectCookieName, userInfo.Sub));

			return new RedirectResult("/enterpriseconnect");
		}
    }
}

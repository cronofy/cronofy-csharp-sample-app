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

            var recordCount = Convert.ToInt32(DatabaseHandler.Scalar("SELECT COUNT(*) FROM UserCredentials WHERE CronofyUID=@accountId AND ServiceAccount=0",
                                                                     new Dictionary<string, object> { { "accountId", account.Id } }));

            if (recordCount == 0)
            {
                DatabaseHandler.ExecuteNonQuery("INSERT INTO UserCredentials(CronofyUID, AccessToken, RefreshToken, ServiceAccount) VALUES(@cronofyUid, @accessToken, @refreshToken, 0)",
                                                new Dictionary<string, object> { { "cronofyUid", account.Id }, { "accessToken", token.AccessToken }, { "refreshToken", token.RefreshToken } });
                LogHelper.Log(String.Format("Create user credentials record - accountId=`{0}`", account.Id));
            }
            else 
            {
                DatabaseHandler.ExecuteNonQuery("UPDATE UserCredentials SET AccessToken=@accessToken, RefreshToken=@refreshToken WHERE CronofyUID=@cronofyUid",
                                                new Dictionary<string, object> { { "accessToken", token.AccessToken }, { "refreshToken", token.RefreshToken }, { "cronofyUid", account.Id } });
                LogHelper.Log(String.Format("Update user credentials record - accountId=`{0}`", account.Id));
            }

            Response.SetCookie(new HttpCookie(CronofyHelper.CookieName, account.Id));

            return new RedirectResult("/");
        }

        public ActionResult EnterpriseConnect()
        {
            var token = CronofyHelper.GetEnterpriseConnectOAuthToken(Request.QueryString["code"]);
            CronofyHelper.SetToken(token, true);

            var userInfo = CronofyHelper.GetEnterpriseConnectUserInfo();

            var recordCount = Convert.ToInt32(DatabaseHandler.Scalar("SELECT COUNT(*) FROM UserCredentials WHERE CronofyUID=@cronofyUid AND ServiceAccount=1",
                                                                     new Dictionary<string, object> { { "cronofyUid", userInfo.Sub } }));

            if (recordCount == 0)
            {
                DatabaseHandler.ExecuteNonQuery("INSERT INTO UserCredentials(CronofyUID, AccessToken, RefreshToken, ServiceAccount) VALUES(@cronofyUid, @accessToken, @refreshToken, 1)",
                                                new Dictionary<string, object> { { "cronofyUid", userInfo.Sub }, { "accessToken", token.AccessToken }, { "refreshToken", token.RefreshToken } });
                LogHelper.Log(String.Format("Create enterprise connect user credentials record - sub=`{0}`", userInfo.Sub));
            }
            else
            {
                DatabaseHandler.ExecuteNonQuery("UPDATE UserCredentials SET AccessToken=@accessToken, RefreshToken=@refreshToken WHERE CronofyUID=@cronofyUid",
                                                new Dictionary<string, object> { { "accessToken", token.AccessToken }, { "refreshToken", token.RefreshToken }, { "cronofyUid", userInfo.Sub } });
                LogHelper.Log(String.Format("Update enterprise connect user credentials record - sub=`{0}`", userInfo.Sub));
            }

            Response.SetCookie(new HttpCookie(CronofyHelper.EnterpriseConnectCookieName, userInfo.Sub));

            return new RedirectResult("/enterpriseconnect");
        }
    }
}

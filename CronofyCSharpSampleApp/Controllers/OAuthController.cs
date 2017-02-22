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

            var recordCount = Convert.ToInt32(DatabaseHandler.Scalar(String.Format("SELECT COUNT(*) FROM UserCredentials WHERE CronofyUID='{0}' AND ServiceAccount=0", account.Id)));

            if (recordCount == 0)
            {
                DatabaseHandler.ExecuteNonQuery(String.Format("INSERT INTO UserCredentials(CronofyUID, AccessToken, RefreshToken, ServiceAccount) VALUES('{0}', '{1}', '{2}', 0)", account.Id, token.AccessToken, token.RefreshToken));
                LogHelper.Log(String.Format("Create user credentials record - accountId=`{0}`", account.Id));
            }
            else 
            {
                DatabaseHandler.ExecuteNonQuery(String.Format("UPDATE UserCredentials SET AccessToken='{0}', RefreshToken='{1}' WHERE CronofyUID='{2}'", token.AccessToken, token.RefreshToken, account.Id));
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

            var recordCount = Convert.ToInt32(DatabaseHandler.Scalar(String.Format("SELECT COUNT(*) FROM UserCredentials WHERE CronofyUID='{0}' AND ServiceAccount=1", userInfo.Sub)));

            if (recordCount == 0)
            {
                DatabaseHandler.ExecuteNonQuery(String.Format("INSERT INTO UserCredentials(CronofyUID, AccessToken, RefreshToken, ServiceAccount) VALUES('{0}', '{1}', '{2}', 1)", userInfo.Sub, token.AccessToken, token.RefreshToken));
                LogHelper.Log(String.Format("Create enterprise connect user credentials record - sub=`{0}`", userInfo.Sub));
            }
            else
            {
                DatabaseHandler.ExecuteNonQuery(String.Format("UPDATE UserCredentials SET AccessToken='{0}', RefreshToken='{1}' WHERE CronofyUID='{2}'", token.AccessToken, token.RefreshToken, userInfo.Sub));
                LogHelper.Log(String.Format("Update enterprise connect user credentials record - sub=`{0}`", userInfo.Sub));
            }

            Response.SetCookie(new HttpCookie(CronofyHelper.EnterpriseConnectCookieName, userInfo.Sub));

            return new RedirectResult("/enterpriseconnect");
        }
    }
}

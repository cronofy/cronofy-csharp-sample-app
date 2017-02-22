using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CronofyCSharpSampleApp.Persistence.Models;
using Newtonsoft.Json;

namespace CronofyCSharpSampleApp.Controllers
{
    public class ServiceAccountCallbackController : Controller
    {
        public ActionResult Authorize([Bind(Prefix = "id")] string enterpriseConnectId, string email)
        {
            LogHelper.Log(String.Format("Receive Service Account callback - enterpriseConnectId=`{0}`", enterpriseConnectId));

            try
            {
                var inputStream = Request.InputStream;
                inputStream.Seek(0, SeekOrigin.Begin);
                var body = new StreamReader(inputStream).ReadToEnd();

                var data = JsonConvert.DeserializeObject<ServiceAccountAuthorizationData>(body);

                if (data.Authorization.Error != null)
                {
                    DatabaseHandler.ExecuteNonQuery(String.Format("UPDATE EnterpriseConnectUserData SET Status='{0}' WHERE Email='{1}' AND OwnedBy='{2}'", (int)EnterpriseConnectUserData.ConnectedStatus.Failed, email, enterpriseConnectId));

                    LogHelper.Log(String.Format("Service Account callback failure - error=`{0}` - error_key=`{1}` - error_description=`{2}`", data.Authorization.Error, data.Authorization.ErrorKey, data.Authorization.ErrorDescription));
                }
                else 
                {
                    var userToken = CronofyHelper.GetEnterpriseConnectUserOAuthToken(enterpriseConnectId, email, data.Authorization.Code);

                    DatabaseHandler.ExecuteNonQuery(String.Format("INSERT INTO UserCredentials(CronofyUID, AccessToken, RefreshToken, ServiceAccount) VALUES('{0}', '{1}', '{2}', 0)", userToken.LinkingProfile.Id, userToken.AccessToken, userToken.RefreshToken));
                    DatabaseHandler.ExecuteNonQuery(String.Format("UPDATE EnterpriseConnectUserData SET Status='{0}', CronofyUID='{1}' WHERE Email='{2}' AND OwnedBy='{3}'", (int)EnterpriseConnectUserData.ConnectedStatus.Linked, userToken.LinkingProfile.Id, userToken.LinkingProfile.Name, enterpriseConnectId));

                    LogHelper.Log(String.Format("Service Account callback success - id=`{0}` - email=`{1}`", userToken.LinkingProfile.Id, userToken.LinkingProfile.Name));
                }
            }
            catch (Exception ex)
            {
                LogHelper.Log(String.Format("Service Account callback failure - ex.Source=`{0}` - ex.Message=`{1}`", ex.Source, ex.Message));
            }

            return new EmptyResult();
        }
    }
}

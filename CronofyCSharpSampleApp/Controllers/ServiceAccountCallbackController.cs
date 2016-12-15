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
            LogHelper.Log($"Receive Service Account callback - enterpriseConnectId=`{enterpriseConnectId}`");

            try
            {
                var inputStream = Request.InputStream;
                inputStream.Seek(0, SeekOrigin.Begin);
                var body = new StreamReader(inputStream).ReadToEnd();

                var data = JsonConvert.DeserializeObject<ServiceAccountAuthorizationData>(body);

                if (data.Authorization.Error != null)
                {
                    DatabaseHandler.ExecuteNonQuery($"UPDATE EnterpriseConnectUserData SET Status='{(int)EnterpriseConnectUserData.ConnectedStatus.Failed}' WHERE Email='{email}' AND OwnedBy='{enterpriseConnectId}'");

                    LogHelper.Log($"Service Account callback failure - error=`{data.Authorization.Error}` - error_key=`{data.Authorization.ErrorKey}` - error_description=`{data.Authorization.ErrorDescription}`");
                }
                else 
                {
                    var userToken = CronofyHelper.GetEnterpriseConnectUserOAuthToken(enterpriseConnectId, email, data.Authorization.Code);

                    DatabaseHandler.ExecuteNonQuery($"INSERT INTO UserCredentials(CronofyUID, AccessToken, RefreshToken, ServiceAccount) VALUES('{userToken.LinkingProfile.Id}', '{userToken.AccessToken}', '{userToken.RefreshToken}', 0)");
                    DatabaseHandler.ExecuteNonQuery($"UPDATE EnterpriseConnectUserData SET Status='{(int)EnterpriseConnectUserData.ConnectedStatus.Linked}', CronofyUID='{userToken.LinkingProfile.Id}' WHERE Email='{userToken.LinkingProfile.Name}' AND OwnedBy='{enterpriseConnectId}'");

                    LogHelper.Log($"Service Account callback success - id=`{userToken.LinkingProfile.Id}` - email=`{userToken.LinkingProfile.Name}`");
                }
            }
            catch (Exception ex)
            {
                LogHelper.Log($"Service Account callback failure - ex.Source=`{ex.Source}` - ex.Message=`{ex.Message}`");
            }

            return new EmptyResult();
        }
    }
}

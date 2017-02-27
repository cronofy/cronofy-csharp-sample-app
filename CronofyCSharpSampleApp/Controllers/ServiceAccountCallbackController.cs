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
                    DatabaseHandler.ExecuteNonQuery("UPDATE EnterpriseConnectUserData SET Status=@status WHERE Email=@email AND OwnedBy=@ownedBy",
                                                    new Dictionary<string, object> { { "status", (int)EnterpriseConnectUserData.ConnectedStatus.Failed }, { "email", email }, { "ownedBy", enterpriseConnectId } });

                    LogHelper.Log(String.Format("Service Account callback failure - error=`{0}` - error_key=`{1}` - error_description=`{2}`", data.Authorization.Error, data.Authorization.ErrorKey, data.Authorization.ErrorDescription));
                }
                else 
                {
                    var userToken = CronofyHelper.GetEnterpriseConnectUserOAuthToken(enterpriseConnectId, email, data.Authorization.Code);

                    DatabaseHandler.ExecuteNonQuery("INSERT INTO UserCredentials(CronofyUID, AccessToken, RefreshToken, ServiceAccount) VALUES(@cronofyUid, @accessToken, @refreshToken, 0)",
                                                    new Dictionary<string, object> { { "cronofyUid", userToken.LinkingProfile.Id }, { "accessToken", userToken.AccessToken }, { "refreshToken", userToken.RefreshToken } });
                    DatabaseHandler.ExecuteNonQuery("UPDATE EnterpriseConnectUserData SET Status=@status, CronofyUID=@cronofyUid WHERE Email=@email AND OwnedBy=@ownedBy",
                                                    new Dictionary<string, object> { { "status", (int)EnterpriseConnectUserData.ConnectedStatus.Linked }, { "cronofyUid", userToken.LinkingProfile.Id }, { "email", userToken.LinkingProfile.Name }, { "ownedBy", enterpriseConnectId } });

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

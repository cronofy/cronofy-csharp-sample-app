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
        public ActionResult Authorize([Bind(Prefix = "id")] string enterpriseConnectId)
        {
            var inputStream = Request.InputStream;
            inputStream.Seek(0, SeekOrigin.Begin);
            var body = new StreamReader(inputStream).ReadToEnd();

            var data = JsonConvert.DeserializeObject<ServiceAccountAuthorizationData>(body);

            var userToken = CronofyHelper.GetEnterpriseConnectUserOAuthToken(enterpriseConnectId, data.Authorization.Code);
            CronofyHelper.SetToken(userToken, false);

            DatabaseHandler.ExecuteNonQuery($"INSERT INTO Users(CronofyUID, AccessToken, RefreshToken) VALUES('{userToken.LinkingProfile.Id}', '{userToken.AccessToken}', '{userToken.RefreshToken}')");
            DatabaseHandler.ExecuteNonQuery($"UPDATE EnterpriseConnectUserData SET Status='{(int)EnterpriseConnectUserData.ConnectedStatus.Linked}', CronofyUID='{userToken.LinkingProfile.Id}' WHERE Email='{userToken.LinkingProfile.Name}' AND OwnedBy='{enterpriseConnectId}'");

            return new EmptyResult();
        }
    }
}

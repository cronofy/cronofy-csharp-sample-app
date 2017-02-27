using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CronofyCSharpSampleApp.Controllers
{
    public class PushController : Controller
    {
        [HttpPost]
        public ActionResult Channel(string id)
        {
            LogHelper.Log(String.Format("Receive push notification - id=`{0}`", id));

            try
            {
                var inputStream = Request.InputStream;
                inputStream.Seek(0, SeekOrigin.Begin);
                var body = new StreamReader(inputStream).ReadToEnd();

                var data = JsonConvert.DeserializeObject<Models.ChannelData>(body);

                var record = String.Format("{0} - {1}", DateTime.UtcNow, data.Notification.Type);
                if (data.Notification.ChangesSince.HasValue)
                {
                    record += String.Format(": {0}", data.Notification.ChangesSince);
                }

                DatabaseHandler.ExecuteNonQuery("INSERT INTO ChannelData(ChannelId, Record, OccurredOn) VALUES(@channelId, @record, @occurredOn)",
                                                new Dictionary<string, object> { { "channelId", data.Channel.ChannelId }, { "record", record }, { "occurredOn", DateTime.Now } });

                LogHelper.Log(String.Format("Push notification success - channelId=`{0}`", data.Channel.ChannelId));
            }
            catch (Exception ex)
            {
                LogHelper.Log(String.Format("Push notification failure - ex.Source=`{0}` - ex.Message=`{1}`", ex.Source, ex.Message));
            }

            return new EmptyResult();
        }
    }
}

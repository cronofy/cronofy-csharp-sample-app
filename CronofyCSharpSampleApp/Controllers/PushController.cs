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
            LogHelper.Log($"Receive push notification - id=`{id}`");

            try
            {
                var inputStream = Request.InputStream;
                inputStream.Seek(0, SeekOrigin.Begin);
                var body = new StreamReader(inputStream).ReadToEnd();

                var data = JsonConvert.DeserializeObject<ChannelData>(body);

                var records = data.Notification.Select(x => $"{x.Key}: {x.Value}");

                DatabaseHandler.ExecuteNonQuery($"INSERT INTO ChannelData(ChannelId, Record, OccurredOn) VALUES('{data.Channel.ChannelId}', '{String.Join("\n", records)}', '{DateTime.Now}')");

                LogHelper.Log($"Push notification success - channelId=`{data.Channel.ChannelId}`");
            }
            catch (Exception ex)
            {
                LogHelper.Log($"Push notification failure - ex.Source=`{ex.Source}` - ex.Message=`{ex.Message}`");
            }

			return new EmptyResult();
		}

		public class ChannelData
		{
			[JsonProperty("notification")]
			public IDictionary<string, string> Notification { get; set; }

			[JsonProperty("channel")]
			public NotifyingChannel Channel { get; set; }

			public class NotifyingChannel
			{
				[JsonProperty("channel_id")]
				public string ChannelId { get; set; }
			}
		}
    }
}

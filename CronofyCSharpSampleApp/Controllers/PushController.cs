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
			var inputStream = Request.InputStream;
			inputStream.Seek(0, SeekOrigin.Begin);
			var body = new StreamReader(inputStream).ReadToEnd();

			var data = JsonConvert.DeserializeObject<ChannelData>(body);

			var records = data.Notification.Select(x => $"{x.Key}: {x.Value}");

			DatabaseHandler.ExecuteNonQuery($"INSERT INTO ChannelData(ChannelId, Record, OccurredOn) VALUES('{data.Channel.ChannelId}', '{String.Join("\n", records)}', '{DateTime.Now}')");

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

			/*public class NotificationData
			{
				[JsonProperty("type")]
				public string Type { get; set; }

				[JsonProperty("changes_since")]
				public DateTime ChangesSince { get; set; }
			}*/
		}
    }
}

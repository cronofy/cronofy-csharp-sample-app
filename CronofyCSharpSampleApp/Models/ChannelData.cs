﻿using System;
using Newtonsoft.Json;

namespace CronofyCSharpSampleApp.Models
{
    public class ChannelData
    {
        [JsonProperty("notification")]
        public NotificationData Notification { get; set; }

        [JsonProperty("channel")]
        public NotifyingChannel Channel { get; set; }

        public class NotifyingChannel
        {
            [JsonProperty("channel_id")]
            public string ChannelId { get; set; }

            [JsonProperty("callback_url")]
            public string CallbackUrl { get; set; }

            [JsonProperty("filters")]
            public Cronofy.Channel.ChannelFilters Filters { get; set; }
        }
        public class NotificationData
        {
            [JsonProperty("type")]
            public string Type { get; set; }

            [JsonProperty("changes_since")]
            public DateTime ChangesSince { get; set; }
        }
    }
}

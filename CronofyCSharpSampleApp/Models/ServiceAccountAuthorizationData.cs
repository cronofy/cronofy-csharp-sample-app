using System;
using Newtonsoft.Json;

namespace CronofyCSharpSampleApp
{
    public class ServiceAccountAuthorizationData
    {
        [JsonProperty("authorization")]
        public AuthorizationData Authorization { get; set; }

        public class AuthorizationData
        {
            [JsonProperty("code")]
            public string Code { get; set; }
        }
    }
}

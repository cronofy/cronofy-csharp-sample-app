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

            [JsonProperty("error")]
            public string Error { get; set; }

            [JsonProperty("error_key")]
            public string ErrorKey { get; set; }

            [JsonProperty("error_description")]
            public string ErrorDescription { get; set; }
        }
    }
}

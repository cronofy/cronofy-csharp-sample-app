using System;
using System.Net;
using System.Web;
using Newtonsoft.Json;

namespace CronofyCSharpSampleApp
{
    public static class ModelHelper
    {
        public static IHtmlString ResponseError(this ResourceWithError resource)
        {
            if (resource.NoErrors())
            {
                return null;
            }

            var error = JsonConvert.DeserializeObject<object>(HttpUtility.HtmlDecode(resource.Error));

            var formattedError = "<pre>" + JsonConvert.SerializeObject(error, Formatting.Indented) + "</pre>";

            return new HtmlString(string.Format(@"
            <div id='error_explanation' class='alert alert-danger'>
                {0}
                {1}
            </div>", ErrorCodeString(resource.ErrorCode.Value), error != null ? formattedError : string.Empty));
        }

        private static string ErrorCodeString(HttpStatusCode code)
        {
            var codeName = code.ToString();

            if((int)code == 422)
            {
                codeName = "Unprocessable";
            }

            return "<h4>" + (int)code + @" - " + codeName + @"</h4>";
        }
    }
}

using System;
using System.Web;
using Newtonsoft.Json;

namespace CronofyCSharpSampleApp
{
    public static class ModelHelper
    {
        public static IHtmlString ResponseError(this ResourceWithError resource)
        {
            if (string.IsNullOrWhiteSpace(resource.Error))
            {
                return null;
            }

            var error = JsonConvert.DeserializeObject<object>(HttpUtility.HtmlDecode(resource.Error));
            var formattedError = JsonConvert.SerializeObject(error, Formatting.Indented);

            return new HtmlString(@"
            <div id='error_explanation' class='alert alert-danger'>
                <h4>Invalid Request Error</h4>
                <pre>" + formattedError + @"</pre>
            </div>");
        }
    }
}

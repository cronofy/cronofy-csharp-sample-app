using System.Net;
using Cronofy;

namespace CronofyCSharpSampleApp
{
    public abstract class ResourceWithError
    {
        public HttpStatusCode? ErrorCode { get; set; }
        public string Error { get; set; }

        public void SetError(CronofyResponseException ex)
        {
            ErrorCode = (HttpStatusCode)ex.Response.Code;
            Error = ex.Response.Body;
        }

        public bool NoErrors()
        {
            return !ErrorCode.HasValue;
        }
    }
}

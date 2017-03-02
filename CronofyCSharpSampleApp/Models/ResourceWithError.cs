using Cronofy;

namespace CronofyCSharpSampleApp
{
    public abstract class ResourceWithError
    {
        public string Error { get; set; }

        public void SetError(CronofyResponseException ex)
        {
            Error = ex.Response.Body;
        }
    }
}

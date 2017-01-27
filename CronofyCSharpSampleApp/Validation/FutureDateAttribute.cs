using System;
using System.ComponentModel.DataAnnotations;

namespace CronofyCSharpSampleApp
{
    public class FutureDateAttribute : ValidationAttribute
    {
        public override Boolean IsValid(object value)
        {
            return value != null && (DateTime)value > DateTime.Now;
        }
    }
}

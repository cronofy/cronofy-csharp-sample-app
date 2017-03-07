using System;
using System.ComponentModel.DataAnnotations;

namespace CronofyCSharpSampleApp.Models
{
    public class Calendar : ResourceWithError
    {
        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Profile ID is required")]
        public string ProfileId { get; set; }
    }
}

using System;
using System.ComponentModel.DataAnnotations;

namespace CronofyCSharpSampleApp.Models
{
    public class EnterpriseConnectUser
    {
        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Scopes are required")]
        public string Scopes { get; set; }
    }
}

using System;
using System.ComponentModel.DataAnnotations;

namespace CronofyCSharpSampleApp.Models
{
    public class Event
    {
        // Used to track enterprise connected user's ID
        public string UserId { get; set; }

        [Required(ErrorMessage = "Calendar ID is required")]
        public string CalendarId { get; set; }

        [Required(ErrorMessage = "Event ID is required")]
        public string EventId { get; set; }

        [Required(ErrorMessage = "Summary is required")]
        public string Summary { get; set; }

        public string Description { get; set; }

        [Required(ErrorMessage = "Start time is required")]
        public DateTime Start { get; set; }

        [Required(ErrorMessage = "End time is required")]
        public DateTime End { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CronofyCSharpSampleApp.Models
{
    public class Event : ResourceWithError, IValidatableObject
    {
        public Cronofy.Calendar Calendar { get; set; }

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

        public string LocationDescription { get; set; }

        public string Latitude { get; set; }
        public string Longitude { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var results = new List<ValidationResult>();

            if (!(String.IsNullOrEmpty(Latitude) && String.IsNullOrEmpty(Longitude)))
            {
                float lat, lng;

                if (String.IsNullOrEmpty(Latitude))
                {
                    results.Add(new ValidationResult("must be set if longitude is set", new[] { "Latitude" }));
                }
                else if (!float.TryParse(Latitude, out lat))
                {
                    results.Add(new ValidationResult("must be a float", new[] { "Latitude" }));
                }
                else if (lat < -85.05115 || lat > 85.05115)
                {
                    results.Add(new ValidationResult("must be between -85.05115 and 85.05115", new[] { "Latitude" }));
                }

                if (String.IsNullOrEmpty(Longitude))
                {
                    results.Add(new ValidationResult("must be set if latitude is set", new[] { "Longitude" }));
                }
                else if (!float.TryParse(Longitude, out lng))
                {
                    results.Add(new ValidationResult("must be a float", new[] { "Longitude" }));
                }
                else if (lng < -180 || lng > 180)
                {
                    results.Add(new ValidationResult("must be between -180 and 180", new[] { "Longitude" }));
                }
            }

            return results;
        }
    }
}

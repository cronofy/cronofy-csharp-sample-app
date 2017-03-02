using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CronofyCSharpSampleApp.Models
{
    public class Availability : ResourceWithError, IValidatableObject
    {
        public string AuthUrl { get; set; }

        public IEnumerable<Cronofy.AvailablePeriod> AvailablePeriods { get; set; }

        [Required]
        public string AccountId1 { get; set; }

        public string AccountId2 { get; set; }

        [Required]
        public string RequiredParticipants { get; set; }

        [Required]
        public int Duration { get; set; }

        [Required]
        public DateTime Start { get; set; }

        [Required]
        public DateTime End { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!(RequiredParticipants == "All" || RequiredParticipants == "1"))
                yield return new ValidationResult("Required participants must be All or 1", new[] { "RequiredParticipants" });

            if (Start < DateTime.Now)
                yield return new ValidationResult("Start time must be in the future", new[] { "Start" });

            if (End < Start)
                yield return new ValidationResult("End time must be after Start time", new[] { "Start", "End" });

            if ((Start - DateTime.Now).TotalDays >= 35)
                yield return new ValidationResult("Start time must be within 35 days of today", new[] { "Start" });

            if ((End - Start).TotalDays > 1)
                yield return new ValidationResult("End time must be within 24 hours and 1 minute of start time", new[] { "Start", "End" });
        }
    }
}

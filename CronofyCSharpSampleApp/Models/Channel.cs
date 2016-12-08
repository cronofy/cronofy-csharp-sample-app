using System;
using System.Collections.Generic;

namespace CronofyCSharpSampleApp.Models
{
	public class Channel
	{
		public string Path { get; set; }
		public bool OnlyManaged { get; set; }
		public IEnumerable<string> CalendarIds { get; set; }
	}
}

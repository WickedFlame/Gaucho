using System;
using System.Collections.Generic;
using System.Text;
using Gaucho.Diagnostics;

namespace Gaucho.Dashboard.Monitoring
{
	public class DashboardElements
	{
		public string Key { get; set; }

		public string Title { get; set; }

		public List<object> Elements { get; } = new List<object>();
	}

	public class DashboardLog
	{
		public DateTime Timestamp { get; set; }

		public string Message { get; set; }

		public string Level { get; set; }

		public string Source { get; set; }
	}
}

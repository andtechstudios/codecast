using System;

namespace Andtech.Codecast.Console
{
	public class UnityLogEntry
	{
		public LogType LogType => Enum.Parse<LogType>(logType);
		public DateTime Timestamp => DateTime.Parse(timestamp);

		public string logType { get; set; }
		public string message { get; set; }
		public string timestamp { get; set; }
	}

}


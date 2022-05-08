using System;
using UnityEngine;

namespace Andtech.Codecast
{
	public class UnityLogEntry
	{
		public string message;
		public string logType;
		public string timestamp;

		public UnityLogEntry(object message, LogType logType)
		{
			this.message = message.ToString();
			this.logType = logType.ToString();
			timestamp = DateTime.UtcNow.ToString("o");
		}

		public static UnityLogEntry Log(string message) => new UnityLogEntry(message, LogType.Log);

		public static UnityLogEntry Warning(string message) => new UnityLogEntry(message, LogType.Warning);

		public static UnityLogEntry Error(string message) => new UnityLogEntry(message, LogType.Error);
	}
}
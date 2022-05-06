using System;
using UnityEngine;

namespace Andtech.Codecast
{

	public abstract class BaseLogger : ILogger
	{
		public ILogHandler logHandler { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public bool logEnabled { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public LogType filterLogType { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

		public bool IsLogTypeAllowed(LogType logType) => true;

		public void Log(LogType logType, object message)
		{
			var data = new UnityLogEntry()
			{
				logType = logType.ToString(),
				message = message.ToString(),
				timestamp = DateTime.UtcNow.ToString("o"),
			};

			OnLog(data);
		}

		public void Log(LogType logType, object message, UnityEngine.Object context) => throw new NotSupportedException();

		public void Log(LogType logType, string tag, object message) => throw new NotSupportedException();

		public void Log(LogType logType, string tag, object message, UnityEngine.Object context) => throw new NotSupportedException();

		public void Log(object message) => Log(LogType.Log, message);

		public void Log(string tag, object message) => throw new NotSupportedException();

		public void Log(string tag, object message, UnityEngine.Object context) => throw new NotSupportedException();

		public void LogError(string tag, object message) => throw new NotSupportedException();

		public void LogError(string tag, object message, UnityEngine.Object context) => throw new NotSupportedException();

		public void LogException(Exception exception) => Log(LogType.Exception, exception);

		public void LogException(Exception exception, UnityEngine.Object context) => throw new NotSupportedException();

		public void LogFormat(LogType logType, string format, params object[] args)
		{
			var data = new UnityLogEntry()
			{
				logType = logType.ToString(),
				message = string.Format(format, args),
				timestamp = DateTime.UtcNow.ToString("o"),
			};

			OnLog(data);
		}

		public void LogFormat(LogType logType, UnityEngine.Object context, string format, params object[] args) => throw new NotSupportedException();

		public void LogWarning(string tag, object message) => throw new NotSupportedException();

		public void LogWarning(string tag, object message, UnityEngine.Object context) => throw new NotSupportedException();

		public event Action<UnityLogEntry> OnLog;
	}

}
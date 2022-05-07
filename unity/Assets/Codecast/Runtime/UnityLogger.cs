using System;
using UnityEngine;

namespace Andtech.Codecast
{
	public class UnityLogger : BaseLogger
	{
		public CodecastBroadcaster Broadcaster => broadcaster;

		private readonly CodecastBroadcaster broadcaster;
		private bool isActive;

		public UnityLogger(CodecastBroadcaster broadcaster)
		{
			this.broadcaster = broadcaster;
		}

		~UnityLogger()
		{
			if (isActive)
			{
				Stop();
			}
		}

		public void Start()
		{
			if (isActive)
			{
				Debug.LogWarning("The logger is already active.");
				return;
			}

			Application.logMessageReceived += Application_logMessageReceived;

			isActive = true;
		}

		public void Stop()
		{
			if (!isActive)
			{
				Debug.LogWarning("The logger is not active.");
				return;
			}

			Application.logMessageReceived -= Application_logMessageReceived;

			isActive = false;
		}

		private void Application_logMessageReceived(string message, string stackTrace, LogType logType)
		{
			if (!isActive)
			{
				Debug.LogWarning("The logger is not active.");
				return;
			}

			var data = new UnityLogEntry(message, logType);

			broadcaster.Send(JsonUtility.ToJson(data));
		}
	}

}
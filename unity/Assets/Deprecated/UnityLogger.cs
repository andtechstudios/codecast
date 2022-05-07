using System;
using UnityEngine;

namespace Andtech.Codecast
{
	public class UnityLogger : BaseLogger
	{
		public CodecastBroadcaster Broadcaster => broadcaster;
		public bool Enabled { get; set; }

		private readonly CodecastBroadcaster broadcaster;
		private bool isActive;

		public UnityLogger(CodecastBroadcaster broadcaster)
		{
			this.broadcaster = broadcaster;
		}

		~UnityLogger()
		{
			Stop();
		}

		public void Start()
		{
			if (isActive)
			{
				return;
			}

			Application.logMessageReceived += Application_logMessageReceived;

			isActive = true;
		}

		public void Stop()
		{
			if (!isActive)
			{
				return;
			}

			Application.logMessageReceived -= Application_logMessageReceived;

			isActive = false;
		}

		private void Application_logMessageReceived(string message, string stackTrace, LogType logType)
		{
			if (!Enabled)
			{
				return;
			}

			var data = new UnityLogEntry(message, logType);
			broadcaster.Send(JsonUtility.ToJson(data));
		}
	}

}
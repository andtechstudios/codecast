using UnityEngine;

namespace Andtech.Codecast
{
	public class UnityLogger : BaseLogger
	{
		public CodecastBroadcaster Broadcaster => broadcaster;

		private readonly CodecastBroadcaster broadcaster;

		public UnityLogger(CodecastBroadcaster broadcaster)
		{
			this.broadcaster = broadcaster;

			OnLog += UnityLogger_OnLog;
		}

		private void UnityLogger_OnLog(UnityLogEntry obj)
		{
			broadcaster.Send(JsonUtility.ToJson(obj));
		}
	}

}
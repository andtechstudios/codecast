using UnityEngine;

namespace Andtech.Codecast
{

#if UNITY_EDITOR
	[UnityEditor.InitializeOnLoad]
#endif
	public static partial class Codecast
	{
		public static bool SendUnityLogs { get; set; }
		public static bool RawUnityLogs { get; set; }
		public static bool IsActive => broadcaster?.IsActive ?? false;
		public static bool IsConnected => broadcaster?.Connected ?? false;
		public static BufferInfo ClientBufferInfo => broadcaster?.BufferInfo ?? default;

		private static CodecastBroadcaster broadcaster;

		static Codecast()
		{
			Application.logMessageReceived += Application_logMessageReceived;

#if UNITY_EDITOR
			Initialize();
#endif
		}

		static void Initialize()
		{
			var ipAddress = UnityEditor.EditorPrefs.GetString("ipAddress", "127.0.0.1");
			var port = UnityEditor.EditorPrefs.GetInt("port", 8080);
			var autoStart = UnityEditor.EditorPrefs.GetBool("autoStart", false);
			SendUnityLogs = UnityEditor.EditorPrefs.GetBool("sendUnityLogs", false);
			RawUnityLogs = UnityEditor.EditorPrefs.GetBool("rawUnityLogs", false);

			if (autoStart)
			{
				Start(ipAddress, port);
			}
		}

		public static void Start(string ipAddress, int port = 8080)
		{
			broadcaster = new CodecastBroadcaster(ipAddress, port);
			broadcaster.Start();
		}

		public static void Stop()
		{
			broadcaster?.Stop();
		}

		public static void Clear()
		{
			broadcaster?.Clear();
		}

		public static void Write(object message)
		{
#if UNITY_EDITOR
			if (IsActive)
			{
				broadcaster.Send(message.ToString());
			}
			else
			{
				Debug.LogWarning("Codecast server not active");
			}
#else
#endif
		}

		public static void WriteLine(object message)
		{
#if UNITY_EDITOR
			if (IsActive)
			{
				broadcaster.Send(message.ToString() + "\n");
			}
			else
			{
				Debug.LogWarning("Codecast server not active");
			}
#else
#endif
		}

		private static void Application_logMessageReceived(string message, string stackTrace, LogType logType)
		{
#if UNITY_EDITOR
			if (!SendUnityLogs)
			{
				return;
			}

			string data;
			if (RawUnityLogs)
			{
				data = message;
			}
			else
			{
				var entry = new UnityLogEntry(message, logType);
				data = JsonUtility.ToJson(entry);
			}

			Write(data);
#endif
		}
	}
}

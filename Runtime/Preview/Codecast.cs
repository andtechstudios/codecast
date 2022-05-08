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
		public static bool IsRunning => broadcaster?.IsRunning ?? false;
		public static bool IsConnected => broadcaster?.IsConnected ?? false;
		public static BufferInfo ClientBufferInfo => broadcaster?.BufferInfo ?? default;

		private static bool hasShownWarning;
		private static CodecastBroadcaster broadcaster;

		static Codecast()
		{
			Application.logMessageReceived += Application_logMessageReceived;

#if UNITY_EDITOR
			UnityEditor.EditorApplication.playModeStateChanged += EditorApplication_playModeStateChanged;
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
			if (IsRunning)
			{
				if (broadcaster.IsConnected)
				{
					broadcaster.Send(message.ToString());
				}
			}
			else
			{
				if (!hasShownWarning)
				{
					Debug.LogWarning("Codecast server not running");

					hasShownWarning = true;
				}
			}
#else
#endif
		}

		public static void WriteLine(object message)
		{
#if UNITY_EDITOR
			Write(message + "\n");
#else
#endif
		}

#if UNITY_EDITOR
		private static void EditorApplication_playModeStateChanged(UnityEditor.PlayModeStateChange obj)
		{
			switch (obj)
			{
				case UnityEditor.PlayModeStateChange.EnteredPlayMode:
					hasShownWarning = false;
					break;
			}
		}
#endif

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

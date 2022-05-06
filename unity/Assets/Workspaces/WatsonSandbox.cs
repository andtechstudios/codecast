using Andtech.Codecast;
using NaughtyAttributes;
using System.Net;
using UnityEngine;
using WatsonTcp;

public class WatsonSandbox : MonoBehaviour
{
	CodecastBroadcaster broadcaster;
	[TextArea(3, 999)]
	public string message;

	public UnityLogger logger;

	void Awake()
	{
		var ipAddress = IPAddress.Parse("127.0.0.1");
		var endpoint = new IPEndPoint(ipAddress, 8080);
		broadcaster = new CodecastBroadcaster(endpoint);
		broadcaster.ClientConnected += Broadcaster_ClientConnected;

		logger = new UnityLogger(broadcaster);
	}

	private void Broadcaster_ClientConnected(object sender, ConnectionEventArgs e)
	{
		logger.Log("hello world");
		logger.Log(LogType.Warning, "warning world");
		logger.Log(LogType.Error, "error world");
	}

	void Start()
	{
		broadcaster.Start();
		Debug.Log("Server started");
	}

	void OnDestroy()
	{
		broadcaster.Stop();
	}

	[Button]
	public void Send()
	{
		broadcaster.Send(message);
	}

	[Button]
	public void GetBufferInfo()
	{
		var info = broadcaster.GetClientBufferInfo();
		Debug.Log($"({info.width}, {info.height})");
	}

	[Button]
	public void Clear()
	{
		broadcaster.Clear();
	}
}

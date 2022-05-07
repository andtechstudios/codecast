using Andtech.Codecast;
using Cysharp.Threading.Tasks;
using NaughtyAttributes;
using System.Net;
using System.Threading;
using UnityEngine;

public class WatsonSandbox : MonoBehaviour
{
	CodecastBroadcaster broadcaster;
	IPEndPoint endpoint;
	[TextArea(3, 999)]
	public string message;

	public UnityLogger logger;

	void Awake()
	{
		var ipAddress = IPAddress.Parse("127.0.0.1");
		endpoint = new IPEndPoint(ipAddress, 8080);
		broadcaster = new CodecastBroadcaster(endpoint);
		broadcaster.ClientConnected += Broadcaster_ClientConnected;
	}

	private void Broadcaster_ClientConnected(object sender, WatsonTcp.ConnectionEventArgs e)
	{
		Debug.Log(broadcaster.BufferInfo);
	}

	void Start()
	{
		RunAsync(cancellationToken: this.GetCancellationTokenOnDestroy()).Forget();
	}

	async UniTask RunAsync(CancellationToken cancellationToken)
	{
		broadcaster.Start();
		Debug.Log("Broadcaster started at: " + endpoint);

		logger = new UnityLogger(broadcaster);
		logger.Start();
	}

	void OnDestroy()
	{
		broadcaster.Stop();
		logger.Stop();
	}

	[Button]
	public void SendRaw()
	{
		broadcaster.Send(message);
	}

	[Button]
	public void Send()
	{
		broadcaster.Send(JsonUtility.ToJson(UnityLogEntry.Log(message)));
	}

	[Button]
	public void SendWarning()
	{
		broadcaster.Send(JsonUtility.ToJson(UnityLogEntry.Warning(message)));
	}

	[Button]
	public void SendError()
	{
		broadcaster.Send(JsonUtility.ToJson(UnityLogEntry.Error(message)));
	}

	[Button]
	public void GetBufferInfo()
	{
		var info = broadcaster.GetClientBufferInfo();
		Debug.Log(info);
	}

	[Button]
	public void Clear()
	{
		broadcaster.Clear();
	}
}

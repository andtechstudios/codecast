using System;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using UnityEngine;
using WatsonTcp;

namespace Andtech.Codecast
{

	public class CodecastBroadcaster
	{
		public bool Connected => !string.IsNullOrEmpty(clientIpPort);
		public BufferInfo BufferInfo => clientBufferInfo;

		private readonly WatsonTcpServer server;
		private BufferInfo clientBufferInfo;
		private string clientIpPort;
		private bool hasWarned;

		public CodecastBroadcaster(IPEndPoint endpoint)
		{
			server = new WatsonTcpServer(endpoint.Address.ToString(), endpoint.Port);
			server.Events.ClientConnected += Server_ClientConnected;
			server.Events.ClientDisconnected += Server_ClientDisconnected;
			server.Events.MessageReceived += Events_MessageReceived;
		}

		public void Start()
		{
			if (!Connected)
			{
				server.Start();
			}
		}

		public void Stop()
		{
			if (Connected)
			{
				server.DisconnectClients();
			}

			server.Stop();
			clientIpPort = null;
			clientBufferInfo = null;
		}

		public void Send(string data)
		{
			if (!CheckConnection())
			{
				return;
			}

			server.Send(clientIpPort, data);
		}

		public void Clear()
		{
			if (!CheckConnection())
			{
				return;
			}

			try
			{
				var response = server.SendAndWait(5000, clientIpPort, "CLEAR");
			}
			catch (TimeoutException ex)
			{
				Debug.LogError(ex);
			}
		}

		public BufferInfo GetClientBufferInfo()
		{
			try
			{
				var response = server.SendAndWait(5000, clientIpPort, "GET_BUFFER_INFO");
				return JsonUtility.FromJson<BufferInfo>(Encoding.UTF8.GetString(response.Data));
			}
			catch (TimeoutException ex)
			{
				Debug.LogError(ex);
				return null;
			}
		}

		private void Events_MessageReceived(object sender, MessageReceivedEventArgs e)
		{
			throw new NotSupportedException();
		}

		void Server_ClientConnected(object sender, ConnectionEventArgs args)
		{
			Debug.Log("Client connected: " + args.IpPort);

			clientIpPort = args.IpPort;
			clientBufferInfo = GetClientBufferInfo();

			ClientConnected?.Invoke(this, args);
		}

		void Server_ClientDisconnected(object sender, DisconnectionEventArgs args)
		{
			Debug.Log("Client disconnected: " + args.IpPort + ": " + args.Reason.ToString());

			clientIpPort = null;
			clientBufferInfo = null;

			ClientDisconnected?.Invoke(this, args);
		}

		bool CheckConnection()
		{
			if (!Connected)
			{
				if (!hasWarned)
				{
					Debug.LogWarning("No client connected.");
					hasWarned = true;
				}

				return false;
			}

			return true;
		}

		public event EventHandler<ConnectionEventArgs> ClientConnected;
		public event EventHandler<DisconnectionEventArgs> ClientDisconnected;
	}
}
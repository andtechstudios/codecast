using System;
using System.Linq;
using System.Net;
using System.Text;
using UnityEngine;
using WatsonTcp;

namespace Andtech.Codecast
{

	public class BufferInfo
	{
		public int width;
		public int height;
	}

	public class CodecastBroadcaster
	{
		private readonly WatsonTcpServer server;

		public CodecastBroadcaster(IPEndPoint endpoint)
		{
			server = new WatsonTcpServer(endpoint.Address.ToString(), endpoint.Port);
			server.Events.ClientConnected += Server_ClientConnected;
			server.Events.ClientDisconnected += Server_ClientDisconnected;
			server.Events.MessageReceived += Events_MessageReceived;
		}

		private void Events_MessageReceived(object sender, MessageReceivedEventArgs e)
		{
			throw new NotSupportedException();
		}

		public void Start()
		{
			server.Start();
		}

		public void Stop()
		{
			server.DisconnectClients();
			server.Stop();
		}

		public void Send(string data)
		{
			foreach (var client in server.ListClients())
			{
				server.Send(client, data);
			}
		}

		public BufferInfo GetClientBufferInfo()
		{
			var client = server.ListClients().First();
			try
			{
				var response = server.SendAndWait(5000, client, "GET_BUFFER_INFO");
				var info = JsonUtility.FromJson<BufferInfo>(Encoding.UTF8.GetString(response.Data));

				return info;
			}
			catch (TimeoutException ex)
			{
				Debug.LogError(ex);
			}

			return new BufferInfo()
			{
				width = -1,
				height = -1,
			};
		}

		public void Clear()
		{
			foreach (var client in server.ListClients())
			{
				try
				{
					var response = server.SendAndWait(5000, client, "CLEAR");
				}
				catch (TimeoutException ex)
				{
					Debug.LogError(ex);
				}
			}
		}

		void Server_ClientConnected(object sender, ConnectionEventArgs args)
		{
			Debug.Log("Client connected: " + args.IpPort);
			ClientConnected?.Invoke(this, args);
		}

		void Server_ClientDisconnected(object sender, DisconnectionEventArgs args)
		{
			Debug.Log("Client disconnected: " + args.IpPort + ": " + args.Reason.ToString());
			ClientDisconnected?.Invoke(this, args);
		}

		void MessageReceived(object sender, MessageReceivedEventArgs args)
		{
			Debug.Log("Message from " + args.IpPort + ": " + Encoding.UTF8.GetString(args.Data));
		}

		public event EventHandler<ConnectionEventArgs> ClientConnected;
		public event EventHandler<DisconnectionEventArgs> ClientDisconnected;
	}
}
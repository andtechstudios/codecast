using Andtech.Common;
using System;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading;
using WatsonTcp;

namespace Andtech.Codecast
{

	public class CodecastClient
	{
		public bool Connected => client.Connected;

		private readonly WatsonTcpClient client;
		private readonly AutoResetEvent autoEvent = new AutoResetEvent(false);

		public CodecastClient(IPEndPoint endpoint)
		{
			client = new WatsonTcpClient(endpoint.Address.ToString(), endpoint.Port);
			client.Events.ServerConnected += ServerConnected;
			client.Events.ServerDisconnected += ServerDisconnected;
			client.Events.MessageReceived += MessageReceived;
			client.Callbacks.SyncRequestReceived = SyncRequestReceived;
		}

		public void Connect()
		{
			client.Connect();
			autoEvent.Reset();
		}

		public void Wait()
		{
			autoEvent.WaitOne();
		}

		void MessageReceived(object sender, MessageReceivedEventArgs args)
		{
			var message = Encoding.UTF8.GetString(args.Data);
			Log.WriteLine("Message from " + args.IpPort + ": " + message, Verbosity.silly);
			DataReceived?.Invoke(message);
		}

		void ServerConnected(object sender, ConnectionEventArgs args)
		{
			Log.WriteLine("Server " + args.IpPort + " connected", Verbosity.diagnostic);
		}

		void ServerDisconnected(object sender, DisconnectionEventArgs args)
		{
			Log.WriteLine("Server " + args.IpPort + " disconnected", Verbosity.diagnostic);
			autoEvent.Set();
		}

		SyncResponse SyncRequestReceived(SyncRequest req)
		{
			switch (Encoding.UTF8.GetString(req.Data))
			{
				case "GET_BUFFER_INFO":
					var info = new BufferInfo(Console.BufferWidth, Console.BufferWidth);
					var json = JsonSerializer.Serialize(info);
					return new SyncResponse(req, json);
				case "CLEAR":
					Console.Clear();
					return new SyncResponse(req, "OK");
			}

			return new SyncResponse(req, "Unknown request");
		}

		public event Action<string> DataReceived;
	}
}

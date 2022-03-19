using Andtech.Common;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Andtech.Codecast
{

	public class CodecastClient
	{
		private Socket socket;

		public async Task ConnectAsync(int port = 8080, CancellationToken cancellationToken = default)
		{
			IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
			IPAddress ipAddress = ipHostInfo.AddressList[0];

			await ConnectAsync(ipAddress, port, cancellationToken);
		}

		public async Task ConnectAsync(IPAddress ipAddress, int port = 8080, CancellationToken cancellationToken = default)
		{
			IPEndPoint endpoint = new IPEndPoint(ipAddress, port);

			await ConnectAsync(endpoint, cancellationToken);
		}

		public async Task ConnectAsync(IPEndPoint remoteEP, CancellationToken cancellationToken = default)
		{
			socket = new Socket(remoteEP.Address.AddressFamily,
				SocketType.Stream, ProtocolType.Tcp);

			await socket.ConnectAsync(remoteEP, cancellationToken: cancellationToken);
		}

		public async Task RunAsync(CancellationToken cancellationToken)
		{
			var pingTask = PingAsync(socket, cancellationToken: cancellationToken);
			var listenTask = ListenAsync(socket, cancellationToken: cancellationToken);

			await Task.WhenAll(pingTask, listenTask);
		}

		async Task ListenAsync(Socket socket, CancellationToken cancellationToken)
		{
			byte[] buffer = new byte[1024];
			var arraySegment = new ArraySegment<byte>(buffer);

			while (true)
			{
				string candidate = null;

				while (true)
				{
					cancellationToken.ThrowIfCancellationRequested();

					int bytesRec = await socket.ReceiveAsync(arraySegment, SocketFlags.None);
					var chunk = Encoding.ASCII.GetString(buffer, 0, bytesRec);
					candidate = candidate + chunk;

					Log.WriteLine(chunk, ConsoleColor.DarkRed, Verbosity.diagnostic);

					if (chunk.EndsWith("<EOF>"))
					{
						break;
					}
				}

				var data = candidate.Clone() as string;

				foreach (var token in data.Split("<EOF>", StringSplitOptions.RemoveEmptyEntries))
				{
					DataReceived?.Invoke(token);
				}
			}
		}

		async Task PingAsync(Socket localSocket, CancellationToken cancellationToken)
		{
			while (true)
			{
				cancellationToken.ThrowIfCancellationRequested();

				if (!localSocket.IsConnected())
				{
					throw new InvalidOperationException("The client seems to have disconnected");
				}

				await Task.Delay(100, cancellationToken: cancellationToken);
			}
		}

		public event Action<string> DataReceived;
	}
}

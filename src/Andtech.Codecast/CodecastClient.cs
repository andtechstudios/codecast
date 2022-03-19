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

		public void Connect(int port = 8080, CancellationToken cancellationToken = default)
		{
			IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
			IPAddress ipAddress = ipHostInfo.AddressList[0];

			Connect(ipAddress, port, cancellationToken);
		}

		public void Connect(IPAddress ipAddress, int port = 8080, CancellationToken cancellationToken = default)
		{
			IPEndPoint endpoint = new IPEndPoint(ipAddress, port);

			Connect(endpoint, cancellationToken);
		}

		public void Connect(IPEndPoint remoteEP, CancellationToken cancellationToken = default)
		{
			socket = new Socket(remoteEP.Address.AddressFamily,
				SocketType.Stream, ProtocolType.Tcp);

			IAsyncResult result = socket.BeginConnect(remoteEP, null, null);
			result.AsyncWaitHandle.WaitOne(1000, true);

			if (socket.Connected)
			{
				socket.EndConnect(result);
			}
			else
			{
				socket.Close();
				throw new SocketException(10060); // Connection timed out.
			}
		}

		public async Task RunAsync(CancellationToken cancellationToken)
		{
			var pingTask = PingAsync(socket, cancellationToken: cancellationToken);
			var listenTask = ListenAsync(socket, cancellationToken: cancellationToken);

			try
			{
				await Task.WhenAny(pingTask, listenTask);
			}
			catch (Exception ex)
			{
				Log.WriteLine(ex.Message);
			}
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

					if (Log.Verbosity <= Verbosity.diagnostic)
					{
						if (!string.IsNullOrEmpty(chunk))
						{
							Log.WriteLine(chunk, ConsoleColor.DarkBlue, Verbosity.diagnostic);
						}
					}
					else
					{
						Log.WriteLine(chunk, ConsoleColor.DarkBlue, Verbosity.silly);
					}

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

				Log.WriteLine("ping", ConsoleColor.Gray, Verbosity.diagnostic);

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

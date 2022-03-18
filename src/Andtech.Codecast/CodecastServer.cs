using Andtech.Common;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Andtech.Codecast
{

	public class CodecastServer
	{
		public static int Port { get; set; } = 8080;

		private CancellationTokenSource listenCTS;

		public async Task RunAsync()
		{
			// Establish the local endpoint for the socket.  
			// Dns.GetHostName returns the name of the
			// host running the application.  
			IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
			IPAddress ipAddress = ipHostInfo.AddressList[0];
			IPEndPoint localEndPoint = new IPEndPoint(ipAddress, Port);

			// Create a TCP/IP socket.  
			Socket socket = new Socket(ipAddress.AddressFamily,
				SocketType.Stream, ProtocolType.Tcp);

			// Bind the socket to the local endpoint and
			// listen for incoming connections.  
			socket.Bind(localEndPoint);
			socket.Listen();

			Log.WriteLine("Waiting for client...", Verbosity.verbose);
			// Program is suspended while waiting for an incoming connection.  
			Socket handler = socket.Accept();
			Log.WriteLine("Connection established...", ConsoleColor.Green, Verbosity.verbose);

			listenCTS?.Dispose();
			listenCTS = new CancellationTokenSource();
			var token = listenCTS.Token;

			try
			{
				var pingTask = PingAsync(handler, cancellationToken: token);
				var listenTask = ListenAsync(socket, handler, cancellationToken: token);

				Task.WaitAny(listenTask, pingTask);
			}
			catch
			{
				throw;
			}
			finally
			{
				listenCTS.Cancel();
				Log.WriteLine("Cleaning up...", Verbosity.diagnostic);
				handler.Shutdown(SocketShutdown.Both);
				handler.Close();
				socket.Close();
				Log.WriteLine("Clean up succeeed!", Verbosity.diagnostic);
			}
		}

		async Task ListenAsync(Socket socket, Socket handler, CancellationToken cancellationToken)
		{
			byte[] buffer = new byte[1024];
			var arraySegment = new ArraySegment<byte>(buffer);

			while (true)
			{
				string candidate = null;

				while (true)
				{
					cancellationToken.ThrowIfCancellationRequested();

					int bytesRec = await handler.ReceiveAsync(arraySegment, SocketFlags.None);
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

		async Task PingAsync(Socket handler, CancellationToken cancellationToken)
		{
			while (true)
			{
				cancellationToken.ThrowIfCancellationRequested();

				if (!handler.IsConnected())
				{
					throw new InvalidOperationException("The client seems to have disconnected");
				}

				await Task.Delay(1000, cancellationToken: cancellationToken);
			}
		}

		public event Action<string> DataReceived;
	}
}

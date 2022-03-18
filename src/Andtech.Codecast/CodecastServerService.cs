using Andtech.Common;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Andtech.Codecast
{

	public class CodecastServerService
	{
		public int RetryDelay { get; set; } = 500;
		public bool UseLoopback { get; set; } = false;

		public async Task RunAsync()
		{
			while (true)
			{
				var server = new CodecastServer();
				server.DataReceived += Server_DataReceived;

				try
				{
					var localEndpoint = UseLoopback ? new IPEndPoint(IPAddress.Loopback, 8080) : GetHostIPEndpoint(8080);
					await server.RunAsync(localEndpoint);
				}
				catch (SocketException socketEx)
				{
					Log.Error.WriteLine(socketEx, ConsoleColor.Red, Verbosity.diagnostic);
				}
				catch (Exception ex)
				{
					Log.Error.WriteLine(ex, ConsoleColor.Red);
				}

				Log.WriteLine($"Will retry in {RetryDelay}ms...", Verbosity.silly);
				Thread.Sleep(RetryDelay);
				Log.WriteLine("Retrying...", Verbosity.silly);
			}
		}

		public IPEndPoint GetHostIPEndpoint(int port)
		{
			IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
			IPAddress ipAddress = ipHostInfo.AddressList[0];
			return new IPEndPoint(ipAddress, port);
		}

		void Server_DataReceived(string data)
		{
			DataReceived?.Invoke(data);
		}

		public event Action<string> DataReceived;
	}
}

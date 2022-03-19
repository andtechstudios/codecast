using Andtech.Common;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Andtech.Codecast
{

	public class CodecastClientService
	{
		public int RetryDelay { get; set; } = 500;
		public bool Loopback { get; set; } = false;

		public async Task RunAsync(string address, int port)
		{
			var client = new CodecastClient();
			client.DataReceived += Server_DataReceived;

			var ipAddress = IPAddress.Parse(address);
			var endpoint = new IPEndPoint(ipAddress, port);

			Log.WriteLine($"Attempting connection to {endpoint}...", Verbosity.silly);
			while (true)
			{
				try
				{
					await client.ConnectAsync(endpoint);
					Log.WriteLine($"Connection established...", ConsoleColor.Green, Verbosity.verbose);
					await client.RunAsync(cancellationToken: default);
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

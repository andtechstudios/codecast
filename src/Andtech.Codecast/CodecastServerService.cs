using Andtech.Common;
using System;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Andtech.Codecast
{

	public class CodecastServerService
	{
		public int RetryDelay { get; set; } = 500;

		public async Task RunAsync()
		{
			while (true)
			{
				var server = new CodecastServer();
				server.DataReceived += Server_DataReceived;

				try
				{
					await server.RunAsync();
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

		void Server_DataReceived(string data)
		{
			DataReceived?.Invoke(data);
		}

		public event Action<string> DataReceived;
	}
}

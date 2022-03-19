using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Andtech.Codecast
{
	public static class SocketExtensions
	{

		public static bool IsConnected(this Socket socket)
		{
			bool part1 = socket.Poll(250, SelectMode.SelectRead);
			bool part2 = (socket.Available == 0);
			if (part1 && part2)
				return false;
			else
				return true;
		}

		public static async Task ConnectAsync(this Socket socket, IPEndPoint remoteEndpoint, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();

			using (cancellationToken.Register(() => socket.Close()))
			{
				try
				{
					cancellationToken.ThrowIfCancellationRequested();

					await socket.ConnectAsync(remoteEndpoint).ConfigureAwait(false);
				}
				catch (NullReferenceException) when (cancellationToken.IsCancellationRequested)
				{
					cancellationToken.ThrowIfCancellationRequested();
				}
			}
		}
	}
}

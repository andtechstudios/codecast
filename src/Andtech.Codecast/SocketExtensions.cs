using System.Net.Sockets;

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
	}
}

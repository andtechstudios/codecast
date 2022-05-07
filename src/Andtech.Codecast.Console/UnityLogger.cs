using Andtech.Common;
using System;
using System.Text.Json;

namespace Andtech.Codecast.Console
{
	internal class UnityLogger
	{
		public bool UseTimestamp { get; set; } = false;

		public void PrintData(string data)
		{
			UnityLogEntry entry = default;
			try
			{
				entry = JsonSerializer.Deserialize<UnityLogEntry>(data);
			}
			catch
			{
				if (Log.Verbosity >= Verbosity.verbose)
				{
					Log.Error.WriteLine("Malformed JSON: " + data);
				}
				else
				{
					Log.Error.WriteLine("Malformed JSON!");
				}
			}

			var message = TextUtility.ParseColor(entry.message);
			if (UseTimestamp)
			{
				message = entry.Timestamp.ToLocalTime().ToString("[HH:mm:ss] ") + message;
			}

			switch (entry.LogType)
			{
				case LogType.Log:
					Log.WriteLine(message);
					break;
				case LogType.Warning:
					Log.WriteLine(message, ConsoleColor.Yellow);
					break;
				case LogType.Error:
				case LogType.Exception:
					Log.WriteLine(message, ConsoleColor.Red);
					break;
			}
		}
	}

}


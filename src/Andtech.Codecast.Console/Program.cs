using Andtech.Common;
using CommandLine;
using Pastel;
using System;
using System.Drawing;
using System.Text.Json;
using System.Threading.Tasks;

namespace Andtech.Codecast.Console
{

	public enum LogType
	{
		Error,
		Assert,
		Warning,
		Log,
		Exception,
	}

	public class UnityLogEntry
	{
		public LogType LogType => Enum.Parse<LogType>(logType);
		public DateTime Timestamp => DateTime.Parse(timestamp);

		public string logType { get; set; }
		public string message { get; set; }
		public string timestamp { get; set; }
	}

	internal class UnityLogger
	{
		public bool UseTimestamp { get; set; } = false;

		public void PrintData(string data)
		{
			var entry = JsonSerializer.Deserialize<UnityLogEntry>(data);

			var message = entry.message;
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

	internal class ExampleLogger
	{

		public void PrintData(string data)
		{
			System.Console.WriteLine("Received: " + data);
		}
	}

	internal class Options
	{
		[Option("verbosity", HelpText = "Logging verbosity.")]
		public Verbosity Verbosity { get; set; }
		[Value(0, MetaName = "address", Default = "localhost", Required = false)]
		public string Address { get; set; }
		[Value(1, MetaName = "port", Default = 8080, Required = false)]
		public int Port { get; set; }

		[Option("loopback", HelpText = "Use loopback.")]
		public bool UseLoopback { get; set; }

		[Option("unity", HelpText = "Process messages in Unity format.")]
		public bool UnityMode { get; set; }
	}

	internal class Program
	{

		public static async Task Main(string[] args)
		{
			var parser = Parser.Default.ParseArguments<Options>(args);
			await parser.WithParsedAsync(OnParse);
		}

		static async Task OnParse(Options options)
		{
			Log.Verbosity = options.Verbosity;

			var service = new CodecastClientService()
			{
				Loopback = options.UseLoopback,
			};

			if (options.UnityMode)
			{
				service.DataReceived += new UnityLogger().PrintData;
			}
			else
			{
				service.DataReceived += new ExampleLogger().PrintData;
			}

			await service.RunAsync(options.Address, options.Port);
		}
	}

}


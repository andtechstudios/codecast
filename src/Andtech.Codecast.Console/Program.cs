using Andtech.Codecast;
using Andtech.Common;
using CommandLine;
using System;
using System.Threading.Tasks;

namespace Andtech.Codecast.Console
{

	internal class Options
	{
		[Option("verbosity", HelpText = "Logging verbosity.")]
		public Verbosity Verbosity { get; set; }
		[Option("loopback", HelpText = "Use loopback.")]
		public bool UseLoopback { get; set; }
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

			var service = new CodecastServerService()
			{
				UseLoopback = options.UseLoopback,
			};
			service.DataReceived += Server_DataRecieved;
			await service.RunAsync();
		}

		private static void Server_DataRecieved(string data)
		{
			System.Console.WriteLine("Received: " + data);
		}
	}

}


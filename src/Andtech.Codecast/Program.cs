using Andtech.Codecast;
using Andtech.Common;
using CommandLine;
using System;
using System.Threading.Tasks;

internal class Options
{
	[Option("verbosity", HelpText = "Logging verbosity.")]
	public Verbosity Verbosity { get; set; }
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

		var service = new CodecastServerService();
		service.DataReceived += Server_DataRecieved;
		await service.RunAsync();
	}

	private static void Server_DataRecieved(string data)
	{
		Console.WriteLine("Received: " + data);
	}
}


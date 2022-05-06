using Andtech.Common;
using CommandLine;
using System.Net;
using System.Threading;

namespace Andtech.Codecast.Console
{

	internal class Program
	{

		public static void Main(string[] args)
		{
			var parser = Parser.Default.ParseArguments<Options>(args);
			parser.WithParsed(OnParse);
		}

		static void OnParse(Options options)
		{
			Log.Verbosity = options.Verbosity;

			var endpoint = IPEndPoint.Parse(options.Host);

			var client = new CodecastClient(endpoint);

			if (options.UnityMode)
			{
				client.DataReceived += new UnityLogger().PrintData;
			}
			else
			{
				client.DataReceived += new ExampleLogger().PrintData;
			}

			client.Connect();
			client.Wait();
		}

		internal class Options
		{
			[Option("verbosity", HelpText = "Logging verbosity.")]
			public Verbosity Verbosity { get; set; }
			[Value(0, MetaName = "host", Default = "127.0.0.1:8080", Required = false)]
			public string Host { get; set; }

			[Option("unity", HelpText = "Process messages in Unity format.")]
			public bool UnityMode { get; set; }
		}
	}

}


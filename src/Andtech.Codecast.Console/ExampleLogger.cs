using Pastel;
using System.Drawing;
using System.Text.RegularExpressions;

namespace Andtech.Codecast.Console
{
	internal class ExampleLogger
	{

		public void PrintData(string data)
		{
			data = TextUtility.ParseColor(data);

			System.Console.Write(data);
		}
	}
}


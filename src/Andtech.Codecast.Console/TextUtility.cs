using Pastel;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Andtech.Codecast.Console
{
	public static class TextUtility
	{

		public static string ParseColor(string value)
		{
			value = Regex.Unescape(value);

			value = Regex.Replace(value, @"\<color=\""(?<color>.+)\""\>(?<inner>.*)\<\/color\>", Replace);

			string Replace(Match match)
			{
				var htmlColor = match.Groups["color"].Value;
				var inner = match.Groups["inner"].Value;

				Color color;
				try
				{
					color = ColorTranslator.FromHtml(htmlColor);
				}
				catch
				{
					color = Color.White;
				}

				return inner.Pastel(color);
			}

			return value;
		}
	}
}

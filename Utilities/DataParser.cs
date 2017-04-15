using System;

namespace Utilities
{
	public class DataParser
	{
		public static string[] splitCommaStrings(string raw)
		{
			return raw.Split(new char[]
			{
				','
			});
		}
	}
}

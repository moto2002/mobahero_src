using System;

namespace MobaClient
{
	public class Log
	{
		public static bool EnableLog = true;

		public static void info(string str)
		{
			if (Log.EnableLog)
			{
				switch (GlobalManager.ClientType)
				{
				case 3:
					Console.WriteLine(str);
					break;
				}
			}
		}

		public static void debug(string str)
		{
			if (Log.EnableLog)
			{
				switch (GlobalManager.ClientType)
				{
				case 3:
					Console.WriteLine(str);
					break;
				}
			}
		}

		public static void error(string str)
		{
			if (Log.EnableLog)
			{
				switch (GlobalManager.ClientType)
				{
				case 3:
					Console.WriteLine(str);
					break;
				}
			}
		}

		public static void warning(string str)
		{
			if (Log.EnableLog)
			{
				switch (GlobalManager.ClientType)
				{
				case 3:
					Console.WriteLine(str);
					break;
				}
			}
		}
	}
}

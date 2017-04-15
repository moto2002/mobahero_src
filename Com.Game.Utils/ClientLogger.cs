using System;
using System.Diagnostics;

namespace Com.Game.Utils
{
	public class ClientLogger
	{
		private static ILogger _logger;

		public static LogLevel Level
		{
			get;
			set;
		}

		public static ILogger Logger
		{
			get
			{
				return ClientLogger._logger;
			}
			set
			{
				if (value == null)
				{
					value = new ConsoleLogger();
				}
				ClientLogger._logger = value;
			}
		}

		static ClientLogger()
		{
			ClientLogger._logger = new ConsoleLogger();
			ClientLogger.Level = LogLevel.Debug;
		}

		public static void Error(object message)
		{
			if (ClientLogger.Level <= LogLevel.Error)
			{
				ClientLogger._logger.Error(message);
			}
		}

		public static void ErrorFormat(string fmt, params object[] objs)
		{
			if (ClientLogger.Level <= LogLevel.Error)
			{
				ClientLogger._logger.Error(string.Format(fmt, objs));
			}
		}

		public static void Info(object message)
		{
			if (ClientLogger.Level <= LogLevel.Info)
			{
				ClientLogger._logger.Info(message);
			}
		}

		public static void InfoFormat(string fmt, params object[] objs)
		{
			if (ClientLogger.Level <= LogLevel.Info)
			{
				ClientLogger._logger.Info(string.Format(fmt, objs));
			}
		}

		public static void Warn(object message)
		{
			if (ClientLogger.Level <= LogLevel.Warn)
			{
				ClientLogger._logger.Warn(message);
			}
		}

		public static void WarnFormat(string fmt, params object[] objs)
		{
			if (ClientLogger.Level <= LogLevel.Warn)
			{
				ClientLogger._logger.Warn(string.Format(fmt, objs));
			}
		}

		public static void LogException(Exception e)
		{
			if (ClientLogger.Level <= LogLevel.Error)
			{
				ClientLogger._logger.LogException(e);
			}
		}

		[Conditional("UNITY_EDITOR"), Conditional("LOGGER_DEBUG")]
		public static void Debug(string tag, object message)
		{
			if (ClientLogger.Level <= LogLevel.Debug)
			{
				ClientLogger._logger.Debug(tag, message);
			}
		}

		[Conditional("LOGGER_DEBUG"), Conditional("UNITY_EDITOR")]
		public static void DebugFormat(string tag, string fmt, params object[] objs)
		{
			if (ClientLogger.Level <= LogLevel.Debug)
			{
				ClientLogger._logger.Debug(tag, string.Format(fmt, objs));
			}
		}

		public static void Assert(bool expr, string msg = null)
		{
			if (msg == null)
			{
				msg = "Assert failed.";
			}
			if (!expr)
			{
				ClientLogger.Error(msg);
			}
		}

		public static void AssertNotNull(object obj, string msg = null)
		{
			if (msg == null)
			{
				msg = "Assert failed, parameter is null";
			}
			if (obj == null)
			{
				ClientLogger.Error(msg);
			}
		}
	}
}

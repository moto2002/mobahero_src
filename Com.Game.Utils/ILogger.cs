using System;

namespace Com.Game.Utils
{
	public interface ILogger
	{
		void Error(object msg);

		void Warn(object msg);

		void Info(object msg);

		void Debug(string tag, object msg);

		void LogException(Exception e);

		void Close();
	}
}

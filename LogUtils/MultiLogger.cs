using Com.Game.Utils;
using System;
using System.Collections.Generic;

namespace LogUtils
{
	public class MultiLogger : ILogger
	{
		private readonly List<ILogger> _loggers = new List<ILogger>();

		public bool Add(ILogger logger)
		{
			bool result;
			if (this._loggers.Find((ILogger x) => x == logger) == null)
			{
				this._loggers.Add(logger);
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		public bool Remove(ILogger logger)
		{
			return this._loggers.RemoveAll((ILogger x) => x == logger) > 0;
		}

		public MultiLogger(params ILogger[] loggers)
		{
			this._loggers.AddRange(loggers);
		}

		public void Error(object msg)
		{
			this._loggers.ForEach(delegate(ILogger x)
			{
				x.Error(msg);
			});
		}

		public void Warn(object msg)
		{
			this._loggers.ForEach(delegate(ILogger x)
			{
				x.Warn(msg);
			});
		}

		public void Info(object msg)
		{
			this._loggers.ForEach(delegate(ILogger x)
			{
				x.Info(msg);
			});
		}

		public void Debug(string tag, object msg)
		{
			this._loggers.ForEach(delegate(ILogger x)
			{
				x.Debug(tag, msg);
			});
		}

		public void LogException(Exception e)
		{
			this._loggers.ForEach(delegate(ILogger x)
			{
				x.LogException(e);
			});
		}

		public void Close()
		{
			this._loggers.ForEach(delegate(ILogger x)
			{
				x.Close();
			});
			this._loggers.Clear();
		}
	}
}

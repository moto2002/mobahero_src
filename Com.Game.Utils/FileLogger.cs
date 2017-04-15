using System;
using System.Diagnostics;
using System.IO;

namespace Com.Game.Utils
{
	public class FileLogger : ILogger
	{
		private StreamWriter _streamWriter;

		public bool LogTraceback
		{
			get;
			set;
		}

		public FileLogger(string file, bool append)
		{
			this._streamWriter = new StreamWriter(file, append);
		}

		public void Error(object msg)
		{
			this.Log("Error", msg);
		}

		public void Warn(object msg)
		{
			this.Log("Warn", msg);
		}

		public void Info(object msg)
		{
			this.Log("Info", msg);
		}

		public void Debug(string tag, object msg)
		{
			this.Log("Debug:" + tag, msg);
		}

		public void LogException(Exception e)
		{
			this.Log("Exception", e);
		}

		private void Log(string prefix, object content)
		{
			this._streamWriter.WriteLine(string.Format("[{0}]\t{1}\t{2}", prefix, DateTime.Now, content));
			if (this.LogTraceback)
			{
				this._streamWriter.WriteLine(new StackTrace());
			}
		}

		public void Close()
		{
			this._streamWriter.Flush();
			this._streamWriter.Close();
			this._streamWriter = null;
		}
	}
}

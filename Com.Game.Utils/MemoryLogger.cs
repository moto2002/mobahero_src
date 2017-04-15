using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Com.Game.Utils
{
	public class MemoryLogger : ILogger
	{
		public class Entry
		{
			public string Message;

			public StackTrace StackTrace;

			public DateTime Time;

			public string Prefix;
		}

		private readonly Queue<MemoryLogger.Entry> _entries = new Queue<MemoryLogger.Entry>();

		public bool EnableFilter
		{
			get;
			set;
		}

		public int MaxLimit
		{
			get;
			set;
		}

		public IEnumerable<MemoryLogger.Entry> Entries
		{
			get
			{
				return this._entries;
			}
		}

		public MemoryLogger(bool enableFilter, int maxLimit = 2147483647)
		{
			this.EnableFilter = enableFilter;
			this.MaxLimit = maxLimit;
		}

		private string GetString(object o)
		{
			return (o == null) ? "null" : o.ToString();
		}

		private MemoryLogger.Entry Create()
		{
			return new MemoryLogger.Entry
			{
				Time = DateTime.Now,
				StackTrace = new StackTrace()
			};
		}

		private void Add(MemoryLogger.Entry entry)
		{
			this._entries.Enqueue(entry);
			int num = this._entries.Count - this.MaxLimit;
			for (int i = 0; i < num; i++)
			{
				this._entries.Dequeue();
			}
		}

		public void Error(object msg)
		{
			MemoryLogger.Entry entry = this.Create();
			entry.Message = this.GetString(msg);
			entry.Prefix = "Error";
		}

		public void Warn(object msg)
		{
			MemoryLogger.Entry entry = this.Create();
			entry.Message = this.GetString(msg);
			entry.Prefix = "Error";
			this.Add(entry);
		}

		public void Info(object msg)
		{
			MemoryLogger.Entry entry = this.Create();
			entry.Message = this.GetString(msg);
			entry.Prefix = "Info";
			this.Add(entry);
		}

		public void Debug(string tag, object msg)
		{
			if (this.EnableFilter)
			{
				LogTagMgr.Instance.EnsureTagExists(tag);
				if (!LogTagMgr.Instance[tag].Enable)
				{
					return;
				}
			}
			MemoryLogger.Entry entry = this.Create();
			entry.Message = this.GetString(msg);
			entry.Prefix = "Debug:" + tag;
			this.Add(entry);
		}

		public void LogException(Exception e)
		{
			MemoryLogger.Entry entry = this.Create();
			entry.Message = this.GetString(e);
			entry.Prefix = "Exception";
			this.Add(entry);
		}

		public void Close()
		{
		}
	}
}

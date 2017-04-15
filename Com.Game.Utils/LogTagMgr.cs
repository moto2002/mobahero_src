using System;
using System.Collections.Generic;

namespace Com.Game.Utils
{
	public class LogTagMgr
	{
		public class TagInfo
		{
			public string Color;

			public bool Enable;
		}

		private static readonly LogTagMgr _inst = new LogTagMgr();

		private readonly Dictionary<string, LogTagMgr.TagInfo> _tags = new Dictionary<string, LogTagMgr.TagInfo>();

		public static LogTagMgr Instance
		{
			get
			{
				return LogTagMgr._inst;
			}
		}

		public IEnumerable<string> TagNames
		{
			get
			{
				return this._tags.Keys;
			}
		}

		public LogTagMgr.TagInfo this[string idx]
		{
			get
			{
				return this._tags[idx];
			}
		}

		public void EnsureTagExists(string tag)
		{
			if (!this._tags.ContainsKey(tag))
			{
				this._tags[tag] = new LogTagMgr.TagInfo
				{
					Color = null,
					Enable = true
				};
			}
		}

		public LogTagMgr.TagInfo GetTagInfo(string tag)
		{
			LogTagMgr.TagInfo result;
			if (!this._tags.ContainsKey(tag))
			{
				result = null;
			}
			else
			{
				result = this._tags[tag];
			}
			return result;
		}

		public void SetTagInfo(string tag, string color, bool enabled)
		{
			this._tags[tag] = new LogTagMgr.TagInfo
			{
				Color = color,
				Enable = enabled
			};
		}

		public void Clear()
		{
			this._tags.Clear();
		}
	}
}

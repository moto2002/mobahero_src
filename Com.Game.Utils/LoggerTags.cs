using System;
using System.Collections.Generic;
using UnityEngine;

namespace Com.Game.Utils
{
	public class LoggerTags : MonoBehaviour
	{
		[Serializable]
		public class LoggerEntry
		{
			public string Tag;

			public bool Enable;

			public string Color;
		}

		public List<LoggerTags.LoggerEntry> Tags;

		private void Awake()
		{
			UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
			this.Apply();
		}

		public void Refresh()
		{
			foreach (string tag in LogTagMgr.Instance.TagNames)
			{
				LogTagMgr.TagInfo tagInfo = LogTagMgr.Instance.GetTagInfo(tag);
				if (tagInfo != null)
				{
					LoggerTags.LoggerEntry loggerEntry = this.Tags.Find((LoggerTags.LoggerEntry x) => x.Tag == tag);
					if (loggerEntry == null)
					{
						this.Tags.Add(new LoggerTags.LoggerEntry
						{
							Color = tagInfo.Color,
							Enable = tagInfo.Enable,
							Tag = tag
						});
					}
					else
					{
						loggerEntry.Color = tagInfo.Color;
						loggerEntry.Enable = tagInfo.Enable;
					}
				}
			}
		}

		public void Apply()
		{
			foreach (LoggerTags.LoggerEntry current in this.Tags)
			{
				LogTagMgr.Instance.SetTagInfo(current.Tag, current.Color, current.Enable);
			}
		}

		public void EnableAll(bool enabled)
		{
			foreach (LoggerTags.LoggerEntry current in this.Tags)
			{
				current.Enable = enabled;
			}
		}

		public void DeleteAll()
		{
			LogTagMgr.Instance.Clear();
			this.Tags.Clear();
		}
	}
}

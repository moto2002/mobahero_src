using System;
using UnityEngine;

namespace Com.Game.Utils
{
	public class ConsoleLogger : ILogger
	{
		private const string ColorFmt = "<color={0}>{1}</color>";

		public bool IsFrameCountShown
		{
			get;
			set;
		}

		public ConsoleLogger()
		{
			this.IsFrameCountShown = false;
		}

		private object MessageWithFrameCount(object msg)
		{
			return this.IsFrameCountShown ? string.Concat(new object[]
			{
				"#",
				Time.frameCount,
				" ",
				msg
			}) : msg;
		}

		public void Error(object message)
		{
			UnityEngine.Debug.LogError(this.MessageWithFrameCount(message));
		}

		public void Warn(object message)
		{
			UnityEngine.Debug.LogWarning(this.MessageWithFrameCount(message));
		}

		public void Info(object message)
		{
			UnityEngine.Debug.Log(this.MessageWithFrameCount(message));
		}

		public void Debug(string tag, object message)
		{
			LogTagMgr.Instance.EnsureTagExists(tag);
			LogTagMgr instance = LogTagMgr.Instance;
			if (instance[tag].Enable)
			{
				if (string.IsNullOrEmpty(instance[tag].Color))
				{
					UnityEngine.Debug.Log(this.MessageWithFrameCount(message));
				}
				else
				{
					UnityEngine.Debug.Log(string.Format("<color={0}>{1}</color>", instance[tag].Color, this.MessageWithFrameCount(message)));
				}
			}
		}

		public void LogException(Exception e)
		{
			UnityEngine.Debug.LogException(e);
		}

		public void Close()
		{
		}
	}
}

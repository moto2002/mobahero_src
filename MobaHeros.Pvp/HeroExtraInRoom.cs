using System;

namespace MobaHeros.Pvp
{
	public class HeroExtraInRoom
	{
		public DateTime? TimeToRelive;

		public TimeSpan ReliveInterval = TimeSpan.MaxValue;

		public int LoadProgress;

		public float? ReliveRatio
		{
			get
			{
				if (!this.TimeToRelive.HasValue)
				{
					return null;
				}
				double num = (this.TimeToRelive.Value - DateTime.Now).TotalSeconds;
				if (num < 0.0)
				{
					num = 0.0;
				}
				return new float?((float)(num / this.ReliveInterval.TotalSeconds));
			}
		}

		public float? WaitSecondsToRelive
		{
			get
			{
				if (!this.TimeToRelive.HasValue)
				{
					return null;
				}
				float num = (float)(this.TimeToRelive.Value - DateTime.Now).TotalSeconds;
				return new float?((num <= 0f) ? 0f : num);
			}
		}
	}
}

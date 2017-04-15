using System;

namespace CruncherPlugin
{
	public class CruncherAdjustment
	{
		public enum Type
		{
			TargetQuality,
			TargetQuantity
		}

		public CruncherAdjustment.Type type;

		public float quality;

		public int quantity;
	}
}

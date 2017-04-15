using System;

namespace MobaHeros
{
	[Serializable]
	internal class AutoTestEventBase : EventArgs
	{
		public AutoTestTag Tag
		{
			get;
			private set;
		}

		public AutoTestEventBase(AutoTestTag tag)
		{
			this.Tag = tag;
		}
	}
}

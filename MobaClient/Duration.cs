using System;

namespace MobaClient
{
	[Serializable]
	public class Duration
	{
		public int Seconds
		{
			get;
			set;
		}

		public DateTime DateTime
		{
			get;
			set;
		}
	}
}

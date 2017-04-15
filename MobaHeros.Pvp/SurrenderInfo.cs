using System;
using System.Collections.Generic;

namespace MobaHeros.Pvp
{
	public class SurrenderInfo
	{
		public const float MaxTime = 30f;

		public int Count;

		public string StarterId;

		public DateTime StartTime = default(DateTime);

		public Dictionary<string, bool> Votes = new Dictionary<string, bool>();
	}
}

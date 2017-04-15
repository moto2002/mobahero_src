using System;

namespace HUD.Module
{
	internal class SourceData
	{
		public int heroKill;

		public int death;

		public int assist;

		public int monsterKill;

		public bool firstKill;

		public void Clear()
		{
			this.heroKill = 0;
			this.death = 0;
			this.assist = 0;
			this.monsterKill = 0;
			this.firstKill = false;
		}
	}
}

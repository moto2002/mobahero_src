using System;

namespace BattleAttrGrowth
{
	public class KillData
	{
		private readonly TeamType _team;

		public int KillFirstHeroId
		{
			get;
			private set;
		}

		public int KillFirstMonsterId
		{
			get;
			private set;
		}

		public TeamType Team
		{
			get
			{
				return this._team;
			}
		}

		public int TitleLevel
		{
			get;
			private set;
		}

		public bool KillFirstHero
		{
			get;
			private set;
		}

		public bool KillFirstMonster
		{
			get;
			private set;
		}

		public int FinishKillingTimes
		{
			get;
			private set;
		}

		public int TotalMonsterKill
		{
			get;
			private set;
		}

		public int TotalHeroKill
		{
			get;
			private set;
		}

		public static int GlobalMonsterKill
		{
			get;
			private set;
		}

		public static int GlobalHeroKill
		{
			get;
			private set;
		}

		public KillData(TeamType teamType)
		{
			this._team = teamType;
			this.ResetData();
		}

		public void ResetData()
		{
			this.TitleLevel = 0;
			this.KillFirstHero = false;
			this.KillFirstMonster = false;
			this.FinishKillingTimes = 0;
			this.TotalHeroKill = 0;
			this.TotalMonsterKill = 0;
			KillData.GlobalHeroKill = 0;
			KillData.GlobalMonsterKill = 0;
			this.KillFirstHeroId = -1;
			this.KillFirstMonsterId = -1;
		}

		public void AddHeroKill(int attackerId)
		{
			this.TotalHeroKill++;
			this.TitleLevel++;
			if (KillData.GlobalHeroKill == 0)
			{
				this.KillFirstHero = true;
				this.KillFirstHeroId = attackerId;
			}
			KillData.AddGlobalHeroKill();
		}

		public void AddMonsterKill(int attackerId)
		{
			this.TotalMonsterKill++;
			if (KillData.GlobalMonsterKill == 0)
			{
				this.KillFirstMonster = true;
				this.KillFirstMonsterId = attackerId;
			}
			KillData.AddGlobalMonsterKill();
		}

		public void AddFinishKillingTimes()
		{
			this.FinishKillingTimes++;
		}

		public void OnHeroDeath()
		{
			this.TitleLevel = 0;
		}

		private static void AddGlobalMonsterKill()
		{
			KillData.GlobalMonsterKill++;
		}

		private static void AddGlobalHeroKill()
		{
			KillData.GlobalHeroKill++;
		}
	}
}

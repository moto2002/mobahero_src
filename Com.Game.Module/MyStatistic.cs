using System;
using System.Collections.Generic;

namespace Com.Game.Module
{
	internal class MyStatistic
	{
		private enum KillType
		{
			ByHero = 1,
			ByMonster,
			ByTower
		}

		private enum UnitType
		{
			Monster = 1,
			Hero,
			Tower,
			None
		}

		public class KillData
		{
			public int byHero;

			public int byMonster;

			public int byTower;

			private Dictionary<int, int> _heroKillRecord = new Dictionary<int, int>();

			public int TotalKill
			{
				get
				{
					return this.byHero + this.byMonster + this.byTower;
				}
			}

			public KillData()
			{
				this.byHero = 0;
				this.byMonster = 0;
				this.byTower = 0;
			}

			public void AddHeroRecord(int uid)
			{
				if (this._heroKillRecord.ContainsKey(uid))
				{
					Dictionary<int, int> heroKillRecord;
					Dictionary<int, int> expr_17 = heroKillRecord = this._heroKillRecord;
					int num = heroKillRecord[uid];
					expr_17[uid] = num + 1;
				}
				else
				{
					this._heroKillRecord.Add(uid, 1);
				}
			}

			public int GetHeroRecord(int uid)
			{
				if (this._heroKillRecord != null && this._heroKillRecord.ContainsKey(uid))
				{
					return this._heroKillRecord[uid];
				}
				return 0;
			}
		}

		public class MyStatisticData
		{
			public MyStatistic.KillData heroKill;

			public MyStatistic.KillData monsterKill;

			public MyStatistic.KillData towerKill;

			public MyStatisticData()
			{
				this.heroKill = new MyStatistic.KillData();
				this.monsterKill = new MyStatistic.KillData();
				this.towerKill = new MyStatistic.KillData();
			}
		}

		private static MyStatistic _instance;

		private Dictionary<int, MyStatistic.MyStatisticData> _allDatas = new Dictionary<int, MyStatistic.MyStatisticData>();

		public static MyStatistic Instance
		{
			get
			{
				if (MyStatistic._instance == null)
				{
					MyStatistic._instance = new MyStatistic();
				}
				return MyStatistic._instance;
			}
		}

		private MyStatistic()
		{
			this.Init();
		}

		public void Init()
		{
			this._allDatas.Clear();
			this._allDatas.Add(1, new MyStatistic.MyStatisticData());
			this._allDatas.Add(0, new MyStatistic.MyStatisticData());
		}

		public void End()
		{
			this._allDatas.Clear();
		}

		public MyStatistic.MyStatisticData GetDataByTeam(TeamType type)
		{
			if (this._allDatas.ContainsKey((int)type))
			{
				return this._allDatas[(int)type];
			}
			return null;
		}

		public void UpdateData(Units deadUnits, Units attacker)
		{
			MyStatistic.MyStatisticData myStatisticData = null;
			if (attacker == null)
			{
				return;
			}
			if (attacker.TeamType == TeamType.Neutral)
			{
				return;
			}
			int teamType = attacker.teamType;
			if (this._allDatas.ContainsKey(teamType))
			{
				myStatisticData = this._allDatas[teamType];
			}
			if (myStatisticData != null)
			{
				MyStatistic.UnitType unitType = this.GetUnitType(deadUnits);
				MyStatistic.UnitType unitType2 = this.GetUnitType(attacker);
				this.UpdateDataByDeadType(myStatisticData, unitType, unitType2, attacker);
			}
		}

		private MyStatistic.UnitType GetUnitType(Units target)
		{
			if (target != null)
			{
				if (target.isMonster)
				{
					return MyStatistic.UnitType.Monster;
				}
				if (target.isHero)
				{
					return MyStatistic.UnitType.Hero;
				}
				if (target.isTower)
				{
					return MyStatistic.UnitType.Tower;
				}
			}
			return MyStatistic.UnitType.None;
		}

		private void UpdateDataByDeadType(MyStatistic.MyStatisticData data, MyStatistic.UnitType deadType, MyStatistic.UnitType attackerType, Units attacker)
		{
			switch (deadType)
			{
			case MyStatistic.UnitType.Monster:
				this.UpdateDatabyKillType(data.monsterKill, attackerType, attacker);
				break;
			case MyStatistic.UnitType.Hero:
				this.UpdateDatabyKillType(data.heroKill, attackerType, attacker);
				break;
			case MyStatistic.UnitType.Tower:
				this.UpdateDatabyKillType(data.towerKill, attackerType, attacker);
				break;
			}
		}

		private void UpdateDatabyKillType(MyStatistic.KillData data, MyStatistic.UnitType attackerType, Units attacker)
		{
			switch (attackerType)
			{
			case MyStatistic.UnitType.Monster:
				data.byMonster++;
				break;
			case MyStatistic.UnitType.Hero:
				data.byHero++;
				data.AddHeroRecord(attacker.unique_id);
				break;
			case MyStatistic.UnitType.Tower:
				data.byTower++;
				break;
			}
		}
	}
}

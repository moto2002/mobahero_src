using Com.Game.Data;
using Com.Game.Manager;
using Com.Game.Module;
using MobaProtocol.Data;
using System;
using System.Collections.Generic;

namespace MobaHeros.Pvp
{
	public class PvpStatisticMgr
	{
		public class GroupData
		{
			private int _teamType;

			private int _teamKill;

			private int _teamLv;

			private int _teamTotalExp;

			private int _teamCurExp;

			private int _teamDeath;

			private int _teamEpicMonsterKill;

			private int _teamTowerDestroy;

			public int TeamEpicMonsterKill
			{
				get
				{
					return this._teamEpicMonsterKill;
				}
				set
				{
					this._teamEpicMonsterKill = value;
				}
			}

			public int TeamTowerDestroy
			{
				get
				{
					return this._teamTowerDestroy;
				}
				set
				{
					this._teamTowerDestroy = value;
				}
			}

			public int TeamType
			{
				get
				{
					return this._teamType;
				}
				set
				{
					this._teamType = value;
				}
			}

			public int TeamKill
			{
				get
				{
					return this._teamKill;
				}
				set
				{
					this._teamKill = value;
				}
			}

			public int TeamDeath
			{
				get
				{
					return this._teamDeath;
				}
				set
				{
					this._teamDeath = value;
				}
			}

			public int TeamLv
			{
				get
				{
					return this._teamLv;
				}
				set
				{
					this._teamLv = value;
				}
			}

			public float TeamExpRatio
			{
				get
				{
					if (this._teamLv == PvpStatisticMgr.MAX_LEVEL)
					{
						return 1f;
					}
					float expByLv = PvpStatisticMgr.GetExpByLv(this._teamLv);
					return (float)this._teamCurExp / expByLv;
				}
			}

			public int TeamTotalExp
			{
				get
				{
					return this._teamTotalExp;
				}
				set
				{
					this._teamTotalExp = value;
				}
			}

			public int TeamCurExp
			{
				get
				{
					return this._teamCurExp;
				}
				set
				{
					this._teamCurExp = value;
				}
			}

			public GroupData(int team)
			{
				this._teamType = team;
				this.Init();
			}

			private void Init()
			{
				this._teamKill = 0;
				this._teamLv = 1;
				this._teamTotalExp = 0;
				this._teamCurExp = 0;
				this._teamDeath = 0;
			}
		}

		public class HeroData
		{
			private int _uid;

			private int _heroKill;

			private int _death;

			private int _monsterKill;

			private int _assist;

			private bool _firstKill;

			private int _curLv = 1;

			private int _totalExp;

			private int _gold;

			private int _curExp;

			public int CurExp
			{
				get
				{
					return this._curExp;
				}
				set
				{
					this._curExp = value;
				}
			}

			public int CurGold
			{
				get
				{
					return this._gold;
				}
				set
				{
					this._gold = value;
				}
			}

			public int TotalGold
			{
				get;
				set;
			}

			public int CurLv
			{
				get
				{
					return this._curLv;
				}
				set
				{
					this._curLv = value;
				}
			}

			public int TotalExp
			{
				get
				{
					return this._totalExp;
				}
				set
				{
					this._totalExp = value;
				}
			}

			public int HeroKill
			{
				get
				{
					return this._heroKill;
				}
				set
				{
					this._heroKill = value;
				}
			}

			public int Death
			{
				get
				{
					return this._death;
				}
				set
				{
					this._death = value;
				}
			}

			public int MonsterKill
			{
				get
				{
					return this._monsterKill;
				}
				set
				{
					this._monsterKill = value;
				}
			}

			public int Assist
			{
				get
				{
					return this._assist;
				}
				set
				{
					this._assist = value;
				}
			}

			public bool FirstKill
			{
				get
				{
					return this._firstKill;
				}
				set
				{
					this._firstKill = value;
				}
			}

			public HeroData(int id)
			{
				this._uid = id;
				this.Init();
			}

			private void Init()
			{
				this._heroKill = 0;
				this._death = 0;
				this._monsterKill = 0;
				this._assist = 0;
				this._firstKill = false;
			}
		}

		private bool hasInit;

		private Dictionary<int, PvpStatisticMgr.HeroData> _heroDatas;

		private Dictionary<int, PvpStatisticMgr.GroupData> _groupDatas;

		private static Dictionary<int, float> _expDics;

		private static int MAX_LEVEL = 25;

		public PvpStatisticMgr()
		{
			this.Init();
		}

		private void Init()
		{
			this._heroDatas = new Dictionary<int, PvpStatisticMgr.HeroData>();
			this._groupDatas = new Dictionary<int, PvpStatisticMgr.GroupData>();
			PvpStatisticMgr._expDics = new Dictionary<int, float>();
		}

		private static void InitExpDic()
		{
			if (PvpStatisticMgr._expDics != null && PvpStatisticMgr._expDics.Count > 0)
			{
				return;
			}
			Dictionary<string, object>.ValueCollection values = BaseDataMgr.instance.GetDicByType<SysBattleAttrLvVo>().Values;
			PvpStatisticMgr.MAX_LEVEL = values.Count;
			PvpStatisticMgr._expDics = new Dictionary<int, float>();
			for (int i = 1; i <= PvpStatisticMgr.MAX_LEVEL; i++)
			{
				SysBattleAttrLvVo dataById = BaseDataMgr.instance.GetDataById<SysBattleAttrLvVo>(i.ToString());
				if (dataById != null)
				{
					PvpStatisticMgr._expDics[i] = dataById.exp_to_next;
				}
			}
		}

		public static float GetExpByLv(int lv)
		{
			PvpStatisticMgr.InitExpDic();
			if (lv > PvpStatisticMgr.MAX_LEVEL)
			{
				return -1f;
			}
			return PvpStatisticMgr._expDics[lv];
		}

		public static int GetMaxLv()
		{
			PvpStatisticMgr.InitExpDic();
			return PvpStatisticMgr.MAX_LEVEL;
		}

		public PvpStatisticMgr.GroupData GetGroupData(int teamType)
		{
			if (this._groupDatas.ContainsKey(teamType))
			{
				return this._groupDatas[teamType];
			}
			this._groupDatas.Add(teamType, new PvpStatisticMgr.GroupData(teamType));
			return this._groupDatas[teamType];
		}

		public int GetTotalMeney(TeamType team)
		{
			List<ReadyPlayerSampleInfo> playersByTeam = Singleton<PvpManager>.Instance.GetPlayersByTeam(team);
			int num = 0;
			List<ReadyPlayerSampleInfo>.Enumerator enumerator = playersByTeam.GetEnumerator();
			while (enumerator.MoveNext())
			{
				int key = -enumerator.Current.newUid;
				if (this._heroDatas.ContainsKey(key))
				{
					num += this._heroDatas[key].TotalGold;
				}
			}
			return num;
		}

		public PvpStatisticMgr.HeroData GetHeroData(int uid)
		{
			if (this._heroDatas.ContainsKey(uid))
			{
				return this._heroDatas[uid];
			}
			this._heroDatas.Add(uid, new PvpStatisticMgr.HeroData(uid));
			return this._heroDatas[uid];
		}
	}
}

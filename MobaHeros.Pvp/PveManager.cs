using Com.Game.Module;
using MobaHeros.Pvp.State;
using MobaProtocol.Data;
using System;
using System.Collections.Generic;

namespace MobaHeros.Pvp
{
	public class PveManager : Singleton<PveManager>
	{
		private PveBattlePreloadInfo _fightInfo;

		private bool IsSelfLoadingOk;

		public List<PvePlayerInfo> LmPlayers
		{
			get
			{
				return this._fightInfo.lmList;
			}
		}

		public List<PvePlayerInfo> BlPlayers
		{
			get
			{
				return this._fightInfo.blList;
			}
		}

		private List<EntityVo> LmHeroes
		{
			get
			{
				List<EntityVo> list = new List<EntityVo>();
				foreach (PvePlayerInfo current in this.LmPlayers)
				{
					EntityVo vo = this.GetVo(current);
					vo.uid = -current.newUid;
					list.Add(vo);
				}
				return list;
			}
		}

		private List<EntityVo> BlHeroes
		{
			get
			{
				List<EntityVo> list = new List<EntityVo>();
				foreach (PvePlayerInfo current in this.BlPlayers)
				{
					EntityVo vo = this.GetVo(current);
					vo.uid = -current.newUid;
					list.Add(vo);
				}
				return list;
			}
		}

		public int MyLobbyUserId
		{
			get;
			private set;
		}

		public int MyHeroUniqueId
		{
			get;
			private set;
		}

		public KeyValuePair<int, int> PvpTowerLevel
		{
			get
			{
				int num = 1;
				int num2 = 1;
				if (num <= 0)
				{
					num = 1;
				}
				if (num2 <= 0)
				{
					num2 = 1;
				}
				return new KeyValuePair<int, int>(num, num2);
			}
		}

		private EntityVo GetVo(PvePlayerInfo info)
		{
			if (info == null)
			{
				throw new ArgumentException("info is null");
			}
			if (info.heroInfo == null)
			{
				throw new ArgumentException("heroinfo is null");
			}
			return new EntityVo(EntityType.Hero, info.heroInfo.heroId, CharacterDataMgr.instance.GetLevel(info.heroInfo.exp), info.heroInfo.star, info.heroInfo.quality, 0f, 0f, 0, 0);
		}

		public void ResetState()
		{
			this._fightInfo = null;
			this.IsSelfLoadingOk = false;
			this.MyLobbyUserId = 0;
			this.MyHeroUniqueId = 0;
		}

		public void SetBattleInfo(PveBattlePreloadInfo fightInfo)
		{
			this._fightInfo = fightInfo;
			this.MyLobbyUserId = -this.LmHeroes[0].uid;
			this.MyHeroUniqueId = -this.MyLobbyUserId;
		}

		public void LoadPvpSceneBegin()
		{
			this.IsSelfLoadingOk = false;
			LevelManager.m_CurLevel.SetAllHeroes(new Dictionary<TeamType, List<EntityVo>>
			{
				{
					TeamType.LM,
					this.LmHeroes
				},
				{
					TeamType.BL,
					this.BlHeroes
				}
			});
			SceneManager.Instance.ChangeScene(SceneType.Map, true);
		}

		public void LoadPvpSceneEnd()
		{
			this.IsSelfLoadingOk = true;
			PvpStateManager.Instance.ChangeState(new PveStateStart());
		}
	}
}

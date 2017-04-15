using Com.Game.Data;
using Com.Game.Module;
using Com.Game.Utils;
using MobaHeros.Pvp;
using System;

namespace MobaHeros.Spawners
{
	public class DefaultVictoryChecker : BaseVictoryChecker
	{
		private readonly SysBattleSceneVo _scene;

		public DefaultVictoryChecker(SysBattleSceneVo scene)
		{
			this._scene = scene;
		}

		private bool CheckPvp()
		{
			if (!Singleton<PvpManager>.Instance.IsInPvp)
			{
				return false;
			}
			TeamType? winTeam = Singleton<PvpManager>.Instance.RoomInfo.WinTeam;
			base.WinnerTeam = ((!winTeam.HasValue) ? TeamType.None : winTeam.Value);
			return true;
		}

		protected override void CheckVictory()
		{
			if (this.CheckPvp())
			{
				return;
			}
			switch (this._scene.victory_conditions)
			{
			case 1:
				if (MapManager.Instance.IsHomeDestroyed(TeamType.BL))
				{
					base.WinnerTeam = TeamType.LM;
				}
				else if (MapManager.Instance.IsHomeDestroyed(TeamType.LM))
				{
					base.WinnerTeam = TeamType.BL;
				}
				break;
			case 2:
				if (MapManager.Instance.IsEnemyHeroAllDead())
				{
					base.WinnerTeam = TeamType.LM;
				}
				else if (MapManager.Instance.IsOurHeroAllDead())
				{
					base.WinnerTeam = TeamType.BL;
				}
				break;
			case 3:
				if (PlayerControlMgr.Instance.IsPlayerDead())
				{
					base.WinnerTeam = TeamType.BL;
				}
				else if (MapManager.Instance.IsEnemyHeroAllDead())
				{
					base.WinnerTeam = TeamType.LM;
				}
				break;
			case 4:
				if (MapManager.Instance.IsEnemyAllDead())
				{
					base.WinnerTeam = TeamType.LM;
				}
				else if (MapManager.Instance.IsOurAllDead())
				{
					base.WinnerTeam = TeamType.BL;
				}
				break;
			case 5:
				ClientLogger.Error("not supported");
				break;
			}
		}
	}
}

using Com.Game.Module;
using MobaFrame.SkillAction;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace MobaHeros.Pvp
{
	public class ChaosFightMgr : BaseGameModule
	{
		private TeamType? _leaderTeam;

		private readonly List<KeyValuePair<Units, PlayEffectAction>> _leadingUnitEffects = new List<KeyValuePair<Units, PlayEffectAction>>();

		public TeamType? LeadingTeam
		{
			get
			{
				return this._leaderTeam;
			}
		}

		public static bool IsChaosFight
		{
			get
			{
				return Singleton<PvpManager>.Instance.IsInPvp && LevelManager.CurBattleId == "80007";
			}
		}

		public void ChangeLeadingTeam(TeamType team)
		{
			this._leaderTeam = new TeamType?(team);
			foreach (KeyValuePair<Units, PlayEffectAction> current in this._leadingUnitEffects)
			{
				if (current.Key && current.Value != null)
				{
					current.Value.Destroy();
				}
			}
			this._leadingUnitEffects.Clear();
			foreach (Units current2 in MapManager.Instance.GetAllHeroes())
			{
				if (current2.TeamType == team)
				{
					this._leadingUnitEffects.Add(new KeyValuePair<Units, PlayEffectAction>(current2, this.CreateEffect(current2)));
				}
			}
		}

		private TeamType FindLeadingTeam()
		{
			TeamType result = TeamType.None;
			int num = 0;
			for (int i = 0; i < 4; i++)
			{
				int teamKill = Singleton<PvpManager>.Instance.StatisticMgr.GetGroupData(i).TeamKill;
				if (teamKill > num)
				{
					num = teamKill;
					result = (TeamType)i;
				}
			}
			if (this._leaderTeam.HasValue)
			{
				int teamKill2 = Singleton<PvpManager>.Instance.StatisticMgr.GetGroupData((int)this._leaderTeam.Value).TeamKill;
				if (num <= teamKill2)
				{
					result = this._leaderTeam.Value;
				}
			}
			return result;
		}

		public void TryChangeLeadingTeam(Units attacker)
		{
			if (!ChaosFightMgr.IsChaosFight)
			{
				return;
			}
			TeamType teamType = this.FindLeadingTeam();
			if (this._leaderTeam != teamType)
			{
				if (this._leaderTeam.HasValue)
				{
					UIMessageBox.ShowTextPrompt("1178");
				}
				this.ChangeLeadingTeam(teamType);
				AudioMgr.Play("Play_Businessman_Switch", null, false, false);
			}
		}

		public void ShowChestAlert(float x, float y)
		{
			Singleton<MiniMapView>.Instance.ShowChestAlert(x, y);
			UIMessageBox.ShowTextPrompt("1176");
		}

		public override void Init()
		{
			this._leaderTeam = null;
			this._leadingUnitEffects.Clear();
			base.Init();
			if (ChaosFightMgr.IsChaosFight)
			{
				Units.OnUnitsRebirth += new Action<Units>(this.Units_OnUnitsRebirth);
			}
		}

		public override void Uninit()
		{
			base.Uninit();
			Units.OnUnitsRebirth -= new Action<Units>(this.Units_OnUnitsRebirth);
		}

		private void Units_OnUnitsRebirth(Units units)
		{
			if (units.isHero && units.TeamType == this.LeadingTeam)
			{
				this._leadingUnitEffects.RemoveAll((KeyValuePair<Units, PlayEffectAction> x) => x.Key == units);
				this._leadingUnitEffects.Add(new KeyValuePair<Units, PlayEffectAction>(units, this.CreateEffect(units)));
			}
		}

		private PlayEffectAction CreateEffect(Units units)
		{
			ActionManager.PlayEffect("wangguan_ready", units, new Vector3?(new Vector3(0f, 1f, 0f)), null, true, string.Empty, null);
			return ActionManager.PlayEffect("3v3v3_wangguan", units, new Vector3?(new Vector3(0f, 1f, 0f)), null, true, string.Empty, null);
		}
	}
}

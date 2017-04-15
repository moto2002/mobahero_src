using Com.Game.Module;
using MobaHeros.Pvp;
using Newbie;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerControlMgr : BaseGameModule
{
	public delegate bool PlayerChangeCondition();

	public delegate void PlayerChangeAction();

	private float maxselecteddistance = 14f;

	private readonly Dictionary<string, Dictionary<PlayerControlMgr.PlayerChangeAction, PlayerControlMgr.PlayerChangeCondition>> _changePlayerCallBacks = new Dictionary<string, Dictionary<PlayerControlMgr.PlayerChangeAction, PlayerControlMgr.PlayerChangeCondition>>();

	private VTrigger _trigger;

	private Units _player;

	private Units _selectedTarget;

	private float targetLastHp;

	private int killMon;

	private int killHero;

	private int beKilled;

	private int assistNum;

	public static PlayerControlMgr Instance
	{
		get
		{
			if (null == GameManager.Instance)
			{
				return null;
			}
			return GameManager.Instance.PlayerControlMgr;
		}
	}

	public override void Init()
	{
		this._trigger = TriggerManager.CreateGameEventTrigger(GameEvent.ChangePlayer, null, new TriggerAction(this.OnChangePlayer));
		MobaMessageManager.RegistMessage((ClientMsg)25038, new MobaMessageFunc(this.OnSpawnFinished));
	}

	public override void Uninit()
	{
		MobaMessageManager.UnRegistMessage((ClientMsg)25038, new MobaMessageFunc(this.OnSpawnFinished));
		TriggerManager.DestroyTrigger(this._trigger);
		this._trigger = null;
		this._changePlayerCallBacks.Clear();
	}

	public void RegisterPlayerChangeCallBack(string invokerName, PlayerControlMgr.PlayerChangeAction action, PlayerControlMgr.PlayerChangeCondition condition = null)
	{
		if (this._changePlayerCallBacks.ContainsKey(invokerName))
		{
			if (!this._changePlayerCallBacks[invokerName].ContainsKey(action))
			{
				this._changePlayerCallBacks[invokerName].Add(action, condition);
			}
		}
		else
		{
			Dictionary<PlayerControlMgr.PlayerChangeAction, PlayerControlMgr.PlayerChangeCondition> dictionary = new Dictionary<PlayerControlMgr.PlayerChangeAction, PlayerControlMgr.PlayerChangeCondition>();
			dictionary.Add(action, condition);
			this._changePlayerCallBacks.Add(invokerName, dictionary);
		}
	}

	public void UnRegisterPlayerChangeCallBack(string invokerName, PlayerControlMgr.PlayerChangeAction action = null)
	{
		if (this._changePlayerCallBacks.ContainsKey(invokerName))
		{
			if (action != null)
			{
				this._changePlayerCallBacks[invokerName].Remove(action);
			}
			else
			{
				this._changePlayerCallBacks.Remove(invokerName);
			}
		}
	}

	private void OnChangePlayer()
	{
		Dictionary<string, Dictionary<PlayerControlMgr.PlayerChangeAction, PlayerControlMgr.PlayerChangeCondition>>.KeyCollection keys = this._changePlayerCallBacks.Keys;
		foreach (string current in keys)
		{
			Dictionary<PlayerControlMgr.PlayerChangeAction, PlayerControlMgr.PlayerChangeCondition> dictionary = this._changePlayerCallBacks[current];
			Dictionary<PlayerControlMgr.PlayerChangeAction, PlayerControlMgr.PlayerChangeCondition>.KeyCollection keys2 = dictionary.Keys;
			foreach (PlayerControlMgr.PlayerChangeAction current2 in keys2)
			{
				if (this._changePlayerCallBacks[current][current2] != null)
				{
					if (this._changePlayerCallBacks[current][current2]())
					{
						current2();
					}
				}
				else
				{
					current2();
				}
			}
		}
	}

	public void TryChangeTargetOnSkillStart(Units inActionUnit, List<Units> inTargets)
	{
		if (inActionUnit == null || !inActionUnit.isPlayer)
		{
			return;
		}
		if (inTargets == null || inTargets.Count < 1 || inTargets[0] == null)
		{
			return;
		}
		this.SetSelectedTarget(inTargets[0]);
	}

	public bool IsPlayerDead()
	{
		return this._player == null || !this._player.isLive;
	}

	public Units GetPlayer()
	{
		return this._player;
	}

	public void ResetPlayer()
	{
		this._player = null;
	}

	public void ChangeNextPlayer()
	{
		IList<Units> mapUnits = MapManager.Instance.GetMapUnits(TeamType.LM, TargetTag.Hero);
		if (mapUnits != null && mapUnits.Count > 1)
		{
			Units player = this.GetPlayer();
			if (player == null)
			{
				return;
			}
			int num = mapUnits.IndexOf(player) + 1;
			num %= mapUnits.Count;
			for (int i = 0; i < mapUnits.Count; i++)
			{
				if (mapUnits[num].isLive)
				{
					break;
				}
				num++;
				num %= mapUnits.Count;
			}
			this.ChangePlayer(mapUnits[num], false);
		}
		else if (mapUnits != null && mapUnits.Count == 1)
		{
			this.ChangePlayer(mapUnits[0], false);
		}
	}

	public void ChangePlayer(int uid)
	{
		Units unit = MapManager.Instance.GetUnit(uid);
		if (unit != null)
		{
			this.ChangePlayer(unit, false);
		}
	}

	public void ChangePlayer(Units player, bool moveCameraImmediately = false)
	{
		if (player == this._player)
		{
			return;
		}
		if (player != null && player.isLive)
		{
			bool flag = false;
			if (this._player != null && this._player != player)
			{
				this._player.SetPlayer(false);
				this.SetAttackTarget(false);
				flag = true;
			}
			if (flag || this._player == null)
			{
				this.SetPlayer(player);
				Singleton<TriggerManager>.Instance.SendGameStateEvent(GameEvent.ChangePlayer);
			}
			else
			{
				this.SetPlayer(player);
			}
			this.SetAttackTarget(true);
			BattleCameraMgr.Instance.SetRoleObj(this._player, moveCameraImmediately);
		}
	}

	private void SetAttackTarget(bool ismark)
	{
		if (this._player != null)
		{
			Units attackTarget = this._player.GetAttackTarget();
			if (ismark)
			{
				if (attackTarget != null && attackTarget.isLive)
				{
					attackTarget.MarkAsTarget(true);
					this._selectedTarget = attackTarget;
					BattleCameraMgr.Instance.SetTarget(attackTarget);
					HUDModuleMsgTools.Get_AttactTarget(attackTarget);
				}
			}
			else if (this._selectedTarget != null)
			{
				this._selectedTarget.MarkAsTarget(false);
				HUDModuleMsgTools.Get_AttactTarget(null);
			}
		}
	}

	private void SetPlayer(Units player)
	{
		if (this._player != null)
		{
			this._player.MarkAsMainPlayer(false);
		}
		this._player = player;
		if (Singleton<PvpManager>.Instance.IsObserver)
		{
			this._player.SetPlayer(false);
		}
		else
		{
			this._player.SetPlayer(true);
		}
		this._player.MarkAsMainPlayer(true);
		MobaMessageManager.ExecuteMsg(ClientC2C.PlayerAttached, this._player.unique_id, 0f);
	}

	private void OnSpawnFinished(MobaMessage msg)
	{
		bool flag = false;
		IList<Units> mapUnits = MapManager.Instance.GetMapUnits(TeamManager.MyTeam, TargetTag.Player);
		if (Singleton<PvpManager>.Instance.IsInPvp)
		{
			if (mapUnits != null)
			{
				Units units = mapUnits.FirstOrDefault((Units x) => Singleton<PvpManager>.Instance.IsMyHero(x));
				if (units != null)
				{
					flag = true;
					this.ChangePlayer(units, true);
				}
			}
		}
		else if (mapUnits != null && mapUnits.Count > 0)
		{
			flag = true;
			this.ChangePlayer(mapUnits[0], true);
			if (LevelManager.CurBattleType == 6)
			{
				mapUnits[0].SetLockInputState(true);
			}
		}
		if (flag)
		{
			BattleCameraMgr.Instance.RegisterPlayerEvent();
		}
	}

	public void CleanSelectTag()
	{
		this._selectedTarget = null;
	}

	private void ClearOtherTargetTag()
	{
		Units player = MapManager.Instance.GetPlayer();
		if (player == null)
		{
			return;
		}
		if (player.mManualController != null)
		{
			if (player.mManualController.mcSkillCrazy != null)
			{
				player.mManualController.mcSkillCrazy.lastTarget = null;
			}
			if (player.mManualController.mcSkillNormal != null)
			{
				player.mManualController.mcSkillNormal.lastTarget = null;
			}
		}
	}

	public Units GetSelectedTarget()
	{
		if (this._selectedTarget != null && (!this._selectedTarget.isSelected || !this._selectedTarget.isLive))
		{
			HUDModuleMsgTools.Get_AttactTarget(null);
			this._selectedTarget.MarkAsTarget(false);
			this._selectedTarget = null;
			this.ClearOtherTargetTag();
			return null;
		}
		Units player = MapManager.Instance.GetPlayer();
		if (player != null && player.transform != null && this._selectedTarget != null && Vector3.Distance(this._selectedTarget.transform.position, player.transform.position) > this.maxselecteddistance)
		{
			this._selectedTarget.MarkAsTarget(false);
			HUDModuleMsgTools.Get_AttactTarget(null);
			this._selectedTarget = null;
			this.ClearOtherTargetTag();
			return null;
		}
		return this._selectedTarget;
	}

	public Units GetSelectedTargetFast()
	{
		return this._selectedTarget;
	}

	public void SetSelectedTarget(Units target)
	{
		if (BattleCameraMgr.Instance == null || PlayerControlMgr.Instance == null || PlayerControlMgr.Instance.GetPlayer() == null)
		{
			return;
		}
		if (target != null && target.teamType == PlayerControlMgr.Instance.GetPlayer().teamType)
		{
			return;
		}
		if (target == null)
		{
			HUDModuleMsgTools.Get_AttactTarget(null);
			if (this._selectedTarget != null)
			{
				this._selectedTarget.MarkAsTarget(false);
			}
			this.CleanSelectTag();
		}
		else
		{
			if (NewbieManager.Instance.IsForbidSelectTower(target))
			{
				return;
			}
			if (target != this._selectedTarget)
			{
				if (this._selectedTarget != null)
				{
					this._selectedTarget.MarkAsTarget(false);
					this.CleanSelectTag();
				}
				HUDModuleMsgTools.Get_AttactTarget(target);
				target.MarkAsTarget(true);
				this._selectedTarget = target;
				Singleton<TriggerManager>.Instance.SendGameStateEvent(GameEvent.ClickEnemyHero);
				BattleCameraMgr.Instance.SetTarget(this._selectedTarget);
			}
			NewbieManager.Instance.TryStopCheckSelEnemyHero(target);
		}
	}

	private void BackupTargetInfo(Units target)
	{
		this.targetLastHp = target.hp;
		this.killMon = target.GetKillMonsterNum();
		this.killHero = target.GetKillHeroNum(null);
		this.beKilled = target.GetDeathNum();
		this.assistNum = target.assistantNum;
	}

	private bool NeedUpdateTargetInfo(Units target)
	{
		return target != null && (target.hp != this.targetLastHp || target.GetKillMonsterNum() != this.killMon || target.GetKillHeroNum(null) != this.killHero || target.GetDeathNum() != this.beKilled || target.assistantNum != this.assistNum);
	}

	public void TryUpdateTargetIcon()
	{
		Units selectedTarget = this.GetSelectedTarget();
		if (this.NeedUpdateTargetInfo(selectedTarget))
		{
			HUDModuleMsgTools.Get_AttactTarget(selectedTarget);
			this.BackupTargetInfo(selectedTarget);
		}
		if (selectedTarget == null || !selectedTarget.isLive)
		{
			HUDModuleMsgTools.Get_AttactTarget(null);
		}
	}
}

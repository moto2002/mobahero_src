using Com.Game.Module;
using Com.Game.Utils;
using MobaFrame.SkillAction;
using MobaHeros;
using MobaHeros.Pvp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;

public class TowerAttackIndicator : UnitComponent
{
	public enum WarnLevel
	{
		None,
		Green,
		Yellow,
		Red
	}

	private const float FAR_RADIUS_OFFSET = 1f;

	private LinkAction linkAction;

	private List<Units> targets = new List<Units>();

	private Units lastTarget;

	private TowerPointer towerPointer;

	private ResourceHandle towerPointerHandle;

	private TowerAttackIndicator.WarnLevel warnLevel = TowerAttackIndicator.WarnLevel.Green;

	private float colorAlpha = 1f;

	private Task _updateTask;

	private bool _willUpdatePointer;

	private Building _selfBuilding;

	private TowerAttackIndicator.WarnLevel mWarnLevelRecord;

	public Units CurAttackTarget
	{
		get;
		set;
	}

	public TowerAttackIndicator()
	{
	}

	public TowerAttackIndicator(Units self) : base(self)
	{
	}

	[DebuggerHidden]
	private IEnumerator UpdateSelf_Coroutine()
	{
		TowerAttackIndicator.<UpdateSelf_Coroutine>c__Iterator2D <UpdateSelf_Coroutine>c__Iterator2D = new TowerAttackIndicator.<UpdateSelf_Coroutine>c__Iterator2D();
		<UpdateSelf_Coroutine>c__Iterator2D.<>f__this = this;
		return <UpdateSelf_Coroutine>c__Iterator2D;
	}

	public override void OnExit()
	{
		if (this.linkAction != null)
		{
			this.linkAction.Destroy();
		}
	}

	public override void OnDeath(Units attacker)
	{
		Task.Clear(ref this._updateTask);
	}

	private bool CanAttack(Units unit)
	{
		return unit.skillManager.GetAttacks().Count > 0;
	}

	public override void OnStart()
	{
		this._selfBuilding = (this.self as Building);
		ClientLogger.AssertNotNull(this._selfBuilding, null);
		Task.Clear(ref this._updateTask);
		this._updateTask = new Task(this.UpdateSelf_Coroutine(), true);
		this._willUpdatePointer = (this.CanAttack(this.self) && this.self.isEnemy && !Singleton<PvpManager>.Instance.IsObserver);
	}

	public override void OnStop()
	{
		Task.Clear(ref this._updateTask);
	}

	private bool HasAttackOurHeroRecently(Units player)
	{
		return false;
	}

	private int GetMonsterNumInRange()
	{
		IEnumerable<KeyValuePair<int, Units>> source = from x in MapManager.Instance.GetAllMapUnits()
		where x.Value.isLive && x.Value.tag == "Monster" && TeamManager.CanAttack(this.self, x.Value) && this.IsTargetInRange(x.Value)
		select x;
		return source.Count<KeyValuePair<int, Units>>();
	}

	private bool IsTowerAttackingMainEnemy()
	{
		return this._selfBuilding && this._selfBuilding.BuildingAttackTarget == PlayerControlMgr.Instance.GetPlayer();
	}

	private void CheckWarnLevel(Units player)
	{
		this.colorAlpha = 1f;
		this.warnLevel = TowerAttackIndicator.WarnLevel.None;
		if (player.teamType == this.self.teamType || !this.self.isLive)
		{
			return;
		}
		float num = Vector3.Distance(player.transform.position, this.self.transform.position);
		float attackRange = this.self.GetAttackRange(player);
		float num2 = 1f + attackRange;
		if (num < attackRange)
		{
			bool flag = this.IsTowerAttackingMainEnemy();
			if (flag)
			{
				this.warnLevel = TowerAttackIndicator.WarnLevel.Red;
			}
			else
			{
				int monsterNumInRange = this.GetMonsterNumInRange();
				if (monsterNumInRange == 0)
				{
					this.warnLevel = TowerAttackIndicator.WarnLevel.Red;
				}
				else if (monsterNumInRange > 1)
				{
					this.warnLevel = TowerAttackIndicator.WarnLevel.Green;
				}
				else
				{
					this.warnLevel = TowerAttackIndicator.WarnLevel.Yellow;
				}
			}
		}
		else if (num < num2)
		{
			this.warnLevel = TowerAttackIndicator.WarnLevel.Yellow;
			if (this.GetMonsterNumInRange() >= 1 && !this.HasAttackOurHeroRecently(player))
			{
				this.warnLevel = TowerAttackIndicator.WarnLevel.Green;
			}
			this.colorAlpha = 0.1f + 0.9f * (num2 - num) / (num2 - attackRange);
		}
	}

	private void ShowPointer()
	{
		bool animateShow = false;
		if (this.towerPointerHandle == null)
		{
			this.towerPointer = null;
			this.towerPointerHandle = MapManager.Instance.SpawnResourceHandle("TowerPointer", null, 0);
			if (this.towerPointerHandle != null)
			{
				Transform raw = this.towerPointerHandle.Raw;
				raw.gameObject.layer = LayerMask.NameToLayer("SkillPointer");
				raw.parent = this.self.transform;
				raw.localPosition = new Vector3(0f, 0.3f, 0f);
				raw.localRotation = Quaternion.Euler(Vector3.zero);
				this.towerPointer = raw.GetComponent<TowerPointer>();
				animateShow = true;
			}
		}
		Units player = MapManager.Instance.GetPlayer();
		if (this.towerPointer)
		{
			this.towerPointer.ShowPointer(this.self.GetAttackRange(player), this.warnLevel, animateShow, this.colorAlpha);
		}
		if (this.warnLevel == TowerAttackIndicator.WarnLevel.Red && this.mWarnLevelRecord != TowerAttackIndicator.WarnLevel.Red)
		{
			AudioMgr.Play("Play_Tower_Alert", this.self.gameObject, false, false);
		}
		this.mWarnLevelRecord = this.warnLevel;
	}

	private void HidePointer()
	{
		ResourceHandle.SafeRelease(ref this.towerPointerHandle);
		this.towerPointer = null;
	}

	private void UpdateSkillPointer()
	{
		if (!this._willUpdatePointer)
		{
			return;
		}
		Units player = PlayerControlMgr.Instance.GetPlayer();
		if (!player || !player.isLive)
		{
			this.HidePointer();
			return;
		}
		this.CheckWarnLevel(player);
		if (this.warnLevel == TowerAttackIndicator.WarnLevel.None)
		{
			this.HidePointer();
		}
		else
		{
			this.ShowPointer();
		}
	}

	private bool IsTargetInRange(Units target)
	{
		return target && Vector3.Distance(target.mTransform.position, this.self.mTransform.position) < this.self.GetAttackRange(target);
	}

	private void UpdateLink()
	{
		Units curAttackTarget = this.CurAttackTarget;
		if (curAttackTarget == null || !this.self.isLive || !this.IsTargetInRange(curAttackTarget) || !curAttackTarget.isLive || !curAttackTarget.CanBeSelected)
		{
			this.lastTarget = null;
			this.ClearLink();
			return;
		}
		if (this.lastTarget == curAttackTarget)
		{
			return;
		}
		this.ClearLink();
		this.lastTarget = curAttackTarget;
		this.targets.Clear();
		this.targets.Add(this.lastTarget);
		this.CreateLink();
	}

	private void ClearLink()
	{
		if (this.linkAction != null)
		{
			this.linkAction.Destroy();
		}
	}

	private void CreateLink()
	{
		if (this.self.TeamType == TeamType.LM)
		{
			this.linkAction = ActionManager.Link(new SkillDataKey(string.Empty, 0, 0), "PaotaLink", this.self, this.targets, null, null);
		}
		else if (this.self.TeamType == TeamType.BL)
		{
			this.linkAction = ActionManager.Link(new SkillDataKey(string.Empty, 0, 0), "PaotaLink2", this.self, this.targets, null, null);
		}
		else
		{
			this.linkAction = ActionManager.Link(new SkillDataKey(string.Empty, 0, 0), "PaotaLink", this.self, this.targets, null, null);
		}
	}

	public static TowerAttackIndicator TryAddIndicator(Units owner)
	{
		return owner.AddUnitComponent<TowerAttackIndicator>();
	}
}

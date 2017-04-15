using Com.Game.Module;
using MobaFrame.SkillAction;
using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

public class SkillUnits_Blood : SkillUnit
{
	public GameObject BombEffect;

	private BloodBallGuangHuanBehavior behavior;

	private Units targetUnit;

	public override bool IsMaster
	{
		get
		{
			return true;
		}
	}

	protected override void OnStart()
	{
		base.OnStart();
		if (this.BombEffect != null)
		{
			this.BombEffect.SetActive(false);
		}
		this.ShowEffect();
	}

	protected override void ShowEffect()
	{
		if (this.unit_data.data.config.item_type == 1)
		{
			return;
		}
		if (MapManager.Instance != null && PlayerControlMgr.Instance.GetPlayer() != null)
		{
			if (!TeamManager.CheckTeam(base.gameObject, PlayerControlMgr.Instance.GetPlayer().teamType))
			{
				this.EnableAllRenders(false);
			}
			else
			{
				this.EnableAllRenders(true);
			}
		}
	}

	private void DoMorphy()
	{
		this.EnableAllRenders(false);
		if (this.BombEffect != null)
		{
			this.BombEffect.SetActive(true);
		}
	}

	public void DoAdsorbent(Units target)
	{
		if (target == null)
		{
			return;
		}
		this.targetUnit = target;
		this.behavior = base.gameObject.GetComponentInChildren<BloodBallGuangHuanBehavior>();
		this.DoMorphy();
		GlobalObject.Instance.StartCoroutine(this.Adsorbent());
	}

	[DebuggerHidden]
	private IEnumerator Adsorbent()
	{
		SkillUnits_Blood.<Adsorbent>c__Iterator1F <Adsorbent>c__Iterator1F = new SkillUnits_Blood.<Adsorbent>c__Iterator1F();
		<Adsorbent>c__Iterator1F.<>f__this = this;
		return <Adsorbent>c__Iterator1F;
	}

	protected override void OnStop()
	{
		base.OnStop();
		BloodBallGuangHuanBehavior componentInChildren = base.gameObject.GetComponentInChildren<BloodBallGuangHuanBehavior>();
		if (componentInChildren != null)
		{
			componentInChildren.DoStop();
		}
		Singleton<TriggerManager>.Instance.SendGameStateEvent(GameEvent.BuffItemDespawn);
	}

	public override void RemoveSelf(float delay = 0f)
	{
		base.RemoveSelf(delay);
		Singleton<MiniMapView>.Instance.ForceUpdateMapItemByUnits(this, false);
	}
}

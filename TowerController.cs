using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

[RequireComponent(typeof(SimpleToggle))]
public class TowerController : BaseBuildingController
{
	public SimpleToggle StateToggle;

	public GameObject ActiveFx;

	private Tower _owner;

	private bool _isBroken;

	public override void OnCreate(Tower owner)
	{
		this._owner = owner;
		this._isBroken = false;
		if (this.StateToggle)
		{
			this.StateToggle.ActiveIndex = 0;
		}
		if (this.ActiveFx)
		{
			this.ActiveFx.SetActive(true);
		}
	}

	public override void OnDamage(Units attacker, float damage)
	{
		if (!this._isBroken && this._owner.hp / this._owner.hp_max <= 0.5f && !this._isBroken)
		{
			this._isBroken = true;
			if (this.StateToggle)
			{
				if (!AudioMgr.Instance.isEffMute())
				{
					AudioMgr.Play("Play_Tower_Damage", this._owner.gameObject, false, false);
				}
				this.StateToggle.ActiveIndex = 1;
				this._owner.ResetUnitRenderer(true);
			}
		}
	}

	public override void OnDead()
	{
		if (!this._owner.isHome)
		{
			AudioMgr.PlayUI("Play_Tower_Falldown", null, false, false);
		}
		if (this.StateToggle)
		{
			this.StateToggle.ActiveIndex = 2;
		}
		if (this.ActiveFx)
		{
			this.ActiveFx.SetActive(false);
		}
	}

	[DebuggerHidden]
	public override IEnumerator OnPrewarm()
	{
		TowerController.<OnPrewarm>c__Iterator1D5 <OnPrewarm>c__Iterator1D = new TowerController.<OnPrewarm>c__Iterator1D5();
		<OnPrewarm>c__Iterator1D.<>f__this = this;
		return <OnPrewarm>c__Iterator1D;
	}
}

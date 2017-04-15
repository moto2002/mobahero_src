using Com.Game.Module;
using MobaHeros.Pvp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace MobaFrame.SkillAction
{
	public class BloodBallGuangHuanBehavior : GuangHuanBehavior
	{
		private bool isBeAdsorbent;

		private bool isUpdate;

		private bool isInit;

		private Units targetUnit;

		private List<Units> hitUnits = new List<Units>();

		private SkillUnits_Blood bloodUnits;

		public override void Init(string higheffId, string skillId, Units self)
		{
			if (Singleton<PvpManager>.Instance.IsInPvp)
			{
				return;
			}
			if (this.isInit)
			{
				return;
			}
			base.Init(higheffId, skillId, self);
			this.isInit = true;
			this.bloodUnits = (self as SkillUnits_Blood);
		}

		protected override void AssianGuangHuanShape()
		{
			if (Singleton<PvpManager>.Instance.IsInPvp)
			{
				return;
			}
			this.mCoroutineManager.StartCoroutine(this.AssianGuangHuanShapeEnumerator(), true);
		}

		[DebuggerHidden]
		private IEnumerator AssianGuangHuanShapeEnumerator()
		{
			BloodBallGuangHuanBehavior.<AssianGuangHuanShapeEnumerator>c__Iterator77 <AssianGuangHuanShapeEnumerator>c__Iterator = new BloodBallGuangHuanBehavior.<AssianGuangHuanShapeEnumerator>c__Iterator77();
			<AssianGuangHuanShapeEnumerator>c__Iterator.<>f__this = this;
			return <AssianGuangHuanShapeEnumerator>c__Iterator;
		}

		protected override void hitIn(GameObject go)
		{
			if (Singleton<PvpManager>.Instance.IsInPvp)
			{
				return;
			}
			if (this.isBeAdsorbent)
			{
				return;
			}
			if (!this.isActive)
			{
				return;
			}
			if (this.isUpdate)
			{
				return;
			}
			if (!TagManager.IsCharacterTarget(go))
			{
				return;
			}
			if (!TagManager.CheckTag(go, this.data.targetTag))
			{
				return;
			}
			if (!TeamManager.CheckTeam(this.self.gameObject, go, this.data.targetCamp, this.parent))
			{
				return;
			}
			Units component = go.GetComponent<Units>();
			if (component == null || !component.isLive || this.active_targets.Contains(component))
			{
				return;
			}
			if (!component.IsMaster)
			{
				return;
			}
			if (this.data.maxNum != 0 && this.active_targets.Count > this.data.maxNum)
			{
				return;
			}
			if (component == null || this.active_targets.Contains(component))
			{
				return;
			}
			if (!this.doWithItemType(component))
			{
				return;
			}
			if (this.hitUnits.Contains(component))
			{
				return;
			}
			this.hitUnits.Add(component);
			this.isBeAdsorbent = true;
			this.bloodUnits.DoAdsorbent(component);
		}

		protected override void hitOut(GameObject go)
		{
		}

		public void DoStop()
		{
			this.isBeAdsorbent = false;
			this.isUpdate = false;
			this.isInit = false;
			this.hitUnits.Clear();
		}

		protected override bool doWithItemType(Units target)
		{
			if (target == null)
			{
				return false;
			}
			if (!(this.self is SkillUnit))
			{
				return true;
			}
			SkillUnit skillUnit = this.self as SkillUnit;
			int itemType = skillUnit.itemType;
			if (itemType != 2)
			{
				return itemType != 3 || target.mp < target.mp_max;
			}
			return target.hp < target.hp_max;
		}
	}
}

using MobaHeros;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace MobaFrame.SkillAction
{
	public class DragonMissileAction : MissileAction
	{
		public List<Units> targetUnits;

		private int curTargetIndex;

		private float totalDamageMp;

		protected override bool doAction()
		{
			return this.targetUnits != null && this.targetUnits.Count > 0 && base.doAction();
		}

		[DebuggerHidden]
		protected override IEnumerator Missile_Coroutine()
		{
			DragonMissileAction.<Missile_Coroutine>c__Iterator59 <Missile_Coroutine>c__Iterator = new DragonMissileAction.<Missile_Coroutine>c__Iterator59();
			<Missile_Coroutine>c__Iterator.<>f__this = this;
			return <Missile_Coroutine>c__Iterator;
		}

		protected override void MoveDelta(Vector3 targetPos, float moveSpeedDelta, out float currentDistance)
		{
			base.transform.LookAt(targetPos);
			Vector3 vector = targetPos - base.transform.position;
			if (vector != Vector3.zero)
			{
				this.daoDanRot = Quaternion.LookRotation(vector);
				base.transform.rotation = this.daoDanRot;
			}
			base.transform.Translate(Vector3.forward * moveSpeedDelta);
			currentDistance = Vector3.Distance(targetPos, base.transform.position);
		}

		protected override void OnHit(BaseAction action, Units target, int count = 1)
		{
			this.doDamageToTarget(target);
			if (this.skillData.hit_actions != null && StringUtils.CheckValid(this.skillData.hit_actions[0]))
			{
				this.AddAction(ActionManager.PlayEffect(this.skillData.hit_actions[0], target, null, null, true, string.Empty, null));
			}
		}

		private void OnEnd(BaseAction action)
		{
			this.isActive = false;
			this.doDamageToSelf();
			if (this.skillData.hit_actions != null && StringUtils.CheckValid(this.skillData.hit_actions[1]))
			{
				this.AddAction(ActionManager.PlayEffect(this.skillData.hit_actions[1], base.unit, null, null, true, string.Empty, null));
			}
		}

		private void doDamageToTarget(Units target)
		{
			if (!this.IsMaster || target == null)
			{
				return;
			}
			Dictionary<short, float> dictionary = target.dataChange.doSkillWoundAction(this.skillData.damage_ids, base.unit, true, new float[0]);
			if (dictionary != null && dictionary.ContainsKey(2))
			{
				this.totalDamageMp += dictionary[2];
			}
		}

		private void doDamageToSelf()
		{
			base.unit.dataChange.doDataChangeAction(AttrType.Mp, this.totalDamageMp * -1f, base.unit, false);
		}

		private Units GetCurTarget()
		{
			if (this.curTargetIndex >= this.targetUnits.Count)
			{
				return base.unit;
			}
			return this.targetUnits[this.curTargetIndex];
		}

		protected override Vector3? GetTargetPosition()
		{
			Units curTarget = this.GetCurTarget();
			if (curTarget != null)
			{
				return new Vector3?(curTarget.GetCenter());
			}
			return null;
		}

		protected override bool CheckTargetExist()
		{
			Units curTarget = this.GetCurTarget();
			return curTarget != null && curTarget.isLive;
		}
	}
}

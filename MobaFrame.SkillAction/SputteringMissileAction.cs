using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace MobaFrame.SkillAction
{
	public class SputteringMissileAction : MissileAction
	{
		public List<Units> targetUnits;

		public BaseHighEffAction highEffAction;

		protected int curTargetIndex;

		private List<Units> hitTargets = new List<Units>();

		protected override bool doAction()
		{
			return this.targetUnits != null && this.targetUnits.Count > 0 && base.doAction();
		}

		[DebuggerHidden]
		protected override IEnumerator Missile_Coroutine()
		{
			SputteringMissileAction.<Missile_Coroutine>c__Iterator5D <Missile_Coroutine>c__Iterator5D = new SputteringMissileAction.<Missile_Coroutine>c__Iterator5D();
			<Missile_Coroutine>c__Iterator5D.<>f__this = this;
			return <Missile_Coroutine>c__Iterator5D;
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			if (this.highEffAction != null)
			{
				this.highEffAction.Destroy();
			}
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
			if (target == null)
			{
				return;
			}
			if (this.skillData == null)
			{
				return;
			}
			this.doDamageToTarget(target, count);
			if (this.skillData.hit_actions != null && StringUtils.CheckValid(this.skillData.hit_actions[0]))
			{
				ActionManager.PlayEffect(this.skillData.hit_actions[0], target, null, null, true, string.Empty, this.CasterUnit);
			}
		}

		private void OnEnd(BaseAction action)
		{
			this.isActive = false;
			if (this.skillData.hit_actions != null && StringUtils.CheckValid(this.skillData.hit_actions[1]))
			{
				ActionManager.PlayEffect(this.skillData.hit_actions[1], base.unit, null, null, true, string.Empty, null);
			}
		}

		protected virtual void doDamageToTarget(Units target, int count = 1)
		{
			if (this.highEffAction != null)
			{
				this.highEffAction.DoDamage(target, count);
			}
			else
			{
				UnityEngine.Debug.LogError("highEffAction is null");
			}
		}

		protected virtual Units GetCurTarget()
		{
			if (this.curTargetIndex >= this.targetUnits.Count)
			{
				this.Destroy();
				return null;
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

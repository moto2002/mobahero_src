using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace MobaFrame.SkillAction
{
	public class AbsorbAction : MissileAction
	{
		protected override bool doAction()
		{
			base.CreateNode(NodeType.SkillNode, this.performId);
			this.missileType = (MissileType)this.data.effectParam1;
			this.colliderType = (ColliderType)this.data.effectParam2;
			this.AddAction(ActionManager.PlayAnim(this.performId, this.targetUnit, true));
			this.mPlayEffectAction = ActionManager.PlayEffect(this.performId, this.targetUnit, null, null, true, string.Empty, this.targetUnit);
			this.AddAction(this.mPlayEffectAction);
			if (this.useCollider)
			{
				List<Units> list = new List<Units>();
				list.Add(this.targetUnit);
			}
			this.isActive = true;
			if (base.IsInstantHit())
			{
				base.InstantHit();
			}
			else
			{
				base.mCoroutineManager.StartCoroutine(this.Missile_Coroutine(), true);
			}
			return true;
		}

		[DebuggerHidden]
		protected override IEnumerator Missile_Coroutine()
		{
			AbsorbAction.<Missile_Coroutine>c__Iterator54 <Missile_Coroutine>c__Iterator = new AbsorbAction.<Missile_Coroutine>c__Iterator54();
			<Missile_Coroutine>c__Iterator.<>f__this = this;
			return <Missile_Coroutine>c__Iterator;
		}

		protected override void MoveDelta(Vector3 targetPos, float moveSpeedDelta, out float currentDistance)
		{
			Vector3 vector = targetPos - base.transform.position;
			if (vector != Vector3.zero)
			{
				this.daoDanRot = Quaternion.LookRotation(vector);
				base.transform.rotation = this.daoDanRot;
			}
			base.transform.Translate(Vector3.forward * moveSpeedDelta);
			currentDistance = Vector3.Distance(targetPos, base.transform.position);
		}

		protected override void SetPosition()
		{
			if (this.mPlayEffectAction != null)
			{
				if (this.mPlayEffectAction.transform == null)
				{
					return;
				}
				if (this.mPlayEffectAction.transform.parent != null)
				{
					base.transform.parent = this.mPlayEffectAction.transform.parent;
				}
				base.transform.localPosition = this.mPlayEffectAction.transform.localPosition;
				base.transform.localRotation = Quaternion.Euler(this.mPlayEffectAction.transform.localRotation.eulerAngles);
				base.AttachSubAction(this.mPlayEffectAction);
			}
			else if (this.targetUnit != null)
			{
				base.transform.position = this.targetUnit.GetCenter();
				base.transform.forward = base.unit.mTransform.forward;
			}
		}

		protected override bool CheckTargetExist()
		{
			return base.unit != null && base.unit.isLive;
		}

		protected override Vector3? GetTargetPosition()
		{
			if (base.unit != null)
			{
				return new Vector3?(base.unit.GetCenter());
			}
			return null;
		}
	}
}

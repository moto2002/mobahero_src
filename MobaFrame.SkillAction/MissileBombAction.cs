using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace MobaFrame.SkillAction
{
	public class MissileBombAction : BombAction
	{
		public float startDistanceToTarget;

		private float startAngle = 60f;

		protected bool isMoveTarget;

		protected bool isReachedTarget;

		protected Vector3 targetDeathPos;

		protected Quaternion daoDanRot;

		protected PlayEffectAction mPlayEffectAction;

		protected float actionDelay;

		protected float bombRadius;

		protected override bool doAction()
		{
			base.CreateNode(NodeType.SkillNode, this.performId);
			this.bombType = (BombType)this.data.effectParam1;
			this.startAngle = this.data.effectParam2;
			this.bombRadius = this.data.effectParam3;
			this.actionDelay = BaseAction.GetPerformEffectDelay(base.unit, this.data);
			this.AddAction(ActionManager.PlayAnim(this.performId, base.unit, true));
			if (StringUtils.CheckValid(this.performId))
			{
				this.mPlayEffectAction = ActionManager.PlayEffect(this.performId, base.unit, null, null, true, string.Empty, null);
				this.AddAction(this.mPlayEffectAction);
			}
			if (this.useCollider)
			{
				List<Units> list = new List<Units>();
				list.Add(this.targetUnit);
			}
			this.isActive = true;
			base.mCoroutineManager.StartCoroutine(this.Bomb_Coroutine(), true);
			return true;
		}

		[DebuggerHidden]
		protected IEnumerator Bomb_Coroutine()
		{
			MissileBombAction.<Bomb_Coroutine>c__Iterator5C <Bomb_Coroutine>c__Iterator5C = new MissileBombAction.<Bomb_Coroutine>c__Iterator5C();
			<Bomb_Coroutine>c__Iterator5C.<>f__this = this;
			return <Bomb_Coroutine>c__Iterator5C;
		}

		protected virtual void MoveDelta(Vector3 targetPos, float moveSpeedDelta, out float currentDistance)
		{
			base.transform.LookAt(targetPos);
			float num = Mathf.Min(1f, Vector3.Distance(base.transform.position, targetPos) / this.startDistanceToTarget) * this.startAngle;
			base.transform.rotation = base.transform.rotation * Quaternion.Euler(Mathf.Clamp(-num, -num, num), 0f, 0f);
			float b = Vector3.Distance(base.transform.position, targetPos);
			base.transform.Translate(Vector3.forward * Mathf.Min(moveSpeedDelta, b));
			currentDistance = Vector3.Distance(base.transform.position, targetPos);
		}

		protected void OnHit(BaseAction action, Units target)
		{
			if (action != null)
			{
				action.Destroy();
			}
		}

		protected override void OnDamage(BaseAction action, List<Units> targets)
		{
			base.OnDamage(action, targets);
			if (this.bombType == BombType.ColliderBomb)
			{
				this.Destroy();
			}
		}

		protected virtual void SetPosition()
		{
			if (base.transform == null)
			{
				return;
			}
			if (this.mPlayEffectAction != null)
			{
				base.transform.position = base.transform.position + this.mPlayEffectAction.transform.position;
				base.transform.rotation = Quaternion.Euler(base.transform.rotation.eulerAngles + this.mPlayEffectAction.transform.rotation.eulerAngles);
				base.AttachSubAction(this.mPlayEffectAction);
			}
			else if (base.unit != null)
			{
				base.transform.position = base.unit.GetCenter();
				base.transform.forward = base.unit.mTransform.forward;
			}
		}

		private new Vector3? GetTargetPosition()
		{
			if (this.targetUnit != null)
			{
				return new Vector3?(this.targetUnit.GetCenter());
			}
			Vector3? targetPosition = this.targetPosition;
			if (targetPosition.HasValue)
			{
				Vector3? targetPosition2 = this.targetPosition;
				return new Vector3?(targetPosition2.Value);
			}
			return null;
		}

		private new bool CheckTargetExist()
		{
			if (this.targetUnit != null)
			{
				return this.targetUnit.isLive;
			}
			Vector3? targetPosition = this.targetPosition;
			return targetPosition.HasValue;
		}
	}
}

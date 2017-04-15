using Com.Game.Module;
using MobaHeros.Pvp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace MobaFrame.SkillAction
{
	public class MissileAction : BasePerformAction
	{
		public Units targetUnit;

		public Vector3? targetPosition;

		public Units CasterUnit;

		protected bool isMoveTarget;

		protected bool isReachedTarget;

		protected Vector3 targetDeathPos;

		protected Quaternion daoDanRot;

		protected MissileType missileType;

		protected ColliderType colliderType;

		protected PlayEffectAction mPlayEffectAction;

		private Skill skill;

		protected override bool doAction()
		{
			base.CreateNode(NodeType.SkillNode, this.performId);
			this.missileType = (MissileType)this.data.effectParam1;
			this.colliderType = (ColliderType)this.data.effectParam2;
			this.AddAction(ActionManager.PlayAnim(this.performId, base.unit, true));
			this.mPlayEffectAction = ActionManager.PlayEffect(this.performId, base.unit, null, null, true, string.Empty, this.CasterUnit);
			this.AddAction(this.mPlayEffectAction);
			if (!Singleton<PvpManager>.Instance.IsInPvp && this.useCollider)
			{
				List<Units> list = new List<Units>();
				list.Add(this.targetUnit);
			}
			this.isActive = true;
			if (this.IsInstantHit())
			{
				this.InstantHit();
			}
			else
			{
				base.mCoroutineManager.StartCoroutine(this.Missile_Coroutine(), true);
			}
			if (base.unit == null)
			{
				return false;
			}
			if (this.skillData == null)
			{
				return false;
			}
			this.skill = base.unit.getSkillOrAttackById(this.skillData.skillId);
			if (this.skill != null)
			{
				Skill expr_133 = this.skill;
				expr_133.DestroyAction = (Callback<string, int>)Delegate.Combine(expr_133.DestroyAction, new Callback<string, int>(this.ExternDestroy));
			}
			return true;
		}

		protected void ExternDestroy(string eventPerformId, int bulletIndex)
		{
			if (this.performId == eventPerformId)
			{
				if (this.mPlayEffectAction != null)
				{
					this.mPlayEffectAction.Destroy();
				}
				this.Destroy();
			}
		}

		protected void InstantHit()
		{
			if (this.data.config.effect_speed == -1f)
			{
				base.transform.position = this.GetTargetPosition().Value;
			}
		}

		protected virtual float MinDistance(float moveSpeedDelta)
		{
			return moveSpeedDelta / 2f;
		}

		[DebuggerHidden]
		protected virtual IEnumerator Missile_Coroutine()
		{
			MissileAction.<Missile_Coroutine>c__Iterator53 <Missile_Coroutine>c__Iterator = new MissileAction.<Missile_Coroutine>c__Iterator53();
			<Missile_Coroutine>c__Iterator.<>f__this = this;
			return <Missile_Coroutine>c__Iterator;
		}

		protected virtual void MoveDelta(Vector3 targetPos, float moveSpeedDelta, out float currentDistance)
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

		protected virtual void SetPosition()
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
			else if (base.unit != null)
			{
				base.transform.position = base.unit.GetCenter();
				base.transform.forward = base.unit.mTransform.forward;
			}
		}

		protected virtual void OnHit(BaseAction action, Units target, int count = 1)
		{
		}

		protected void OnMissileHit(PlayEffectAction action, Units target)
		{
			action.Destroy();
			this.OnHit(action, target, 1);
		}

		protected override void OnDestroy()
		{
			if (this.skill != null && this.skill.DestroyAction != null)
			{
				Skill expr_21 = this.skill;
				expr_21.DestroyAction = (Callback<string, int>)Delegate.Remove(expr_21.DestroyAction, new Callback<string, int>(this.ExternDestroy));
			}
			base.OnDestroy();
		}

		protected virtual Vector3? GetTargetPosition()
		{
			if (this.targetUnit != null)
			{
				return new Vector3?(this.targetUnit.GetCenter());
			}
			Vector3? vector = this.targetPosition;
			if (vector.HasValue)
			{
				return this.targetPosition;
			}
			return null;
		}

		protected virtual bool CheckTargetExist()
		{
			if (this.targetUnit != null)
			{
				return this.targetUnit.isLive;
			}
			Vector3? vector = this.targetPosition;
			return vector.HasValue;
		}

		public bool IsInstantHit()
		{
			return this.data.config.effect_speed == -1f;
		}
	}
}

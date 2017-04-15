using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace MobaFrame.SkillAction
{
	public class BulletAction : BasePerformAction
	{
		public Units targetUnit;

		public Vector3? targetPosition;

		protected float moveSpeed;

		protected float existTime;

		protected float flyDelay;

		protected float startAngle;

		protected BulletType bulletType;

		private Skill skill;

		protected bool isMoveTarget;

		protected PlayEffectAction mPlayEffectAction;

		protected Vector3 offsetPosition;

		protected Vector3 offsetRotation;

		protected Transform _weaponFirTrans;

		protected Transform _weaponSecTrans;

		protected override bool doAction()
		{
			base.CreateNode(NodeType.SkillNode, this.performId);
			this.bulletType = (BulletType)this.data.effectParam1;
			this.flyDelay = this.data.effectParam2 + this.data.config.effect_delay;
			this.startAngle = this.data.effectParam3;
			this.moveSpeed = this.data.config.effect_speed;
			this.existTime = this.data.config.effect_time;
			this.AddAction(ActionManager.PlayAnim(this.performId, base.unit, true));
			this.mPlayEffectAction = ActionManager.PlayEffect(this.performId, base.unit, null, null, true, string.Empty, null);
			this.AddAction(this.mPlayEffectAction);
			this.skill = base.unit.getSkillOrAttackById(this.skillData.skillId);
			if (this.useCollider)
			{
				List<Units> list = new List<Units>();
				list.Add(this.targetUnit);
			}
			if (base.IsUseColliderData)
			{
				this.index = this.skill.BulletIndex;
				this.skill.BulletIndex++;
			}
			this.isActive = true;
			base.mCoroutineManager.StartCoroutine(this.Bullet_Coroutine(), true);
			Skill expr_15A = this.skill;
			expr_15A.OnPerformHitCallback = (Callback<string, int>)Delegate.Combine(expr_15A.OnPerformHitCallback, new Callback<string, int>(this.OnPvpHit));
			Skill expr_181 = this.skill;
			expr_181.DestroyAction = (Callback<string, int>)Delegate.Combine(expr_181.DestroyAction, new Callback<string, int>(this.ExternDestroy));
			return true;
		}

		[DebuggerHidden]
		protected virtual IEnumerator Bullet_Coroutine()
		{
			BulletAction.<Bullet_Coroutine>c__Iterator55 <Bullet_Coroutine>c__Iterator = new BulletAction.<Bullet_Coroutine>c__Iterator55();
			<Bullet_Coroutine>c__Iterator.<>f__this = this;
			return <Bullet_Coroutine>c__Iterator;
		}

		protected virtual void MoveDelta(Vector3 startDirection, float angle, float moveDelta)
		{
			base.transform.rotation = Quaternion.LookRotation(startDirection) * Quaternion.AngleAxis(angle, Vector3.up);
			base.transform.position += base.transform.forward * moveDelta;
		}

		protected override void OnDamage(BaseAction action, List<Units> targets)
		{
			base.OnDamage(action, targets);
			if (this.bulletType == BulletType.ColliderBullet)
			{
				this.OnHit(this.mPlayEffectAction, null);
			}
		}

		protected void OnHit(BaseAction action, Units target)
		{
			if (action != null)
			{
				action.Destroy();
			}
			this.isActive = false;
		}

		protected void OnPvpHit(string eventPerformId, int bulletIndex)
		{
			if (this.bulletType == BulletType.ColliderBullet && bulletIndex == this.index)
			{
				this.OnHit(this.mPlayEffectAction, null);
			}
		}

		protected void ExternDestroy(string eventPerformId, int bulletIndex)
		{
			if (bulletIndex == this.index)
			{
				if (this.mPlayEffectAction != null)
				{
					this.mPlayEffectAction.Destroy();
				}
				this.Destroy();
			}
		}

		protected override void OnDestroy()
		{
			if (this.skillData != null && this.skill != null)
			{
				if (this.skill.OnPerformHitCallback != null)
				{
					Skill expr_2C = this.skill;
					expr_2C.OnPerformHitCallback = (Callback<string, int>)Delegate.Remove(expr_2C.OnPerformHitCallback, new Callback<string, int>(this.OnPvpHit));
				}
				if (this.skill.DestroyAction != null)
				{
					Skill expr_63 = this.skill;
					expr_63.DestroyAction = (Callback<string, int>)Delegate.Remove(expr_63.DestroyAction, new Callback<string, int>(this.ExternDestroy));
				}
			}
			base.OnDestroy();
		}

		protected virtual void SetPosition()
		{
			if (base.transform != null)
			{
				if (this.mPlayEffectAction != null)
				{
					if (this.mPlayEffectAction.transform == null)
					{
						return;
					}
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
		}

		protected Vector3? GetTargetPosition()
		{
			if (this.targetUnit != null)
			{
				return new Vector3?(this.targetUnit.GetCenter());
			}
			Vector3? vector = this.targetPosition;
			if (vector.HasValue)
			{
				Vector3? vector2 = this.targetPosition;
				return new Vector3?(vector2.Value);
			}
			return null;
		}

		protected bool CheckTargetExist()
		{
			if (this.targetUnit != null)
			{
				return this.targetUnit.isLive;
			}
			Vector3? vector = this.targetPosition;
			return vector.HasValue;
		}

		protected virtual void TryShowWeapon()
		{
		}

		protected virtual void TryHideWeapon()
		{
		}

		protected bool IsAffectWeapon()
		{
			return this.data.isAffectWeapon;
		}

		protected void GetWeaponPosFromBoneWeapon(out Transform inWeaponFir)
		{
			inWeaponFir = null;
			if (base.unit != null)
			{
				Transform transform = null;
				Vector3 zero = Vector3.zero;
				base.unit.GetBone(8, out transform, out zero);
				if (transform != null && transform.childCount > 0)
				{
					inWeaponFir = transform.GetChild(0);
				}
			}
		}
	}
}

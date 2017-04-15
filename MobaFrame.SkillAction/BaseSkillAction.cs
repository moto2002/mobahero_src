using System;
using System.Collections.Generic;
using UnityEngine;

namespace MobaFrame.SkillAction
{
	public abstract class BaseSkillAction : CompositeAction
	{
		public SkillDataKey skillKey;

		public List<Units> targetUnits;

		public Vector3? targetPosition;

		protected PerformData data;

		protected SkillData skillData;

		protected Skill skill;

		public Callback<BaseSkillAction, List<Units>> OnSkillDamageCallback;

		public Callback<BaseSkillAction> OnSkillEndCallback;

		protected new virtual bool useCollider
		{
			get
			{
				return this.skillData != null && this.skillData.config.hit_trigger_type != 0 && this.data.useCollider && this.IsMaster && base.IsC2P;
			}
		}

		protected override void OnInit()
		{
			base.OnInit();
			if (GameManager.Instance == null)
			{
				return;
			}
			this.skillData = GameManager.Instance.SkillData.GetData(this.skillKey);
			this.skill = base.unit.getSkillOrAttackById(this.skillKey.SkillID);
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			this.OnSkillDamageCallback = null;
			this.OnSkillEndCallback = null;
		}

		protected virtual void OnDamage(BaseAction action, List<Units> targets)
		{
			this.OnSkillDamage(this, targets);
		}

		protected virtual void OnDamageEnd(BaseAction action)
		{
			this.OnActionEndCallback = (Callback<BaseAction>)Delegate.Remove(this.OnActionEndCallback, new Callback<BaseAction>(this.OnDamageEnd));
			this.OnSkillEnd(this);
		}

		protected override void OnActionEnd()
		{
			this.OnDamageEnd(this);
			base.OnActionEnd();
		}

		protected virtual void OnSkillDamage(BaseSkillAction action, List<Units> targets)
		{
			if (this.OnSkillDamageCallback != null)
			{
				this.OnSkillDamageCallback(action, targets);
			}
		}

		protected virtual void OnSkillEnd(BaseSkillAction action)
		{
			if (this.OnSkillEndCallback != null)
			{
				this.OnSkillEndCallback(this);
			}
		}

		protected void AddHighEff(SkillDataKey skill_key, SkillPhrase skillPhrase, List<Units> targets = null, Vector3? skillPosition = null)
		{
			SkillUtility.AddHighEff(base.unit, this.skillKey, skillPhrase, targets, skillPosition);
		}

		protected void AddBuff(SkillDataKey skill_key, SkillPhrase skillPhrase, List<Units> targets = null)
		{
			SkillUtility.AddBuff(base.unit, this.skillKey, skillPhrase, targets);
		}

		public void DoRatate(bool isFast = true, float limitTime = 0f)
		{
			if (this.skill == null)
			{
				return;
			}
			if (this.skill.IsAttack)
			{
				if (this.targetUnits != null && this.targetUnits.Count > 0 && this.targetUnits[0] != null && !base.unit.isBuilding)
				{
					base.unit.TurnToTarget(new Vector3?(this.targetUnits[0].trans.position), isFast, true, limitTime);
				}
			}
			else
			{
				Vector3? vector = this.targetPosition;
				if (vector.HasValue)
				{
					if (this.targetUnits == null || this.targetUnits.Count <= 0 || !(this.targetUnits[0] != null) || this.targetUnits[0].unique_id != base.unit.unique_id)
					{
						if (!this.skill.IsInstance)
						{
							if (!base.unit.isBuilding && !TagManager.CheckTag(base.unit, TargetTag.Pet))
							{
								Units arg_14D_0 = base.unit;
								Vector3? vector2 = this.targetPosition;
								arg_14D_0.TurnToTarget(new Vector3?(vector2.Value), isFast, true, limitTime);
							}
						}
					}
				}
			}
		}

		protected override void RemoveActionFromSkill(BaseAction action)
		{
			if (this.skill == null)
			{
				return;
			}
			this.skill.RemAction(action);
		}

		protected override void AddActionToSkill(SkillCastPhase phase, BaseAction action)
		{
			if (this.skill == null)
			{
				return;
			}
			this.skill.AddAction(phase, action);
		}
	}
}

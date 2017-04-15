using System;
using System.Collections.Generic;
using UnityEngine;

namespace MobaFrame.SkillAction
{
	public class PerformAction : BasePerformAction
	{
		public Units CasterUnit;

		public List<Units> targetUnits;

		public Vector3? targetPosition;

		public BaseHighEffAction highEffAction;

		private BasePerformAction BasePerformAction;

		protected override void OnRecordStart()
		{
		}

		protected override bool doAction()
		{
			if (this.data == null)
			{
				Debug.LogError(" #### perform error : cannot find " + this.performId);
				return false;
			}
			switch (this.data.effect_type)
			{
			case PerformType.Normal:
				this.BasePerformAction = ActionManager.Simple(this.skillKey, this.performId, base.unit, this.CasterUnit, this.targetPosition);
				break;
			case PerformType.Missile:
				if (this.targetUnits == null || this.targetUnits.Count == 0 || this.targetUnits[0] == null)
				{
					return false;
				}
				this.BasePerformAction = ActionManager.Missile(this.skillKey, this.performId, base.unit, this.targetUnits[0], this.targetPosition);
				break;
			case PerformType.Link:
				this.BasePerformAction = ActionManager.Link(this.skillKey, this.performId, base.unit, this.targetUnits, this.targetPosition, null);
				break;
			case PerformType.Bullet:
				this.BasePerformAction = ActionManager.Bullet(this.skillKey, this.performId, base.unit, this.GetFirstTarget(), this.targetPosition);
				break;
			case PerformType.Sputtering:
				if (this.data.effectParam1 == 1f)
				{
					this.BasePerformAction = ActionManager.SputteringMissile(this.skillKey, this.performId, base.unit, this.targetUnits, this.highEffAction, this.CasterUnit);
				}
				else if (this.data.effectParam1 == 2f)
				{
					this.BasePerformAction = ActionManager.SputteringLink(this.skillKey, this.performId, base.unit, this.targetUnits, this.highEffAction, this.CasterUnit);
				}
				else if (this.data.effectParam1 == 3f)
				{
					this.BasePerformAction = ActionManager.SputteringMissile(this.skillKey, this.performId, base.unit, this.targetUnits, this.highEffAction, this.CasterUnit);
				}
				break;
			case PerformType.Dragon:
				this.BasePerformAction = ActionManager.DragonMissile(this.skillKey, this.performId, base.unit, this.targetUnits);
				break;
			case PerformType.Absorb:
				this.BasePerformAction = ActionManager.AbsorbMissile(this.skillKey, this.performId, base.unit, base.unit.GetLastKilledTarget());
				break;
			case PerformType.Parabola:
			{
				Vector3? vector = this.targetPosition;
				float distanceToTarget;
				if (vector.HasValue)
				{
					Vector3? vector2 = this.targetPosition;
					distanceToTarget = Vector3.Distance(vector2.Value, base.unit.mTransform.position);
				}
				else
				{
					if (this.GetFirstTarget() == null)
					{
						return false;
					}
					if (base.unit == null)
					{
						return false;
					}
					distanceToTarget = Vector3.Distance(this.GetFirstTarget().mTransform.position, base.unit.mTransform.position);
				}
				this.BasePerformAction = ActionManager.ParabolaMissile(this.skillKey, this.performId, base.unit, distanceToTarget, this.GetFirstTarget(), this.targetPosition);
				break;
			}
			case PerformType.MissileBomb:
			{
				Vector3? vector3 = this.targetPosition;
				float distanceToTarget2;
				if (vector3.HasValue)
				{
					Vector3? vector4 = this.targetPosition;
					distanceToTarget2 = Vector3.Distance(vector4.Value, base.unit.mTransform.position);
				}
				else
				{
					if (this.IsNoTargets())
					{
						return false;
					}
					distanceToTarget2 = Vector3.Distance(this.GetFirstTarget().mTransform.position, base.unit.mTransform.position);
				}
				this.BasePerformAction = ActionManager.MissileBomb(this.skillKey, this.performId, base.unit, distanceToTarget2, this.GetFirstTarget(), this.targetPosition);
				break;
			}
			case PerformType.Bomb:
				this.BasePerformAction = ActionManager.Bomb(this.skillKey, this.performId, base.unit, this.GetFirstTarget(), this.targetPosition);
				break;
			case PerformType.Hit:
				if (this.targetUnits != null)
				{
					for (int i = 0; i < this.targetUnits.Count; i++)
					{
						this.BasePerformAction = ActionManager.Hit(this.skillKey, this.performId, this.targetUnits[i], base.unit);
					}
				}
				break;
			case PerformType.FenLieJian:
				this.BasePerformAction = ActionManager.FenLieJian(this.skillKey, this.performId, base.unit, this.targetUnits, null);
				break;
			case PerformType.ChongZhuang:
				this.BasePerformAction = ActionManager.ChongZhuangAction(this.skillKey, this.performId, base.unit, this.GetFirstTarget(), this.targetPosition);
				break;
			case PerformType.Darts:
				this.BasePerformAction = ActionManager.Darts(this.skillKey, this.performId, base.unit, this.targetPosition);
				break;
			case PerformType.PassiveSputtering:
				this.BasePerformAction = ActionManager.PassiveLink(this.skillKey, this.performId, base.unit, this.targetUnits, this.highEffAction);
				break;
			case PerformType.BulletJinkesi:
				this.BasePerformAction = ActionManager.BulletJinkesi(this.skillKey, this.performId, base.unit, this.GetFirstTarget(), this.targetPosition);
				break;
			case PerformType.Hook:
				this.BasePerformAction = ActionManager.Hook(this.skillKey, this.performId, base.unit, this.GetFirstTarget(), this.targetPosition);
				break;
			case PerformType.PointMissile:
				this.BasePerformAction = ActionManager.PointMissile(this.skillKey, this.performId, base.unit, this.targetPosition);
				break;
			case PerformType.CurveMissile:
			{
				Vector3? vector5 = this.targetPosition;
				float distanceToTarget3;
				if (vector5.HasValue)
				{
					Vector3? vector6 = this.targetPosition;
					distanceToTarget3 = Vector3.Distance(vector6.Value, base.unit.mTransform.position);
				}
				else
				{
					if (this.GetFirstTarget() == null)
					{
						return false;
					}
					if (base.unit == null)
					{
						return false;
					}
					distanceToTarget3 = Vector3.Distance(this.GetFirstTarget().mTransform.position, base.unit.mTransform.position);
				}
				this.BasePerformAction = ActionManager.CurveMissile(this.skillKey, this.performId, base.unit, distanceToTarget3, this.GetFirstTarget(), this.targetPosition);
				break;
			}
			}
			if (this.BasePerformAction != null)
			{
				this.BasePerformAction.OnDamageCallback = new Callback<BaseAction, List<Units>>(this.OnDamage);
				this.BasePerformAction.OnDamageEndCallback = new Callback<BaseAction>(this.OnDamageEnd);
				this.AddAction(this.BasePerformAction);
				if (base.unit != null)
				{
					Skill skillById = base.unit.skillManager.getSkillById(this.skillKey.SkillID);
					if (skillById != null)
					{
						skillById.OnPerFormeCreate(this.BasePerformAction);
					}
				}
				return true;
			}
			return false;
		}

		protected void OnActionEnd(BaseAction action)
		{
			this.Destroy();
		}

		public bool IsNoTargets()
		{
			return this.targetUnits != null && this.targetUnits.Count == 0;
		}

		public Units GetFirstTarget()
		{
			if (this.targetUnits == null)
			{
				return null;
			}
			if (this.targetUnits.Count == 0)
			{
				return null;
			}
			return this.targetUnits[0];
		}
	}
}

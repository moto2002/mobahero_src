using Com.Game.Module;
using MobaProtocol.Data;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace MobaFrame.SkillAction
{
	public class BaseHighEffAction : CompositeAction
	{
		public string higheffId;

		public Units owner;

		public Vector3? skillPosition;

		public List<Units> targetUnits;

		public string skillId;

		public float rotateY;

		protected HighEffectData data;

		protected string[] performIds;

		protected bool isSetState;

		protected Skill skill;

		public Callback Callback_OnDestroy;

		protected virtual bool IsInState
		{
			get
			{
				return false;
			}
		}

		protected override void OnInit()
		{
			base.OnInit();
			this.data = Singleton<HighEffectDataManager>.Instance.GetVo(this.higheffId);
			if (this.data == null)
			{
				Debug.LogError("没有这个高级效果，检查表,error ID=" + this.higheffId);
				this.Destroy();
				return;
			}
			if (base.unit != null)
			{
				this.skill = base.unit.getSkillOrAttackById(this.skillId);
			}
			this.performIds = this.data.performIds;
			base.IsAutoDestroy = this.data.isAutoDestroy;
		}

		public override void Destroy()
		{
			base.Destroy();
			if (this.Callback_OnDestroy != null)
			{
				this.Callback_OnDestroy();
			}
		}

		protected override bool doAction()
		{
			if (!this.IsInState && this.CheckCondition())
			{
				this.SetState();
				this.StartHighEff();
				return true;
			}
			this.isPlaying = false;
			return false;
		}

		protected override void OnStop()
		{
			base.OnStop();
			this.StopHighEff();
			if (this.isSetState)
			{
				this.RevertState();
			}
			base.RecordEnd();
			base.SendEnd();
		}

		protected virtual void StartHighEff()
		{
			if (this.skill != null)
			{
				this.skill.OnHighEffStart(this);
			}
			this.doStartHighEffect_Damage();
			this.doStartHighEffect_Perform();
			this.doStartHighEffect_AttachHighEffect();
			this.doStartHighEffect_AttachBuff();
			this.doStartHighEffect_Special();
		}

		protected virtual void StopHighEff()
		{
		}

		protected virtual void SetState()
		{
			this.isSetState = true;
		}

		protected virtual void RevertState()
		{
			this.isSetState = false;
		}

		protected virtual bool CheckCondition()
		{
			return true;
		}

		protected override void OnRecordStart()
		{
		}

		protected override void OnRecordEnd()
		{
		}

		protected override void OnSendStart()
		{
		}

		protected override void OnSendEnd()
		{
		}

		protected virtual void doStartHighEffect_AttachHighEffect()
		{
			if (this.targetUnits == null)
			{
				return;
			}
			if (this.data.attachHighEffs != null)
			{
				for (int i = 0; i < this.data.attachHighEffs.Length; i++)
				{
					string text = this.data.attachHighEffs[i];
					if (StringUtils.CheckValid(text))
					{
						for (int j = 0; j < this.targetUnits.Count; j++)
						{
							if (!SkillUtility.IsImmunityHighEff(this.targetUnits[j], text))
							{
								ActionManager.AddHighEffect(text, this.skillId, this.targetUnits[j], base.unit, null, true);
							}
						}
					}
				}
			}
		}

		public void PVP_DoStartHighEffect_AttachPerformHighEffect(HighEffInfo info)
		{
			if (this.targetUnits == null)
			{
				return;
			}
			if (this.data.attachHighEffs != null)
			{
				for (int i = 0; i < this.data.attachHighEffs.Length; i++)
				{
					string text = this.data.attachHighEffs[i];
					if (StringUtils.CheckValid(text))
					{
						for (int j = 0; j < this.targetUnits.Count; j++)
						{
							if (!SkillUtility.IsImmunityHighEff(this.targetUnits[j], text))
							{
								ActionManager.AddHighEffect(text, this.skillId, this.targetUnits[j], base.unit, null, true);
							}
						}
					}
				}
			}
		}

		protected virtual void doStartHighEffect_AttachBuff()
		{
			if (this.targetUnits == null)
			{
				return;
			}
			if (this.data.attachBuffs != null)
			{
				for (int i = 0; i < this.data.attachBuffs.Length; i++)
				{
					string text = this.data.attachBuffs[i];
					if (StringUtils.CheckValid(text))
					{
						for (int j = 0; j < this.targetUnits.Count; j++)
						{
							if (!SkillUtility.IsImmunityBuff(this.targetUnits[j], text))
							{
								ActionManager.AddBuff(text, this.targetUnits[j], base.unit, true, string.Empty);
							}
						}
					}
				}
			}
		}

		protected virtual void doStartHighEffect_Perform()
		{
			if (this.targetUnits != null)
			{
				for (int i = 0; i < this.targetUnits.Count; i++)
				{
					if (this.targetUnits[i] != null)
					{
						this.PlayPerform(this.targetUnits[i]);
					}
				}
			}
		}

		protected virtual void doStartHighEffect_Damage()
		{
			if (this.targetUnits == null)
			{
				return;
			}
			if (this.data.damage_ids != null)
			{
				for (int i = 0; i < this.targetUnits.Count; i++)
				{
					if (this.targetUnits[i] != null && this.targetUnits[i].dataChange != null)
					{
						this.targetUnits[i].dataChange.doSkillWoundAction(this.data.damage_ids, base.unit, true, new float[0]);
					}
				}
			}
		}

		protected virtual void doStartHighEffect_Special()
		{
		}

		protected virtual void doStartHighEffect_End()
		{
		}

		public virtual void DoDamage(Units target, int count = 1)
		{
		}

		protected void PlayPerform(Units targetUnit)
		{
			if (this.performIds != null)
			{
				for (int i = 0; i < this.performIds.Length; i++)
				{
					this.AddAction(ActionManager.PlayPerform(new SkillDataKey(string.Empty, 0, 0), this.performIds[i], targetUnit, null, null, true, base.unit));
				}
			}
		}

		protected void PlayPerform(string perform_id, Units actionUnit, List<Units> targetUntis)
		{
			this.AddAction(ActionManager.PlayPerform(new SkillDataKey(string.Empty, 0, 0), perform_id, actionUnit, this.targetUnits, null, true, null));
		}

		protected void PlayEffects(Units targetUnit)
		{
			if (this.performIds != null)
			{
				for (int i = 0; i < this.performIds.Length; i++)
				{
					this.AddAction(ActionManager.PlayEffect(this.performIds[i], targetUnit, null, null, true, string.Empty, base.unit));
				}
			}
		}

		protected void PlayEffects(string perform_id, Units targetUnit)
		{
			this.AddAction(ActionManager.PlayEffect(perform_id, targetUnit, null, null, true, string.Empty, base.unit));
		}

		protected void EnableAction(Units targetUnit, bool b, float cool = 0f)
		{
			if (targetUnit == null)
			{
				return;
			}
			if (!b)
			{
				targetUnit.InterruptAction(SkillInterruptType.Passive);
				if (targetUnit.isPlayer && targetUnit.UnitController != null)
				{
					targetUnit.UnitController.OnStopSkill();
				}
			}
			targetUnit.StopMove();
			targetUnit.SetCanAIControl(b);
			targetUnit.SetCanAction(b);
			targetUnit.SetCanMove(b);
			targetUnit.SetCanEnableActionHighEff(b);
		}

		protected bool IsTargetUnitValid(Units targetUnit)
		{
			return !(targetUnit == null) && targetUnit.isLive && !targetUnit.isTower && !targetUnit.isHome;
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

		protected void AddActionToSkill(BaseAction action)
		{
			if (this.skill == null)
			{
				return;
			}
			this.AddActionToSkill(this.skill.CastPhase, action);
		}
	}
}

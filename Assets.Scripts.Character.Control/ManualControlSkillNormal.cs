using Assets.MobaTools.TriggerPlugin.Scripts;
using Com.Game.Module;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Character.Control
{
	public class ManualControlSkillNormal : ManualControllerCom
	{
		private List<VTrigger> listTrigger = new List<VTrigger>();

		private ManualController controller;

		private ManualControlTarget targetCom;

		private ManualControlSignalMng signalCom;

		private Skill tmpLastSkill;

		private float tmpLastSkillTm;

		private Skill readySkill;

		private Skill lastReadySkill;

		private float readyTm;

		public ManualControlSkillNormal(ManualController c)
		{
			this.controller = c;
			this.self = c.ControlUnit;
			this.targetCom = this.controller.TargetCom;
			this.signalCom = this.controller.SignalCom;
		}

		private void ClearReadySkill()
		{
			if (this.readySkill != null)
			{
				Singleton<TriggerManager>.Instance.SendUnitSkillPointerEvent(UnitEvent.UnitSkillCmdHidePointer, this.self, this.readySkill, Vector3.zero);
				this.readySkill = null;
				Singleton<SkillView>.Instance.ClearSelectState();
				Singleton<SkillView>.Instance.HideSkillWarn();
			}
		}

		private void SetReadySkill(Skill skill, Units targetUnits)
		{
			this.ClearReadySkill();
			this.readySkill = skill;
			this.lastReadySkill = skill;
			this.readyTm = Time.realtimeSinceStartup;
			Singleton<SkillView>.Instance.SetSelectState();
			Singleton<SkillView>.Instance.ShowSkillWarn("请选择目标");
			Singleton<TriggerManager>.Instance.SendUnitSkillPointerEvent(UnitEvent.UnitSkillCmdShowPointer, this.self, this.readySkill, this.GetSkillPointerPos(skill, targetUnits));
		}

		private void DragSkillPointer(Vector3 targetPos)
		{
			Singleton<TriggerManager>.Instance.SendUnitSkillPointerEvent(UnitEvent.UnitSkillCmdDragPointer, this.self, this.readySkill, targetPos);
		}

		private bool ReadySkillOld()
		{
			return this.readySkill == null || Time.realtimeSinceStartup - this.readyTm > 0.6f;
		}

		public override void OnInit()
		{
			base.OnInit();
			VTrigger item = TriggerManager.CreateUnitEventTrigger(UnitEvent.UnitSkillOver, null, new TriggerAction(this.OnSkillOver), this.self.unique_id);
			this.listTrigger.Add(item);
			item = TriggerManager.CreateUnitEventTrigger(UnitEvent.UnitSkillFailed, null, new TriggerAction(this.OnSkillFailed), this.self.unique_id);
			this.listTrigger.Add(item);
			item = TriggerManager.CreateUnitEventTrigger(UnitEvent.UnitDeath, null, new TriggerAction(this.OnUnitDeath), this.self.unique_id);
			this.listTrigger.Add(item);
			item = TriggerManager.CreateUnitEventTrigger(UnitEvent.UnitSkillCmdStart, null, new TriggerAction(this.OnSkillStart), this.self.unique_id);
			this.listTrigger.Add(item);
			this.self.ClearBornPowerObjSkillData();
		}

		public override void OnStop()
		{
			base.OnStop();
			this.targetCom.ClearSkillFlag();
			this.targetCom.ClearMoveFlag();
		}

		public override void OnExit()
		{
			foreach (VTrigger current in this.listTrigger)
			{
				TriggerManager.DestroyTrigger(current);
			}
			base.OnExit();
		}

		public override void OnDown(TriggerParamTouch param)
		{
			if (GlobalSettings.Instance.isLockView && this.readySkill == null)
			{
				this.OnTrySkillOrMove(param, TouchEventType.down);
			}
		}

		public override void OnPress(TriggerParamTouch param)
		{
			if (GlobalSettings.Instance.isLockView)
			{
				this.targetCom.ClearTarget();
				this.targetCom.UpdateTarget(param.Pos);
				Vector3 groundPoint = this.targetCom.GroundPoint;
				Units targetUnit = this.targetCom.TargetUnit;
				if (!(targetUnit != null) || !targetUnit.CanBeSelected)
				{
				}
				if (!this.self.CanManualControl())
				{
					return;
				}
				if (this.readySkill != null)
				{
					this.targetCom.ClearMoveFlag();
					this.DragSkillPointer(groundPoint);
				}
				else
				{
					this.OnTrySkillOrMove(param, TouchEventType.press);
				}
			}
		}

		public override void OnUp(TriggerParamTouch param)
		{
			if (GlobalSettings.Instance.isLockView)
			{
				if (this.readySkill != null)
				{
					this.OnTrySkillOrMove(param, TouchEventType.up);
				}
			}
			else
			{
				this.OnTrySkillOrMove(param, TouchEventType.up);
			}
		}

		public new virtual void OnMoveEnd(TriggerParamTouch param)
		{
			if (GlobalSettings.Instance.isLockView)
			{
				if (this.readySkill != null)
				{
					this.OnTrySkillOrMove(param, TouchEventType.moveEnd);
				}
				else
				{
					this.targetCom.ClearTarget();
					this.targetCom.UpdateTarget(param.Pos);
					Units units = this.targetCom.TargetUnit;
					if (units != null && !units.CanBeSelected)
					{
						units = null;
					}
					if (!this.self.CanManualControl())
					{
						return;
					}
					if (units == null)
					{
						this.OnTrySkillOrMove(param, TouchEventType.moveEnd);
					}
				}
			}
		}

		private void OnTrySkillOrMove(TriggerParamTouch param, TouchEventType type)
		{
			this.targetCom.ClearTarget();
			this.targetCom.UpdateTarget(param.Pos);
			Vector3 groundPoint = this.targetCom.GroundPoint;
			Units units = this.targetCom.TargetUnit;
			bool forceSend = type != TouchEventType.press;
			if (units != null && !units.CanBeSelected)
			{
				units = null;
			}
			if (!this.self.CanManualControl())
			{
				return;
			}
			if (this.readySkill == null)
			{
				if (units == null)
				{
					if (!this.targetCom.ValidGroudPoint)
					{
						return;
					}
					this.DisableAIAutoAttackMove();
					this.targetCom.DrawMoveFlag();
					this.self.mCmdCacheController.EnqueueMoveCmd(groundPoint, forceSend);
					this.lastTarget = null;
				}
				else if (type == TouchEventType.down || (type != TouchEventType.down && this.lastTarget != units))
				{
					this.lastTarget = units;
					this.DisableAIAutoAttackMove();
					this.targetCom.ClearMoveFlag();
					this.self.mCmdCacheController.EnqueueSkillCmd(this.self.attacks[0], groundPoint, units, true, false);
				}
			}
			else
			{
				this.targetCom.ClearTarget();
				this.targetCom.UpdateTargetAll(param.Pos, this.readySkill.data.targetCamp);
				this.targetCom.ClearMoveFlag();
				this.DragSkillPointer(this.targetCom.GroundPoint);
				this.LaunchSkill(this.readySkill, this.targetCom.TargetUnit, this.targetCom.GroundPoint, false);
				this.ClearReadySkill();
				this.lastTarget = null;
			}
		}

		private void OnSkillOver()
		{
		}

		private void OnSkillFailed()
		{
		}

		private void OnUnitDeath()
		{
			this.ClearReadySkill();
		}

		private void OnSkillStart()
		{
			Skill skillContext = this.self.GetSkillContext();
			if (skillContext != null && skillContext == this.readySkill)
			{
				this.ClearReadySkill();
			}
		}

		public override void OnSkill(ITriggerDoActionParam param)
		{
			TriggerParamSkillControl triggerParamSkillControl = param as TriggerParamSkillControl;
			Skill skillOrAttackById = this.self.getSkillOrAttackById(triggerParamSkillControl.SkillID);
			Units selectedTarget = PlayerControlMgr.Instance.GetSelectedTarget();
			Units units = null;
			TargetTag targetTag = skillOrAttackById.data.targetTag;
			if (selectedTarget == null || skillOrAttackById.NeedAutoLaunchToHero())
			{
				if (selectedTarget == null && skillOrAttackById.skillIndex == 4 && units == null && targetTag == TargetTag.HeroAndMonster)
				{
					units = this.self;
				}
				else
				{
					units = FindTargetHelper.FindAutoSkillTarget(this.self, this.self.trans.position, TargetTag.Hero, (this.self.atk_type != 1) ? (base.GetSkillRange(skillOrAttackById) + 3f) : 6.5f, skillOrAttackById.data.targetCamp, null);
				}
			}
			if (units == null)
			{
				units = selectedTarget;
			}
			if (!skillOrAttackById.data.needTarget && skillOrAttackById.data.isMoveSkill)
			{
				units = null;
			}
			else if (((units == null || units.isHero) && targetTag == TargetTag.CreepsAndMinions) || targetTag == TargetTag.Monster || targetTag == TargetTag.Minions || targetTag == TargetTag.Creeps)
			{
				units = FindTargetHelper.FindAutoSkillTarget(this.self, this.self.trans.position, targetTag, (this.self.atk_type != 1) ? (base.GetSkillRange(skillOrAttackById) + 3f) : 6.5f, skillOrAttackById.data.targetCamp, null);
			}
			else if (units == null && targetTag == TargetTag.HeroAndMonster)
			{
				units = FindTargetHelper.FindAutoSkillTarget(this.self, this.self.trans.position, targetTag, (this.self.atk_type != 1) ? (base.GetSkillRange(skillOrAttackById) + 3f) : 6.5f, skillOrAttackById.data.targetCamp, skillOrAttackById);
			}
			this.targetCom.ClearMoveFlag();
			if (!this.self.CanManualControl())
			{
				return;
			}
			if (this.self.IsSkillCanTriggerBornPowerObj(triggerParamSkillControl.SkillID))
			{
				this.ClearReadySkill();
				this.LaunchSkill(skillOrAttackById, units, true);
				return;
			}
			if (!this.DoseSkillNeedDoubleClick(skillOrAttackById))
			{
				this.ClearReadySkill();
				this.LaunchSkill(skillOrAttackById, units, true);
				return;
			}
			if (skillOrAttackById == this.readySkill)
			{
				if (this.ReadySkillOld())
				{
					this.readyTm = Time.realtimeSinceStartup;
					this.ClearReadySkill();
				}
				else
				{
					this.ClearReadySkill();
					this.LaunchSkill(skillOrAttackById, units, true);
				}
				return;
			}
			if (this.readySkill == null && this.lastReadySkill == skillOrAttackById && Time.realtimeSinceStartup - this.readyTm < 0.5f)
			{
				this.ClearReadySkill();
				this.LaunchSkill(skillOrAttackById, units, true);
				return;
			}
			this.SetReadySkill(skillOrAttackById, units);
		}

		public override void OnNavigateEnd()
		{
			this.self.mCmdRunningController.OnMoveEnd();
			this.targetCom.ClearMoveFlag();
		}

		private Vector3 GetSkillPointerPos(Skill skill, Units targetUnits)
		{
			if (targetUnits != null)
			{
				return targetUnits.mTransform.position;
			}
			return this.self.mTransform.position + this.self.mTransform.forward * 10f;
		}

		private void LaunchSkill(Skill skill, Units targetUnits, bool isCrazyMode)
		{
			Vector3 vector = Vector3.zero;
			if (!skill.needTarget)
			{
				if (targetUnits != null && !targetUnits.CanBeSelected)
				{
					targetUnits = null;
				}
				if (targetUnits == this.self)
				{
					targetUnits = null;
				}
				vector = this.self.mTransform.forward;
				if (targetUnits != null)
				{
					vector = targetUnits.mTransform.position;
					if (isCrazyMode && Vector3.Distance(vector, this.self.mTransform.position) > skill.distance + 3f)
					{
						Vector3 a = vector - this.self.mTransform.position;
						a.Normalize();
						vector = this.self.mTransform.position + a * skill.distance;
					}
				}
				else
				{
					vector.Normalize();
					if (skill.distance == 0f)
					{
						vector = this.self.mTransform.position + vector;
					}
					else
					{
						vector = this.self.mTransform.position + vector * skill.distance;
					}
					if (isCrazyMode && skill.NeedResetTargetPos())
					{
						vector = skill.GetExtraTargetPos(targetUnits, vector, isCrazyMode);
					}
				}
				this.LaunchSkill(skill, targetUnits, vector, isCrazyMode);
			}
			else
			{
				targetUnits = base.GetSkillTarget(skill, targetUnits, true);
				targetUnits = skill.ReselectTarget(targetUnits, isCrazyMode);
				if (targetUnits != null)
				{
					this.LaunchSkill(skill, targetUnits, vector, isCrazyMode);
				}
				else
				{
					UIMessageBox.ShowMessage("未选择目标，不能释放！", 1.5f, 0);
				}
			}
		}

		private void LaunchSkill(Skill skill, Units targetUnits, Vector3 targetPos, bool isCrazyMode)
		{
			if (skill.needTarget)
			{
				targetUnits = base.GetSkillTarget(skill, targetUnits, isCrazyMode);
				if (targetUnits != null && !targetUnits.CanBeSelected)
				{
					targetUnits = null;
				}
				targetUnits = skill.ReselectTarget(targetUnits, isCrazyMode);
				if (targetUnits != null)
				{
					this.DisableAIAutoAttackMove();
					this.targetCom.ClearMoveFlag();
					if (targetUnits != null)
					{
						this.self.mCmdCacheController.EnqueueSkillCmd(skill.realSkillMainId, targetUnits.mTransform.position, targetUnits, true, isCrazyMode);
					}
				}
			}
			else
			{
				if (targetUnits != null && !targetUnits.CanBeSelected)
				{
					targetUnits = null;
				}
				if (targetUnits == this.self)
				{
					targetUnits = null;
				}
				this.DisableAIAutoAttackMove();
				this.targetCom.ClearMoveFlag();
				this.self.mCmdCacheController.EnqueueSkillCmd(skill.realSkillMainId, targetPos, targetUnits, true, isCrazyMode);
			}
		}

		private bool DoseSkillNeedDoubleClick(Skill skill)
		{
			return !skill.IsInstance;
		}

		private void DisableAIAutoAttackMove()
		{
			if (this.self.aiManager != null)
			{
				this.self.aiManager.EnableSearchTarget(false);
			}
			this.self.SetAIAutoAttackMove(false);
		}
	}
}

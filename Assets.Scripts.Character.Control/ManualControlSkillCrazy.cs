using Assets.MobaTools.TriggerPlugin.Scripts;
using Com.Game.Module;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Character.Control
{
	public class ManualControlSkillCrazy : ManualControllerCom
	{
		private ManualController controller;

		private List<VTrigger> listTrigger = new List<VTrigger>();

		private ManualControlSignalMng signalCom;

		private ManualControlTarget targetCom;

		public ManualControlSkillCrazy(ManualController c)
		{
			this.controller = c;
			this.self = c.ControlUnit;
			this.targetCom = this.controller.TargetCom;
			this.signalCom = this.controller.SignalCom;
		}

		public override void OnDown(TriggerParamTouch param)
		{
			if (GlobalSettings.Instance.isLockView)
			{
				this.OnTryMoveOrSkill(param, TouchEventType.down);
			}
		}

		public override void OnExit()
		{
			foreach (VTrigger current in this.listTrigger)
			{
				TriggerManager.DestroyTrigger(current);
			}
			base.OnExit();
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
			this.self.ClearBornPowerObjSkillData();
		}

		public new virtual void OnMoveEnd(TriggerParamTouch param)
		{
			if (GlobalSettings.Instance.isLockView)
			{
				this.OnTryMoveOrSkill(param, TouchEventType.moveEnd);
			}
		}

		public override void OnNavigateEnd()
		{
			this.self.mCmdRunningController.OnMoveEnd();
			this.targetCom.ClearMoveFlag();
		}

		public override void OnPress(TriggerParamTouch param)
		{
			if (GlobalSettings.Instance.isLockView)
			{
				this.OnTryMoveOrSkill(param, TouchEventType.press);
			}
		}

		public override void OnSkill(ITriggerDoActionParam param)
		{
			TriggerParamSkillControl triggerParamSkillControl = param as TriggerParamSkillControl;
			if (!this.self.CanManualControl())
			{
				return;
			}
			Skill skillOrAttackById = this.self.getSkillOrAttackById(triggerParamSkillControl.SkillID);
			Units selectedTarget = PlayerControlMgr.Instance.GetSelectedTarget();
			Units units = null;
			TargetTag targetTag = skillOrAttackById.data.targetTag;
			if (!skillOrAttackById.NeedCustomTargetInCrazy())
			{
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
					units = FindTargetHelper.FindAutoSkillTarget(this.self, this.self.trans.position, targetTag, (this.self.atk_type != 1) ? (base.GetSkillRange(skillOrAttackById) + 3f) : 6.5f, SkillTargetCamp.None, null);
				}
				else if (units == null && targetTag == TargetTag.HeroAndMonster)
				{
					units = FindTargetHelper.FindAutoSkillTarget(this.self, this.self.trans.position, targetTag, (this.self.atk_type != 1) ? (base.GetSkillRange(skillOrAttackById) + 3f) : 6.5f, skillOrAttackById.data.targetCamp, skillOrAttackById);
				}
			}
			else
			{
				units = skillOrAttackById.CustomTargetInCrazy();
			}
			if (skillOrAttackById.needTarget)
			{
				units = base.GetSkillTarget(skillOrAttackById, units, true);
				if (units != null && !units.CanBeSelected)
				{
					units = null;
				}
				units = skillOrAttackById.ReselectTarget(units, true);
				if (units != null)
				{
					this.DisableAIAutoAttackMove();
					this.targetCom.ClearMoveFlag();
					if (units != null)
					{
						this.self.mCmdCacheController.EnqueueSkillCmd(triggerParamSkillControl.SkillID, units.mTransform.position, units, true, true);
					}
				}
				else
				{
					Singleton<SkillView>.Instance.ShowSkillWarn("需要指定一个有效目标!");
					Singleton<TriggerManager>.Instance.SendUnitSkillStateEvent(UnitEvent.UnitSkillCmdCrazyPointer, this.self, skillOrAttackById);
				}
			}
			else
			{
				if (units != null && !units.CanBeSelected)
				{
					units = null;
				}
				if (units == this.self)
				{
					units = null;
				}
				Vector3 vector = this.self.mTransform.forward;
				if (units != null)
				{
					vector = units.mTransform.position;
					if (Vector3.Distance(vector, this.self.mTransform.position) > skillOrAttackById.distance + 3f)
					{
						Vector3 a = vector - this.self.mTransform.position;
						a.Normalize();
						vector = this.self.mTransform.position + a * skillOrAttackById.distance;
					}
				}
				else
				{
					vector.Normalize();
					if (skillOrAttackById.distance == 0f)
					{
						vector = this.self.mTransform.position + vector;
					}
					else
					{
						vector = this.self.mTransform.position + vector * skillOrAttackById.distance;
					}
				}
				if (skillOrAttackById.NeedResetTargetPos())
				{
					vector = skillOrAttackById.GetExtraTargetPos(units, vector, true);
				}
				this.DisableAIAutoAttackMove();
				this.targetCom.ClearMoveFlag();
				this.self.mCmdCacheController.EnqueueSkillCmd(triggerParamSkillControl.SkillID, vector, units, true, true);
			}
		}

		public override void OnStop()
		{
			base.OnStop();
			this.targetCom.ClearSkillFlag();
		}

		public override void OnUp(TriggerParamTouch param)
		{
			if (!GlobalSettings.Instance.isLockView)
			{
				this.OnTryMoveOrSkill(param, TouchEventType.up);
			}
		}

		private void OnSkillFailed()
		{
		}

		private void OnSkillOver()
		{
		}

		private void OnTryMoveOrSkill(TriggerParamTouch param, TouchEventType type)
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
				this.self.mCmdCacheController.EnqueueSkillCmd(this.self.attacks[0], groundPoint, units, true, true);
			}
		}

		private void OnUnitDeath()
		{
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

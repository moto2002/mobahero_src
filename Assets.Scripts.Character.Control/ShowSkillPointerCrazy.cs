using MobaHeros;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace Assets.Scripts.Character.Control
{
	public class ShowSkillPointerCrazy : StaticUnitComponent
	{
		private CoroutineManager coroutineManager = new CoroutineManager();

		private List<VTrigger> listTrigger = new List<VTrigger>();

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
			VTrigger item = TriggerManager.CreateUnitEventTrigger(UnitEvent.UnitSkillCmdReady, null, new TriggerAction(this.OnSkillReady), this.self.unique_id);
			this.listTrigger.Add(item);
			item = TriggerManager.CreateUnitEventTrigger(UnitEvent.UnitSkillCmdStart, null, new TriggerAction(this.OnSkillStart), this.self.unique_id);
			this.listTrigger.Add(item);
			item = TriggerManager.CreateUnitEventTrigger(UnitEvent.UnitSkillCmdFailed, null, new TriggerAction(this.OnSkillFailed), this.self.unique_id);
			this.listTrigger.Add(item);
			item = TriggerManager.CreateUnitEventTrigger(UnitEvent.UnitSkillCmdEnd, null, new TriggerAction(this.OnSkillFailed), this.self.unique_id);
			this.listTrigger.Add(item);
			item = TriggerManager.CreateUnitEventTrigger(UnitEvent.UnitSkillCmdOver, null, new TriggerAction(this.OnSkillFailed), this.self.unique_id);
			this.listTrigger.Add(item);
			item = TriggerManager.CreateUnitEventTrigger(UnitEvent.UnitSkillControlReset, null, new TriggerAction(this.OnReset), this.self.unique_id);
			this.listTrigger.Add(item);
			item = TriggerManager.CreateUnitEventTrigger(UnitEvent.UnitSkillCmdShowPointer, null, new TriggerAction(this.OnShowPointer), this.self.unique_id);
			this.listTrigger.Add(item);
			item = TriggerManager.CreateUnitEventTrigger(UnitEvent.UnitSkillCmdHidePointer, null, new TriggerAction(this.OnHidePointer), this.self.unique_id);
			this.listTrigger.Add(item);
			item = TriggerManager.CreateUnitEventTrigger(UnitEvent.UnitSkillCmdCrazyPointer, null, new TriggerAction(this.OnCrazyPointer), this.self.unique_id);
			this.listTrigger.Add(item);
		}

		private void doShowPointer(Skill skill, Vector3 targetPos)
		{
			SkillPointerManager.CreateSkillPointer(this.self, skill.data.skillId, skill.distance);
			if (this.self.mSkillPointer.IsValid<SkillPointer>())
			{
				this.self.mSkillPointer.Component.ChangeTra(targetPos);
			}
			this.self.ShowSkillPointer();
		}

		private void HideSkillPointer()
		{
			this.self.HideSkillPointer();
		}

		[DebuggerHidden]
		private IEnumerator Hide()
		{
			ShowSkillPointerCrazy.<Hide>c__Iterator35 <Hide>c__Iterator = new ShowSkillPointerCrazy.<Hide>c__Iterator35();
			<Hide>c__Iterator.<>f__this = this;
			return <Hide>c__Iterator;
		}

		private void OnCrazyPointer()
		{
			this.ShowPointer();
			this.coroutineManager.StartCoroutine(this.Hide(), true);
		}

		private void ShowPointer()
		{
			if (!this.self.isPlayer)
			{
				return;
			}
			Skill skillContext = this.self.GetSkillContext();
			Vector3 posContext = this.self.GetPosContext();
			if (skillContext != null && skillContext.needTarget)
			{
				SkillPointerManager.CreateSkillPointer(this.self, skillContext.data.skillId, skillContext.distance);
				if (this.self.mSkillPointer.IsValid<SkillPointer>())
				{
					this.self.mSkillPointer.Component.ChangeTra(posContext);
				}
				this.self.ShowSkillPointer();
			}
		}

		private void ShowSkillPointer()
		{
			CmdRunningSkill runningSkillCmd = this.self.mCmdRunningController.GetRunningSkillCmd();
			if (runningSkillCmd != null && runningSkillCmd.skill != null && runningSkillCmd.skill.IsDrag && !runningSkillCmd.skill.needTarget && runningSkillCmd.skill.data.skillCastingType == 1)
			{
				this.doShowPointer(runningSkillCmd.skill, runningSkillCmd.targetPos);
			}
		}

		private void OnHidePointer()
		{
			if (!this.self.isPlayer)
			{
				return;
			}
			this.HideSkillPointer();
		}

		private void OnReset()
		{
			if (!this.self.isPlayer)
			{
				return;
			}
			this.HideSkillPointer();
		}

		private void OnShowPointer()
		{
			if (!this.self.isPlayer)
			{
				return;
			}
			this.ShowSkillPointer();
		}

		private void OnSkillFailed()
		{
			if (!this.self.isPlayer)
			{
				return;
			}
			this.HideSkillPointer();
		}

		private void OnSkillReady()
		{
			if (!this.self.isPlayer)
			{
				return;
			}
			this.ShowSkillPointer();
		}

		private void OnSkillStart()
		{
			if (!this.self.isPlayer)
			{
				return;
			}
			this.HideSkillPointer();
		}
	}
}

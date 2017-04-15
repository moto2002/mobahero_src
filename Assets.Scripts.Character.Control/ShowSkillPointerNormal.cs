using Com.Game.Module;
using MobaHeros;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Character.Control
{
	public class ShowSkillPointerNormal : StaticUnitComponent
	{
		private List<VTrigger> listTrigger = new List<VTrigger>();

		public override void OnInit()
		{
			base.OnInit();
			VTrigger item = TriggerManager.CreateUnitEventTrigger(UnitEvent.UnitSkillCmdShowPointer, null, new TriggerAction(this.OnShowPointer), this.self.unique_id);
			this.listTrigger.Add(item);
			item = TriggerManager.CreateUnitEventTrigger(UnitEvent.UnitSkillCmdHidePointer, null, new TriggerAction(this.OnHidePointer), this.self.unique_id);
			this.listTrigger.Add(item);
			item = TriggerManager.CreateUnitEventTrigger(UnitEvent.UnitSkillCmdDragPointer, null, new TriggerAction(this.OnDragPointer), this.self.unique_id);
			this.listTrigger.Add(item);
			item = TriggerManager.CreateUnitEventTrigger(UnitEvent.UnitSkillControlReset, null, new TriggerAction(this.OnReset), this.self.unique_id);
			this.listTrigger.Add(item);
		}

		public override void OnExit()
		{
			foreach (VTrigger current in this.listTrigger)
			{
				TriggerManager.DestroyTrigger(current);
			}
			base.OnExit();
		}

		private void ShowSkillPointer()
		{
			Skill skillContext = this.self.GetSkillContext();
			Vector3 posContext = this.self.GetPosContext();
			if (skillContext != null && skillContext.IsDrag)
			{
				this.doShowPointer(skillContext, posContext);
			}
		}

		private void HideSkillPointer()
		{
			this.self.HideSkillPointer();
			if (this.self.isPlayer)
			{
				Singleton<MiniMapView>.Instance.CancelTapMiniMap();
			}
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

		private void DragSkillPointer()
		{
			Skill skillContext = this.self.GetSkillContext();
			Vector3 posContext = this.self.GetPosContext();
			if (this.self.mSkillPointer.IsValid<SkillPointer>())
			{
				this.self.mSkillPointer.Component.ChangeTra(posContext);
			}
		}

		private void OnShowPointer()
		{
			if (!this.self.isPlayer)
			{
				return;
			}
			this.ShowSkillPointer();
		}

		private void OnHidePointer()
		{
			if (!this.self.isPlayer)
			{
				return;
			}
			this.HideSkillPointer();
		}

		private void OnDragPointer()
		{
			if (!this.self.isPlayer)
			{
				return;
			}
			this.DragSkillPointer();
		}

		private void OnReset()
		{
			if (!this.self.isPlayer)
			{
				return;
			}
			this.HideSkillPointer();
		}
	}
}

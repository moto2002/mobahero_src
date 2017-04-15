using Com.Game.Module;
using MobaHeros.Pvp;
using MobaProtocol.Data;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace MobaFrame.SkillAction
{
	public class ReadySkillAction : BaseSkillAction
	{
		public float castBeforeTime;

		protected override void OnDestroy()
		{
			this.RemoveActionFromSkill(this);
			base.OnDestroy();
		}

		protected override void OnRecordStart()
		{
		}

		protected override void OnSendStart()
		{
			if (Singleton<PvpManager>.Instance.IsInPvp && base.unit.isPlayer && PvpServerStartSkillHeroList.IsStartByServer(base.unit.npc_id))
			{
				return;
			}
			ReadySkillInfo readySkillInfo = new ReadySkillInfo();
			readySkillInfo.unitId = base.unit.unique_id;
			readySkillInfo.skillId = this.skillKey.SkillID;
			Vector3? targetPosition = this.targetPosition;
			if (targetPosition.HasValue)
			{
				readySkillInfo.targetPosition = MoveController.Vector3ToSVector3(this.targetPosition.Value);
			}
			List<short> list = null;
			if (this.targetUnits != null)
			{
				list = new List<short>();
				for (int i = 0; i < this.targetUnits.Count; i++)
				{
					list.Add((short)this.targetUnits[i].unique_id);
				}
			}
			readySkillInfo.targetUnits = list;
			PvpEvent.SendReadySkillEvent(readySkillInfo);
		}

		protected override bool doAction()
		{
			if (this.skillData == null)
			{
				return false;
			}
			if (!this.skill.data.isCanMoveInSkillCastin && base.unit != null && base.unit.animController != null)
			{
				base.unit.animController.PlayAnim(AnimationType.Move, false, 0, true, false);
			}
			this.skill.OnSkillReadyBegin(this);
			base.DoRatate(false, this.castBeforeTime);
			this.AddActionToSkill(SkillCastPhase.Cast_Before, this);
			switch (this.skillData.config.skill_logic_type)
			{
			case 1:
			case 2:
			case 3:
			case 4:
			case 6:
			case 9:
			case 10:
			case 12:
			case 13:
			case 17:
			case 18:
			case 19:
			{
				SimpleReadySkillAction simpleReadySkillAction = ActionManager.SimpleReadySkill(this.skillKey, base.unit, this.targetUnits, this.targetPosition);
				if (simpleReadySkillAction != null)
				{
					this.AddAction(simpleReadySkillAction);
				}
				break;
			}
			case 5:
			{
				RenWuReadyAction renWuReadyAction = ActionManager.RenWuReadySkill(this.skillKey, base.unit, this.targetUnits, this.targetPosition);
				if (renWuReadyAction != null)
				{
					this.AddAction(renWuReadyAction);
				}
				break;
			}
			case 11:
			{
				base.unit.SetMoveAnimCool(this.skillData.castBefore + 0.5f, false);
				SimpleReadySkillAction simpleReadySkillAction2 = ActionManager.SimpleReadySkill(this.skillKey, base.unit, this.targetUnits, this.targetPosition);
				if (simpleReadySkillAction2 != null)
				{
					this.AddAction(simpleReadySkillAction2);
				}
				break;
			}
			case 14:
			{
				AnAhaReadyAction anAhaReadyAction = ActionManager.AnShaReadySkill(this.skillKey, base.unit, this.targetUnits, this.targetPosition);
				if (anAhaReadyAction != null)
				{
					this.AddAction(anAhaReadyAction);
				}
				break;
			}
			}
			return true;
		}
	}
}

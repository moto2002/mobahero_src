using Com.Game.Module;
using MobaHeros.Pvp;
using MobaProtocol.Data;
using System;
using System.Collections.Generic;

namespace MobaFrame.SkillAction
{
	public class HitSkillAction : BaseSkillAction
	{
		protected override void OnSendStart()
		{
			if (Singleton<PvpManager>.Instance.IsInPvp && PvpServerStartSkillHeroList.IsStartByServer(base.unit.npc_id))
			{
				return;
			}
			HitSkillInfo hitSkillInfo = new HitSkillInfo();
			hitSkillInfo.unitId = base.unit.unique_id;
			hitSkillInfo.skillId = this.skillKey.SkillID;
			if (this.targetUnits != null)
			{
				hitSkillInfo.targetIds = new List<short>();
				for (int i = 0; i < this.targetUnits.Count; i++)
				{
					if (this.targetUnits[i] != null)
					{
						hitSkillInfo.targetIds.Add((short)this.targetUnits[i].unique_id);
					}
				}
			}
			PvpEvent.SendHitSkillEvent(hitSkillInfo);
		}

		protected override bool doAction()
		{
			if (this.skillData == null)
			{
				return false;
			}
			string[] array = this.skillData.hit_actions;
			if (base.unit.HitActions != null)
			{
				array = base.unit.HitActions;
			}
			if (array != null)
			{
				for (int i = 0; i < array.Length; i++)
				{
					string text = array[i];
					string text2 = (this.skillData.friend_hit_actions == null || this.skillData.friend_hit_actions.Length <= i) ? array[i] : this.skillData.friend_hit_actions[i];
					for (int j = 0; j < this.targetUnits.Count; j++)
					{
						if (!(this.targetUnits[j] == null))
						{
							string perform_id;
							if (TeamManager.CanAssist(base.unit, this.targetUnits[j]))
							{
								perform_id = text2;
							}
							else
							{
								perform_id = text;
							}
							this.AddAction(ActionManager.Hit(this.skillKey, perform_id, this.targetUnits[j], base.unit));
						}
					}
				}
			}
			for (int k = 0; k < this.targetUnits.Count; k++)
			{
				this.DoEventDispatch(this.skillKey, this.targetUnits[k], base.unit);
			}
			base.unit.HitActions = null;
			if (this.skill != null)
			{
				this.skill.OnSkillHitBegin(this);
			}
			return true;
		}

		private void DoEventDispatch(SkillDataKey skill_key, Units targetUnit, Units casterUnit)
		{
			if (targetUnit == null || casterUnit == null)
			{
				return;
			}
			SkillData data = GameManager.Instance.SkillData.GetData(skill_key);
			if (data == null)
			{
				return;
			}
			Skill skillOrAttackById = casterUnit.getSkillOrAttackById(data.skillId);
			if (skillOrAttackById == null)
			{
				return;
			}
			if (skillOrAttackById.IsAttack)
			{
				targetUnit.SetAttackedYouTarget(casterUnit);
				Singleton<TriggerManager>.Instance.SendUnitStateEvent(UnitEvent.UnitAttackHitOther, casterUnit, null, null);
			}
			if (skillOrAttackById.IsSkill)
			{
				if (targetUnit.unique_id != base.unit.unique_id)
				{
					casterUnit.SetSkillHitedTarget(targetUnit);
				}
				if (casterUnit.isEnemy)
				{
					targetUnit.SetSkillHitedYouTarget(casterUnit);
				}
				Singleton<TriggerManager>.Instance.SendUnitStateEvent(UnitEvent.UnitSkillHitOther, casterUnit, null, null);
				if (skillOrAttackById.skillIndex != 3)
				{
					Singleton<TriggerManager>.Instance.SendUnitStateEvent(UnitEvent.UnitConjureQWE_HitOther, casterUnit, null, null);
				}
			}
			Singleton<TriggerManager>.Instance.SendUnitStateEvent(UnitEvent.UnitHit, targetUnit, null, null);
			if (skillOrAttackById.IsAttack)
			{
				Singleton<TriggerManager>.Instance.SendUnitStateEvent(UnitEvent.UnitBeAttackHit, targetUnit, null, null);
			}
			if (skillOrAttackById.IsSkill)
			{
				Singleton<TriggerManager>.Instance.SendUnitStateEvent(UnitEvent.UnitBeSkillHit, targetUnit, null, null);
			}
			if (StringUtils.CheckValid(base.unit.attackForTargetBuff))
			{
				ActionManager.AddBuff(base.unit.attackForTargetBuff, targetUnit, casterUnit, true, string.Empty);
				casterUnit.attackForTargetBuff = string.Empty;
			}
			if (!casterUnit.isLocalUnit && targetUnit != null && targetUnit.isLocalUnit)
			{
				if (casterUnit.m_nVisibleState >= 2)
				{
					casterUnit.m_fVisibleTimer = 3f;
				}
				if (GlobalSettings.FogMode == 1)
				{
					if (casterUnit != null && FogMgr.Instance.IsInFog(casterUnit) && targetUnit.isLocalUnit)
					{
						FogMgr.Instance.AddFogItem(casterUnit, 1f, 3f);
					}
				}
				else if (GlobalSettings.FogMode >= 2 && casterUnit != null && !FOWSystem.Instance.IsVisible(casterUnit.transform.position) && targetUnit.isLocalUnit)
				{
					FOWSystem.CreateStaticTimeRevealer(casterUnit.transform.position, 1f, 3f);
				}
			}
			if (targetUnit == PlayerControlMgr.Instance.GetPlayer())
			{
				Singleton<TriggerManager>.Instance.SendGameStateEvent(GameEvent.MainPlayerHitted);
			}
		}
	}
}

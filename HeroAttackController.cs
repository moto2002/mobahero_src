using Assets.Scripts.Character.Control;
using Com.Game.Module;
using Com.Game.Utils;
using MobaFrame.SkillAction;
using MobaHeros.Pvp;
using MobaProtocol.Data;
using System;
using System.Collections.Generic;
using UnityEngine;

public class HeroAttackController : AttackController
{
	private int combo_index = 1;

	private bool isComboAttack;

	private int max_combo_index = 3;

	public Dictionary<string, bool> sendUseSkills = new Dictionary<string, bool>();

	private Dictionary<string, long> sendUseSkillTicks = new Dictionary<string, long>();

	private bool PreComboAttack(int index)
	{
		if (this.self.getAttackByIndex(index) == null)
		{
			return false;
		}
		Units attackTarget = this.self.GetAttackTarget();
		return attackTarget == null && false;
	}

	public override void ComboAttack(Units target = null)
	{
		if (this.PreComboAttack(this.combo_index))
		{
			this.ComboAttack(3, target);
		}
		else
		{
			this.ComboAttack(this.combo_index, target);
		}
	}

	public void ComboAttack(int index, Units target)
	{
		if (Singleton<PvpManager>.Instance.IsInPvp)
		{
			if (this.self.CanAttack && !this.isComboAttack)
			{
				Skill attackByIndex = this.self.getAttackByIndex(index);
				if (attackByIndex == null)
				{
					ClientLogger.Error(" ComboAttack Error : attack " + index + " is NULL !");
					base.OnAttackFailedBeforeStart(null);
					return;
				}
				if (!this.sendUseSkillTicks.ContainsKey(attackByIndex.skillMainId))
				{
					this.sendUseSkillTicks.Add(attackByIndex.skillMainId, 0L);
				}
				if (!this.sendUseSkills.ContainsKey(attackByIndex.skillMainId))
				{
					this.sendUseSkills.Add(attackByIndex.skillMainId, false);
				}
				if (DateTime.Now.Ticks - this.sendUseSkillTicks[attackByIndex.skillMainId] > 5000000L)
				{
					this.sendUseSkills[attackByIndex.skillMainId] = false;
				}
				if (this.sendUseSkills[attackByIndex.skillMainId])
				{
					return;
				}
				if (target == null)
				{
					target = this.self.GetAttackTarget();
				}
				if (PvpServerStartSkillHeroList.IsStartByServer(this.self.npc_id))
				{
					this.ComboAttack_Impl(index, target, true);
				}
				else
				{
					ReadySkillCheckInfo readySkillCheckInfo = new ReadySkillCheckInfo();
					readySkillCheckInfo.unitId = this.self.unique_id;
					readySkillCheckInfo.skillId = attackByIndex.skillMainId;
					List<short> list = new List<short>();
					if (null != target)
					{
						list.Add((short)target.unique_id);
					}
					readySkillCheckInfo.targetUnits = list;
					PvpEvent.SendReadySkillCheckEvent(readySkillCheckInfo);
				}
			}
			else
			{
				base.OnAttackFailedBeforeStart(this.self.getAttackByIndex(0));
			}
		}
		else
		{
			this.ComboAttack_Impl(index, target, false);
		}
	}

	public void ComboAttack_Impl(int index, Units target, bool isPvp = false)
	{
		Skill attackByIndex = this.self.getAttackByIndex(index);
		if (this.self.CanAttack && !this.isComboAttack)
		{
			if (attackByIndex == null)
			{
				base.OnAttackFailedBeforeStart(null);
				return;
			}
			this.isComboAttack = true;
			if (target == null)
			{
				target = this.self.GetAttackTarget();
			}
			attackByIndex.attackTarget = target;
			if (attackByIndex.CheckCondition() && attackByIndex.CheckTargets())
			{
				if (!base.IsCurAttackRunnning() && this.self.CanAttack)
				{
					if (isPvp)
					{
						UseSkillInfo useSkillInfo = new UseSkillInfo
						{
							unitId = this.self.unique_id,
							skillId = attackByIndex.skillMainId,
							targetUnit = (!(target != null)) ? 0 : target.unique_id,
							targetPosition = (!(target != null)) ? null : MoveController.Vector3ToSVector3(target.transform.position),
							targetRotate = this.self.transform.eulerAngles.y
						};
						PvpEvent.SendUseSkill(useSkillInfo);
						this.sendUseSkills[attackByIndex.skillMainId] = true;
						this.sendUseSkillTicks[attackByIndex.skillMainId] = DateTime.Now.Ticks;
						this.self.moveController.clearContinueMovingTarget();
						if (GlobalSettings.Instance.ClientGoAhead)
						{
							List<Units> list = new List<Units>();
							if (target != null)
							{
								list.Add(target);
							}
							ActionManager.ReadySkill(new SkillDataKey(useSkillInfo.skillId, attackByIndex.skillLevel, 0), this.self, list, null, attackByIndex, false);
						}
					}
					else
					{
						attackByIndex.Start();
						attackByIndex.OnSkillStartCallback = new Callback<Skill>(base.OnAttackStart);
						attackByIndex.OnSkillEndCallback = new Callback<Skill>(base.OnAttackEnd);
						attackByIndex.OnSkillFailedBeforeStartCallback = new Callback<Skill>(base.OnAttackFailedBeforeStart);
						base.CurAttack = attackByIndex;
						this.self.playVoice("onNormalAttack");
						this.IncComboIndex();
					}
				}
			}
			else
			{
				base.OnAttackFailedBeforeStart(attackByIndex);
			}
			this.isComboAttack = false;
		}
		else
		{
			base.OnAttackFailedBeforeStart(null);
		}
	}

	public void IncComboIndex()
	{
		this.combo_index++;
		if (this.combo_index >= this.max_combo_index)
		{
			this.combo_index = 0;
		}
	}

	public override void Conjure(string skillId, Units target = null, Vector3? targetPos = null)
	{
		this.self.InterruptConjure(SkillInterruptType.Initiative);
		Skill skillById = this.self.getSkillById(skillId);
		if (!skillById.CheckCD(false))
		{
			Singleton<TriggerManager>.Instance.SendUnitSkillStateEvent(UnitEvent.UnitSkillCmdFailed, this.self, skillById);
			return;
		}
		if (!this.sendUseSkillTicks.ContainsKey(skillId))
		{
			this.sendUseSkillTicks.Add(skillId, 0L);
		}
		if (!this.sendUseSkills.ContainsKey(skillId))
		{
			this.sendUseSkills.Add(skillId, false);
		}
		if (DateTime.Now.Ticks - this.sendUseSkillTicks[skillId] > 5000000L)
		{
			this.sendUseSkills[skillId] = false;
		}
		if (this.sendUseSkills[skillId])
		{
			Singleton<TriggerManager>.Instance.SendUnitSkillStateEvent(UnitEvent.UnitSkillCmdFailed, this.self, skillById);
			return;
		}
		if (targetPos.HasValue)
		{
			float y = AstarPath.active.GetPosHeight(targetPos.Value) + 0.05f;
			targetPos = new Vector3?(new Vector3(targetPos.Value.x, y, targetPos.Value.z));
		}
		if (this.self.CanSkill || skillById.CheckSkillCanUseSpecial)
		{
			if (target == null && !targetPos.HasValue)
			{
				target = this.self.GetAttackTarget();
			}
			skillById.attackTarget = target;
			skillById.attackPosition = targetPos;
			skillById.externalPostion = targetPos;
			if (Singleton<PvpManager>.Instance.IsInPvp && this.self.isPlayer)
			{
				if (PvpServerStartSkillHeroList.IsStartByServer(this.self.npc_id))
				{
					this.Conjure_Impl(skillId, target, targetPos, true);
				}
				else if (UnitsSnapReporter.Instance.NetworkDelayInMs > 400L || UnityEngine.Random.Range(0, 100) > 90)
				{
					ReadySkillCheckInfo readySkillCheckInfo = new ReadySkillCheckInfo();
					readySkillCheckInfo.unitId = this.self.unique_id;
					readySkillCheckInfo.skillId = skillId;
					List<Units> skillTargets = skillById.GetSkillTargets();
					if (targetPos.HasValue)
					{
						readySkillCheckInfo.targetPosition = MoveController.Vector3ToSVector3(targetPos.Value);
					}
					List<short> list = new List<short>();
					foreach (Units current in skillTargets)
					{
						list.Add((short)current.unique_id);
					}
					readySkillCheckInfo.targetUnits = list;
					PvpEvent.SendReadySkillCheckEvent(readySkillCheckInfo);
					this.sendUseSkills[skillId] = true;
					this.sendUseSkillTicks[skillId] = DateTime.Now.Ticks;
				}
				else
				{
					this.Conjure_Impl(skillId, target, targetPos, false);
				}
			}
			else
			{
				this.Conjure_Impl(skillId, target, targetPos, false);
			}
		}
		else
		{
			Singleton<TriggerManager>.Instance.SendUnitSkillStateEvent(UnitEvent.UnitSkillCmdFailed, this.self, skillById);
		}
	}

	public void Conjure_Impl(string skillId, Units target, Vector3? targetPos, bool isPvp = false)
	{
		Skill skillById = this.self.getSkillById(skillId);
		if (SkillUtility.IsBurnUnitSkill(skillById) && targetPos.HasValue)
		{
			Units touchUnit = ManualControlTarget.GetTouchUnit(targetPos.Value);
			if (touchUnit != null && (touchUnit.GetType() == typeof(Tower) || touchUnit.GetType() == typeof(Home)))
			{
				base.OnSkillFailedBeforeStart(skillById);
				ClientLogger.Warn(string.Concat(new object[]
				{
					"不能摆蘑菇在这里",
					touchUnit,
					"  ",
					targetPos.Value
				}));
				return;
			}
		}
		if (this.self.CanSkill || skillById.CheckSkillCanUseSpecial)
		{
			if (target == null && !targetPos.HasValue)
			{
				target = this.self.GetAttackTarget();
			}
			skillById.attackTarget = target;
			skillById.attackPosition = targetPos;
			if (skillById.CheckCondition() && skillById.CheckTargets())
			{
				if (isPvp)
				{
					UseSkillInfo useSkillInfo = new UseSkillInfo
					{
						unitId = this.self.unique_id,
						skillId = skillById.skillMainId,
						targetUnit = (!(skillById.attackTarget != null)) ? 0 : skillById.attackTarget.unique_id,
						targetPosition = (!skillById.attackPosition.HasValue) ? null : MoveController.Vector3ToSVector3(skillById.attackPosition.Value),
						targetRotate = this.self.transform.eulerAngles.y
					};
					PvpEvent.SendUseSkill(useSkillInfo);
					this.sendUseSkills[skillId] = true;
					this.sendUseSkillTicks[skillId] = DateTime.Now.Ticks;
					this.self.moveController.clearContinueMovingTarget();
					if (GlobalSettings.Instance.ClientGoAhead)
					{
						List<Units> list = new List<Units>();
						if (target != null)
						{
							list.Add(target);
						}
						ActionManager.ReadySkill(new SkillDataKey(useSkillInfo.skillId, skillById.skillLevel, 0), this.self, list, skillById.attackPosition, skillById, false);
					}
				}
				else
				{
					this.self.InterruptAction(SkillInterruptType.Initiative);
					skillById.OnSkillStartCallback = new Callback<Skill>(base.OnSkillStart);
					skillById.OnSkillEndCallback = new Callback<Skill>(base.OnSkillEnd);
					skillById.OnSkillFailedBeforeStartCallback = new Callback<Skill>(base.OnSkillFailedBeforeStart);
					skillById.Start();
					base.currSkill = skillById;
				}
			}
			else
			{
				base.OnSkillFailedBeforeStart(skillById);
			}
		}
	}

	public void ReadySkillCheck(ReadySkillCheckInfo info)
	{
		Skill skillOrAttackById = this.self.getSkillOrAttackById(info.skillId);
		Units units = null;
		if (info.targetUnits != null && info.targetUnits.Count > 0)
		{
			units = MapManager.Instance.GetUnit((int)info.targetUnits[0]);
		}
		if (null == units && ((skillOrAttackById.needTarget && skillOrAttackById.IsSkill) || skillOrAttackById.IsAttack))
		{
			ClientLogger.Error("Recv ReadySkillCheck target null: " + info);
			return;
		}
		if (info.checkSuccess)
		{
			if (skillOrAttackById.IsAttack)
			{
				this.ComboAttack_Impl(skillOrAttackById.skillIndex, units, false);
			}
			else if (info.targetPosition == null)
			{
				this.Conjure_Impl(info.skillId, units, null, false);
			}
			else
			{
				this.Conjure_Impl(info.skillId, units, new Vector3?(MoveController.SVectgor3ToVector3(info.targetPosition)), false);
			}
		}
		else if (info.targetPosition != null && units != null)
		{
			units.transform.position = MoveController.SVectgor3ToVector3(info.targetPosition);
		}
	}
}

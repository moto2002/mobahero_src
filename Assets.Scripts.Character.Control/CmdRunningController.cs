using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace Assets.Scripts.Character.Control
{
	public class CmdRunningController : UnitComponent
	{
		private CmdRunningMove m_cmdRunningMove;

		private CmdRunningSkill m_cmdRunningSkill;

		private CmdSkill m_cmdTaskSkill;

		private CmdBase m_lastCmd;

		public CmdCacheController m_cmdCacheController;

		private Skill m_skill;

		private MoveController m_moveController;

		private List<VTrigger> listTrigger = new List<VTrigger>();

		private float m_idleTm;

		private bool m_AIStarted;

		private Task skillTask;

		public CmdRunningController()
		{
		}

		public CmdRunningController(Units inSelf) : base(inSelf)
		{
		}

		public CmdRunningSkill GetRunningSkillCmd()
		{
			if (this.m_cmdRunningSkill.skillState == CmdRunningSkill.CmdRunningSkillState.notRunning)
			{
				return null;
			}
			return this.m_cmdRunningSkill;
		}

		public override void OnInit()
		{
			this.m_moveController = this.self.moveController;
			this.m_cmdRunningMove = new CmdRunningMove();
			this.m_cmdRunningSkill = new CmdRunningSkill();
			VTrigger item = TriggerManager.CreateUnitEventTrigger(UnitEvent.UnitSkillCmdStart, null, new TriggerAction(this.OnSkillStart), this.self.unique_id);
			this.listTrigger.Add(item);
			item = TriggerManager.CreateUnitEventTrigger(UnitEvent.UnitSkillCmdEnd, null, new TriggerAction(this.OnSkillEnd), this.self.unique_id);
			this.listTrigger.Add(item);
			item = TriggerManager.CreateUnitEventTrigger(UnitEvent.UnitSkillCmdOver, null, new TriggerAction(this.OnSkillOver), this.self.unique_id);
			this.listTrigger.Add(item);
			item = TriggerManager.CreateUnitEventTrigger(UnitEvent.UnitSkillCmdFailed, null, new TriggerAction(this.OnSkillFailed), this.self.unique_id);
			this.listTrigger.Add(item);
			item = TriggerManager.CreateUnitEventTrigger(UnitEvent.UnitDeath, null, new TriggerAction(this.OnUnitDeath), this.self.unique_id);
			this.listTrigger.Add(item);
		}

		public override void OnExit()
		{
			foreach (VTrigger current in this.listTrigger)
			{
				TriggerManager.DestroyTrigger(current);
			}
			this.m_CoroutineManager.StopAllCoroutine();
		}

		public override void OnUpdate(float deltaTime)
		{
			this.m_idleTm += deltaTime;
			if (!this.m_AIStarted && this.m_idleTm > 0.1f && !this.m_cmdCacheController.HasCmd() && !this.m_cmdRunningMove.isMoving && (this.m_cmdRunningSkill.skillState == CmdRunningSkill.CmdRunningSkillState.end || this.m_cmdRunningSkill.skillState == CmdRunningSkill.CmdRunningSkillState.notRunning))
			{
				this.m_AIStarted = true;
				Units selectedTarget = PlayerControlMgr.Instance.GetSelectedTarget();
				if (selectedTarget != null)
				{
					if (Vector3.Distance(selectedTarget.mTransform.position, this.self.mTransform.position) >= this.self.attack_range + 1f)
					{
					}
				}
				if (this.m_lastCmd != null && this.m_lastCmd is CmdSkill)
				{
					if (this.self.aiManager != null)
					{
						this.self.aiManager.EnableSearchTarget(true);
					}
					this.self.SetAIAutoAttackMove(true);
				}
			}
		}

		public void RunCmd(CmdBase cmd)
		{
			if (cmd.isMoveCmd)
			{
				this.RunMoveCmd(cmd);
			}
			else
			{
				this.RunSkillCmd(cmd);
			}
			this.m_idleTm = 0f;
			this.m_AIStarted = false;
		}

		public bool CanAttack(CmdSkill cmdSkill)
		{
			if (this.m_cmdRunningSkill != null)
			{
				this.m_cmdRunningSkill.TryDesert();
			}
			if (!this.self.CanAttack)
			{
				if (this.m_cmdRunningMove.isMoving)
				{
					this.m_moveController.StopMoveForSkill();
					this.m_cmdRunningMove.Finish(false);
				}
				if (cmdSkill.targetUnits != null)
				{
					this.self.moveController.TurnToTarget(new Vector3?(cmdSkill.targetUnits.mTransform.position), false, false, 0f);
				}
			}
			if (this.m_cmdRunningSkill != null && this.m_cmdRunningSkill.skill != null && this.m_cmdRunningSkill.skill.IsSkill)
			{
				return (this.m_cmdRunningSkill.skillState != CmdRunningSkill.CmdRunningSkillState.ready && this.m_cmdRunningSkill.skillState != CmdRunningSkill.CmdRunningSkillState.start) || this.m_cmdRunningSkill.skill.data.CanBeInterruptInCastManual;
			}
			return (this.m_cmdRunningSkill.skillState != CmdRunningSkill.CmdRunningSkillState.ready && this.m_cmdRunningSkill.skillState != CmdRunningSkill.CmdRunningSkillState.start && this.m_cmdRunningSkill.skillState != CmdRunningSkill.CmdRunningSkillState.end) || this.m_cmdRunningSkill.skill.data.CanBeInterruptInCastManual || (this.m_cmdRunningSkill.skill.IsAttack && this.m_cmdRunningSkill.skillState == CmdRunningSkill.CmdRunningSkillState.ready);
		}

		public bool CanSkill(CmdSkill cmdSkill)
		{
			if (this.m_cmdRunningSkill != null)
			{
				this.m_cmdRunningSkill.TryDesert();
			}
			Skill skillOrAttackById = this.self.getSkillOrAttackById(cmdSkill.skillMainID);
			return (this.self.CanSkill || skillOrAttackById.CheckSkillCanUseSpecial) && ((this.m_cmdRunningSkill.skillState != CmdRunningSkill.CmdRunningSkillState.ready && this.m_cmdRunningSkill.skillState != CmdRunningSkill.CmdRunningSkillState.start) || this.m_cmdRunningSkill.skill.data.CanBeInterruptInCastManual);
		}

		public bool CanMove()
		{
			return this.self.CanMove;
		}

		private void RunMoveCmd(CmdBase cmd)
		{
			if (this.skillTask != null)
			{
				this.StopMoveAndSkillCoroutinue();
			}
			this.m_moveController.MoveToPoint(new Vector3?(cmd.targetPos), 0f, false);
			this.m_cmdRunningMove.SetMoving(cmd.targetPos);
			if (this.m_cmdRunningSkill.skillState == CmdRunningSkill.CmdRunningSkillState.end)
			{
				this.m_cmdRunningSkill.Finish(false);
			}
			this.m_lastCmd = cmd;
			this.self.playVoice("onUpdateMoveTarget");
		}

		private void RunSkillCmd(CmdBase cmd)
		{
			CmdSkill cmdSkill = cmd as CmdSkill;
			Skill skillOrAttackById = this.self.getSkillOrAttackById(cmdSkill.skillMainID);
			if (this.m_cmdRunningSkill != null && this.m_cmdRunningSkill.IsAlreadyRunning(cmdSkill))
			{
				return;
			}
			if (skillOrAttackById.needTarget)
			{
				if (cmdSkill.targetUnits == null || !cmdSkill.targetUnits.isLive)
				{
					return;
				}
				if (!cmdSkill.targetUnits.CanBeSelected)
				{
					return;
				}
			}
			if (this.skillTask != null && skillOrAttackById.needTarget)
			{
				if (cmdSkill == null || this.m_cmdTaskSkill == null)
				{
					return;
				}
				if (cmdSkill.targetUnits != null && !cmdSkill.targetUnits.CanBeSelected)
				{
					return;
				}
				if (cmdSkill.skillMainID == this.m_cmdTaskSkill.skillMainID && cmdSkill.targetUnits == this.m_cmdTaskSkill.targetUnits)
				{
					return;
				}
			}
			this.StopMoveAndSkillCoroutinue();
			if (this.SkillNeedMoveToFirst(cmdSkill, skillOrAttackById))
			{
				this.SetSkillTask(cmdSkill);
				this.m_cmdRunningMove.SetMoving(cmdSkill.targetPos);
				this.skillTask = this.m_CoroutineManager.StartCoroutine(this.MoveAndSkillCoroutinue(cmdSkill), true);
			}
			else
			{
				this.AssignSkillTargetPosition(cmdSkill, skillOrAttackById);
				this.LaunchSkill(cmdSkill);
			}
		}

		private void AssignSkillTargetPosition(CmdSkill skillCmd, Skill skill)
		{
			if (!skill.needTarget && skill.data.skillCastingType == 2 && skill.data.isMoveSkill)
			{
				float num = Vector3.Distance(this.self.mTransform.position, skillCmd.targetPos);
				if (num > skill.distance)
				{
					num = skill.distance;
				}
				Vector3 a = skillCmd.targetPos - this.self.mTransform.position;
				a.Normalize();
				skillCmd.targetPos = this.self.mTransform.position + a * num;
			}
		}

		private bool SkillNeedMoveToFirst(CmdSkill skillCmd, Skill skill)
		{
			float num = -1f;
			if (skill.needTarget)
			{
				Units targetUnits = skillCmd.targetUnits;
				num = Vector3.Distance(this.self.mTransform.position, targetUnits.mTransform.position);
			}
			else if (skill.data.skillCastingType == 2 && !skill.data.isMoveSkill)
			{
				Vector3 targetPos = skillCmd.targetPos;
				num = Vector3.Distance(this.self.mTransform.position, targetPos);
			}
			float num2;
			if (skill.IsAttack)
			{
				num2 = this.self.GetAttackRange(skillCmd.targetUnits);
			}
			else
			{
				num2 = skill.distance;
			}
			return num != -1f && num > num2;
		}

		private void LaunchSkill(CmdSkill skillCmd)
		{
			Skill skill = this.self.getSkillById(skillCmd.skillMainID);
			if (skill == null)
			{
				skill = this.self.getAttackById(skillCmd.skillMainID);
			}
			if (this.m_cmdRunningSkill.skill != null && !this.m_cmdRunningSkill.skill.data.CanBeInterruptInCastManual)
			{
				this.m_cmdRunningSkill.skill.InterruptEndTask();
			}
			this.m_cmdRunningSkill.SetSkillState(CmdRunningSkill.CmdRunningSkillState.ready);
			this.m_cmdRunningSkill.skill = skill;
			this.m_cmdRunningSkill.SetTargets(skillCmd.targetUnits, skillCmd.targetPos);
			if (this.m_cmdRunningMove.isMoving && !skill.data.isCanMoveInSkillCastin)
			{
				this.m_moveController.StopMoveForSkill();
				if (skill.data.continueMoveAfterSkillEnd)
				{
					this.m_cmdRunningMove.Finish(true);
				}
				else
				{
					this.m_cmdRunningMove.Finish(false);
				}
			}
			if (skill.IsAttack)
			{
				this.self.ComboAttack(skillCmd.targetUnits);
			}
			else
			{
				this.self.Conjure(skillCmd.skillMainID, skillCmd.targetUnits, new Vector3?(skillCmd.targetPos));
			}
			this.m_lastCmd = skillCmd;
		}

		private void StopMoveAndSkillCoroutinue()
		{
			if (this.skillTask != null)
			{
				this.m_CoroutineManager.StopCoroutine(this.skillTask);
				this.skillTask = null;
				this.m_cmdTaskSkill = null;
			}
		}

		[DebuggerHidden]
		private IEnumerator MoveAndSkillCoroutinue(CmdSkill skillCmd)
		{
			CmdRunningController.<MoveAndSkillCoroutinue>c__Iterator34 <MoveAndSkillCoroutinue>c__Iterator = new CmdRunningController.<MoveAndSkillCoroutinue>c__Iterator34();
			<MoveAndSkillCoroutinue>c__Iterator.skillCmd = skillCmd;
			<MoveAndSkillCoroutinue>c__Iterator.<$>skillCmd = skillCmd;
			<MoveAndSkillCoroutinue>c__Iterator.<>f__this = this;
			return <MoveAndSkillCoroutinue>c__Iterator;
		}

		private Vector3 GetTargetPos(Skill skill, CmdSkill skillCmd)
		{
			if (!skill.needTarget)
			{
				return skillCmd.targetPos;
			}
			if (skillCmd.targetUnits != null)
			{
				return skillCmd.targetUnits.mTransform.position;
			}
			return skillCmd.targetPos;
		}

		private void MoveToTarget(Skill skill, Vector3 targetPos, Units targetUnits, float dis)
		{
			if (skill.needTarget)
			{
				this.self.moveController.MoveToTarget(targetUnits, dis * 0.9f);
				if (targetUnits != null)
				{
					this.m_cmdRunningMove.SetMoving(targetUnits.mTransform.position);
				}
				else
				{
					this.m_cmdRunningMove.SetMoving(targetPos);
				}
			}
			else if (skill.data.skillCastingType == 2)
			{
				this.self.moveController.MoveToPoint(new Vector3?(targetPos), dis * 0.9f, false);
				this.m_cmdRunningMove.SetMoving(targetPos);
			}
		}

		public void Reset()
		{
			if (this.m_cmdRunningSkill != null)
			{
				this.m_cmdRunningSkill.Finish(false);
			}
			if (this.m_cmdRunningMove != null)
			{
				this.m_cmdRunningMove.Finish(false);
			}
			this.StopMoveAndSkillCoroutinue();
		}

		private void ContinueNormalAttack()
		{
			if (!this.m_cmdCacheController.HasCmd() && this.m_cmdRunningMove != null && !this.m_cmdRunningMove.isMoving)
			{
				Units selectedTarget = PlayerControlMgr.Instance.GetSelectedTarget();
				if (selectedTarget != null && selectedTarget != this.self && selectedTarget.CanBeSelected && selectedTarget.teamType != this.self.teamType)
				{
					this.m_cmdCacheController.EnqueueSkillCmd(this.self.attacks[0], this.m_cmdRunningSkill.targetPos, selectedTarget, false, true);
				}
				else
				{
					this.ClearSelectTarget();
				}
			}
		}

		private bool IsCurRunningSkill(Skill eventSkill)
		{
			return eventSkill != null && this.m_cmdRunningSkill != null && this.m_cmdRunningSkill.skill != null && (eventSkill == this.m_cmdRunningSkill.skill || (eventSkill.skillIndex == this.m_cmdRunningSkill.skill.skillIndex && eventSkill.IsAttack == this.m_cmdRunningSkill.skill.IsAttack) || (eventSkill.IsAttack && this.m_cmdRunningSkill.skill.IsAttack));
		}

		private void OnSkillStart()
		{
			if (!this.self.isPlayer)
			{
				return;
			}
			Skill skillContext = this.self.GetSkillContext();
			if (this.IsCurRunningSkill(skillContext))
			{
				this.m_cmdRunningSkill.SetSkillState(CmdRunningSkill.CmdRunningSkillState.start);
			}
		}

		private void OnSkillEnd()
		{
			if (!this.self.isPlayer)
			{
				return;
			}
			Skill skillContext = this.self.GetSkillContext();
			if (this.IsCurRunningSkill(skillContext))
			{
				this.m_cmdRunningSkill.SetSkillState(CmdRunningSkill.CmdRunningSkillState.end);
				if (!SkillUtility.IsBackHomeSkill(skillContext))
				{
					if (this.IsMovmentSkill() && (this.m_cmdCacheController == null || !this.m_cmdCacheController.HasCmd()) && !this.m_cmdRunningMove.isMoving && this.m_cmdRunningMove.isInterrupted)
					{
						this.m_cmdCacheController.EnqueueMoveCmd(this.m_cmdRunningMove.targetPos, false);
						return;
					}
					if (!this.m_cmdCacheController.HasCmd() && !this.m_cmdRunningMove.isMoving && this.m_cmdRunningSkill.targetUnits != null && this.m_cmdRunningSkill.targetUnits.CanBeSelected && this.m_cmdRunningSkill.targetUnits.teamType != this.self.teamType)
					{
						if (this.m_cmdRunningSkill.skill.IsAttack)
						{
							this.m_cmdCacheController.EnqueueSkillCmd(this.m_cmdRunningSkill.skill.skillMainId, this.m_cmdRunningSkill.targetPos, this.m_cmdRunningSkill.targetUnits, false, true);
						}
						else if (this.m_cmdRunningSkill.skill.IsSkill && this.m_cmdRunningSkill.skill.needTarget)
						{
							this.m_cmdCacheController.EnqueueSkillCmd(this.self.attacks[0], this.m_cmdRunningSkill.targetPos, this.m_cmdRunningSkill.targetUnits, false, true);
						}
						else
						{
							Units selectedTarget = PlayerControlMgr.Instance.GetSelectedTarget();
							if (selectedTarget != null && selectedTarget.isLive)
							{
								this.m_cmdCacheController.EnqueueSkillCmd(this.self.attacks[0], this.m_cmdRunningSkill.targetPos, selectedTarget, false, true);
							}
						}
					}
				}
				else
				{
					this.ClearSelectTarget();
				}
			}
		}

		private void OnSkillOver()
		{
			if (!this.self.isPlayer)
			{
				return;
			}
			Skill skillContext = this.self.GetSkillContext();
			if (this.IsCurRunningSkill(skillContext))
			{
				if (!SkillUtility.IsBackHomeSkill(skillContext))
				{
					this.ContinueNormalAttack();
				}
				else
				{
					this.ClearSelectTarget();
				}
				this.m_cmdRunningSkill.Finish(false);
			}
			this.m_idleTm = 0f;
		}

		private bool IsMovmentSkill()
		{
			return false;
		}

		private bool IsTargetOutOfRange(Units target)
		{
			return target == null || Vector3.Distance(target.mTransform.position, this.self.mTransform.position) >= this.self.attack_range + 2f;
		}

		private void OnSkillFailed()
		{
			if (!this.self.isPlayer)
			{
				return;
			}
			Skill skillContext = this.self.GetSkillContext();
			if (this.IsCurRunningSkill(skillContext))
			{
				this.ContinueNormalAttack();
				this.m_cmdRunningSkill.Finish(false);
			}
			this.m_idleTm = 0f;
		}

		private void OnUnitDeath()
		{
			if (!this.self.isPlayer)
			{
				return;
			}
			this.Reset();
		}

		public void OnMoveEnd()
		{
			if (!this.self.isPlayer)
			{
				return;
			}
			this.m_cmdRunningMove.Finish(false);
			ManualController unitComponent = this.self.GetUnitComponent<ManualController>();
			if (unitComponent != null)
			{
				unitComponent.TargetCom.ClearMoveFlag();
			}
			if (this.skillTask != null)
			{
				CmdSkill cmdTaskSkill = this.m_cmdTaskSkill;
				this.StopMoveAndSkillCoroutinue();
				if (this.m_cmdCacheController != null && !this.m_cmdCacheController.HasCmd())
				{
					if (cmdTaskSkill != null && cmdTaskSkill.targetUnits != null && cmdTaskSkill.targetUnits.CanBeSelected)
					{
						this.m_cmdCacheController.EnqueueSkillCmd(cmdTaskSkill.skillMainID, cmdTaskSkill.targetPos, cmdTaskSkill.targetUnits, false, true);
					}
					else
					{
						this.ClearAttackTarget();
					}
				}
			}
			else
			{
				this.ClearAttackTarget();
			}
			this.m_idleTm = 0f;
		}

		private void ClearSelectTarget()
		{
			this.self.SetSelectTarget(null);
			this.self.SetAttackTarget(null);
			PlayerControlMgr.Instance.SetSelectedTarget(null);
		}

		private void ClearAttackTarget()
		{
			this.self.SetAttackTarget(null);
		}

		private void SetSkillTask(CmdSkill skillCmd)
		{
			this.m_cmdTaskSkill = new CmdSkill();
			this.m_cmdTaskSkill.skillMainID = skillCmd.skillMainID;
			this.m_cmdTaskSkill.targetPos = skillCmd.targetPos;
			this.m_cmdTaskSkill.targetUnits = skillCmd.targetUnits;
			this.m_cmdTaskSkill.skill = skillCmd.skill;
			this.m_cmdTaskSkill.isMoveCmd = skillCmd.isMoveCmd;
			this.m_cmdTaskSkill.isSkillCmd = skillCmd.isSkillCmd;
		}
	}
}

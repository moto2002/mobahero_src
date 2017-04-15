using Com.Game.Module;
using MobaHeros.Pvp;
using MobaProtocol.Data;
using Newbie;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Character.Control
{
	public class CmdCacheController : UnitComponent
	{
		private CmdBase m_cmdReady;

		private CmdMove m_preloadCmdMove;

		private CmdSkill m_preloadCmdSkill;

		private CmdRunningController m_cmdRunningController;

		private List<VTrigger> listTrigger = new List<VTrigger>();

		private Vector3 lastSendPos = Vector3.zero;

		private float lastSendTime;

		public CmdCacheController()
		{
		}

		public CmdCacheController(Units inSelf) : base(inSelf)
		{
		}

		public override void OnInit()
		{
			this.m_preloadCmdMove = new CmdMove();
			this.m_preloadCmdSkill = new CmdSkill();
			this.m_cmdRunningController = this.self.mCmdRunningController;
			VTrigger item = TriggerManager.CreateUnitEventTrigger(UnitEvent.UnitDeath, null, new TriggerAction(this.OnUnitDeath), this.self.unique_id);
			this.listTrigger.Add(item);
		}

		public override void OnExit()
		{
			foreach (VTrigger current in this.listTrigger)
			{
				TriggerManager.DestroyTrigger(current);
			}
		}

		public void EnqueueMoveCmd(Vector3 targetPos, bool forceSend = false)
		{
			if (Singleton<PvpManager>.Instance.IsInPvp)
			{
				float miniDis = (!forceSend) ? 0.2f : 0.1f;
				if (this.NeedSendMsg(targetPos, miniDis, forceSend))
				{
					this.SendMoveMsg(targetPos);
					this.self.playVoice("onUpdateMoveTarget");
				}
			}
			else
			{
				this.m_cmdReady = new CmdMove();
				this.m_cmdReady.targetPos = targetPos;
				this.doCacheCmd();
			}
		}

		public void EnqueueSkillCmd(string skillID, Vector3 targetPos, Units targetUnits, bool exeImmediate = true, bool isCrazyMode = true)
		{
			if (!this.DoExtraActionFirst(skillID))
			{
				return;
			}
			Skill skillOrAttackById = this.self.getSkillOrAttackById(skillID);
			if (skillOrAttackById.needTarget)
			{
				if ((targetUnits == null || !targetUnits.isLive) && skillOrAttackById.IsAttack)
				{
					return;
				}
				if (targetUnits == null || !targetUnits.isLive)
				{
					UIMessageBox.ShowMessage("未选择目标，不能释放！", 1.5f, 0);
					return;
				}
				this.self.SetSelectTarget(targetUnits);
				PlayerControlMgr.Instance.SetSelectedTarget(targetUnits);
			}
			if (Singleton<PvpManager>.Instance.IsInPvp)
			{
				this.SendUseSkill(skillID, targetUnits, targetPos, isCrazyMode);
				NewbieManager.Instance.TryHandleNotifyServerUseSkill();
			}
			else
			{
				CmdSkill cmdSkill = new CmdSkill();
				this.m_cmdReady = cmdSkill;
				cmdSkill.skillMainID = skillID;
				cmdSkill.targetPos = targetPos;
				cmdSkill.targetUnits = targetUnits;
				cmdSkill.skill = skillOrAttackById;
				if (exeImmediate)
				{
					this.doCacheCmd();
				}
			}
		}

		public bool HasCmd()
		{
			return this.m_cmdReady != null;
		}

		private void doCacheCmd()
		{
			if (this.m_cmdReady != null)
			{
				if (this.m_cmdReady.isMoveCmd)
				{
					if (this.CanMove())
					{
						CmdBase cmdReady = this.m_cmdReady;
						this.m_cmdReady = null;
						this.doCmd(cmdReady);
					}
				}
				else if (this.CanSkill(this.m_cmdReady))
				{
					CmdBase cmdReady2 = this.m_cmdReady;
					this.m_cmdReady = null;
					this.doCmd(cmdReady2);
				}
			}
		}

		private bool CanMove()
		{
			return this.m_cmdRunningController.CanMove();
		}

		private bool CanSkill(CmdBase cmd)
		{
			CmdSkill cmdSkill = cmd as CmdSkill;
			if (cmdSkill.skill.IsSkill)
			{
				if (this.m_cmdRunningController.CanSkill(cmdSkill))
				{
					return true;
				}
			}
			else if (this.m_cmdRunningController.CanAttack(cmdSkill))
			{
				return true;
			}
			return false;
		}

		private void doCmd(CmdBase cmd)
		{
			this.m_cmdRunningController.RunCmd(cmd);
		}

		public override void OnUpdate(float deltaTime)
		{
			if (this.self.isPlayer)
			{
				Units selectedTarget = PlayerControlMgr.Instance.GetSelectedTarget();
				if (selectedTarget != null)
				{
					float num = Vector3.Distance(this.self.mTransform.position, selectedTarget.mTransform.position);
					if (num > 20f)
					{
						PlayerControlMgr.Instance.SetSelectedTarget(null);
					}
				}
			}
			this.doCacheCmd();
		}

		private void OnUnitDeath()
		{
			this.m_cmdReady = null;
		}

		private bool DoExtraActionFirst(string skillID)
		{
			if (this.self.IsSkillCanTriggerBornPowerObj(skillID))
			{
				if (Singleton<PvpManager>.Instance.IsInPvp)
				{
					PvpEvent.SendDoSkillEvent(this.self.unique_id, skillID);
				}
				else if (this.self.skillManager != null)
				{
					this.self.skillManager.TriggerBornPowerObj(skillID, true);
				}
				return false;
			}
			return true;
		}

		protected void SendUseSkill(string skillID, Units target, Vector3 targetPos, bool isCrazyMode = true)
		{
			UseSkillInfo info = new UseSkillInfo
			{
				unitId = this.self.unique_id,
				skillId = skillID,
				targetUnit = (!(target != null)) ? 0 : target.unique_id,
				targetPosition = SVector3.Build(targetPos.x, targetPos.y, targetPos.z),
				targetRotate = this.self.transform.eulerAngles.y,
				controlMode = (!isCrazyMode) ? 1 : 0
			};
			PvpEvent.SendUseSkill(info);
		}

		protected void SendMoveMsg(Vector3 targetPos)
		{
			this.self.moveController.SendMoveToPos(targetPos, -1f);
		}

		protected bool NeedSendMsg(Vector3 targetPos, float miniDis, bool forceSend)
		{
			float num = Vector3.Distance(targetPos, this.lastSendPos);
			if (num < miniDis)
			{
				return false;
			}
			float num2 = Time.realtimeSinceStartup - this.lastSendTime;
			if (num2 < 0.2f && !forceSend)
			{
				return false;
			}
			this.lastSendTime = Time.realtimeSinceStartup;
			this.lastSendPos = targetPos;
			return true;
		}
	}
}

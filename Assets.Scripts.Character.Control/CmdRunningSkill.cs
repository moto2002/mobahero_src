using System;
using UnityEngine;

namespace Assets.Scripts.Character.Control
{
	public class CmdRunningSkill : CmdRunningBase
	{
		public enum CmdRunningSkillState
		{
			notRunning,
			ready,
			start,
			end
		}

		public Vector3 targetPos;

		public Units targetUnits;

		public Skill skill;

		private float beginTime;

		public CmdRunningSkill.CmdRunningSkillState skillState;

		public void SetSkillState(CmdRunningSkill.CmdRunningSkillState state)
		{
			this.skillState = state;
			this.isInterrupted = false;
			this.beginTime = Time.time;
		}

		public void TryDesert()
		{
			float num = Time.time - this.beginTime;
			if (this.skillState != CmdRunningSkill.CmdRunningSkillState.notRunning && num > 16f)
			{
				this.SetSkillState(CmdRunningSkill.CmdRunningSkillState.notRunning);
			}
		}

		public void SetTargets(Units inTargetUnits, Vector3 inTargetPos)
		{
			this.targetUnits = inTargetUnits;
			this.targetPos = inTargetPos;
		}

		public override void Finish(bool inIsInterrupted = false)
		{
			base.Finish(inIsInterrupted);
			this.skillState = CmdRunningSkill.CmdRunningSkillState.notRunning;
		}

		public bool ContinueMoveAfterSkillEnd()
		{
			return this.skill.data.continueMoveAfterSkillEnd;
		}

		public bool IsAlreadyRunning(CmdSkill skillCmd)
		{
			return this.skill != null && (this.skillState == CmdRunningSkill.CmdRunningSkillState.ready && this.skill.data.skillId == skillCmd.skillMainID);
		}
	}
}

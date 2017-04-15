using Assets.MobaTools.TriggerPlugin.Scripts;
using System;

namespace Assets.Scripts.Character.Control
{
	public class ManualControllerCom
	{
		protected Units self;

		private bool pause = true;

		public Units lastTarget;

		public virtual void OnInit()
		{
		}

		public virtual void OnStart()
		{
		}

		public virtual void OnExit()
		{
		}

		public virtual void OnStop()
		{
		}

		public virtual void OnDown(TriggerParamTouch param)
		{
		}

		public virtual void OnPress(TriggerParamTouch param)
		{
		}

		public virtual void OnUp(TriggerParamTouch param)
		{
		}

		public virtual void OnMoveEnd(TriggerParamTouch param)
		{
		}

		public virtual void OnSkill(ITriggerDoActionParam param)
		{
		}

		public virtual void OnNavigateEnd()
		{
		}

		protected float GetSkillRange(Skill skill)
		{
			if (skill.IsAttack)
			{
				return this.self.GetAttackRange(null);
			}
			return skill.distance;
		}

		protected Units GetSkillTarget(Skill skill, Units enemyTarget, bool isCrazyMode = true)
		{
			if (skill.needTarget)
			{
				if (enemyTarget == null)
				{
					if (skill.data.targetCamp == SkillTargetCamp.Self)
					{
						return this.self;
					}
					if (skill.data.targetCamp == SkillTargetCamp.Partener)
					{
						if (!isCrazyMode)
						{
							return null;
						}
						return this.self;
					}
					else if (skill.data.targetCamp == SkillTargetCamp.AllWhitOutPartener)
					{
						if (!isCrazyMode)
						{
							return null;
						}
						return this.self;
					}
				}
				else
				{
					if (skill.data.targetCamp == SkillTargetCamp.Self)
					{
						return this.self;
					}
					if (skill.data.targetCamp == SkillTargetCamp.Partener)
					{
						if (enemyTarget.teamType == this.self.teamType)
						{
							return enemyTarget;
						}
						return this.self;
					}
					else if (skill.data.targetCamp == SkillTargetCamp.AllWhitOutPartener)
					{
						if (enemyTarget.teamType == this.self.teamType)
						{
							return this.self;
						}
						return enemyTarget;
					}
				}
				return enemyTarget;
			}
			return null;
		}
	}
}

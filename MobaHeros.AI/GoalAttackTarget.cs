using System;

namespace MobaHeros.AI
{
	public class GoalAttackTarget : GoalComposite<Units>
	{
		public GoalAttackTarget(Units pOwner) : base(pOwner, 13)
		{
		}

		public override void Activate()
		{
		}

		public override int Process()
		{
			return 0;
		}

		public override void Terminate()
		{
			this.m_iStatus = GoalState.completed;
		}
	}
}

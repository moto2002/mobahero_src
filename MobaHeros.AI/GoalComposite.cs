using System;
using System.Collections.Generic;
using UnityEngine;

namespace MobaHeros.AI
{
	public class GoalComposite<T> : GoalBase<T> where T : class
	{
		private List<GoalBase<T>> m_SubGoals;

		public GoalComposite(T pE, int type) : base(pE, type)
		{
		}

		private int ProcessSubgoals()
		{
			return 0;
		}

		public override void Destroy()
		{
			this.RemoveAllSubgoals();
		}

		private void RemoveAllSubgoals()
		{
			this.m_SubGoals.Clear();
			this.m_SubGoals = null;
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
		}

		public override void AddSubgoal(GoalBase<T> g)
		{
		}

		public override void RenderAtPos(Vector3 pos, string tts)
		{
		}

		public override void Render()
		{
		}
	}
}

using System;
using UnityEngine;

namespace MobaHeros.AI
{
	public class GoalBase<T> : IGoal<T> where T : class
	{
		protected int m_iType;

		protected T m_pOwner;

		protected GoalState m_iStatus;

		public GoalBase(T pE, int type)
		{
			this.m_pOwner = pE;
			this.m_iType = type;
		}

		private void ActivateIfInactive()
		{
		}

		private void ReactivateIfFailed()
		{
		}

		public virtual void Destroy()
		{
		}

		public virtual void Activate()
		{
		}

		public virtual int Process()
		{
			return 0;
		}

		public virtual void Terminate()
		{
		}

		public virtual void AddSubgoal(GoalBase<T> g)
		{
			throw new Exception("Cannot add goals to atomic goals");
		}

		private bool isComplete()
		{
			return this.m_iStatus == GoalState.completed;
		}

		private bool isActive()
		{
			return this.m_iStatus == GoalState.active;
		}

		private bool isInactive()
		{
			return this.m_iStatus == GoalState.inactive;
		}

		private bool hasFailed()
		{
			return this.m_iStatus == GoalState.failed;
		}

		private new int GetType()
		{
			return this.m_iType;
		}

		public virtual void RenderAtPos(Vector3 pos, string tts)
		{
		}

		public virtual void Render()
		{
		}
	}
}

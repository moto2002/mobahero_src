using System;
using UnityEngine;

namespace MobaHeros.AI
{
	public class GoalSeekToPosition : GoalBase<Units>
	{
		private Vector3 m_vPosition;

		private double m_dTimeToReachPos;

		private double m_dStartTime;

		public GoalSeekToPosition(Units pBot, Vector3 target) : base(pBot, 3)
		{
		}

		public bool isStuck()
		{
			return false;
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

		public override void Render()
		{
		}
	}
}

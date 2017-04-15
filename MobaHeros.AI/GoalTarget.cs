using System;
using UnityEngine;

namespace MobaHeros.AI
{
	public class GoalTarget
	{
		public InputTargetType m_TargetType;

		public Units m_Unit;

		public Transform transform;

		public Transform m_Transform
		{
			get
			{
				if (this.transform == null && this.m_Unit != null)
				{
					this.transform = this.m_Unit.mTransform;
				}
				return this.transform;
			}
		}

		public int m_GoalId
		{
			get
			{
				if (this.m_Unit != null)
				{
					return this.m_Unit.unique_id;
				}
				return -1;
			}
		}

		public string m_GoalNpcId
		{
			get
			{
				if (this.m_Unit != null)
				{
					return this.m_Unit.npc_id;
				}
				return null;
			}
		}

		public void Set(InputTargetType targetType, Units target)
		{
			this.m_Unit = target;
			this.m_TargetType = targetType;
		}

		public void Set(InputTargetType targetType, Transform target)
		{
			this.transform = target;
			this.m_TargetType = targetType;
		}
	}
}

using System;
using System.Collections.Generic;
using UnityEngine;

namespace MobaTools.Move
{
	public class Queue
	{
		private bool DrawGizmos;

		private List<Vector3> m_QueueOffsets = new List<Vector3>();

		private Transform m_Center;

		private int m_AgentCount;

		private int nodeIndex;

		private int circleIndex = 1;

		public Quaternion rotation
		{
			get
			{
				return this.m_Center.rotation;
			}
		}

		public Vector3 position
		{
			get
			{
				return this.m_Center.position;
			}
		}

		public Queue(int queue_id, Transform target, QueueType queueType, int agentCount = 3)
		{
			this.m_Center = target;
			this.m_AgentCount = agentCount;
			this.UpdatePosition();
		}

		public void SetTargetCount(int count)
		{
			this.m_AgentCount = count;
			this.UpdatePosition();
		}

		public void AddAgent()
		{
			this.m_AgentCount++;
			this.UpdatePosition();
		}

		public void RemoveAgent()
		{
			this.m_AgentCount--;
			this.UpdatePosition();
		}

		private void UpdatePosition()
		{
			this.m_QueueOffsets.Clear();
			if (this.m_AgentCount == 1)
			{
				this.m_QueueOffsets.Add(this.m_Center.position);
			}
			else if (this.m_AgentCount == 2)
			{
				this.m_QueueOffsets.Add(this.m_Center.rotation * new Vector3(-1f, 0f, 0f));
				this.m_QueueOffsets.Add(this.m_Center.rotation * new Vector3(1f, 0f, 0f));
			}
			else if (this.m_AgentCount == 3)
			{
				this.m_QueueOffsets.Add(this.m_Center.rotation * new Vector3(0f, 0f, 1f));
				this.m_QueueOffsets.Add(this.m_Center.rotation * new Vector3(-1f, 0f, 0f));
				this.m_QueueOffsets.Add(this.m_Center.rotation * new Vector3(1f, 0f, 0f));
			}
			else
			{
				this.m_QueueOffsets.Add(this.m_Center.rotation * new Vector3(0f, 0f, 1f));
				this.m_QueueOffsets.Add(this.m_Center.rotation * new Vector3(1f, 0f, 1f));
				this.m_QueueOffsets.Add(this.m_Center.rotation * new Vector3(1f, 0f, 0f));
				this.m_QueueOffsets.Add(this.m_Center.rotation * new Vector3(1f, 0f, -1f));
				this.m_QueueOffsets.Add(this.m_Center.rotation * new Vector3(-1f, 0f, 0f));
				this.m_QueueOffsets.Add(this.m_Center.rotation * new Vector3(-1f, 0f, -1f));
				this.m_QueueOffsets.Add(this.m_Center.rotation * new Vector3(0f, 0f, -1f));
				this.m_QueueOffsets.Add(this.m_Center.rotation * new Vector3(-1f, 0f, 1f));
			}
		}

		public Vector3 GetRandomPosition(Vector3 randomRadius, Vector3 randomAllowedAxes)
		{
			Vector3 vector = Vector3.Scale(UnityEngine.Random.insideUnitSphere, randomRadius);
			vector = Vector3.Scale(vector, randomAllowedAxes);
			return this.m_Center.position + vector;
		}

		public Vector3 GetNextPosition()
		{
			if (this.nodeIndex >= this.m_AgentCount)
			{
				this.nodeIndex = 0;
				this.circleIndex++;
			}
			Vector3 result = this.m_Center.position + this.m_QueueOffsets[this.nodeIndex] * (float)this.circleIndex;
			this.nodeIndex++;
			return result;
		}

		public void ResetPosition()
		{
			this.nodeIndex = 0;
			this.circleIndex = 1;
		}

		public void OnDrawGizmos()
		{
			if (!this.DrawGizmos)
			{
				return;
			}
			if (this.m_QueueOffsets == null)
			{
				return;
			}
			for (int i = 0; i < this.m_QueueOffsets.Count; i++)
			{
				Gizmos.color = Color.red;
				Gizmos.DrawSphere(this.m_Center.position + this.m_QueueOffsets[i], 0.2f);
			}
		}
	}
}

using System;
using System.Collections.Generic;
using UnityEngine;

namespace MobaTools.Move
{
	public class QueueManager
	{
		public static Dictionary<int, Queue> m_Queues = new Dictionary<int, Queue>();

		private static int queueId = 0;

		public static Queue CreateQueue(Transform target, QueueType queueType)
		{
			int num = QueueManager.assingQueueId();
			Queue queue = new Queue(num, target, queueType, 3);
			QueueManager.m_Queues.Add(num, queue);
			return queue;
		}

		public static int assingQueueId()
		{
			return QueueManager.queueId++;
		}

		public static Queue GetQueue(int queue_id)
		{
			if (QueueManager.m_Queues.ContainsKey(queue_id))
			{
				return QueueManager.m_Queues[queue_id];
			}
			return null;
		}

		public static void RemoveQueue(int queue_id)
		{
			if (QueueManager.m_Queues.ContainsKey(queue_id))
			{
				QueueManager.m_Queues.Remove(queue_id);
			}
		}

		public static void ClearQueues()
		{
			QueueManager.m_Queues.Clear();
		}
	}
}

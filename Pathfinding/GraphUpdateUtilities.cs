using Pathfinding.Util;
using System;
using System.Collections.Generic;

namespace Pathfinding
{
	public static class GraphUpdateUtilities
	{
		[Obsolete("This function has been moved to Pathfinding.Util.PathUtilities. Please use the version in that class")]
		public static bool IsPathPossible(GraphNode n1, GraphNode n2)
		{
			return n1.Walkable && n2.Walkable && n1.Area == n2.Area;
		}

		[Obsolete("This function has been moved to Pathfinding.Util.PathUtilities. Please use the version in that class")]
		public static bool IsPathPossible(List<GraphNode> nodes)
		{
			uint area = nodes[0].Area;
			for (int i = 0; i < nodes.Count; i++)
			{
				if (!nodes[i].Walkable || nodes[i].Area != area)
				{
					return false;
				}
			}
			return true;
		}

		public static bool UpdateGraphsNoBlock(GraphUpdateObject guo, GraphNode node1, GraphNode node2, bool alwaysRevert = false)
		{
			List<GraphNode> list = ListPool<GraphNode>.Claim();
			list.Add(node1);
			list.Add(node2);
			bool result = GraphUpdateUtilities.UpdateGraphsNoBlock(guo, list, alwaysRevert);
			ListPool<GraphNode>.Release(list);
			return result;
		}

		public static bool UpdateGraphsNoBlock(GraphUpdateObject guo, List<GraphNode> nodes, bool alwaysRevert = false)
		{
			for (int i = 0; i < nodes.Count; i++)
			{
				if (!nodes[i].Walkable)
				{
					return false;
				}
			}
			guo.trackChangedNodes = true;
			bool worked = true;
			AstarPath.RegisterSafeUpdate(delegate
			{
				AstarPath.active.UpdateGraphs(guo);
				AstarPath.active.QueueGraphUpdates();
				AstarPath.active.FlushGraphUpdates();
				worked = (worked && PathUtilities.IsPathPossible(nodes));
				if (!worked || alwaysRevert)
				{
					guo.RevertFromBackup();
					AstarPath.active.FloodFill();
				}
			});
			AstarPath.active.FlushThreadSafeCallbacks();
			guo.trackChangedNodes = false;
			return worked;
		}
	}
}

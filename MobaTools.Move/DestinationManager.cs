using Pathfinding;
using System;
using UnityEngine;

namespace MobaTools.Move
{
	public class DestinationManager
	{
		public static Vector3 GetNearestWalkableNode(Vector3 point)
		{
			Vector3 result = point;
			int num = AstarPath.active.graphs.Length;
			for (int i = 0; i < num; i++)
			{
				GridGraph gridGraph = AstarPath.active.graphs[i] as GridGraph;
				if (gridGraph != null)
				{
					GraphNode node = gridGraph.GetNearest(point).node;
					if (node != null)
					{
						result = (Vector3)node.position;
						break;
					}
				}
			}
			return result;
		}

		public static bool IsPointWalkable(Units owner, Vector3 point)
		{
			int num = AstarPath.active.graphs.Length;
			for (int i = 0; i < num; i++)
			{
				GridGraph gridGraph = AstarPath.active.graphs[i] as GridGraph;
				if (gridGraph != null)
				{
					GraphNode node = gridGraph.GetNodeByPosition(point).node;
					if (node != null)
					{
						int tagsChange = owner.GetTagsChange();
						return node.Walkable && (tagsChange >> (int)node.Tag & 1) != 0;
					}
				}
			}
			return false;
		}
	}
}

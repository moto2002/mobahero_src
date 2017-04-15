using Pathfinding.Util;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Pathfinding
{
	public class GraphUpdateObject
	{
		public Bounds bounds;

		public bool requiresFloodFill = true;

		public bool updatePhysics = true;

		public bool resetPenaltyOnPhysics = true;

		public bool updateErosion = true;

		public NNConstraint nnConstraint = NNConstraint.None;

		public int addPenalty;

		public bool modifyWalkability;

		public bool setWalkability;

		public bool modifyTag;

		public int setTag;

		public bool trackChangedNodes;

		public List<GraphNode> changedNodes;

		private List<uint> backupData;

		private List<Int3> backupPositionData;

		public GraphUpdateShape shape;

		public static Dictionary<int, int> nodeTagCount = new Dictionary<int, int>();

		public GraphUpdateObject()
		{
		}

		public GraphUpdateObject(Bounds b)
		{
			this.bounds = b;
		}

		public virtual void WillUpdateNode(GraphNode node)
		{
			if (this.trackChangedNodes && node != null)
			{
				if (this.changedNodes == null)
				{
					this.changedNodes = ListPool<GraphNode>.Claim();
					this.backupData = ListPool<uint>.Claim();
					this.backupPositionData = ListPool<Int3>.Claim();
				}
				this.changedNodes.Add(node);
				this.backupPositionData.Add(node.position);
				this.backupData.Add(node.Penalty);
				this.backupData.Add(node.Flags);
				GridNode gridNode = node as GridNode;
				if (gridNode != null)
				{
					this.backupData.Add((uint)gridNode.InternalGridFlags);
				}
			}
		}

		public virtual void RevertFromBackup()
		{
			if (!this.trackChangedNodes)
			{
				throw new InvalidOperationException("Changed nodes have not been tracked, cannot revert from backup");
			}
			if (this.changedNodes == null)
			{
				return;
			}
			int num = 0;
			for (int i = 0; i < this.changedNodes.Count; i++)
			{
				this.changedNodes[i].Penalty = this.backupData[num];
				num++;
				this.changedNodes[i].Flags = this.backupData[num];
				num++;
				GridNode gridNode = this.changedNodes[i] as GridNode;
				if (gridNode != null)
				{
					gridNode.InternalGridFlags = (ushort)this.backupData[num];
					num++;
				}
				this.changedNodes[i].position = this.backupPositionData[i];
			}
			ListPool<GraphNode>.Release(this.changedNodes);
			ListPool<uint>.Release(this.backupData);
			ListPool<Int3>.Release(this.backupPositionData);
		}

		public virtual void Apply(GraphNode node)
		{
			if (this.shape == null || this.shape.Contains(node))
			{
				node.Penalty = (uint)((ulong)node.Penalty + (ulong)((long)this.addPenalty));
				if (node.Tag == 15u)
				{
					return;
				}
				if (this.modifyWalkability)
				{
					if (this.setWalkability)
					{
						int num = 0;
						if (node.Tag != 0u && GraphUpdateObject.nodeTagCount.ContainsKey(node.NodeIndex))
						{
							Dictionary<int, int> dictionary;
							Dictionary<int, int> expr_7D = dictionary = GraphUpdateObject.nodeTagCount;
							int num2;
							int expr_85 = num2 = node.NodeIndex;
							num2 = dictionary[num2];
							expr_7D[expr_85] = num2 - 1;
							num = GraphUpdateObject.nodeTagCount[node.NodeIndex];
						}
						if (num <= 0)
						{
							node.Walkable = true;
							node.Tag = 0u;
						}
					}
					else
					{
						if (GraphUpdateObject.nodeTagCount.ContainsKey(node.NodeIndex))
						{
							Dictionary<int, int> dictionary2;
							Dictionary<int, int> expr_DC = dictionary2 = GraphUpdateObject.nodeTagCount;
							int num2;
							int expr_E4 = num2 = node.NodeIndex;
							num2 = dictionary2[num2];
							expr_DC[expr_E4] = num2 + 1;
						}
						else
						{
							GraphUpdateObject.nodeTagCount.Add(node.NodeIndex, 1);
						}
						node.Walkable = false;
						if (this.setTag > (int)node.Tag)
						{
							node.Tag = (uint)this.setTag;
						}
					}
				}
			}
		}
	}
}

using Pathfinding.Util;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Pathfinding
{
	public class MultiTargetPath : ABPath
	{
		public enum HeuristicMode
		{
			None,
			Average,
			MovingAverage,
			Midpoint,
			MovingMidpoint,
			Sequential
		}

		public OnPathDelegate[] callbacks;

		public GraphNode[] targetNodes;

		protected int targetNodeCount;

		public bool[] targetsFound;

		public Vector3[] targetPoints;

		public Vector3[] originalTargetPoints;

		public List<Vector3>[] vectorPaths;

		public List<GraphNode>[] nodePaths;

		public int endsFound;

		public bool pathsForAll = true;

		public int chosenTarget = -1;

		public int sequentialTarget;

		public MultiTargetPath.HeuristicMode heuristicMode = MultiTargetPath.HeuristicMode.Sequential;

		public bool inverted = true;

		public MultiTargetPath()
		{
		}

		[Obsolete("Please use the Construct method instead")]
		public MultiTargetPath(Vector3[] startPoints, Vector3 target, OnPathDelegate[] callbackDelegates, OnPathDelegate callbackDelegate = null) : this(target, startPoints, callbackDelegates, callbackDelegate)
		{
			this.inverted = true;
		}

		[Obsolete("Please use the Construct method instead")]
		public MultiTargetPath(Vector3 start, Vector3[] targets, OnPathDelegate[] callbackDelegates, OnPathDelegate callbackDelegate = null)
		{
		}

		public static MultiTargetPath Construct(Vector3[] startPoints, Vector3 target, OnPathDelegate[] callbackDelegates, OnPathDelegate callback = null)
		{
			MultiTargetPath multiTargetPath = MultiTargetPath.Construct(target, startPoints, callbackDelegates, callback);
			multiTargetPath.inverted = true;
			return multiTargetPath;
		}

		public static MultiTargetPath Construct(Vector3 start, Vector3[] targets, OnPathDelegate[] callbackDelegates, OnPathDelegate callback = null)
		{
			MultiTargetPath path = PathPool<MultiTargetPath>.GetPath();
			path.Setup(start, targets, callbackDelegates, callback);
			return path;
		}

		protected void Setup(Vector3 start, Vector3[] targets, OnPathDelegate[] callbackDelegates, OnPathDelegate callback)
		{
			this.inverted = false;
			this.callback = callback;
			this.callbacks = callbackDelegates;
			this.targetPoints = targets;
			this.originalStartPoint = start;
			this.startPoint = start;
			this.startIntPoint = (Int3)start;
			if (targets.Length == 0)
			{
				base.Error();
				base.LogError("No targets were assigned to the MultiTargetPath");
				return;
			}
			this.endPoint = targets[0];
			this.originalTargetPoints = new Vector3[this.targetPoints.Length];
			for (int i = 0; i < this.targetPoints.Length; i++)
			{
				this.originalTargetPoints[i] = this.targetPoints[i];
			}
		}

		protected override void Recycle()
		{
			PathPool<MultiTargetPath>.Recycle(this);
		}

		public override void OnEnterPool()
		{
			if (this.vectorPaths != null)
			{
				for (int i = 0; i < this.vectorPaths.Length; i++)
				{
					if (this.vectorPaths[i] != null)
					{
						ListPool<Vector3>.Release(this.vectorPaths[i]);
					}
				}
			}
			this.vectorPaths = null;
			this.vectorPath = null;
			if (this.nodePaths != null)
			{
				for (int j = 0; j < this.nodePaths.Length; j++)
				{
					if (this.nodePaths[j] != null)
					{
						ListPool<GraphNode>.Release(this.nodePaths[j]);
					}
				}
			}
			this.nodePaths = null;
			this.path = null;
			base.OnEnterPool();
		}

		public override void ReturnPath()
		{
			if (base.error)
			{
				if (this.callbacks != null)
				{
					for (int i = 0; i < this.callbacks.Length; i++)
					{
						if (this.callbacks[i] != null)
						{
							this.callbacks[i](this);
						}
					}
				}
				if (this.callback != null)
				{
					this.callback(this);
				}
				return;
			}
			bool flag = false;
			Vector3 originalStartPoint = this.originalStartPoint;
			Vector3 startPoint = this.startPoint;
			GraphNode startNode = this.startNode;
			for (int j = 0; j < this.nodePaths.Length; j++)
			{
				this.path = this.nodePaths[j];
				if (this.path != null)
				{
					base.CompleteState = PathCompleteState.Complete;
					flag = true;
				}
				else
				{
					base.CompleteState = PathCompleteState.Error;
				}
				if (this.callbacks != null && this.callbacks[j] != null)
				{
					this.vectorPath = this.vectorPaths[j];
					if (this.inverted)
					{
						this.endPoint = startPoint;
						this.endNode = startNode;
						this.startNode = this.targetNodes[j];
						this.startPoint = this.targetPoints[j];
						this.originalEndPoint = originalStartPoint;
						this.originalStartPoint = this.originalTargetPoints[j];
					}
					else
					{
						this.endPoint = this.targetPoints[j];
						this.originalEndPoint = this.originalTargetPoints[j];
						this.endNode = this.targetNodes[j];
					}
					this.callbacks[j](this);
					this.vectorPaths[j] = this.vectorPath;
				}
			}
			if (flag)
			{
				base.CompleteState = PathCompleteState.Complete;
				if (!this.pathsForAll)
				{
					this.path = this.nodePaths[this.chosenTarget];
					this.vectorPath = this.vectorPaths[this.chosenTarget];
					if (this.inverted)
					{
						this.endPoint = startPoint;
						this.endNode = startNode;
						this.startNode = this.targetNodes[this.chosenTarget];
						this.startPoint = this.targetPoints[this.chosenTarget];
						this.originalEndPoint = originalStartPoint;
						this.originalStartPoint = this.originalTargetPoints[this.chosenTarget];
					}
					else
					{
						this.endPoint = this.targetPoints[this.chosenTarget];
						this.originalEndPoint = this.originalTargetPoints[this.chosenTarget];
						this.endNode = this.targetNodes[this.chosenTarget];
					}
				}
			}
			else
			{
				base.CompleteState = PathCompleteState.Error;
			}
			if (this.callback != null)
			{
				this.callback(this);
			}
		}

		public void FoundTarget(PathNode nodeR, int i)
		{
			nodeR.flag1 = false;
			this.Trace(nodeR);
			this.vectorPaths[i] = this.vectorPath;
			this.nodePaths[i] = this.path;
			this.vectorPath = ListPool<Vector3>.Claim();
			this.path = ListPool<GraphNode>.Claim();
			this.targetsFound[i] = true;
			this.targetNodeCount--;
			if (!this.pathsForAll)
			{
				base.CompleteState = PathCompleteState.Complete;
				this.chosenTarget = i;
				this.targetNodeCount = 0;
				return;
			}
			if (this.targetNodeCount <= 0)
			{
				base.CompleteState = PathCompleteState.Complete;
				return;
			}
			if (this.heuristicMode == MultiTargetPath.HeuristicMode.MovingAverage)
			{
				Vector3 vector = Vector3.zero;
				int num = 0;
				for (int j = 0; j < this.targetPoints.Length; j++)
				{
					if (!this.targetsFound[j])
					{
						vector += (Vector3)this.targetNodes[j].position;
						num++;
					}
				}
				if (num > 0)
				{
					vector /= (float)num;
				}
				this.hTarget = (Int3)vector;
				this.RebuildOpenList();
			}
			else if (this.heuristicMode == MultiTargetPath.HeuristicMode.MovingMidpoint)
			{
				Vector3 vector2 = Vector3.zero;
				Vector3 vector3 = Vector3.zero;
				bool flag = false;
				for (int k = 0; k < this.targetPoints.Length; k++)
				{
					if (!this.targetsFound[k])
					{
						if (!flag)
						{
							vector2 = (Vector3)this.targetNodes[k].position;
							vector3 = (Vector3)this.targetNodes[k].position;
							flag = true;
						}
						else
						{
							vector2 = Vector3.Min((Vector3)this.targetNodes[k].position, vector2);
							vector3 = Vector3.Max((Vector3)this.targetNodes[k].position, vector3);
						}
					}
				}
				Int3 hTarget = (Int3)((vector2 + vector3) * 0.5f);
				this.hTarget = hTarget;
				this.RebuildOpenList();
			}
			else if (this.heuristicMode == MultiTargetPath.HeuristicMode.Sequential && this.sequentialTarget == i)
			{
				float num2 = 0f;
				for (int l = 0; l < this.targetPoints.Length; l++)
				{
					if (!this.targetsFound[l])
					{
						float sqrMagnitude = (this.targetNodes[l].position - this.startNode.position).sqrMagnitude;
						if (sqrMagnitude > num2)
						{
							num2 = sqrMagnitude;
							this.hTarget = (Int3)this.targetPoints[l];
							this.sequentialTarget = l;
						}
					}
				}
				this.RebuildOpenList();
			}
		}

		protected void RebuildOpenList()
		{
			BinaryHeapM heap = this.pathHandler.GetHeap();
			for (int i = 0; i < heap.numberOfItems; i++)
			{
				PathNode node = heap.GetNode(i);
				node.H = base.CalculateHScore(node.node);
				heap.SetF(i, node.F);
			}
			this.pathHandler.RebuildHeap();
		}

		public override void Prepare()
		{
			this.nnConstraint.tags = this.enabledTags;
			NNInfo nearest = AstarPath.active.GetNearest(this.startPoint, this.nnConstraint, this.startHint);
			this.startNode = nearest.node;
			if (this.startNode == null)
			{
				base.LogError("Could not find start node for multi target path");
				base.Error();
				return;
			}
			if (!this.startNode.Walkable)
			{
				base.LogError("Nearest node to the start point is not walkable");
				base.Error();
				return;
			}
			PathNNConstraint pathNNConstraint = this.nnConstraint as PathNNConstraint;
			if (pathNNConstraint != null)
			{
				pathNNConstraint.SetStart(nearest.node);
			}
			this.vectorPaths = new List<Vector3>[this.targetPoints.Length];
			this.nodePaths = new List<GraphNode>[this.targetPoints.Length];
			this.targetNodes = new GraphNode[this.targetPoints.Length];
			this.targetsFound = new bool[this.targetPoints.Length];
			this.targetNodeCount = this.targetPoints.Length;
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			for (int i = 0; i < this.targetPoints.Length; i++)
			{
				NNInfo nearest2 = AstarPath.active.GetNearest(this.targetPoints[i], this.nnConstraint);
				this.targetNodes[i] = nearest2.node;
				this.targetPoints[i] = nearest2.clampedPosition;
				if (this.targetNodes[i] != null)
				{
					flag3 = true;
					this.endNode = this.targetNodes[i];
				}
				bool flag4 = false;
				if (nearest2.node != null && nearest2.node.Walkable)
				{
					flag = true;
				}
				else
				{
					flag4 = true;
				}
				if (nearest2.node != null && nearest2.node.Area == this.startNode.Area)
				{
					flag2 = true;
				}
				else
				{
					flag4 = true;
				}
				if (flag4)
				{
					this.targetsFound[i] = true;
					this.targetNodeCount--;
				}
			}
			this.startPoint = nearest.clampedPosition;
			this.startIntPoint = (Int3)this.startPoint;
			if (this.startNode == null || !flag3)
			{
				base.LogError(string.Concat(new string[]
				{
					"Couldn't find close nodes to either the start or the end (start = ",
					(this.startNode == null) ? "not found" : "found",
					" end = ",
					(!flag3) ? "none found" : "at least one found",
					")"
				}));
				base.Error();
				return;
			}
			if (!this.startNode.Walkable)
			{
				base.LogError("The node closest to the start point is not walkable");
				base.Error();
				return;
			}
			if (!flag)
			{
				base.LogError("No target nodes were walkable");
				base.Error();
				return;
			}
			if (!flag2)
			{
				base.LogError("There are no valid paths to the targets");
				base.Error();
				return;
			}
			if (this.pathsForAll)
			{
				if (this.heuristicMode == MultiTargetPath.HeuristicMode.None)
				{
					this.heuristic = Heuristic.None;
					this.heuristicScale = 0f;
				}
				else if (this.heuristicMode == MultiTargetPath.HeuristicMode.Average || this.heuristicMode == MultiTargetPath.HeuristicMode.MovingAverage)
				{
					Vector3 vector = Vector3.zero;
					for (int j = 0; j < this.targetNodes.Length; j++)
					{
						vector += (Vector3)this.targetNodes[j].position;
					}
					vector /= (float)this.targetNodes.Length;
					this.hTarget = (Int3)vector;
				}
				else if (this.heuristicMode == MultiTargetPath.HeuristicMode.Midpoint || this.heuristicMode == MultiTargetPath.HeuristicMode.MovingMidpoint)
				{
					Vector3 vector2 = Vector3.zero;
					Vector3 vector3 = Vector3.zero;
					bool flag5 = false;
					for (int k = 0; k < this.targetPoints.Length; k++)
					{
						if (!this.targetsFound[k])
						{
							if (!flag5)
							{
								vector2 = (Vector3)this.targetNodes[k].position;
								vector3 = (Vector3)this.targetNodes[k].position;
								flag5 = true;
							}
							else
							{
								vector2 = Vector3.Min((Vector3)this.targetNodes[k].position, vector2);
								vector3 = Vector3.Max((Vector3)this.targetNodes[k].position, vector3);
							}
						}
					}
					Vector3 ob = (vector2 + vector3) * 0.5f;
					this.hTarget = (Int3)ob;
				}
				else if (this.heuristicMode == MultiTargetPath.HeuristicMode.Sequential)
				{
					float num = 0f;
					for (int l = 0; l < this.targetNodes.Length; l++)
					{
						if (!this.targetsFound[l])
						{
							float sqrMagnitude = (this.targetNodes[l].position - this.startNode.position).sqrMagnitude;
							if (sqrMagnitude > num)
							{
								num = sqrMagnitude;
								this.hTarget = (Int3)this.targetPoints[l];
								this.sequentialTarget = l;
							}
						}
					}
				}
			}
			else
			{
				this.heuristic = Heuristic.None;
				this.heuristicScale = 0f;
			}
		}

		public override void Initialize()
		{
			for (int i = 0; i < this.targetNodes.Length; i++)
			{
				if (this.startNode == this.targetNodes[i])
				{
					PathNode pathNode = this.pathHandler.GetPathNode(this.startNode);
					this.FoundTarget(pathNode, i);
				}
				else if (this.targetNodes[i] != null)
				{
					this.pathHandler.GetPathNode(this.targetNodes[i]).flag1 = true;
				}
			}
			AstarPath.OnPathPostSearch = (OnPathDelegate)Delegate.Combine(AstarPath.OnPathPostSearch, new OnPathDelegate(this.ResetFlags));
			if (this.targetNodeCount <= 0)
			{
				base.CompleteState = PathCompleteState.Complete;
				return;
			}
			PathNode pathNode2 = this.pathHandler.GetPathNode(this.startNode);
			pathNode2.node = this.startNode;
			pathNode2.pathID = this.pathID;
			pathNode2.parent = null;
			pathNode2.cost = 0u;
			pathNode2.G = this.GetTraversalCost(this.startNode);
			pathNode2.H = base.CalculateHScore(this.startNode);
			this.startNode.Open(this, pathNode2, this.pathHandler);
			this.searchedNodes++;
			if (this.pathHandler.HeapEmpty())
			{
				base.LogError("No open points, the start node didn't open any nodes");
				base.Error();
				return;
			}
			this.currentR = this.pathHandler.PopNode();
		}

		public void ResetFlags(Path p)
		{
			AstarPath.OnPathPostSearch = (OnPathDelegate)Delegate.Remove(AstarPath.OnPathPostSearch, new OnPathDelegate(this.ResetFlags));
			if (p != this)
			{
				Debug.LogError("This should have been cleared after it was called on 'this' path. Was it not called? Or did the delegate reset not work?");
			}
			for (int i = 0; i < this.targetNodes.Length; i++)
			{
				if (this.targetNodes[i] != null)
				{
					this.pathHandler.GetPathNode(this.targetNodes[i]).flag1 = false;
				}
			}
		}

		public override void CalculateStep(long targetTick)
		{
			int num = 0;
			while (base.CompleteState == PathCompleteState.NotCalculated)
			{
				this.searchedNodes++;
				if (this.currentR.flag1)
				{
					for (int i = 0; i < this.targetNodes.Length; i++)
					{
						if (!this.targetsFound[i] && this.currentR.node == this.targetNodes[i])
						{
							this.FoundTarget(this.currentR, i);
							if (base.CompleteState != PathCompleteState.NotCalculated)
							{
								break;
							}
						}
					}
					if (this.targetNodeCount <= 0)
					{
						base.CompleteState = PathCompleteState.Complete;
						break;
					}
				}
				this.currentR.node.Open(this, this.currentR, this.pathHandler);
				if (this.pathHandler.HeapEmpty())
				{
					base.CompleteState = PathCompleteState.Complete;
					break;
				}
				this.currentR = this.pathHandler.PopNode();
				if (num > 500)
				{
					if (DateTime.UtcNow.Ticks >= targetTick)
					{
						return;
					}
					num = 0;
				}
				num++;
			}
		}

		protected override void Trace(PathNode node)
		{
			base.Trace(node);
			if (this.inverted)
			{
				int num = this.path.Count / 2;
				for (int i = 0; i < num; i++)
				{
					GraphNode value = this.path[i];
					this.path[i] = this.path[this.path.Count - i - 1];
					this.path[this.path.Count - i - 1] = value;
				}
				for (int j = 0; j < num; j++)
				{
					Vector3 value2 = this.vectorPath[j];
					this.vectorPath[j] = this.vectorPath[this.vectorPath.Count - j - 1];
					this.vectorPath[this.vectorPath.Count - j - 1] = value2;
				}
			}
		}

		public override string DebugString(PathLog logMode)
		{
			if (logMode == PathLog.None || (!base.error && logMode == PathLog.OnlyErrors))
			{
				return string.Empty;
			}
			StringBuilder debugStringBuilder = this.pathHandler.DebugStringBuilder;
			debugStringBuilder.Length = 0;
			debugStringBuilder.Append((!base.error) ? "Path Completed : " : "Path Failed : ");
			debugStringBuilder.Append("Computation Time ");
			debugStringBuilder.Append(this.duration.ToString((logMode != PathLog.Heavy) ? "0.00" : "0.000"));
			debugStringBuilder.Append(" ms Searched Nodes ");
			debugStringBuilder.Append(this.searchedNodes);
			if (!base.error)
			{
				debugStringBuilder.Append("\nLast Found Path Length ");
				debugStringBuilder.Append((this.path != null) ? this.path.Count.ToString() : "Null");
				if (logMode == PathLog.Heavy)
				{
					debugStringBuilder.Append("\nSearch Iterations " + this.searchIterations);
					debugStringBuilder.Append("\nPaths (").Append(this.targetsFound.Length).Append("):");
					for (int i = 0; i < this.targetsFound.Length; i++)
					{
						debugStringBuilder.Append("\n\n\tPath " + i).Append(" Found: ").Append(this.targetsFound[i]);
						GraphNode graphNode = (this.nodePaths[i] != null) ? this.nodePaths[i][this.nodePaths[i].Count - 1] : null;
						if (this.nodePaths[i] != null)
						{
							debugStringBuilder.Append("\n\t\tLength: ");
							debugStringBuilder.Append(this.nodePaths[i].Count);
							if (graphNode != null)
							{
								PathNode pathNode = this.pathHandler.GetPathNode(this.endNode);
								if (pathNode != null)
								{
									debugStringBuilder.Append("\n\t\tEnd Node");
									debugStringBuilder.Append("\n\t\t\tG: ");
									debugStringBuilder.Append(pathNode.G);
									debugStringBuilder.Append("\n\t\t\tH: ");
									debugStringBuilder.Append(pathNode.H);
									debugStringBuilder.Append("\n\t\t\tF: ");
									debugStringBuilder.Append(pathNode.F);
									debugStringBuilder.Append("\n\t\t\tPoint: ");
									debugStringBuilder.Append(this.endPoint.ToString());
									debugStringBuilder.Append("\n\t\t\tGraph: ");
									debugStringBuilder.Append(this.endNode.GraphIndex);
								}
								else
								{
									debugStringBuilder.Append("\n\t\tEnd Node: Null");
								}
							}
						}
					}
					debugStringBuilder.Append("\nStart Node");
					debugStringBuilder.Append("\n\tPoint: ");
					debugStringBuilder.Append(this.endPoint.ToString());
					debugStringBuilder.Append("\n\tGraph: ");
					debugStringBuilder.Append(this.startNode.GraphIndex);
					debugStringBuilder.Append("\nBinary Heap size at completion: ");
					debugStringBuilder.AppendLine((this.pathHandler.GetHeap() != null) ? (this.pathHandler.GetHeap().numberOfItems - 2).ToString() : "Null");
				}
			}
			if (base.error)
			{
				debugStringBuilder.Append("\nError: ");
				debugStringBuilder.Append(base.errorLog);
				debugStringBuilder.AppendLine();
			}
			if (logMode == PathLog.Heavy && !AstarPath.IsUsingMultithreading)
			{
				debugStringBuilder.Append("\nCallback references ");
				if (this.callback != null)
				{
					debugStringBuilder.Append(this.callback.Target.GetType().FullName).AppendLine();
				}
				else
				{
					debugStringBuilder.AppendLine("NULL");
				}
			}
			debugStringBuilder.Append("\nPath Number ");
			debugStringBuilder.Append(this.pathID);
			return debugStringBuilder.ToString();
		}
	}
}

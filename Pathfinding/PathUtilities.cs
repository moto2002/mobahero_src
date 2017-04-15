using Pathfinding.Util;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Pathfinding
{
	public static class PathUtilities
	{
		private static Queue<GraphNode> BFSQueue;

		private static Dictionary<GraphNode, int> BFSMap;

		public static bool IsPathPossible(GraphNode n1, GraphNode n2)
		{
			return n1.Walkable && n2.Walkable && n1.Area == n2.Area;
		}

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

		public static List<GraphNode> GetReachableNodes(GraphNode seed, int tagMask = -1)
		{
			Stack<GraphNode> stack = StackPool<GraphNode>.Claim();
			List<GraphNode> list = ListPool<GraphNode>.Claim();
			HashSet<GraphNode> map = new HashSet<GraphNode>();
			GraphNodeDelegate graphNodeDelegate;
			if (tagMask == -1)
			{
				graphNodeDelegate = delegate(GraphNode node)
				{
					if (node.Walkable && map.Add(node))
					{
						list.Add(node);
						stack.Push(node);
					}
				};
			}
			else
			{
				graphNodeDelegate = delegate(GraphNode node)
				{
					if (node.Walkable && (tagMask >> (int)node.Tag & 1) != 0 && map.Add(node))
					{
						list.Add(node);
						stack.Push(node);
					}
				};
			}
			graphNodeDelegate(seed);
			while (stack.Count > 0)
			{
				stack.Pop().GetConnections(graphNodeDelegate);
			}
			StackPool<GraphNode>.Release(stack);
			return list;
		}

		public static List<GraphNode> BFS(GraphNode seed, int depth, int tagMask = -1)
		{
			if (PathUtilities.BFSQueue == null)
			{
				PathUtilities.BFSQueue = new Queue<GraphNode>();
			}
			Queue<GraphNode> que = PathUtilities.BFSQueue;
			if (PathUtilities.BFSMap == null)
			{
				PathUtilities.BFSMap = new Dictionary<GraphNode, int>();
			}
			Dictionary<GraphNode, int> map = PathUtilities.BFSMap;
			que.Clear();
			map.Clear();
			List<GraphNode> result = ListPool<GraphNode>.Claim();
			int currentDist = -1;
			GraphNodeDelegate graphNodeDelegate;
			if (tagMask == -1)
			{
				graphNodeDelegate = delegate(GraphNode node)
				{
					if (node.Walkable && !map.ContainsKey(node))
					{
						map.Add(node, currentDist + 1);
						result.Add(node);
						que.Enqueue(node);
					}
				};
			}
			else
			{
				graphNodeDelegate = delegate(GraphNode node)
				{
					if (node.Walkable && (tagMask >> (int)node.Tag & 1) != 0 && !map.ContainsKey(node))
					{
						map.Add(node, currentDist + 1);
						result.Add(node);
						que.Enqueue(node);
					}
				};
			}
			graphNodeDelegate(seed);
			while (que.Count > 0)
			{
				GraphNode graphNode = que.Dequeue();
				currentDist = map[graphNode];
				if (currentDist >= depth)
				{
					break;
				}
				graphNode.GetConnections(graphNodeDelegate);
			}
			que.Clear();
			map.Clear();
			return result;
		}

		public static List<Vector3> GetSpiralPoints(int count, float clearance)
		{
			List<Vector3> list = ListPool<Vector3>.Claim(count);
			float num = clearance / 6.28318548f;
			float num2 = 0f;
			list.Add(PathUtilities.InvoluteOfCircle(num, num2));
			for (int i = 0; i < count; i++)
			{
				Vector3 b = list[list.Count - 1];
				float num3 = -num2 / 2f + Mathf.Sqrt(num2 * num2 / 4f + 2f * clearance / num);
				float num4 = num2 + num3;
				float num5 = num2 + 2f * num3;
				while (num5 - num4 > 0.01f)
				{
					float num6 = (num4 + num5) / 2f;
					Vector3 a = PathUtilities.InvoluteOfCircle(num, num6);
					if ((a - b).sqrMagnitude < clearance * clearance)
					{
						num4 = num6;
					}
					else
					{
						num5 = num6;
					}
				}
				list.Add(PathUtilities.InvoluteOfCircle(num, num5));
				num2 = num5;
			}
			return list;
		}

		private static Vector3 InvoluteOfCircle(float a, float t)
		{
			return new Vector3(a * (Mathf.Cos(t) + t * Mathf.Sin(t)), 0f, a * (Mathf.Sin(t) - t * Mathf.Cos(t)));
		}

		public static void GetPointsAroundPointWorld(Vector3 p, IRaycastableGraph g, List<Vector3> previousPoints, float radius, float clearanceRadius)
		{
			if (previousPoints.Count == 0)
			{
				return;
			}
			Vector3 vector = Vector3.zero;
			for (int i = 0; i < previousPoints.Count; i++)
			{
				vector += previousPoints[i];
			}
			vector /= (float)previousPoints.Count;
			for (int j = 0; j < previousPoints.Count; j++)
			{
				int index;
				int expr_50 = index = j;
				Vector3 a = previousPoints[index];
				previousPoints[expr_50] = a - vector;
			}
			PathUtilities.GetPointsAroundPoint(p, g, previousPoints, radius, clearanceRadius);
		}

		public static void GetPointsAroundPoint(Vector3 p, IRaycastableGraph g, List<Vector3> previousPoints, float radius, float clearanceRadius)
		{
			if (g == null)
			{
				throw new ArgumentNullException("g");
			}
			NavGraph navGraph = g as NavGraph;
			if (navGraph == null)
			{
				throw new ArgumentException("g is not a NavGraph");
			}
			NNInfo nearestForce = navGraph.GetNearestForce(p, NNConstraint.Default);
			p = nearestForce.clampedPosition;
			if (nearestForce.node == null)
			{
				return;
			}
			radius = Mathf.Max(radius, 1.4142f * clearanceRadius * Mathf.Sqrt((float)previousPoints.Count));
			clearanceRadius *= clearanceRadius;
			for (int i = 0; i < previousPoints.Count; i++)
			{
				Vector3 vector = previousPoints[i];
				float magnitude = vector.magnitude;
				if (magnitude > 0f)
				{
					vector /= magnitude;
				}
				float num = radius;
				vector *= num;
				bool flag = false;
				int num2 = 0;
				do
				{
					Vector3 vector2 = p + vector;
					GraphHitInfo graphHitInfo;
					if (g.Linecast(p, vector2, nearestForce.node, out graphHitInfo))
					{
						vector2 = graphHitInfo.point;
					}
					for (float num3 = 0.1f; num3 <= 1f; num3 += 0.05f)
					{
						Vector3 vector3 = (vector2 - p) * num3 + p;
						flag = true;
						for (int j = 0; j < i; j++)
						{
							if ((previousPoints[j] - vector3).sqrMagnitude < clearanceRadius)
							{
								flag = false;
								break;
							}
						}
						if (flag)
						{
							previousPoints[i] = vector3;
							break;
						}
					}
					if (!flag)
					{
						if (num2 > 8)
						{
							flag = true;
						}
						else
						{
							clearanceRadius *= 0.9f;
							vector = UnityEngine.Random.onUnitSphere * Mathf.Lerp(num, radius, (float)(num2 / 5));
							vector.y = 0f;
							num2++;
						}
					}
				}
				while (!flag);
			}
		}
	}
}

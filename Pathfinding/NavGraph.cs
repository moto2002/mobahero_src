using Pathfinding.Serialization;
using Pathfinding.Serialization.JsonFx;
using Pathfinding.Util;
using System;
using UnityEngine;

namespace Pathfinding
{
	public abstract class NavGraph
	{
		public byte[] _sguid;

		public AstarPath active;

		[JsonMember]
		public uint initialPenalty;

		[JsonMember]
		public bool open;

		public uint graphIndex;

		[JsonMember]
		public string name;

		[JsonMember]
		public bool drawGizmos = true;

		[JsonMember]
		public bool infoScreenOpen;

		[JsonMember]
		public Matrix4x4 matrix;

		public Matrix4x4 inverseMatrix;

		[JsonMember]
		public Pathfinding.Util.Guid guid
		{
			get
			{
				if (this._sguid == null || this._sguid.Length != 16)
				{
					this._sguid = Pathfinding.Util.Guid.NewGuid().ToByteArray();
				}
				return new Pathfinding.Util.Guid(this._sguid);
			}
			set
			{
				this._sguid = value.ToByteArray();
			}
		}

		public virtual int CountNodes()
		{
			int count = 0;
			GraphNodeDelegateCancelable del = delegate(GraphNode node)
			{
				count++;
				return true;
			};
			this.GetNodes(del);
			return count;
		}

		public abstract void GetNodes(GraphNodeDelegateCancelable del);

		public void SetMatrix(Matrix4x4 m)
		{
			this.matrix = m;
			this.inverseMatrix = m.inverse;
		}

		public virtual void CreateNodes(int number)
		{
			throw new NotSupportedException();
		}

		public virtual void RelocateNodes(Matrix4x4 oldMatrix, Matrix4x4 newMatrix)
		{
			Matrix4x4 inverse = oldMatrix.inverse;
			Matrix4x4 m = inverse * newMatrix;
			this.GetNodes(delegate(GraphNode node)
			{
				node.position = (Int3)m.MultiplyPoint((Vector3)node.position);
				return true;
			});
			this.SetMatrix(newMatrix);
		}

		public NNInfo GetNearest(Vector3 position)
		{
			return this.GetNearest(position, NNConstraint.None);
		}

		public NNInfo GetNearest(Vector3 position, NNConstraint constraint)
		{
			return this.GetNearest(position, constraint, null);
		}

		public virtual NNInfo GetNearest(Vector3 position, NNConstraint constraint, GraphNode hint)
		{
			float maxDistSqr = (!constraint.constrainDistance) ? float.PositiveInfinity : AstarPath.active.maxNearestNodeDistanceSqr;
			float minDist = float.PositiveInfinity;
			GraphNode minNode = null;
			float minConstDist = float.PositiveInfinity;
			GraphNode minConstNode = null;
			this.GetNodes(delegate(GraphNode node)
			{
				float sqrMagnitude = (position - (Vector3)node.position).sqrMagnitude;
				if (sqrMagnitude < minDist)
				{
					minDist = sqrMagnitude;
					minNode = node;
				}
				if (sqrMagnitude < minConstDist && sqrMagnitude < maxDistSqr && constraint.Suitable(node))
				{
					minConstDist = sqrMagnitude;
					minConstNode = node;
				}
				return true;
			});
			NNInfo result = new NNInfo(minNode);
			result.constrainedNode = minConstNode;
			if (minConstNode != null)
			{
				result.constClampedPosition = (Vector3)minConstNode.position;
			}
			else if (minNode != null)
			{
				result.constrainedNode = minNode;
				result.constClampedPosition = (Vector3)minNode.position;
			}
			return result;
		}

		public virtual NNInfo GetNearestForce(Vector3 position, NNConstraint constraint)
		{
			return this.GetNearest(position, constraint);
		}

		public virtual void Awake()
		{
		}

		public void SafeOnDestroy()
		{
			AstarPath.RegisterSafeUpdate(new OnVoidDelegate(this.OnDestroy));
		}

		public virtual void OnDestroy()
		{
			this.GetNodes(delegate(GraphNode node)
			{
				node.Destroy();
				return true;
			});
		}

		public void ScanGraph()
		{
			if (AstarPath.OnPreScan != null)
			{
				AstarPath.OnPreScan(AstarPath.active);
			}
			if (AstarPath.OnGraphPreScan != null)
			{
				AstarPath.OnGraphPreScan(this);
			}
			this.ScanInternal();
			if (AstarPath.OnGraphPostScan != null)
			{
				AstarPath.OnGraphPostScan(this);
			}
			if (AstarPath.OnPostScan != null)
			{
				AstarPath.OnPostScan(AstarPath.active);
			}
		}

		[Obsolete("Please use AstarPath.active.Scan or if you really want this.ScanInternal which has the same functionality as this method had")]
		public void Scan()
		{
			throw new Exception("This method is deprecated. Please use AstarPath.active.Scan or if you really want this.ScanInternal which has the same functionality as this method had.");
		}

		public void ScanInternal()
		{
			this.ScanInternal(null);
		}

		public abstract void ScanInternal(OnScanStatus statusCallback);

		public virtual Color NodeColor(GraphNode node, PathHandler data)
		{
			Color result = AstarColor.NodeConnection;
			bool flag = false;
			if (node == null)
			{
				return AstarColor.NodeConnection;
			}
			GraphDebugMode debugMode = AstarPath.active.debugMode;
			switch (debugMode)
			{
			case GraphDebugMode.Penalty:
				result = Color.Lerp(AstarColor.ConnectionLowLerp, AstarColor.ConnectionHighLerp, (node.Penalty - AstarPath.active.debugFloor) / (AstarPath.active.debugRoof - AstarPath.active.debugFloor));
				flag = true;
				goto IL_A9;
			case GraphDebugMode.Connections:
				IL_33:
				if (debugMode != GraphDebugMode.Areas)
				{
					goto IL_A9;
				}
				result = AstarColor.GetAreaColor(node.Area);
				flag = true;
				goto IL_A9;
			case GraphDebugMode.Tags:
				result = AstarMath.IntToColor((int)node.Tag, 0.5f);
				flag = true;
				goto IL_A9;
			}
			goto IL_33;
			IL_A9:
			if (!flag)
			{
				if (data == null)
				{
					return AstarColor.NodeConnection;
				}
				PathNode pathNode = data.GetPathNode(node);
				switch (AstarPath.active.debugMode)
				{
				case GraphDebugMode.G:
					result = Color.Lerp(AstarColor.ConnectionLowLerp, AstarColor.ConnectionHighLerp, (pathNode.G - AstarPath.active.debugFloor) / (AstarPath.active.debugRoof - AstarPath.active.debugFloor));
					break;
				case GraphDebugMode.H:
					result = Color.Lerp(AstarColor.ConnectionLowLerp, AstarColor.ConnectionHighLerp, (pathNode.H - AstarPath.active.debugFloor) / (AstarPath.active.debugRoof - AstarPath.active.debugFloor));
					break;
				case GraphDebugMode.F:
					result = Color.Lerp(AstarColor.ConnectionLowLerp, AstarColor.ConnectionHighLerp, (pathNode.F - AstarPath.active.debugFloor) / (AstarPath.active.debugRoof - AstarPath.active.debugFloor));
					break;
				}
			}
			result.a *= 0.5f;
			return result;
		}

		public virtual void SerializeExtraInfo(GraphSerializationContext ctx)
		{
		}

		public virtual void DeserializeExtraInfo(GraphSerializationContext ctx)
		{
		}

		public virtual void PostDeserialization()
		{
		}

		public static bool InSearchTree(GraphNode node, Path path)
		{
			if (path == null || path.pathHandler == null)
			{
				return true;
			}
			PathNode pathNode = path.pathHandler.GetPathNode(node);
			return pathNode.pathID == path.pathID;
		}

		public virtual void OnDrawGizmos(bool drawNodes)
		{
			if (!drawNodes)
			{
				return;
			}
			PathHandler data = AstarPath.active.debugPathData;
			GraphNode node = null;
			GraphNodeDelegate del = delegate(GraphNode o)
			{
				Gizmos.DrawLine((Vector3)node.position, (Vector3)o.position);
			};
			this.GetNodes(delegate(GraphNode _node)
			{
				node = _node;
				Gizmos.color = this.NodeColor(node, AstarPath.active.debugPathData);
				if (AstarPath.active.showSearchTree && !NavGraph.InSearchTree(node, AstarPath.active.debugPath))
				{
					return true;
				}
				PathNode pathNode = (data == null) ? null : data.GetPathNode(node);
				if (AstarPath.active.showSearchTree && pathNode != null && pathNode.parent != null)
				{
					Gizmos.DrawLine((Vector3)node.position, (Vector3)pathNode.parent.node.position);
				}
				else
				{
					node.GetConnections(del);
				}
				return true;
			});
		}
	}
}

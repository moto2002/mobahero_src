using Pathfinding.Serialization;
using Pathfinding.Serialization.JsonFx;
using Pathfinding.Util;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Pathfinding
{
	[JsonOptIn]
	public class GridGraph : NavGraph, IUpdatableGraph, IRaycastableGraph
	{
		public class TextureData
		{
			public enum ChannelUse
			{
				None,
				Penalty,
				Position,
				WalkablePenalty
			}

			public bool enabled;

			public Texture2D source;

			public float[] factors = new float[3];

			public GridGraph.TextureData.ChannelUse[] channels = new GridGraph.TextureData.ChannelUse[3];

			private Color32[] data;

			public void Initialize()
			{
				if (this.enabled && this.source != null)
				{
					for (int i = 0; i < this.channels.Length; i++)
					{
						if (this.channels[i] != GridGraph.TextureData.ChannelUse.None)
						{
							try
							{
								this.data = this.source.GetPixels32();
							}
							catch (UnityException ex)
							{
								Debug.LogWarning(ex.ToString());
								this.data = null;
							}
							break;
						}
					}
				}
			}

			public void Apply(GridNode node, int x, int z)
			{
				if (this.enabled && this.data != null && x < this.source.width && z < this.source.height)
				{
					Color32 color = this.data[z * this.source.width + x];
					if (this.channels[0] != GridGraph.TextureData.ChannelUse.None)
					{
						this.ApplyChannel(node, x, z, (int)color.r, this.channels[0], this.factors[0]);
					}
					if (this.channels[1] != GridGraph.TextureData.ChannelUse.None)
					{
						this.ApplyChannel(node, x, z, (int)color.g, this.channels[1], this.factors[1]);
					}
					if (this.channels[2] != GridGraph.TextureData.ChannelUse.None)
					{
						this.ApplyChannel(node, x, z, (int)color.b, this.channels[2], this.factors[2]);
					}
				}
			}

			private void ApplyChannel(GridNode node, int x, int z, int value, GridGraph.TextureData.ChannelUse channelUse, float factor)
			{
				switch (channelUse)
				{
				case GridGraph.TextureData.ChannelUse.Penalty:
					node.Penalty += (uint)Mathf.RoundToInt((float)value * factor);
					break;
				case GridGraph.TextureData.ChannelUse.Position:
					node.position = GridNode.GetGridGraph(node.GraphIndex).GetNodePosition(node.NodeInGridIndex, Mathf.RoundToInt((float)value * factor * 1000f));
					break;
				case GridGraph.TextureData.ChannelUse.WalkablePenalty:
					if (value == 0)
					{
						node.Walkable = false;
					}
					else
					{
						node.Penalty += (uint)Mathf.RoundToInt((float)(value - 1) * factor);
					}
					break;
				}
			}
		}

		public const int getNearestForceOverlap = 2;

		public int width;

		public int depth;

		[JsonMember]
		public float aspectRatio = 1f;

		[JsonMember]
		public float isometricAngle;

		[JsonMember]
		public Vector3 rotation;

		public Bounds bounds;

		[JsonMember]
		public Vector3 center;

		[JsonMember]
		public Vector2 unclampedSize;

		[JsonMember]
		public float nodeSize = 1f;

		[JsonMember]
		public GraphCollision collision;

		[JsonMember]
		public float maxClimb = 0.4f;

		[JsonMember]
		public int maxClimbAxis = 1;

		[JsonMember]
		public float maxSlope = 90f;

		[JsonMember]
		public int erodeIterations;

		[JsonMember]
		public bool erosionUseTags;

		[JsonMember]
		public int erosionFirstTag = 1;

		[JsonMember]
		public bool autoLinkGrids;

		[JsonMember]
		public float autoLinkDistLimit = 10f;

		[JsonMember]
		public NumNeighbours neighbours = NumNeighbours.Eight;

		[JsonMember]
		public bool cutCorners = true;

		[JsonMember]
		public float penaltyPositionOffset;

		[JsonMember]
		public bool penaltyPosition;

		[JsonMember]
		public float penaltyPositionFactor = 1f;

		[JsonMember]
		public bool penaltyAngle;

		[JsonMember]
		public float penaltyAngleFactor = 100f;

		[JsonMember]
		public float penaltyAnglePower = 1f;

		[JsonMember]
		public bool useJumpPointSearch;

		[JsonMember]
		public GridGraph.TextureData textureData = new GridGraph.TextureData();

		public Vector2 size;

		[NonSerialized]
		public readonly int[] neighbourOffsets = new int[8];

		[NonSerialized]
		public readonly uint[] neighbourCosts = new uint[8];

		[NonSerialized]
		public readonly int[] neighbourXOffsets = new int[8];

		[NonSerialized]
		public readonly int[] neighbourZOffsets = new int[8];

		public Matrix4x4 boundsMatrix;

		public Matrix4x4 boundsMatrix2;

		public int scans;

		public GridNode[] nodes;

		[NonSerialized]
		protected int[] corners;

		public virtual bool uniformWidthDepthGrid
		{
			get
			{
				return true;
			}
		}

		public bool useRaycastNormal
		{
			get
			{
				return Math.Abs(90f - this.maxSlope) > 1.401298E-45f;
			}
		}

		public int Width
		{
			get
			{
				return this.width;
			}
			set
			{
				this.width = value;
			}
		}

		public int Depth
		{
			get
			{
				return this.depth;
			}
			set
			{
				this.depth = value;
			}
		}

		public GridGraph()
		{
			this.unclampedSize = new Vector2(10f, 10f);
			this.nodeSize = 1f;
			this.collision = new GraphCollision();
		}

		public override void OnDestroy()
		{
			base.OnDestroy();
			this.RemoveGridGraphFromStatic();
		}

		public void RemoveGridGraphFromStatic()
		{
			GridNode.SetGridGraph(AstarPath.active.astarData.GetGraphIndex(this), null);
		}

		public override void GetNodes(GraphNodeDelegateCancelable del)
		{
			if (this.nodes == null)
			{
				return;
			}
			int num = 0;
			while (num < this.nodes.Length && del(this.nodes[num]))
			{
				num++;
			}
		}

		public Int3 GetNodePosition(int index, int yOffset)
		{
			int num = index / this.Width;
			int num2 = index - num * this.Width;
			return (Int3)this.matrix.MultiplyPoint3x4(new Vector3((float)num2 + 0.5f, (float)yOffset * 0.001f, (float)num + 0.5f));
		}

		public uint GetConnectionCost(int dir)
		{
			return this.neighbourCosts[dir];
		}

		public GridNode GetNodeConnection(GridNode node, int dir)
		{
			if (!node.GetConnectionInternal(dir))
			{
				return null;
			}
			if (!node.EdgeNode)
			{
				return this.nodes[node.NodeInGridIndex + this.neighbourOffsets[dir]];
			}
			int nodeInGridIndex = node.NodeInGridIndex;
			int num = nodeInGridIndex / this.Width;
			int x = nodeInGridIndex - num * this.Width;
			return this.GetNodeConnection(nodeInGridIndex, x, num, dir);
		}

		public bool HasNodeConnection(GridNode node, int dir)
		{
			if (!node.GetConnectionInternal(dir))
			{
				return false;
			}
			if (!node.EdgeNode)
			{
				return true;
			}
			int nodeInGridIndex = node.NodeInGridIndex;
			int num = nodeInGridIndex / this.Width;
			int x = nodeInGridIndex - num * this.Width;
			return this.HasNodeConnection(nodeInGridIndex, x, num, dir);
		}

		public void SetNodeConnection(GridNode node, int dir, bool value)
		{
			int nodeInGridIndex = node.NodeInGridIndex;
			int num = nodeInGridIndex / this.Width;
			int x = nodeInGridIndex - num * this.Width;
			this.SetNodeConnection(nodeInGridIndex, x, num, dir, value);
		}

		private GridNode GetNodeConnection(int index, int x, int z, int dir)
		{
			if (!this.nodes[index].GetConnectionInternal(dir))
			{
				return null;
			}
			int num = x + this.neighbourXOffsets[dir];
			if (num < 0 || num >= this.Width)
			{
				return null;
			}
			int num2 = z + this.neighbourZOffsets[dir];
			if (num2 < 0 || num2 >= this.Depth)
			{
				return null;
			}
			int num3 = index + this.neighbourOffsets[dir];
			return this.nodes[num3];
		}

		public void SetNodeConnection(int index, int x, int z, int dir, bool value)
		{
			this.nodes[index].SetConnectionInternal(dir, value);
		}

		public bool HasNodeConnection(int index, int x, int z, int dir)
		{
			if (!this.nodes[index].GetConnectionInternal(dir))
			{
				return false;
			}
			int num = x + this.neighbourXOffsets[dir];
			if (num < 0 || num >= this.Width)
			{
				return false;
			}
			int num2 = z + this.neighbourZOffsets[dir];
			return num2 >= 0 && num2 < this.Depth;
		}

		public void UpdateSizeFromWidthDepth()
		{
			this.unclampedSize = new Vector2((float)this.width, (float)this.depth) * this.nodeSize;
			this.GenerateMatrix();
		}

		public void GenerateMatrix()
		{
			this.size = this.unclampedSize;
			this.size.x = this.size.x * Mathf.Sign(this.size.x);
			this.size.y = this.size.y * Mathf.Sign(this.size.y);
			this.nodeSize = Mathf.Clamp(this.nodeSize, this.size.x / 1024f, float.PositiveInfinity);
			this.nodeSize = Mathf.Clamp(this.nodeSize, this.size.y / 1024f, float.PositiveInfinity);
			this.size.x = ((this.size.x >= this.nodeSize) ? this.size.x : this.nodeSize);
			this.size.y = ((this.size.y >= this.nodeSize) ? this.size.y : this.nodeSize);
			Matrix4x4 rhs = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0f, 45f, 0f), Vector3.one);
			rhs = Matrix4x4.Scale(new Vector3(Mathf.Cos(0.0174532924f * this.isometricAngle), 1f, 1f)) * rhs;
			rhs = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0f, -45f, 0f), Vector3.one) * rhs;
			this.boundsMatrix = Matrix4x4.TRS(this.center, Quaternion.Euler(this.rotation), new Vector3(this.aspectRatio, 1f, 1f)) * rhs;
			this.width = Mathf.FloorToInt(this.size.x / this.nodeSize);
			this.depth = Mathf.FloorToInt(this.size.y / this.nodeSize);
			if (Mathf.Approximately(this.size.x / this.nodeSize, (float)Mathf.CeilToInt(this.size.x / this.nodeSize)))
			{
				this.width = Mathf.CeilToInt(this.size.x / this.nodeSize);
			}
			if (Mathf.Approximately(this.size.y / this.nodeSize, (float)Mathf.CeilToInt(this.size.y / this.nodeSize)))
			{
				this.depth = Mathf.CeilToInt(this.size.y / this.nodeSize);
			}
			Matrix4x4 matrix = Matrix4x4.TRS(this.boundsMatrix.MultiplyPoint3x4(-new Vector3(this.size.x, 0f, this.size.y) * 0.5f), Quaternion.Euler(this.rotation), new Vector3(this.nodeSize * this.aspectRatio, 1f, this.nodeSize)) * rhs;
			base.SetMatrix(matrix);
		}

		public override NNInfo GetNearest(Vector3 position, NNConstraint constraint, GraphNode hint)
		{
			if (this.nodes == null || this.depth * this.width != this.nodes.Length)
			{
				return default(NNInfo);
			}
			position = this.inverseMatrix.MultiplyPoint3x4(position);
			float num = position.x - 0.5f;
			float num2 = position.z - 0.5f;
			int num3 = Mathf.Clamp(Mathf.RoundToInt(num), 0, this.width - 1);
			int num4 = Mathf.Clamp(Mathf.RoundToInt(num2), 0, this.depth - 1);
			NNInfo result = new NNInfo(this.nodes[num4 * this.width + num3]);
			float y = this.inverseMatrix.MultiplyPoint3x4((Vector3)this.nodes[num4 * this.width + num3].position).y;
			result.clampedPosition = this.matrix.MultiplyPoint3x4(new Vector3(Mathf.Clamp(num, (float)num3 - 0.5f, (float)num3 + 0.5f) + 0.5f, y, Mathf.Clamp(num2, (float)num4 - 0.5f, (float)num4 + 0.5f) + 0.5f));
			return result;
		}

		public NNInfo GetNodeByPosition(Vector3 position)
		{
			if (this.nodes == null || this.depth * this.width != this.nodes.Length)
			{
				return default(NNInfo);
			}
			position = this.inverseMatrix.MultiplyPoint3x4(position);
			float num = position.x - 0.5f;
			float num2 = position.z - 0.5f;
			int num3 = Mathf.RoundToInt(num);
			int num4 = Mathf.RoundToInt(num2);
			if (num3 < 0 || num3 > this.width - 1)
			{
				return new NNInfo(null);
			}
			if (num4 < 0 || num4 > this.depth - 1)
			{
				return new NNInfo(null);
			}
			num3 = Mathf.Clamp(Mathf.RoundToInt(num), 0, this.width - 1);
			num4 = Mathf.Clamp(Mathf.RoundToInt(num2), 0, this.depth - 1);
			NNInfo result = new NNInfo(this.nodes[num4 * this.width + num3]);
			float y = this.inverseMatrix.MultiplyPoint3x4((Vector3)this.nodes[num4 * this.width + num3].position).y;
			result.clampedPosition = this.matrix.MultiplyPoint3x4(new Vector3(Mathf.Clamp(num, (float)num3 - 0.5f, (float)num3 + 0.5f) + 0.5f, y, Mathf.Clamp(num2, (float)num4 - 0.5f, (float)num4 + 0.5f) + 0.5f));
			return result;
		}

		public override NNInfo GetNearestForce(Vector3 position, NNConstraint constraint)
		{
			if (this.nodes == null || this.depth * this.width != this.nodes.Length)
			{
				return default(NNInfo);
			}
			Vector3 b = position;
			position = this.inverseMatrix.MultiplyPoint3x4(position);
			float num = position.x - 0.5f;
			float num2 = position.z - 0.5f;
			int num3 = Mathf.Clamp(Mathf.RoundToInt(num), 0, this.width - 1);
			int num4 = Mathf.Clamp(Mathf.RoundToInt(num2), 0, this.depth - 1);
			GridNode gridNode = this.nodes[num3 + num4 * this.width];
			GridNode gridNode2 = null;
			float num5 = float.PositiveInfinity;
			int num6 = 2;
			Vector3 clampedPosition = Vector3.zero;
			NNInfo result = new NNInfo(null);
			if (constraint.Suitable(gridNode))
			{
				gridNode2 = gridNode;
				num5 = ((Vector3)gridNode2.position - b).sqrMagnitude;
				float y = this.inverseMatrix.MultiplyPoint3x4((Vector3)gridNode.position).y;
				clampedPosition = this.matrix.MultiplyPoint3x4(new Vector3(Mathf.Clamp(num, (float)num3 - 0.5f, (float)num3 + 0.5f) + 0.5f, y, Mathf.Clamp(num2, (float)num4 - 0.5f, (float)num4 + 0.5f) + 0.5f));
			}
			if (gridNode2 != null)
			{
				result.node = gridNode2;
				result.clampedPosition = clampedPosition;
				if (num6 == 0)
				{
					return result;
				}
				num6--;
			}
			float num7 = (!constraint.constrainDistance) ? float.PositiveInfinity : AstarPath.active.maxNearestNodeDistance;
			float num8 = num7 * num7;
			int num9 = 1;
			while (this.nodeSize * (float)num9 <= num7)
			{
				bool flag = false;
				int i = num4 + num9;
				int num10 = i * this.width;
				int j;
				for (j = num3 - num9; j <= num3 + num9; j++)
				{
					if (j >= 0 && i >= 0 && j < this.width && i < this.depth)
					{
						flag = true;
						if (constraint.Suitable(this.nodes[j + num10]))
						{
							float sqrMagnitude = ((Vector3)this.nodes[j + num10].position - b).sqrMagnitude;
							if (sqrMagnitude < num5 && sqrMagnitude < num8)
							{
								num5 = sqrMagnitude;
								gridNode2 = this.nodes[j + num10];
								clampedPosition = this.matrix.MultiplyPoint3x4(new Vector3(Mathf.Clamp(num, (float)j - 0.5f, (float)j + 0.5f) + 0.5f, this.inverseMatrix.MultiplyPoint3x4((Vector3)gridNode2.position).y, Mathf.Clamp(num2, (float)i - 0.5f, (float)i + 0.5f) + 0.5f));
							}
						}
					}
				}
				i = num4 - num9;
				num10 = i * this.width;
				for (j = num3 - num9; j <= num3 + num9; j++)
				{
					if (j >= 0 && i >= 0 && j < this.width && i < this.depth)
					{
						flag = true;
						if (constraint.Suitable(this.nodes[j + num10]))
						{
							float sqrMagnitude2 = ((Vector3)this.nodes[j + num10].position - b).sqrMagnitude;
							if (sqrMagnitude2 < num5 && sqrMagnitude2 < num8)
							{
								num5 = sqrMagnitude2;
								gridNode2 = this.nodes[j + num10];
								clampedPosition = this.matrix.MultiplyPoint3x4(new Vector3(Mathf.Clamp(num, (float)j - 0.5f, (float)j + 0.5f) + 0.5f, this.inverseMatrix.MultiplyPoint3x4((Vector3)gridNode2.position).y, Mathf.Clamp(num2, (float)i - 0.5f, (float)i + 0.5f) + 0.5f));
							}
						}
					}
				}
				j = num3 - num9;
				i = num4 - num9 + 1;
				for (i = num4 - num9 + 1; i <= num4 + num9 - 1; i++)
				{
					if (j >= 0 && i >= 0 && j < this.width && i < this.depth)
					{
						flag = true;
						if (constraint.Suitable(this.nodes[j + i * this.width]))
						{
							float sqrMagnitude3 = ((Vector3)this.nodes[j + i * this.width].position - b).sqrMagnitude;
							if (sqrMagnitude3 < num5 && sqrMagnitude3 < num8)
							{
								num5 = sqrMagnitude3;
								gridNode2 = this.nodes[j + i * this.width];
								clampedPosition = this.matrix.MultiplyPoint3x4(new Vector3(Mathf.Clamp(num, (float)j - 0.5f, (float)j + 0.5f) + 0.5f, this.inverseMatrix.MultiplyPoint3x4((Vector3)gridNode2.position).y, Mathf.Clamp(num2, (float)i - 0.5f, (float)i + 0.5f) + 0.5f));
							}
						}
					}
				}
				j = num3 + num9;
				for (i = num4 - num9 + 1; i <= num4 + num9 - 1; i++)
				{
					if (j >= 0 && i >= 0 && j < this.width && i < this.depth)
					{
						flag = true;
						if (constraint.Suitable(this.nodes[j + i * this.width]))
						{
							float sqrMagnitude4 = ((Vector3)this.nodes[j + i * this.width].position - b).sqrMagnitude;
							if (sqrMagnitude4 < num5 && sqrMagnitude4 < num8)
							{
								num5 = sqrMagnitude4;
								gridNode2 = this.nodes[j + i * this.width];
								clampedPosition = this.matrix.MultiplyPoint3x4(new Vector3(Mathf.Clamp(num, (float)j - 0.5f, (float)j + 0.5f) + 0.5f, this.inverseMatrix.MultiplyPoint3x4((Vector3)gridNode2.position).y, Mathf.Clamp(num2, (float)i - 0.5f, (float)i + 0.5f) + 0.5f));
							}
						}
					}
				}
				if (gridNode2 != null)
				{
					if (num6 == 0)
					{
						result.node = gridNode2;
						result.clampedPosition = clampedPosition;
						return result;
					}
					num6--;
				}
				if (!flag)
				{
					result.node = gridNode2;
					result.clampedPosition = clampedPosition;
					return result;
				}
				num9++;
			}
			result.node = gridNode2;
			result.clampedPosition = clampedPosition;
			return result;
		}

		public virtual void SetUpOffsetsAndCosts()
		{
			this.neighbourOffsets[0] = -this.width;
			this.neighbourOffsets[1] = 1;
			this.neighbourOffsets[2] = this.width;
			this.neighbourOffsets[3] = -1;
			this.neighbourOffsets[4] = -this.width + 1;
			this.neighbourOffsets[5] = this.width + 1;
			this.neighbourOffsets[6] = this.width - 1;
			this.neighbourOffsets[7] = -this.width - 1;
			uint num = (uint)Mathf.RoundToInt(this.nodeSize * 1000f);
			uint num2 = (uint)Mathf.RoundToInt(this.nodeSize * Mathf.Sqrt(2f) * 1000f);
			this.neighbourCosts[0] = num;
			this.neighbourCosts[1] = num;
			this.neighbourCosts[2] = num;
			this.neighbourCosts[3] = num;
			this.neighbourCosts[4] = num2;
			this.neighbourCosts[5] = num2;
			this.neighbourCosts[6] = num2;
			this.neighbourCosts[7] = num2;
			this.neighbourXOffsets[0] = 0;
			this.neighbourXOffsets[1] = 1;
			this.neighbourXOffsets[2] = 0;
			this.neighbourXOffsets[3] = -1;
			this.neighbourXOffsets[4] = 1;
			this.neighbourXOffsets[5] = 1;
			this.neighbourXOffsets[6] = -1;
			this.neighbourXOffsets[7] = -1;
			this.neighbourZOffsets[0] = -1;
			this.neighbourZOffsets[1] = 0;
			this.neighbourZOffsets[2] = 1;
			this.neighbourZOffsets[3] = 0;
			this.neighbourZOffsets[4] = -1;
			this.neighbourZOffsets[5] = 1;
			this.neighbourZOffsets[6] = 1;
			this.neighbourZOffsets[7] = -1;
		}

		public override void ScanInternal(OnScanStatus statusCallback)
		{
			AstarPath.OnPostScan = (OnScanDelegate)Delegate.Combine(AstarPath.OnPostScan, new OnScanDelegate(this.OnPostScan));
			this.scans++;
			if (this.nodeSize <= 0f)
			{
				return;
			}
			this.GenerateMatrix();
			if (this.width > 1024 || this.depth > 1024)
			{
				Debug.LogError("One of the grid's sides is longer than 1024 nodes");
				return;
			}
			if (this.useJumpPointSearch)
			{
				Debug.LogError("Trying to use Jump Point Search, but support for it is not enabled. Please enable it in the inspector (Grid Graph settings).");
			}
			this.SetUpOffsetsAndCosts();
			int graphIndex = AstarPath.active.astarData.GetGraphIndex(this);
			GridNode.SetGridGraph(graphIndex, this);
			this.nodes = new GridNode[this.width * this.depth];
			for (int i = 0; i < this.nodes.Length; i++)
			{
				this.nodes[i] = new GridNode(this.active);
				this.nodes[i].GraphIndex = (uint)graphIndex;
			}
			if (this.collision == null)
			{
				this.collision = new GraphCollision();
			}
			this.collision.Initialize(this.matrix, this.nodeSize);
			this.textureData.Initialize();
			for (int j = 0; j < this.depth; j++)
			{
				for (int k = 0; k < this.width; k++)
				{
					GridNode gridNode = this.nodes[j * this.width + k];
					gridNode.NodeInGridIndex = j * this.width + k;
					this.UpdateNodePositionCollision(gridNode, k, j, true);
					this.textureData.Apply(gridNode, k, j);
				}
			}
			for (int l = 0; l < this.depth; l++)
			{
				for (int m = 0; m < this.width; m++)
				{
					GridNode node = this.nodes[l * this.width + m];
					this.CalculateConnections(this.nodes, m, l, node);
				}
			}
			this.ErodeWalkableArea();
		}

		public virtual void UpdateNodePositionCollision(GridNode node, int x, int z, bool resetPenalty = true)
		{
			node.position = this.GetNodePosition(node.NodeInGridIndex, 0);
			bool flag = true;
			RaycastHit raycastHit;
			Vector3 ob = this.collision.CheckHeight((Vector3)node.position, out raycastHit, out flag);
			node.position = (Int3)ob;
			if (resetPenalty)
			{
				node.Penalty = this.initialPenalty;
			}
			if (this.penaltyPosition && resetPenalty)
			{
				node.Penalty += (uint)Mathf.RoundToInt(((float)node.position.y - this.penaltyPositionOffset) * this.penaltyPositionFactor);
			}
			if (flag && this.useRaycastNormal && this.collision.heightCheck && raycastHit.normal != Vector3.zero)
			{
				float num = Vector3.Dot(raycastHit.normal.normalized, this.collision.up);
				if (Mathf.Abs(1f - this.collision.up.magnitude) > 0.1f)
				{
					Debug.Log("HI");
				}
				if (this.penaltyAngle && resetPenalty)
				{
					node.Penalty += (uint)Mathf.RoundToInt((1f - Mathf.Pow(num, this.penaltyAnglePower)) * this.penaltyAngleFactor);
				}
				float num2 = Mathf.Cos(this.maxSlope * 0.0174532924f);
				if (num < num2)
				{
					flag = false;
				}
			}
			if (flag)
			{
				node.Walkable = this.collision.Check((Vector3)node.position);
			}
			else
			{
				node.Walkable = flag;
			}
			node.WalkableErosion = node.Walkable;
			if (!node.Walkable)
			{
				node.Tag = 15u;
			}
		}

		public virtual void ErodeWalkableArea()
		{
			this.ErodeWalkableArea(0, 0, this.Width, this.Depth);
		}

		public virtual void ErodeWalkableArea(int xmin, int zmin, int xmax, int zmax)
		{
			xmin = ((xmin >= 0) ? ((xmin <= this.Width) ? xmin : this.Width) : 0);
			xmax = ((xmax >= 0) ? ((xmax <= this.Width) ? xmax : this.Width) : 0);
			zmin = ((zmin >= 0) ? ((zmin <= this.Depth) ? zmin : this.Depth) : 0);
			zmax = ((zmax >= 0) ? ((zmax <= this.Depth) ? zmax : this.Depth) : 0);
			if (!this.erosionUseTags)
			{
				for (int i = 0; i < this.erodeIterations; i++)
				{
					for (int j = zmin; j < zmax; j++)
					{
						for (int k = xmin; k < xmax; k++)
						{
							GridNode gridNode = this.nodes[j * this.Width + k];
							if (gridNode.Walkable)
							{
								bool flag = false;
								for (int l = 0; l < 4; l++)
								{
									if (!this.HasNodeConnection(gridNode, l))
									{
										flag = true;
										break;
									}
								}
								if (flag)
								{
									gridNode.Walkable = false;
								}
							}
						}
					}
					for (int m = zmin; m < zmax; m++)
					{
						for (int n = xmin; n < xmax; n++)
						{
							GridNode node = this.nodes[m * this.Width + n];
							this.CalculateConnections(this.nodes, n, m, node);
						}
					}
				}
			}
			else
			{
				if (this.erodeIterations + this.erosionFirstTag > 31)
				{
					Debug.LogError(string.Concat(new object[]
					{
						"Too few tags available for ",
						this.erodeIterations,
						" erode iterations and starting with tag ",
						this.erosionFirstTag,
						" (erodeIterations+erosionFirstTag > 31)"
					}));
					return;
				}
				if (this.erosionFirstTag <= 0)
				{
					Debug.LogError("First erosion tag must be greater or equal to 1");
					return;
				}
				for (int num = 0; num < this.erodeIterations; num++)
				{
					for (int num2 = zmin; num2 < zmax; num2++)
					{
						for (int num3 = xmin; num3 < xmax; num3++)
						{
							GridNode gridNode2 = this.nodes[num2 * this.width + num3];
							if (gridNode2.Walkable && (ulong)gridNode2.Tag >= (ulong)((long)this.erosionFirstTag) && (ulong)gridNode2.Tag < (ulong)((long)(this.erosionFirstTag + num)))
							{
								for (int num4 = 0; num4 < 4; num4++)
								{
									GridNode nodeConnection = this.GetNodeConnection(gridNode2, num4);
									if (nodeConnection != null)
									{
										uint tag = nodeConnection.Tag;
										if ((ulong)tag > (ulong)((long)(this.erosionFirstTag + num)) || (ulong)tag < (ulong)((long)this.erosionFirstTag))
										{
											nodeConnection.Tag = (uint)(this.erosionFirstTag + num);
										}
									}
								}
							}
							else if (gridNode2.Walkable && num == 0)
							{
								bool flag2 = false;
								for (int num5 = 0; num5 < 4; num5++)
								{
									if (!this.HasNodeConnection(gridNode2, num5))
									{
										flag2 = true;
										break;
									}
								}
								if (flag2)
								{
									gridNode2.Tag = (uint)(this.erosionFirstTag + num);
								}
							}
						}
					}
				}
			}
		}

		public virtual bool IsValidConnection(GridNode n1, GridNode n2)
		{
			return n1.Walkable && n2.Walkable && (this.maxClimb == 0f || (float)Mathf.Abs(n1.position[this.maxClimbAxis] - n2.position[this.maxClimbAxis]) <= this.maxClimb * 1000f);
		}

		public static void CalculateConnections(GridNode node)
		{
			GridGraph gridGraph = AstarData.GetGraph(node) as GridGraph;
			if (gridGraph != null)
			{
				int nodeInGridIndex = node.NodeInGridIndex;
				int x = nodeInGridIndex % gridGraph.width;
				int z = nodeInGridIndex / gridGraph.width;
				gridGraph.CalculateConnections(gridGraph.nodes, x, z, node);
			}
		}

		public virtual void CalculateConnections(GridNode[] nodes, int x, int z, GridNode node)
		{
			node.ResetConnectionsInternal();
			if (!node.Walkable)
			{
				return;
			}
			int nodeInGridIndex = node.NodeInGridIndex;
			if (this.corners == null)
			{
				this.corners = new int[4];
			}
			else
			{
				for (int i = 0; i < 4; i++)
				{
					this.corners[i] = 0;
				}
			}
			int j = 0;
			int num = 3;
			while (j < 4)
			{
				int num2 = x + this.neighbourXOffsets[j];
				int num3 = z + this.neighbourZOffsets[j];
				if (num2 >= 0 && num3 >= 0 && num2 < this.width && num3 < this.depth)
				{
					GridNode n = nodes[nodeInGridIndex + this.neighbourOffsets[j]];
					if (this.IsValidConnection(node, n))
					{
						node.SetConnectionInternal(j, true);
						this.corners[j]++;
						this.corners[num]++;
					}
					else
					{
						node.SetConnectionInternal(j, false);
					}
				}
				num = j;
				j++;
			}
			if (this.neighbours == NumNeighbours.Eight)
			{
				if (this.cutCorners)
				{
					for (int k = 0; k < 4; k++)
					{
						if (this.corners[k] >= 1)
						{
							int num4 = x + this.neighbourXOffsets[k + 4];
							int num5 = z + this.neighbourZOffsets[k + 4];
							if (num4 >= 0 && num5 >= 0 && num4 < this.width && num5 < this.depth)
							{
								GridNode n2 = nodes[nodeInGridIndex + this.neighbourOffsets[k + 4]];
								node.SetConnectionInternal(k + 4, this.IsValidConnection(node, n2));
							}
						}
					}
				}
				else
				{
					for (int l = 0; l < 4; l++)
					{
						if (this.corners[l] == 2)
						{
							GridNode n3 = nodes[nodeInGridIndex + this.neighbourOffsets[l + 4]];
							node.SetConnectionInternal(l + 4, this.IsValidConnection(node, n3));
						}
					}
				}
			}
		}

		public void OnPostScan(AstarPath script)
		{
			AstarPath.OnPostScan = (OnScanDelegate)Delegate.Remove(AstarPath.OnPostScan, new OnScanDelegate(this.OnPostScan));
			if (!this.autoLinkGrids || this.autoLinkDistLimit <= 0f)
			{
				return;
			}
			throw new NotSupportedException();
		}

		public override void OnDrawGizmos(bool drawNodes)
		{
			Gizmos.matrix = this.boundsMatrix;
			Gizmos.color = Color.white;
			Gizmos.DrawWireCube(Vector3.zero, new Vector3(this.size.x, 0f, this.size.y));
			Gizmos.matrix = Matrix4x4.identity;
			if (!drawNodes)
			{
				return;
			}
			if (this.nodes == null || this.depth * this.width != this.nodes.Length)
			{
				return;
			}
			PathHandler debugPathData = AstarPath.active.debugPathData;
			for (int i = 0; i < this.depth; i++)
			{
				for (int j = 0; j < this.width; j++)
				{
					GridNode gridNode = this.nodes[i * this.width + j];
					if (gridNode.Walkable)
					{
						Gizmos.color = this.NodeColor(gridNode, AstarPath.active.debugPathData);
						if (AstarPath.active.showSearchTree && debugPathData != null)
						{
							if (NavGraph.InSearchTree(gridNode, AstarPath.active.debugPath))
							{
								PathNode pathNode = debugPathData.GetPathNode(gridNode);
								if (pathNode != null && pathNode.parent != null)
								{
									Gizmos.DrawLine((Vector3)gridNode.position, (Vector3)pathNode.parent.node.position);
								}
							}
						}
						else
						{
							for (int k = 0; k < 8; k++)
							{
								GridNode nodeConnection = this.GetNodeConnection(gridNode, k);
								if (nodeConnection != null)
								{
									Gizmos.DrawLine((Vector3)gridNode.position, (Vector3)nodeConnection.position);
								}
							}
							if (gridNode.connections != null)
							{
								for (int l = 0; l < gridNode.connections.Length; l++)
								{
									GraphNode graphNode = gridNode.connections[l];
									Gizmos.DrawLine((Vector3)gridNode.position, (Vector3)graphNode.position);
								}
							}
						}
					}
				}
			}
		}

		public void GetBoundsMinMax(Bounds b, Matrix4x4 matrix, out Vector3 min, out Vector3 max)
		{
			Vector3[] array = new Vector3[]
			{
				matrix.MultiplyPoint3x4(b.center + new Vector3(b.extents.x, b.extents.y, b.extents.z)),
				matrix.MultiplyPoint3x4(b.center + new Vector3(b.extents.x, b.extents.y, -b.extents.z)),
				matrix.MultiplyPoint3x4(b.center + new Vector3(b.extents.x, -b.extents.y, b.extents.z)),
				matrix.MultiplyPoint3x4(b.center + new Vector3(b.extents.x, -b.extents.y, -b.extents.z)),
				matrix.MultiplyPoint3x4(b.center + new Vector3(-b.extents.x, b.extents.y, b.extents.z)),
				matrix.MultiplyPoint3x4(b.center + new Vector3(-b.extents.x, b.extents.y, -b.extents.z)),
				matrix.MultiplyPoint3x4(b.center + new Vector3(-b.extents.x, -b.extents.y, b.extents.z)),
				matrix.MultiplyPoint3x4(b.center + new Vector3(-b.extents.x, -b.extents.y, -b.extents.z))
			};
			min = array[0];
			max = array[0];
			for (int i = 1; i < 8; i++)
			{
				min = Vector3.Min(min, array[i]);
				max = Vector3.Max(max, array[i]);
			}
		}

		public List<GraphNode> GetNodesInArea(Bounds b)
		{
			return this.GetNodesInArea(b, null);
		}

		public List<GraphNode> GetNodesInArea(GraphUpdateShape shape)
		{
			return this.GetNodesInArea(shape.GetBounds(), shape);
		}

		private List<GraphNode> GetNodesInArea(Bounds b, GraphUpdateShape shape)
		{
			if (this.nodes == null || this.width * this.depth != this.nodes.Length)
			{
				return null;
			}
			List<GraphNode> list = ListPool<GraphNode>.Claim();
			Vector3 vector;
			Vector3 vector2;
			this.GetBoundsMinMax(b, this.inverseMatrix, out vector, out vector2);
			int xmin = Mathf.RoundToInt(vector.x - 0.5f);
			int xmax = Mathf.RoundToInt(vector2.x - 0.5f);
			int ymin = Mathf.RoundToInt(vector.z - 0.5f);
			int ymax = Mathf.RoundToInt(vector2.z - 0.5f);
			IntRect a = new IntRect(xmin, ymin, xmax, ymax);
			IntRect b2 = new IntRect(0, 0, this.width - 1, this.depth - 1);
			IntRect intRect = IntRect.Intersection(a, b2);
			for (int i = intRect.xmin; i <= intRect.xmax; i++)
			{
				for (int j = intRect.ymin; j <= intRect.ymax; j++)
				{
					int num = j * this.width + i;
					GraphNode graphNode = this.nodes[num];
					if (b.Contains((Vector3)graphNode.position) && (shape == null || shape.Contains((Vector3)graphNode.position)))
					{
						list.Add(graphNode);
					}
				}
			}
			return list;
		}

		public GraphUpdateThreading CanUpdateAsync(GraphUpdateObject o)
		{
			return GraphUpdateThreading.UnityThread;
		}

		public void UpdateAreaInit(GraphUpdateObject o)
		{
		}

		public void UpdateArea(GraphUpdateObject o)
		{
			if (this.nodes == null || this.nodes.Length != this.width * this.depth)
			{
				Debug.LogWarning("The Grid Graph is not scanned, cannot update area ");
				return;
			}
			Bounds b = o.bounds;
			Vector3 a;
			Vector3 a2;
			this.GetBoundsMinMax(b, this.inverseMatrix, out a, out a2);
			int xmin = Mathf.RoundToInt(a.x - 0.5f);
			int xmax = Mathf.RoundToInt(a2.x - 0.5f);
			int ymin = Mathf.RoundToInt(a.z - 0.5f);
			int ymax = Mathf.RoundToInt(a2.z - 0.5f);
			IntRect intRect = new IntRect(xmin, ymin, xmax, ymax);
			IntRect intRect2 = intRect;
			IntRect b2 = new IntRect(0, 0, this.width - 1, this.depth - 1);
			IntRect intRect3 = intRect;
			int num = (!o.updateErosion) ? 0 : this.erodeIterations;
			bool flag = o.updatePhysics || o.modifyWalkability;
			if (o.updatePhysics && !o.modifyWalkability && this.collision.collisionCheck)
			{
				Vector3 a3 = new Vector3(this.collision.diameter, 0f, this.collision.diameter) * 0.5f;
				a -= a3 * 1.02f;
				a2 += a3 * 1.02f;
				intRect3 = new IntRect(Mathf.RoundToInt(a.x - 0.5f), Mathf.RoundToInt(a.z - 0.5f), Mathf.RoundToInt(a2.x - 0.5f), Mathf.RoundToInt(a2.z - 0.5f));
				intRect2 = IntRect.Union(intRect3, intRect2);
			}
			if (flag || num > 0)
			{
				intRect2 = intRect2.Expand(num + 1);
			}
			IntRect intRect4 = IntRect.Intersection(intRect2, b2);
			for (int i = intRect4.xmin; i <= intRect4.xmax; i++)
			{
				for (int j = intRect4.ymin; j <= intRect4.ymax; j++)
				{
					o.WillUpdateNode(this.nodes[j * this.width + i]);
				}
			}
			if (o.updatePhysics && !o.modifyWalkability)
			{
				this.collision.Initialize(this.matrix, this.nodeSize);
				intRect4 = IntRect.Intersection(intRect3, b2);
				for (int k = intRect4.xmin; k <= intRect4.xmax; k++)
				{
					for (int l = intRect4.ymin; l <= intRect4.ymax; l++)
					{
						int num2 = l * this.width + k;
						GridNode node = this.nodes[num2];
						this.UpdateNodePositionCollision(node, k, l, o.resetPenaltyOnPhysics);
					}
				}
			}
			intRect4 = IntRect.Intersection(intRect, b2);
			for (int m = intRect4.xmin; m <= intRect4.xmax; m++)
			{
				for (int n = intRect4.ymin; n <= intRect4.ymax; n++)
				{
					int num3 = n * this.width + m;
					GridNode gridNode = this.nodes[num3];
					if (flag)
					{
						gridNode.Walkable = gridNode.WalkableErosion;
						if (o.bounds.Contains((Vector3)gridNode.position))
						{
							o.Apply(gridNode);
						}
						gridNode.WalkableErosion = gridNode.Walkable;
					}
					else if (o.bounds.Contains((Vector3)gridNode.position))
					{
						o.Apply(gridNode);
					}
				}
			}
			if (flag && num == 0)
			{
				intRect4 = IntRect.Intersection(intRect2, b2);
				for (int num4 = intRect4.xmin; num4 <= intRect4.xmax; num4++)
				{
					for (int num5 = intRect4.ymin; num5 <= intRect4.ymax; num5++)
					{
						int num6 = num5 * this.width + num4;
						GridNode node2 = this.nodes[num6];
						this.CalculateConnections(this.nodes, num4, num5, node2);
					}
				}
			}
			else if (flag && num > 0)
			{
				IntRect a4 = IntRect.Union(intRect, intRect3).Expand(num);
				IntRect a5 = a4.Expand(num);
				a4 = IntRect.Intersection(a4, b2);
				a5 = IntRect.Intersection(a5, b2);
				for (int num7 = a5.xmin; num7 <= a5.xmax; num7++)
				{
					for (int num8 = a5.ymin; num8 <= a5.ymax; num8++)
					{
						int num9 = num8 * this.width + num7;
						GridNode gridNode2 = this.nodes[num9];
						bool walkable = gridNode2.Walkable;
						gridNode2.Walkable = gridNode2.WalkableErosion;
						if (!a4.Contains(num7, num8))
						{
							gridNode2.TmpWalkable = walkable;
						}
					}
				}
				for (int num10 = a5.xmin; num10 <= a5.xmax; num10++)
				{
					for (int num11 = a5.ymin; num11 <= a5.ymax; num11++)
					{
						int num12 = num11 * this.width + num10;
						GridNode node3 = this.nodes[num12];
						this.CalculateConnections(this.nodes, num10, num11, node3);
					}
				}
				this.ErodeWalkableArea(a5.xmin, a5.ymin, a5.xmax + 1, a5.ymax + 1);
				for (int num13 = a5.xmin; num13 <= a5.xmax; num13++)
				{
					for (int num14 = a5.ymin; num14 <= a5.ymax; num14++)
					{
						if (!a4.Contains(num13, num14))
						{
							int num15 = num14 * this.width + num13;
							GridNode gridNode3 = this.nodes[num15];
							gridNode3.Walkable = gridNode3.TmpWalkable;
						}
					}
				}
				for (int num16 = a5.xmin; num16 <= a5.xmax; num16++)
				{
					for (int num17 = a5.ymin; num17 <= a5.ymax; num17++)
					{
						int num18 = num17 * this.width + num16;
						GridNode node4 = this.nodes[num18];
						this.CalculateConnections(this.nodes, num16, num17, node4);
					}
				}
			}
		}

		public bool Linecast(Vector3 _a, Vector3 _b)
		{
			GraphHitInfo graphHitInfo;
			return this.Linecast(_a, _b, null, out graphHitInfo);
		}

		public bool Linecast(Vector3 _a, Vector3 _b, GraphNode hint)
		{
			GraphHitInfo graphHitInfo;
			return this.Linecast(_a, _b, hint, out graphHitInfo);
		}

		public bool Linecast(Vector3 _a, Vector3 _b, GraphNode hint, out GraphHitInfo hit)
		{
			return this.Linecast(_a, _b, hint, out hit, null);
		}

		public bool Linecast(Vector3 _a, Vector3 _b, GraphNode hint, out GraphHitInfo hit, List<GraphNode> trace)
		{
			hit = default(GraphHitInfo);
			_a = this.inverseMatrix.MultiplyPoint3x4(_a);
			_a.x -= 0.5f;
			_a.z -= 0.5f;
			_b = this.inverseMatrix.MultiplyPoint3x4(_b);
			_b.x -= 0.5f;
			_b.z -= 0.5f;
			if (_a.x < -0.5f || _a.z < -0.5f || _a.x >= (float)this.width - 0.5f || _a.z >= (float)this.depth - 0.5f || _b.x < -0.5f || _b.z < -0.5f || _b.x >= (float)this.width - 0.5f || _b.z >= (float)this.depth - 0.5f)
			{
				Vector3 vector = new Vector3(-0.5f, 0f, -0.5f);
				Vector3 vector2 = new Vector3(-0.5f, 0f, (float)this.depth - 0.5f);
				Vector3 vector3 = new Vector3((float)this.width - 0.5f, 0f, (float)this.depth - 0.5f);
				Vector3 vector4 = new Vector3((float)this.width - 0.5f, 0f, -0.5f);
				int num = 0;
				bool flag = false;
				Vector3 vector5 = Polygon.SegmentIntersectionPoint(vector, vector2, _a, _b, out flag);
				if (flag)
				{
					num++;
					if (!Polygon.Left(vector, vector2, _a))
					{
						_a = vector5;
					}
					else
					{
						_b = vector5;
					}
				}
				vector5 = Polygon.SegmentIntersectionPoint(vector2, vector3, _a, _b, out flag);
				if (flag)
				{
					num++;
					if (!Polygon.Left(vector2, vector3, _a))
					{
						_a = vector5;
					}
					else
					{
						_b = vector5;
					}
				}
				vector5 = Polygon.SegmentIntersectionPoint(vector3, vector4, _a, _b, out flag);
				if (flag)
				{
					num++;
					if (!Polygon.Left(vector3, vector4, _a))
					{
						_a = vector5;
					}
					else
					{
						_b = vector5;
					}
				}
				vector5 = Polygon.SegmentIntersectionPoint(vector4, vector, _a, _b, out flag);
				if (flag)
				{
					num++;
					if (!Polygon.Left(vector4, vector, _a))
					{
						_a = vector5;
					}
					else
					{
						_b = vector5;
					}
				}
				if (num == 0)
				{
					return false;
				}
			}
			Vector3 a = _b - _a;
			float magnitude = a.magnitude;
			if (magnitude == 0f)
			{
				return false;
			}
			float num2 = 0.2f;
			float num3 = this.nodeSize * num2;
			num3 -= this.nodeSize * 0.02f;
			a = a / magnitude * num3;
			int num4 = (int)(magnitude / num3);
			Vector3 a2 = _a + a * this.nodeSize * 0.01f;
			GraphNode graphNode = null;
			for (int i = 0; i <= num4; i++)
			{
				Vector3 vector6 = a2 + a * (float)i;
				int num5 = Mathf.RoundToInt(vector6.x);
				int num6 = Mathf.RoundToInt(vector6.z);
				num5 = ((num5 >= 0) ? ((num5 < this.width) ? num5 : (this.width - 1)) : 0);
				num6 = ((num6 >= 0) ? ((num6 < this.depth) ? num6 : (this.depth - 1)) : 0);
				GraphNode graphNode2 = this.nodes[num6 * this.width + num5];
				if (graphNode2 != graphNode)
				{
					if (!graphNode2.Walkable)
					{
						if (i > 0)
						{
							hit.point = this.matrix.MultiplyPoint3x4(a2 + a * (float)(i - 1) + new Vector3(0.5f, 0f, 0.5f));
						}
						else
						{
							hit.point = this.matrix.MultiplyPoint3x4(_a + new Vector3(0.5f, 0f, 0.5f));
						}
						hit.origin = this.matrix.MultiplyPoint3x4(_a + new Vector3(0.5f, 0f, 0.5f));
						hit.node = graphNode2;
						return true;
					}
					if (i > num4 - 1 && (Mathf.Abs(vector6.x - _b.x) <= 0.50001f || Mathf.Abs(vector6.z - _b.z) <= 0.50001f))
					{
						return false;
					}
					if (trace != null)
					{
						trace.Add(graphNode2);
					}
					graphNode = graphNode2;
				}
			}
			return false;
		}

		public bool SnappedLinecast(Vector3 _a, Vector3 _b, GraphNode hint, out GraphHitInfo hit)
		{
			hit = default(GraphHitInfo);
			GraphNode node = base.GetNearest(_a, NNConstraint.None).node;
			GraphNode node2 = base.GetNearest(_b, NNConstraint.None).node;
			_a = this.inverseMatrix.MultiplyPoint3x4((Vector3)node.position);
			_a.x -= 0.5f;
			_a.z -= 0.5f;
			_b = this.inverseMatrix.MultiplyPoint3x4((Vector3)node2.position);
			_b.x -= 0.5f;
			_b.z -= 0.5f;
			Int3 @int = new Int3(Mathf.RoundToInt(_a.x), Mathf.RoundToInt(_a.y), Mathf.RoundToInt(_a.z));
			Int3 int2 = new Int3(Mathf.RoundToInt(_b.x), Mathf.RoundToInt(_b.y), Mathf.RoundToInt(_b.z));
			hit.origin = (Vector3)@int;
			if (!this.nodes[@int.z * this.width + @int.x].Walkable)
			{
				hit.node = this.nodes[@int.z * this.width + @int.x];
				hit.point = this.matrix.MultiplyPoint3x4(new Vector3((float)@int.x + 0.5f, 0f, (float)@int.z + 0.5f));
				hit.point.y = ((Vector3)hit.node.position).y;
				return true;
			}
			int num = Mathf.Abs(@int.x - int2.x);
			int num2 = Mathf.Abs(@int.z - int2.z);
			int num3;
			if (@int.x < int2.x)
			{
				num3 = 1;
			}
			else
			{
				num3 = -1;
			}
			int num4;
			if (@int.z < int2.z)
			{
				num4 = 1;
			}
			else
			{
				num4 = -1;
			}
			int num5 = num - num2;
			while (@int.x != int2.x || @int.z != int2.z)
			{
				int num6 = num5 * 2;
				int num7 = 0;
				Int3 int3 = @int;
				if (num6 > -num2)
				{
					num5 -= num2;
					num7 = num3;
					int3.x += num3;
				}
				if (num6 < num)
				{
					num5 += num;
					num7 += this.width * num4;
					int3.z += num4;
				}
				if (num7 == 0)
				{
					Debug.LogError("Offset is zero, this should not happen");
					return false;
				}
				int i = 0;
				while (i < this.neighbourOffsets.Length)
				{
					if (this.neighbourOffsets[i] == num7)
					{
						if (!this.CheckConnection(this.nodes[@int.z * this.width + @int.x], i))
						{
							hit.node = this.nodes[@int.z * this.width + @int.x];
							hit.point = this.matrix.MultiplyPoint3x4(new Vector3((float)@int.x + 0.5f, 0f, (float)@int.z + 0.5f));
							hit.point.y = ((Vector3)hit.node.position).y;
							return true;
						}
						if (!this.nodes[int3.z * this.width + int3.x].Walkable)
						{
							hit.node = this.nodes[@int.z * this.width + @int.x];
							hit.point = this.matrix.MultiplyPoint3x4(new Vector3((float)@int.x + 0.5f, 0f, (float)@int.z + 0.5f));
							hit.point.y = ((Vector3)hit.node.position).y;
							return true;
						}
						@int = int3;
						break;
					}
					else
					{
						i++;
					}
				}
			}
			return false;
		}

		public bool CheckConnection(GridNode node, int dir)
		{
			if (this.neighbours == NumNeighbours.Eight)
			{
				return this.HasNodeConnection(node, dir);
			}
			int num = dir - 4 - 1 & 3;
			int num2 = dir - 4 + 1 & 3;
			if (!this.HasNodeConnection(node, num) || !this.HasNodeConnection(node, num2))
			{
				return false;
			}
			GridNode gridNode = this.nodes[node.NodeInGridIndex + this.neighbourOffsets[num]];
			GridNode gridNode2 = this.nodes[node.NodeInGridIndex + this.neighbourOffsets[num2]];
			return gridNode.Walkable && gridNode2.Walkable && this.HasNodeConnection(gridNode2, num) && this.HasNodeConnection(gridNode, num2);
		}

		public override void SerializeExtraInfo(GraphSerializationContext ctx)
		{
			if (this.nodes == null)
			{
				ctx.writer.Write(-1);
				return;
			}
			ctx.writer.Write(this.nodes.Length);
			for (int i = 0; i < this.nodes.Length; i++)
			{
				this.nodes[i].SerializeNode(ctx);
			}
			ctx.writer.Write(this.matrix.m00);
			ctx.writer.Write(this.matrix.m01);
			ctx.writer.Write(this.matrix.m02);
			ctx.writer.Write(this.matrix.m03);
			ctx.writer.Write(this.matrix.m10);
			ctx.writer.Write(this.matrix.m11);
			ctx.writer.Write(this.matrix.m12);
			ctx.writer.Write(this.matrix.m13);
			ctx.writer.Write(this.matrix.m20);
			ctx.writer.Write(this.matrix.m21);
			ctx.writer.Write(this.matrix.m22);
			ctx.writer.Write(this.matrix.m23);
			ctx.writer.Write(this.matrix.m30);
			ctx.writer.Write(this.matrix.m31);
			ctx.writer.Write(this.matrix.m32);
			ctx.writer.Write(this.matrix.m33);
			ctx.writer.Write(this.inverseMatrix.m00);
			ctx.writer.Write(this.inverseMatrix.m01);
			ctx.writer.Write(this.inverseMatrix.m02);
			ctx.writer.Write(this.inverseMatrix.m03);
			ctx.writer.Write(this.inverseMatrix.m10);
			ctx.writer.Write(this.inverseMatrix.m11);
			ctx.writer.Write(this.inverseMatrix.m12);
			ctx.writer.Write(this.inverseMatrix.m13);
			ctx.writer.Write(this.inverseMatrix.m20);
			ctx.writer.Write(this.inverseMatrix.m21);
			ctx.writer.Write(this.inverseMatrix.m22);
			ctx.writer.Write(this.inverseMatrix.m23);
			ctx.writer.Write(this.inverseMatrix.m30);
			ctx.writer.Write(this.inverseMatrix.m31);
			ctx.writer.Write(this.inverseMatrix.m32);
			ctx.writer.Write(this.inverseMatrix.m33);
		}

		public override void DeserializeExtraInfo(GraphSerializationContext ctx)
		{
			int num = ctx.reader.ReadInt32();
			if (num == -1)
			{
				this.nodes = null;
				return;
			}
			this.nodes = new GridNode[num];
			for (int i = 0; i < this.nodes.Length; i++)
			{
				this.nodes[i] = new GridNode(this.active);
				this.nodes[i].DeserializeNode(ctx);
			}
		}

		public override void PostDeserialization()
		{
			this.GenerateMatrix();
			this.SetUpOffsetsAndCosts();
			if (this.nodes == null || this.nodes.Length == 0)
			{
				return;
			}
			if (this.width * this.depth != this.nodes.Length)
			{
				Debug.LogWarning("Node data did not match with bounds data. Probably a change to the bounds/width/depth data was made after scanning the graph just prior to saving it. Nodes will be discarded");
				this.nodes = new GridNode[0];
				return;
			}
			GridNode.SetGridGraph(AstarPath.active.astarData.GetGraphIndex(this), this);
			for (int i = 0; i < this.depth; i++)
			{
				for (int j = 0; j < this.width; j++)
				{
					GridNode gridNode = this.nodes[i * this.width + j];
					if (gridNode == null)
					{
						Debug.LogError("Deserialization Error : Couldn't cast the node to the appropriate type - GridGenerator. Check the CreateNodes function");
						return;
					}
					gridNode.NodeInGridIndex = i * this.width + j;
				}
			}
		}
	}
}

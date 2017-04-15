using System;
using System.Collections.Generic;

namespace Pathfinding.ClipperLib
{
	public class Clipper : ClipperBase
	{
		private class PolyOffsetBuilder
		{
			private const int m_buffLength = 128;

			private List<List<IntPoint>> m_p;

			private List<IntPoint> currentPoly;

			private List<DoublePoint> normals = new List<DoublePoint>();

			private double m_delta;

			private double m_sinA;

			private double m_sin;

			private double m_cos;

			private double m_miterLim;

			private double m_Steps360;

			private int m_i;

			private int m_j;

			private int m_k;

			public PolyOffsetBuilder(List<List<IntPoint>> pts, out List<List<IntPoint>> solution, double delta, JoinType jointype, EndType endtype, double limit = 0.0)
			{
				solution = new List<List<IntPoint>>();
				if (ClipperBase.near_zero(delta))
				{
					solution = pts;
					return;
				}
				this.m_p = pts;
				if (endtype != EndType.etClosed && delta < 0.0)
				{
					delta = -delta;
				}
				this.m_delta = delta;
				if (jointype == JoinType.jtMiter)
				{
					if (limit > 2.0)
					{
						this.m_miterLim = 2.0 / (limit * limit);
					}
					else
					{
						this.m_miterLim = 0.5;
					}
					if (endtype == EndType.etRound)
					{
						limit = 0.25;
					}
				}
				if (jointype == JoinType.jtRound || endtype == EndType.etRound)
				{
					if (limit <= 0.0)
					{
						limit = 0.25;
					}
					else if (limit > Math.Abs(delta) * 0.25)
					{
						limit = Math.Abs(delta) * 0.25;
					}
					this.m_Steps360 = 3.1415926535897931 / Math.Acos(1.0 - limit / Math.Abs(delta));
					this.m_sin = Math.Sin(6.2831853071795862 / this.m_Steps360);
					this.m_cos = Math.Cos(6.2831853071795862 / this.m_Steps360);
					this.m_Steps360 /= 6.2831853071795862;
					if (delta < 0.0)
					{
						this.m_sin = -this.m_sin;
					}
				}
				double num = delta * delta;
				solution.Capacity = pts.Count;
				this.m_i = 0;
				while (this.m_i < pts.Count)
				{
					int count = pts[this.m_i].Count;
					if (count != 0 && (count >= 3 || delta > 0.0))
					{
						if (count == 1)
						{
							if (jointype == JoinType.jtRound)
							{
								double num2 = 1.0;
								double num3 = 0.0;
								for (long num4 = 1L; num4 <= Clipper.Round(this.m_Steps360 * 2.0 * 3.1415926535897931); num4 += 1L)
								{
									this.AddPoint(new IntPoint(Clipper.Round((double)this.m_p[this.m_i][0].X + num2 * delta), Clipper.Round((double)this.m_p[this.m_i][0].Y + num3 * delta)));
									double num5 = num2;
									num2 = num2 * this.m_cos - this.m_sin * num3;
									num3 = num5 * this.m_sin + num3 * this.m_cos;
								}
							}
							else
							{
								double num6 = -1.0;
								double num7 = -1.0;
								for (int i = 0; i < 4; i++)
								{
									this.AddPoint(new IntPoint(Clipper.Round((double)this.m_p[this.m_i][0].X + num6 * delta), Clipper.Round((double)this.m_p[this.m_i][0].Y + num7 * delta)));
									if (num6 < 0.0)
									{
										num6 = 1.0;
									}
									else if (num7 < 0.0)
									{
										num7 = 1.0;
									}
									else
									{
										num6 = -1.0;
									}
								}
							}
						}
						else
						{
							this.normals.Clear();
							this.normals.Capacity = count;
							for (int j = 0; j < count - 1; j++)
							{
								this.normals.Add(Clipper.GetUnitNormal(pts[this.m_i][j], pts[this.m_i][j + 1]));
							}
							if (endtype == EndType.etClosed)
							{
								this.normals.Add(Clipper.GetUnitNormal(pts[this.m_i][count - 1], pts[this.m_i][0]));
							}
							else
							{
								this.normals.Add(new DoublePoint(this.normals[count - 2]));
							}
							this.currentPoly = new List<IntPoint>();
							if (endtype == EndType.etClosed)
							{
								this.m_k = count - 1;
								this.m_j = 0;
								while (this.m_j < count)
								{
									this.OffsetPoint(jointype);
									this.m_j++;
								}
								solution.Add(this.currentPoly);
							}
							else
							{
								this.m_k = 0;
								this.m_j = 1;
								while (this.m_j < count - 1)
								{
									this.OffsetPoint(jointype);
									this.m_j++;
								}
								if (endtype == EndType.etButt)
								{
									this.m_j = count - 1;
									IntPoint pt = new IntPoint(Clipper.Round((double)pts[this.m_i][this.m_j].X + this.normals[this.m_j].X * delta), Clipper.Round((double)pts[this.m_i][this.m_j].Y + this.normals[this.m_j].Y * delta));
									this.AddPoint(pt);
									pt = new IntPoint(Clipper.Round((double)pts[this.m_i][this.m_j].X - this.normals[this.m_j].X * delta), Clipper.Round((double)pts[this.m_i][this.m_j].Y - this.normals[this.m_j].Y * delta));
									this.AddPoint(pt);
								}
								else
								{
									this.m_j = count - 1;
									this.m_k = count - 2;
									this.m_sinA = 0.0;
									this.normals[this.m_j] = new DoublePoint(-this.normals[this.m_j].X, -this.normals[this.m_j].Y);
									if (endtype == EndType.etSquare)
									{
										this.DoSquare();
									}
									else
									{
										this.DoRound();
									}
								}
								for (int k = count - 1; k > 0; k--)
								{
									this.normals[k] = new DoublePoint(-this.normals[k - 1].X, -this.normals[k - 1].Y);
								}
								this.normals[0] = new DoublePoint(-this.normals[1].X, -this.normals[1].Y);
								this.m_k = count - 1;
								this.m_j = this.m_k - 1;
								while (this.m_j > 0)
								{
									this.OffsetPoint(jointype);
									this.m_j--;
								}
								if (endtype == EndType.etButt)
								{
									IntPoint pt = new IntPoint(Clipper.Round((double)pts[this.m_i][0].X - this.normals[0].X * delta), Clipper.Round((double)pts[this.m_i][0].Y - this.normals[0].Y * delta));
									this.AddPoint(pt);
									pt = new IntPoint(Clipper.Round((double)pts[this.m_i][0].X + this.normals[0].X * delta), Clipper.Round((double)pts[this.m_i][0].Y + this.normals[0].Y * delta));
									this.AddPoint(pt);
								}
								else
								{
									this.m_k = 1;
									this.m_sinA = 0.0;
									if (endtype == EndType.etSquare)
									{
										this.DoSquare();
									}
									else
									{
										this.DoRound();
									}
								}
								solution.Add(this.currentPoly);
							}
						}
					}
					this.m_i++;
				}
				Clipper clipper = new Clipper(0);
				clipper.AddPaths(solution, PolyType.ptSubject, true);
				if (delta > 0.0)
				{
					clipper.Execute(ClipType.ctUnion, solution, PolyFillType.pftPositive, PolyFillType.pftPositive);
				}
				else
				{
					IntRect bounds = clipper.GetBounds();
					clipper.AddPath(new List<IntPoint>(4)
					{
						new IntPoint(bounds.left - 10L, bounds.bottom + 10L),
						new IntPoint(bounds.right + 10L, bounds.bottom + 10L),
						new IntPoint(bounds.right + 10L, bounds.top - 10L),
						new IntPoint(bounds.left - 10L, bounds.top - 10L)
					}, PolyType.ptSubject, true);
					clipper.ReverseSolution = true;
					clipper.Execute(ClipType.ctUnion, solution, PolyFillType.pftNegative, PolyFillType.pftNegative);
					if (solution.Count > 0)
					{
						solution.RemoveAt(0);
					}
				}
			}

			private void OffsetPoint(JoinType jointype)
			{
				this.m_sinA = this.normals[this.m_k].X * this.normals[this.m_j].Y - this.normals[this.m_j].X * this.normals[this.m_k].Y;
				if (this.m_sinA < 5E-05 && this.m_sinA > -5E-05)
				{
					return;
				}
				if (this.m_sinA > 1.0)
				{
					this.m_sinA = 1.0;
				}
				else if (this.m_sinA < -1.0)
				{
					this.m_sinA = -1.0;
				}
				if (this.m_sinA * this.m_delta < 0.0)
				{
					this.AddPoint(new IntPoint(Clipper.Round((double)this.m_p[this.m_i][this.m_j].X + this.normals[this.m_k].X * this.m_delta), Clipper.Round((double)this.m_p[this.m_i][this.m_j].Y + this.normals[this.m_k].Y * this.m_delta)));
					this.AddPoint(this.m_p[this.m_i][this.m_j]);
					this.AddPoint(new IntPoint(Clipper.Round((double)this.m_p[this.m_i][this.m_j].X + this.normals[this.m_j].X * this.m_delta), Clipper.Round((double)this.m_p[this.m_i][this.m_j].Y + this.normals[this.m_j].Y * this.m_delta)));
				}
				else
				{
					switch (jointype)
					{
					case JoinType.jtSquare:
						this.DoSquare();
						break;
					case JoinType.jtRound:
						this.DoRound();
						break;
					case JoinType.jtMiter:
					{
						double num = 1.0 + (this.normals[this.m_j].X * this.normals[this.m_k].X + this.normals[this.m_j].Y * this.normals[this.m_k].Y);
						if (num >= this.m_miterLim)
						{
							this.DoMiter(num);
						}
						else
						{
							this.DoSquare();
						}
						break;
					}
					}
				}
				this.m_k = this.m_j;
			}

			internal void AddPoint(IntPoint pt)
			{
				if (this.currentPoly.Count == this.currentPoly.Capacity)
				{
					this.currentPoly.Capacity += 128;
				}
				this.currentPoly.Add(pt);
			}

			internal void DoSquare()
			{
				double num = Math.Tan(Math.Atan2(this.m_sinA, this.normals[this.m_k].X * this.normals[this.m_j].X + this.normals[this.m_k].Y * this.normals[this.m_j].Y) / 4.0);
				this.AddPoint(new IntPoint(Clipper.Round((double)this.m_p[this.m_i][this.m_j].X + this.m_delta * (this.normals[this.m_k].X - this.normals[this.m_k].Y * num)), Clipper.Round((double)this.m_p[this.m_i][this.m_j].Y + this.m_delta * (this.normals[this.m_k].Y + this.normals[this.m_k].X * num))));
				this.AddPoint(new IntPoint(Clipper.Round((double)this.m_p[this.m_i][this.m_j].X + this.m_delta * (this.normals[this.m_j].X + this.normals[this.m_j].Y * num)), Clipper.Round((double)this.m_p[this.m_i][this.m_j].Y + this.m_delta * (this.normals[this.m_j].Y - this.normals[this.m_j].X * num))));
			}

			internal void DoMiter(double r)
			{
				double num = this.m_delta / r;
				this.AddPoint(new IntPoint(Clipper.Round((double)this.m_p[this.m_i][this.m_j].X + (this.normals[this.m_k].X + this.normals[this.m_j].X) * num), Clipper.Round((double)this.m_p[this.m_i][this.m_j].Y + (this.normals[this.m_k].Y + this.normals[this.m_j].Y) * num)));
			}

			internal void DoRound()
			{
				double value = Math.Atan2(this.m_sinA, this.normals[this.m_k].X * this.normals[this.m_j].X + this.normals[this.m_k].Y * this.normals[this.m_j].Y);
				int num = (int)Clipper.Round(this.m_Steps360 * Math.Abs(value));
				double num2 = this.normals[this.m_k].X;
				double num3 = this.normals[this.m_k].Y;
				for (int i = 0; i < num; i++)
				{
					this.AddPoint(new IntPoint(Clipper.Round((double)this.m_p[this.m_i][this.m_j].X + num2 * this.m_delta), Clipper.Round((double)this.m_p[this.m_i][this.m_j].Y + num3 * this.m_delta)));
					double num4 = num2;
					num2 = num2 * this.m_cos - this.m_sin * num3;
					num3 = num4 * this.m_sin + num3 * this.m_cos;
				}
				this.AddPoint(new IntPoint(Clipper.Round((double)this.m_p[this.m_i][this.m_j].X + this.normals[this.m_j].X * this.m_delta), Clipper.Round((double)this.m_p[this.m_i][this.m_j].Y + this.normals[this.m_j].Y * this.m_delta)));
			}
		}

		internal enum NodeType
		{
			ntAny,
			ntOpen,
			ntClosed
		}

		public const int ioReverseSolution = 1;

		public const int ioStrictlySimple = 2;

		public const int ioPreserveCollinear = 4;

		private List<OutRec> m_PolyOuts;

		private ClipType m_ClipType;

		private Scanbeam m_Scanbeam;

		private TEdge m_ActiveEdges;

		private TEdge m_SortedEdges;

		private IntersectNode m_IntersectNodes;

		private bool m_ExecuteLocked;

		private PolyFillType m_ClipFillType;

		private PolyFillType m_SubjFillType;

		private List<Join> m_Joins;

		private List<Join> m_GhostJoins;

		private bool m_UsingPolyTree;

		public bool ReverseSolution
		{
			get;
			set;
		}

		public bool StrictlySimple
		{
			get;
			set;
		}

		public Clipper(int InitOptions = 0)
		{
			this.m_Scanbeam = null;
			this.m_ActiveEdges = null;
			this.m_SortedEdges = null;
			this.m_IntersectNodes = null;
			this.m_ExecuteLocked = false;
			this.m_UsingPolyTree = false;
			this.m_PolyOuts = new List<OutRec>();
			this.m_Joins = new List<Join>();
			this.m_GhostJoins = new List<Join>();
			this.ReverseSolution = ((1 & InitOptions) != 0);
			this.StrictlySimple = ((2 & InitOptions) != 0);
			base.PreserveCollinear = ((4 & InitOptions) != 0);
		}

		public override void Clear()
		{
			if (this.m_edges.Count == 0)
			{
				return;
			}
			this.DisposeAllPolyPts();
			base.Clear();
		}

		private void DisposeScanbeamList()
		{
			while (this.m_Scanbeam != null)
			{
				Scanbeam next = this.m_Scanbeam.Next;
				this.m_Scanbeam = null;
				this.m_Scanbeam = next;
			}
		}

		protected override void Reset()
		{
			base.Reset();
			this.m_Scanbeam = null;
			this.m_ActiveEdges = null;
			this.m_SortedEdges = null;
			this.DisposeAllPolyPts();
			for (LocalMinima localMinima = this.m_MinimaList; localMinima != null; localMinima = localMinima.Next)
			{
				this.InsertScanbeam(localMinima.Y);
			}
		}

		private void InsertScanbeam(long Y)
		{
			if (this.m_Scanbeam == null)
			{
				this.m_Scanbeam = new Scanbeam();
				this.m_Scanbeam.Next = null;
				this.m_Scanbeam.Y = Y;
			}
			else if (Y > this.m_Scanbeam.Y)
			{
				this.m_Scanbeam = new Scanbeam
				{
					Y = Y,
					Next = this.m_Scanbeam
				};
			}
			else
			{
				Scanbeam scanbeam = this.m_Scanbeam;
				while (scanbeam.Next != null && Y <= scanbeam.Next.Y)
				{
					scanbeam = scanbeam.Next;
				}
				if (Y == scanbeam.Y)
				{
					return;
				}
				scanbeam.Next = new Scanbeam
				{
					Y = Y,
					Next = scanbeam.Next
				};
			}
		}

		public bool Execute(ClipType clipType, List<List<IntPoint>> solution, PolyFillType subjFillType, PolyFillType clipFillType)
		{
			if (this.m_ExecuteLocked)
			{
				return false;
			}
			if (this.m_HasOpenPaths)
			{
				throw new ClipperException("Error: PolyTree struct is need for open path clipping.");
			}
			this.m_ExecuteLocked = true;
			solution.Clear();
			this.m_SubjFillType = subjFillType;
			this.m_ClipFillType = clipFillType;
			this.m_ClipType = clipType;
			this.m_UsingPolyTree = false;
			bool flag = this.ExecuteInternal();
			if (flag)
			{
				this.BuildResult(solution);
			}
			this.m_ExecuteLocked = false;
			return flag;
		}

		public bool Execute(ClipType clipType, PolyTree polytree, PolyFillType subjFillType, PolyFillType clipFillType)
		{
			if (this.m_ExecuteLocked)
			{
				return false;
			}
			this.m_ExecuteLocked = true;
			this.m_SubjFillType = subjFillType;
			this.m_ClipFillType = clipFillType;
			this.m_ClipType = clipType;
			this.m_UsingPolyTree = true;
			bool flag = this.ExecuteInternal();
			if (flag)
			{
				this.BuildResult2(polytree);
			}
			this.m_ExecuteLocked = false;
			return flag;
		}

		public bool Execute(ClipType clipType, List<List<IntPoint>> solution)
		{
			return this.Execute(clipType, solution, PolyFillType.pftEvenOdd, PolyFillType.pftEvenOdd);
		}

		public bool Execute(ClipType clipType, PolyTree polytree)
		{
			return this.Execute(clipType, polytree, PolyFillType.pftEvenOdd, PolyFillType.pftEvenOdd);
		}

		internal void FixHoleLinkage(OutRec outRec)
		{
			if (outRec.FirstLeft == null || (outRec.IsHole != outRec.FirstLeft.IsHole && outRec.FirstLeft.Pts != null))
			{
				return;
			}
			OutRec firstLeft = outRec.FirstLeft;
			while (firstLeft != null && (firstLeft.IsHole == outRec.IsHole || firstLeft.Pts == null))
			{
				firstLeft = firstLeft.FirstLeft;
			}
			outRec.FirstLeft = firstLeft;
		}

		private bool ExecuteInternal()
		{
			bool result;
			try
			{
				this.Reset();
				if (this.m_CurrentLM == null)
				{
					result = false;
				}
				else
				{
					long botY = this.PopScanbeam();
					do
					{
						this.InsertLocalMinimaIntoAEL(botY);
						this.m_GhostJoins.Clear();
						this.ProcessHorizontals(false);
						if (this.m_Scanbeam == null)
						{
							break;
						}
						long num = this.PopScanbeam();
						if (!this.ProcessIntersections(botY, num))
						{
							goto Block_4;
						}
						this.ProcessEdgesAtTopOfScanbeam(num);
						botY = num;
					}
					while (this.m_Scanbeam != null || this.m_CurrentLM != null);
					goto IL_82;
					Block_4:
					result = false;
					return result;
					IL_82:
					for (int i = 0; i < this.m_PolyOuts.Count; i++)
					{
						OutRec outRec = this.m_PolyOuts[i];
						if (outRec.Pts != null && !outRec.IsOpen)
						{
							if ((outRec.IsHole ^ this.ReverseSolution) == this.Area(outRec) > 0.0)
							{
								this.ReversePolyPtLinks(outRec.Pts);
							}
						}
					}
					this.JoinCommonEdges();
					for (int j = 0; j < this.m_PolyOuts.Count; j++)
					{
						OutRec outRec2 = this.m_PolyOuts[j];
						if (outRec2.Pts != null && !outRec2.IsOpen)
						{
							this.FixupOutPolygon(outRec2);
						}
					}
					if (this.StrictlySimple)
					{
						this.DoSimplePolygons();
					}
					result = true;
				}
			}
			finally
			{
				this.m_Joins.Clear();
				this.m_GhostJoins.Clear();
			}
			return result;
		}

		private long PopScanbeam()
		{
			long y = this.m_Scanbeam.Y;
			Scanbeam scanbeam = this.m_Scanbeam;
			this.m_Scanbeam = this.m_Scanbeam.Next;
			return y;
		}

		private void DisposeAllPolyPts()
		{
			for (int i = 0; i < this.m_PolyOuts.Count; i++)
			{
				this.DisposeOutRec(i);
			}
			this.m_PolyOuts.Clear();
		}

		private void DisposeOutRec(int index)
		{
			OutRec outRec = this.m_PolyOuts[index];
			if (outRec.Pts != null)
			{
				this.DisposeOutPts(outRec.Pts);
			}
			this.m_PolyOuts[index] = null;
		}

		private void DisposeOutPts(OutPt pp)
		{
			if (pp == null)
			{
				return;
			}
			pp.Prev.Next = null;
			while (pp != null)
			{
				pp = pp.Next;
			}
		}

		private void AddJoin(OutPt Op1, OutPt Op2, IntPoint OffPt)
		{
			Join join = new Join();
			join.OutPt1 = Op1;
			join.OutPt2 = Op2;
			join.OffPt = OffPt;
			this.m_Joins.Add(join);
		}

		private void AddGhostJoin(OutPt Op, IntPoint OffPt)
		{
			Join join = new Join();
			join.OutPt1 = Op;
			join.OffPt = OffPt;
			this.m_GhostJoins.Add(join);
		}

		private void InsertLocalMinimaIntoAEL(long botY)
		{
			while (this.m_CurrentLM != null && this.m_CurrentLM.Y == botY)
			{
				TEdge leftBound = this.m_CurrentLM.LeftBound;
				TEdge rightBound = this.m_CurrentLM.RightBound;
				base.PopLocalMinima();
				OutPt outPt = null;
				if (leftBound == null)
				{
					this.InsertEdgeIntoAEL(rightBound, null);
					this.SetWindingCount(rightBound);
					if (this.IsContributing(rightBound))
					{
						outPt = this.AddOutPt(rightBound, rightBound.Bot);
					}
				}
				else
				{
					this.InsertEdgeIntoAEL(leftBound, null);
					this.InsertEdgeIntoAEL(rightBound, leftBound);
					this.SetWindingCount(leftBound);
					rightBound.WindCnt = leftBound.WindCnt;
					rightBound.WindCnt2 = leftBound.WindCnt2;
					if (this.IsContributing(leftBound))
					{
						outPt = this.AddLocalMinPoly(leftBound, rightBound, leftBound.Bot);
					}
					this.InsertScanbeam(leftBound.Top.Y);
				}
				if (ClipperBase.IsHorizontal(rightBound))
				{
					this.AddEdgeToSEL(rightBound);
				}
				else
				{
					this.InsertScanbeam(rightBound.Top.Y);
				}
				if (leftBound != null)
				{
					if (outPt != null && ClipperBase.IsHorizontal(rightBound) && this.m_GhostJoins.Count > 0 && rightBound.WindDelta != 0)
					{
						for (int i = 0; i < this.m_GhostJoins.Count; i++)
						{
							Join join = this.m_GhostJoins[i];
							if (this.HorzSegmentsOverlap(join.OutPt1.Pt, join.OffPt, rightBound.Bot, rightBound.Top))
							{
								this.AddJoin(join.OutPt1, outPt, join.OffPt);
							}
						}
					}
					if (leftBound.OutIdx >= 0 && leftBound.PrevInAEL != null && leftBound.PrevInAEL.Curr.X == leftBound.Bot.X && leftBound.PrevInAEL.OutIdx >= 0 && ClipperBase.SlopesEqual(leftBound.PrevInAEL, leftBound, this.m_UseFullRange) && leftBound.WindDelta != 0 && leftBound.PrevInAEL.WindDelta != 0)
					{
						OutPt op = this.AddOutPt(leftBound.PrevInAEL, leftBound.Bot);
						this.AddJoin(outPt, op, leftBound.Top);
					}
					if (leftBound.NextInAEL != rightBound)
					{
						if (rightBound.OutIdx >= 0 && rightBound.PrevInAEL.OutIdx >= 0 && ClipperBase.SlopesEqual(rightBound.PrevInAEL, rightBound, this.m_UseFullRange) && rightBound.WindDelta != 0 && rightBound.PrevInAEL.WindDelta != 0)
						{
							OutPt op2 = this.AddOutPt(rightBound.PrevInAEL, rightBound.Bot);
							this.AddJoin(outPt, op2, rightBound.Top);
						}
						TEdge nextInAEL = leftBound.NextInAEL;
						if (nextInAEL != null)
						{
							while (nextInAEL != rightBound)
							{
								this.IntersectEdges(rightBound, nextInAEL, leftBound.Curr, false);
								nextInAEL = nextInAEL.NextInAEL;
							}
						}
					}
				}
			}
		}

		private void InsertEdgeIntoAEL(TEdge edge, TEdge startEdge)
		{
			if (this.m_ActiveEdges == null)
			{
				edge.PrevInAEL = null;
				edge.NextInAEL = null;
				this.m_ActiveEdges = edge;
			}
			else if (startEdge == null && this.E2InsertsBeforeE1(this.m_ActiveEdges, edge))
			{
				edge.PrevInAEL = null;
				edge.NextInAEL = this.m_ActiveEdges;
				this.m_ActiveEdges.PrevInAEL = edge;
				this.m_ActiveEdges = edge;
			}
			else
			{
				if (startEdge == null)
				{
					startEdge = this.m_ActiveEdges;
				}
				while (startEdge.NextInAEL != null && !this.E2InsertsBeforeE1(startEdge.NextInAEL, edge))
				{
					startEdge = startEdge.NextInAEL;
				}
				edge.NextInAEL = startEdge.NextInAEL;
				if (startEdge.NextInAEL != null)
				{
					startEdge.NextInAEL.PrevInAEL = edge;
				}
				edge.PrevInAEL = startEdge;
				startEdge.NextInAEL = edge;
			}
		}

		private bool E2InsertsBeforeE1(TEdge e1, TEdge e2)
		{
			if (e2.Curr.X != e1.Curr.X)
			{
				return e2.Curr.X < e1.Curr.X;
			}
			if (e2.Top.Y > e1.Top.Y)
			{
				return e2.Top.X < Clipper.TopX(e1, e2.Top.Y);
			}
			return e1.Top.X > Clipper.TopX(e2, e1.Top.Y);
		}

		private bool IsEvenOddFillType(TEdge edge)
		{
			if (edge.PolyTyp == PolyType.ptSubject)
			{
				return this.m_SubjFillType == PolyFillType.pftEvenOdd;
			}
			return this.m_ClipFillType == PolyFillType.pftEvenOdd;
		}

		private bool IsEvenOddAltFillType(TEdge edge)
		{
			if (edge.PolyTyp == PolyType.ptSubject)
			{
				return this.m_ClipFillType == PolyFillType.pftEvenOdd;
			}
			return this.m_SubjFillType == PolyFillType.pftEvenOdd;
		}

		private bool IsContributing(TEdge edge)
		{
			PolyFillType polyFillType;
			PolyFillType polyFillType2;
			if (edge.PolyTyp == PolyType.ptSubject)
			{
				polyFillType = this.m_SubjFillType;
				polyFillType2 = this.m_ClipFillType;
			}
			else
			{
				polyFillType = this.m_ClipFillType;
				polyFillType2 = this.m_SubjFillType;
			}
			switch (polyFillType)
			{
			case PolyFillType.pftEvenOdd:
				if (edge.WindDelta == 0 && edge.WindCnt != 1)
				{
					return false;
				}
				break;
			case PolyFillType.pftNonZero:
				if (Math.Abs(edge.WindCnt) != 1)
				{
					return false;
				}
				break;
			case PolyFillType.pftPositive:
				if (edge.WindCnt != 1)
				{
					return false;
				}
				break;
			default:
				if (edge.WindCnt != -1)
				{
					return false;
				}
				break;
			}
			switch (this.m_ClipType)
			{
			case ClipType.ctIntersection:
				switch (polyFillType2)
				{
				case PolyFillType.pftEvenOdd:
				case PolyFillType.pftNonZero:
					return edge.WindCnt2 != 0;
				case PolyFillType.pftPositive:
					return edge.WindCnt2 > 0;
				default:
					return edge.WindCnt2 < 0;
				}
				break;
			case ClipType.ctUnion:
				switch (polyFillType2)
				{
				case PolyFillType.pftEvenOdd:
				case PolyFillType.pftNonZero:
					return edge.WindCnt2 == 0;
				case PolyFillType.pftPositive:
					return edge.WindCnt2 <= 0;
				default:
					return edge.WindCnt2 >= 0;
				}
				break;
			case ClipType.ctDifference:
				if (edge.PolyTyp == PolyType.ptSubject)
				{
					switch (polyFillType2)
					{
					case PolyFillType.pftEvenOdd:
					case PolyFillType.pftNonZero:
						return edge.WindCnt2 == 0;
					case PolyFillType.pftPositive:
						return edge.WindCnt2 <= 0;
					default:
						return edge.WindCnt2 >= 0;
					}
				}
				else
				{
					switch (polyFillType2)
					{
					case PolyFillType.pftEvenOdd:
					case PolyFillType.pftNonZero:
						return edge.WindCnt2 != 0;
					case PolyFillType.pftPositive:
						return edge.WindCnt2 > 0;
					default:
						return edge.WindCnt2 < 0;
					}
				}
				break;
			case ClipType.ctXor:
				if (edge.WindDelta != 0)
				{
					return true;
				}
				switch (polyFillType2)
				{
				case PolyFillType.pftEvenOdd:
				case PolyFillType.pftNonZero:
					return edge.WindCnt2 == 0;
				case PolyFillType.pftPositive:
					return edge.WindCnt2 <= 0;
				default:
					return edge.WindCnt2 >= 0;
				}
				break;
			default:
				return true;
			}
		}

		private void SetWindingCount(TEdge edge)
		{
			TEdge tEdge = edge.PrevInAEL;
			while (tEdge != null && (tEdge.PolyTyp != edge.PolyTyp || tEdge.WindDelta == 0))
			{
				tEdge = tEdge.PrevInAEL;
			}
			if (tEdge == null)
			{
				edge.WindCnt = ((edge.WindDelta != 0) ? edge.WindDelta : 1);
				edge.WindCnt2 = 0;
				tEdge = this.m_ActiveEdges;
			}
			else if (edge.WindDelta == 0 && this.m_ClipType != ClipType.ctUnion)
			{
				edge.WindCnt = 1;
				edge.WindCnt2 = tEdge.WindCnt2;
				tEdge = tEdge.NextInAEL;
			}
			else if (this.IsEvenOddFillType(edge))
			{
				if (edge.WindDelta == 0)
				{
					bool flag = true;
					for (TEdge prevInAEL = tEdge.PrevInAEL; prevInAEL != null; prevInAEL = prevInAEL.PrevInAEL)
					{
						if (prevInAEL.PolyTyp == tEdge.PolyTyp && prevInAEL.WindDelta != 0)
						{
							flag = !flag;
						}
					}
					edge.WindCnt = ((!flag) ? 1 : 0);
				}
				else
				{
					edge.WindCnt = edge.WindDelta;
				}
				edge.WindCnt2 = tEdge.WindCnt2;
				tEdge = tEdge.NextInAEL;
			}
			else
			{
				if (tEdge.WindCnt * tEdge.WindDelta < 0)
				{
					if (Math.Abs(tEdge.WindCnt) > 1)
					{
						if (tEdge.WindDelta * edge.WindDelta < 0)
						{
							edge.WindCnt = tEdge.WindCnt;
						}
						else
						{
							edge.WindCnt = tEdge.WindCnt + edge.WindDelta;
						}
					}
					else
					{
						edge.WindCnt = ((edge.WindDelta != 0) ? edge.WindDelta : 1);
					}
				}
				else if (edge.WindDelta == 0)
				{
					edge.WindCnt = ((tEdge.WindCnt >= 0) ? (tEdge.WindCnt + 1) : (tEdge.WindCnt - 1));
				}
				else if (tEdge.WindDelta * edge.WindDelta < 0)
				{
					edge.WindCnt = tEdge.WindCnt;
				}
				else
				{
					edge.WindCnt = tEdge.WindCnt + edge.WindDelta;
				}
				edge.WindCnt2 = tEdge.WindCnt2;
				tEdge = tEdge.NextInAEL;
			}
			if (this.IsEvenOddAltFillType(edge))
			{
				while (tEdge != edge)
				{
					if (tEdge.WindDelta != 0)
					{
						edge.WindCnt2 = ((edge.WindCnt2 != 0) ? 0 : 1);
					}
					tEdge = tEdge.NextInAEL;
				}
			}
			else
			{
				while (tEdge != edge)
				{
					edge.WindCnt2 += tEdge.WindDelta;
					tEdge = tEdge.NextInAEL;
				}
			}
		}

		private void AddEdgeToSEL(TEdge edge)
		{
			if (this.m_SortedEdges == null)
			{
				this.m_SortedEdges = edge;
				edge.PrevInSEL = null;
				edge.NextInSEL = null;
			}
			else
			{
				edge.NextInSEL = this.m_SortedEdges;
				edge.PrevInSEL = null;
				this.m_SortedEdges.PrevInSEL = edge;
				this.m_SortedEdges = edge;
			}
		}

		private void CopyAELToSEL()
		{
			TEdge tEdge = this.m_ActiveEdges;
			this.m_SortedEdges = tEdge;
			while (tEdge != null)
			{
				tEdge.PrevInSEL = tEdge.PrevInAEL;
				tEdge.NextInSEL = tEdge.NextInAEL;
				tEdge = tEdge.NextInAEL;
			}
		}

		private void SwapPositionsInAEL(TEdge edge1, TEdge edge2)
		{
			if (edge1.NextInAEL == edge1.PrevInAEL || edge2.NextInAEL == edge2.PrevInAEL)
			{
				return;
			}
			if (edge1.NextInAEL == edge2)
			{
				TEdge nextInAEL = edge2.NextInAEL;
				if (nextInAEL != null)
				{
					nextInAEL.PrevInAEL = edge1;
				}
				TEdge prevInAEL = edge1.PrevInAEL;
				if (prevInAEL != null)
				{
					prevInAEL.NextInAEL = edge2;
				}
				edge2.PrevInAEL = prevInAEL;
				edge2.NextInAEL = edge1;
				edge1.PrevInAEL = edge2;
				edge1.NextInAEL = nextInAEL;
			}
			else if (edge2.NextInAEL == edge1)
			{
				TEdge nextInAEL2 = edge1.NextInAEL;
				if (nextInAEL2 != null)
				{
					nextInAEL2.PrevInAEL = edge2;
				}
				TEdge prevInAEL2 = edge2.PrevInAEL;
				if (prevInAEL2 != null)
				{
					prevInAEL2.NextInAEL = edge1;
				}
				edge1.PrevInAEL = prevInAEL2;
				edge1.NextInAEL = edge2;
				edge2.PrevInAEL = edge1;
				edge2.NextInAEL = nextInAEL2;
			}
			else
			{
				TEdge nextInAEL3 = edge1.NextInAEL;
				TEdge prevInAEL3 = edge1.PrevInAEL;
				edge1.NextInAEL = edge2.NextInAEL;
				if (edge1.NextInAEL != null)
				{
					edge1.NextInAEL.PrevInAEL = edge1;
				}
				edge1.PrevInAEL = edge2.PrevInAEL;
				if (edge1.PrevInAEL != null)
				{
					edge1.PrevInAEL.NextInAEL = edge1;
				}
				edge2.NextInAEL = nextInAEL3;
				if (edge2.NextInAEL != null)
				{
					edge2.NextInAEL.PrevInAEL = edge2;
				}
				edge2.PrevInAEL = prevInAEL3;
				if (edge2.PrevInAEL != null)
				{
					edge2.PrevInAEL.NextInAEL = edge2;
				}
			}
			if (edge1.PrevInAEL == null)
			{
				this.m_ActiveEdges = edge1;
			}
			else if (edge2.PrevInAEL == null)
			{
				this.m_ActiveEdges = edge2;
			}
		}

		private void SwapPositionsInSEL(TEdge edge1, TEdge edge2)
		{
			if (edge1.NextInSEL == null && edge1.PrevInSEL == null)
			{
				return;
			}
			if (edge2.NextInSEL == null && edge2.PrevInSEL == null)
			{
				return;
			}
			if (edge1.NextInSEL == edge2)
			{
				TEdge nextInSEL = edge2.NextInSEL;
				if (nextInSEL != null)
				{
					nextInSEL.PrevInSEL = edge1;
				}
				TEdge prevInSEL = edge1.PrevInSEL;
				if (prevInSEL != null)
				{
					prevInSEL.NextInSEL = edge2;
				}
				edge2.PrevInSEL = prevInSEL;
				edge2.NextInSEL = edge1;
				edge1.PrevInSEL = edge2;
				edge1.NextInSEL = nextInSEL;
			}
			else if (edge2.NextInSEL == edge1)
			{
				TEdge nextInSEL2 = edge1.NextInSEL;
				if (nextInSEL2 != null)
				{
					nextInSEL2.PrevInSEL = edge2;
				}
				TEdge prevInSEL2 = edge2.PrevInSEL;
				if (prevInSEL2 != null)
				{
					prevInSEL2.NextInSEL = edge1;
				}
				edge1.PrevInSEL = prevInSEL2;
				edge1.NextInSEL = edge2;
				edge2.PrevInSEL = edge1;
				edge2.NextInSEL = nextInSEL2;
			}
			else
			{
				TEdge nextInSEL3 = edge1.NextInSEL;
				TEdge prevInSEL3 = edge1.PrevInSEL;
				edge1.NextInSEL = edge2.NextInSEL;
				if (edge1.NextInSEL != null)
				{
					edge1.NextInSEL.PrevInSEL = edge1;
				}
				edge1.PrevInSEL = edge2.PrevInSEL;
				if (edge1.PrevInSEL != null)
				{
					edge1.PrevInSEL.NextInSEL = edge1;
				}
				edge2.NextInSEL = nextInSEL3;
				if (edge2.NextInSEL != null)
				{
					edge2.NextInSEL.PrevInSEL = edge2;
				}
				edge2.PrevInSEL = prevInSEL3;
				if (edge2.PrevInSEL != null)
				{
					edge2.PrevInSEL.NextInSEL = edge2;
				}
			}
			if (edge1.PrevInSEL == null)
			{
				this.m_SortedEdges = edge1;
			}
			else if (edge2.PrevInSEL == null)
			{
				this.m_SortedEdges = edge2;
			}
		}

		private void AddLocalMaxPoly(TEdge e1, TEdge e2, IntPoint pt)
		{
			this.AddOutPt(e1, pt);
			if (e1.OutIdx == e2.OutIdx)
			{
				e1.OutIdx = -1;
				e2.OutIdx = -1;
			}
			else if (e1.OutIdx < e2.OutIdx)
			{
				this.AppendPolygon(e1, e2);
			}
			else
			{
				this.AppendPolygon(e2, e1);
			}
		}

		private OutPt AddLocalMinPoly(TEdge e1, TEdge e2, IntPoint pt)
		{
			OutPt outPt;
			TEdge tEdge;
			TEdge prevInAEL;
			if (ClipperBase.IsHorizontal(e2) || e1.Dx > e2.Dx)
			{
				outPt = this.AddOutPt(e1, pt);
				e2.OutIdx = e1.OutIdx;
				e1.Side = EdgeSide.esLeft;
				e2.Side = EdgeSide.esRight;
				tEdge = e1;
				if (tEdge.PrevInAEL == e2)
				{
					prevInAEL = e2.PrevInAEL;
				}
				else
				{
					prevInAEL = tEdge.PrevInAEL;
				}
			}
			else
			{
				outPt = this.AddOutPt(e2, pt);
				e1.OutIdx = e2.OutIdx;
				e1.Side = EdgeSide.esRight;
				e2.Side = EdgeSide.esLeft;
				tEdge = e2;
				if (tEdge.PrevInAEL == e1)
				{
					prevInAEL = e1.PrevInAEL;
				}
				else
				{
					prevInAEL = tEdge.PrevInAEL;
				}
			}
			if (prevInAEL != null && prevInAEL.OutIdx >= 0 && Clipper.TopX(prevInAEL, pt.Y) == Clipper.TopX(tEdge, pt.Y) && ClipperBase.SlopesEqual(tEdge, prevInAEL, this.m_UseFullRange) && tEdge.WindDelta != 0 && prevInAEL.WindDelta != 0)
			{
				OutPt op = this.AddOutPt(prevInAEL, pt);
				this.AddJoin(outPt, op, tEdge.Top);
			}
			return outPt;
		}

		private OutRec CreateOutRec()
		{
			OutRec outRec = new OutRec();
			outRec.Idx = -1;
			outRec.IsHole = false;
			outRec.IsOpen = false;
			outRec.FirstLeft = null;
			outRec.Pts = null;
			outRec.BottomPt = null;
			outRec.PolyNode = null;
			this.m_PolyOuts.Add(outRec);
			outRec.Idx = this.m_PolyOuts.Count - 1;
			return outRec;
		}

		private OutPt AddOutPt(TEdge e, IntPoint pt)
		{
			bool flag = e.Side == EdgeSide.esLeft;
			if (e.OutIdx < 0)
			{
				OutRec outRec = this.CreateOutRec();
				outRec.IsOpen = (e.WindDelta == 0);
				OutPt outPt = new OutPt();
				outRec.Pts = outPt;
				outPt.Idx = outRec.Idx;
				outPt.Pt = pt;
				outPt.Next = outPt;
				outPt.Prev = outPt;
				if (!outRec.IsOpen)
				{
					this.SetHoleState(e, outRec);
				}
				e.OutIdx = outRec.Idx;
				return outPt;
			}
			OutRec outRec2 = this.m_PolyOuts[e.OutIdx];
			OutPt pts = outRec2.Pts;
			if (flag && pt == pts.Pt)
			{
				return pts;
			}
			if (!flag && pt == pts.Prev.Pt)
			{
				return pts.Prev;
			}
			OutPt outPt2 = new OutPt();
			outPt2.Idx = outRec2.Idx;
			outPt2.Pt = pt;
			outPt2.Next = pts;
			outPt2.Prev = pts.Prev;
			outPt2.Prev.Next = outPt2;
			pts.Prev = outPt2;
			if (flag)
			{
				outRec2.Pts = outPt2;
			}
			return outPt2;
		}

		internal void SwapPoints(ref IntPoint pt1, ref IntPoint pt2)
		{
			IntPoint intPoint = new IntPoint(pt1);
			pt1 = pt2;
			pt2 = intPoint;
		}

		private bool HorzSegmentsOverlap(IntPoint Pt1a, IntPoint Pt1b, IntPoint Pt2a, IntPoint Pt2b)
		{
			return Pt1a.X > Pt2a.X == Pt1a.X < Pt2b.X || Pt1b.X > Pt2a.X == Pt1b.X < Pt2b.X || Pt2a.X > Pt1a.X == Pt2a.X < Pt1b.X || Pt2b.X > Pt1a.X == Pt2b.X < Pt1b.X || (Pt1a.X == Pt2a.X && Pt1b.X == Pt2b.X) || (Pt1a.X == Pt2b.X && Pt1b.X == Pt2a.X);
		}

		private OutPt InsertPolyPtBetween(OutPt p1, OutPt p2, IntPoint pt)
		{
			OutPt outPt = new OutPt();
			outPt.Pt = pt;
			if (p2 == p1.Next)
			{
				p1.Next = outPt;
				p2.Prev = outPt;
				outPt.Next = p2;
				outPt.Prev = p1;
			}
			else
			{
				p2.Next = outPt;
				p1.Prev = outPt;
				outPt.Next = p1;
				outPt.Prev = p2;
			}
			return outPt;
		}

		private void SetHoleState(TEdge e, OutRec outRec)
		{
			bool flag = false;
			for (TEdge prevInAEL = e.PrevInAEL; prevInAEL != null; prevInAEL = prevInAEL.PrevInAEL)
			{
				if (prevInAEL.OutIdx >= 0)
				{
					flag = !flag;
					if (outRec.FirstLeft == null)
					{
						outRec.FirstLeft = this.m_PolyOuts[prevInAEL.OutIdx];
					}
				}
			}
			if (flag)
			{
				outRec.IsHole = true;
			}
		}

		private double GetDx(IntPoint pt1, IntPoint pt2)
		{
			if (pt1.Y == pt2.Y)
			{
				return -3.4E+38;
			}
			return (double)(pt2.X - pt1.X) / (double)(pt2.Y - pt1.Y);
		}

		private bool FirstIsBottomPt(OutPt btmPt1, OutPt btmPt2)
		{
			OutPt outPt = btmPt1.Prev;
			while (outPt.Pt == btmPt1.Pt && outPt != btmPt1)
			{
				outPt = outPt.Prev;
			}
			double num = Math.Abs(this.GetDx(btmPt1.Pt, outPt.Pt));
			outPt = btmPt1.Next;
			while (outPt.Pt == btmPt1.Pt && outPt != btmPt1)
			{
				outPt = outPt.Next;
			}
			double num2 = Math.Abs(this.GetDx(btmPt1.Pt, outPt.Pt));
			outPt = btmPt2.Prev;
			while (outPt.Pt == btmPt2.Pt && outPt != btmPt2)
			{
				outPt = outPt.Prev;
			}
			double num3 = Math.Abs(this.GetDx(btmPt2.Pt, outPt.Pt));
			outPt = btmPt2.Next;
			while (outPt.Pt == btmPt2.Pt && outPt != btmPt2)
			{
				outPt = outPt.Next;
			}
			double num4 = Math.Abs(this.GetDx(btmPt2.Pt, outPt.Pt));
			return (num >= num3 && num >= num4) || (num2 >= num3 && num2 >= num4);
		}

		private OutPt GetBottomPt(OutPt pp)
		{
			OutPt outPt = null;
			OutPt next;
			for (next = pp.Next; next != pp; next = next.Next)
			{
				if (next.Pt.Y > pp.Pt.Y)
				{
					pp = next;
					outPt = null;
				}
				else if (next.Pt.Y == pp.Pt.Y && next.Pt.X <= pp.Pt.X)
				{
					if (next.Pt.X < pp.Pt.X)
					{
						outPt = null;
						pp = next;
					}
					else if (next.Next != pp && next.Prev != pp)
					{
						outPt = next;
					}
				}
			}
			if (outPt != null)
			{
				while (outPt != next)
				{
					if (!this.FirstIsBottomPt(next, outPt))
					{
						pp = outPt;
					}
					outPt = outPt.Next;
					while (outPt.Pt != pp.Pt)
					{
						outPt = outPt.Next;
					}
				}
			}
			return pp;
		}

		private OutRec GetLowermostRec(OutRec outRec1, OutRec outRec2)
		{
			if (outRec1.BottomPt == null)
			{
				outRec1.BottomPt = this.GetBottomPt(outRec1.Pts);
			}
			if (outRec2.BottomPt == null)
			{
				outRec2.BottomPt = this.GetBottomPt(outRec2.Pts);
			}
			OutPt bottomPt = outRec1.BottomPt;
			OutPt bottomPt2 = outRec2.BottomPt;
			if (bottomPt.Pt.Y > bottomPt2.Pt.Y)
			{
				return outRec1;
			}
			if (bottomPt.Pt.Y < bottomPt2.Pt.Y)
			{
				return outRec2;
			}
			if (bottomPt.Pt.X < bottomPt2.Pt.X)
			{
				return outRec1;
			}
			if (bottomPt.Pt.X > bottomPt2.Pt.X)
			{
				return outRec2;
			}
			if (bottomPt.Next == bottomPt)
			{
				return outRec2;
			}
			if (bottomPt2.Next == bottomPt2)
			{
				return outRec1;
			}
			if (this.FirstIsBottomPt(bottomPt, bottomPt2))
			{
				return outRec1;
			}
			return outRec2;
		}

		private bool Param1RightOfParam2(OutRec outRec1, OutRec outRec2)
		{
			while (true)
			{
				outRec1 = outRec1.FirstLeft;
				if (outRec1 == outRec2)
				{
					break;
				}
				if (outRec1 == null)
				{
					return false;
				}
			}
			return true;
		}

		private OutRec GetOutRec(int idx)
		{
			OutRec outRec;
			for (outRec = this.m_PolyOuts[idx]; outRec != this.m_PolyOuts[outRec.Idx]; outRec = this.m_PolyOuts[outRec.Idx])
			{
			}
			return outRec;
		}

		private void AppendPolygon(TEdge e1, TEdge e2)
		{
			OutRec outRec = this.m_PolyOuts[e1.OutIdx];
			OutRec outRec2 = this.m_PolyOuts[e2.OutIdx];
			OutRec outRec3;
			if (this.Param1RightOfParam2(outRec, outRec2))
			{
				outRec3 = outRec2;
			}
			else if (this.Param1RightOfParam2(outRec2, outRec))
			{
				outRec3 = outRec;
			}
			else
			{
				outRec3 = this.GetLowermostRec(outRec, outRec2);
			}
			OutPt pts = outRec.Pts;
			OutPt prev = pts.Prev;
			OutPt pts2 = outRec2.Pts;
			OutPt prev2 = pts2.Prev;
			EdgeSide side;
			if (e1.Side == EdgeSide.esLeft)
			{
				if (e2.Side == EdgeSide.esLeft)
				{
					this.ReversePolyPtLinks(pts2);
					pts2.Next = pts;
					pts.Prev = pts2;
					prev.Next = prev2;
					prev2.Prev = prev;
					outRec.Pts = prev2;
				}
				else
				{
					prev2.Next = pts;
					pts.Prev = prev2;
					pts2.Prev = prev;
					prev.Next = pts2;
					outRec.Pts = pts2;
				}
				side = EdgeSide.esLeft;
			}
			else
			{
				if (e2.Side == EdgeSide.esRight)
				{
					this.ReversePolyPtLinks(pts2);
					prev.Next = prev2;
					prev2.Prev = prev;
					pts2.Next = pts;
					pts.Prev = pts2;
				}
				else
				{
					prev.Next = pts2;
					pts2.Prev = prev;
					pts.Prev = prev2;
					prev2.Next = pts;
				}
				side = EdgeSide.esRight;
			}
			outRec.BottomPt = null;
			if (outRec3 == outRec2)
			{
				if (outRec2.FirstLeft != outRec)
				{
					outRec.FirstLeft = outRec2.FirstLeft;
				}
				outRec.IsHole = outRec2.IsHole;
			}
			outRec2.Pts = null;
			outRec2.BottomPt = null;
			outRec2.FirstLeft = outRec;
			int outIdx = e1.OutIdx;
			int outIdx2 = e2.OutIdx;
			e1.OutIdx = -1;
			e2.OutIdx = -1;
			for (TEdge tEdge = this.m_ActiveEdges; tEdge != null; tEdge = tEdge.NextInAEL)
			{
				if (tEdge.OutIdx == outIdx2)
				{
					tEdge.OutIdx = outIdx;
					tEdge.Side = side;
					break;
				}
			}
			outRec2.Idx = outRec.Idx;
		}

		private void ReversePolyPtLinks(OutPt pp)
		{
			if (pp == null)
			{
				return;
			}
			OutPt outPt = pp;
			do
			{
				OutPt next = outPt.Next;
				outPt.Next = outPt.Prev;
				outPt.Prev = next;
				outPt = next;
			}
			while (outPt != pp);
		}

		private static void SwapSides(TEdge edge1, TEdge edge2)
		{
			EdgeSide side = edge1.Side;
			edge1.Side = edge2.Side;
			edge2.Side = side;
		}

		private static void SwapPolyIndexes(TEdge edge1, TEdge edge2)
		{
			int outIdx = edge1.OutIdx;
			edge1.OutIdx = edge2.OutIdx;
			edge2.OutIdx = outIdx;
		}

		private void IntersectEdges(TEdge e1, TEdge e2, IntPoint pt, bool protect = false)
		{
			bool flag = !protect && e1.NextInLML == null && e1.Top.X == pt.X && e1.Top.Y == pt.Y;
			bool flag2 = !protect && e2.NextInLML == null && e2.Top.X == pt.X && e2.Top.Y == pt.Y;
			bool flag3 = e1.OutIdx >= 0;
			bool flag4 = e2.OutIdx >= 0;
			if (e1.PolyTyp == e2.PolyTyp)
			{
				if (this.IsEvenOddFillType(e1))
				{
					int windCnt = e1.WindCnt;
					e1.WindCnt = e2.WindCnt;
					e2.WindCnt = windCnt;
				}
				else
				{
					if (e1.WindCnt + e2.WindDelta == 0)
					{
						e1.WindCnt = -e1.WindCnt;
					}
					else
					{
						e1.WindCnt += e2.WindDelta;
					}
					if (e2.WindCnt - e1.WindDelta == 0)
					{
						e2.WindCnt = -e2.WindCnt;
					}
					else
					{
						e2.WindCnt -= e1.WindDelta;
					}
				}
			}
			else
			{
				if (!this.IsEvenOddFillType(e2))
				{
					e1.WindCnt2 += e2.WindDelta;
				}
				else
				{
					e1.WindCnt2 = ((e1.WindCnt2 != 0) ? 0 : 1);
				}
				if (!this.IsEvenOddFillType(e1))
				{
					e2.WindCnt2 -= e1.WindDelta;
				}
				else
				{
					e2.WindCnt2 = ((e2.WindCnt2 != 0) ? 0 : 1);
				}
			}
			PolyFillType polyFillType;
			PolyFillType polyFillType2;
			if (e1.PolyTyp == PolyType.ptSubject)
			{
				polyFillType = this.m_SubjFillType;
				polyFillType2 = this.m_ClipFillType;
			}
			else
			{
				polyFillType = this.m_ClipFillType;
				polyFillType2 = this.m_SubjFillType;
			}
			PolyFillType polyFillType3;
			PolyFillType polyFillType4;
			if (e2.PolyTyp == PolyType.ptSubject)
			{
				polyFillType3 = this.m_SubjFillType;
				polyFillType4 = this.m_ClipFillType;
			}
			else
			{
				polyFillType3 = this.m_ClipFillType;
				polyFillType4 = this.m_SubjFillType;
			}
			PolyFillType polyFillType5 = polyFillType;
			int num;
			if (polyFillType5 != PolyFillType.pftPositive)
			{
				if (polyFillType5 != PolyFillType.pftNegative)
				{
					num = Math.Abs(e1.WindCnt);
				}
				else
				{
					num = -e1.WindCnt;
				}
			}
			else
			{
				num = e1.WindCnt;
			}
			polyFillType5 = polyFillType3;
			int num2;
			if (polyFillType5 != PolyFillType.pftPositive)
			{
				if (polyFillType5 != PolyFillType.pftNegative)
				{
					num2 = Math.Abs(e2.WindCnt);
				}
				else
				{
					num2 = -e2.WindCnt;
				}
			}
			else
			{
				num2 = e2.WindCnt;
			}
			if (flag3 && flag4)
			{
				if (flag || flag2 || (num != 0 && num != 1) || (num2 != 0 && num2 != 1) || (e1.PolyTyp != e2.PolyTyp && this.m_ClipType != ClipType.ctXor))
				{
					this.AddLocalMaxPoly(e1, e2, pt);
				}
				else
				{
					this.AddOutPt(e1, pt);
					this.AddOutPt(e2, pt);
					Clipper.SwapSides(e1, e2);
					Clipper.SwapPolyIndexes(e1, e2);
				}
			}
			else if (flag3)
			{
				if (num2 == 0 || num2 == 1)
				{
					this.AddOutPt(e1, pt);
					Clipper.SwapSides(e1, e2);
					Clipper.SwapPolyIndexes(e1, e2);
				}
			}
			else if (flag4)
			{
				if (num == 0 || num == 1)
				{
					this.AddOutPt(e2, pt);
					Clipper.SwapSides(e1, e2);
					Clipper.SwapPolyIndexes(e1, e2);
				}
			}
			else if ((num == 0 || num == 1) && (num2 == 0 || num2 == 1) && !flag && !flag2)
			{
				polyFillType5 = polyFillType2;
				long num3;
				if (polyFillType5 != PolyFillType.pftPositive)
				{
					if (polyFillType5 != PolyFillType.pftNegative)
					{
						num3 = (long)Math.Abs(e1.WindCnt2);
					}
					else
					{
						num3 = (long)(-(long)e1.WindCnt2);
					}
				}
				else
				{
					num3 = (long)e1.WindCnt2;
				}
				polyFillType5 = polyFillType4;
				long num4;
				if (polyFillType5 != PolyFillType.pftPositive)
				{
					if (polyFillType5 != PolyFillType.pftNegative)
					{
						num4 = (long)Math.Abs(e2.WindCnt2);
					}
					else
					{
						num4 = (long)(-(long)e2.WindCnt2);
					}
				}
				else
				{
					num4 = (long)e2.WindCnt2;
				}
				if (e1.PolyTyp != e2.PolyTyp)
				{
					this.AddLocalMinPoly(e1, e2, pt);
				}
				else if (num == 1 && num2 == 1)
				{
					switch (this.m_ClipType)
					{
					case ClipType.ctIntersection:
						if (num3 > 0L && num4 > 0L)
						{
							this.AddLocalMinPoly(e1, e2, pt);
						}
						break;
					case ClipType.ctUnion:
						if (num3 <= 0L && num4 <= 0L)
						{
							this.AddLocalMinPoly(e1, e2, pt);
						}
						break;
					case ClipType.ctDifference:
						if ((e1.PolyTyp == PolyType.ptClip && num3 > 0L && num4 > 0L) || (e1.PolyTyp == PolyType.ptSubject && num3 <= 0L && num4 <= 0L))
						{
							this.AddLocalMinPoly(e1, e2, pt);
						}
						break;
					case ClipType.ctXor:
						this.AddLocalMinPoly(e1, e2, pt);
						break;
					}
				}
				else
				{
					Clipper.SwapSides(e1, e2);
				}
			}
			if (flag != flag2 && ((flag && e1.OutIdx >= 0) || (flag2 && e2.OutIdx >= 0)))
			{
				Clipper.SwapSides(e1, e2);
				Clipper.SwapPolyIndexes(e1, e2);
			}
			if (flag)
			{
				this.DeleteFromAEL(e1);
			}
			if (flag2)
			{
				this.DeleteFromAEL(e2);
			}
		}

		private void DeleteFromAEL(TEdge e)
		{
			TEdge prevInAEL = e.PrevInAEL;
			TEdge nextInAEL = e.NextInAEL;
			if (prevInAEL == null && nextInAEL == null && e != this.m_ActiveEdges)
			{
				return;
			}
			if (prevInAEL != null)
			{
				prevInAEL.NextInAEL = nextInAEL;
			}
			else
			{
				this.m_ActiveEdges = nextInAEL;
			}
			if (nextInAEL != null)
			{
				nextInAEL.PrevInAEL = prevInAEL;
			}
			e.NextInAEL = null;
			e.PrevInAEL = null;
		}

		private void DeleteFromSEL(TEdge e)
		{
			TEdge prevInSEL = e.PrevInSEL;
			TEdge nextInSEL = e.NextInSEL;
			if (prevInSEL == null && nextInSEL == null && e != this.m_SortedEdges)
			{
				return;
			}
			if (prevInSEL != null)
			{
				prevInSEL.NextInSEL = nextInSEL;
			}
			else
			{
				this.m_SortedEdges = nextInSEL;
			}
			if (nextInSEL != null)
			{
				nextInSEL.PrevInSEL = prevInSEL;
			}
			e.NextInSEL = null;
			e.PrevInSEL = null;
		}

		private void UpdateEdgeIntoAEL(ref TEdge e)
		{
			if (e.NextInLML == null)
			{
				throw new ClipperException("UpdateEdgeIntoAEL: invalid call");
			}
			TEdge prevInAEL = e.PrevInAEL;
			TEdge nextInAEL = e.NextInAEL;
			e.NextInLML.OutIdx = e.OutIdx;
			if (prevInAEL != null)
			{
				prevInAEL.NextInAEL = e.NextInLML;
			}
			else
			{
				this.m_ActiveEdges = e.NextInLML;
			}
			if (nextInAEL != null)
			{
				nextInAEL.PrevInAEL = e.NextInLML;
			}
			e.NextInLML.Side = e.Side;
			e.NextInLML.WindDelta = e.WindDelta;
			e.NextInLML.WindCnt = e.WindCnt;
			e.NextInLML.WindCnt2 = e.WindCnt2;
			e = e.NextInLML;
			e.Curr = e.Bot;
			e.PrevInAEL = prevInAEL;
			e.NextInAEL = nextInAEL;
			if (!ClipperBase.IsHorizontal(e))
			{
				this.InsertScanbeam(e.Top.Y);
			}
		}

		private void ProcessHorizontals(bool isTopOfScanbeam)
		{
			for (TEdge sortedEdges = this.m_SortedEdges; sortedEdges != null; sortedEdges = this.m_SortedEdges)
			{
				this.DeleteFromSEL(sortedEdges);
				this.ProcessHorizontal(sortedEdges, isTopOfScanbeam);
			}
		}

		private void GetHorzDirection(TEdge HorzEdge, out Direction Dir, out long Left, out long Right)
		{
			if (HorzEdge.Bot.X < HorzEdge.Top.X)
			{
				Left = HorzEdge.Bot.X;
				Right = HorzEdge.Top.X;
				Dir = Direction.dLeftToRight;
			}
			else
			{
				Left = HorzEdge.Top.X;
				Right = HorzEdge.Bot.X;
				Dir = Direction.dRightToLeft;
			}
		}

		private void PrepareHorzJoins(TEdge horzEdge, bool isTopOfScanbeam)
		{
			OutPt outPt = this.m_PolyOuts[horzEdge.OutIdx].Pts;
			if (horzEdge.Side != EdgeSide.esLeft)
			{
				outPt = outPt.Prev;
			}
			for (int i = 0; i < this.m_GhostJoins.Count; i++)
			{
				Join join = this.m_GhostJoins[i];
				if (this.HorzSegmentsOverlap(join.OutPt1.Pt, join.OffPt, horzEdge.Bot, horzEdge.Top))
				{
					this.AddJoin(join.OutPt1, outPt, join.OffPt);
				}
			}
			if (isTopOfScanbeam)
			{
				if (outPt.Pt == horzEdge.Top)
				{
					this.AddGhostJoin(outPt, horzEdge.Bot);
				}
				else
				{
					this.AddGhostJoin(outPt, horzEdge.Top);
				}
			}
		}

		private void ProcessHorizontal(TEdge horzEdge, bool isTopOfScanbeam)
		{
			Direction direction;
			long num;
			long num2;
			this.GetHorzDirection(horzEdge, out direction, out num, out num2);
			TEdge tEdge = horzEdge;
			TEdge tEdge2 = null;
			while (tEdge.NextInLML != null && ClipperBase.IsHorizontal(tEdge.NextInLML))
			{
				tEdge = tEdge.NextInLML;
			}
			if (tEdge.NextInLML == null)
			{
				tEdge2 = this.GetMaximaPair(tEdge);
			}
			TEdge tEdge3;
			while (true)
			{
				bool flag = horzEdge == tEdge;
				TEdge nextInAEL;
				for (tEdge3 = this.GetNextInAEL(horzEdge, direction); tEdge3 != null; tEdge3 = nextInAEL)
				{
					if (tEdge3.Curr.X == horzEdge.Top.X && horzEdge.NextInLML != null && tEdge3.Dx < horzEdge.NextInLML.Dx)
					{
						break;
					}
					nextInAEL = this.GetNextInAEL(tEdge3, direction);
					if ((direction == Direction.dLeftToRight && tEdge3.Curr.X <= num2) || (direction == Direction.dRightToLeft && tEdge3.Curr.X >= num))
					{
						if (tEdge3 == tEdge2 && flag)
						{
							goto Block_9;
						}
						if (direction == Direction.dLeftToRight)
						{
							IntPoint pt = new IntPoint(tEdge3.Curr.X, horzEdge.Curr.Y);
							this.IntersectEdges(horzEdge, tEdge3, pt, true);
						}
						else
						{
							IntPoint pt2 = new IntPoint(tEdge3.Curr.X, horzEdge.Curr.Y);
							this.IntersectEdges(tEdge3, horzEdge, pt2, true);
						}
						this.SwapPositionsInAEL(horzEdge, tEdge3);
					}
					else if ((direction == Direction.dLeftToRight && tEdge3.Curr.X >= num2) || (direction == Direction.dRightToLeft && tEdge3.Curr.X <= num))
					{
						break;
					}
				}
				if (horzEdge.OutIdx >= 0 && horzEdge.WindDelta != 0)
				{
					this.PrepareHorzJoins(horzEdge, isTopOfScanbeam);
				}
				if (horzEdge.NextInLML == null || !ClipperBase.IsHorizontal(horzEdge.NextInLML))
				{
					goto IL_279;
				}
				this.UpdateEdgeIntoAEL(ref horzEdge);
				if (horzEdge.OutIdx >= 0)
				{
					this.AddOutPt(horzEdge, horzEdge.Bot);
				}
				this.GetHorzDirection(horzEdge, out direction, out num, out num2);
			}
			Block_9:
			if (horzEdge.OutIdx >= 0 && horzEdge.WindDelta != 0)
			{
				this.PrepareHorzJoins(horzEdge, isTopOfScanbeam);
			}
			if (direction == Direction.dLeftToRight)
			{
				this.IntersectEdges(horzEdge, tEdge3, tEdge3.Top, false);
			}
			else
			{
				this.IntersectEdges(tEdge3, horzEdge, tEdge3.Top, false);
			}
			if (tEdge2.OutIdx >= 0)
			{
				throw new ClipperException("ProcessHorizontal error");
			}
			return;
			IL_279:
			if (horzEdge.NextInLML != null)
			{
				if (horzEdge.OutIdx >= 0)
				{
					OutPt op = this.AddOutPt(horzEdge, horzEdge.Top);
					this.UpdateEdgeIntoAEL(ref horzEdge);
					if (horzEdge.WindDelta == 0)
					{
						return;
					}
					TEdge prevInAEL = horzEdge.PrevInAEL;
					TEdge nextInAEL2 = horzEdge.NextInAEL;
					if (prevInAEL != null && prevInAEL.Curr.X == horzEdge.Bot.X && prevInAEL.Curr.Y == horzEdge.Bot.Y && prevInAEL.WindDelta != 0 && prevInAEL.OutIdx >= 0 && prevInAEL.Curr.Y > prevInAEL.Top.Y && ClipperBase.SlopesEqual(horzEdge, prevInAEL, this.m_UseFullRange))
					{
						OutPt op2 = this.AddOutPt(prevInAEL, horzEdge.Bot);
						this.AddJoin(op, op2, horzEdge.Top);
					}
					else if (nextInAEL2 != null && nextInAEL2.Curr.X == horzEdge.Bot.X && nextInAEL2.Curr.Y == horzEdge.Bot.Y && nextInAEL2.WindDelta != 0 && nextInAEL2.OutIdx >= 0 && nextInAEL2.Curr.Y > nextInAEL2.Top.Y && ClipperBase.SlopesEqual(horzEdge, nextInAEL2, this.m_UseFullRange))
					{
						OutPt op3 = this.AddOutPt(nextInAEL2, horzEdge.Bot);
						this.AddJoin(op, op3, horzEdge.Top);
					}
				}
				else
				{
					this.UpdateEdgeIntoAEL(ref horzEdge);
				}
			}
			else if (tEdge2 != null)
			{
				if (tEdge2.OutIdx >= 0)
				{
					if (direction == Direction.dLeftToRight)
					{
						this.IntersectEdges(horzEdge, tEdge2, horzEdge.Top, false);
					}
					else
					{
						this.IntersectEdges(tEdge2, horzEdge, horzEdge.Top, false);
					}
					if (tEdge2.OutIdx >= 0)
					{
						throw new ClipperException("ProcessHorizontal error");
					}
				}
				else
				{
					this.DeleteFromAEL(horzEdge);
					this.DeleteFromAEL(tEdge2);
				}
			}
			else
			{
				if (horzEdge.OutIdx >= 0)
				{
					this.AddOutPt(horzEdge, horzEdge.Top);
				}
				this.DeleteFromAEL(horzEdge);
			}
		}

		private TEdge GetNextInAEL(TEdge e, Direction Direction)
		{
			return (Direction != Direction.dLeftToRight) ? e.PrevInAEL : e.NextInAEL;
		}

		private bool IsMinima(TEdge e)
		{
			return e != null && e.Prev.NextInLML != e && e.Next.NextInLML != e;
		}

		private bool IsMaxima(TEdge e, double Y)
		{
			return e != null && (double)e.Top.Y == Y && e.NextInLML == null;
		}

		private bool IsIntermediate(TEdge e, double Y)
		{
			return (double)e.Top.Y == Y && e.NextInLML != null;
		}

		private TEdge GetMaximaPair(TEdge e)
		{
			TEdge tEdge = null;
			if (e.Next.Top == e.Top && e.Next.NextInLML == null)
			{
				tEdge = e.Next;
			}
			else if (e.Prev.Top == e.Top && e.Prev.NextInLML == null)
			{
				tEdge = e.Prev;
			}
			if (tEdge != null && (tEdge.OutIdx == -2 || (tEdge.NextInAEL == tEdge.PrevInAEL && !ClipperBase.IsHorizontal(tEdge))))
			{
				return null;
			}
			return tEdge;
		}

		private bool ProcessIntersections(long botY, long topY)
		{
			if (this.m_ActiveEdges == null)
			{
				return true;
			}
			try
			{
				this.BuildIntersectList(botY, topY);
				if (this.m_IntersectNodes == null)
				{
					bool result = true;
					return result;
				}
				if (this.m_IntersectNodes.Next != null && !this.FixupIntersectionOrder())
				{
					bool result = false;
					return result;
				}
				this.ProcessIntersectList();
			}
			catch
			{
				this.m_SortedEdges = null;
				this.DisposeIntersectNodes();
				throw new ClipperException("ProcessIntersections error");
			}
			this.m_SortedEdges = null;
			return true;
		}

		private void BuildIntersectList(long botY, long topY)
		{
			if (this.m_ActiveEdges == null)
			{
				return;
			}
			TEdge tEdge = this.m_ActiveEdges;
			this.m_SortedEdges = tEdge;
			while (tEdge != null)
			{
				tEdge.PrevInSEL = tEdge.PrevInAEL;
				tEdge.NextInSEL = tEdge.NextInAEL;
				tEdge.Curr.X = Clipper.TopX(tEdge, topY);
				tEdge = tEdge.NextInAEL;
			}
			bool flag = true;
			while (flag && this.m_SortedEdges != null)
			{
				flag = false;
				tEdge = this.m_SortedEdges;
				while (tEdge.NextInSEL != null)
				{
					TEdge nextInSEL = tEdge.NextInSEL;
					if (tEdge.Curr.X > nextInSEL.Curr.X)
					{
						IntPoint pt;
						if (!this.IntersectPoint(tEdge, nextInSEL, out pt) && tEdge.Curr.X > nextInSEL.Curr.X + 1L)
						{
							throw new ClipperException("Intersection error");
						}
						if (pt.Y > botY)
						{
							pt.Y = botY;
							if (Math.Abs(tEdge.Dx) > Math.Abs(nextInSEL.Dx))
							{
								pt.X = Clipper.TopX(nextInSEL, botY);
							}
							else
							{
								pt.X = Clipper.TopX(tEdge, botY);
							}
						}
						this.InsertIntersectNode(tEdge, nextInSEL, pt);
						this.SwapPositionsInSEL(tEdge, nextInSEL);
						flag = true;
					}
					else
					{
						tEdge = nextInSEL;
					}
				}
				if (tEdge.PrevInSEL == null)
				{
					break;
				}
				tEdge.PrevInSEL.NextInSEL = null;
			}
			this.m_SortedEdges = null;
		}

		private bool EdgesAdjacent(IntersectNode inode)
		{
			return inode.Edge1.NextInSEL == inode.Edge2 || inode.Edge1.PrevInSEL == inode.Edge2;
		}

		private bool FixupIntersectionOrder()
		{
			IntersectNode intersectNode = this.m_IntersectNodes;
			this.CopyAELToSEL();
			while (intersectNode != null)
			{
				if (!this.EdgesAdjacent(intersectNode))
				{
					IntersectNode next = intersectNode.Next;
					while (next != null && !this.EdgesAdjacent(next))
					{
						next = next.Next;
					}
					if (next == null)
					{
						return false;
					}
					this.SwapIntersectNodes(intersectNode, next);
				}
				this.SwapPositionsInSEL(intersectNode.Edge1, intersectNode.Edge2);
				intersectNode = intersectNode.Next;
			}
			return true;
		}

		private void ProcessIntersectList()
		{
			while (this.m_IntersectNodes != null)
			{
				IntersectNode next = this.m_IntersectNodes.Next;
				this.IntersectEdges(this.m_IntersectNodes.Edge1, this.m_IntersectNodes.Edge2, this.m_IntersectNodes.Pt, true);
				this.SwapPositionsInAEL(this.m_IntersectNodes.Edge1, this.m_IntersectNodes.Edge2);
				this.m_IntersectNodes = null;
				this.m_IntersectNodes = next;
			}
		}

		internal static long Round(double value)
		{
			return (value >= 0.0) ? ((long)(value + 0.5)) : ((long)(value - 0.5));
		}

		private static long TopX(TEdge edge, long currentY)
		{
			if (currentY == edge.Top.Y)
			{
				return edge.Top.X;
			}
			return edge.Bot.X + Clipper.Round(edge.Dx * (double)(currentY - edge.Bot.Y));
		}

		private void InsertIntersectNode(TEdge e1, TEdge e2, IntPoint pt)
		{
			IntersectNode intersectNode = new IntersectNode();
			intersectNode.Edge1 = e1;
			intersectNode.Edge2 = e2;
			intersectNode.Pt = pt;
			intersectNode.Next = null;
			if (this.m_IntersectNodes == null)
			{
				this.m_IntersectNodes = intersectNode;
			}
			else if (intersectNode.Pt.Y > this.m_IntersectNodes.Pt.Y)
			{
				intersectNode.Next = this.m_IntersectNodes;
				this.m_IntersectNodes = intersectNode;
			}
			else
			{
				IntersectNode intersectNode2 = this.m_IntersectNodes;
				while (intersectNode2.Next != null && intersectNode.Pt.Y < intersectNode2.Next.Pt.Y)
				{
					intersectNode2 = intersectNode2.Next;
				}
				intersectNode.Next = intersectNode2.Next;
				intersectNode2.Next = intersectNode;
			}
		}

		private void SwapIntersectNodes(IntersectNode int1, IntersectNode int2)
		{
			TEdge edge = int1.Edge1;
			TEdge edge2 = int1.Edge2;
			IntPoint pt = new IntPoint(int1.Pt);
			int1.Edge1 = int2.Edge1;
			int1.Edge2 = int2.Edge2;
			int1.Pt = int2.Pt;
			int2.Edge1 = edge;
			int2.Edge2 = edge2;
			int2.Pt = pt;
		}

		private bool IntersectPoint(TEdge edge1, TEdge edge2, out IntPoint ip)
		{
			ip = default(IntPoint);
			if (ClipperBase.SlopesEqual(edge1, edge2, this.m_UseFullRange) || edge1.Dx == edge2.Dx)
			{
				if (edge2.Bot.Y > edge1.Bot.Y)
				{
					ip.Y = edge2.Bot.Y;
				}
				else
				{
					ip.Y = edge1.Bot.Y;
				}
				return false;
			}
			if (edge1.Delta.X == 0L)
			{
				ip.X = edge1.Bot.X;
				if (ClipperBase.IsHorizontal(edge2))
				{
					ip.Y = edge2.Bot.Y;
				}
				else
				{
					double num = (double)edge2.Bot.Y - (double)edge2.Bot.X / edge2.Dx;
					ip.Y = Clipper.Round((double)ip.X / edge2.Dx + num);
				}
			}
			else if (edge2.Delta.X == 0L)
			{
				ip.X = edge2.Bot.X;
				if (ClipperBase.IsHorizontal(edge1))
				{
					ip.Y = edge1.Bot.Y;
				}
				else
				{
					double num2 = (double)edge1.Bot.Y - (double)edge1.Bot.X / edge1.Dx;
					ip.Y = Clipper.Round((double)ip.X / edge1.Dx + num2);
				}
			}
			else
			{
				double num2 = (double)edge1.Bot.X - (double)edge1.Bot.Y * edge1.Dx;
				double num = (double)edge2.Bot.X - (double)edge2.Bot.Y * edge2.Dx;
				double num3 = (num - num2) / (edge1.Dx - edge2.Dx);
				ip.Y = Clipper.Round(num3);
				if (Math.Abs(edge1.Dx) < Math.Abs(edge2.Dx))
				{
					ip.X = Clipper.Round(edge1.Dx * num3 + num2);
				}
				else
				{
					ip.X = Clipper.Round(edge2.Dx * num3 + num);
				}
			}
			if (ip.Y >= edge1.Top.Y && ip.Y >= edge2.Top.Y)
			{
				return true;
			}
			if (edge1.Top.Y > edge2.Top.Y)
			{
				ip.Y = edge1.Top.Y;
				ip.X = Clipper.TopX(edge2, edge1.Top.Y);
				return ip.X < edge1.Top.X;
			}
			ip.Y = edge2.Top.Y;
			ip.X = Clipper.TopX(edge1, edge2.Top.Y);
			return ip.X > edge2.Top.X;
		}

		private void DisposeIntersectNodes()
		{
			while (this.m_IntersectNodes != null)
			{
				IntersectNode next = this.m_IntersectNodes.Next;
				this.m_IntersectNodes = null;
				this.m_IntersectNodes = next;
			}
		}

		private void ProcessEdgesAtTopOfScanbeam(long topY)
		{
			TEdge tEdge = this.m_ActiveEdges;
			while (tEdge != null)
			{
				bool flag = this.IsMaxima(tEdge, (double)topY);
				if (flag)
				{
					TEdge maximaPair = this.GetMaximaPair(tEdge);
					flag = (maximaPair == null || !ClipperBase.IsHorizontal(maximaPair));
				}
				if (flag)
				{
					TEdge prevInAEL = tEdge.PrevInAEL;
					this.DoMaxima(tEdge);
					if (prevInAEL == null)
					{
						tEdge = this.m_ActiveEdges;
					}
					else
					{
						tEdge = prevInAEL.NextInAEL;
					}
				}
				else
				{
					if (this.IsIntermediate(tEdge, (double)topY) && ClipperBase.IsHorizontal(tEdge.NextInLML))
					{
						this.UpdateEdgeIntoAEL(ref tEdge);
						if (tEdge.OutIdx >= 0)
						{
							this.AddOutPt(tEdge, tEdge.Bot);
						}
						this.AddEdgeToSEL(tEdge);
					}
					else
					{
						tEdge.Curr.X = Clipper.TopX(tEdge, topY);
						tEdge.Curr.Y = topY;
					}
					if (this.StrictlySimple)
					{
						TEdge prevInAEL2 = tEdge.PrevInAEL;
						if (tEdge.OutIdx >= 0 && tEdge.WindDelta != 0 && prevInAEL2 != null && prevInAEL2.OutIdx >= 0 && prevInAEL2.Curr.X == tEdge.Curr.X && prevInAEL2.WindDelta != 0)
						{
							OutPt op = this.AddOutPt(prevInAEL2, tEdge.Curr);
							OutPt op2 = this.AddOutPt(tEdge, tEdge.Curr);
							this.AddJoin(op, op2, tEdge.Curr);
						}
					}
					tEdge = tEdge.NextInAEL;
				}
			}
			this.ProcessHorizontals(true);
			for (tEdge = this.m_ActiveEdges; tEdge != null; tEdge = tEdge.NextInAEL)
			{
				if (this.IsIntermediate(tEdge, (double)topY))
				{
					OutPt outPt = null;
					if (tEdge.OutIdx >= 0)
					{
						outPt = this.AddOutPt(tEdge, tEdge.Top);
					}
					this.UpdateEdgeIntoAEL(ref tEdge);
					TEdge prevInAEL3 = tEdge.PrevInAEL;
					TEdge nextInAEL = tEdge.NextInAEL;
					if (prevInAEL3 != null && prevInAEL3.Curr.X == tEdge.Bot.X && prevInAEL3.Curr.Y == tEdge.Bot.Y && outPt != null && prevInAEL3.OutIdx >= 0 && prevInAEL3.Curr.Y > prevInAEL3.Top.Y && ClipperBase.SlopesEqual(tEdge, prevInAEL3, this.m_UseFullRange) && tEdge.WindDelta != 0 && prevInAEL3.WindDelta != 0)
					{
						OutPt op3 = this.AddOutPt(prevInAEL3, tEdge.Bot);
						this.AddJoin(outPt, op3, tEdge.Top);
					}
					else if (nextInAEL != null && nextInAEL.Curr.X == tEdge.Bot.X && nextInAEL.Curr.Y == tEdge.Bot.Y && outPt != null && nextInAEL.OutIdx >= 0 && nextInAEL.Curr.Y > nextInAEL.Top.Y && ClipperBase.SlopesEqual(tEdge, nextInAEL, this.m_UseFullRange) && tEdge.WindDelta != 0 && nextInAEL.WindDelta != 0)
					{
						OutPt op4 = this.AddOutPt(nextInAEL, tEdge.Bot);
						this.AddJoin(outPt, op4, tEdge.Top);
					}
				}
			}
		}

		private void DoMaxima(TEdge e)
		{
			TEdge maximaPair = this.GetMaximaPair(e);
			if (maximaPair == null)
			{
				if (e.OutIdx >= 0)
				{
					this.AddOutPt(e, e.Top);
				}
				this.DeleteFromAEL(e);
				return;
			}
			TEdge nextInAEL = e.NextInAEL;
			while (nextInAEL != null && nextInAEL != maximaPair)
			{
				this.IntersectEdges(e, nextInAEL, e.Top, true);
				this.SwapPositionsInAEL(e, nextInAEL);
				nextInAEL = e.NextInAEL;
			}
			if (e.OutIdx == -1 && maximaPair.OutIdx == -1)
			{
				this.DeleteFromAEL(e);
				this.DeleteFromAEL(maximaPair);
			}
			else
			{
				if (e.OutIdx < 0 || maximaPair.OutIdx < 0)
				{
					throw new ClipperException("DoMaxima error");
				}
				this.IntersectEdges(e, maximaPair, e.Top, false);
			}
		}

		public static void ReversePaths(List<List<IntPoint>> polys)
		{
			polys.ForEach(delegate(List<IntPoint> poly)
			{
				poly.Reverse();
			});
		}

		public static bool Orientation(List<IntPoint> poly)
		{
			return Clipper.Area(poly) >= 0.0;
		}

		private int PointCount(OutPt pts)
		{
			if (pts == null)
			{
				return 0;
			}
			int num = 0;
			OutPt outPt = pts;
			do
			{
				num++;
				outPt = outPt.Next;
			}
			while (outPt != pts);
			return num;
		}

		private void BuildResult(List<List<IntPoint>> polyg)
		{
			polyg.Clear();
			polyg.Capacity = this.m_PolyOuts.Count;
			for (int i = 0; i < this.m_PolyOuts.Count; i++)
			{
				OutRec outRec = this.m_PolyOuts[i];
				if (outRec.Pts != null)
				{
					OutPt outPt = outRec.Pts;
					int num = this.PointCount(outPt);
					if (num >= 2)
					{
						List<IntPoint> list = new List<IntPoint>(num);
						for (int j = 0; j < num; j++)
						{
							list.Add(outPt.Pt);
							outPt = outPt.Prev;
						}
						polyg.Add(list);
					}
				}
			}
		}

		private void BuildResult2(PolyTree polytree)
		{
			polytree.Clear();
			polytree.m_AllPolys.Capacity = this.m_PolyOuts.Count;
			for (int i = 0; i < this.m_PolyOuts.Count; i++)
			{
				OutRec outRec = this.m_PolyOuts[i];
				int num = this.PointCount(outRec.Pts);
				if ((!outRec.IsOpen || num >= 2) && (outRec.IsOpen || num >= 3))
				{
					this.FixHoleLinkage(outRec);
					PolyNode polyNode = new PolyNode();
					polytree.m_AllPolys.Add(polyNode);
					outRec.PolyNode = polyNode;
					polyNode.m_polygon.Capacity = num;
					OutPt prev = outRec.Pts.Prev;
					for (int j = 0; j < num; j++)
					{
						polyNode.m_polygon.Add(prev.Pt);
						prev = prev.Prev;
					}
				}
			}
			polytree.m_Childs.Capacity = this.m_PolyOuts.Count;
			for (int k = 0; k < this.m_PolyOuts.Count; k++)
			{
				OutRec outRec2 = this.m_PolyOuts[k];
				if (outRec2.PolyNode != null)
				{
					if (outRec2.IsOpen)
					{
						outRec2.PolyNode.IsOpen = true;
						polytree.AddChild(outRec2.PolyNode);
					}
					else if (outRec2.FirstLeft != null)
					{
						outRec2.FirstLeft.PolyNode.AddChild(outRec2.PolyNode);
					}
					else
					{
						polytree.AddChild(outRec2.PolyNode);
					}
				}
			}
		}

		private void FixupOutPolygon(OutRec outRec)
		{
			OutPt outPt = null;
			outRec.BottomPt = null;
			OutPt outPt2 = outRec.Pts;
			while (outPt2.Prev != outPt2 && outPt2.Prev != outPt2.Next)
			{
				if (outPt2.Pt == outPt2.Next.Pt || outPt2.Pt == outPt2.Prev.Pt || (ClipperBase.SlopesEqual(outPt2.Prev.Pt, outPt2.Pt, outPt2.Next.Pt, this.m_UseFullRange) && (!base.PreserveCollinear || !base.Pt2IsBetweenPt1AndPt3(outPt2.Prev.Pt, outPt2.Pt, outPt2.Next.Pt))))
				{
					outPt = null;
					outPt2.Prev.Next = outPt2.Next;
					outPt2.Next.Prev = outPt2.Prev;
					outPt2 = outPt2.Prev;
				}
				else
				{
					if (outPt2 == outPt)
					{
						outRec.Pts = outPt2;
						return;
					}
					if (outPt == null)
					{
						outPt = outPt2;
					}
					outPt2 = outPt2.Next;
				}
			}
			this.DisposeOutPts(outPt2);
			outRec.Pts = null;
		}

		private OutPt DupOutPt(OutPt outPt, bool InsertAfter)
		{
			OutPt outPt2 = new OutPt();
			outPt2.Pt = outPt.Pt;
			outPt2.Idx = outPt.Idx;
			if (InsertAfter)
			{
				outPt2.Next = outPt.Next;
				outPt2.Prev = outPt;
				outPt.Next.Prev = outPt2;
				outPt.Next = outPt2;
			}
			else
			{
				outPt2.Prev = outPt.Prev;
				outPt2.Next = outPt;
				outPt.Prev.Next = outPt2;
				outPt.Prev = outPt2;
			}
			return outPt2;
		}

		private bool GetOverlap(long a1, long a2, long b1, long b2, out long Left, out long Right)
		{
			if (a1 < a2)
			{
				if (b1 < b2)
				{
					Left = Math.Max(a1, b1);
					Right = Math.Min(a2, b2);
				}
				else
				{
					Left = Math.Max(a1, b2);
					Right = Math.Min(a2, b1);
				}
			}
			else if (b1 < b2)
			{
				Left = Math.Max(a2, b1);
				Right = Math.Min(a1, b2);
			}
			else
			{
				Left = Math.Max(a2, b2);
				Right = Math.Min(a1, b1);
			}
			return Left < Right;
		}

		private bool JoinHorz(OutPt op1, OutPt op1b, OutPt op2, OutPt op2b, IntPoint Pt, bool DiscardLeft)
		{
			Direction direction = (op1.Pt.X <= op1b.Pt.X) ? Direction.dLeftToRight : Direction.dRightToLeft;
			Direction direction2 = (op2.Pt.X <= op2b.Pt.X) ? Direction.dLeftToRight : Direction.dRightToLeft;
			if (direction == direction2)
			{
				return false;
			}
			if (direction == Direction.dLeftToRight)
			{
				while (op1.Next.Pt.X <= Pt.X && op1.Next.Pt.X >= op1.Pt.X && op1.Next.Pt.Y == Pt.Y)
				{
					op1 = op1.Next;
				}
				if (DiscardLeft && op1.Pt.X != Pt.X)
				{
					op1 = op1.Next;
				}
				op1b = this.DupOutPt(op1, !DiscardLeft);
				if (op1b.Pt != Pt)
				{
					op1 = op1b;
					op1.Pt = Pt;
					op1b = this.DupOutPt(op1, !DiscardLeft);
				}
			}
			else
			{
				while (op1.Next.Pt.X >= Pt.X && op1.Next.Pt.X <= op1.Pt.X && op1.Next.Pt.Y == Pt.Y)
				{
					op1 = op1.Next;
				}
				if (!DiscardLeft && op1.Pt.X != Pt.X)
				{
					op1 = op1.Next;
				}
				op1b = this.DupOutPt(op1, DiscardLeft);
				if (op1b.Pt != Pt)
				{
					op1 = op1b;
					op1.Pt = Pt;
					op1b = this.DupOutPt(op1, DiscardLeft);
				}
			}
			if (direction2 == Direction.dLeftToRight)
			{
				while (op2.Next.Pt.X <= Pt.X && op2.Next.Pt.X >= op2.Pt.X && op2.Next.Pt.Y == Pt.Y)
				{
					op2 = op2.Next;
				}
				if (DiscardLeft && op2.Pt.X != Pt.X)
				{
					op2 = op2.Next;
				}
				op2b = this.DupOutPt(op2, !DiscardLeft);
				if (op2b.Pt != Pt)
				{
					op2 = op2b;
					op2.Pt = Pt;
					op2b = this.DupOutPt(op2, !DiscardLeft);
				}
			}
			else
			{
				while (op2.Next.Pt.X >= Pt.X && op2.Next.Pt.X <= op2.Pt.X && op2.Next.Pt.Y == Pt.Y)
				{
					op2 = op2.Next;
				}
				if (!DiscardLeft && op2.Pt.X != Pt.X)
				{
					op2 = op2.Next;
				}
				op2b = this.DupOutPt(op2, DiscardLeft);
				if (op2b.Pt != Pt)
				{
					op2 = op2b;
					op2.Pt = Pt;
					op2b = this.DupOutPt(op2, DiscardLeft);
				}
			}
			if (direction == Direction.dLeftToRight == DiscardLeft)
			{
				op1.Prev = op2;
				op2.Next = op1;
				op1b.Next = op2b;
				op2b.Prev = op1b;
			}
			else
			{
				op1.Next = op2;
				op2.Prev = op1;
				op1b.Prev = op2b;
				op2b.Next = op1b;
			}
			return true;
		}

		private bool JoinPoints(Join j, out OutPt p1, out OutPt p2)
		{
			OutRec outRec = this.GetOutRec(j.OutPt1.Idx);
			OutRec outRec2 = this.GetOutRec(j.OutPt2.Idx);
			OutPt outPt = j.OutPt1;
			OutPt outPt2 = j.OutPt2;
			p1 = null;
			p2 = null;
			bool flag = j.OutPt1.Pt.Y == j.OffPt.Y;
			if (flag && j.OffPt == j.OutPt1.Pt && j.OffPt == j.OutPt2.Pt)
			{
				OutPt outPt3 = j.OutPt1.Next;
				while (outPt3 != outPt && outPt3.Pt == j.OffPt)
				{
					outPt3 = outPt3.Next;
				}
				bool flag2 = outPt3.Pt.Y > j.OffPt.Y;
				OutPt outPt4 = j.OutPt2.Next;
				while (outPt4 != outPt2 && outPt4.Pt == j.OffPt)
				{
					outPt4 = outPt4.Next;
				}
				bool flag3 = outPt4.Pt.Y > j.OffPt.Y;
				if (flag2 == flag3)
				{
					return false;
				}
				if (flag2)
				{
					outPt3 = this.DupOutPt(outPt, false);
					outPt4 = this.DupOutPt(outPt2, true);
					outPt.Prev = outPt2;
					outPt2.Next = outPt;
					outPt3.Next = outPt4;
					outPt4.Prev = outPt3;
					p1 = outPt;
					p2 = outPt3;
					return true;
				}
				outPt3 = this.DupOutPt(outPt, true);
				outPt4 = this.DupOutPt(outPt2, false);
				outPt.Next = outPt2;
				outPt2.Prev = outPt;
				outPt3.Prev = outPt4;
				outPt4.Next = outPt3;
				p1 = outPt;
				p2 = outPt3;
				return true;
			}
			else if (flag)
			{
				OutPt outPt3 = outPt;
				while (outPt.Prev.Pt.Y == outPt.Pt.Y && outPt.Prev != outPt3 && outPt.Prev != outPt2)
				{
					outPt = outPt.Prev;
				}
				while (outPt3.Next.Pt.Y == outPt3.Pt.Y && outPt3.Next != outPt && outPt3.Next != outPt2)
				{
					outPt3 = outPt3.Next;
				}
				if (outPt3.Next == outPt || outPt3.Next == outPt2)
				{
					return false;
				}
				OutPt outPt4 = outPt2;
				while (outPt2.Prev.Pt.Y == outPt2.Pt.Y && outPt2.Prev != outPt4 && outPt2.Prev != outPt3)
				{
					outPt2 = outPt2.Prev;
				}
				while (outPt4.Next.Pt.Y == outPt4.Pt.Y && outPt4.Next != outPt2 && outPt4.Next != outPt)
				{
					outPt4 = outPt4.Next;
				}
				if (outPt4.Next == outPt2 || outPt4.Next == outPt)
				{
					return false;
				}
				long num;
				long num2;
				if (!this.GetOverlap(outPt.Pt.X, outPt3.Pt.X, outPt2.Pt.X, outPt4.Pt.X, out num, out num2))
				{
					return false;
				}
				IntPoint pt;
				bool discardLeft;
				if (outPt.Pt.X >= num && outPt.Pt.X <= num2)
				{
					pt = outPt.Pt;
					discardLeft = (outPt.Pt.X > outPt3.Pt.X);
				}
				else if (outPt2.Pt.X >= num && outPt2.Pt.X <= num2)
				{
					pt = outPt2.Pt;
					discardLeft = (outPt2.Pt.X > outPt4.Pt.X);
				}
				else if (outPt3.Pt.X >= num && outPt3.Pt.X <= num2)
				{
					pt = outPt3.Pt;
					discardLeft = (outPt3.Pt.X > outPt.Pt.X);
				}
				else
				{
					pt = outPt4.Pt;
					discardLeft = (outPt4.Pt.X > outPt2.Pt.X);
				}
				p1 = outPt;
				p2 = outPt2;
				return this.JoinHorz(outPt, outPt3, outPt2, outPt4, pt, discardLeft);
			}
			else
			{
				OutPt outPt3 = outPt.Next;
				while (outPt3.Pt == outPt.Pt && outPt3 != outPt)
				{
					outPt3 = outPt3.Next;
				}
				bool flag4 = outPt3.Pt.Y > outPt.Pt.Y || !ClipperBase.SlopesEqual(outPt.Pt, outPt3.Pt, j.OffPt, this.m_UseFullRange);
				if (flag4)
				{
					outPt3 = outPt.Prev;
					while (outPt3.Pt == outPt.Pt && outPt3 != outPt)
					{
						outPt3 = outPt3.Prev;
					}
					if (outPt3.Pt.Y > outPt.Pt.Y || !ClipperBase.SlopesEqual(outPt.Pt, outPt3.Pt, j.OffPt, this.m_UseFullRange))
					{
						return false;
					}
				}
				OutPt outPt4 = outPt2.Next;
				while (outPt4.Pt == outPt2.Pt && outPt4 != outPt2)
				{
					outPt4 = outPt4.Next;
				}
				bool flag5 = outPt4.Pt.Y > outPt2.Pt.Y || !ClipperBase.SlopesEqual(outPt2.Pt, outPt4.Pt, j.OffPt, this.m_UseFullRange);
				if (flag5)
				{
					outPt4 = outPt2.Prev;
					while (outPt4.Pt == outPt2.Pt && outPt4 != outPt2)
					{
						outPt4 = outPt4.Prev;
					}
					if (outPt4.Pt.Y > outPt2.Pt.Y || !ClipperBase.SlopesEqual(outPt2.Pt, outPt4.Pt, j.OffPt, this.m_UseFullRange))
					{
						return false;
					}
				}
				if (outPt3 == outPt || outPt4 == outPt2 || outPt3 == outPt4 || (outRec == outRec2 && flag4 == flag5))
				{
					return false;
				}
				if (flag4)
				{
					outPt3 = this.DupOutPt(outPt, false);
					outPt4 = this.DupOutPt(outPt2, true);
					outPt.Prev = outPt2;
					outPt2.Next = outPt;
					outPt3.Next = outPt4;
					outPt4.Prev = outPt3;
					p1 = outPt;
					p2 = outPt3;
					return true;
				}
				outPt3 = this.DupOutPt(outPt, true);
				outPt4 = this.DupOutPt(outPt2, false);
				outPt.Next = outPt2;
				outPt2.Prev = outPt;
				outPt3.Prev = outPt4;
				outPt4.Next = outPt3;
				p1 = outPt;
				p2 = outPt3;
				return true;
			}
		}

		private bool Poly2ContainsPoly1(OutPt outPt1, OutPt outPt2, bool UseFullRange)
		{
			OutPt outPt3 = outPt1;
			if (base.PointOnPolygon(outPt3.Pt, outPt2, UseFullRange))
			{
				outPt3 = outPt3.Next;
				while (outPt3 != outPt1 && base.PointOnPolygon(outPt3.Pt, outPt2, UseFullRange))
				{
					outPt3 = outPt3.Next;
				}
				if (outPt3 == outPt1)
				{
					return true;
				}
			}
			return base.PointInPolygon(outPt3.Pt, outPt2, UseFullRange);
		}

		private void FixupFirstLefts1(OutRec OldOutRec, OutRec NewOutRec)
		{
			for (int i = 0; i < this.m_PolyOuts.Count; i++)
			{
				OutRec outRec = this.m_PolyOuts[i];
				if (outRec.Pts != null && outRec.FirstLeft == OldOutRec && this.Poly2ContainsPoly1(outRec.Pts, NewOutRec.Pts, this.m_UseFullRange))
				{
					outRec.FirstLeft = NewOutRec;
				}
			}
		}

		private void FixupFirstLefts2(OutRec OldOutRec, OutRec NewOutRec)
		{
			foreach (OutRec current in this.m_PolyOuts)
			{
				if (current.FirstLeft == OldOutRec)
				{
					current.FirstLeft = NewOutRec;
				}
			}
		}

		private void JoinCommonEdges()
		{
			for (int i = 0; i < this.m_Joins.Count; i++)
			{
				Join join = this.m_Joins[i];
				OutRec outRec = this.GetOutRec(join.OutPt1.Idx);
				OutRec outRec2 = this.GetOutRec(join.OutPt2.Idx);
				if (outRec.Pts != null && outRec2.Pts != null)
				{
					OutRec outRec3;
					if (outRec == outRec2)
					{
						outRec3 = outRec;
					}
					else if (this.Param1RightOfParam2(outRec, outRec2))
					{
						outRec3 = outRec2;
					}
					else if (this.Param1RightOfParam2(outRec2, outRec))
					{
						outRec3 = outRec;
					}
					else
					{
						outRec3 = this.GetLowermostRec(outRec, outRec2);
					}
					OutPt pts;
					OutPt pts2;
					if (this.JoinPoints(join, out pts, out pts2))
					{
						if (outRec == outRec2)
						{
							outRec.Pts = pts;
							outRec.BottomPt = null;
							outRec2 = this.CreateOutRec();
							outRec2.Pts = pts2;
							this.UpdateOutPtIdxs(outRec2);
							if (this.Poly2ContainsPoly1(outRec2.Pts, outRec.Pts, this.m_UseFullRange))
							{
								outRec2.IsHole = !outRec.IsHole;
								outRec2.FirstLeft = outRec;
								if (this.m_UsingPolyTree)
								{
									this.FixupFirstLefts2(outRec2, outRec);
								}
								if ((outRec2.IsHole ^ this.ReverseSolution) == this.Area(outRec2) > 0.0)
								{
									this.ReversePolyPtLinks(outRec2.Pts);
								}
							}
							else if (this.Poly2ContainsPoly1(outRec.Pts, outRec2.Pts, this.m_UseFullRange))
							{
								outRec2.IsHole = outRec.IsHole;
								outRec.IsHole = !outRec2.IsHole;
								outRec2.FirstLeft = outRec.FirstLeft;
								outRec.FirstLeft = outRec2;
								if (this.m_UsingPolyTree)
								{
									this.FixupFirstLefts2(outRec, outRec2);
								}
								if ((outRec.IsHole ^ this.ReverseSolution) == this.Area(outRec) > 0.0)
								{
									this.ReversePolyPtLinks(outRec.Pts);
								}
							}
							else
							{
								outRec2.IsHole = outRec.IsHole;
								outRec2.FirstLeft = outRec.FirstLeft;
								if (this.m_UsingPolyTree)
								{
									this.FixupFirstLefts1(outRec, outRec2);
								}
							}
						}
						else
						{
							outRec2.Pts = null;
							outRec2.BottomPt = null;
							outRec2.Idx = outRec.Idx;
							outRec.IsHole = outRec3.IsHole;
							if (outRec3 == outRec2)
							{
								outRec.FirstLeft = outRec2.FirstLeft;
							}
							outRec2.FirstLeft = outRec;
							if (this.m_UsingPolyTree)
							{
								this.FixupFirstLefts2(outRec2, outRec);
							}
						}
					}
				}
			}
		}

		private void UpdateOutPtIdxs(OutRec outrec)
		{
			OutPt outPt = outrec.Pts;
			do
			{
				outPt.Idx = outrec.Idx;
				outPt = outPt.Prev;
			}
			while (outPt != outrec.Pts);
		}

		private void DoSimplePolygons()
		{
			int i = 0;
			while (i < this.m_PolyOuts.Count)
			{
				OutRec outRec = this.m_PolyOuts[i++];
				OutPt outPt = outRec.Pts;
				if (outPt != null)
				{
					do
					{
						for (OutPt outPt2 = outPt.Next; outPt2 != outRec.Pts; outPt2 = outPt2.Next)
						{
							if (outPt.Pt == outPt2.Pt && outPt2.Next != outPt && outPt2.Prev != outPt)
							{
								OutPt prev = outPt.Prev;
								OutPt prev2 = outPt2.Prev;
								outPt.Prev = prev2;
								prev2.Next = outPt;
								outPt2.Prev = prev;
								prev.Next = outPt2;
								outRec.Pts = outPt;
								OutRec outRec2 = this.CreateOutRec();
								outRec2.Pts = outPt2;
								this.UpdateOutPtIdxs(outRec2);
								if (this.Poly2ContainsPoly1(outRec2.Pts, outRec.Pts, this.m_UseFullRange))
								{
									outRec2.IsHole = !outRec.IsHole;
									outRec2.FirstLeft = outRec;
								}
								else if (this.Poly2ContainsPoly1(outRec.Pts, outRec2.Pts, this.m_UseFullRange))
								{
									outRec2.IsHole = outRec.IsHole;
									outRec.IsHole = !outRec2.IsHole;
									outRec2.FirstLeft = outRec.FirstLeft;
									outRec.FirstLeft = outRec2;
								}
								else
								{
									outRec2.IsHole = outRec.IsHole;
									outRec2.FirstLeft = outRec.FirstLeft;
								}
								outPt2 = outPt;
							}
						}
						outPt = outPt.Next;
					}
					while (outPt != outRec.Pts);
				}
			}
		}

		public static double Area(List<IntPoint> poly)
		{
			int num = poly.Count - 1;
			if (num < 2)
			{
				return 0.0;
			}
			double num2 = ((double)poly[num].X + (double)poly[0].X) * ((double)poly[0].Y - (double)poly[num].Y);
			for (int i = 1; i <= num; i++)
			{
				num2 += ((double)poly[i - 1].X + (double)poly[i].X) * ((double)poly[i].Y - (double)poly[i - 1].Y);
			}
			return num2 / 2.0;
		}

		private double Area(OutRec outRec)
		{
			OutPt outPt = outRec.Pts;
			if (outPt == null)
			{
				return 0.0;
			}
			double num = 0.0;
			do
			{
				num += (double)(outPt.Pt.X + outPt.Prev.Pt.X) * (double)(outPt.Prev.Pt.Y - outPt.Pt.Y);
				outPt = outPt.Next;
			}
			while (outPt != outRec.Pts);
			return num / 2.0;
		}

		internal static DoublePoint GetUnitNormal(IntPoint pt1, IntPoint pt2)
		{
			double num = (double)(pt2.X - pt1.X);
			double num2 = (double)(pt2.Y - pt1.Y);
			if (num == 0.0 && num2 == 0.0)
			{
				return default(DoublePoint);
			}
			double num3 = 1.0 / Math.Sqrt(num * num + num2 * num2);
			num *= num3;
			num2 *= num3;
			return new DoublePoint(num2, -num);
		}

		internal static bool UpdateBotPt(IntPoint pt, ref IntPoint botPt)
		{
			if (pt.Y > botPt.Y || (pt.Y == botPt.Y && pt.X < botPt.X))
			{
				botPt = pt;
				return true;
			}
			return false;
		}

		internal static bool StripDupsAndGetBotPt(List<IntPoint> in_path, List<IntPoint> out_path, bool closed, out IntPoint botPt)
		{
			botPt = new IntPoint(0L, 0L);
			int num = in_path.Count;
			if (closed)
			{
				while (num > 0 && in_path[0] == in_path[num - 1])
				{
					num--;
				}
			}
			if (num == 0)
			{
				return false;
			}
			out_path.Capacity = num;
			int num2 = 0;
			out_path.Add(in_path[0]);
			botPt = in_path[0];
			for (int i = 1; i < num; i++)
			{
				if (in_path[i] != out_path[num2])
				{
					out_path.Add(in_path[i]);
					num2++;
					if (out_path[num2].Y > botPt.Y || (out_path[num2].Y == botPt.Y && out_path[num2].X < botPt.X))
					{
						botPt = out_path[num2];
					}
				}
			}
			num2++;
			if (num2 < 2 || (closed && num2 == 2))
			{
				num2 = 0;
			}
			while (out_path.Count > num2)
			{
				out_path.RemoveAt(num2);
			}
			return num2 > 0;
		}

		public static List<List<IntPoint>> OffsetPaths(List<List<IntPoint>> polys, double delta, JoinType jointype, EndType endtype, double MiterLimit)
		{
			List<List<IntPoint>> list = new List<List<IntPoint>>(polys.Count);
			IntPoint intPoint = default(IntPoint);
			int num = -1;
			for (int i = 0; i < polys.Count; i++)
			{
				list.Add(new List<IntPoint>());
				IntPoint intPoint2;
				if (Clipper.StripDupsAndGetBotPt(polys[i], list[i], endtype == EndType.etClosed, out intPoint2) && (num < 0 || intPoint2.Y > intPoint.Y || (intPoint2.Y == intPoint.Y && intPoint2.X < intPoint.X)))
				{
					intPoint = intPoint2;
					num = i;
				}
			}
			if (endtype == EndType.etClosed && num >= 0 && !Clipper.Orientation(list[num]))
			{
				Clipper.ReversePaths(list);
			}
			List<List<IntPoint>> result;
			new Clipper.PolyOffsetBuilder(list, ref result, delta, jointype, endtype, MiterLimit);
			return result;
		}

		public static List<List<IntPoint>> OffsetPolygons(List<List<IntPoint>> poly, double delta, JoinType jointype, double MiterLimit, bool AutoFix)
		{
			return Clipper.OffsetPaths(poly, delta, jointype, EndType.etClosed, MiterLimit);
		}

		public static List<List<IntPoint>> OffsetPolygons(List<List<IntPoint>> poly, double delta, JoinType jointype, double MiterLimit)
		{
			return Clipper.OffsetPaths(poly, delta, jointype, EndType.etClosed, MiterLimit);
		}

		public static List<List<IntPoint>> OffsetPolygons(List<List<IntPoint>> polys, double delta, JoinType jointype)
		{
			return Clipper.OffsetPaths(polys, delta, jointype, EndType.etClosed, 0.0);
		}

		public static List<List<IntPoint>> OffsetPolygons(List<List<IntPoint>> polys, double delta)
		{
			return Clipper.OffsetPolygons(polys, delta, JoinType.jtSquare, 0.0, true);
		}

		public static void ReversePolygons(List<List<IntPoint>> polys)
		{
			polys.ForEach(delegate(List<IntPoint> poly)
			{
				poly.Reverse();
			});
		}

		public static void PolyTreeToPolygons(PolyTree polytree, List<List<IntPoint>> polys)
		{
			polys.Clear();
			polys.Capacity = polytree.Total;
			Clipper.AddPolyNodeToPaths(polytree, Clipper.NodeType.ntAny, polys);
		}

		public static List<List<IntPoint>> SimplifyPolygon(List<IntPoint> poly, PolyFillType fillType = PolyFillType.pftEvenOdd)
		{
			List<List<IntPoint>> list = new List<List<IntPoint>>();
			Clipper clipper = new Clipper(0);
			clipper.StrictlySimple = true;
			clipper.AddPath(poly, PolyType.ptSubject, true);
			clipper.Execute(ClipType.ctUnion, list, fillType, fillType);
			return list;
		}

		public static List<List<IntPoint>> SimplifyPolygons(List<List<IntPoint>> polys, PolyFillType fillType = PolyFillType.pftEvenOdd)
		{
			List<List<IntPoint>> list = new List<List<IntPoint>>();
			Clipper clipper = new Clipper(0);
			clipper.StrictlySimple = true;
			clipper.AddPaths(polys, PolyType.ptSubject, true);
			clipper.Execute(ClipType.ctUnion, list, fillType, fillType);
			return list;
		}

		private static double DistanceSqrd(IntPoint pt1, IntPoint pt2)
		{
			double num = (double)pt1.X - (double)pt2.X;
			double num2 = (double)pt1.Y - (double)pt2.Y;
			return num * num + num2 * num2;
		}

		private static DoublePoint ClosestPointOnLine(IntPoint pt, IntPoint linePt1, IntPoint linePt2)
		{
			double num = (double)linePt2.X - (double)linePt1.X;
			double num2 = (double)linePt2.Y - (double)linePt1.Y;
			if (num == 0.0 && num2 == 0.0)
			{
				return new DoublePoint((double)linePt1.X, (double)linePt1.Y);
			}
			double num3 = ((double)(pt.X - linePt1.X) * num + (double)(pt.Y - linePt1.Y) * num2) / (num * num + num2 * num2);
			return new DoublePoint((1.0 - num3) * (double)linePt1.X + num3 * (double)linePt2.X, (1.0 - num3) * (double)linePt1.Y + num3 * (double)linePt2.Y);
		}

		private static bool SlopesNearCollinear(IntPoint pt1, IntPoint pt2, IntPoint pt3, double distSqrd)
		{
			if (Clipper.DistanceSqrd(pt1, pt2) > Clipper.DistanceSqrd(pt1, pt3))
			{
				return false;
			}
			DoublePoint doublePoint = Clipper.ClosestPointOnLine(pt2, pt1, pt3);
			double num = (double)pt2.X - doublePoint.X;
			double num2 = (double)pt2.Y - doublePoint.Y;
			return num * num + num2 * num2 < distSqrd;
		}

		private static bool PointsAreClose(IntPoint pt1, IntPoint pt2, double distSqrd)
		{
			double num = (double)pt1.X - (double)pt2.X;
			double num2 = (double)pt1.Y - (double)pt2.Y;
			return num * num + num2 * num2 <= distSqrd;
		}

		public static List<IntPoint> CleanPolygon(List<IntPoint> path, double distance = 1.415)
		{
			double distSqrd = distance * distance;
			int num = path.Count - 1;
			List<IntPoint> list = new List<IntPoint>(num + 1);
			while (num > 0 && Clipper.PointsAreClose(path[num], path[0], distSqrd))
			{
				num--;
			}
			if (num < 2)
			{
				return list;
			}
			IntPoint intPoint = path[num];
			int num2 = 0;
			while (true)
			{
				while (num2 < num && Clipper.PointsAreClose(intPoint, path[num2], distSqrd))
				{
					num2 += 2;
				}
				int num3 = num2;
				while (num2 < num && (Clipper.PointsAreClose(path[num2], path[num2 + 1], distSqrd) || Clipper.SlopesNearCollinear(intPoint, path[num2], path[num2 + 1], distSqrd)))
				{
					num2++;
				}
				if (num2 >= num)
				{
					break;
				}
				if (num2 == num3)
				{
					intPoint = path[num2++];
					list.Add(intPoint);
				}
			}
			if (num2 <= num)
			{
				list.Add(path[num2]);
			}
			num2 = list.Count;
			if (num2 > 2 && Clipper.SlopesNearCollinear(list[num2 - 2], list[num2 - 1], list[0], distSqrd))
			{
				list.RemoveAt(num2 - 1);
			}
			if (list.Count < 3)
			{
				list.Clear();
			}
			return list;
		}

		internal static List<List<IntPoint>> Minkowki(List<IntPoint> poly, List<IntPoint> path, bool IsSum, bool IsClosed)
		{
			int num = (!IsClosed) ? 0 : 1;
			int count = poly.Count;
			int count2 = path.Count;
			List<List<IntPoint>> list = new List<List<IntPoint>>(count2);
			if (IsSum)
			{
				for (int i = 0; i < count2; i++)
				{
					List<IntPoint> list2 = new List<IntPoint>(count);
					foreach (IntPoint current in poly)
					{
						list2.Add(new IntPoint(path[i].X + current.X, path[i].Y + current.Y));
					}
					list.Add(list2);
				}
			}
			else
			{
				for (int j = 0; j < count2; j++)
				{
					List<IntPoint> list3 = new List<IntPoint>(count);
					foreach (IntPoint current2 in poly)
					{
						list3.Add(new IntPoint(path[j].X - current2.X, path[j].Y - current2.Y));
					}
					list.Add(list3);
				}
			}
			List<List<IntPoint>> list4 = new List<List<IntPoint>>((count2 + num) * (count + 1));
			for (int k = 0; k <= count2 - 2 + num; k++)
			{
				for (int l = 0; l <= count - 1; l++)
				{
					List<IntPoint> list5 = new List<IntPoint>(4);
					list5.Add(list[k % count2][l % count]);
					list5.Add(list[(k + 1) % count2][l % count]);
					list5.Add(list[(k + 1) % count2][(l + 1) % count]);
					list5.Add(list[k % count2][(l + 1) % count]);
					if (!Clipper.Orientation(list5))
					{
						list5.Reverse();
					}
					list4.Add(list5);
				}
			}
			Clipper clipper = new Clipper(0);
			clipper.AddPaths(list4, PolyType.ptSubject, true);
			clipper.Execute(ClipType.ctUnion, list, PolyFillType.pftNonZero, PolyFillType.pftNonZero);
			return list;
		}

		public static List<List<IntPoint>> MinkowkiSum(List<IntPoint> poly, List<IntPoint> path, bool IsClosed)
		{
			return Clipper.Minkowki(poly, path, true, IsClosed);
		}

		public static List<List<IntPoint>> MinkowkiDiff(List<IntPoint> poly, List<IntPoint> path, bool IsClosed)
		{
			return Clipper.Minkowki(poly, path, false, IsClosed);
		}

		public static List<List<IntPoint>> CleanPolygons(List<List<IntPoint>> polys, double distance = 1.415)
		{
			List<List<IntPoint>> list = new List<List<IntPoint>>(polys.Count);
			for (int i = 0; i < polys.Count; i++)
			{
				list.Add(Clipper.CleanPolygon(polys[i], distance));
			}
			return list;
		}

		public static List<List<IntPoint>> PolyTreeToPaths(PolyTree polytree)
		{
			List<List<IntPoint>> list = new List<List<IntPoint>>();
			list.Capacity = polytree.Total;
			Clipper.AddPolyNodeToPaths(polytree, Clipper.NodeType.ntAny, list);
			return list;
		}

		internal static void AddPolyNodeToPaths(PolyNode polynode, Clipper.NodeType nt, List<List<IntPoint>> paths)
		{
			bool flag = true;
			if (nt != Clipper.NodeType.ntOpen)
			{
				if (nt == Clipper.NodeType.ntClosed)
				{
					flag = !polynode.IsOpen;
				}
				if (polynode.Contour.Count > 0 && flag)
				{
					paths.Add(polynode.Contour);
				}
				foreach (PolyNode current in polynode.Childs)
				{
					Clipper.AddPolyNodeToPaths(current, nt, paths);
				}
				return;
			}
		}

		public static List<List<IntPoint>> OpenPathsFromPolyTree(PolyTree polytree)
		{
			List<List<IntPoint>> list = new List<List<IntPoint>>();
			list.Capacity = polytree.ChildCount;
			for (int i = 0; i < polytree.ChildCount; i++)
			{
				if (polytree.Childs[i].IsOpen)
				{
					list.Add(polytree.Childs[i].Contour);
				}
			}
			return list;
		}

		public static List<List<IntPoint>> ClosedPathsFromPolyTree(PolyTree polytree)
		{
			List<List<IntPoint>> list = new List<List<IntPoint>>();
			list.Capacity = polytree.Total;
			Clipper.AddPolyNodeToPaths(polytree, Clipper.NodeType.ntClosed, list);
			return list;
		}
	}
}

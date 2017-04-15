using System;
using System.Collections.Generic;

namespace Pathfinding.ClipperLib
{
	public class ClipperBase
	{
		protected const double horizontal = -3.4E+38;

		protected const int Skip = -2;

		protected const int Unassigned = -1;

		protected const double tolerance = 1E-20;

		internal const long loRange = 1073741823L;

		internal const long hiRange = 4611686018427387903L;

		internal LocalMinima m_MinimaList;

		internal LocalMinima m_CurrentLM;

		internal List<List<TEdge>> m_edges = new List<List<TEdge>>();

		internal bool m_UseFullRange;

		internal bool m_HasOpenPaths;

		public bool PreserveCollinear
		{
			get;
			set;
		}

		internal ClipperBase()
		{
			this.m_MinimaList = null;
			this.m_CurrentLM = null;
			this.m_UseFullRange = false;
			this.m_HasOpenPaths = false;
		}

		internal static bool near_zero(double val)
		{
			return val > -1E-20 && val < 1E-20;
		}

		internal static bool IsHorizontal(TEdge e)
		{
			return e.Delta.Y == 0L;
		}

		internal bool PointIsVertex(IntPoint pt, OutPt pp)
		{
			OutPt outPt = pp;
			while (!(outPt.Pt == pt))
			{
				outPt = outPt.Next;
				if (outPt == pp)
				{
					return false;
				}
			}
			return true;
		}

		internal bool PointOnLineSegment(IntPoint pt, IntPoint linePt1, IntPoint linePt2, bool UseFullRange)
		{
			if (UseFullRange)
			{
				return (pt.X == linePt1.X && pt.Y == linePt1.Y) || (pt.X == linePt2.X && pt.Y == linePt2.Y) || (pt.X > linePt1.X == pt.X < linePt2.X && pt.Y > linePt1.Y == pt.Y < linePt2.Y && Int128.Int128Mul(pt.X - linePt1.X, linePt2.Y - linePt1.Y) == Int128.Int128Mul(linePt2.X - linePt1.X, pt.Y - linePt1.Y));
			}
			return (pt.X == linePt1.X && pt.Y == linePt1.Y) || (pt.X == linePt2.X && pt.Y == linePt2.Y) || (pt.X > linePt1.X == pt.X < linePt2.X && pt.Y > linePt1.Y == pt.Y < linePt2.Y && (pt.X - linePt1.X) * (linePt2.Y - linePt1.Y) == (linePt2.X - linePt1.X) * (pt.Y - linePt1.Y));
		}

		internal bool PointOnPolygon(IntPoint pt, OutPt pp, bool UseFullRange)
		{
			OutPt outPt = pp;
			while (!this.PointOnLineSegment(pt, outPt.Pt, outPt.Next.Pt, UseFullRange))
			{
				outPt = outPt.Next;
				if (outPt == pp)
				{
					return false;
				}
			}
			return true;
		}

		internal bool PointInPolygon(IntPoint pt, OutPt pp, bool UseFullRange)
		{
			OutPt outPt = pp;
			bool flag = false;
			if (UseFullRange)
			{
				do
				{
					if (outPt.Pt.Y > pt.Y != outPt.Prev.Pt.Y > pt.Y && new Int128(pt.X - outPt.Pt.X) < Int128.Int128Mul(outPt.Prev.Pt.X - outPt.Pt.X, pt.Y - outPt.Pt.Y) / new Int128(outPt.Prev.Pt.Y - outPt.Pt.Y))
					{
						flag = !flag;
					}
					outPt = outPt.Next;
				}
				while (outPt != pp);
			}
			else
			{
				do
				{
					if (outPt.Pt.Y > pt.Y != outPt.Prev.Pt.Y > pt.Y && pt.X - outPt.Pt.X < (outPt.Prev.Pt.X - outPt.Pt.X) * (pt.Y - outPt.Pt.Y) / (outPt.Prev.Pt.Y - outPt.Pt.Y))
					{
						flag = !flag;
					}
					outPt = outPt.Next;
				}
				while (outPt != pp);
			}
			return flag;
		}

		internal static bool SlopesEqual(TEdge e1, TEdge e2, bool UseFullRange)
		{
			if (UseFullRange)
			{
				return Int128.Int128Mul(e1.Delta.Y, e2.Delta.X) == Int128.Int128Mul(e1.Delta.X, e2.Delta.Y);
			}
			return e1.Delta.Y * e2.Delta.X == e1.Delta.X * e2.Delta.Y;
		}

		protected static bool SlopesEqual(IntPoint pt1, IntPoint pt2, IntPoint pt3, bool UseFullRange)
		{
			if (UseFullRange)
			{
				return Int128.Int128Mul(pt1.Y - pt2.Y, pt2.X - pt3.X) == Int128.Int128Mul(pt1.X - pt2.X, pt2.Y - pt3.Y);
			}
			return (pt1.Y - pt2.Y) * (pt2.X - pt3.X) - (pt1.X - pt2.X) * (pt2.Y - pt3.Y) == 0L;
		}

		protected static bool SlopesEqual(IntPoint pt1, IntPoint pt2, IntPoint pt3, IntPoint pt4, bool UseFullRange)
		{
			if (UseFullRange)
			{
				return Int128.Int128Mul(pt1.Y - pt2.Y, pt3.X - pt4.X) == Int128.Int128Mul(pt1.X - pt2.X, pt3.Y - pt4.Y);
			}
			return (pt1.Y - pt2.Y) * (pt3.X - pt4.X) - (pt1.X - pt2.X) * (pt3.Y - pt4.Y) == 0L;
		}

		public virtual void Clear()
		{
			this.DisposeLocalMinimaList();
			for (int i = 0; i < this.m_edges.Count; i++)
			{
				for (int j = 0; j < this.m_edges[i].Count; j++)
				{
					this.m_edges[i][j] = null;
				}
				this.m_edges[i].Clear();
			}
			this.m_edges.Clear();
			this.m_UseFullRange = false;
			this.m_HasOpenPaths = false;
		}

		private void DisposeLocalMinimaList()
		{
			while (this.m_MinimaList != null)
			{
				LocalMinima next = this.m_MinimaList.Next;
				this.m_MinimaList = null;
				this.m_MinimaList = next;
			}
			this.m_CurrentLM = null;
		}

		private void RangeTest(IntPoint Pt, ref bool useFullRange)
		{
			if (useFullRange)
			{
				if (Pt.X > 4611686018427387903L || Pt.Y > 4611686018427387903L || -Pt.X > 4611686018427387903L || -Pt.Y > 4611686018427387903L)
				{
					throw new ClipperException("Coordinate outside allowed range");
				}
			}
			else if (Pt.X > 1073741823L || Pt.Y > 1073741823L || -Pt.X > 1073741823L || -Pt.Y > 1073741823L)
			{
				useFullRange = true;
				this.RangeTest(Pt, ref useFullRange);
			}
		}

		private void InitEdge(TEdge e, TEdge eNext, TEdge ePrev, IntPoint pt)
		{
			e.Next = eNext;
			e.Prev = ePrev;
			e.Curr = pt;
			e.OutIdx = -1;
		}

		private void InitEdge2(TEdge e, PolyType polyType)
		{
			if (e.Curr.Y >= e.Next.Curr.Y)
			{
				e.Bot = e.Curr;
				e.Top = e.Next.Curr;
			}
			else
			{
				e.Top = e.Curr;
				e.Bot = e.Next.Curr;
			}
			this.SetDx(e);
			e.PolyTyp = polyType;
		}

		public bool AddPath(List<IntPoint> pg, PolyType polyType, bool Closed)
		{
			if (!Closed)
			{
				throw new ClipperException("AddPath: Open paths have been disabled.");
			}
			int num = pg.Count - 1;
			bool flag = num > 0 && (Closed || pg[0] == pg[num]);
			while (num > 0 && pg[num] == pg[0])
			{
				num--;
			}
			while (num > 0 && pg[num] == pg[num - 1])
			{
				num--;
			}
			if ((Closed && num < 2) || (!Closed && num < 1))
			{
				return false;
			}
			List<TEdge> list = new List<TEdge>(num + 1);
			for (int i = 0; i <= num; i++)
			{
				list.Add(new TEdge());
			}
			try
			{
				list[1].Curr = pg[1];
				this.RangeTest(pg[0], ref this.m_UseFullRange);
				this.RangeTest(pg[num], ref this.m_UseFullRange);
				this.InitEdge(list[0], list[1], list[num], pg[0]);
				this.InitEdge(list[num], list[0], list[num - 1], pg[num]);
				for (int j = num - 1; j >= 1; j--)
				{
					this.RangeTest(pg[j], ref this.m_UseFullRange);
					this.InitEdge(list[j], list[j + 1], list[j - 1], pg[j]);
				}
			}
			catch
			{
				bool result = false;
				return result;
			}
			TEdge tEdge = list[0];
			if (!flag)
			{
				tEdge.Prev.OutIdx = -2;
			}
			TEdge tEdge2 = tEdge;
			TEdge tEdge3 = tEdge;
			while (true)
			{
				if (tEdge2.Curr == tEdge2.Next.Curr)
				{
					if (tEdge2 == tEdge2.Next)
					{
						break;
					}
					if (tEdge2 == tEdge)
					{
						tEdge = tEdge2.Next;
					}
					tEdge2 = this.RemoveEdge(tEdge2);
					tEdge3 = tEdge2;
				}
				else
				{
					if (tEdge2.Prev == tEdge2.Next)
					{
						break;
					}
					if ((flag || (tEdge2.Prev.OutIdx != -2 && tEdge2.OutIdx != -2 && tEdge2.Next.OutIdx != -2)) && ClipperBase.SlopesEqual(tEdge2.Prev.Curr, tEdge2.Curr, tEdge2.Next.Curr, this.m_UseFullRange) && Closed && (!this.PreserveCollinear || !this.Pt2IsBetweenPt1AndPt3(tEdge2.Prev.Curr, tEdge2.Curr, tEdge2.Next.Curr)))
					{
						if (tEdge2 == tEdge)
						{
							tEdge = tEdge2.Next;
						}
						tEdge2 = this.RemoveEdge(tEdge2);
						tEdge2 = tEdge2.Prev;
						tEdge3 = tEdge2;
					}
					else
					{
						tEdge2 = tEdge2.Next;
						if (tEdge2 == tEdge3)
						{
							break;
						}
					}
				}
			}
			if ((!Closed && tEdge2 == tEdge2.Next) || (Closed && tEdge2.Prev == tEdge2.Next))
			{
				return false;
			}
			this.m_edges.Add(list);
			if (!Closed)
			{
				this.m_HasOpenPaths = true;
			}
			TEdge tEdge4 = tEdge;
			tEdge2 = tEdge;
			do
			{
				this.InitEdge2(tEdge2, polyType);
				if (tEdge2.Top.Y < tEdge4.Top.Y)
				{
					tEdge4 = tEdge2;
				}
				tEdge2 = tEdge2.Next;
			}
			while (tEdge2 != tEdge);
			if (this.AllHorizontal(tEdge2))
			{
				if (flag)
				{
					tEdge2.Prev.OutIdx = -2;
				}
				this.AscendToMax(ref tEdge2, false, false);
				return true;
			}
			tEdge2 = tEdge.Prev;
			if (tEdge2.Prev == tEdge2.Next)
			{
				tEdge4 = tEdge2.Next;
			}
			else if (!flag && tEdge2.Top.Y == tEdge4.Top.Y)
			{
				if ((ClipperBase.IsHorizontal(tEdge2) || ClipperBase.IsHorizontal(tEdge2.Next)) && tEdge2.Next.Bot.Y == tEdge4.Top.Y)
				{
					tEdge4 = tEdge2.Next;
				}
				else if (this.SharedVertWithPrevAtTop(tEdge2))
				{
					tEdge4 = tEdge2;
				}
				else if (tEdge2.Top == tEdge2.Prev.Top)
				{
					tEdge4 = tEdge2.Prev;
				}
				else
				{
					tEdge4 = tEdge2.Next;
				}
			}
			else
			{
				tEdge2 = tEdge4;
				while (ClipperBase.IsHorizontal(tEdge4) || tEdge4.Top == tEdge4.Next.Top || tEdge4.Top == tEdge4.Next.Bot)
				{
					tEdge4 = tEdge4.Next;
					if (tEdge4 == tEdge2)
					{
						while (ClipperBase.IsHorizontal(tEdge4) || !this.SharedVertWithPrevAtTop(tEdge4))
						{
							tEdge4 = tEdge4.Next;
						}
						break;
					}
				}
			}
			tEdge2 = tEdge4;
			do
			{
				tEdge2 = this.AddBoundsToLML(tEdge2, Closed);
			}
			while (tEdge2 != tEdge4);
			return true;
		}

		public bool AddPaths(List<List<IntPoint>> ppg, PolyType polyType, bool closed)
		{
			bool result = false;
			for (int i = 0; i < ppg.Count; i++)
			{
				if (this.AddPath(ppg[i], polyType, closed))
				{
					result = true;
				}
			}
			return result;
		}

		public bool AddPolygon(List<IntPoint> pg, PolyType polyType)
		{
			return this.AddPath(pg, polyType, true);
		}

		public bool AddPolygons(List<List<IntPoint>> ppg, PolyType polyType)
		{
			bool result = false;
			for (int i = 0; i < ppg.Count; i++)
			{
				if (this.AddPath(ppg[i], polyType, true))
				{
					result = true;
				}
			}
			return result;
		}

		internal bool Pt2IsBetweenPt1AndPt3(IntPoint pt1, IntPoint pt2, IntPoint pt3)
		{
			if (pt1 == pt3 || pt1 == pt2 || pt3 == pt2)
			{
				return false;
			}
			if (pt1.X != pt3.X)
			{
				return pt2.X > pt1.X == pt2.X < pt3.X;
			}
			return pt2.Y > pt1.Y == pt2.Y < pt3.Y;
		}

		private TEdge RemoveEdge(TEdge e)
		{
			e.Prev.Next = e.Next;
			e.Next.Prev = e.Prev;
			TEdge next = e.Next;
			e.Prev = null;
			return next;
		}

		private TEdge GetLastHorz(TEdge Edge)
		{
			TEdge tEdge = Edge;
			while (tEdge.OutIdx != -2 && tEdge.Next != Edge && ClipperBase.IsHorizontal(tEdge.Next))
			{
				tEdge = tEdge.Next;
			}
			return tEdge;
		}

		private bool SharedVertWithPrevAtTop(TEdge Edge)
		{
			TEdge tEdge = Edge;
			bool flag = true;
			while (tEdge.Prev != Edge)
			{
				if (tEdge.Top == tEdge.Prev.Top)
				{
					if (tEdge.Bot == tEdge.Prev.Bot)
					{
						tEdge = tEdge.Prev;
						continue;
					}
					flag = true;
				}
				else
				{
					flag = false;
				}
				break;
			}
			while (tEdge != Edge)
			{
				flag = !flag;
				tEdge = tEdge.Next;
			}
			return flag;
		}

		private bool SharedVertWithNextIsBot(TEdge Edge)
		{
			bool flag = true;
			TEdge tEdge = Edge;
			while (tEdge.Prev != Edge)
			{
				bool flag2 = tEdge.Next.Bot == tEdge.Bot;
				bool flag3 = tEdge.Prev.Bot == tEdge.Bot;
				if (flag2 != flag3)
				{
					flag = flag2;
					break;
				}
				flag2 = (tEdge.Next.Top == tEdge.Top);
				flag3 = (tEdge.Prev.Top == tEdge.Top);
				if (flag2 != flag3)
				{
					flag = flag3;
					break;
				}
				tEdge = tEdge.Prev;
			}
			while (tEdge != Edge)
			{
				flag = !flag;
				tEdge = tEdge.Next;
			}
			return flag;
		}

		private bool MoreBelow(TEdge Edge)
		{
			TEdge tEdge = Edge;
			if (ClipperBase.IsHorizontal(tEdge))
			{
				while (ClipperBase.IsHorizontal(tEdge.Next))
				{
					tEdge = tEdge.Next;
				}
				return tEdge.Next.Bot.Y > tEdge.Bot.Y;
			}
			if (ClipperBase.IsHorizontal(tEdge.Next))
			{
				while (ClipperBase.IsHorizontal(tEdge.Next))
				{
					tEdge = tEdge.Next;
				}
				return tEdge.Next.Bot.Y > tEdge.Bot.Y;
			}
			return tEdge.Bot == tEdge.Next.Top;
		}

		private bool JustBeforeLocMin(TEdge Edge)
		{
			TEdge tEdge = Edge;
			if (ClipperBase.IsHorizontal(tEdge))
			{
				while (ClipperBase.IsHorizontal(tEdge.Next))
				{
					tEdge = tEdge.Next;
				}
				return tEdge.Next.Top.Y < tEdge.Bot.Y;
			}
			return this.SharedVertWithNextIsBot(tEdge);
		}

		private bool MoreAbove(TEdge Edge)
		{
			if (ClipperBase.IsHorizontal(Edge))
			{
				Edge = this.GetLastHorz(Edge);
				return Edge.Next.Top.Y < Edge.Top.Y;
			}
			if (ClipperBase.IsHorizontal(Edge.Next))
			{
				Edge = this.GetLastHorz(Edge.Next);
				return Edge.Next.Top.Y < Edge.Top.Y;
			}
			return Edge.Next.Top.Y < Edge.Top.Y;
		}

		private bool AllHorizontal(TEdge Edge)
		{
			if (!ClipperBase.IsHorizontal(Edge))
			{
				return false;
			}
			for (TEdge next = Edge.Next; next != Edge; next = next.Next)
			{
				if (!ClipperBase.IsHorizontal(next))
				{
					return false;
				}
			}
			return true;
		}

		private void SetDx(TEdge e)
		{
			e.Delta.X = e.Top.X - e.Bot.X;
			e.Delta.Y = e.Top.Y - e.Bot.Y;
			if (e.Delta.Y == 0L)
			{
				e.Dx = -3.4E+38;
			}
			else
			{
				e.Dx = (double)e.Delta.X / (double)e.Delta.Y;
			}
		}

		private void DoMinimaLML(TEdge E1, TEdge E2, bool IsClosed)
		{
			if (E1 == null)
			{
				if (E2 == null)
				{
					return;
				}
				LocalMinima localMinima = new LocalMinima();
				localMinima.Next = null;
				localMinima.Y = E2.Bot.Y;
				localMinima.LeftBound = null;
				E2.WindDelta = 0;
				localMinima.RightBound = E2;
				this.InsertLocalMinima(localMinima);
			}
			else
			{
				LocalMinima localMinima2 = new LocalMinima();
				localMinima2.Y = E1.Bot.Y;
				localMinima2.Next = null;
				if (ClipperBase.IsHorizontal(E2))
				{
					if (E2.Bot.X != E1.Bot.X)
					{
						this.ReverseHorizontal(E2);
					}
					localMinima2.LeftBound = E1;
					localMinima2.RightBound = E2;
				}
				else if (E2.Dx < E1.Dx)
				{
					localMinima2.LeftBound = E1;
					localMinima2.RightBound = E2;
				}
				else
				{
					localMinima2.LeftBound = E2;
					localMinima2.RightBound = E1;
				}
				localMinima2.LeftBound.Side = EdgeSide.esLeft;
				localMinima2.RightBound.Side = EdgeSide.esRight;
				if (!IsClosed)
				{
					localMinima2.LeftBound.WindDelta = 0;
				}
				else if (localMinima2.LeftBound.Next == localMinima2.RightBound)
				{
					localMinima2.LeftBound.WindDelta = -1;
				}
				else
				{
					localMinima2.LeftBound.WindDelta = 1;
				}
				localMinima2.RightBound.WindDelta = -localMinima2.LeftBound.WindDelta;
				this.InsertLocalMinima(localMinima2);
			}
		}

		private TEdge DescendToMin(ref TEdge E)
		{
			E.NextInLML = null;
			if (ClipperBase.IsHorizontal(E))
			{
				TEdge tEdge = E;
				while (ClipperBase.IsHorizontal(tEdge.Next))
				{
					tEdge = tEdge.Next;
				}
				if (tEdge.Bot != tEdge.Next.Top)
				{
					this.ReverseHorizontal(E);
				}
			}
			while (true)
			{
				E = E.Next;
				if (E.OutIdx == -2)
				{
					break;
				}
				if (ClipperBase.IsHorizontal(E))
				{
					TEdge tEdge = this.GetLastHorz(E);
					if (tEdge == E.Prev || (tEdge.Next.Top.Y < E.Top.Y && tEdge.Next.Bot.X > E.Prev.Bot.X))
					{
						break;
					}
					if (E.Top.X != E.Prev.Bot.X)
					{
						this.ReverseHorizontal(E);
					}
					if (tEdge.OutIdx == -2)
					{
						tEdge = tEdge.Prev;
					}
					while (E != tEdge)
					{
						E.NextInLML = E.Prev;
						E = E.Next;
						if (E.Top.X != E.Prev.Bot.X)
						{
							this.ReverseHorizontal(E);
						}
					}
				}
				else if (E.Bot.Y == E.Prev.Bot.Y)
				{
					break;
				}
				E.NextInLML = E.Prev;
			}
			return E.Prev;
		}

		private void AscendToMax(ref TEdge E, bool Appending, bool IsClosed)
		{
			if (E.OutIdx == -2)
			{
				E = E.Next;
				if (!this.MoreAbove(E.Prev))
				{
					return;
				}
			}
			if (ClipperBase.IsHorizontal(E) && Appending && E.Bot != E.Prev.Bot)
			{
				this.ReverseHorizontal(E);
			}
			TEdge tEdge = E;
			while (E.Next.OutIdx != -2 && (E.Next.Top.Y != E.Top.Y || ClipperBase.IsHorizontal(E.Next)))
			{
				E.NextInLML = E.Next;
				E = E.Next;
				if (ClipperBase.IsHorizontal(E) && E.Bot.X != E.Prev.Top.X)
				{
					this.ReverseHorizontal(E);
				}
			}
			if (!Appending)
			{
				if (tEdge.OutIdx == -2)
				{
					tEdge = tEdge.Next;
				}
				if (tEdge != E.Next)
				{
					this.DoMinimaLML(null, tEdge, IsClosed);
				}
			}
			E = E.Next;
		}

		private TEdge AddBoundsToLML(TEdge E, bool Closed)
		{
			TEdge tEdge;
			if (E.OutIdx == -2)
			{
				if (this.MoreBelow(E))
				{
					E = E.Next;
					tEdge = this.DescendToMin(ref E);
				}
				else
				{
					tEdge = null;
				}
			}
			else
			{
				tEdge = this.DescendToMin(ref E);
			}
			bool appending;
			if (E.OutIdx == -2)
			{
				this.DoMinimaLML(null, tEdge, Closed);
				appending = false;
				if (E.Bot != E.Prev.Bot && this.MoreBelow(E))
				{
					E = E.Next;
					tEdge = this.DescendToMin(ref E);
					this.DoMinimaLML(tEdge, E, Closed);
					appending = true;
				}
				else if (this.JustBeforeLocMin(E))
				{
					E = E.Next;
				}
			}
			else
			{
				this.DoMinimaLML(tEdge, E, Closed);
				appending = true;
			}
			this.AscendToMax(ref E, appending, Closed);
			if (E.OutIdx == -2 && E.Top != E.Prev.Top)
			{
				if (this.MoreAbove(E))
				{
					E = E.Next;
					this.AscendToMax(ref E, false, Closed);
				}
				else if (E.Top == E.Next.Top || (ClipperBase.IsHorizontal(E.Next) && E.Top == E.Next.Bot))
				{
					E = E.Next;
				}
			}
			return E;
		}

		private void InsertLocalMinima(LocalMinima newLm)
		{
			if (this.m_MinimaList == null)
			{
				this.m_MinimaList = newLm;
			}
			else if (newLm.Y >= this.m_MinimaList.Y)
			{
				newLm.Next = this.m_MinimaList;
				this.m_MinimaList = newLm;
			}
			else
			{
				LocalMinima localMinima = this.m_MinimaList;
				while (localMinima.Next != null && newLm.Y < localMinima.Next.Y)
				{
					localMinima = localMinima.Next;
				}
				newLm.Next = localMinima.Next;
				localMinima.Next = newLm;
			}
		}

		protected void PopLocalMinima()
		{
			if (this.m_CurrentLM == null)
			{
				return;
			}
			this.m_CurrentLM = this.m_CurrentLM.Next;
		}

		private void ReverseHorizontal(TEdge e)
		{
			long x = e.Top.X;
			e.Top.X = e.Bot.X;
			e.Bot.X = x;
		}

		protected virtual void Reset()
		{
			this.m_CurrentLM = this.m_MinimaList;
			if (this.m_CurrentLM == null)
			{
				return;
			}
			for (LocalMinima localMinima = this.m_MinimaList; localMinima != null; localMinima = localMinima.Next)
			{
				TEdge tEdge = localMinima.LeftBound;
				if (tEdge != null)
				{
					tEdge.Curr = tEdge.Bot;
					tEdge.Side = EdgeSide.esLeft;
					if (tEdge.OutIdx != -2)
					{
						tEdge.OutIdx = -1;
					}
				}
				tEdge = localMinima.RightBound;
				tEdge.Curr = tEdge.Bot;
				tEdge.Side = EdgeSide.esRight;
				if (tEdge.OutIdx != -2)
				{
					tEdge.OutIdx = -1;
				}
			}
		}

		public IntRect GetBounds()
		{
			IntRect result = default(IntRect);
			LocalMinima localMinima = this.m_MinimaList;
			if (localMinima == null)
			{
				return result;
			}
			result.left = localMinima.LeftBound.Bot.X;
			result.top = localMinima.LeftBound.Bot.Y;
			result.right = localMinima.LeftBound.Bot.X;
			result.bottom = localMinima.LeftBound.Bot.Y;
			while (localMinima != null)
			{
				if (localMinima.LeftBound.Bot.Y > result.bottom)
				{
					result.bottom = localMinima.LeftBound.Bot.Y;
				}
				TEdge tEdge = localMinima.LeftBound;
				while (true)
				{
					TEdge tEdge2 = tEdge;
					while (tEdge.NextInLML != null)
					{
						if (tEdge.Bot.X < result.left)
						{
							result.left = tEdge.Bot.X;
						}
						if (tEdge.Bot.X > result.right)
						{
							result.right = tEdge.Bot.X;
						}
						tEdge = tEdge.NextInLML;
					}
					if (tEdge.Bot.X < result.left)
					{
						result.left = tEdge.Bot.X;
					}
					if (tEdge.Bot.X > result.right)
					{
						result.right = tEdge.Bot.X;
					}
					if (tEdge.Top.X < result.left)
					{
						result.left = tEdge.Top.X;
					}
					if (tEdge.Top.X > result.right)
					{
						result.right = tEdge.Top.X;
					}
					if (tEdge.Top.Y < result.top)
					{
						result.top = tEdge.Top.Y;
					}
					if (tEdge2 != localMinima.LeftBound)
					{
						break;
					}
					tEdge = localMinima.RightBound;
				}
				localMinima = localMinima.Next;
			}
			return result;
		}
	}
}

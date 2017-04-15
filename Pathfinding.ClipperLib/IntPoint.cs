using System;

namespace Pathfinding.ClipperLib
{
	public struct IntPoint
	{
		public long X;

		public long Y;

		public IntPoint(long X, long Y)
		{
			this.X = X;
			this.Y = Y;
		}

		public IntPoint(double x, double y)
		{
			this.X = (long)x;
			this.Y = (long)y;
		}

		public IntPoint(IntPoint pt)
		{
			this.X = pt.X;
			this.Y = pt.Y;
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return false;
			}
			if (obj is IntPoint)
			{
				IntPoint intPoint = (IntPoint)obj;
				return this.X == intPoint.X && this.Y == intPoint.Y;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public static bool operator ==(IntPoint a, IntPoint b)
		{
			return a.X == b.X && a.Y == b.Y;
		}

		public static bool operator !=(IntPoint a, IntPoint b)
		{
			return a.X != b.X || a.Y != b.Y;
		}
	}
}

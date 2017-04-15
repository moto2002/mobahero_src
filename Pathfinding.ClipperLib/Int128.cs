using System;

namespace Pathfinding.ClipperLib
{
	internal struct Int128
	{
		private long hi;

		private ulong lo;

		public Int128(long _lo)
		{
			this.lo = (ulong)_lo;
			if (_lo < 0L)
			{
				this.hi = -1L;
			}
			else
			{
				this.hi = 0L;
			}
		}

		public Int128(long _hi, ulong _lo)
		{
			this.lo = _lo;
			this.hi = _hi;
		}

		public Int128(Int128 val)
		{
			this.hi = val.hi;
			this.lo = val.lo;
		}

		public bool IsNegative()
		{
			return this.hi < 0L;
		}

		public override bool Equals(object obj)
		{
			if (obj == null || !(obj is Int128))
			{
				return false;
			}
			Int128 @int = (Int128)obj;
			return @int.hi == this.hi && @int.lo == this.lo;
		}

		public override int GetHashCode()
		{
			return this.hi.GetHashCode() ^ this.lo.GetHashCode();
		}

		public static Int128 Int128Mul(long lhs, long rhs)
		{
			bool flag = lhs < 0L != rhs < 0L;
			if (lhs < 0L)
			{
				lhs = -lhs;
			}
			if (rhs < 0L)
			{
				rhs = -rhs;
			}
			ulong num = (ulong)lhs >> 32;
			ulong num2 = (ulong)(lhs & (long)((ulong)-1));
			ulong num3 = (ulong)rhs >> 32;
			ulong num4 = (ulong)(rhs & (long)((ulong)-1));
			ulong num5 = num * num3;
			ulong num6 = num2 * num4;
			ulong num7 = num * num4 + num2 * num3;
			long num8 = (long)(num5 + (num7 >> 32));
			ulong num9 = (num7 << 32) + num6;
			if (num9 < num6)
			{
				num8 += 1L;
			}
			Int128 @int = new Int128(num8, num9);
			return (!flag) ? @int : (-@int);
		}

		public double ToDouble()
		{
			if (this.hi >= 0L)
			{
				return this.lo + (double)this.hi * 1.8446744073709552E+19;
			}
			if (this.lo == 0uL)
			{
				return (double)this.hi * 1.8446744073709552E+19;
			}
			return -(~this.lo + (double)(~(double)this.hi) * 1.8446744073709552E+19);
		}

		public static bool operator ==(Int128 val1, Int128 val2)
		{
			return val1 == val2 || (val1 != null && val2 != null && val1.hi == val2.hi && val1.lo == val2.lo);
		}

		public static bool operator !=(Int128 val1, Int128 val2)
		{
			return !(val1 == val2);
		}

		public static bool operator >(Int128 val1, Int128 val2)
		{
			if (val1.hi != val2.hi)
			{
				return val1.hi > val2.hi;
			}
			return val1.lo > val2.lo;
		}

		public static bool operator <(Int128 val1, Int128 val2)
		{
			if (val1.hi != val2.hi)
			{
				return val1.hi < val2.hi;
			}
			return val1.lo < val2.lo;
		}

		public static Int128 operator +(Int128 lhs, Int128 rhs)
		{
			lhs.hi += rhs.hi;
			lhs.lo += rhs.lo;
			if (lhs.lo < rhs.lo)
			{
				lhs.hi += 1L;
			}
			return lhs;
		}

		public static Int128 operator -(Int128 lhs, Int128 rhs)
		{
			return lhs + -rhs;
		}

		public static Int128 operator -(Int128 val)
		{
			if (val.lo == 0uL)
			{
				return new Int128(-val.hi, 0uL);
			}
			return new Int128(~val.hi, ~val.lo + 1uL);
		}

		public static Int128 operator /(Int128 lhs, Int128 rhs)
		{
			if (rhs.lo == 0uL && rhs.hi == 0L)
			{
				throw new ClipperException("Int128: divide by zero");
			}
			bool flag = rhs.hi < 0L != lhs.hi < 0L;
			if (lhs.hi < 0L)
			{
				lhs = -lhs;
			}
			if (rhs.hi < 0L)
			{
				rhs = -rhs;
			}
			if (rhs < lhs)
			{
				Int128 @int = new Int128(0L);
				Int128 int2 = new Int128(1L);
				while (rhs.hi >= 0L && !(rhs > lhs))
				{
					rhs.hi <<= 1;
					if (rhs.lo < 0uL)
					{
						rhs.hi += 1L;
					}
					rhs.lo <<= 1;
					int2.hi <<= 1;
					if (int2.lo < 0uL)
					{
						int2.hi += 1L;
					}
					int2.lo <<= 1;
				}
				rhs.lo >>= 1;
				if ((rhs.hi & 1L) == 1L)
				{
					rhs.lo |= 9223372036854775808uL;
				}
				rhs.hi = (long)((ulong)rhs.hi >> 1);
				int2.lo >>= 1;
				if ((int2.hi & 1L) == 1L)
				{
					int2.lo |= 9223372036854775808uL;
				}
				int2.hi >>= 1;
				while (int2.hi != 0L || int2.lo != 0uL)
				{
					if (!(lhs < rhs))
					{
						lhs -= rhs;
						@int.hi |= int2.hi;
						@int.lo |= int2.lo;
					}
					rhs.lo >>= 1;
					if ((rhs.hi & 1L) == 1L)
					{
						rhs.lo |= 9223372036854775808uL;
					}
					rhs.hi >>= 1;
					int2.lo >>= 1;
					if ((int2.hi & 1L) == 1L)
					{
						int2.lo |= 9223372036854775808uL;
					}
					int2.hi >>= 1;
				}
				return (!flag) ? @int : (-@int);
			}
			if (rhs == lhs)
			{
				return new Int128(1L);
			}
			return new Int128(0L);
		}
	}
}

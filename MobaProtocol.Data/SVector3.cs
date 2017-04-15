using ProtoBuf;
using System;
using System.Diagnostics;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class SVector3
	{
		public float y
		{
			get
			{
				return 0f;
			}
			set
			{
			}
		}

		[ProtoMember(1)]
		public short _x
		{
			get;
			set;
		}

		public float x
		{
			get
			{
				return (float)this._x / 100f;
			}
			set
			{
				Debug.Assert(-32700f < value && value < 32700f);
				this._x = (short)(value * 100f);
			}
		}

		[ProtoMember(2)]
		public short _z
		{
			get;
			set;
		}

		public float z
		{
			get
			{
				return (float)this._z / 100f;
			}
			set
			{
				Debug.Assert(-32700f < value && value < 32700f);
				this._z = (short)(value * 100f);
			}
		}

		public float Magnitude
		{
			get
			{
				return this.x * this.x + this.z * this.z;
			}
		}

		public float SqrtMagnitude
		{
			get
			{
				return (float)Math.Sqrt((double)this.Magnitude);
			}
		}

		public static SVector3 operator -(SVector3 lhs, SVector3 rhs)
		{
			SVector3 sVector = new SVector3
			{
				x = lhs.x,
				y = lhs.y,
				z = lhs.z
			};
			sVector.x -= rhs.x;
			sVector.y -= rhs.y;
			sVector.z -= rhs.z;
			return sVector;
		}

		public static SVector3 operator +(SVector3 lhs, SVector3 rhs)
		{
			SVector3 sVector = new SVector3
			{
				x = lhs.x,
				y = lhs.y,
				z = lhs.z
			};
			sVector.x += rhs.x;
			sVector.y += rhs.y;
			sVector.z += rhs.z;
			return sVector;
		}

		public static SVector3 operator *(SVector3 lhs, float rhs)
		{
			SVector3 sVector = new SVector3
			{
				x = lhs.x,
				y = lhs.y,
				z = lhs.z
			};
			sVector.x *= rhs;
			sVector.y *= rhs;
			sVector.z *= rhs;
			return sVector;
		}

		public override bool Equals(object obj)
		{
			SVector3 sVector = obj as SVector3;
			return sVector != null && sVector.x == this.x && sVector.y == this.y && sVector.z == this.z;
		}

		public override string ToString()
		{
			return string.Concat(new object[]
			{
				"[",
				this.x,
				",",
				this.z,
				"]"
			});
		}

		public static SVector3 Build(float x, float y, float z)
		{
			return new SVector3
			{
				x = x,
				y = y,
				z = z
			};
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}

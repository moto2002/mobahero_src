using System;
using System.IO;

namespace SevenZip.Compression.RangeCoder
{
	internal class Decoder
	{
		public const uint kTopValue = 16777216u;

		public uint Range;

		public uint Code;

		public Stream Stream;

		public void Init(Stream stream)
		{
			this.Stream = stream;
			this.Code = 0u;
			this.Range = 4294967295u;
			for (int i = 0; i < 5; i++)
			{
				this.Code = (this.Code << 8 | (uint)((byte)this.Stream.ReadByte()));
			}
		}

		public void ReleaseStream()
		{
			this.Stream = null;
		}

		public void CloseStream()
		{
			this.Stream.Close();
		}

		public void Normalize()
		{
			while (this.Range < 16777216u)
			{
				this.Code = (this.Code << 8 | (uint)((byte)this.Stream.ReadByte()));
				this.Range <<= 8;
			}
		}

		public void Normalize2()
		{
			if (this.Range < 16777216u)
			{
				this.Code = (this.Code << 8 | (uint)((byte)this.Stream.ReadByte()));
				this.Range <<= 8;
			}
		}

		public uint GetThreshold(uint total)
		{
			return this.Code / (this.Range /= total);
		}

		public void Decode(uint start, uint size, uint total)
		{
			this.Code -= start * this.Range;
			this.Range *= size;
			this.Normalize();
		}

		public uint DecodeDirectBits(int numTotalBits)
		{
			uint num = this.Range;
			uint num2 = this.Code;
			uint num3 = 0u;
			for (int i = numTotalBits; i > 0; i--)
			{
				num >>= 1;
				uint num4 = num2 - num >> 31;
				num2 -= (num & num4 - 1u);
				num3 = (num3 << 1 | 1u - num4);
				if (num < 16777216u)
				{
					num2 = (num2 << 8 | (uint)((byte)this.Stream.ReadByte()));
					num <<= 8;
				}
			}
			this.Range = num;
			this.Code = num2;
			return num3;
		}

		public uint DecodeBit(uint size0, int numTotalBits)
		{
			uint num = (this.Range >> numTotalBits) * size0;
			uint result;
			if (this.Code < num)
			{
				result = 0u;
				this.Range = num;
			}
			else
			{
				result = 1u;
				this.Code -= num;
				this.Range -= num;
			}
			this.Normalize();
			return result;
		}
	}
}

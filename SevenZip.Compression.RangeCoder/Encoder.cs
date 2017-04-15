using System;
using System.IO;

namespace SevenZip.Compression.RangeCoder
{
	internal class Encoder
	{
		public const uint kTopValue = 16777216u;

		private Stream Stream;

		public ulong Low;

		public uint Range;

		private uint _cacheSize;

		private byte _cache;

		private long StartPosition;

		public void SetStream(Stream stream)
		{
			this.Stream = stream;
		}

		public void ReleaseStream()
		{
			this.Stream = null;
		}

		public void Init()
		{
			this.StartPosition = this.Stream.Position;
			this.Low = 0uL;
			this.Range = 4294967295u;
			this._cacheSize = 1u;
			this._cache = 0;
		}

		public void FlushData()
		{
			for (int i = 0; i < 5; i++)
			{
				this.ShiftLow();
			}
		}

		public void FlushStream()
		{
			this.Stream.Flush();
		}

		public void CloseStream()
		{
			this.Stream.Close();
		}

		public void Encode(uint start, uint size, uint total)
		{
			this.Low += (ulong)(start * (this.Range /= total));
			this.Range *= size;
			while (this.Range < 16777216u)
			{
				this.Range <<= 8;
				this.ShiftLow();
			}
		}

		public void ShiftLow()
		{
			if ((uint)this.Low < 4278190080u || (uint)(this.Low >> 32) == 1u)
			{
				byte b = this._cache;
				do
				{
					this.Stream.WriteByte((byte)((ulong)b + (this.Low >> 32)));
					b = 255;
				}
				while ((this._cacheSize -= 1u) != 0u);
				this._cache = (byte)((uint)this.Low >> 24);
			}
			this._cacheSize += 1u;
			this.Low = (ulong)((ulong)((uint)this.Low) << 8);
		}

		public void EncodeDirectBits(uint v, int numTotalBits)
		{
			for (int i = numTotalBits - 1; i >= 0; i--)
			{
				this.Range >>= 1;
				if ((v >> i & 1u) == 1u)
				{
					this.Low += (ulong)this.Range;
				}
				if (this.Range < 16777216u)
				{
					this.Range <<= 8;
					this.ShiftLow();
				}
			}
		}

		public void EncodeBit(uint size0, int numTotalBits, uint symbol)
		{
			uint num = (this.Range >> numTotalBits) * size0;
			if (symbol == 0u)
			{
				this.Range = num;
			}
			else
			{
				this.Low += (ulong)num;
				this.Range -= num;
			}
			while (this.Range < 16777216u)
			{
				this.Range <<= 8;
				this.ShiftLow();
			}
		}

		public long GetProcessedSizeAdd()
		{
			return (long)((ulong)this._cacheSize + (ulong)this.Stream.Position - (ulong)this.StartPosition + 4uL);
		}
	}
}

using System;

namespace SevenZip.Compression.RangeCoder
{
	internal struct BitTreeDecoder
	{
		private BitDecoder[] Models;

		private int NumBitLevels;

		public BitTreeDecoder(int numBitLevels)
		{
			this.NumBitLevels = numBitLevels;
			this.Models = new BitDecoder[1 << numBitLevels];
		}

		public void Init()
		{
			uint num = 1u;
			while ((ulong)num < (ulong)(1L << (this.NumBitLevels & 31)))
			{
				this.Models[(int)((UIntPtr)num)].Init();
				num += 1u;
			}
		}

		public uint Decode(Decoder rangeDecoder)
		{
			uint num = 1u;
			for (int i = this.NumBitLevels; i > 0; i--)
			{
				num = (num << 1) + this.Models[(int)((UIntPtr)num)].Decode(rangeDecoder);
			}
			return num - (1u << this.NumBitLevels);
		}

		public uint ReverseDecode(Decoder rangeDecoder)
		{
			uint num = 1u;
			uint num2 = 0u;
			for (int i = 0; i < this.NumBitLevels; i++)
			{
				uint num3 = this.Models[(int)((UIntPtr)num)].Decode(rangeDecoder);
				num <<= 1;
				num += num3;
				num2 |= num3 << i;
			}
			return num2;
		}

		public static uint ReverseDecode(BitDecoder[] Models, uint startIndex, Decoder rangeDecoder, int NumBitLevels)
		{
			uint num = 1u;
			uint num2 = 0u;
			for (int i = 0; i < NumBitLevels; i++)
			{
				uint num3 = Models[(int)((UIntPtr)(startIndex + num))].Decode(rangeDecoder);
				num <<= 1;
				num += num3;
				num2 |= num3 << i;
			}
			return num2;
		}
	}
}

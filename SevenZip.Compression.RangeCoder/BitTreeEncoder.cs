using System;

namespace SevenZip.Compression.RangeCoder
{
	internal struct BitTreeEncoder
	{
		private BitEncoder[] Models;

		private int NumBitLevels;

		public BitTreeEncoder(int numBitLevels)
		{
			this.NumBitLevels = numBitLevels;
			this.Models = new BitEncoder[1 << numBitLevels];
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

		public void Encode(Encoder rangeEncoder, uint symbol)
		{
			uint num = 1u;
			int i = this.NumBitLevels;
			while (i > 0)
			{
				i--;
				uint num2 = symbol >> i & 1u;
				this.Models[(int)((UIntPtr)num)].Encode(rangeEncoder, num2);
				num = (num << 1 | num2);
			}
		}

		public void ReverseEncode(Encoder rangeEncoder, uint symbol)
		{
			uint num = 1u;
			uint num2 = 0u;
			while ((ulong)num2 < (ulong)((long)this.NumBitLevels))
			{
				uint num3 = symbol & 1u;
				this.Models[(int)((UIntPtr)num)].Encode(rangeEncoder, num3);
				num = (num << 1 | num3);
				symbol >>= 1;
				num2 += 1u;
			}
		}

		public uint GetPrice(uint symbol)
		{
			uint num = 0u;
			uint num2 = 1u;
			int i = this.NumBitLevels;
			while (i > 0)
			{
				i--;
				uint num3 = symbol >> i & 1u;
				num += this.Models[(int)((UIntPtr)num2)].GetPrice(num3);
				num2 = (num2 << 1) + num3;
			}
			return num;
		}

		public uint ReverseGetPrice(uint symbol)
		{
			uint num = 0u;
			uint num2 = 1u;
			for (int i = this.NumBitLevels; i > 0; i--)
			{
				uint num3 = symbol & 1u;
				symbol >>= 1;
				num += this.Models[(int)((UIntPtr)num2)].GetPrice(num3);
				num2 = (num2 << 1 | num3);
			}
			return num;
		}

		public static uint ReverseGetPrice(BitEncoder[] Models, uint startIndex, int NumBitLevels, uint symbol)
		{
			uint num = 0u;
			uint num2 = 1u;
			for (int i = NumBitLevels; i > 0; i--)
			{
				uint num3 = symbol & 1u;
				symbol >>= 1;
				num += Models[(int)((UIntPtr)(startIndex + num2))].GetPrice(num3);
				num2 = (num2 << 1 | num3);
			}
			return num;
		}

		public static void ReverseEncode(BitEncoder[] Models, uint startIndex, Encoder rangeEncoder, int NumBitLevels, uint symbol)
		{
			uint num = 1u;
			for (int i = 0; i < NumBitLevels; i++)
			{
				uint num2 = symbol & 1u;
				Models[(int)((UIntPtr)(startIndex + num))].Encode(rangeEncoder, num2);
				num = (num << 1 | num2);
				symbol >>= 1;
			}
		}
	}
}

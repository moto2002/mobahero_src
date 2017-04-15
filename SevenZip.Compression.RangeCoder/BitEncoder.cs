using System;

namespace SevenZip.Compression.RangeCoder
{
	internal struct BitEncoder
	{
		public const int kNumBitModelTotalBits = 11;

		public const uint kBitModelTotal = 2048u;

		private const int kNumMoveBits = 5;

		private const int kNumMoveReducingBits = 2;

		public const int kNumBitPriceShiftBits = 6;

		private uint Prob;

		private static uint[] ProbPrices;

		static BitEncoder()
		{
			BitEncoder.ProbPrices = new uint[512];
			for (int i = 8; i >= 0; i--)
			{
				uint num = 1u << 9 - i - 1;
				uint num2 = 1u << 9 - i;
				for (uint num3 = num; num3 < num2; num3 += 1u)
				{
					BitEncoder.ProbPrices[(int)((UIntPtr)num3)] = (uint)((i << 6) + (int)(num2 - num3 << 6 >> 9 - i - 1));
				}
			}
		}

		public void Init()
		{
			this.Prob = 1024u;
		}

		public void UpdateModel(uint symbol)
		{
			if (symbol == 0u)
			{
				this.Prob += 2048u - this.Prob >> 5;
			}
			else
			{
				this.Prob -= this.Prob >> 5;
			}
		}

		public void Encode(Encoder encoder, uint symbol)
		{
			uint num = (encoder.Range >> 11) * this.Prob;
			if (symbol == 0u)
			{
				encoder.Range = num;
				this.Prob += 2048u - this.Prob >> 5;
			}
			else
			{
				encoder.Low += (ulong)num;
				encoder.Range -= num;
				this.Prob -= this.Prob >> 5;
			}
			if (encoder.Range < 16777216u)
			{
				encoder.Range <<= 8;
				encoder.ShiftLow();
			}
		}

		public uint GetPrice(uint symbol)
		{
			return BitEncoder.ProbPrices[(int)(checked((IntPtr)((unchecked((ulong)(this.Prob - symbol) ^ (ulong)((long)(-(long)symbol))) & 2047uL) >> 2)))];
		}

		public uint GetPrice0()
		{
			return BitEncoder.ProbPrices[(int)((UIntPtr)(this.Prob >> 2))];
		}

		public uint GetPrice1()
		{
			return BitEncoder.ProbPrices[(int)((UIntPtr)(2048u - this.Prob >> 2))];
		}
	}
}

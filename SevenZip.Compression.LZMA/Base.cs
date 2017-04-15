using System;

namespace SevenZip.Compression.LZMA
{
	internal abstract class Base
	{
		public struct State
		{
			public uint Index;

			public void Init()
			{
				this.Index = 0u;
			}

			public void UpdateChar()
			{
				if (this.Index < 4u)
				{
					this.Index = 0u;
				}
				else if (this.Index < 10u)
				{
					this.Index -= 3u;
				}
				else
				{
					this.Index -= 6u;
				}
			}

			public void UpdateMatch()
			{
				this.Index = ((this.Index >= 7u) ? 10u : 7u);
			}

			public void UpdateRep()
			{
				this.Index = ((this.Index >= 7u) ? 11u : 8u);
			}

			public void UpdateShortRep()
			{
				this.Index = ((this.Index >= 7u) ? 11u : 9u);
			}

			public bool IsCharState()
			{
				return this.Index < 7u;
			}
		}

		public const uint kNumRepDistances = 4u;

		public const uint kNumStates = 12u;

		public const int kNumPosSlotBits = 6;

		public const int kDicLogSizeMin = 0;

		public const int kNumLenToPosStatesBits = 2;

		public const uint kNumLenToPosStates = 4u;

		public const uint kMatchMinLen = 2u;

		public const int kNumAlignBits = 4;

		public const uint kAlignTableSize = 16u;

		public const uint kAlignMask = 15u;

		public const uint kStartPosModelIndex = 4u;

		public const uint kEndPosModelIndex = 14u;

		public const uint kNumPosModels = 10u;

		public const uint kNumFullDistances = 128u;

		public const uint kNumLitPosStatesBitsEncodingMax = 4u;

		public const uint kNumLitContextBitsMax = 8u;

		public const int kNumPosStatesBitsMax = 4;

		public const uint kNumPosStatesMax = 16u;

		public const int kNumPosStatesBitsEncodingMax = 4;

		public const uint kNumPosStatesEncodingMax = 16u;

		public const int kNumLowLenBits = 3;

		public const int kNumMidLenBits = 3;

		public const int kNumHighLenBits = 8;

		public const uint kNumLowLenSymbols = 8u;

		public const uint kNumMidLenSymbols = 8u;

		public const uint kNumLenSymbols = 272u;

		public const uint kMatchMaxLen = 273u;

		public static uint GetLenToPosState(uint len)
		{
			len -= 2u;
			if (len < 4u)
			{
				return len;
			}
			return 3u;
		}
	}
}

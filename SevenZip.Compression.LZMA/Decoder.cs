using SevenZip.Compression.LZ;
using SevenZip.Compression.RangeCoder;
using System;
using System.IO;

namespace SevenZip.Compression.LZMA
{
	public class Decoder : ICoder, ISetDecoderProperties
	{
		private class LenDecoder
		{
			private BitDecoder m_Choice = default(BitDecoder);

			private BitDecoder m_Choice2 = default(BitDecoder);

			private BitTreeDecoder[] m_LowCoder = new BitTreeDecoder[16];

			private BitTreeDecoder[] m_MidCoder = new BitTreeDecoder[16];

			private BitTreeDecoder m_HighCoder = new BitTreeDecoder(8);

			private uint m_NumPosStates;

			public void Create(uint numPosStates)
			{
				for (uint num = this.m_NumPosStates; num < numPosStates; num += 1u)
				{
					this.m_LowCoder[(int)((UIntPtr)num)] = new BitTreeDecoder(3);
					this.m_MidCoder[(int)((UIntPtr)num)] = new BitTreeDecoder(3);
				}
				this.m_NumPosStates = numPosStates;
			}

			public void Init()
			{
				this.m_Choice.Init();
				for (uint num = 0u; num < this.m_NumPosStates; num += 1u)
				{
					this.m_LowCoder[(int)((UIntPtr)num)].Init();
					this.m_MidCoder[(int)((UIntPtr)num)].Init();
				}
				this.m_Choice2.Init();
				this.m_HighCoder.Init();
			}

			public uint Decode(SevenZip.Compression.RangeCoder.Decoder rangeDecoder, uint posState)
			{
				if (this.m_Choice.Decode(rangeDecoder) == 0u)
				{
					return this.m_LowCoder[(int)((UIntPtr)posState)].Decode(rangeDecoder);
				}
				uint num = 8u;
				if (this.m_Choice2.Decode(rangeDecoder) == 0u)
				{
					num += this.m_MidCoder[(int)((UIntPtr)posState)].Decode(rangeDecoder);
				}
				else
				{
					num += 8u;
					num += this.m_HighCoder.Decode(rangeDecoder);
				}
				return num;
			}
		}

		private class LiteralDecoder
		{
			private struct Decoder2
			{
				private BitDecoder[] m_Decoders;

				public void Create()
				{
					this.m_Decoders = new BitDecoder[768];
				}

				public void Init()
				{
					for (int i = 0; i < 768; i++)
					{
						this.m_Decoders[i].Init();
					}
				}

				public byte DecodeNormal(SevenZip.Compression.RangeCoder.Decoder rangeDecoder)
				{
					uint num = 1u;
					do
					{
						num = (num << 1 | this.m_Decoders[(int)((UIntPtr)num)].Decode(rangeDecoder));
					}
					while (num < 256u);
					return (byte)num;
				}

				public byte DecodeWithMatchByte(SevenZip.Compression.RangeCoder.Decoder rangeDecoder, byte matchByte)
				{
					uint num = 1u;
					while (true)
					{
						uint num2 = (uint)(matchByte >> 7 & 1);
						matchByte = (byte)(matchByte << 1);
						uint num3 = this.m_Decoders[(int)((UIntPtr)((1u + num2 << 8) + num))].Decode(rangeDecoder);
						num = (num << 1 | num3);
						if (num2 != num3)
						{
							break;
						}
						if (num >= 256u)
						{
							goto IL_6D;
						}
					}
					while (num < 256u)
					{
						num = (num << 1 | this.m_Decoders[(int)((UIntPtr)num)].Decode(rangeDecoder));
					}
					IL_6D:
					return (byte)num;
				}
			}

			private Decoder.LiteralDecoder.Decoder2[] m_Coders;

			private int m_NumPrevBits;

			private int m_NumPosBits;

			private uint m_PosMask;

			public void Create(int numPosBits, int numPrevBits)
			{
				if (this.m_Coders != null && this.m_NumPrevBits == numPrevBits && this.m_NumPosBits == numPosBits)
				{
					return;
				}
				this.m_NumPosBits = numPosBits;
				this.m_PosMask = (1u << numPosBits) - 1u;
				this.m_NumPrevBits = numPrevBits;
				uint num = 1u << this.m_NumPrevBits + this.m_NumPosBits;
				this.m_Coders = new Decoder.LiteralDecoder.Decoder2[num];
				for (uint num2 = 0u; num2 < num; num2 += 1u)
				{
					this.m_Coders[(int)((UIntPtr)num2)].Create();
				}
			}

			public void Init()
			{
				uint num = 1u << this.m_NumPrevBits + this.m_NumPosBits;
				for (uint num2 = 0u; num2 < num; num2 += 1u)
				{
					this.m_Coders[(int)((UIntPtr)num2)].Init();
				}
			}

			private uint GetState(uint pos, byte prevByte)
			{
				return ((pos & this.m_PosMask) << this.m_NumPrevBits) + (uint)(prevByte >> 8 - this.m_NumPrevBits);
			}

			public byte DecodeNormal(SevenZip.Compression.RangeCoder.Decoder rangeDecoder, uint pos, byte prevByte)
			{
				return this.m_Coders[(int)((UIntPtr)this.GetState(pos, prevByte))].DecodeNormal(rangeDecoder);
			}

			public byte DecodeWithMatchByte(SevenZip.Compression.RangeCoder.Decoder rangeDecoder, uint pos, byte prevByte, byte matchByte)
			{
				return this.m_Coders[(int)((UIntPtr)this.GetState(pos, prevByte))].DecodeWithMatchByte(rangeDecoder, matchByte);
			}
		}

		private OutWindow m_OutWindow = new OutWindow();

		private SevenZip.Compression.RangeCoder.Decoder m_RangeDecoder = new SevenZip.Compression.RangeCoder.Decoder();

		private BitDecoder[] m_IsMatchDecoders = new BitDecoder[192];

		private BitDecoder[] m_IsRepDecoders = new BitDecoder[12];

		private BitDecoder[] m_IsRepG0Decoders = new BitDecoder[12];

		private BitDecoder[] m_IsRepG1Decoders = new BitDecoder[12];

		private BitDecoder[] m_IsRepG2Decoders = new BitDecoder[12];

		private BitDecoder[] m_IsRep0LongDecoders = new BitDecoder[192];

		private BitTreeDecoder[] m_PosSlotDecoder = new BitTreeDecoder[4];

		private BitDecoder[] m_PosDecoders = new BitDecoder[114];

		private BitTreeDecoder m_PosAlignDecoder = new BitTreeDecoder(4);

		private Decoder.LenDecoder m_LenDecoder = new Decoder.LenDecoder();

		private Decoder.LenDecoder m_RepLenDecoder = new Decoder.LenDecoder();

		private Decoder.LiteralDecoder m_LiteralDecoder = new Decoder.LiteralDecoder();

		private uint m_DictionarySize;

		private uint m_DictionarySizeCheck;

		private uint m_PosStateMask;

		private bool _solid;

		public Decoder()
		{
			this.m_DictionarySize = 4294967295u;
			int num = 0;
			while ((long)num < 4L)
			{
				this.m_PosSlotDecoder[num] = new BitTreeDecoder(6);
				num++;
			}
		}

		private void SetDictionarySize(uint dictionarySize)
		{
			if (this.m_DictionarySize != dictionarySize)
			{
				this.m_DictionarySize = dictionarySize;
				this.m_DictionarySizeCheck = Math.Max(this.m_DictionarySize, 1u);
				uint windowSize = Math.Max(this.m_DictionarySizeCheck, 4096u);
				this.m_OutWindow.Create(windowSize);
			}
		}

		private void SetLiteralProperties(int lp, int lc)
		{
			if (lp > 8)
			{
				throw new InvalidParamException();
			}
			if (lc > 8)
			{
				throw new InvalidParamException();
			}
			this.m_LiteralDecoder.Create(lp, lc);
		}

		private void SetPosBitsProperties(int pb)
		{
			if (pb > 4)
			{
				throw new InvalidParamException();
			}
			uint num = 1u << pb;
			this.m_LenDecoder.Create(num);
			this.m_RepLenDecoder.Create(num);
			this.m_PosStateMask = num - 1u;
		}

		private void Init(Stream inStream, Stream outStream)
		{
			this.m_RangeDecoder.Init(inStream);
			this.m_OutWindow.Init(outStream, this._solid);
			for (uint num = 0u; num < 12u; num += 1u)
			{
				for (uint num2 = 0u; num2 <= this.m_PosStateMask; num2 += 1u)
				{
					uint num3 = (num << 4) + num2;
					this.m_IsMatchDecoders[(int)((UIntPtr)num3)].Init();
					this.m_IsRep0LongDecoders[(int)((UIntPtr)num3)].Init();
				}
				this.m_IsRepDecoders[(int)((UIntPtr)num)].Init();
				this.m_IsRepG0Decoders[(int)((UIntPtr)num)].Init();
				this.m_IsRepG1Decoders[(int)((UIntPtr)num)].Init();
				this.m_IsRepG2Decoders[(int)((UIntPtr)num)].Init();
			}
			this.m_LiteralDecoder.Init();
			for (uint num = 0u; num < 4u; num += 1u)
			{
				this.m_PosSlotDecoder[(int)((UIntPtr)num)].Init();
			}
			for (uint num = 0u; num < 114u; num += 1u)
			{
				this.m_PosDecoders[(int)((UIntPtr)num)].Init();
			}
			this.m_LenDecoder.Init();
			this.m_RepLenDecoder.Init();
			this.m_PosAlignDecoder.Init();
		}

		public void Code(Stream inStream, Stream outStream, long inSize, long outSize, ICodeProgress progress)
		{
			this.Init(inStream, outStream);
			Base.State state = default(Base.State);
			state.Init();
			uint num = 0u;
			uint num2 = 0u;
			uint num3 = 0u;
			uint num4 = 0u;
			ulong num5 = 0uL;
			if (num5 < (ulong)outSize)
			{
				if (this.m_IsMatchDecoders[(int)((UIntPtr)(state.Index << 4))].Decode(this.m_RangeDecoder) != 0u)
				{
					throw new DataErrorException();
				}
				state.UpdateChar();
				byte b = this.m_LiteralDecoder.DecodeNormal(this.m_RangeDecoder, 0u, 0);
				this.m_OutWindow.PutByte(b);
				num5 += 1uL;
			}
			while (num5 < (ulong)outSize)
			{
				uint num6 = (uint)num5 & this.m_PosStateMask;
				if (this.m_IsMatchDecoders[(int)((UIntPtr)((state.Index << 4) + num6))].Decode(this.m_RangeDecoder) == 0u)
				{
					byte @byte = this.m_OutWindow.GetByte(0u);
					byte b2;
					if (!state.IsCharState())
					{
						b2 = this.m_LiteralDecoder.DecodeWithMatchByte(this.m_RangeDecoder, (uint)num5, @byte, this.m_OutWindow.GetByte(num));
					}
					else
					{
						b2 = this.m_LiteralDecoder.DecodeNormal(this.m_RangeDecoder, (uint)num5, @byte);
					}
					this.m_OutWindow.PutByte(b2);
					state.UpdateChar();
					num5 += 1uL;
				}
				else
				{
					uint num8;
					if (this.m_IsRepDecoders[(int)((UIntPtr)state.Index)].Decode(this.m_RangeDecoder) == 1u)
					{
						if (this.m_IsRepG0Decoders[(int)((UIntPtr)state.Index)].Decode(this.m_RangeDecoder) == 0u)
						{
							if (this.m_IsRep0LongDecoders[(int)((UIntPtr)((state.Index << 4) + num6))].Decode(this.m_RangeDecoder) == 0u)
							{
								state.UpdateShortRep();
								this.m_OutWindow.PutByte(this.m_OutWindow.GetByte(num));
								num5 += 1uL;
								continue;
							}
						}
						else
						{
							uint num7;
							if (this.m_IsRepG1Decoders[(int)((UIntPtr)state.Index)].Decode(this.m_RangeDecoder) == 0u)
							{
								num7 = num2;
							}
							else
							{
								if (this.m_IsRepG2Decoders[(int)((UIntPtr)state.Index)].Decode(this.m_RangeDecoder) == 0u)
								{
									num7 = num3;
								}
								else
								{
									num7 = num4;
									num4 = num3;
								}
								num3 = num2;
							}
							num2 = num;
							num = num7;
						}
						num8 = this.m_RepLenDecoder.Decode(this.m_RangeDecoder, num6) + 2u;
						state.UpdateRep();
					}
					else
					{
						num4 = num3;
						num3 = num2;
						num2 = num;
						num8 = 2u + this.m_LenDecoder.Decode(this.m_RangeDecoder, num6);
						state.UpdateMatch();
						uint num9 = this.m_PosSlotDecoder[(int)((UIntPtr)Base.GetLenToPosState(num8))].Decode(this.m_RangeDecoder);
						if (num9 >= 4u)
						{
							int num10 = (int)((num9 >> 1) - 1u);
							num = (2u | (num9 & 1u)) << num10;
							if (num9 < 14u)
							{
								num += BitTreeDecoder.ReverseDecode(this.m_PosDecoders, num - num9 - 1u, this.m_RangeDecoder, num10);
							}
							else
							{
								num += this.m_RangeDecoder.DecodeDirectBits(num10 - 4) << 4;
								num += this.m_PosAlignDecoder.ReverseDecode(this.m_RangeDecoder);
							}
						}
						else
						{
							num = num9;
						}
					}
					if ((ulong)num >= (ulong)this.m_OutWindow.TrainSize + num5 || num >= this.m_DictionarySizeCheck)
					{
						if (num == 4294967295u)
						{
							break;
						}
						throw new DataErrorException();
					}
					else
					{
						this.m_OutWindow.CopyBlock(num, num8);
						num5 += (ulong)num8;
					}
				}
			}
			this.m_OutWindow.Flush();
			this.m_OutWindow.ReleaseStream();
			this.m_RangeDecoder.ReleaseStream();
		}

		public void SetDecoderProperties(byte[] properties)
		{
			if (properties.Length < 5)
			{
				throw new InvalidParamException();
			}
			int lc = (int)(properties[0] % 9);
			int num = (int)(properties[0] / 9);
			int lp = num % 5;
			int num2 = num / 5;
			if (num2 > 4)
			{
				throw new InvalidParamException();
			}
			uint num3 = 0u;
			for (int i = 0; i < 4; i++)
			{
				num3 += (uint)((uint)properties[1 + i] << i * 8);
			}
			this.SetDictionarySize(num3);
			this.SetLiteralProperties(lp, lc);
			this.SetPosBitsProperties(num2);
		}

		public bool Train(Stream stream)
		{
			this._solid = true;
			return this.m_OutWindow.Train(stream);
		}
	}
}

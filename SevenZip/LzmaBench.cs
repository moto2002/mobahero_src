using SevenZip.Compression.LZMA;
using System;
using System.IO;

namespace SevenZip
{
	internal abstract class LzmaBench
	{
		private class CRandomGenerator
		{
			private uint A1;

			private uint A2;

			public CRandomGenerator()
			{
				this.Init();
			}

			public void Init()
			{
				this.A1 = 362436069u;
				this.A2 = 521288629u;
			}

			public uint GetRnd()
			{
				return (this.A1 = 36969u * (this.A1 & 65535u) + (this.A1 >> 16)) << 16 ^ (this.A2 = 18000u * (this.A2 & 65535u) + (this.A2 >> 16));
			}
		}

		private class CBitRandomGenerator
		{
			private LzmaBench.CRandomGenerator RG = new LzmaBench.CRandomGenerator();

			private uint Value;

			private int NumBits;

			public void Init()
			{
				this.Value = 0u;
				this.NumBits = 0;
			}

			public uint GetRnd(int numBits)
			{
				uint num;
				if (this.NumBits > numBits)
				{
					num = (this.Value & (1u << numBits) - 1u);
					this.Value >>= numBits;
					this.NumBits -= numBits;
					return num;
				}
				numBits -= this.NumBits;
				num = this.Value << numBits;
				this.Value = this.RG.GetRnd();
				num |= (this.Value & (1u << numBits) - 1u);
				this.Value >>= numBits;
				this.NumBits = 32 - numBits;
				return num;
			}
		}

		private class CBenchRandomGenerator
		{
			private LzmaBench.CBitRandomGenerator RG = new LzmaBench.CBitRandomGenerator();

			private uint Pos;

			private uint Rep0;

			public uint BufferSize;

			public byte[] Buffer;

			public void Set(uint bufferSize)
			{
				this.Buffer = new byte[bufferSize];
				this.Pos = 0u;
				this.BufferSize = bufferSize;
			}

			private uint GetRndBit()
			{
				return this.RG.GetRnd(1);
			}

			private uint GetLogRandBits(int numBits)
			{
				uint rnd = this.RG.GetRnd(numBits);
				return this.RG.GetRnd((int)rnd);
			}

			private uint GetOffset()
			{
				if (this.GetRndBit() == 0u)
				{
					return this.GetLogRandBits(4);
				}
				return this.GetLogRandBits(4) << 10 | this.RG.GetRnd(10);
			}

			private uint GetLen1()
			{
				return this.RG.GetRnd((int)(1u + this.RG.GetRnd(2)));
			}

			private uint GetLen2()
			{
				return this.RG.GetRnd((int)(2u + this.RG.GetRnd(2)));
			}

			public void Generate()
			{
				this.RG.Init();
				this.Rep0 = 1u;
				while (this.Pos < this.BufferSize)
				{
					if (this.GetRndBit() == 0u || this.Pos < 1u)
					{
						this.Buffer[(int)((UIntPtr)(this.Pos++))] = (byte)this.RG.GetRnd(8);
					}
					else
					{
						uint num;
						if (this.RG.GetRnd(3) == 0u)
						{
							num = 1u + this.GetLen1();
						}
						else
						{
							do
							{
								this.Rep0 = this.GetOffset();
							}
							while (this.Rep0 >= this.Pos);
							this.Rep0 += 1u;
							num = 2u + this.GetLen2();
						}
						uint num2 = 0u;
						while (num2 < num && this.Pos < this.BufferSize)
						{
							this.Buffer[(int)((UIntPtr)this.Pos)] = this.Buffer[(int)((UIntPtr)(this.Pos - this.Rep0))];
							num2 += 1u;
							this.Pos += 1u;
						}
					}
				}
			}
		}

		private class CrcOutStream : Stream
		{
			public CRC CRC = new CRC();

			public override bool CanRead
			{
				get
				{
					return false;
				}
			}

			public override bool CanSeek
			{
				get
				{
					return false;
				}
			}

			public override bool CanWrite
			{
				get
				{
					return true;
				}
			}

			public override long Length
			{
				get
				{
					return 0L;
				}
			}

			public override long Position
			{
				get
				{
					return 0L;
				}
				set
				{
				}
			}

			public void Init()
			{
				this.CRC.Init();
			}

			public uint GetDigest()
			{
				return this.CRC.GetDigest();
			}

			public override void Flush()
			{
			}

			public override long Seek(long offset, SeekOrigin origin)
			{
				return 0L;
			}

			public override void SetLength(long value)
			{
			}

			public override int Read(byte[] buffer, int offset, int count)
			{
				return 0;
			}

			public override void WriteByte(byte b)
			{
				this.CRC.UpdateByte(b);
			}

			public override void Write(byte[] buffer, int offset, int count)
			{
				this.CRC.Update(buffer, (uint)offset, (uint)count);
			}
		}

		private class CProgressInfo : ICodeProgress
		{
			public long ApprovedStart;

			public long InSize;

			public DateTime Time;

			public void Init()
			{
				this.InSize = 0L;
			}

			public void SetProgress(long inSize, long outSize)
			{
				if (inSize >= this.ApprovedStart && this.InSize == 0L)
				{
					this.Time = DateTime.UtcNow;
					this.InSize = inSize;
				}
			}
		}

		private const uint kAdditionalSize = 6291456u;

		private const uint kCompressedAdditionalSize = 1024u;

		private const uint kMaxLzmaPropSize = 10u;

		private const int kSubBits = 8;

		private static uint GetLogSize(uint size)
		{
			for (int i = 8; i < 32; i++)
			{
				for (uint num = 0u; num < 256u; num += 1u)
				{
					if (size <= (1u << i) + (num << i - 8))
					{
						return (uint)((i << 8) + (int)num);
					}
				}
			}
			return 8192u;
		}

		private static ulong MyMultDiv64(ulong value, ulong elapsedTime)
		{
			ulong num = 10000000uL;
			ulong num2 = elapsedTime;
			while (num > 1000000uL)
			{
				num >>= 1;
				num2 >>= 1;
			}
			if (num2 == 0uL)
			{
				num2 = 1uL;
			}
			return value * num / num2;
		}

		private static ulong GetCompressRating(uint dictionarySize, ulong elapsedTime, ulong size)
		{
			ulong num = (ulong)(LzmaBench.GetLogSize(dictionarySize) - 4608u);
			ulong num2 = 1060uL + (num * num * 10uL >> 16);
			ulong value = size * num2;
			return LzmaBench.MyMultDiv64(value, elapsedTime);
		}

		private static ulong GetDecompressRating(ulong elapsedTime, ulong outSize, ulong inSize)
		{
			ulong value = inSize * 220uL + outSize * 20uL;
			return LzmaBench.MyMultDiv64(value, elapsedTime);
		}

		private static ulong GetTotalRating(uint dictionarySize, ulong elapsedTimeEn, ulong sizeEn, ulong elapsedTimeDe, ulong inSizeDe, ulong outSizeDe)
		{
			return (LzmaBench.GetCompressRating(dictionarySize, elapsedTimeEn, sizeEn) + LzmaBench.GetDecompressRating(elapsedTimeDe, inSizeDe, outSizeDe)) / 2uL;
		}

		private static void PrintValue(ulong v)
		{
			string text = v.ToString();
			int num = 0;
			while (num + text.Length < 6)
			{
				Console.Write(" ");
				num++;
			}
			Console.Write(text);
		}

		private static void PrintRating(ulong rating)
		{
			LzmaBench.PrintValue(rating / 1000000uL);
			Console.Write(" MIPS");
		}

		private static void PrintResults(uint dictionarySize, ulong elapsedTime, ulong size, bool decompressMode, ulong secondSize)
		{
			ulong num = LzmaBench.MyMultDiv64(size, elapsedTime);
			LzmaBench.PrintValue(num / 1024uL);
			Console.Write(" KB/s  ");
			ulong rating;
			if (decompressMode)
			{
				rating = LzmaBench.GetDecompressRating(elapsedTime, size, secondSize);
			}
			else
			{
				rating = LzmaBench.GetCompressRating(dictionarySize, elapsedTime, size);
			}
			LzmaBench.PrintRating(rating);
		}

		public static int LzmaBenchmark(int numIterations, uint dictionarySize)
		{
			if (numIterations <= 0)
			{
				return 0;
			}
			if (dictionarySize < 262144u)
			{
				Console.WriteLine("\nError: dictionary size for benchmark must be >= 19 (512 KB)");
				return 1;
			}
			Console.Write("\n       Compressing                Decompressing\n\n");
			Encoder encoder = new Encoder();
			Decoder decoder = new Decoder();
			CoderPropID[] propIDs = new CoderPropID[]
			{
				CoderPropID.DictionarySize
			};
			object[] properties = new object[]
			{
				(int)dictionarySize
			};
			uint num = dictionarySize + 6291456u;
			uint capacity = num / 2u + 1024u;
			encoder.SetCoderProperties(propIDs, properties);
			MemoryStream memoryStream = new MemoryStream();
			encoder.WriteCoderProperties(memoryStream);
			byte[] decoderProperties = memoryStream.ToArray();
			LzmaBench.CBenchRandomGenerator cBenchRandomGenerator = new LzmaBench.CBenchRandomGenerator();
			cBenchRandomGenerator.Set(num);
			cBenchRandomGenerator.Generate();
			CRC cRC = new CRC();
			cRC.Init();
			cRC.Update(cBenchRandomGenerator.Buffer, 0u, cBenchRandomGenerator.BufferSize);
			LzmaBench.CProgressInfo cProgressInfo = new LzmaBench.CProgressInfo();
			cProgressInfo.ApprovedStart = (long)((ulong)dictionarySize);
			ulong num2 = 0uL;
			ulong num3 = 0uL;
			ulong num4 = 0uL;
			ulong num5 = 0uL;
			MemoryStream memoryStream2 = new MemoryStream(cBenchRandomGenerator.Buffer, 0, (int)cBenchRandomGenerator.BufferSize);
			MemoryStream memoryStream3 = new MemoryStream((int)capacity);
			LzmaBench.CrcOutStream crcOutStream = new LzmaBench.CrcOutStream();
			for (int i = 0; i < numIterations; i++)
			{
				cProgressInfo.Init();
				memoryStream2.Seek(0L, SeekOrigin.Begin);
				memoryStream3.Seek(0L, SeekOrigin.Begin);
				encoder.Code(memoryStream2, memoryStream3, -1L, -1L, cProgressInfo);
				ulong ticks = (ulong)(DateTime.UtcNow - cProgressInfo.Time).Ticks;
				long position = memoryStream3.Position;
				if (cProgressInfo.InSize == 0L)
				{
					throw new Exception("Internal ERROR 1282");
				}
				ulong num6 = 0uL;
				for (int j = 0; j < 2; j++)
				{
					memoryStream3.Seek(0L, SeekOrigin.Begin);
					crcOutStream.Init();
					decoder.SetDecoderProperties(decoderProperties);
					ulong outSize = (ulong)num;
					DateTime utcNow = DateTime.UtcNow;
					decoder.Code(memoryStream3, crcOutStream, 0L, (long)outSize, null);
					num6 = (ulong)(DateTime.UtcNow - utcNow).Ticks;
					if (crcOutStream.GetDigest() != cRC.GetDigest())
					{
						throw new Exception("CRC Error");
					}
				}
				ulong num7 = (ulong)num - (ulong)cProgressInfo.InSize;
				LzmaBench.PrintResults(dictionarySize, ticks, num7, false, 0uL);
				Console.Write("     ");
				LzmaBench.PrintResults(dictionarySize, num6, (ulong)num, true, (ulong)position);
				Console.WriteLine();
				num2 += num7;
				num3 += ticks;
				num4 += num6;
				num5 += (ulong)position;
			}
			Console.WriteLine("---------------------------------------------------");
			LzmaBench.PrintResults(dictionarySize, num3, num2, false, 0uL);
			Console.Write("     ");
			LzmaBench.PrintResults(dictionarySize, num4, (ulong)num * (ulong)((long)numIterations), true, num5);
			Console.WriteLine("    Average");
			return 0;
		}
	}
}

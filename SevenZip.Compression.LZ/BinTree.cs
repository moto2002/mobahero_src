using System;
using System.IO;

namespace SevenZip.Compression.LZ
{
	public class BinTree : InWindow, IInWindowStream, IMatchFinder
	{
		private const uint kHash2Size = 1024u;

		private const uint kHash3Size = 65536u;

		private const uint kBT2HashSize = 65536u;

		private const uint kStartMaxLen = 1u;

		private const uint kHash3Offset = 1024u;

		private const uint kEmptyHashValue = 0u;

		private const uint kMaxValForNormalize = 2147483647u;

		private uint _cyclicBufferPos;

		private uint _cyclicBufferSize;

		private uint _matchMaxLen;

		private uint[] _son;

		private uint[] _hash;

		private uint _cutValue = 255u;

		private uint _hashMask;

		private uint _hashSizeSum;

		private bool HASH_ARRAY = true;

		private uint kNumHashDirectBytes;

		private uint kMinMatchCheck = 4u;

		private uint kFixHashSize = 66560u;

		public void SetType(int numHashBytes)
		{
			this.HASH_ARRAY = (numHashBytes > 2);
			if (this.HASH_ARRAY)
			{
				this.kNumHashDirectBytes = 0u;
				this.kMinMatchCheck = 4u;
				this.kFixHashSize = 66560u;
			}
			else
			{
				this.kNumHashDirectBytes = 2u;
				this.kMinMatchCheck = 3u;
				this.kFixHashSize = 0u;
			}
		}

		public new void SetStream(Stream stream)
		{
			base.SetStream(stream);
		}

		public new void ReleaseStream()
		{
			base.ReleaseStream();
		}

		public new void Init()
		{
			base.Init();
			for (uint num = 0u; num < this._hashSizeSum; num += 1u)
			{
				this._hash[(int)((UIntPtr)num)] = 0u;
			}
			this._cyclicBufferPos = 0u;
			base.ReduceOffsets(-1);
		}

		public new void MovePos()
		{
			if ((this._cyclicBufferPos += 1u) >= this._cyclicBufferSize)
			{
				this._cyclicBufferPos = 0u;
			}
			base.MovePos();
			if (this._pos == 2147483647u)
			{
				this.Normalize();
			}
		}

		public new byte GetIndexByte(int index)
		{
			return base.GetIndexByte(index);
		}

		public new uint GetMatchLen(int index, uint distance, uint limit)
		{
			return base.GetMatchLen(index, distance, limit);
		}

		public new uint GetNumAvailableBytes()
		{
			return base.GetNumAvailableBytes();
		}

		public void Create(uint historySize, uint keepAddBufferBefore, uint matchMaxLen, uint keepAddBufferAfter)
		{
			if (historySize > 2147483391u)
			{
				throw new Exception();
			}
			this._cutValue = 16u + (matchMaxLen >> 1);
			uint keepSizeReserv = (historySize + keepAddBufferBefore + matchMaxLen + keepAddBufferAfter) / 2u + 256u;
			base.Create(historySize + keepAddBufferBefore, matchMaxLen + keepAddBufferAfter, keepSizeReserv);
			this._matchMaxLen = matchMaxLen;
			uint num = historySize + 1u;
			if (this._cyclicBufferSize != num)
			{
				this._son = new uint[(this._cyclicBufferSize = num) * 2u];
			}
			uint num2 = 65536u;
			if (this.HASH_ARRAY)
			{
				num2 = historySize - 1u;
				num2 |= num2 >> 1;
				num2 |= num2 >> 2;
				num2 |= num2 >> 4;
				num2 |= num2 >> 8;
				num2 >>= 1;
				num2 |= 65535u;
				if (num2 > 16777216u)
				{
					num2 >>= 1;
				}
				this._hashMask = num2;
				num2 += 1u;
				num2 += this.kFixHashSize;
			}
			if (num2 != this._hashSizeSum)
			{
				this._hash = new uint[this._hashSizeSum = num2];
			}
		}

		public uint GetMatches(uint[] distances)
		{
			uint num;
			if (this._pos + this._matchMaxLen <= this._streamPos)
			{
				num = this._matchMaxLen;
			}
			else
			{
				num = this._streamPos - this._pos;
				if (num < this.kMinMatchCheck)
				{
					this.MovePos();
					return 0u;
				}
			}
			uint num2 = 0u;
			uint num3 = (this._pos <= this._cyclicBufferSize) ? 0u : (this._pos - this._cyclicBufferSize);
			uint num4 = this._bufferOffset + this._pos;
			uint num5 = 1u;
			uint num6 = 0u;
			uint num7 = 0u;
			uint num9;
			if (this.HASH_ARRAY)
			{
				uint num8 = CRC.Table[(int)this._bufferBase[(int)((UIntPtr)num4)]] ^ (uint)this._bufferBase[(int)((UIntPtr)(num4 + 1u))];
				num6 = (num8 & 1023u);
				num8 ^= (uint)((uint)this._bufferBase[(int)((UIntPtr)(num4 + 2u))] << 8);
				num7 = (num8 & 65535u);
				num9 = ((num8 ^ CRC.Table[(int)this._bufferBase[(int)((UIntPtr)(num4 + 3u))]] << 5) & this._hashMask);
			}
			else
			{
				num9 = (uint)((int)this._bufferBase[(int)((UIntPtr)num4)] ^ (int)this._bufferBase[(int)((UIntPtr)(num4 + 1u))] << 8);
			}
			uint num10 = this._hash[(int)((UIntPtr)(this.kFixHashSize + num9))];
			if (this.HASH_ARRAY)
			{
				uint num11 = this._hash[(int)((UIntPtr)num6)];
				uint num12 = this._hash[(int)((UIntPtr)(1024u + num7))];
				this._hash[(int)((UIntPtr)num6)] = this._pos;
				this._hash[(int)((UIntPtr)(1024u + num7))] = this._pos;
				if (num11 > num3 && this._bufferBase[(int)((UIntPtr)(this._bufferOffset + num11))] == this._bufferBase[(int)((UIntPtr)num4)])
				{
					num5 = (distances[(int)((UIntPtr)(num2++))] = 2u);
					distances[(int)((UIntPtr)(num2++))] = this._pos - num11 - 1u;
				}
				if (num12 > num3 && this._bufferBase[(int)((UIntPtr)(this._bufferOffset + num12))] == this._bufferBase[(int)((UIntPtr)num4)])
				{
					if (num12 == num11)
					{
						num2 -= 2u;
					}
					num5 = (distances[(int)((UIntPtr)(num2++))] = 3u);
					distances[(int)((UIntPtr)(num2++))] = this._pos - num12 - 1u;
					num11 = num12;
				}
				if (num2 != 0u && num11 == num10)
				{
					num2 -= 2u;
					num5 = 1u;
				}
			}
			this._hash[(int)((UIntPtr)(this.kFixHashSize + num9))] = this._pos;
			uint num13 = (this._cyclicBufferPos << 1) + 1u;
			uint num14 = this._cyclicBufferPos << 1;
			uint val2;
			uint val = val2 = this.kNumHashDirectBytes;
			if (this.kNumHashDirectBytes != 0u && num10 > num3 && this._bufferBase[(int)((UIntPtr)(this._bufferOffset + num10 + this.kNumHashDirectBytes))] != this._bufferBase[(int)((UIntPtr)(num4 + this.kNumHashDirectBytes))])
			{
				num5 = (distances[(int)((UIntPtr)(num2++))] = this.kNumHashDirectBytes);
				distances[(int)((UIntPtr)(num2++))] = this._pos - num10 - 1u;
			}
			uint cutValue = this._cutValue;
			while (num10 > num3 && cutValue-- != 0u)
			{
				uint num15 = this._pos - num10;
				uint num16 = ((num15 > this._cyclicBufferPos) ? (this._cyclicBufferPos - num15 + this._cyclicBufferSize) : (this._cyclicBufferPos - num15)) << 1;
				uint num17 = this._bufferOffset + num10;
				uint num18 = Math.Min(val2, val);
				if (this._bufferBase[(int)((UIntPtr)(num17 + num18))] == this._bufferBase[(int)((UIntPtr)(num4 + num18))])
				{
					while ((num18 += 1u) != num)
					{
						if (this._bufferBase[(int)((UIntPtr)(num17 + num18))] != this._bufferBase[(int)((UIntPtr)(num4 + num18))])
						{
							break;
						}
					}
					if (num5 < num18)
					{
						num5 = (distances[(int)((UIntPtr)(num2++))] = num18);
						distances[(int)((UIntPtr)(num2++))] = num15 - 1u;
						if (num18 == num)
						{
							this._son[(int)((UIntPtr)num14)] = this._son[(int)((UIntPtr)num16)];
							this._son[(int)((UIntPtr)num13)] = this._son[(int)((UIntPtr)(num16 + 1u))];
							IL_461:
							this.MovePos();
							return num2;
						}
					}
				}
				if (this._bufferBase[(int)((UIntPtr)(num17 + num18))] < this._bufferBase[(int)((UIntPtr)(num4 + num18))])
				{
					this._son[(int)((UIntPtr)num14)] = num10;
					num14 = num16 + 1u;
					num10 = this._son[(int)((UIntPtr)num14)];
					val = num18;
				}
				else
				{
					this._son[(int)((UIntPtr)num13)] = num10;
					num13 = num16;
					num10 = this._son[(int)((UIntPtr)num13)];
					val2 = num18;
				}
			}
			this._son[(int)((UIntPtr)num13)] = (this._son[(int)((UIntPtr)num14)] = 0u);
			goto IL_461;
		}

		public void Skip(uint num)
		{
			while (true)
			{
				uint num2;
				if (this._pos + this._matchMaxLen <= this._streamPos)
				{
					num2 = this._matchMaxLen;
					goto IL_49;
				}
				num2 = this._streamPos - this._pos;
				if (num2 >= this.kMinMatchCheck)
				{
					goto IL_49;
				}
				this.MovePos();
				IL_2F9:
				if ((num -= 1u) == 0u)
				{
					break;
				}
				continue;
				IL_49:
				uint num3 = (this._pos <= this._cyclicBufferSize) ? 0u : (this._pos - this._cyclicBufferSize);
				uint num4 = this._bufferOffset + this._pos;
				uint num8;
				if (this.HASH_ARRAY)
				{
					uint num5 = CRC.Table[(int)this._bufferBase[(int)((UIntPtr)num4)]] ^ (uint)this._bufferBase[(int)((UIntPtr)(num4 + 1u))];
					uint num6 = num5 & 1023u;
					this._hash[(int)((UIntPtr)num6)] = this._pos;
					num5 ^= (uint)((uint)this._bufferBase[(int)((UIntPtr)(num4 + 2u))] << 8);
					uint num7 = num5 & 65535u;
					this._hash[(int)((UIntPtr)(1024u + num7))] = this._pos;
					num8 = ((num5 ^ CRC.Table[(int)this._bufferBase[(int)((UIntPtr)(num4 + 3u))]] << 5) & this._hashMask);
				}
				else
				{
					num8 = (uint)((int)this._bufferBase[(int)((UIntPtr)num4)] ^ (int)this._bufferBase[(int)((UIntPtr)(num4 + 1u))] << 8);
				}
				uint num9 = this._hash[(int)((UIntPtr)(this.kFixHashSize + num8))];
				this._hash[(int)((UIntPtr)(this.kFixHashSize + num8))] = this._pos;
				uint num10 = (this._cyclicBufferPos << 1) + 1u;
				uint num11 = this._cyclicBufferPos << 1;
				uint val2;
				uint val = val2 = this.kNumHashDirectBytes;
				uint cutValue = this._cutValue;
				while (num9 > num3 && cutValue-- != 0u)
				{
					uint num12 = this._pos - num9;
					uint num13 = ((num12 > this._cyclicBufferPos) ? (this._cyclicBufferPos - num12 + this._cyclicBufferSize) : (this._cyclicBufferPos - num12)) << 1;
					uint num14 = this._bufferOffset + num9;
					uint num15 = Math.Min(val2, val);
					if (this._bufferBase[(int)((UIntPtr)(num14 + num15))] == this._bufferBase[(int)((UIntPtr)(num4 + num15))])
					{
						while ((num15 += 1u) != num2)
						{
							if (this._bufferBase[(int)((UIntPtr)(num14 + num15))] != this._bufferBase[(int)((UIntPtr)(num4 + num15))])
							{
								break;
							}
						}
						if (num15 == num2)
						{
							this._son[(int)((UIntPtr)num11)] = this._son[(int)((UIntPtr)num13)];
							this._son[(int)((UIntPtr)num10)] = this._son[(int)((UIntPtr)(num13 + 1u))];
							IL_2F3:
							this.MovePos();
							goto IL_2F9;
						}
					}
					if (this._bufferBase[(int)((UIntPtr)(num14 + num15))] < this._bufferBase[(int)((UIntPtr)(num4 + num15))])
					{
						this._son[(int)((UIntPtr)num11)] = num9;
						num11 = num13 + 1u;
						num9 = this._son[(int)((UIntPtr)num11)];
						val = num15;
					}
					else
					{
						this._son[(int)((UIntPtr)num10)] = num9;
						num10 = num13;
						num9 = this._son[(int)((UIntPtr)num10)];
						val2 = num15;
					}
				}
				this._son[(int)((UIntPtr)num10)] = (this._son[(int)((UIntPtr)num11)] = 0u);
				goto IL_2F3;
			}
		}

		private void NormalizeLinks(uint[] items, uint numItems, uint subValue)
		{
			for (uint num = 0u; num < numItems; num += 1u)
			{
				uint num2 = items[(int)((UIntPtr)num)];
				if (num2 <= subValue)
				{
					num2 = 0u;
				}
				else
				{
					num2 -= subValue;
				}
				items[(int)((UIntPtr)num)] = num2;
			}
		}

		private void Normalize()
		{
			uint subValue = this._pos - this._cyclicBufferSize;
			this.NormalizeLinks(this._son, this._cyclicBufferSize * 2u, subValue);
			this.NormalizeLinks(this._hash, this._hashSizeSum, subValue);
			base.ReduceOffsets((int)subValue);
		}

		public void SetCutValue(uint cutValue)
		{
			this._cutValue = cutValue;
		}
	}
}

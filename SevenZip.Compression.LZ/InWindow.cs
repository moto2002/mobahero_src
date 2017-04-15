using System;
using System.IO;

namespace SevenZip.Compression.LZ
{
	public class InWindow
	{
		public byte[] _bufferBase;

		private Stream _stream;

		private uint _posLimit;

		private bool _streamEndWasReached;

		private uint _pointerToLastSafePosition;

		public uint _bufferOffset;

		public uint _blockSize;

		public uint _pos;

		private uint _keepSizeBefore;

		private uint _keepSizeAfter;

		public uint _streamPos;

		public void MoveBlock()
		{
			uint num = this._bufferOffset + this._pos - this._keepSizeBefore;
			if (num > 0u)
			{
				num -= 1u;
			}
			uint num2 = this._bufferOffset + this._streamPos - num;
			for (uint num3 = 0u; num3 < num2; num3 += 1u)
			{
				this._bufferBase[(int)((UIntPtr)num3)] = this._bufferBase[(int)((UIntPtr)(num + num3))];
			}
			this._bufferOffset -= num;
		}

		public virtual void ReadBlock()
		{
			if (this._streamEndWasReached)
			{
				return;
			}
			while (true)
			{
				int num = (int)(0u - this._bufferOffset + this._blockSize - this._streamPos);
				if (num == 0)
				{
					break;
				}
				int num2 = this._stream.Read(this._bufferBase, (int)(this._bufferOffset + this._streamPos), num);
				if (num2 == 0)
				{
					goto Block_3;
				}
				this._streamPos += (uint)num2;
				if (this._streamPos >= this._pos + this._keepSizeAfter)
				{
					this._posLimit = this._streamPos - this._keepSizeAfter;
				}
			}
			return;
			Block_3:
			this._posLimit = this._streamPos;
			uint num3 = this._bufferOffset + this._posLimit;
			if (num3 > this._pointerToLastSafePosition)
			{
				this._posLimit = this._pointerToLastSafePosition - this._bufferOffset;
			}
			this._streamEndWasReached = true;
		}

		private void Free()
		{
			this._bufferBase = null;
		}

		public void Create(uint keepSizeBefore, uint keepSizeAfter, uint keepSizeReserv)
		{
			this._keepSizeBefore = keepSizeBefore;
			this._keepSizeAfter = keepSizeAfter;
			uint num = keepSizeBefore + keepSizeAfter + keepSizeReserv;
			if (this._bufferBase == null || this._blockSize != num)
			{
				this.Free();
				this._blockSize = num;
				this._bufferBase = new byte[this._blockSize];
			}
			this._pointerToLastSafePosition = this._blockSize - keepSizeAfter;
		}

		public void SetStream(Stream stream)
		{
			this._stream = stream;
		}

		public void ReleaseStream()
		{
			this._stream = null;
		}

		public void Init()
		{
			this._bufferOffset = 0u;
			this._pos = 0u;
			this._streamPos = 0u;
			this._streamEndWasReached = false;
			this.ReadBlock();
		}

		public void MovePos()
		{
			this._pos += 1u;
			if (this._pos > this._posLimit)
			{
				uint num = this._bufferOffset + this._pos;
				if (num > this._pointerToLastSafePosition)
				{
					this.MoveBlock();
				}
				this.ReadBlock();
			}
		}

		public byte GetIndexByte(int index)
		{
			return this._bufferBase[(int)(checked((IntPtr)(unchecked((ulong)(this._bufferOffset + this._pos) + (ulong)((long)index)))))];
		}

		public uint GetMatchLen(int index, uint distance, uint limit)
		{
			if (this._streamEndWasReached && (ulong)this._pos + (ulong)((long)index) + (ulong)limit > (ulong)this._streamPos)
			{
				limit = this._streamPos - (uint)((ulong)this._pos + (ulong)((long)index));
			}
			distance += 1u;
			uint num = this._bufferOffset + this._pos + (uint)index;
			uint num2 = 0u;
			while (num2 < limit && this._bufferBase[(int)((UIntPtr)(num + num2))] == this._bufferBase[(int)((UIntPtr)(num + num2 - distance))])
			{
				num2 += 1u;
			}
			return num2;
		}

		public uint GetNumAvailableBytes()
		{
			return this._streamPos - this._pos;
		}

		public void ReduceOffsets(int subValue)
		{
			this._bufferOffset += (uint)subValue;
			this._posLimit -= (uint)subValue;
			this._pos -= (uint)subValue;
			this._streamPos -= (uint)subValue;
		}
	}
}

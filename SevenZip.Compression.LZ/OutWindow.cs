using System;
using System.IO;

namespace SevenZip.Compression.LZ
{
	public class OutWindow
	{
		private byte[] _buffer;

		private uint _pos;

		private uint _windowSize;

		private uint _streamPos;

		private Stream _stream;

		public uint TrainSize;

		public void Create(uint windowSize)
		{
			if (this._windowSize != windowSize)
			{
				this._buffer = new byte[windowSize];
			}
			this._windowSize = windowSize;
			this._pos = 0u;
			this._streamPos = 0u;
		}

		public void Init(Stream stream, bool solid)
		{
			this.ReleaseStream();
			this._stream = stream;
			if (!solid)
			{
				this._streamPos = 0u;
				this._pos = 0u;
				this.TrainSize = 0u;
			}
		}

		public bool Train(Stream stream)
		{
			long length = stream.Length;
			uint num = (length >= (long)((ulong)this._windowSize)) ? this._windowSize : ((uint)length);
			this.TrainSize = num;
			stream.Position = length - (long)((ulong)num);
			this._streamPos = (this._pos = 0u);
			while (num > 0u)
			{
				uint num2 = this._windowSize - this._pos;
				if (num < num2)
				{
					num2 = num;
				}
				int num3 = stream.Read(this._buffer, (int)this._pos, (int)num2);
				if (num3 == 0)
				{
					return false;
				}
				num -= (uint)num3;
				this._pos += (uint)num3;
				this._streamPos += (uint)num3;
				if (this._pos == this._windowSize)
				{
					this._streamPos = (this._pos = 0u);
				}
			}
			return true;
		}

		public void ReleaseStream()
		{
			this.Flush();
			this._stream = null;
		}

		public void Flush()
		{
			uint num = this._pos - this._streamPos;
			if (num == 0u)
			{
				return;
			}
			this._stream.Write(this._buffer, (int)this._streamPos, (int)num);
			if (this._pos >= this._windowSize)
			{
				this._pos = 0u;
			}
			this._streamPos = this._pos;
		}

		public void CopyBlock(uint distance, uint len)
		{
			uint num = this._pos - distance - 1u;
			if (num >= this._windowSize)
			{
				num += this._windowSize;
			}
			while (len > 0u)
			{
				if (num >= this._windowSize)
				{
					num = 0u;
				}
				this._buffer[(int)((UIntPtr)(this._pos++))] = this._buffer[(int)((UIntPtr)(num++))];
				if (this._pos >= this._windowSize)
				{
					this.Flush();
				}
				len -= 1u;
			}
		}

		public void PutByte(byte b)
		{
			this._buffer[(int)((UIntPtr)(this._pos++))] = b;
			if (this._pos >= this._windowSize)
			{
				this.Flush();
			}
		}

		public byte GetByte(uint distance)
		{
			uint num = this._pos - distance - 1u;
			if (num >= this._windowSize)
			{
				num += this._windowSize;
			}
			return this._buffer[(int)((UIntPtr)num)];
		}
	}
}

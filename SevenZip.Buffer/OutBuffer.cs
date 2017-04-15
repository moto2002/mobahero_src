using System;
using System.IO;

namespace SevenZip.Buffer
{
	public class OutBuffer
	{
		private byte[] m_Buffer;

		private uint m_Pos;

		private uint m_BufferSize;

		private Stream m_Stream;

		private ulong m_ProcessedSize;

		public OutBuffer(uint bufferSize)
		{
			this.m_Buffer = new byte[bufferSize];
			this.m_BufferSize = bufferSize;
		}

		public void SetStream(Stream stream)
		{
			this.m_Stream = stream;
		}

		public void FlushStream()
		{
			this.m_Stream.Flush();
		}

		public void CloseStream()
		{
			this.m_Stream.Close();
		}

		public void ReleaseStream()
		{
			this.m_Stream = null;
		}

		public void Init()
		{
			this.m_ProcessedSize = 0uL;
			this.m_Pos = 0u;
		}

		public void WriteByte(byte b)
		{
			this.m_Buffer[(int)((UIntPtr)(this.m_Pos++))] = b;
			if (this.m_Pos >= this.m_BufferSize)
			{
				this.FlushData();
			}
		}

		public void FlushData()
		{
			if (this.m_Pos == 0u)
			{
				return;
			}
			this.m_Stream.Write(this.m_Buffer, 0, (int)this.m_Pos);
			this.m_Pos = 0u;
		}

		public ulong GetProcessedSize()
		{
			return this.m_ProcessedSize + (ulong)this.m_Pos;
		}
	}
}

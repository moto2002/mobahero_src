using System;
using System.IO;

namespace SevenZip
{
	public class CDoubleStream : Stream
	{
		public Stream s1;

		public Stream s2;

		public int fileIndex;

		public long skipSize;

		public override bool CanRead
		{
			get
			{
				return true;
			}
		}

		public override bool CanWrite
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

		public override long Length
		{
			get
			{
				return this.s1.Length + this.s2.Length - this.skipSize;
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

		public override void Flush()
		{
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			int num = 0;
			while (count > 0)
			{
				if (this.fileIndex == 0)
				{
					int num2 = this.s1.Read(buffer, offset, count);
					offset += num2;
					count -= num2;
					num += num2;
					if (num2 == 0)
					{
						this.fileIndex++;
					}
				}
				if (this.fileIndex == 1)
				{
					return num + this.s2.Read(buffer, offset, count);
				}
			}
			return num;
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			throw new Exception("can't Write");
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			throw new Exception("can't Seek");
		}

		public override void SetLength(long value)
		{
			throw new Exception("can't SetLength");
		}
	}
}

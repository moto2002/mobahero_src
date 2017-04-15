using System;
using System.IO;

namespace Pathfinding.Ionic.Zip
{
	public class CountingStream : Stream
	{
		private Stream _s;

		private long _bytesWritten;

		private long _bytesRead;

		private long _initialOffset;

		public Stream WrappedStream
		{
			get
			{
				return this._s;
			}
		}

		public long BytesWritten
		{
			get
			{
				return this._bytesWritten;
			}
		}

		public long BytesRead
		{
			get
			{
				return this._bytesRead;
			}
		}

		public override bool CanRead
		{
			get
			{
				return this._s.CanRead;
			}
		}

		public override bool CanSeek
		{
			get
			{
				return this._s.CanSeek;
			}
		}

		public override bool CanWrite
		{
			get
			{
				return this._s.CanWrite;
			}
		}

		public override long Length
		{
			get
			{
				return this._s.Length;
			}
		}

		public long ComputedPosition
		{
			get
			{
				return this._initialOffset + this._bytesWritten;
			}
		}

		public override long Position
		{
			get
			{
				return this._s.Position;
			}
			set
			{
				this._s.Seek(value, SeekOrigin.Begin);
			}
		}

		public CountingStream(Stream stream)
		{
			this._s = stream;
			try
			{
				this._initialOffset = this._s.Position;
			}
			catch
			{
				this._initialOffset = 0L;
			}
		}

		public void Adjust(long delta)
		{
			this._bytesWritten -= delta;
			if (this._bytesWritten < 0L)
			{
				throw new InvalidOperationException();
			}
			if (this._s is CountingStream)
			{
				((CountingStream)this._s).Adjust(delta);
			}
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			int num = this._s.Read(buffer, offset, count);
			this._bytesRead += (long)num;
			return num;
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			if (count == 0)
			{
				return;
			}
			this._s.Write(buffer, offset, count);
			this._bytesWritten += (long)count;
		}

		public override void Flush()
		{
			this._s.Flush();
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			return this._s.Seek(offset, origin);
		}

		public override void SetLength(long value)
		{
			this._s.SetLength(value);
		}
	}
}

using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace Pathfinding.Ionic.BZip2
{
	public class BZip2OutputStream : Stream
	{
		[Flags]
		private enum TraceBits : uint
		{
			None = 0u,
			Crc = 1u,
			Write = 2u,
			All = 4294967295u
		}

		private int totalBytesWrittenIn;

		private bool leaveOpen;

		private BZip2Compressor compressor;

		private uint combinedCRC;

		private Stream output;

		private BitWriter bw;

		private int blockSize100k;

		private BZip2OutputStream.TraceBits desiredTrace = BZip2OutputStream.TraceBits.Crc | BZip2OutputStream.TraceBits.Write;

		public int BlockSize
		{
			get
			{
				return this.blockSize100k;
			}
		}

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
				if (this.output == null)
				{
					throw new ObjectDisposedException("BZip2Stream");
				}
				return this.output.CanWrite;
			}
		}

		public override long Length
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public override long Position
		{
			get
			{
				return (long)this.totalBytesWrittenIn;
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public BZip2OutputStream(Stream output) : this(output, BZip2.MaxBlockSize, false)
		{
		}

		public BZip2OutputStream(Stream output, int blockSize) : this(output, blockSize, false)
		{
		}

		public BZip2OutputStream(Stream output, bool leaveOpen) : this(output, BZip2.MaxBlockSize, leaveOpen)
		{
		}

		public BZip2OutputStream(Stream output, int blockSize, bool leaveOpen)
		{
			if (blockSize < BZip2.MinBlockSize || blockSize > BZip2.MaxBlockSize)
			{
				string message = string.Format("blockSize={0} is out of range; must be between {1} and {2}", blockSize, BZip2.MinBlockSize, BZip2.MaxBlockSize);
				throw new ArgumentException(message, "blockSize");
			}
			this.output = output;
			if (!this.output.CanWrite)
			{
				throw new ArgumentException("The stream is not writable.", "output");
			}
			this.bw = new BitWriter(this.output);
			this.blockSize100k = blockSize;
			this.compressor = new BZip2Compressor(this.bw, blockSize);
			this.leaveOpen = leaveOpen;
			this.combinedCRC = 0u;
			this.EmitHeader();
		}

		public override void Close()
		{
			if (this.output != null)
			{
				Stream stream = this.output;
				this.Finish();
				if (!this.leaveOpen)
				{
					stream.Close();
				}
			}
		}

		public override void Flush()
		{
			if (this.output != null)
			{
				this.bw.Flush();
				this.output.Flush();
			}
		}

		private void EmitHeader()
		{
			byte[] array = new byte[]
			{
				66,
				90,
				104,
				0
			};
			this.output.Write(array, 0, array.Length);
		}

		private void EmitTrailer()
		{
			this.bw.WriteByte(23);
			this.bw.WriteByte(114);
			this.bw.WriteByte(69);
			this.bw.WriteByte(56);
			this.bw.WriteByte(80);
			this.bw.WriteByte(144);
			this.bw.WriteInt(this.combinedCRC);
			this.bw.FinishAndPad();
		}

		private void Finish()
		{
			try
			{
				int totalBytesWrittenOut = this.bw.TotalBytesWrittenOut;
				this.compressor.CompressAndWrite();
				this.combinedCRC = (this.combinedCRC << 1 | this.combinedCRC >> 31);
				this.combinedCRC ^= this.compressor.Crc32;
				this.EmitTrailer();
			}
			finally
			{
				this.output = null;
				this.compressor = null;
				this.bw = null;
			}
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			if (offset < 0)
			{
				throw new IndexOutOfRangeException(string.Format("offset ({0}) must be > 0", offset));
			}
			if (count < 0)
			{
				throw new IndexOutOfRangeException(string.Format("count ({0}) must be > 0", count));
			}
			if (offset + count > buffer.Length)
			{
				throw new IndexOutOfRangeException(string.Format("offset({0}) count({1}) bLength({2})", offset, count, buffer.Length));
			}
			if (this.output == null)
			{
				throw new IOException("the stream is not open");
			}
			if (count == 0)
			{
				return;
			}
			int num = 0;
			int num2 = count;
			do
			{
				int num3 = this.compressor.Fill(buffer, offset, num2);
				if (num3 != num2)
				{
					int totalBytesWrittenOut = this.bw.TotalBytesWrittenOut;
					this.compressor.CompressAndWrite();
					this.combinedCRC = (this.combinedCRC << 1 | this.combinedCRC >> 31);
					this.combinedCRC ^= this.compressor.Crc32;
					offset += num3;
				}
				num2 -= num3;
				num += num3;
			}
			while (num2 > 0);
			this.totalBytesWrittenIn += num;
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			throw new NotImplementedException();
		}

		public override void SetLength(long value)
		{
			throw new NotImplementedException();
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			throw new NotImplementedException();
		}

		[Conditional("Trace")]
		private void TraceOutput(BZip2OutputStream.TraceBits bits, string format, params object[] varParams)
		{
			if ((bits & this.desiredTrace) != BZip2OutputStream.TraceBits.None)
			{
				int hashCode = Thread.CurrentThread.GetHashCode();
				Console.Write("{0:000} PBOS ", hashCode);
				Console.WriteLine(format, varParams);
			}
		}
	}
}

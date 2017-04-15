using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace Pathfinding.Ionic.BZip2
{
	public class ParallelBZip2OutputStream : Stream
	{
		[Flags]
		private enum TraceBits : uint
		{
			None = 0u,
			Crc = 1u,
			Write = 2u,
			All = 4294967295u
		}

		private static readonly int BufferPairsPerCore = 4;

		private int _maxWorkers;

		private bool firstWriteDone;

		private int lastFilled;

		private int lastWritten;

		private int latestCompressed;

		private int currentlyFilling;

		private volatile Exception pendingException;

		private bool handlingException;

		private bool emitting;

		private Queue<int> toWrite;

		private Queue<int> toFill;

		private List<WorkItem> pool;

		private object latestLock = new object();

		private object eLock = new object();

		private object outputLock = new object();

		private AutoResetEvent newlyCompressedBlob;

		private long totalBytesWrittenIn;

		private long totalBytesWrittenOut;

		private bool leaveOpen;

		private uint combinedCRC;

		private Stream output;

		private BitWriter bw;

		private int blockSize100k;

		private ParallelBZip2OutputStream.TraceBits desiredTrace = ParallelBZip2OutputStream.TraceBits.Crc | ParallelBZip2OutputStream.TraceBits.Write;

		public int MaxWorkers
		{
			get
			{
				return this._maxWorkers;
			}
			set
			{
				if (value < 4)
				{
					throw new ArgumentException("MaxWorkers", "Value must be 4 or greater.");
				}
				this._maxWorkers = value;
			}
		}

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
				return this.totalBytesWrittenIn;
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public long BytesWrittenOut
		{
			get
			{
				return this.totalBytesWrittenOut;
			}
		}

		public ParallelBZip2OutputStream(Stream output) : this(output, BZip2.MaxBlockSize, false)
		{
		}

		public ParallelBZip2OutputStream(Stream output, int blockSize) : this(output, blockSize, false)
		{
		}

		public ParallelBZip2OutputStream(Stream output, bool leaveOpen) : this(output, BZip2.MaxBlockSize, leaveOpen)
		{
		}

		public ParallelBZip2OutputStream(Stream output, int blockSize, bool leaveOpen)
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
			this.leaveOpen = leaveOpen;
			this.combinedCRC = 0u;
			this.MaxWorkers = 16;
			this.EmitHeader();
		}

		private void InitializePoolOfWorkItems()
		{
			this.toWrite = new Queue<int>();
			this.toFill = new Queue<int>();
			this.pool = new List<WorkItem>();
			int num = ParallelBZip2OutputStream.BufferPairsPerCore * Environment.ProcessorCount;
			num = Math.Min(num, this.MaxWorkers);
			for (int i = 0; i < num; i++)
			{
				this.pool.Add(new WorkItem(i, this.blockSize100k));
				this.toFill.Enqueue(i);
			}
			this.newlyCompressedBlob = new AutoResetEvent(false);
			this.currentlyFilling = -1;
			this.lastFilled = -1;
			this.lastWritten = -1;
			this.latestCompressed = -1;
		}

		public override void Close()
		{
			if (this.pendingException != null)
			{
				this.handlingException = true;
				Exception ex = this.pendingException;
				this.pendingException = null;
				throw ex;
			}
			if (this.handlingException)
			{
				return;
			}
			if (this.output == null)
			{
				return;
			}
			Stream stream = this.output;
			try
			{
				this.FlushOutput(true);
			}
			finally
			{
				this.output = null;
				this.bw = null;
			}
			if (!this.leaveOpen)
			{
				stream.Close();
			}
		}

		private void FlushOutput(bool lastInput)
		{
			if (this.emitting)
			{
				return;
			}
			if (this.currentlyFilling >= 0)
			{
				WorkItem wi = this.pool[this.currentlyFilling];
				this.CompressOne(wi);
				this.currentlyFilling = -1;
			}
			if (lastInput)
			{
				this.EmitPendingBuffers(true, false);
				this.EmitTrailer();
			}
			else
			{
				this.EmitPendingBuffers(false, false);
			}
		}

		public override void Flush()
		{
			if (this.output != null)
			{
				this.FlushOutput(false);
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

		public override void Write(byte[] buffer, int offset, int count)
		{
			bool mustWait = false;
			if (this.output == null)
			{
				throw new IOException("the stream is not open");
			}
			if (this.pendingException != null)
			{
				this.handlingException = true;
				Exception ex = this.pendingException;
				this.pendingException = null;
				throw ex;
			}
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
			if (count == 0)
			{
				return;
			}
			if (!this.firstWriteDone)
			{
				this.InitializePoolOfWorkItems();
				this.firstWriteDone = true;
			}
			int num = 0;
			int num2 = count;
			while (true)
			{
				this.EmitPendingBuffers(false, mustWait);
				mustWait = false;
				int index;
				if (this.currentlyFilling >= 0)
				{
					index = this.currentlyFilling;
					goto IL_124;
				}
				if (this.toFill.Count != 0)
				{
					index = this.toFill.Dequeue();
					this.lastFilled++;
					goto IL_124;
				}
				mustWait = true;
				IL_1A0:
				if (num2 <= 0)
				{
					goto Block_12;
				}
				continue;
				IL_124:
				WorkItem workItem = this.pool[index];
				workItem.ordinal = this.lastFilled;
				int num3 = workItem.Compressor.Fill(buffer, offset, num2);
				if (num3 != num2)
				{
					if (!ThreadPool.QueueUserWorkItem(new WaitCallback(this.CompressOne), workItem))
					{
						break;
					}
					this.currentlyFilling = -1;
					offset += num3;
				}
				else
				{
					this.currentlyFilling = index;
				}
				num2 -= num3;
				num += num3;
				goto IL_1A0;
			}
			throw new Exception("Cannot enqueue workitem");
			Block_12:
			this.totalBytesWrittenIn += (long)num;
		}

		private void EmitPendingBuffers(bool doAll, bool mustWait)
		{
			if (this.emitting)
			{
				return;
			}
			this.emitting = true;
			if (doAll || mustWait)
			{
				this.newlyCompressedBlob.WaitOne();
			}
			do
			{
				int num = -1;
				int num2 = (!doAll) ? ((!mustWait) ? 0 : -1) : 200;
				int num3 = -1;
				do
				{
					if (Monitor.TryEnter(this.toWrite, num2))
					{
						num3 = -1;
						try
						{
							if (this.toWrite.Count > 0)
							{
								num3 = this.toWrite.Dequeue();
							}
						}
						finally
						{
							Monitor.Exit(this.toWrite);
						}
						if (num3 >= 0)
						{
							WorkItem workItem = this.pool[num3];
							if (workItem.ordinal != this.lastWritten + 1)
							{
								object obj = this.toWrite;
								lock (obj)
								{
									this.toWrite.Enqueue(num3);
								}
								if (num == num3)
								{
									this.newlyCompressedBlob.WaitOne();
									num = -1;
								}
								else if (num == -1)
								{
									num = num3;
								}
							}
							else
							{
								num = -1;
								BitWriter bitWriter = workItem.bw;
								bitWriter.Flush();
								MemoryStream ms = workItem.ms;
								ms.Seek(0L, SeekOrigin.Begin);
								long num4 = 0L;
								byte[] array = new byte[1024];
								int num5;
								while ((num5 = ms.Read(array, 0, array.Length)) > 0)
								{
									for (int i = 0; i < num5; i++)
									{
										this.bw.WriteByte(array[i]);
									}
									num4 += (long)num5;
								}
								if (bitWriter.NumRemainingBits > 0)
								{
									this.bw.WriteBits(bitWriter.NumRemainingBits, (uint)bitWriter.RemainingBits);
								}
								this.combinedCRC = (this.combinedCRC << 1 | this.combinedCRC >> 31);
								this.combinedCRC ^= workItem.Compressor.Crc32;
								this.totalBytesWrittenOut += num4;
								bitWriter.Reset();
								this.lastWritten = workItem.ordinal;
								workItem.ordinal = -1;
								this.toFill.Enqueue(workItem.index);
								if (num2 == -1)
								{
									num2 = 0;
								}
							}
						}
					}
					else
					{
						num3 = -1;
					}
				}
				while (num3 >= 0);
			}
			while (doAll && this.lastWritten != this.latestCompressed);
			if (doAll)
			{
			}
			this.emitting = false;
		}

		private void CompressOne(object wi)
		{
			WorkItem workItem = (WorkItem)wi;
			try
			{
				workItem.Compressor.CompressAndWrite();
				object obj = this.latestLock;
				lock (obj)
				{
					if (workItem.ordinal > this.latestCompressed)
					{
						this.latestCompressed = workItem.ordinal;
					}
				}
				object obj2 = this.toWrite;
				lock (obj2)
				{
					this.toWrite.Enqueue(workItem.index);
				}
				this.newlyCompressedBlob.Set();
			}
			catch (Exception ex)
			{
				object obj3 = this.eLock;
				lock (obj3)
				{
					if (this.pendingException != null)
					{
						this.pendingException = ex;
					}
				}
			}
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
		private void TraceOutput(ParallelBZip2OutputStream.TraceBits bits, string format, params object[] varParams)
		{
			if ((bits & this.desiredTrace) != ParallelBZip2OutputStream.TraceBits.None)
			{
				object obj = this.outputLock;
				lock (obj)
				{
					int hashCode = Thread.CurrentThread.GetHashCode();
					Console.Write("{0:000} PBOS ", hashCode);
					Console.WriteLine(format, varParams);
				}
			}
		}
	}
}

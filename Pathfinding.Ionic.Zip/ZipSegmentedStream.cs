using System;
using System.IO;

namespace Pathfinding.Ionic.Zip
{
	internal class ZipSegmentedStream : Stream
	{
		private enum RwMode
		{
			None,
			ReadOnly,
			Write
		}

		private ZipSegmentedStream.RwMode rwMode;

		private bool _exceptionPending;

		private string _baseName;

		private string _baseDir;

		private string _currentName;

		private string _currentTempName;

		private uint _currentDiskNumber;

		private uint _maxDiskNumber;

		private int _maxSegmentSize;

		private Stream _innerStream;

		public bool ContiguousWrite
		{
			get;
			set;
		}

		public uint CurrentSegment
		{
			get
			{
				return this._currentDiskNumber;
			}
			private set
			{
				this._currentDiskNumber = value;
				this._currentName = null;
			}
		}

		public string CurrentName
		{
			get
			{
				if (this._currentName == null)
				{
					this._currentName = this._NameForSegment(this.CurrentSegment);
				}
				return this._currentName;
			}
		}

		public string CurrentTempName
		{
			get
			{
				return this._currentTempName;
			}
		}

		public override bool CanRead
		{
			get
			{
				return this.rwMode == ZipSegmentedStream.RwMode.ReadOnly && this._innerStream != null && this._innerStream.CanRead;
			}
		}

		public override bool CanSeek
		{
			get
			{
				return this._innerStream != null && this._innerStream.CanSeek;
			}
		}

		public override bool CanWrite
		{
			get
			{
				return this.rwMode == ZipSegmentedStream.RwMode.Write && this._innerStream != null && this._innerStream.CanWrite;
			}
		}

		public override long Length
		{
			get
			{
				return this._innerStream.Length;
			}
		}

		public override long Position
		{
			get
			{
				return this._innerStream.Position;
			}
			set
			{
				this._innerStream.Position = value;
			}
		}

		private ZipSegmentedStream()
		{
			this._exceptionPending = false;
		}

		public static ZipSegmentedStream ForReading(string name, uint initialDiskNumber, uint maxDiskNumber)
		{
			ZipSegmentedStream zipSegmentedStream = new ZipSegmentedStream
			{
				rwMode = ZipSegmentedStream.RwMode.ReadOnly,
				CurrentSegment = initialDiskNumber,
				_maxDiskNumber = maxDiskNumber,
				_baseName = name
			};
			zipSegmentedStream._SetReadStream();
			return zipSegmentedStream;
		}

		public static ZipSegmentedStream ForWriting(string name, int maxSegmentSize)
		{
			ZipSegmentedStream zipSegmentedStream = new ZipSegmentedStream
			{
				rwMode = ZipSegmentedStream.RwMode.Write,
				CurrentSegment = 0u,
				_baseName = name,
				_maxSegmentSize = maxSegmentSize,
				_baseDir = Path.GetDirectoryName(name)
			};
			if (zipSegmentedStream._baseDir == string.Empty)
			{
				zipSegmentedStream._baseDir = ".";
			}
			zipSegmentedStream._SetWriteStream(0u);
			return zipSegmentedStream;
		}

		public static Stream ForUpdate(string name, uint diskNumber)
		{
			if (diskNumber >= 99u)
			{
				throw new ArgumentOutOfRangeException("diskNumber");
			}
			string path = string.Format("{0}.z{1:D2}", Path.Combine(Path.GetDirectoryName(name), Path.GetFileNameWithoutExtension(name)), diskNumber + 1u);
			return File.Open(path, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
		}

		private string _NameForSegment(uint diskNumber)
		{
			if (diskNumber >= 99u)
			{
				this._exceptionPending = true;
				throw new OverflowException("The number of zip segments would exceed 99.");
			}
			return string.Format("{0}.z{1:D2}", Path.Combine(Path.GetDirectoryName(this._baseName), Path.GetFileNameWithoutExtension(this._baseName)), diskNumber + 1u);
		}

		public uint ComputeSegment(int length)
		{
			if (this._innerStream.Position + (long)length > (long)this._maxSegmentSize)
			{
				return this.CurrentSegment + 1u;
			}
			return this.CurrentSegment;
		}

		public override string ToString()
		{
			return string.Format("{0}[{1}][{2}], pos=0x{3:X})", new object[]
			{
				"ZipSegmentedStream",
				this.CurrentName,
				this.rwMode.ToString(),
				this.Position
			});
		}

		private void _SetReadStream()
		{
			if (this._innerStream != null)
			{
				this._innerStream.Dispose();
			}
			if (this.CurrentSegment + 1u == this._maxDiskNumber)
			{
				this._currentName = this._baseName;
			}
			this._innerStream = File.OpenRead(this.CurrentName);
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			if (this.rwMode != ZipSegmentedStream.RwMode.ReadOnly)
			{
				this._exceptionPending = true;
				throw new InvalidOperationException("Stream Error: Cannot Read.");
			}
			int num = this._innerStream.Read(buffer, offset, count);
			int num2 = num;
			while (num2 != count)
			{
				if (this._innerStream.Position != this._innerStream.Length)
				{
					this._exceptionPending = true;
					throw new ZipException(string.Format("Read error in file {0}", this.CurrentName));
				}
				if (this.CurrentSegment + 1u == this._maxDiskNumber)
				{
					return num;
				}
				this.CurrentSegment += 1u;
				this._SetReadStream();
				offset += num2;
				count -= num2;
				num2 = this._innerStream.Read(buffer, offset, count);
				num += num2;
			}
			return num;
		}

		private void _SetWriteStream(uint increment)
		{
			if (this._innerStream != null)
			{
				this._innerStream.Dispose();
				if (File.Exists(this.CurrentName))
				{
					File.Delete(this.CurrentName);
				}
				File.Move(this._currentTempName, this.CurrentName);
			}
			if (increment > 0u)
			{
				this.CurrentSegment += increment;
			}
			SharedUtilities.CreateAndOpenUniqueTempFile(this._baseDir, out this._innerStream, out this._currentTempName);
			if (this.CurrentSegment == 0u)
			{
				this._innerStream.Write(BitConverter.GetBytes(134695760), 0, 4);
			}
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			if (this.rwMode != ZipSegmentedStream.RwMode.Write)
			{
				this._exceptionPending = true;
				throw new InvalidOperationException("Stream Error: Cannot Write.");
			}
			if (this.ContiguousWrite)
			{
				if (this._innerStream.Position + (long)count > (long)this._maxSegmentSize)
				{
					this._SetWriteStream(1u);
				}
			}
			else
			{
				while (this._innerStream.Position + (long)count > (long)this._maxSegmentSize)
				{
					int num = this._maxSegmentSize - (int)this._innerStream.Position;
					this._innerStream.Write(buffer, offset, num);
					this._SetWriteStream(1u);
					count -= num;
					offset += num;
				}
			}
			this._innerStream.Write(buffer, offset, count);
		}

		public long TruncateBackward(uint diskNumber, long offset)
		{
			if (diskNumber >= 99u)
			{
				throw new ArgumentOutOfRangeException("diskNumber");
			}
			if (this.rwMode != ZipSegmentedStream.RwMode.Write)
			{
				this._exceptionPending = true;
				throw new ZipException("bad state.");
			}
			if (diskNumber == this.CurrentSegment)
			{
				return this._innerStream.Seek(offset, SeekOrigin.Begin);
			}
			if (this._innerStream != null)
			{
				this._innerStream.Dispose();
				if (File.Exists(this._currentTempName))
				{
					File.Delete(this._currentTempName);
				}
			}
			for (uint num = this.CurrentSegment - 1u; num > diskNumber; num -= 1u)
			{
				string path = this._NameForSegment(num);
				if (File.Exists(path))
				{
					File.Delete(path);
				}
			}
			this.CurrentSegment = diskNumber;
			for (int i = 0; i < 3; i++)
			{
				try
				{
					this._currentTempName = SharedUtilities.InternalGetTempFileName();
					File.Move(this.CurrentName, this._currentTempName);
					break;
				}
				catch (IOException)
				{
					if (i == 2)
					{
						throw;
					}
				}
			}
			this._innerStream = new FileStream(this._currentTempName, FileMode.Open);
			return this._innerStream.Seek(offset, SeekOrigin.Begin);
		}

		public override void Flush()
		{
			this._innerStream.Flush();
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			return this._innerStream.Seek(offset, origin);
		}

		public override void SetLength(long value)
		{
			if (this.rwMode != ZipSegmentedStream.RwMode.Write)
			{
				this._exceptionPending = true;
				throw new InvalidOperationException();
			}
			this._innerStream.SetLength(value);
		}

		protected override void Dispose(bool disposing)
		{
			try
			{
				if (this._innerStream != null)
				{
					this._innerStream.Dispose();
					if (this.rwMode != ZipSegmentedStream.RwMode.Write || this._exceptionPending)
					{
					}
				}
			}
			finally
			{
				base.Dispose(disposing);
			}
		}
	}
}

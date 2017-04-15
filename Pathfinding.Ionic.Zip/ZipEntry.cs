using Pathfinding.Ionic.Crc;
using Pathfinding.Ionic.Zlib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace Pathfinding.Ionic.Zip
{
	[ClassInterface(ClassInterfaceType.AutoDispatch), ComVisible(true), Guid("ebc25cf6-9120-4283-b972-0e5520d00004")]
	public class ZipEntry
	{
		private class CopyHelper
		{
			private static Regex re = new Regex(" \\(copy (\\d+)\\)$");

			private static int callCount = 0;

			internal static string AppendCopyToFileName(string f)
			{
				ZipEntry.CopyHelper.callCount++;
				if (ZipEntry.CopyHelper.callCount > 25)
				{
					throw new OverflowException("overflow while creating filename");
				}
				int num = 1;
				int num2 = f.LastIndexOf(".");
				if (num2 == -1)
				{
					Match match = ZipEntry.CopyHelper.re.Match(f);
					if (match.Success)
					{
						num = int.Parse(match.Groups[1].Value) + 1;
						string str = string.Format(" (copy {0})", num);
						f = f.Substring(0, match.Index) + str;
					}
					else
					{
						string str2 = string.Format(" (copy {0})", num);
						f += str2;
					}
				}
				else
				{
					Match match2 = ZipEntry.CopyHelper.re.Match(f.Substring(0, num2));
					if (match2.Success)
					{
						num = int.Parse(match2.Groups[1].Value) + 1;
						string str3 = string.Format(" (copy {0})", num);
						f = f.Substring(0, match2.Index) + str3 + f.Substring(num2);
					}
					else
					{
						string str4 = string.Format(" (copy {0})", num);
						f = f.Substring(0, num2) + str4 + f.Substring(num2);
					}
				}
				return f;
			}
		}

		private delegate T Func<T>();

		private short _VersionMadeBy;

		private short _InternalFileAttrs;

		private int _ExternalFileAttrs;

		private short _filenameLength;

		private short _extraFieldLength;

		private short _commentLength;

		private Stream _inputDecryptorStream;

		private int _readExtraDepth;

		private object _outputLock = new object();

		private ZipCrypto _zipCrypto_forExtract;

		private ZipCrypto _zipCrypto_forWrite;

		internal DateTime _LastModified;

		private DateTime _Mtime;

		private DateTime _Atime;

		private DateTime _Ctime;

		private bool _ntfsTimesAreSet;

		private bool _emitNtfsTimes = true;

		private bool _emitUnixTimes;

		private bool _TrimVolumeFromFullyQualifiedPaths = true;

		internal string _LocalFileName;

		private string _FileNameInArchive;

		internal short _VersionNeeded;

		internal short _BitField;

		internal short _CompressionMethod;

		private short _CompressionMethod_FromZipFile;

		private CompressionLevel _CompressionLevel;

		internal string _Comment;

		private bool _IsDirectory;

		private byte[] _CommentBytes;

		internal long _CompressedSize;

		internal long _CompressedFileDataSize;

		internal long _UncompressedSize;

		internal int _TimeBlob;

		private bool _crcCalculated;

		internal int _Crc32;

		internal byte[] _Extra;

		private bool _metadataChanged;

		private bool _restreamRequiredOnSave;

		private bool _sourceIsEncrypted;

		private bool _skippedDuringSave;

		private uint _diskNumber;

		private static Encoding ibm437 = Encoding.UTF8;

		private Encoding _actualEncoding;

		internal ZipContainer _container;

		private long __FileDataPosition = -1L;

		private byte[] _EntryHeader;

		internal long _RelativeOffsetOfLocalHeader;

		private long _future_ROLH;

		private long _TotalEntrySize;

		private int _LengthOfHeader;

		private int _LengthOfTrailer;

		internal bool _InputUsesZip64;

		private uint _UnsupportedAlgorithmId;

		internal string _Password;

		internal ZipEntrySource _Source;

		internal EncryptionAlgorithm _Encryption;

		internal EncryptionAlgorithm _Encryption_FromZipFile;

		internal byte[] _WeakEncryptionHeader;

		internal Stream _archiveStream;

		private Stream _sourceStream;

		private long? _sourceStreamOriginalPosition;

		private bool _sourceWasJitProvided;

		private bool _ioOperationCanceled;

		private bool _presumeZip64;

		private bool? _entryRequiresZip64;

		private bool? _OutputUsesZip64;

		private bool _IsText;

		private ZipEntryTimestamp _timestamp;

		private static DateTime _unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

		private static DateTime _win32Epoch = DateTime.FromFileTimeUtc(0L);

		private static DateTime _zeroHour = new DateTime(1, 1, 1, 0, 0, 0, DateTimeKind.Utc);

		private WriteDelegate _WriteDelegate;

		private OpenDelegate _OpenDelegate;

		private CloseDelegate _CloseDelegate;

		internal bool AttributesIndicateDirectory
		{
			get
			{
				return this._InternalFileAttrs == 0 && (this._ExternalFileAttrs & 16) == 16;
			}
		}

		public string Info
		{
			get
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append(string.Format("          ZipEntry: {0}\n", this.FileName)).Append(string.Format("   Version Made By: {0}\n", this._VersionMadeBy)).Append(string.Format(" Needed to extract: {0}\n", this.VersionNeeded));
				if (this._IsDirectory)
				{
					stringBuilder.Append("        Entry type: directory\n");
				}
				else
				{
					stringBuilder.Append(string.Format("         File type: {0}\n", (!this._IsText) ? "binary" : "text")).Append(string.Format("       Compression: {0}\n", this.CompressionMethod)).Append(string.Format("        Compressed: 0x{0:X}\n", this.CompressedSize)).Append(string.Format("      Uncompressed: 0x{0:X}\n", this.UncompressedSize)).Append(string.Format("             CRC32: 0x{0:X8}\n", this._Crc32));
				}
				stringBuilder.Append(string.Format("       Disk Number: {0}\n", this._diskNumber));
				if (this._RelativeOffsetOfLocalHeader > (long)((ulong)-1))
				{
					stringBuilder.Append(string.Format("   Relative Offset: 0x{0:X16}\n", this._RelativeOffsetOfLocalHeader));
				}
				else
				{
					stringBuilder.Append(string.Format("   Relative Offset: 0x{0:X8}\n", this._RelativeOffsetOfLocalHeader));
				}
				stringBuilder.Append(string.Format("         Bit Field: 0x{0:X4}\n", this._BitField)).Append(string.Format("        Encrypted?: {0}\n", this._sourceIsEncrypted)).Append(string.Format("          Timeblob: 0x{0:X8}\n", this._TimeBlob)).Append(string.Format("              Time: {0}\n", SharedUtilities.PackedToDateTime(this._TimeBlob)));
				stringBuilder.Append(string.Format("         Is Zip64?: {0}\n", this._InputUsesZip64));
				if (!string.IsNullOrEmpty(this._Comment))
				{
					stringBuilder.Append(string.Format("           Comment: {0}\n", this._Comment));
				}
				stringBuilder.Append("\n");
				return stringBuilder.ToString();
			}
		}

		private string UnsupportedAlgorithm
		{
			get
			{
				string result = string.Empty;
				uint unsupportedAlgorithmId = this._UnsupportedAlgorithmId;
				switch (unsupportedAlgorithmId)
				{
				case 26121u:
					result = "3DES-112";
					return result;
				case 26122u:
				case 26123u:
				case 26124u:
				case 26125u:
					IL_39:
					switch (unsupportedAlgorithmId)
					{
					case 26113u:
						result = "DES";
						return result;
					case 26114u:
						result = "RC2";
						return result;
					case 26115u:
						result = "3DES-168";
						return result;
					default:
						if (unsupportedAlgorithmId == 26400u)
						{
							result = "Blowfish";
							return result;
						}
						if (unsupportedAlgorithmId == 26401u)
						{
							result = "Twofish";
							return result;
						}
						if (unsupportedAlgorithmId == 0u)
						{
							result = "--";
							return result;
						}
						if (unsupportedAlgorithmId == 26370u)
						{
							result = "RC2";
							return result;
						}
						if (unsupportedAlgorithmId != 26625u)
						{
							if (unsupportedAlgorithmId != 65535u)
							{
							}
							result = string.Format("Unknown (0x{0:X4})", this._UnsupportedAlgorithmId);
							return result;
						}
						result = "RC4";
						return result;
					}
					break;
				case 26126u:
					result = "PKWare AES128";
					return result;
				case 26127u:
					result = "PKWare AES192";
					return result;
				case 26128u:
					result = "PKWare AES256";
					return result;
				}
				goto IL_39;
			}
		}

		private string UnsupportedCompressionMethod
		{
			get
			{
				string result = string.Empty;
				int compressionMethod = (int)this._CompressionMethod;
				switch (compressionMethod)
				{
				case 8:
					result = "DEFLATE";
					return result;
				case 9:
					result = "Deflate64";
					return result;
				case 10:
				case 11:
				case 13:
					IL_31:
					if (compressionMethod == 0)
					{
						result = "Store";
						return result;
					}
					if (compressionMethod == 1)
					{
						result = "Shrink";
						return result;
					}
					if (compressionMethod == 19)
					{
						result = "LZ77";
						return result;
					}
					if (compressionMethod != 98)
					{
						result = string.Format("Unknown (0x{0:X4})", this._CompressionMethod);
						return result;
					}
					result = "PPMd";
					return result;
				case 12:
					result = "BZIP2";
					return result;
				case 14:
					result = "LZMA";
					return result;
				}
				goto IL_31;
			}
		}

		public DateTime LastModified
		{
			get
			{
				return this._LastModified.ToLocalTime();
			}
			set
			{
				this._LastModified = ((value.Kind != DateTimeKind.Unspecified) ? value.ToLocalTime() : DateTime.SpecifyKind(value, DateTimeKind.Local));
				this._Mtime = SharedUtilities.AdjustTime_Reverse(this._LastModified).ToUniversalTime();
				this._metadataChanged = true;
			}
		}

		private int BufferSize
		{
			get
			{
				return this._container.BufferSize;
			}
		}

		public DateTime ModifiedTime
		{
			get
			{
				return this._Mtime;
			}
			set
			{
				this.SetEntryTimes(this._Ctime, this._Atime, value);
			}
		}

		public DateTime AccessedTime
		{
			get
			{
				return this._Atime;
			}
			set
			{
				this.SetEntryTimes(this._Ctime, value, this._Mtime);
			}
		}

		public DateTime CreationTime
		{
			get
			{
				return this._Ctime;
			}
			set
			{
				this.SetEntryTimes(value, this._Atime, this._Mtime);
			}
		}

		public bool EmitTimesInWindowsFormatWhenSaving
		{
			get
			{
				return this._emitNtfsTimes;
			}
			set
			{
				this._emitNtfsTimes = value;
				this._metadataChanged = true;
			}
		}

		public bool EmitTimesInUnixFormatWhenSaving
		{
			get
			{
				return this._emitUnixTimes;
			}
			set
			{
				this._emitUnixTimes = value;
				this._metadataChanged = true;
			}
		}

		public ZipEntryTimestamp Timestamp
		{
			get
			{
				return this._timestamp;
			}
		}

		public FileAttributes Attributes
		{
			get
			{
				return (FileAttributes)this._ExternalFileAttrs;
			}
			set
			{
				this._ExternalFileAttrs = (int)value;
				this._VersionMadeBy = 45;
				this._metadataChanged = true;
			}
		}

		internal string LocalFileName
		{
			get
			{
				return this._LocalFileName;
			}
		}

		public string FileName
		{
			get
			{
				return this._FileNameInArchive;
			}
			set
			{
				if (this._container.ZipFile == null)
				{
					throw new ZipException("Cannot rename; this is not supported in ZipOutputStream/ZipInputStream.");
				}
				if (string.IsNullOrEmpty(value))
				{
					throw new ZipException("The FileName must be non empty and non-null.");
				}
				string text = ZipEntry.NameInArchive(value, null);
				if (this._FileNameInArchive == text)
				{
					return;
				}
				this._container.ZipFile.RemoveEntry(this);
				this._container.ZipFile.InternalAddEntry(text, this);
				this._FileNameInArchive = text;
				this._container.ZipFile.NotifyEntryChanged();
				this._metadataChanged = true;
			}
		}

		public Stream InputStream
		{
			get
			{
				return this._sourceStream;
			}
			set
			{
				if (this._Source != ZipEntrySource.Stream)
				{
					throw new ZipException("You must not set the input stream for this entry.");
				}
				this._sourceWasJitProvided = true;
				this._sourceStream = value;
			}
		}

		public bool InputStreamWasJitProvided
		{
			get
			{
				return this._sourceWasJitProvided;
			}
		}

		public ZipEntrySource Source
		{
			get
			{
				return this._Source;
			}
		}

		public short VersionNeeded
		{
			get
			{
				return this._VersionNeeded;
			}
		}

		public string Comment
		{
			get
			{
				return this._Comment;
			}
			set
			{
				this._Comment = value;
				this._metadataChanged = true;
			}
		}

		public bool? RequiresZip64
		{
			get
			{
				return this._entryRequiresZip64;
			}
		}

		public bool? OutputUsedZip64
		{
			get
			{
				return this._OutputUsesZip64;
			}
		}

		public short BitField
		{
			get
			{
				return this._BitField;
			}
		}

		public CompressionMethod CompressionMethod
		{
			get
			{
				return (CompressionMethod)this._CompressionMethod;
			}
			set
			{
				if (value == (CompressionMethod)this._CompressionMethod)
				{
					return;
				}
				if (value != CompressionMethod.None && value != CompressionMethod.Deflate)
				{
					throw new InvalidOperationException("Unsupported compression method.");
				}
				this._CompressionMethod = (short)value;
				if (this._CompressionMethod == 0)
				{
					this._CompressionLevel = CompressionLevel.None;
				}
				else if (this.CompressionLevel == CompressionLevel.None)
				{
					this._CompressionLevel = CompressionLevel.Default;
				}
				if (this._container.ZipFile != null)
				{
					this._container.ZipFile.NotifyEntryChanged();
				}
				this._restreamRequiredOnSave = true;
			}
		}

		public CompressionLevel CompressionLevel
		{
			get
			{
				return this._CompressionLevel;
			}
			set
			{
				if (this._CompressionMethod != 8 && this._CompressionMethod != 0)
				{
					return;
				}
				if (value == CompressionLevel.Default && this._CompressionMethod == 8)
				{
					return;
				}
				this._CompressionLevel = value;
				if (value == CompressionLevel.None && this._CompressionMethod == 0)
				{
					return;
				}
				if (this._CompressionLevel == CompressionLevel.None)
				{
					this._CompressionMethod = 0;
				}
				else
				{
					this._CompressionMethod = 8;
				}
				if (this._container.ZipFile != null)
				{
					this._container.ZipFile.NotifyEntryChanged();
				}
				this._restreamRequiredOnSave = true;
			}
		}

		public long CompressedSize
		{
			get
			{
				return this._CompressedSize;
			}
		}

		public long UncompressedSize
		{
			get
			{
				return this._UncompressedSize;
			}
		}

		public double CompressionRatio
		{
			get
			{
				if (this.UncompressedSize == 0L)
				{
					return 0.0;
				}
				return 100.0 * (1.0 - 1.0 * (double)this.CompressedSize / (1.0 * (double)this.UncompressedSize));
			}
		}

		public int Crc
		{
			get
			{
				return this._Crc32;
			}
		}

		public bool IsDirectory
		{
			get
			{
				return this._IsDirectory;
			}
		}

		public bool UsesEncryption
		{
			get
			{
				return this._Encryption_FromZipFile != EncryptionAlgorithm.None;
			}
		}

		public EncryptionAlgorithm Encryption
		{
			get
			{
				return this._Encryption;
			}
			set
			{
				if (value == this._Encryption)
				{
					return;
				}
				if (value == EncryptionAlgorithm.Unsupported)
				{
					throw new InvalidOperationException("You may not set Encryption to that value.");
				}
				this._Encryption = value;
				this._restreamRequiredOnSave = true;
				if (this._container.ZipFile != null)
				{
					this._container.ZipFile.NotifyEntryChanged();
				}
			}
		}

		public string Password
		{
			private get
			{
				return this._Password;
			}
			set
			{
				this._Password = value;
				if (this._Password == null)
				{
					this._Encryption = EncryptionAlgorithm.None;
				}
				else
				{
					if (this._Source == ZipEntrySource.ZipFile && !this._sourceIsEncrypted)
					{
						this._restreamRequiredOnSave = true;
					}
					if (this.Encryption == EncryptionAlgorithm.None)
					{
						this._Encryption = EncryptionAlgorithm.PkzipWeak;
					}
				}
			}
		}

		internal bool IsChanged
		{
			get
			{
				return this._restreamRequiredOnSave | this._metadataChanged;
			}
		}

		public ExtractExistingFileAction ExtractExistingFile
		{
			get;
			set;
		}

		public ZipErrorAction ZipErrorAction
		{
			get;
			set;
		}

		public bool IncludedInMostRecentSave
		{
			get
			{
				return !this._skippedDuringSave;
			}
		}

		public SetCompressionCallback SetCompression
		{
			get;
			set;
		}

		[Obsolete("Beginning with v1.9.1.6 of DotNetZip, this property is obsolete.  It will be removed in a future version of the library. Your applications should  use AlternateEncoding and AlternateEncodingUsage instead.")]
		public bool UseUnicodeAsNecessary
		{
			get
			{
				return this.AlternateEncoding == Encoding.GetEncoding("UTF-8") && this.AlternateEncodingUsage == ZipOption.AsNecessary;
			}
			set
			{
				if (value)
				{
					this.AlternateEncoding = Encoding.GetEncoding("UTF-8");
					this.AlternateEncodingUsage = ZipOption.AsNecessary;
				}
				else
				{
					this.AlternateEncoding = ZipFile.DefaultEncoding;
					this.AlternateEncodingUsage = ZipOption.Default;
				}
			}
		}

		[Obsolete("This property is obsolete since v1.9.1.6. Use AlternateEncoding and AlternateEncodingUsage instead.", true)]
		public Encoding ProvisionalAlternateEncoding
		{
			get;
			set;
		}

		public Encoding AlternateEncoding
		{
			get;
			set;
		}

		public ZipOption AlternateEncodingUsage
		{
			get;
			set;
		}

		public bool IsText
		{
			get
			{
				return this._IsText;
			}
			set
			{
				this._IsText = value;
			}
		}

		internal Stream ArchiveStream
		{
			get
			{
				if (this._archiveStream == null)
				{
					if (this._container.ZipFile != null)
					{
						ZipFile zipFile = this._container.ZipFile;
						zipFile.Reset(false);
						this._archiveStream = zipFile.StreamForDiskNumber(this._diskNumber);
					}
					else
					{
						this._archiveStream = this._container.ZipOutputStream.OutputStream;
					}
				}
				return this._archiveStream;
			}
		}

		internal long FileDataPosition
		{
			get
			{
				if (this.__FileDataPosition == -1L)
				{
					this.SetFdpLoh();
				}
				return this.__FileDataPosition;
			}
		}

		private int LengthOfHeader
		{
			get
			{
				if (this._LengthOfHeader == 0)
				{
					this.SetFdpLoh();
				}
				return this._LengthOfHeader;
			}
		}

		public ZipEntry()
		{
			this._CompressionMethod = 8;
			this._CompressionLevel = CompressionLevel.Default;
			this._Encryption = EncryptionAlgorithm.None;
			this._Source = ZipEntrySource.None;
			this.AlternateEncoding = Encoding.UTF8;
			this.AlternateEncodingUsage = ZipOption.Default;
		}

		internal void ResetDirEntry()
		{
			this.__FileDataPosition = -1L;
			this._LengthOfHeader = 0;
		}

		internal static ZipEntry ReadDirEntry(ZipFile zf, Dictionary<string, object> previouslySeen)
		{
			Stream readStream = zf.ReadStream;
			Encoding encoding = (zf.AlternateEncodingUsage != ZipOption.Always) ? ZipFile.DefaultEncoding : zf.AlternateEncoding;
			int num = SharedUtilities.ReadSignature(readStream);
			if (ZipEntry.IsNotValidZipDirEntrySig(num))
			{
				readStream.Seek(-4L, SeekOrigin.Current);
				if ((long)num != 101010256L && (long)num != 101075792L && num != 67324752)
				{
					throw new BadReadException(string.Format("  Bad signature (0x{0:X8}) at position 0x{1:X8}", num, readStream.Position));
				}
				return null;
			}
			else
			{
				int num2 = 46;
				byte[] array = new byte[42];
				int num3 = readStream.Read(array, 0, array.Length);
				if (num3 != array.Length)
				{
					return null;
				}
				int num4 = 0;
				ZipEntry zipEntry = new ZipEntry();
				zipEntry.AlternateEncoding = encoding;
				zipEntry._Source = ZipEntrySource.ZipFile;
				zipEntry._container = new ZipContainer(zf);
				zipEntry._VersionMadeBy = (short)((int)array[num4++] + (int)array[num4++] * 256);
				zipEntry._VersionNeeded = (short)((int)array[num4++] + (int)array[num4++] * 256);
				zipEntry._BitField = (short)((int)array[num4++] + (int)array[num4++] * 256);
				zipEntry._CompressionMethod = (short)((int)array[num4++] + (int)array[num4++] * 256);
				zipEntry._TimeBlob = (int)array[num4++] + (int)array[num4++] * 256 + (int)array[num4++] * 256 * 256 + (int)array[num4++] * 256 * 256 * 256;
				zipEntry._LastModified = SharedUtilities.PackedToDateTime(zipEntry._TimeBlob);
				zipEntry._timestamp |= ZipEntryTimestamp.DOS;
				zipEntry._Crc32 = (int)array[num4++] + (int)array[num4++] * 256 + (int)array[num4++] * 256 * 256 + (int)array[num4++] * 256 * 256 * 256;
				zipEntry._CompressedSize = (long)((ulong)((int)array[num4++] + (int)array[num4++] * 256 + (int)array[num4++] * 256 * 256 + (int)array[num4++] * 256 * 256 * 256));
				zipEntry._UncompressedSize = (long)((ulong)((int)array[num4++] + (int)array[num4++] * 256 + (int)array[num4++] * 256 * 256 + (int)array[num4++] * 256 * 256 * 256));
				zipEntry._CompressionMethod_FromZipFile = zipEntry._CompressionMethod;
				zipEntry._filenameLength = (short)((int)array[num4++] + (int)array[num4++] * 256);
				zipEntry._extraFieldLength = (short)((int)array[num4++] + (int)array[num4++] * 256);
				zipEntry._commentLength = (short)((int)array[num4++] + (int)array[num4++] * 256);
				zipEntry._diskNumber = (uint)array[num4++] + (uint)array[num4++] * 256u;
				zipEntry._InternalFileAttrs = (short)((int)array[num4++] + (int)array[num4++] * 256);
				zipEntry._ExternalFileAttrs = (int)array[num4++] + (int)array[num4++] * 256 + (int)array[num4++] * 256 * 256 + (int)array[num4++] * 256 * 256 * 256;
				zipEntry._RelativeOffsetOfLocalHeader = (long)((ulong)((int)array[num4++] + (int)array[num4++] * 256 + (int)array[num4++] * 256 * 256 + (int)array[num4++] * 256 * 256 * 256));
				zipEntry.IsText = ((zipEntry._InternalFileAttrs & 1) == 1);
				array = new byte[(int)zipEntry._filenameLength];
				num3 = readStream.Read(array, 0, array.Length);
				num2 += num3;
				if ((zipEntry._BitField & 2048) == 2048)
				{
					zipEntry._FileNameInArchive = SharedUtilities.Utf8StringFromBuffer(array);
				}
				else
				{
					zipEntry._FileNameInArchive = SharedUtilities.StringFromBuffer(array, encoding);
				}
				while (previouslySeen.ContainsKey(zipEntry._FileNameInArchive))
				{
					zipEntry._FileNameInArchive = ZipEntry.CopyHelper.AppendCopyToFileName(zipEntry._FileNameInArchive);
					zipEntry._metadataChanged = true;
				}
				if (zipEntry.AttributesIndicateDirectory)
				{
					zipEntry.MarkAsDirectory();
				}
				else if (zipEntry._FileNameInArchive.EndsWith("/"))
				{
					zipEntry.MarkAsDirectory();
				}
				zipEntry._CompressedFileDataSize = zipEntry._CompressedSize;
				if ((zipEntry._BitField & 1) == 1)
				{
					zipEntry._Encryption_FromZipFile = (zipEntry._Encryption = EncryptionAlgorithm.PkzipWeak);
					zipEntry._sourceIsEncrypted = true;
				}
				if (zipEntry._extraFieldLength > 0)
				{
					zipEntry._InputUsesZip64 = (zipEntry._CompressedSize == (long)((ulong)-1) || zipEntry._UncompressedSize == (long)((ulong)-1) || zipEntry._RelativeOffsetOfLocalHeader == (long)((ulong)-1));
					num2 += zipEntry.ProcessExtraField(readStream, zipEntry._extraFieldLength);
					zipEntry._CompressedFileDataSize = zipEntry._CompressedSize;
				}
				if (zipEntry._Encryption == EncryptionAlgorithm.PkzipWeak)
				{
					zipEntry._CompressedFileDataSize -= 12L;
				}
				if ((zipEntry._BitField & 8) == 8)
				{
					if (zipEntry._InputUsesZip64)
					{
						zipEntry._LengthOfTrailer += 24;
					}
					else
					{
						zipEntry._LengthOfTrailer += 16;
					}
				}
				zipEntry.AlternateEncoding = (((zipEntry._BitField & 2048) != 2048) ? encoding : Encoding.UTF8);
				zipEntry.AlternateEncodingUsage = ZipOption.Always;
				if (zipEntry._commentLength > 0)
				{
					array = new byte[(int)zipEntry._commentLength];
					num3 = readStream.Read(array, 0, array.Length);
					num2 += num3;
					if ((zipEntry._BitField & 2048) == 2048)
					{
						zipEntry._Comment = SharedUtilities.Utf8StringFromBuffer(array);
					}
					else
					{
						zipEntry._Comment = SharedUtilities.StringFromBuffer(array, encoding);
					}
				}
				return zipEntry;
			}
		}

		internal static bool IsNotValidZipDirEntrySig(int signature)
		{
			return signature != 33639248;
		}

		public void Extract()
		{
			this.InternalExtract(".", null, null);
		}

		public void Extract(ExtractExistingFileAction extractExistingFile)
		{
			this.ExtractExistingFile = extractExistingFile;
			this.InternalExtract(".", null, null);
		}

		public void Extract(Stream stream)
		{
			this.InternalExtract(null, stream, null);
		}

		public void Extract(string baseDirectory)
		{
			this.InternalExtract(baseDirectory, null, null);
		}

		public void Extract(string baseDirectory, ExtractExistingFileAction extractExistingFile)
		{
			this.ExtractExistingFile = extractExistingFile;
			this.InternalExtract(baseDirectory, null, null);
		}

		public void ExtractWithPassword(string password)
		{
			this.InternalExtract(".", null, password);
		}

		public void ExtractWithPassword(string baseDirectory, string password)
		{
			this.InternalExtract(baseDirectory, null, password);
		}

		public void ExtractWithPassword(ExtractExistingFileAction extractExistingFile, string password)
		{
			this.ExtractExistingFile = extractExistingFile;
			this.InternalExtract(".", null, password);
		}

		public void ExtractWithPassword(string baseDirectory, ExtractExistingFileAction extractExistingFile, string password)
		{
			this.ExtractExistingFile = extractExistingFile;
			this.InternalExtract(baseDirectory, null, password);
		}

		public void ExtractWithPassword(Stream stream, string password)
		{
			this.InternalExtract(null, stream, password);
		}

		public CrcCalculatorStream OpenReader()
		{
			if (this._container.ZipFile == null)
			{
				throw new InvalidOperationException("Use OpenReader() only with ZipFile.");
			}
			return this.InternalOpenReader(this._Password ?? this._container.Password);
		}

		public CrcCalculatorStream OpenReader(string password)
		{
			if (this._container.ZipFile == null)
			{
				throw new InvalidOperationException("Use OpenReader() only with ZipFile.");
			}
			return this.InternalOpenReader(password);
		}

		internal CrcCalculatorStream InternalOpenReader(string password)
		{
			this.ValidateCompression();
			this.ValidateEncryption();
			this.SetupCryptoForExtract(password);
			if (this._Source != ZipEntrySource.ZipFile)
			{
				throw new BadStateException("You must call ZipFile.Save before calling OpenReader");
			}
			long length = (this._CompressionMethod_FromZipFile != 0) ? this.UncompressedSize : this._CompressedFileDataSize;
			Stream archiveStream = this.ArchiveStream;
			this.ArchiveStream.Seek(this.FileDataPosition, SeekOrigin.Begin);
			this._inputDecryptorStream = this.GetExtractDecryptor(archiveStream);
			Stream extractDecompressor = this.GetExtractDecompressor(this._inputDecryptorStream);
			return new CrcCalculatorStream(extractDecompressor, length);
		}

		private void OnExtractProgress(long bytesWritten, long totalBytesToWrite)
		{
			if (this._container.ZipFile != null)
			{
				this._ioOperationCanceled = this._container.ZipFile.OnExtractBlock(this, bytesWritten, totalBytesToWrite);
			}
		}

		private void OnBeforeExtract(string path)
		{
			if (this._container.ZipFile != null && !this._container.ZipFile._inExtractAll)
			{
				this._ioOperationCanceled = this._container.ZipFile.OnSingleEntryExtract(this, path, true);
			}
		}

		private void OnAfterExtract(string path)
		{
			if (this._container.ZipFile != null && !this._container.ZipFile._inExtractAll)
			{
				this._container.ZipFile.OnSingleEntryExtract(this, path, false);
			}
		}

		private void OnExtractExisting(string path)
		{
			if (this._container.ZipFile != null)
			{
				this._ioOperationCanceled = this._container.ZipFile.OnExtractExisting(this, path);
			}
		}

		private static void ReallyDelete(string fileName)
		{
			File.Delete(fileName);
		}

		private void WriteStatus(string format, params object[] args)
		{
			if (this._container.ZipFile != null && this._container.ZipFile.Verbose)
			{
				this._container.ZipFile.StatusMessageTextWriter.WriteLine(format, args);
			}
		}

		private void InternalExtract(string baseDir, Stream outstream, string password)
		{
			if (this._container == null)
			{
				throw new BadStateException("This entry is an orphan");
			}
			if (this._container.ZipFile == null)
			{
				throw new InvalidOperationException("Use Extract() only with ZipFile.");
			}
			this._container.ZipFile.Reset(false);
			if (this._Source != ZipEntrySource.ZipFile)
			{
				throw new BadStateException("You must call ZipFile.Save before calling any Extract method");
			}
			this.OnBeforeExtract(baseDir);
			this._ioOperationCanceled = false;
			string text = null;
			Stream stream = null;
			bool flag = false;
			bool flag2 = false;
			try
			{
				this.ValidateCompression();
				this.ValidateEncryption();
				if (this.ValidateOutput(baseDir, outstream, out text))
				{
					this.WriteStatus("extract dir {0}...", new object[]
					{
						text
					});
					this.OnAfterExtract(baseDir);
				}
				else
				{
					if (text != null && File.Exists(text))
					{
						flag = true;
						int num = this.CheckExtractExistingFile(baseDir, text);
						if (num == 2)
						{
							goto IL_2C6;
						}
						if (num == 1)
						{
							return;
						}
					}
					string text2 = password ?? (this._Password ?? this._container.Password);
					if (this._Encryption_FromZipFile != EncryptionAlgorithm.None)
					{
						if (text2 == null)
						{
							throw new BadPasswordException();
						}
						this.SetupCryptoForExtract(text2);
					}
					if (text != null)
					{
						this.WriteStatus("extract file {0}...", new object[]
						{
							text
						});
						text += ".tmp";
						string directoryName = Path.GetDirectoryName(text);
						if (!Directory.Exists(directoryName))
						{
							Directory.CreateDirectory(directoryName);
						}
						else if (this._container.ZipFile != null)
						{
							flag2 = this._container.ZipFile._inExtractAll;
						}
						stream = new FileStream(text, FileMode.CreateNew);
					}
					else
					{
						this.WriteStatus("extract entry {0} to stream...", new object[]
						{
							this.FileName
						});
						stream = outstream;
					}
					if (!this._ioOperationCanceled)
					{
						int actualCrc = this.ExtractOne(stream);
						if (!this._ioOperationCanceled)
						{
							this.VerifyCrcAfterExtract(actualCrc);
							if (text != null)
							{
								stream.Close();
								stream = null;
								string text3 = text;
								string text4 = null;
								text = text3.Substring(0, text3.Length - 4);
								if (flag)
								{
									text4 = text + ".PendingOverwrite";
								}
								this._SetTimes(text, true);
								if (text4 != null && File.Exists(text4))
								{
									ZipEntry.ReallyDelete(text4);
								}
								if (flag2 && this.FileName.IndexOf('/') != -1)
								{
									string directoryName2 = Path.GetDirectoryName(this.FileName);
									if (this._container.ZipFile[directoryName2] == null)
									{
										this._SetTimes(Path.GetDirectoryName(text), false);
									}
								}
								if (((int)this._VersionMadeBy & 65280) == 2560 || ((int)this._VersionMadeBy & 65280) == 0)
								{
									File.SetAttributes(text, (FileAttributes)this._ExternalFileAttrs);
								}
							}
							this.OnAfterExtract(baseDir);
						}
					}
					IL_2C6:;
				}
			}
			catch (Exception)
			{
				this._ioOperationCanceled = true;
				throw;
			}
			finally
			{
				if (this._ioOperationCanceled && text != null)
				{
					try
					{
						if (stream != null)
						{
							stream.Close();
						}
						if (File.Exists(text) && !flag)
						{
							File.Delete(text);
						}
					}
					finally
					{
					}
				}
			}
		}

		internal void VerifyCrcAfterExtract(int actualCrc32)
		{
			if (actualCrc32 != this._Crc32)
			{
				throw new BadCrcException("CRC error: the file being extracted appears to be corrupted. " + string.Format("Expected 0x{0:X8}, Actual 0x{1:X8}", this._Crc32, actualCrc32));
			}
		}

		private int CheckExtractExistingFile(string baseDir, string targetFileName)
		{
			int num = 0;
			while (true)
			{
				switch (this.ExtractExistingFile)
				{
				case ExtractExistingFileAction.OverwriteSilently:
					goto IL_24;
				case ExtractExistingFileAction.DoNotOverwrite:
					goto IL_3B;
				case ExtractExistingFileAction.InvokeExtractProgressEvent:
					if (num > 0)
					{
						goto Block_2;
					}
					this.OnExtractExisting(baseDir);
					if (this._ioOperationCanceled)
					{
						return 2;
					}
					num++;
					continue;
				}
				break;
			}
			goto IL_8F;
			IL_24:
			this.WriteStatus("the file {0} exists; will overwrite it...", new object[]
			{
				targetFileName
			});
			return 0;
			IL_3B:
			this.WriteStatus("the file {0} exists; not extracting entry...", new object[]
			{
				this.FileName
			});
			this.OnAfterExtract(baseDir);
			return 1;
			Block_2:
			throw new ZipException(string.Format("The file {0} already exists.", targetFileName));
			IL_8F:
			throw new ZipException(string.Format("The file {0} already exists.", targetFileName));
		}

		private void _CheckRead(int nbytes)
		{
			if (nbytes == 0)
			{
				throw new BadReadException(string.Format("bad read of entry {0} from compressed archive.", this.FileName));
			}
		}

		private int ExtractOne(Stream output)
		{
			int result = 0;
			Stream archiveStream = this.ArchiveStream;
			try
			{
				archiveStream.Seek(this.FileDataPosition, SeekOrigin.Begin);
				byte[] array = new byte[this.BufferSize];
				long num = (this._CompressionMethod_FromZipFile == 0) ? this._CompressedFileDataSize : this.UncompressedSize;
				this._inputDecryptorStream = this.GetExtractDecryptor(archiveStream);
				Stream extractDecompressor = this.GetExtractDecompressor(this._inputDecryptorStream);
				long num2 = 0L;
				using (CrcCalculatorStream crcCalculatorStream = new CrcCalculatorStream(extractDecompressor))
				{
					while (num > 0L)
					{
						int count = (num <= (long)array.Length) ? ((int)num) : array.Length;
						int num3 = crcCalculatorStream.Read(array, 0, count);
						this._CheckRead(num3);
						output.Write(array, 0, num3);
						num -= (long)num3;
						num2 += (long)num3;
						this.OnExtractProgress(num2, this.UncompressedSize);
						if (this._ioOperationCanceled)
						{
							break;
						}
					}
					result = crcCalculatorStream.Crc;
				}
			}
			finally
			{
				ZipSegmentedStream zipSegmentedStream = archiveStream as ZipSegmentedStream;
				if (zipSegmentedStream != null)
				{
					zipSegmentedStream.Dispose();
					this._archiveStream = null;
				}
			}
			return result;
		}

		internal Stream GetExtractDecompressor(Stream input2)
		{
			short compressionMethod_FromZipFile = this._CompressionMethod_FromZipFile;
			if (compressionMethod_FromZipFile == 0)
			{
				return input2;
			}
			if (compressionMethod_FromZipFile != 8)
			{
				return null;
			}
			return new DeflateStream(input2, CompressionMode.Decompress, true);
		}

		internal Stream GetExtractDecryptor(Stream input)
		{
			Stream result;
			if (this._Encryption_FromZipFile == EncryptionAlgorithm.PkzipWeak)
			{
				result = new ZipCipherStream(input, this._zipCrypto_forExtract, CryptoMode.Decrypt);
			}
			else
			{
				result = input;
			}
			return result;
		}

		internal void _SetTimes(string fileOrDirectory, bool isFile)
		{
		}

		internal void ValidateEncryption()
		{
			if (this.Encryption == EncryptionAlgorithm.PkzipWeak || this.Encryption == EncryptionAlgorithm.None)
			{
				return;
			}
			if (this._UnsupportedAlgorithmId != 0u)
			{
				throw new ZipException(string.Format("Cannot extract: Entry {0} is encrypted with an algorithm not supported by DotNetZip: {1}", this.FileName, this.UnsupportedAlgorithm));
			}
			throw new ZipException(string.Format("Cannot extract: Entry {0} uses an unsupported encryption algorithm ({1:X2})", this.FileName, (int)this.Encryption));
		}

		private void ValidateCompression()
		{
			if (this._CompressionMethod_FromZipFile != 0 && this._CompressionMethod_FromZipFile != 8)
			{
				throw new ZipException(string.Format("Entry {0} uses an unsupported compression method (0x{1:X2}, {2})", this.FileName, this._CompressionMethod_FromZipFile, this.UnsupportedCompressionMethod));
			}
		}

		private void SetupCryptoForExtract(string password)
		{
			if (this._Encryption_FromZipFile == EncryptionAlgorithm.None)
			{
				return;
			}
			if (this._Encryption_FromZipFile == EncryptionAlgorithm.PkzipWeak)
			{
				if (password == null)
				{
					throw new ZipException("Missing password.");
				}
				this.ArchiveStream.Seek(this.FileDataPosition - 12L, SeekOrigin.Begin);
				this._zipCrypto_forExtract = ZipCrypto.ForRead(password, this);
			}
		}

		private bool ValidateOutput(string basedir, Stream outstream, out string outFileName)
		{
			if (basedir != null)
			{
				string text = this.FileName.Replace("\\", "/");
				if (text.IndexOf(':') == 1)
				{
					text = text.Substring(2);
				}
				if (text.StartsWith("/"))
				{
					text = text.Substring(1);
				}
				if (this._container.ZipFile.FlattenFoldersOnExtract)
				{
					outFileName = Path.Combine(basedir, (text.IndexOf('/') == -1) ? text : Path.GetFileName(text));
				}
				else
				{
					outFileName = Path.Combine(basedir, text);
				}
				outFileName = outFileName.Replace("/", "\\");
				if (this.IsDirectory || this.FileName.EndsWith("/"))
				{
					if (!Directory.Exists(outFileName))
					{
						Directory.CreateDirectory(outFileName);
						this._SetTimes(outFileName, false);
					}
					else if (this.ExtractExistingFile == ExtractExistingFileAction.OverwriteSilently)
					{
						this._SetTimes(outFileName, false);
					}
					return true;
				}
				return false;
			}
			else
			{
				if (outstream != null)
				{
					outFileName = null;
					return this.IsDirectory || this.FileName.EndsWith("/");
				}
				throw new ArgumentNullException("outstream");
			}
		}

		private void ReadExtraField()
		{
			this._readExtraDepth++;
			long position = this.ArchiveStream.Position;
			this.ArchiveStream.Seek(this._RelativeOffsetOfLocalHeader, SeekOrigin.Begin);
			byte[] array = new byte[30];
			this.ArchiveStream.Read(array, 0, array.Length);
			int num = 26;
			short num2 = (short)((int)array[num++] + (int)array[num++] * 256);
			short extraFieldLength = (short)((int)array[num++] + (int)array[num++] * 256);
			this.ArchiveStream.Seek((long)num2, SeekOrigin.Current);
			this.ProcessExtraField(this.ArchiveStream, extraFieldLength);
			this.ArchiveStream.Seek(position, SeekOrigin.Begin);
			this._readExtraDepth--;
		}

		private static bool ReadHeader(ZipEntry ze, Encoding defaultEncoding)
		{
			int num = 0;
			ze._RelativeOffsetOfLocalHeader = ze.ArchiveStream.Position;
			int num2 = SharedUtilities.ReadEntrySignature(ze.ArchiveStream);
			num += 4;
			if (ZipEntry.IsNotValidSig(num2))
			{
				ze.ArchiveStream.Seek(-4L, SeekOrigin.Current);
				if (ZipEntry.IsNotValidZipDirEntrySig(num2) && (long)num2 != 101010256L)
				{
					throw new BadReadException(string.Format("  Bad signature (0x{0:X8}) at position  0x{1:X8}", num2, ze.ArchiveStream.Position));
				}
				return false;
			}
			else
			{
				byte[] array = new byte[26];
				int num3 = ze.ArchiveStream.Read(array, 0, array.Length);
				if (num3 != array.Length)
				{
					return false;
				}
				num += num3;
				int num4 = 0;
				ze._VersionNeeded = (short)((int)array[num4++] + (int)array[num4++] * 256);
				ze._BitField = (short)((int)array[num4++] + (int)array[num4++] * 256);
				ze._CompressionMethod_FromZipFile = (ze._CompressionMethod = (short)((int)array[num4++] + (int)array[num4++] * 256));
				ze._TimeBlob = (int)array[num4++] + (int)array[num4++] * 256 + (int)array[num4++] * 256 * 256 + (int)array[num4++] * 256 * 256 * 256;
				ze._LastModified = SharedUtilities.PackedToDateTime(ze._TimeBlob);
				ze._timestamp |= ZipEntryTimestamp.DOS;
				if ((ze._BitField & 1) == 1)
				{
					ze._Encryption_FromZipFile = (ze._Encryption = EncryptionAlgorithm.PkzipWeak);
					ze._sourceIsEncrypted = true;
				}
				ze._Crc32 = (int)array[num4++] + (int)array[num4++] * 256 + (int)array[num4++] * 256 * 256 + (int)array[num4++] * 256 * 256 * 256;
				ze._CompressedSize = (long)((ulong)((int)array[num4++] + (int)array[num4++] * 256 + (int)array[num4++] * 256 * 256 + (int)array[num4++] * 256 * 256 * 256));
				ze._UncompressedSize = (long)((ulong)((int)array[num4++] + (int)array[num4++] * 256 + (int)array[num4++] * 256 * 256 + (int)array[num4++] * 256 * 256 * 256));
				if ((uint)ze._CompressedSize == 4294967295u || (uint)ze._UncompressedSize == 4294967295u)
				{
					ze._InputUsesZip64 = true;
				}
				short num5 = (short)((int)array[num4++] + (int)array[num4++] * 256);
				short extraFieldLength = (short)((int)array[num4++] + (int)array[num4++] * 256);
				array = new byte[(int)num5];
				num3 = ze.ArchiveStream.Read(array, 0, array.Length);
				num += num3;
				if ((ze._BitField & 2048) == 2048)
				{
					ze.AlternateEncoding = Encoding.UTF8;
					ze.AlternateEncodingUsage = ZipOption.Always;
				}
				ze._FileNameInArchive = ze.AlternateEncoding.GetString(array, 0, array.Length);
				if (ze._FileNameInArchive.EndsWith("/"))
				{
					ze.MarkAsDirectory();
				}
				num += ze.ProcessExtraField(ze.ArchiveStream, extraFieldLength);
				ze._LengthOfTrailer = 0;
				if (!ze._FileNameInArchive.EndsWith("/") && (ze._BitField & 8) == 8)
				{
					long position = ze.ArchiveStream.Position;
					bool flag = true;
					long num6 = 0L;
					int num7 = 0;
					while (flag)
					{
						num7++;
						if (ze._container.ZipFile != null)
						{
							ze._container.ZipFile.OnReadBytes(ze);
						}
						long num8 = SharedUtilities.FindSignature(ze.ArchiveStream, 134695760);
						if (num8 == -1L)
						{
							return false;
						}
						num6 += num8;
						if (ze._InputUsesZip64)
						{
							array = new byte[20];
							num3 = ze.ArchiveStream.Read(array, 0, array.Length);
							if (num3 != 20)
							{
								return false;
							}
							num4 = 0;
							ze._Crc32 = (int)array[num4++] + (int)array[num4++] * 256 + (int)array[num4++] * 256 * 256 + (int)array[num4++] * 256 * 256 * 256;
							ze._CompressedSize = BitConverter.ToInt64(array, num4);
							num4 += 8;
							ze._UncompressedSize = BitConverter.ToInt64(array, num4);
							num4 += 8;
							ze._LengthOfTrailer += 24;
						}
						else
						{
							array = new byte[12];
							num3 = ze.ArchiveStream.Read(array, 0, array.Length);
							if (num3 != 12)
							{
								return false;
							}
							num4 = 0;
							ze._Crc32 = (int)array[num4++] + (int)array[num4++] * 256 + (int)array[num4++] * 256 * 256 + (int)array[num4++] * 256 * 256 * 256;
							ze._CompressedSize = (long)((ulong)((int)array[num4++] + (int)array[num4++] * 256 + (int)array[num4++] * 256 * 256 + (int)array[num4++] * 256 * 256 * 256));
							ze._UncompressedSize = (long)((ulong)((int)array[num4++] + (int)array[num4++] * 256 + (int)array[num4++] * 256 * 256 + (int)array[num4++] * 256 * 256 * 256));
							ze._LengthOfTrailer += 16;
						}
						flag = (num6 != ze._CompressedSize);
						if (flag)
						{
							ze.ArchiveStream.Seek(-12L, SeekOrigin.Current);
							num6 += 4L;
						}
					}
					ze.ArchiveStream.Seek(position, SeekOrigin.Begin);
				}
				ze._CompressedFileDataSize = ze._CompressedSize;
				if ((ze._BitField & 1) == 1)
				{
					ze._WeakEncryptionHeader = new byte[12];
					num += ZipEntry.ReadWeakEncryptionHeader(ze._archiveStream, ze._WeakEncryptionHeader);
					ze._CompressedFileDataSize -= 12L;
				}
				ze._LengthOfHeader = num;
				ze._TotalEntrySize = (long)ze._LengthOfHeader + ze._CompressedFileDataSize + (long)ze._LengthOfTrailer;
				return true;
			}
		}

		internal static int ReadWeakEncryptionHeader(Stream s, byte[] buffer)
		{
			int num = s.Read(buffer, 0, 12);
			if (num != 12)
			{
				throw new ZipException(string.Format("Unexpected end of data at position 0x{0:X8}", s.Position));
			}
			return num;
		}

		private static bool IsNotValidSig(int signature)
		{
			return signature != 67324752;
		}

		internal static ZipEntry ReadEntry(ZipContainer zc, bool first)
		{
			ZipFile zipFile = zc.ZipFile;
			Stream readStream = zc.ReadStream;
			Encoding alternateEncoding = zc.AlternateEncoding;
			ZipEntry zipEntry = new ZipEntry();
			zipEntry._Source = ZipEntrySource.ZipFile;
			zipEntry._container = zc;
			zipEntry._archiveStream = readStream;
			if (zipFile != null)
			{
				zipFile.OnReadEntry(true, null);
			}
			if (first)
			{
				ZipEntry.HandlePK00Prefix(readStream);
			}
			if (!ZipEntry.ReadHeader(zipEntry, alternateEncoding))
			{
				return null;
			}
			zipEntry.__FileDataPosition = zipEntry.ArchiveStream.Position;
			readStream.Seek(zipEntry._CompressedFileDataSize + (long)zipEntry._LengthOfTrailer, SeekOrigin.Current);
			ZipEntry.HandleUnexpectedDataDescriptor(zipEntry);
			if (zipFile != null)
			{
				zipFile.OnReadBytes(zipEntry);
				zipFile.OnReadEntry(false, zipEntry);
			}
			return zipEntry;
		}

		internal static void HandlePK00Prefix(Stream s)
		{
			uint num = (uint)SharedUtilities.ReadInt(s);
			if (num != 808471376u)
			{
				s.Seek(-4L, SeekOrigin.Current);
			}
		}

		private static void HandleUnexpectedDataDescriptor(ZipEntry entry)
		{
			Stream archiveStream = entry.ArchiveStream;
			uint num = (uint)SharedUtilities.ReadInt(archiveStream);
			if ((ulong)num == (ulong)((long)entry._Crc32))
			{
				int num2 = SharedUtilities.ReadInt(archiveStream);
				if ((long)num2 == entry._CompressedSize)
				{
					num2 = SharedUtilities.ReadInt(archiveStream);
					if ((long)num2 != entry._UncompressedSize)
					{
						archiveStream.Seek(-12L, SeekOrigin.Current);
					}
				}
				else
				{
					archiveStream.Seek(-8L, SeekOrigin.Current);
				}
			}
			else
			{
				archiveStream.Seek(-4L, SeekOrigin.Current);
			}
		}

		internal static int FindExtraFieldSegment(byte[] extra, int offx, ushort targetHeaderId)
		{
			int num = offx;
			while (num + 3 < extra.Length)
			{
				ushort num2 = (ushort)((int)extra[num++] + (int)extra[num++] * 256);
				if (num2 == targetHeaderId)
				{
					return num - 2;
				}
				short num3 = (short)((int)extra[num++] + (int)extra[num++] * 256);
				num += (int)num3;
			}
			return -1;
		}

		internal int ProcessExtraField(Stream s, short extraFieldLength)
		{
			int num = 0;
			if (extraFieldLength > 0)
			{
				byte[] array = this._Extra = new byte[(int)extraFieldLength];
				num = s.Read(array, 0, array.Length);
				long posn = s.Position - (long)num;
				int num2 = 0;
				while (num2 + 3 < array.Length)
				{
					int num3 = num2;
					ushort num4 = (ushort)((int)array[num2++] + (int)array[num2++] * 256);
					short num5 = (short)((int)array[num2++] + (int)array[num2++] * 256);
					ushort num6 = num4;
					if (num6 != 1)
					{
						if (num6 != 10)
						{
							if (num6 != 23)
							{
								if (num6 != 21589)
								{
									if (num6 != 22613)
									{
										if (num6 != 30805)
										{
											if (num6 != 30837)
											{
											}
										}
									}
									else
									{
										num2 = this.ProcessExtraFieldInfoZipTimes(array, num2, num5, posn);
									}
								}
								else
								{
									num2 = this.ProcessExtraFieldUnixTimes(array, num2, num5, posn);
								}
							}
							else
							{
								num2 = this.ProcessExtraFieldPkwareStrongEncryption(array, num2);
							}
						}
						else
						{
							num2 = this.ProcessExtraFieldWindowsTimes(array, num2, num5, posn);
						}
					}
					else
					{
						num2 = this.ProcessExtraFieldZip64(array, num2, num5, posn);
					}
					num2 = num3 + (int)num5 + 4;
				}
			}
			return num;
		}

		private int ProcessExtraFieldPkwareStrongEncryption(byte[] Buffer, int j)
		{
			j += 2;
			this._UnsupportedAlgorithmId = (uint)((ushort)((int)Buffer[j++] + (int)Buffer[j++] * 256));
			this._Encryption_FromZipFile = (this._Encryption = EncryptionAlgorithm.Unsupported);
			return j;
		}

		private int ProcessExtraFieldZip64(byte[] buffer, int j, short dataSize, long posn)
		{
			this._InputUsesZip64 = true;
			if (dataSize > 28)
			{
				throw new BadReadException(string.Format("  Inconsistent size (0x{0:X4}) for ZIP64 extra field at position 0x{1:X16}", dataSize, posn));
			}
			return j;
		}

		private int ProcessExtraFieldInfoZipTimes(byte[] buffer, int j, short dataSize, long posn)
		{
			if (dataSize != 12 && dataSize != 8)
			{
				throw new BadReadException(string.Format("  Unexpected size (0x{0:X4}) for InfoZip v1 extra field at position 0x{1:X16}", dataSize, posn));
			}
			int num = BitConverter.ToInt32(buffer, j);
			this._Mtime = ZipEntry._unixEpoch.AddSeconds((double)num);
			j += 4;
			num = BitConverter.ToInt32(buffer, j);
			this._Atime = ZipEntry._unixEpoch.AddSeconds((double)num);
			j += 4;
			this._Ctime = DateTime.UtcNow;
			this._ntfsTimesAreSet = true;
			this._timestamp |= ZipEntryTimestamp.InfoZip1;
			return j;
		}

		private int ProcessExtraFieldUnixTimes(byte[] buffer, int j, short dataSize, long posn)
		{
			if (dataSize != 13 && dataSize != 9 && dataSize != 5)
			{
				throw new BadReadException(string.Format("  Unexpected size (0x{0:X4}) for Extended Timestamp extra field at position 0x{1:X16}", dataSize, posn));
			}
			return j;
		}

		private int ProcessExtraFieldWindowsTimes(byte[] buffer, int j, short dataSize, long posn)
		{
			if (dataSize != 32)
			{
				throw new BadReadException(string.Format("  Unexpected size (0x{0:X4}) for NTFS times extra field at position 0x{1:X16}", dataSize, posn));
			}
			j += 4;
			short num = (short)((int)buffer[j] + (int)buffer[j + 1] * 256);
			short num2 = (short)((int)buffer[j + 2] + (int)buffer[j + 3] * 256);
			j += 4;
			if (num == 1 && num2 == 24)
			{
				long fileTime = BitConverter.ToInt64(buffer, j);
				this._Mtime = DateTime.FromFileTimeUtc(fileTime);
				j += 8;
				fileTime = BitConverter.ToInt64(buffer, j);
				this._Atime = DateTime.FromFileTimeUtc(fileTime);
				j += 8;
				fileTime = BitConverter.ToInt64(buffer, j);
				this._Ctime = DateTime.FromFileTimeUtc(fileTime);
				j += 8;
				this._ntfsTimesAreSet = true;
				this._timestamp |= ZipEntryTimestamp.Windows;
				this._emitNtfsTimes = true;
			}
			return j;
		}

		internal void WriteCentralDirectoryEntry(Stream s)
		{
			byte[] array = new byte[4096];
			int num = 0;
			array[num++] = 80;
			array[num++] = 75;
			array[num++] = 1;
			array[num++] = 2;
			array[num++] = (byte)(this._VersionMadeBy & 255);
			array[num++] = (byte)(((int)this._VersionMadeBy & 65280) >> 8);
			short num2 = (this.VersionNeeded == 0) ? 20 : this.VersionNeeded;
			bool? outputUsesZip = this._OutputUsesZip64;
			if (!outputUsesZip.HasValue)
			{
				this._OutputUsesZip64 = new bool?(this._container.Zip64 == Zip64Option.Always);
			}
			short num3 = (!this._OutputUsesZip64.Value) ? num2 : 45;
			array[num++] = (byte)(num3 & 255);
			array[num++] = (byte)(((int)num3 & 65280) >> 8);
			array[num++] = (byte)(this._BitField & 255);
			array[num++] = (byte)(((int)this._BitField & 65280) >> 8);
			array[num++] = (byte)(this._CompressionMethod & 255);
			array[num++] = (byte)(((int)this._CompressionMethod & 65280) >> 8);
			array[num++] = (byte)(this._TimeBlob & 255);
			array[num++] = (byte)((this._TimeBlob & 65280) >> 8);
			array[num++] = (byte)((this._TimeBlob & 16711680) >> 16);
			array[num++] = (byte)(((long)this._TimeBlob & (long)((ulong)-16777216)) >> 24);
			array[num++] = (byte)(this._Crc32 & 255);
			array[num++] = (byte)((this._Crc32 & 65280) >> 8);
			array[num++] = (byte)((this._Crc32 & 16711680) >> 16);
			array[num++] = (byte)(((long)this._Crc32 & (long)((ulong)-16777216)) >> 24);
			if (this._OutputUsesZip64.Value)
			{
				for (int i = 0; i < 8; i++)
				{
					array[num++] = 255;
				}
			}
			else
			{
				array[num++] = (byte)(this._CompressedSize & 255L);
				array[num++] = (byte)((this._CompressedSize & 65280L) >> 8);
				array[num++] = (byte)((this._CompressedSize & 16711680L) >> 16);
				array[num++] = (byte)((this._CompressedSize & (long)((ulong)-16777216)) >> 24);
				array[num++] = (byte)(this._UncompressedSize & 255L);
				array[num++] = (byte)((this._UncompressedSize & 65280L) >> 8);
				array[num++] = (byte)((this._UncompressedSize & 16711680L) >> 16);
				array[num++] = (byte)((this._UncompressedSize & (long)((ulong)-16777216)) >> 24);
			}
			byte[] encodedFileNameBytes = this.GetEncodedFileNameBytes();
			short num4 = (short)encodedFileNameBytes.Length;
			array[num++] = (byte)(num4 & 255);
			array[num++] = (byte)(((int)num4 & 65280) >> 8);
			this._presumeZip64 = this._OutputUsesZip64.Value;
			this._Extra = this.ConstructExtraField(true);
			short num5 = (short)((this._Extra != null) ? this._Extra.Length : 0);
			array[num++] = (byte)(num5 & 255);
			array[num++] = (byte)(((int)num5 & 65280) >> 8);
			int num6 = (this._CommentBytes != null) ? this._CommentBytes.Length : 0;
			if (num6 + num > array.Length)
			{
				num6 = array.Length - num;
			}
			array[num++] = (byte)(num6 & 255);
			array[num++] = (byte)((num6 & 65280) >> 8);
			bool flag = this._container.ZipFile != null && this._container.ZipFile.MaxOutputSegmentSize != 0;
			if (flag)
			{
				array[num++] = (byte)(this._diskNumber & 255u);
				array[num++] = (byte)((this._diskNumber & 65280u) >> 8);
			}
			else
			{
				array[num++] = 0;
				array[num++] = 0;
			}
			array[num++] = ((!this._IsText) ? 0 : 1);
			array[num++] = 0;
			array[num++] = (byte)(this._ExternalFileAttrs & 255);
			array[num++] = (byte)((this._ExternalFileAttrs & 65280) >> 8);
			array[num++] = (byte)((this._ExternalFileAttrs & 16711680) >> 16);
			array[num++] = (byte)(((long)this._ExternalFileAttrs & (long)((ulong)-16777216)) >> 24);
			if (this._RelativeOffsetOfLocalHeader > (long)((ulong)-1))
			{
				array[num++] = 255;
				array[num++] = 255;
				array[num++] = 255;
				array[num++] = 255;
			}
			else
			{
				array[num++] = (byte)(this._RelativeOffsetOfLocalHeader & 255L);
				array[num++] = (byte)((this._RelativeOffsetOfLocalHeader & 65280L) >> 8);
				array[num++] = (byte)((this._RelativeOffsetOfLocalHeader & 16711680L) >> 16);
				array[num++] = (byte)((this._RelativeOffsetOfLocalHeader & (long)((ulong)-16777216)) >> 24);
			}
			Buffer.BlockCopy(encodedFileNameBytes, 0, array, num, (int)num4);
			num += (int)num4;
			if (this._Extra != null)
			{
				byte[] extra = this._Extra;
				int srcOffset = 0;
				Buffer.BlockCopy(extra, srcOffset, array, num, (int)num5);
				num += (int)num5;
			}
			if (num6 != 0)
			{
				Buffer.BlockCopy(this._CommentBytes, 0, array, num, num6);
				num += num6;
			}
			s.Write(array, 0, num);
		}

		private byte[] ConstructExtraField(bool forCentralDirectory)
		{
			List<byte[]> list = new List<byte[]>();
			if (this._container.Zip64 == Zip64Option.Always || (this._container.Zip64 == Zip64Option.AsNecessary && (!forCentralDirectory || this._entryRequiresZip64.Value)))
			{
				int num = 4 + ((!forCentralDirectory) ? 16 : 28);
				byte[] array = new byte[num];
				int num2 = 0;
				if (this._presumeZip64 || forCentralDirectory)
				{
					array[num2++] = 1;
					array[num2++] = 0;
				}
				else
				{
					array[num2++] = 153;
					array[num2++] = 153;
				}
				array[num2++] = (byte)(num - 4);
				array[num2++] = 0;
				Array.Copy(BitConverter.GetBytes(this._UncompressedSize), 0, array, num2, 8);
				num2 += 8;
				Array.Copy(BitConverter.GetBytes(this._CompressedSize), 0, array, num2, 8);
				num2 += 8;
				if (forCentralDirectory)
				{
					Array.Copy(BitConverter.GetBytes(this._RelativeOffsetOfLocalHeader), 0, array, num2, 8);
					num2 += 8;
					Array.Copy(BitConverter.GetBytes(0), 0, array, num2, 4);
				}
				list.Add(array);
			}
			if (this._ntfsTimesAreSet && this._emitNtfsTimes)
			{
				byte[] array = new byte[36];
				int num3 = 0;
				array[num3++] = 10;
				array[num3++] = 0;
				array[num3++] = 32;
				array[num3++] = 0;
				num3 += 4;
				array[num3++] = 1;
				array[num3++] = 0;
				array[num3++] = 24;
				array[num3++] = 0;
				long value = this._Mtime.ToFileTime();
				Array.Copy(BitConverter.GetBytes(value), 0, array, num3, 8);
				num3 += 8;
				value = this._Atime.ToFileTime();
				Array.Copy(BitConverter.GetBytes(value), 0, array, num3, 8);
				num3 += 8;
				value = this._Ctime.ToFileTime();
				Array.Copy(BitConverter.GetBytes(value), 0, array, num3, 8);
				num3 += 8;
				list.Add(array);
			}
			if (this._ntfsTimesAreSet && this._emitUnixTimes)
			{
				int num4 = 9;
				if (!forCentralDirectory)
				{
					num4 += 8;
				}
				byte[] array = new byte[num4];
				int num5 = 0;
				array[num5++] = 85;
				array[num5++] = 84;
				array[num5++] = (byte)(num4 - 4);
				array[num5++] = 0;
				array[num5++] = 7;
				int value2 = (int)(this._Mtime - ZipEntry._unixEpoch).TotalSeconds;
				Array.Copy(BitConverter.GetBytes(value2), 0, array, num5, 4);
				num5 += 4;
				if (!forCentralDirectory)
				{
					value2 = (int)(this._Atime - ZipEntry._unixEpoch).TotalSeconds;
					Array.Copy(BitConverter.GetBytes(value2), 0, array, num5, 4);
					num5 += 4;
					value2 = (int)(this._Ctime - ZipEntry._unixEpoch).TotalSeconds;
					Array.Copy(BitConverter.GetBytes(value2), 0, array, num5, 4);
					num5 += 4;
				}
				list.Add(array);
			}
			byte[] array2 = null;
			if (list.Count > 0)
			{
				int num6 = 0;
				int num7 = 0;
				for (int i = 0; i < list.Count; i++)
				{
					num6 += list[i].Length;
				}
				array2 = new byte[num6];
				for (int i = 0; i < list.Count; i++)
				{
					Array.Copy(list[i], 0, array2, num7, list[i].Length);
					num7 += list[i].Length;
				}
			}
			return array2;
		}

		private string NormalizeFileName()
		{
			string text = this.FileName.Replace("\\", "/");
			string result;
			if (this._TrimVolumeFromFullyQualifiedPaths && this.FileName.Length >= 3 && this.FileName[1] == ':' && text[2] == '/')
			{
				result = text.Substring(3);
			}
			else if (this.FileName.Length >= 4 && text[0] == '/' && text[1] == '/')
			{
				int num = text.IndexOf('/', 2);
				if (num == -1)
				{
					throw new ArgumentException("The path for that entry appears to be badly formatted");
				}
				result = text.Substring(num + 1);
			}
			else if (this.FileName.Length >= 3 && text[0] == '.' && text[1] == '/')
			{
				result = text.Substring(2);
			}
			else
			{
				result = text;
			}
			return result;
		}

		private byte[] GetEncodedFileNameBytes()
		{
			string text = this.NormalizeFileName();
			switch (this.AlternateEncodingUsage)
			{
			case ZipOption.Default:
				if (this._Comment != null && this._Comment.Length != 0)
				{
					this._CommentBytes = ZipEntry.ibm437.GetBytes(this._Comment);
				}
				this._actualEncoding = ZipEntry.ibm437;
				return ZipEntry.ibm437.GetBytes(text);
			case ZipOption.Always:
				if (this._Comment != null && this._Comment.Length != 0)
				{
					this._CommentBytes = this.AlternateEncoding.GetBytes(this._Comment);
				}
				this._actualEncoding = this.AlternateEncoding;
				return this.AlternateEncoding.GetBytes(text);
			}
			byte[] bytes = ZipEntry.ibm437.GetBytes(text);
			string @string = ZipEntry.ibm437.GetString(bytes, 0, bytes.Length);
			this._CommentBytes = null;
			if (@string != text)
			{
				bytes = this.AlternateEncoding.GetBytes(text);
				if (this._Comment != null && this._Comment.Length != 0)
				{
					this._CommentBytes = this.AlternateEncoding.GetBytes(this._Comment);
				}
				this._actualEncoding = this.AlternateEncoding;
				return bytes;
			}
			this._actualEncoding = ZipEntry.ibm437;
			if (this._Comment == null || this._Comment.Length == 0)
			{
				return bytes;
			}
			byte[] bytes2 = ZipEntry.ibm437.GetBytes(this._Comment);
			string string2 = ZipEntry.ibm437.GetString(bytes2, 0, bytes2.Length);
			if (string2 != this.Comment)
			{
				bytes = this.AlternateEncoding.GetBytes(text);
				this._CommentBytes = this.AlternateEncoding.GetBytes(this._Comment);
				this._actualEncoding = this.AlternateEncoding;
				return bytes;
			}
			this._CommentBytes = bytes2;
			return bytes;
		}

		private bool WantReadAgain()
		{
			return this._UncompressedSize >= 16L && this._CompressionMethod != 0 && this.CompressionLevel != CompressionLevel.None && this._CompressedSize >= this._UncompressedSize && (this._Source != ZipEntrySource.Stream || this._sourceStream.CanSeek) && (this._zipCrypto_forWrite == null || this.CompressedSize - 12L > this.UncompressedSize);
		}

		private void MaybeUnsetCompressionMethodForWriting(int cycle)
		{
			if (cycle > 1)
			{
				this._CompressionMethod = 0;
				return;
			}
			if (this.IsDirectory)
			{
				this._CompressionMethod = 0;
				return;
			}
			if (this._Source == ZipEntrySource.ZipFile)
			{
				return;
			}
			if (this._Source == ZipEntrySource.Stream)
			{
				if (this._sourceStream != null && this._sourceStream.CanSeek)
				{
					long length = this._sourceStream.Length;
					if (length == 0L)
					{
						this._CompressionMethod = 0;
						return;
					}
				}
			}
			else if (this._Source == ZipEntrySource.FileSystem && SharedUtilities.GetFileLength(this.LocalFileName) == 0L)
			{
				this._CompressionMethod = 0;
				return;
			}
			if (this.SetCompression != null)
			{
				this.CompressionLevel = this.SetCompression(this.LocalFileName, this._FileNameInArchive);
			}
			if (this.CompressionLevel == CompressionLevel.None && this.CompressionMethod == CompressionMethod.Deflate)
			{
				this._CompressionMethod = 0;
			}
		}

		internal void WriteHeader(Stream s, int cycle)
		{
			CountingStream countingStream = s as CountingStream;
			this._future_ROLH = ((countingStream == null) ? s.Position : countingStream.ComputedPosition);
			int num = 0;
			byte[] array = new byte[30];
			array[num++] = 80;
			array[num++] = 75;
			array[num++] = 3;
			array[num++] = 4;
			this._presumeZip64 = (this._container.Zip64 == Zip64Option.Always || (this._container.Zip64 == Zip64Option.AsNecessary && !s.CanSeek));
			short num2 = (!this._presumeZip64) ? 20 : 45;
			array[num++] = (byte)(num2 & 255);
			array[num++] = (byte)(((int)num2 & 65280) >> 8);
			byte[] encodedFileNameBytes = this.GetEncodedFileNameBytes();
			short num3 = (short)encodedFileNameBytes.Length;
			if (this._Encryption == EncryptionAlgorithm.None)
			{
				this._BitField &= -2;
			}
			else
			{
				this._BitField |= 1;
			}
			if (this._actualEncoding.WebName == "utf-8")
			{
				this._BitField |= 2048;
			}
			if (this.IsDirectory || cycle == 99)
			{
				this._BitField &= -9;
				this._BitField &= -2;
				this.Encryption = EncryptionAlgorithm.None;
				this.Password = null;
			}
			else if (!s.CanSeek)
			{
				this._BitField |= 8;
			}
			array[num++] = (byte)(this._BitField & 255);
			array[num++] = (byte)(((int)this._BitField & 65280) >> 8);
			if (this.__FileDataPosition == -1L)
			{
				this._CompressedSize = 0L;
				this._crcCalculated = false;
			}
			this.MaybeUnsetCompressionMethodForWriting(cycle);
			array[num++] = (byte)(this._CompressionMethod & 255);
			array[num++] = (byte)(((int)this._CompressionMethod & 65280) >> 8);
			if (cycle == 99)
			{
				this.SetZip64Flags();
			}
			this._TimeBlob = SharedUtilities.DateTimeToPacked(this.LastModified);
			array[num++] = (byte)(this._TimeBlob & 255);
			array[num++] = (byte)((this._TimeBlob & 65280) >> 8);
			array[num++] = (byte)((this._TimeBlob & 16711680) >> 16);
			array[num++] = (byte)(((long)this._TimeBlob & (long)((ulong)-16777216)) >> 24);
			array[num++] = (byte)(this._Crc32 & 255);
			array[num++] = (byte)((this._Crc32 & 65280) >> 8);
			array[num++] = (byte)((this._Crc32 & 16711680) >> 16);
			array[num++] = (byte)(((long)this._Crc32 & (long)((ulong)-16777216)) >> 24);
			if (this._presumeZip64)
			{
				for (int i = 0; i < 8; i++)
				{
					array[num++] = 255;
				}
			}
			else
			{
				array[num++] = (byte)(this._CompressedSize & 255L);
				array[num++] = (byte)((this._CompressedSize & 65280L) >> 8);
				array[num++] = (byte)((this._CompressedSize & 16711680L) >> 16);
				array[num++] = (byte)((this._CompressedSize & (long)((ulong)-16777216)) >> 24);
				array[num++] = (byte)(this._UncompressedSize & 255L);
				array[num++] = (byte)((this._UncompressedSize & 65280L) >> 8);
				array[num++] = (byte)((this._UncompressedSize & 16711680L) >> 16);
				array[num++] = (byte)((this._UncompressedSize & (long)((ulong)-16777216)) >> 24);
			}
			array[num++] = (byte)(num3 & 255);
			array[num++] = (byte)(((int)num3 & 65280) >> 8);
			this._Extra = this.ConstructExtraField(false);
			short num4 = (short)((this._Extra != null) ? this._Extra.Length : 0);
			array[num++] = (byte)(num4 & 255);
			array[num++] = (byte)(((int)num4 & 65280) >> 8);
			byte[] array2 = new byte[num + (int)num3 + (int)num4];
			Buffer.BlockCopy(array, 0, array2, 0, num);
			Buffer.BlockCopy(encodedFileNameBytes, 0, array2, num, encodedFileNameBytes.Length);
			num += encodedFileNameBytes.Length;
			if (this._Extra != null)
			{
				Buffer.BlockCopy(this._Extra, 0, array2, num, this._Extra.Length);
				num += this._Extra.Length;
			}
			this._LengthOfHeader = num;
			ZipSegmentedStream zipSegmentedStream = s as ZipSegmentedStream;
			if (zipSegmentedStream != null)
			{
				zipSegmentedStream.ContiguousWrite = true;
				uint num5 = zipSegmentedStream.ComputeSegment(num);
				if (num5 != zipSegmentedStream.CurrentSegment)
				{
					this._future_ROLH = 0L;
				}
				else
				{
					this._future_ROLH = zipSegmentedStream.Position;
				}
				this._diskNumber = num5;
			}
			if (this._container.Zip64 == Zip64Option.Default && (uint)this._RelativeOffsetOfLocalHeader >= 4294967295u)
			{
				throw new ZipException("Offset within the zip archive exceeds 0xFFFFFFFF. Consider setting the UseZip64WhenSaving property on the ZipFile instance.");
			}
			s.Write(array2, 0, num);
			if (zipSegmentedStream != null)
			{
				zipSegmentedStream.ContiguousWrite = false;
			}
			this._EntryHeader = array2;
		}

		private int FigureCrc32()
		{
			if (!this._crcCalculated)
			{
				Stream stream = null;
				if (this._Source == ZipEntrySource.WriteDelegate)
				{
					CrcCalculatorStream crcCalculatorStream = new CrcCalculatorStream(Stream.Null);
					this._WriteDelegate(this.FileName, crcCalculatorStream);
					this._Crc32 = crcCalculatorStream.Crc;
				}
				else if (this._Source != ZipEntrySource.ZipFile)
				{
					if (this._Source == ZipEntrySource.Stream)
					{
						this.PrepSourceStream();
						stream = this._sourceStream;
					}
					else if (this._Source == ZipEntrySource.JitStream)
					{
						if (this._sourceStream == null)
						{
							this._sourceStream = this._OpenDelegate(this.FileName);
						}
						this.PrepSourceStream();
						stream = this._sourceStream;
					}
					else if (this._Source != ZipEntrySource.ZipOutputStream)
					{
						stream = File.Open(this.LocalFileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
					}
					CRC32 cRC = new CRC32();
					this._Crc32 = cRC.GetCrc32(stream);
					if (this._sourceStream == null)
					{
						stream.Dispose();
					}
				}
				this._crcCalculated = true;
			}
			return this._Crc32;
		}

		private void PrepSourceStream()
		{
			if (this._sourceStream == null)
			{
				throw new ZipException(string.Format("The input stream is null for entry '{0}'.", this.FileName));
			}
			long? sourceStreamOriginalPosition = this._sourceStreamOriginalPosition;
			if (sourceStreamOriginalPosition.HasValue)
			{
				this._sourceStream.Position = this._sourceStreamOriginalPosition.Value;
			}
			else if (this._sourceStream.CanSeek)
			{
				this._sourceStreamOriginalPosition = new long?(this._sourceStream.Position);
			}
			else if (this.Encryption == EncryptionAlgorithm.PkzipWeak && this._Source != ZipEntrySource.ZipFile && (this._BitField & 8) != 8)
			{
				throw new ZipException("It is not possible to use PKZIP encryption on a non-seekable input stream");
			}
		}

		internal void CopyMetaData(ZipEntry source)
		{
			this.__FileDataPosition = source.__FileDataPosition;
			this.CompressionMethod = source.CompressionMethod;
			this._CompressionMethod_FromZipFile = source._CompressionMethod_FromZipFile;
			this._CompressedFileDataSize = source._CompressedFileDataSize;
			this._UncompressedSize = source._UncompressedSize;
			this._BitField = source._BitField;
			this._Source = source._Source;
			this._LastModified = source._LastModified;
			this._Mtime = source._Mtime;
			this._Atime = source._Atime;
			this._Ctime = source._Ctime;
			this._ntfsTimesAreSet = source._ntfsTimesAreSet;
			this._emitUnixTimes = source._emitUnixTimes;
			this._emitNtfsTimes = source._emitNtfsTimes;
		}

		private void OnWriteBlock(long bytesXferred, long totalBytesToXfer)
		{
			if (this._container.ZipFile != null)
			{
				this._ioOperationCanceled = this._container.ZipFile.OnSaveBlock(this, bytesXferred, totalBytesToXfer);
			}
		}

		private void _WriteEntryData(Stream s)
		{
			Stream stream = null;
			long _FileDataPosition = -1L;
			try
			{
				_FileDataPosition = s.Position;
			}
			catch (Exception)
			{
			}
			try
			{
				long num = this.SetInputAndFigureFileLength(ref stream);
				CountingStream countingStream = new CountingStream(s);
				Stream stream2;
				Stream stream3;
				if (num != 0L)
				{
					stream2 = this.MaybeApplyEncryption(countingStream);
					stream3 = this.MaybeApplyCompression(stream2, num);
				}
				else
				{
					stream3 = (stream2 = countingStream);
				}
				CrcCalculatorStream crcCalculatorStream = new CrcCalculatorStream(stream3, true);
				if (this._Source == ZipEntrySource.WriteDelegate)
				{
					this._WriteDelegate(this.FileName, crcCalculatorStream);
				}
				else
				{
					byte[] array = new byte[this.BufferSize];
					int count;
					while ((count = SharedUtilities.ReadWithRetry(stream, array, 0, array.Length, this.FileName)) != 0)
					{
						crcCalculatorStream.Write(array, 0, count);
						this.OnWriteBlock(crcCalculatorStream.TotalBytesSlurped, num);
						if (this._ioOperationCanceled)
						{
							break;
						}
					}
				}
				this.FinishOutputStream(s, countingStream, stream2, stream3, crcCalculatorStream);
			}
			finally
			{
				if (this._Source == ZipEntrySource.JitStream)
				{
					if (this._CloseDelegate != null)
					{
						this._CloseDelegate(this.FileName, stream);
					}
				}
				else if (stream is FileStream)
				{
					stream.Dispose();
				}
			}
			if (this._ioOperationCanceled)
			{
				return;
			}
			this.__FileDataPosition = _FileDataPosition;
			this.PostProcessOutput(s);
		}

		private long SetInputAndFigureFileLength(ref Stream input)
		{
			long result = -1L;
			if (this._Source == ZipEntrySource.Stream)
			{
				this.PrepSourceStream();
				input = this._sourceStream;
				try
				{
					result = this._sourceStream.Length;
				}
				catch (NotSupportedException)
				{
				}
			}
			else if (this._Source == ZipEntrySource.ZipFile)
			{
				string password = (this._Encryption_FromZipFile != EncryptionAlgorithm.None) ? (this._Password ?? this._container.Password) : null;
				this._sourceStream = this.InternalOpenReader(password);
				this.PrepSourceStream();
				input = this._sourceStream;
				result = this._sourceStream.Length;
			}
			else if (this._Source == ZipEntrySource.JitStream)
			{
				if (this._sourceStream == null)
				{
					this._sourceStream = this._OpenDelegate(this.FileName);
				}
				this.PrepSourceStream();
				input = this._sourceStream;
				try
				{
					result = this._sourceStream.Length;
				}
				catch (NotSupportedException)
				{
				}
			}
			else if (this._Source == ZipEntrySource.FileSystem)
			{
				FileShare fileShare = FileShare.ReadWrite;
				fileShare |= FileShare.Delete;
				input = File.Open(this.LocalFileName, FileMode.Open, FileAccess.Read, fileShare);
				result = input.Length;
			}
			return result;
		}

		internal void FinishOutputStream(Stream s, CountingStream entryCounter, Stream encryptor, Stream compressor, CrcCalculatorStream output)
		{
			if (output == null)
			{
				return;
			}
			output.Close();
			if (compressor is DeflateStream)
			{
				compressor.Close();
			}
			else if (compressor is ParallelDeflateOutputStream)
			{
				compressor.Close();
			}
			encryptor.Flush();
			encryptor.Close();
			this._LengthOfTrailer = 0;
			this._UncompressedSize = output.TotalBytesSlurped;
			this._CompressedFileDataSize = entryCounter.BytesWritten;
			this._CompressedSize = this._CompressedFileDataSize;
			this._Crc32 = output.Crc;
			this.StoreRelativeOffset();
		}

		internal void PostProcessOutput(Stream s)
		{
			CountingStream countingStream = s as CountingStream;
			if (this._UncompressedSize == 0L && this._CompressedSize == 0L)
			{
				if (this._Source == ZipEntrySource.ZipOutputStream)
				{
					return;
				}
				if (this._Password != null)
				{
					int num = 0;
					if (this.Encryption == EncryptionAlgorithm.PkzipWeak)
					{
						num = 12;
					}
					if (this._Source == ZipEntrySource.ZipOutputStream && !s.CanSeek)
					{
						throw new ZipException("Zero bytes written, encryption in use, and non-seekable output.");
					}
					if (this.Encryption != EncryptionAlgorithm.None)
					{
						s.Seek((long)(-1 * num), SeekOrigin.Current);
						s.SetLength(s.Position);
						if (countingStream != null)
						{
							countingStream.Adjust((long)num);
						}
						this._LengthOfHeader -= num;
						this.__FileDataPosition -= (long)num;
					}
					this._Password = null;
					this._BitField &= -2;
					int num2 = 6;
					this._EntryHeader[num2++] = (byte)(this._BitField & 255);
					this._EntryHeader[num2++] = (byte)(((int)this._BitField & 65280) >> 8);
				}
				this.CompressionMethod = CompressionMethod.None;
				this.Encryption = EncryptionAlgorithm.None;
			}
			else if (this._zipCrypto_forWrite != null && this.Encryption == EncryptionAlgorithm.PkzipWeak)
			{
				this._CompressedSize += 12L;
			}
			int num3 = 8;
			this._EntryHeader[num3++] = (byte)(this._CompressionMethod & 255);
			this._EntryHeader[num3++] = (byte)(((int)this._CompressionMethod & 65280) >> 8);
			num3 = 14;
			this._EntryHeader[num3++] = (byte)(this._Crc32 & 255);
			this._EntryHeader[num3++] = (byte)((this._Crc32 & 65280) >> 8);
			this._EntryHeader[num3++] = (byte)((this._Crc32 & 16711680) >> 16);
			this._EntryHeader[num3++] = (byte)(((long)this._Crc32 & (long)((ulong)-16777216)) >> 24);
			this.SetZip64Flags();
			short num4 = (short)((int)this._EntryHeader[26] + (int)this._EntryHeader[27] * 256);
			short num5 = (short)((int)this._EntryHeader[28] + (int)this._EntryHeader[29] * 256);
			if (this._OutputUsesZip64.Value)
			{
				this._EntryHeader[4] = 45;
				this._EntryHeader[5] = 0;
				for (int i = 0; i < 8; i++)
				{
					this._EntryHeader[num3++] = 255;
				}
				num3 = (int)(30 + num4);
				this._EntryHeader[num3++] = 1;
				this._EntryHeader[num3++] = 0;
				num3 += 2;
				Array.Copy(BitConverter.GetBytes(this._UncompressedSize), 0, this._EntryHeader, num3, 8);
				num3 += 8;
				Array.Copy(BitConverter.GetBytes(this._CompressedSize), 0, this._EntryHeader, num3, 8);
			}
			else
			{
				this._EntryHeader[4] = 20;
				this._EntryHeader[5] = 0;
				num3 = 18;
				this._EntryHeader[num3++] = (byte)(this._CompressedSize & 255L);
				this._EntryHeader[num3++] = (byte)((this._CompressedSize & 65280L) >> 8);
				this._EntryHeader[num3++] = (byte)((this._CompressedSize & 16711680L) >> 16);
				this._EntryHeader[num3++] = (byte)((this._CompressedSize & (long)((ulong)-16777216)) >> 24);
				this._EntryHeader[num3++] = (byte)(this._UncompressedSize & 255L);
				this._EntryHeader[num3++] = (byte)((this._UncompressedSize & 65280L) >> 8);
				this._EntryHeader[num3++] = (byte)((this._UncompressedSize & 16711680L) >> 16);
				this._EntryHeader[num3++] = (byte)((this._UncompressedSize & (long)((ulong)-16777216)) >> 24);
				if (num5 != 0)
				{
					num3 = (int)(30 + num4);
					short num6 = (short)((int)this._EntryHeader[num3 + 2] + (int)this._EntryHeader[num3 + 3] * 256);
					if (num6 == 16)
					{
						this._EntryHeader[num3++] = 153;
						this._EntryHeader[num3++] = 153;
					}
				}
			}
			if ((this._BitField & 8) != 8 || (this._Source == ZipEntrySource.ZipOutputStream && s.CanSeek))
			{
				ZipSegmentedStream zipSegmentedStream = s as ZipSegmentedStream;
				if (zipSegmentedStream != null && this._diskNumber != zipSegmentedStream.CurrentSegment)
				{
					using (Stream stream = ZipSegmentedStream.ForUpdate(this._container.ZipFile.Name, this._diskNumber))
					{
						stream.Seek(this._RelativeOffsetOfLocalHeader, SeekOrigin.Begin);
						stream.Write(this._EntryHeader, 0, this._EntryHeader.Length);
					}
				}
				else
				{
					s.Seek(this._RelativeOffsetOfLocalHeader, SeekOrigin.Begin);
					s.Write(this._EntryHeader, 0, this._EntryHeader.Length);
					if (countingStream != null)
					{
						countingStream.Adjust((long)this._EntryHeader.Length);
					}
					s.Seek(this._CompressedSize, SeekOrigin.Current);
				}
			}
			if ((this._BitField & 8) == 8 && !this.IsDirectory)
			{
				byte[] array = new byte[16 + ((!this._OutputUsesZip64.Value) ? 0 : 8)];
				num3 = 0;
				Array.Copy(BitConverter.GetBytes(134695760), 0, array, num3, 4);
				num3 += 4;
				Array.Copy(BitConverter.GetBytes(this._Crc32), 0, array, num3, 4);
				num3 += 4;
				if (this._OutputUsesZip64.Value)
				{
					Array.Copy(BitConverter.GetBytes(this._CompressedSize), 0, array, num3, 8);
					num3 += 8;
					Array.Copy(BitConverter.GetBytes(this._UncompressedSize), 0, array, num3, 8);
					num3 += 8;
				}
				else
				{
					array[num3++] = (byte)(this._CompressedSize & 255L);
					array[num3++] = (byte)((this._CompressedSize & 65280L) >> 8);
					array[num3++] = (byte)((this._CompressedSize & 16711680L) >> 16);
					array[num3++] = (byte)((this._CompressedSize & (long)((ulong)-16777216)) >> 24);
					array[num3++] = (byte)(this._UncompressedSize & 255L);
					array[num3++] = (byte)((this._UncompressedSize & 65280L) >> 8);
					array[num3++] = (byte)((this._UncompressedSize & 16711680L) >> 16);
					array[num3++] = (byte)((this._UncompressedSize & (long)((ulong)-16777216)) >> 24);
				}
				s.Write(array, 0, array.Length);
				this._LengthOfTrailer += array.Length;
			}
		}

		private void SetZip64Flags()
		{
			this._entryRequiresZip64 = new bool?(this._CompressedSize >= (long)((ulong)-1) || this._UncompressedSize >= (long)((ulong)-1) || this._RelativeOffsetOfLocalHeader >= (long)((ulong)-1));
			if (this._container.Zip64 == Zip64Option.Default && this._entryRequiresZip64.Value)
			{
				throw new ZipException("Compressed or Uncompressed size, or offset exceeds the maximum value. Consider setting the UseZip64WhenSaving property on the ZipFile instance.");
			}
			this._OutputUsesZip64 = new bool?(this._container.Zip64 == Zip64Option.Always || this._entryRequiresZip64.Value);
		}

		internal void PrepOutputStream(Stream s, long streamLength, out CountingStream outputCounter, out Stream encryptor, out Stream compressor, out CrcCalculatorStream output)
		{
			outputCounter = new CountingStream(s);
			if (streamLength != 0L)
			{
				encryptor = this.MaybeApplyEncryption(outputCounter);
				compressor = this.MaybeApplyCompression(encryptor, streamLength);
			}
			else
			{
				Stream stream;
				compressor = (stream = outputCounter);
				encryptor = stream;
			}
			output = new CrcCalculatorStream(compressor, true);
		}

		private Stream MaybeApplyCompression(Stream s, long streamLength)
		{
			if (this._CompressionMethod != 8 || this.CompressionLevel == CompressionLevel.None)
			{
				return s;
			}
			if (this._container.ParallelDeflateThreshold == 0L || (streamLength > this._container.ParallelDeflateThreshold && this._container.ParallelDeflateThreshold > 0L))
			{
				if (this._container.ParallelDeflater == null)
				{
					this._container.ParallelDeflater = new ParallelDeflateOutputStream(s, this.CompressionLevel, this._container.Strategy, true);
					if (this._container.CodecBufferSize > 0)
					{
						this._container.ParallelDeflater.BufferSize = this._container.CodecBufferSize;
					}
					if (this._container.ParallelDeflateMaxBufferPairs > 0)
					{
						this._container.ParallelDeflater.MaxBufferPairs = this._container.ParallelDeflateMaxBufferPairs;
					}
				}
				ParallelDeflateOutputStream parallelDeflater = this._container.ParallelDeflater;
				parallelDeflater.Reset(s);
				return parallelDeflater;
			}
			DeflateStream deflateStream = new DeflateStream(s, CompressionMode.Compress, this.CompressionLevel, true);
			if (this._container.CodecBufferSize > 0)
			{
				deflateStream.BufferSize = this._container.CodecBufferSize;
			}
			deflateStream.Strategy = this._container.Strategy;
			return deflateStream;
		}

		private Stream MaybeApplyEncryption(Stream s)
		{
			if (this.Encryption == EncryptionAlgorithm.PkzipWeak)
			{
				return new ZipCipherStream(s, this._zipCrypto_forWrite, CryptoMode.Encrypt);
			}
			return s;
		}

		private void OnZipErrorWhileSaving(Exception e)
		{
			if (this._container.ZipFile != null)
			{
				this._ioOperationCanceled = this._container.ZipFile.OnZipErrorSaving(this, e);
			}
		}

		internal void Write(Stream s)
		{
			CountingStream countingStream = s as CountingStream;
			ZipSegmentedStream zipSegmentedStream = s as ZipSegmentedStream;
			bool flag = false;
			do
			{
				try
				{
					if (this._Source == ZipEntrySource.ZipFile && !this._restreamRequiredOnSave)
					{
						this.CopyThroughOneEntry(s);
						break;
					}
					if (this.IsDirectory)
					{
						this.WriteHeader(s, 1);
						this.StoreRelativeOffset();
						this._entryRequiresZip64 = new bool?(this._RelativeOffsetOfLocalHeader >= (long)((ulong)-1));
						this._OutputUsesZip64 = new bool?(this._container.Zip64 == Zip64Option.Always || this._entryRequiresZip64.Value);
						if (zipSegmentedStream != null)
						{
							this._diskNumber = zipSegmentedStream.CurrentSegment;
						}
						break;
					}
					int num = 0;
					bool flag2;
					do
					{
						num++;
						this.WriteHeader(s, num);
						this.WriteSecurityMetadata(s);
						this._WriteEntryData(s);
						this._TotalEntrySize = (long)this._LengthOfHeader + this._CompressedFileDataSize + (long)this._LengthOfTrailer;
						flag2 = (num <= 1 && s.CanSeek && this.WantReadAgain());
						if (flag2)
						{
							if (zipSegmentedStream != null)
							{
								zipSegmentedStream.TruncateBackward(this._diskNumber, this._RelativeOffsetOfLocalHeader);
							}
							else
							{
								s.Seek(this._RelativeOffsetOfLocalHeader, SeekOrigin.Begin);
							}
							s.SetLength(s.Position);
							if (countingStream != null)
							{
								countingStream.Adjust(this._TotalEntrySize);
							}
						}
					}
					while (flag2);
					this._skippedDuringSave = false;
					flag = true;
				}
				catch (Exception ex)
				{
					ZipErrorAction zipErrorAction = this.ZipErrorAction;
					int num2 = 0;
					while (this.ZipErrorAction != ZipErrorAction.Throw)
					{
						if (this.ZipErrorAction == ZipErrorAction.Skip || this.ZipErrorAction == ZipErrorAction.Retry)
						{
							long num3 = (countingStream == null) ? s.Position : countingStream.ComputedPosition;
							long num4 = num3 - this._future_ROLH;
							if (num4 > 0L)
							{
								s.Seek(num4, SeekOrigin.Current);
								long position = s.Position;
								s.SetLength(s.Position);
								if (countingStream != null)
								{
									countingStream.Adjust(num3 - position);
								}
							}
							if (this.ZipErrorAction == ZipErrorAction.Skip)
							{
								this.WriteStatus("Skipping file {0} (exception: {1})", new object[]
								{
									this.LocalFileName,
									ex.ToString()
								});
								this._skippedDuringSave = true;
								flag = true;
							}
							else
							{
								this.ZipErrorAction = zipErrorAction;
							}
						}
						else
						{
							if (num2 > 0)
							{
								throw;
							}
							if (this.ZipErrorAction == ZipErrorAction.InvokeErrorEvent)
							{
								this.OnZipErrorWhileSaving(ex);
								if (this._ioOperationCanceled)
								{
									flag = true;
									goto IL_283;
								}
							}
							num2++;
							continue;
						}
						IL_283:
						goto IL_288;
					}
					throw;
				}
				IL_288:;
			}
			while (!flag);
		}

		internal void StoreRelativeOffset()
		{
			this._RelativeOffsetOfLocalHeader = this._future_ROLH;
		}

		internal void NotifySaveComplete()
		{
			this._Encryption_FromZipFile = this._Encryption;
			this._CompressionMethod_FromZipFile = this._CompressionMethod;
			this._restreamRequiredOnSave = false;
			this._metadataChanged = false;
			this._Source = ZipEntrySource.ZipFile;
		}

		internal void WriteSecurityMetadata(Stream outstream)
		{
			if (this.Encryption == EncryptionAlgorithm.None)
			{
				return;
			}
			string password = this._Password;
			if (this._Source == ZipEntrySource.ZipFile && password == null)
			{
				password = this._container.Password;
			}
			if (password == null)
			{
				this._zipCrypto_forWrite = null;
				return;
			}
			if (this.Encryption == EncryptionAlgorithm.PkzipWeak)
			{
				this._zipCrypto_forWrite = ZipCrypto.ForWrite(password);
				Random random = new Random();
				byte[] array = new byte[12];
				random.NextBytes(array);
				if ((this._BitField & 8) == 8)
				{
					this._TimeBlob = SharedUtilities.DateTimeToPacked(this.LastModified);
					array[11] = (byte)(this._TimeBlob >> 8 & 255);
				}
				else
				{
					this.FigureCrc32();
					array[11] = (byte)(this._Crc32 >> 24 & 255);
				}
				byte[] array2 = this._zipCrypto_forWrite.EncryptMessage(array, array.Length);
				outstream.Write(array2, 0, array2.Length);
				this._LengthOfHeader += array2.Length;
			}
		}

		private void CopyThroughOneEntry(Stream outStream)
		{
			if (this.LengthOfHeader == 0)
			{
				throw new BadStateException("Bad header length.");
			}
			bool flag = this._metadataChanged || this.ArchiveStream is ZipSegmentedStream || outStream is ZipSegmentedStream || (this._InputUsesZip64 && this._container.UseZip64WhenSaving == Zip64Option.Default) || (!this._InputUsesZip64 && this._container.UseZip64WhenSaving == Zip64Option.Always);
			if (flag)
			{
				this.CopyThroughWithRecompute(outStream);
			}
			else
			{
				this.CopyThroughWithNoChange(outStream);
			}
			this._entryRequiresZip64 = new bool?(this._CompressedSize >= (long)((ulong)-1) || this._UncompressedSize >= (long)((ulong)-1) || this._RelativeOffsetOfLocalHeader >= (long)((ulong)-1));
			this._OutputUsesZip64 = new bool?(this._container.Zip64 == Zip64Option.Always || this._entryRequiresZip64.Value);
		}

		private void CopyThroughWithRecompute(Stream outstream)
		{
			byte[] array = new byte[this.BufferSize];
			CountingStream countingStream = new CountingStream(this.ArchiveStream);
			long relativeOffsetOfLocalHeader = this._RelativeOffsetOfLocalHeader;
			int lengthOfHeader = this.LengthOfHeader;
			this.WriteHeader(outstream, 0);
			this.StoreRelativeOffset();
			if (!this.FileName.EndsWith("/"))
			{
				long num = relativeOffsetOfLocalHeader + (long)lengthOfHeader;
				int num2 = ZipEntry.GetLengthOfCryptoHeaderBytes(this._Encryption_FromZipFile);
				num -= (long)num2;
				this._LengthOfHeader += num2;
				countingStream.Seek(num, SeekOrigin.Begin);
				long num3 = this._CompressedSize;
				while (num3 > 0L)
				{
					num2 = ((num3 <= (long)array.Length) ? ((int)num3) : array.Length);
					int num4 = countingStream.Read(array, 0, num2);
					outstream.Write(array, 0, num4);
					num3 -= (long)num4;
					this.OnWriteBlock(countingStream.BytesRead, this._CompressedSize);
					if (this._ioOperationCanceled)
					{
						break;
					}
				}
				if ((this._BitField & 8) == 8)
				{
					int num5 = 16;
					if (this._InputUsesZip64)
					{
						num5 += 8;
					}
					byte[] buffer = new byte[num5];
					countingStream.Read(buffer, 0, num5);
					if (this._InputUsesZip64 && this._container.UseZip64WhenSaving == Zip64Option.Default)
					{
						outstream.Write(buffer, 0, 8);
						if (this._CompressedSize > (long)((ulong)-1))
						{
							throw new InvalidOperationException("ZIP64 is required");
						}
						outstream.Write(buffer, 8, 4);
						if (this._UncompressedSize > (long)((ulong)-1))
						{
							throw new InvalidOperationException("ZIP64 is required");
						}
						outstream.Write(buffer, 16, 4);
						this._LengthOfTrailer -= 8;
					}
					else if (!this._InputUsesZip64 && this._container.UseZip64WhenSaving == Zip64Option.Always)
					{
						byte[] buffer2 = new byte[4];
						outstream.Write(buffer, 0, 8);
						outstream.Write(buffer, 8, 4);
						outstream.Write(buffer2, 0, 4);
						outstream.Write(buffer, 12, 4);
						outstream.Write(buffer2, 0, 4);
						this._LengthOfTrailer += 8;
					}
					else
					{
						outstream.Write(buffer, 0, num5);
					}
				}
			}
			this._TotalEntrySize = (long)this._LengthOfHeader + this._CompressedFileDataSize + (long)this._LengthOfTrailer;
		}

		private void CopyThroughWithNoChange(Stream outstream)
		{
			byte[] array = new byte[this.BufferSize];
			CountingStream countingStream = new CountingStream(this.ArchiveStream);
			countingStream.Seek(this._RelativeOffsetOfLocalHeader, SeekOrigin.Begin);
			if (this._TotalEntrySize == 0L)
			{
				this._TotalEntrySize = (long)this._LengthOfHeader + this._CompressedFileDataSize + (long)this._LengthOfTrailer;
			}
			CountingStream countingStream2 = outstream as CountingStream;
			this._RelativeOffsetOfLocalHeader = ((countingStream2 == null) ? outstream.Position : countingStream2.ComputedPosition);
			long num = this._TotalEntrySize;
			while (num > 0L)
			{
				int count = (num <= (long)array.Length) ? ((int)num) : array.Length;
				int num2 = countingStream.Read(array, 0, count);
				outstream.Write(array, 0, num2);
				num -= (long)num2;
				this.OnWriteBlock(countingStream.BytesRead, this._TotalEntrySize);
				if (this._ioOperationCanceled)
				{
					break;
				}
			}
		}

		[Conditional("Trace")]
		private void TraceWriteLine(string format, params object[] varParams)
		{
			object outputLock = this._outputLock;
			lock (outputLock)
			{
				int hashCode = Thread.CurrentThread.GetHashCode();
				Console.Write("{0:000} ZipEntry.Write ", hashCode);
				Console.WriteLine(format, varParams);
			}
		}

		public void SetEntryTimes(DateTime created, DateTime accessed, DateTime modified)
		{
			this._ntfsTimesAreSet = true;
			if (created == ZipEntry._zeroHour && created.Kind == ZipEntry._zeroHour.Kind)
			{
				created = ZipEntry._win32Epoch;
			}
			if (accessed == ZipEntry._zeroHour && accessed.Kind == ZipEntry._zeroHour.Kind)
			{
				accessed = ZipEntry._win32Epoch;
			}
			if (modified == ZipEntry._zeroHour && modified.Kind == ZipEntry._zeroHour.Kind)
			{
				modified = ZipEntry._win32Epoch;
			}
			this._Ctime = created.ToUniversalTime();
			this._Atime = accessed.ToUniversalTime();
			this._Mtime = modified.ToUniversalTime();
			this._LastModified = this._Mtime;
			if (!this._emitUnixTimes && !this._emitNtfsTimes)
			{
				this._emitNtfsTimes = true;
			}
			this._metadataChanged = true;
		}

		internal static string NameInArchive(string filename, string directoryPathInArchive)
		{
			string pathName;
			if (directoryPathInArchive == null)
			{
				pathName = filename;
			}
			else if (string.IsNullOrEmpty(directoryPathInArchive))
			{
				pathName = Path.GetFileName(filename);
			}
			else
			{
				pathName = Path.Combine(directoryPathInArchive, Path.GetFileName(filename));
			}
			return SharedUtilities.NormalizePathForUseInZipFile(pathName);
		}

		internal static ZipEntry CreateFromNothing(string nameInArchive)
		{
			return ZipEntry.Create(nameInArchive, ZipEntrySource.None, null, null);
		}

		internal static ZipEntry CreateFromFile(string filename, string nameInArchive)
		{
			return ZipEntry.Create(nameInArchive, ZipEntrySource.FileSystem, filename, null);
		}

		internal static ZipEntry CreateForStream(string entryName, Stream s)
		{
			return ZipEntry.Create(entryName, ZipEntrySource.Stream, s, null);
		}

		internal static ZipEntry CreateForWriter(string entryName, WriteDelegate d)
		{
			return ZipEntry.Create(entryName, ZipEntrySource.WriteDelegate, d, null);
		}

		internal static ZipEntry CreateForJitStreamProvider(string nameInArchive, OpenDelegate opener, CloseDelegate closer)
		{
			return ZipEntry.Create(nameInArchive, ZipEntrySource.JitStream, opener, closer);
		}

		internal static ZipEntry CreateForZipOutputStream(string nameInArchive)
		{
			return ZipEntry.Create(nameInArchive, ZipEntrySource.ZipOutputStream, null, null);
		}

		private static ZipEntry Create(string nameInArchive, ZipEntrySource source, object arg1, object arg2)
		{
			if (string.IsNullOrEmpty(nameInArchive))
			{
				throw new ZipException("The entry name must be non-null and non-empty.");
			}
			ZipEntry zipEntry = new ZipEntry();
			zipEntry._VersionMadeBy = 45;
			zipEntry._Source = source;
			zipEntry._Mtime = (zipEntry._Atime = (zipEntry._Ctime = DateTime.UtcNow));
			if (source == ZipEntrySource.Stream)
			{
				zipEntry._sourceStream = (arg1 as Stream);
			}
			else if (source == ZipEntrySource.WriteDelegate)
			{
				zipEntry._WriteDelegate = (arg1 as WriteDelegate);
			}
			else if (source == ZipEntrySource.JitStream)
			{
				zipEntry._OpenDelegate = (arg1 as OpenDelegate);
				zipEntry._CloseDelegate = (arg2 as CloseDelegate);
			}
			else if (source != ZipEntrySource.ZipOutputStream)
			{
				if (source == ZipEntrySource.None)
				{
					zipEntry._Source = ZipEntrySource.FileSystem;
				}
				else
				{
					string text = arg1 as string;
					if (string.IsNullOrEmpty(text))
					{
						throw new ZipException("The filename must be non-null and non-empty.");
					}
					try
					{
						zipEntry._Mtime = (zipEntry._Ctime = (zipEntry._Atime = DateTime.UtcNow));
						zipEntry._ExternalFileAttrs = 0;
						zipEntry._ntfsTimesAreSet = true;
						zipEntry._LocalFileName = Path.GetFullPath(text);
					}
					catch (PathTooLongException innerException)
					{
						string message = string.Format("The path is too long, filename={0}", text);
						throw new ZipException(message, innerException);
					}
				}
			}
			zipEntry._LastModified = zipEntry._Mtime;
			zipEntry._FileNameInArchive = SharedUtilities.NormalizePathForUseInZipFile(nameInArchive);
			return zipEntry;
		}

		internal void MarkAsDirectory()
		{
			this._IsDirectory = true;
			if (!this._FileNameInArchive.EndsWith("/"))
			{
				this._FileNameInArchive += "/";
			}
		}

		public override string ToString()
		{
			return string.Format("ZipEntry::{0}", this.FileName);
		}

		private void SetFdpLoh()
		{
			long position = this.ArchiveStream.Position;
			try
			{
				this.ArchiveStream.Seek(this._RelativeOffsetOfLocalHeader, SeekOrigin.Begin);
			}
			catch (IOException innerException)
			{
				string message = string.Format("Exception seeking  entry({0}) offset(0x{1:X8}) len(0x{2:X8})", this.FileName, this._RelativeOffsetOfLocalHeader, this.ArchiveStream.Length);
				throw new BadStateException(message, innerException);
			}
			byte[] array = new byte[30];
			this.ArchiveStream.Read(array, 0, array.Length);
			short num = (short)((int)array[26] + (int)array[27] * 256);
			short num2 = (short)((int)array[28] + (int)array[29] * 256);
			this.ArchiveStream.Seek((long)(num + num2), SeekOrigin.Current);
			this._LengthOfHeader = (int)(30 + num2 + num) + ZipEntry.GetLengthOfCryptoHeaderBytes(this._Encryption_FromZipFile);
			this.__FileDataPosition = this._RelativeOffsetOfLocalHeader + (long)this._LengthOfHeader;
			this.ArchiveStream.Seek(position, SeekOrigin.Begin);
		}

		internal static int GetLengthOfCryptoHeaderBytes(EncryptionAlgorithm a)
		{
			if (a == EncryptionAlgorithm.None)
			{
				return 0;
			}
			if (a == EncryptionAlgorithm.PkzipWeak)
			{
				return 12;
			}
			throw new ZipException("internal error");
		}
	}
}

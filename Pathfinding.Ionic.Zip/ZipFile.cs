using Pathfinding.Ionic.Zlib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace Pathfinding.Ionic.Zip
{
	[ClassInterface(ClassInterfaceType.AutoDispatch), ComVisible(true), Guid("ebc25cf6-9120-4283-b972-0e5520d00005")]
	public class ZipFile : IEnumerable<ZipEntry>, IDisposable, IEnumerable
	{
		private long _lengthOfReadStream = -99L;

		private TextWriter _StatusMessageTextWriter;

		private bool _CaseSensitiveRetrieval;

		private Stream _readstream;

		private Stream _writestream;

		private ushort _versionMadeBy;

		private ushort _versionNeededToExtract;

		private uint _diskNumberWithCd;

		private int _maxOutputSegmentSize;

		private uint _numberOfSegmentsForMostRecentSave;

		private ZipErrorAction _zipErrorAction;

		private bool _disposed;

		private Dictionary<string, ZipEntry> _entries;

		private List<ZipEntry> _zipEntriesAsList;

		private string _name;

		private string _readName;

		private string _Comment;

		internal string _Password;

		private bool _emitNtfsTimes = true;

		private bool _emitUnixTimes;

		private CompressionStrategy _Strategy;

		private CompressionMethod _compressionMethod = CompressionMethod.Deflate;

		private bool _fileAlreadyExists;

		private string _temporaryFileName;

		private bool _contentsChanged;

		private bool _hasBeenSaved;

		private string _TempFileFolder;

		private bool _ReadStreamIsOurs = true;

		private object LOCK = new object();

		private bool _saveOperationCanceled;

		private bool _extractOperationCanceled;

		private bool _addOperationCanceled;

		private EncryptionAlgorithm _Encryption;

		private bool _JustSaved;

		private long _locEndOfCDS = -1L;

		private uint _OffsetOfCentralDirectory;

		private long _OffsetOfCentralDirectory64;

		private bool? _OutputUsesZip64;

		internal bool _inExtractAll;

		private static Encoding _defaultEncoding = Encoding.UTF8;

		private Encoding _alternateEncoding = Encoding.UTF8;

		private ZipOption _alternateEncodingUsage;

		private int _BufferSize = ZipFile.BufferSizeDefault;

		internal ParallelDeflateOutputStream ParallelDeflater;

		private long _ParallelDeflateThreshold;

		private int _maxBufferPairs = 16;

		internal Zip64Option _zip64;

		private bool _SavingSfx;

		public static readonly int BufferSizeDefault = 32768;

		public event EventHandler<SaveProgressEventArgs> SaveProgress
		{
			add
			{
				EventHandler<SaveProgressEventArgs> eventHandler = this.SaveProgress;
				EventHandler<SaveProgressEventArgs> eventHandler2;
				do
				{
					eventHandler2 = eventHandler;
					eventHandler = Interlocked.CompareExchange<EventHandler<SaveProgressEventArgs>>(ref this.SaveProgress, (EventHandler<SaveProgressEventArgs>)Delegate.Combine(eventHandler2, value), eventHandler);
				}
				while (eventHandler != eventHandler2);
			}
			remove
			{
				EventHandler<SaveProgressEventArgs> eventHandler = this.SaveProgress;
				EventHandler<SaveProgressEventArgs> eventHandler2;
				do
				{
					eventHandler2 = eventHandler;
					eventHandler = Interlocked.CompareExchange<EventHandler<SaveProgressEventArgs>>(ref this.SaveProgress, (EventHandler<SaveProgressEventArgs>)Delegate.Remove(eventHandler2, value), eventHandler);
				}
				while (eventHandler != eventHandler2);
			}
		}

		public event EventHandler<ReadProgressEventArgs> ReadProgress
		{
			add
			{
				EventHandler<ReadProgressEventArgs> eventHandler = this.ReadProgress;
				EventHandler<ReadProgressEventArgs> eventHandler2;
				do
				{
					eventHandler2 = eventHandler;
					eventHandler = Interlocked.CompareExchange<EventHandler<ReadProgressEventArgs>>(ref this.ReadProgress, (EventHandler<ReadProgressEventArgs>)Delegate.Combine(eventHandler2, value), eventHandler);
				}
				while (eventHandler != eventHandler2);
			}
			remove
			{
				EventHandler<ReadProgressEventArgs> eventHandler = this.ReadProgress;
				EventHandler<ReadProgressEventArgs> eventHandler2;
				do
				{
					eventHandler2 = eventHandler;
					eventHandler = Interlocked.CompareExchange<EventHandler<ReadProgressEventArgs>>(ref this.ReadProgress, (EventHandler<ReadProgressEventArgs>)Delegate.Remove(eventHandler2, value), eventHandler);
				}
				while (eventHandler != eventHandler2);
			}
		}

		public event EventHandler<ExtractProgressEventArgs> ExtractProgress
		{
			add
			{
				EventHandler<ExtractProgressEventArgs> eventHandler = this.ExtractProgress;
				EventHandler<ExtractProgressEventArgs> eventHandler2;
				do
				{
					eventHandler2 = eventHandler;
					eventHandler = Interlocked.CompareExchange<EventHandler<ExtractProgressEventArgs>>(ref this.ExtractProgress, (EventHandler<ExtractProgressEventArgs>)Delegate.Combine(eventHandler2, value), eventHandler);
				}
				while (eventHandler != eventHandler2);
			}
			remove
			{
				EventHandler<ExtractProgressEventArgs> eventHandler = this.ExtractProgress;
				EventHandler<ExtractProgressEventArgs> eventHandler2;
				do
				{
					eventHandler2 = eventHandler;
					eventHandler = Interlocked.CompareExchange<EventHandler<ExtractProgressEventArgs>>(ref this.ExtractProgress, (EventHandler<ExtractProgressEventArgs>)Delegate.Remove(eventHandler2, value), eventHandler);
				}
				while (eventHandler != eventHandler2);
			}
		}

		public event EventHandler<AddProgressEventArgs> AddProgress
		{
			add
			{
				EventHandler<AddProgressEventArgs> eventHandler = this.AddProgress;
				EventHandler<AddProgressEventArgs> eventHandler2;
				do
				{
					eventHandler2 = eventHandler;
					eventHandler = Interlocked.CompareExchange<EventHandler<AddProgressEventArgs>>(ref this.AddProgress, (EventHandler<AddProgressEventArgs>)Delegate.Combine(eventHandler2, value), eventHandler);
				}
				while (eventHandler != eventHandler2);
			}
			remove
			{
				EventHandler<AddProgressEventArgs> eventHandler = this.AddProgress;
				EventHandler<AddProgressEventArgs> eventHandler2;
				do
				{
					eventHandler2 = eventHandler;
					eventHandler = Interlocked.CompareExchange<EventHandler<AddProgressEventArgs>>(ref this.AddProgress, (EventHandler<AddProgressEventArgs>)Delegate.Remove(eventHandler2, value), eventHandler);
				}
				while (eventHandler != eventHandler2);
			}
		}

		public event EventHandler<ZipErrorEventArgs> ZipError
		{
			add
			{
				EventHandler<ZipErrorEventArgs> eventHandler = this.ZipError;
				EventHandler<ZipErrorEventArgs> eventHandler2;
				do
				{
					eventHandler2 = eventHandler;
					eventHandler = Interlocked.CompareExchange<EventHandler<ZipErrorEventArgs>>(ref this.ZipError, (EventHandler<ZipErrorEventArgs>)Delegate.Combine(eventHandler2, value), eventHandler);
				}
				while (eventHandler != eventHandler2);
			}
			remove
			{
				EventHandler<ZipErrorEventArgs> eventHandler = this.ZipError;
				EventHandler<ZipErrorEventArgs> eventHandler2;
				do
				{
					eventHandler2 = eventHandler;
					eventHandler = Interlocked.CompareExchange<EventHandler<ZipErrorEventArgs>>(ref this.ZipError, (EventHandler<ZipErrorEventArgs>)Delegate.Remove(eventHandler2, value), eventHandler);
				}
				while (eventHandler != eventHandler2);
			}
		}

		public string Info
		{
			get
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append(string.Format("          ZipFile: {0}\n", this.Name));
				if (!string.IsNullOrEmpty(this._Comment))
				{
					stringBuilder.Append(string.Format("          Comment: {0}\n", this._Comment));
				}
				if (this._versionMadeBy != 0)
				{
					stringBuilder.Append(string.Format("  version made by: 0x{0:X4}\n", this._versionMadeBy));
				}
				if (this._versionNeededToExtract != 0)
				{
					stringBuilder.Append(string.Format("needed to extract: 0x{0:X4}\n", this._versionNeededToExtract));
				}
				stringBuilder.Append(string.Format("       uses ZIP64: {0}\n", this.InputUsesZip64));
				stringBuilder.Append(string.Format("     disk with CD: {0}\n", this._diskNumberWithCd));
				if (this._OffsetOfCentralDirectory == 4294967295u)
				{
					stringBuilder.Append(string.Format("      CD64 offset: 0x{0:X16}\n", this._OffsetOfCentralDirectory64));
				}
				else
				{
					stringBuilder.Append(string.Format("        CD offset: 0x{0:X8}\n", this._OffsetOfCentralDirectory));
				}
				stringBuilder.Append("\n");
				foreach (ZipEntry current in this._entries.Values)
				{
					stringBuilder.Append(current.Info);
				}
				return stringBuilder.ToString();
			}
		}

		private string ArchiveNameForEvent
		{
			get
			{
				return (this._name == null) ? "(stream)" : this._name;
			}
		}

		private long LengthOfReadStream
		{
			get
			{
				if (this._lengthOfReadStream == -99L)
				{
					this._lengthOfReadStream = ((!this._ReadStreamIsOurs) ? -1L : SharedUtilities.GetFileLength(this._name));
				}
				return this._lengthOfReadStream;
			}
		}

		public bool FullScan
		{
			get;
			set;
		}

		public bool SortEntriesBeforeSaving
		{
			get;
			set;
		}

		public bool AddDirectoryWillTraverseReparsePoints
		{
			get;
			set;
		}

		public int BufferSize
		{
			get
			{
				return this._BufferSize;
			}
			set
			{
				this._BufferSize = value;
			}
		}

		public int CodecBufferSize
		{
			get;
			set;
		}

		public bool FlattenFoldersOnExtract
		{
			get;
			set;
		}

		public CompressionStrategy Strategy
		{
			get
			{
				return this._Strategy;
			}
			set
			{
				this._Strategy = value;
			}
		}

		public string Name
		{
			get
			{
				return this._name;
			}
			set
			{
				this._name = value;
			}
		}

		public CompressionLevel CompressionLevel
		{
			get;
			set;
		}

		public CompressionMethod CompressionMethod
		{
			get
			{
				return this._compressionMethod;
			}
			set
			{
				this._compressionMethod = value;
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
				this._contentsChanged = true;
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
			}
		}

		internal bool Verbose
		{
			get
			{
				return this._StatusMessageTextWriter != null;
			}
		}

		public bool CaseSensitiveRetrieval
		{
			get
			{
				return this._CaseSensitiveRetrieval;
			}
			set
			{
				if (value != this._CaseSensitiveRetrieval)
				{
					this._CaseSensitiveRetrieval = value;
					this._initEntriesDictionary();
				}
			}
		}

		[Obsolete("Beginning with v1.9.1.6 of DotNetZip, this property is obsolete.  It will be removed in a future version of the library. Your applications should  use AlternateEncoding and AlternateEncodingUsage instead.")]
		public bool UseUnicodeAsNecessary
		{
			get
			{
				return this._alternateEncoding == Encoding.GetEncoding("UTF-8") && this._alternateEncodingUsage == ZipOption.AsNecessary;
			}
			set
			{
				if (value)
				{
					this._alternateEncoding = Encoding.GetEncoding("UTF-8");
					this._alternateEncodingUsage = ZipOption.AsNecessary;
				}
				else
				{
					this._alternateEncoding = ZipFile.DefaultEncoding;
					this._alternateEncodingUsage = ZipOption.Default;
				}
			}
		}

		public Zip64Option UseZip64WhenSaving
		{
			get
			{
				return this._zip64;
			}
			set
			{
				this._zip64 = value;
			}
		}

		public bool? RequiresZip64
		{
			get
			{
				if (this._entries.Count > 65534)
				{
					return new bool?(true);
				}
				if (!this._hasBeenSaved || this._contentsChanged)
				{
					return null;
				}
				foreach (ZipEntry current in this._entries.Values)
				{
					if (current.RequiresZip64.Value)
					{
						return new bool?(true);
					}
				}
				return new bool?(false);
			}
		}

		public bool? OutputUsedZip64
		{
			get
			{
				return this._OutputUsesZip64;
			}
		}

		public bool? InputUsesZip64
		{
			get
			{
				if (this._entries.Count > 65534)
				{
					return new bool?(true);
				}
				foreach (ZipEntry current in this)
				{
					if (current.Source != ZipEntrySource.ZipFile)
					{
						bool? result = null;
						return result;
					}
					if (current._InputUsesZip64)
					{
						bool? result = new bool?(true);
						return result;
					}
				}
				return new bool?(false);
			}
		}

		[Obsolete("use AlternateEncoding instead.")]
		public Encoding ProvisionalAlternateEncoding
		{
			get
			{
				if (this._alternateEncodingUsage == ZipOption.AsNecessary)
				{
					return this._alternateEncoding;
				}
				return null;
			}
			set
			{
				this._alternateEncoding = value;
				this._alternateEncodingUsage = ZipOption.AsNecessary;
			}
		}

		public Encoding AlternateEncoding
		{
			get
			{
				return this._alternateEncoding;
			}
			set
			{
				this._alternateEncoding = value;
			}
		}

		public ZipOption AlternateEncodingUsage
		{
			get
			{
				return this._alternateEncodingUsage;
			}
			set
			{
				this._alternateEncodingUsage = value;
			}
		}

		public static Encoding DefaultEncoding
		{
			get
			{
				return ZipFile._defaultEncoding;
			}
		}

		public TextWriter StatusMessageTextWriter
		{
			get
			{
				return this._StatusMessageTextWriter;
			}
			set
			{
				this._StatusMessageTextWriter = value;
			}
		}

		public string TempFileFolder
		{
			get
			{
				return this._TempFileFolder;
			}
			set
			{
				this._TempFileFolder = value;
				if (value == null)
				{
					return;
				}
				if (!Directory.Exists(value))
				{
					throw new FileNotFoundException(string.Format("That directory ({0}) does not exist.", value));
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
					this.Encryption = EncryptionAlgorithm.None;
				}
				else if (this.Encryption == EncryptionAlgorithm.None)
				{
					this.Encryption = EncryptionAlgorithm.PkzipWeak;
				}
			}
		}

		public ExtractExistingFileAction ExtractExistingFile
		{
			get;
			set;
		}

		public ZipErrorAction ZipErrorAction
		{
			get
			{
				if (this.ZipError != null)
				{
					this._zipErrorAction = ZipErrorAction.InvokeErrorEvent;
				}
				return this._zipErrorAction;
			}
			set
			{
				this._zipErrorAction = value;
				if (this._zipErrorAction != ZipErrorAction.InvokeErrorEvent && this.ZipError != null)
				{
					this.ZipError = null;
				}
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
				if (value == EncryptionAlgorithm.Unsupported)
				{
					throw new InvalidOperationException("You may not set Encryption to that value.");
				}
				this._Encryption = value;
			}
		}

		public SetCompressionCallback SetCompression
		{
			get;
			set;
		}

		public int MaxOutputSegmentSize
		{
			get
			{
				return this._maxOutputSegmentSize;
			}
			set
			{
				if (value < 65536 && value != 0)
				{
					throw new ZipException("The minimum acceptable segment size is 65536.");
				}
				this._maxOutputSegmentSize = value;
			}
		}

		public int NumberOfSegmentsForMostRecentSave
		{
			get
			{
				return (int)(this._numberOfSegmentsForMostRecentSave + 1u);
			}
		}

		public long ParallelDeflateThreshold
		{
			get
			{
				return this._ParallelDeflateThreshold;
			}
			set
			{
				if (value != 0L && value != -1L && value < 65536L)
				{
					throw new ArgumentOutOfRangeException("ParallelDeflateThreshold should be -1, 0, or > 65536");
				}
				this._ParallelDeflateThreshold = value;
			}
		}

		public int ParallelDeflateMaxBufferPairs
		{
			get
			{
				return this._maxBufferPairs;
			}
			set
			{
				if (value < 4)
				{
					throw new ArgumentOutOfRangeException("ParallelDeflateMaxBufferPairs", "Value must be 4 or greater.");
				}
				this._maxBufferPairs = value;
			}
		}

		public static Version LibraryVersion
		{
			get
			{
				return Assembly.GetExecutingAssembly().GetName().Version;
			}
		}

		private List<ZipEntry> ZipEntriesAsList
		{
			get
			{
				if (this._zipEntriesAsList == null)
				{
					this._zipEntriesAsList = new List<ZipEntry>(this._entries.Values);
				}
				return this._zipEntriesAsList;
			}
		}

		public ZipEntry this[int ix]
		{
			get
			{
				return this.ZipEntriesAsList[ix];
			}
		}

		public ZipEntry this[string fileName]
		{
			get
			{
				string text = SharedUtilities.NormalizePathForUseInZipFile(fileName);
				if (this._entries.ContainsKey(text))
				{
					return this._entries[text];
				}
				text = text.Replace("/", "\\");
				if (this._entries.ContainsKey(text))
				{
					return this._entries[text];
				}
				return null;
			}
		}

		public ICollection<string> EntryFileNames
		{
			get
			{
				return this._entries.Keys;
			}
		}

		public ICollection<ZipEntry> Entries
		{
			get
			{
				return this._entries.Values;
			}
		}

		public ICollection<ZipEntry> EntriesSorted
		{
			get
			{
				List<ZipEntry> list = new List<ZipEntry>();
				foreach (ZipEntry current in this.Entries)
				{
					list.Add(current);
				}
				int arg_54_0 = (!this.CaseSensitiveRetrieval) ? 5 : 4;
				return list.AsReadOnly();
			}
		}

		public int Count
		{
			get
			{
				return this._entries.Count;
			}
		}

		internal Stream ReadStream
		{
			get
			{
				if (this._readstream == null && (this._readName != null || this._name != null))
				{
					this._readstream = File.Open(this._readName ?? this._name, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
					this._ReadStreamIsOurs = true;
				}
				return this._readstream;
			}
		}

		private Stream WriteStream
		{
			get
			{
				if (this._writestream != null)
				{
					return this._writestream;
				}
				if (this._name == null)
				{
					return this._writestream;
				}
				if (this._maxOutputSegmentSize != 0)
				{
					this._writestream = ZipSegmentedStream.ForWriting(this._name, this._maxOutputSegmentSize);
					return this._writestream;
				}
				SharedUtilities.CreateAndOpenUniqueTempFile(this.TempFileFolder ?? Path.GetDirectoryName(this._name), out this._writestream, out this._temporaryFileName);
				return this._writestream;
			}
			set
			{
				if (value != null)
				{
					throw new ZipException("Cannot set the stream to a non-null value.");
				}
				this._writestream = null;
			}
		}

		public ZipFile(string fileName)
		{
			try
			{
				this._InitInstance(fileName, null);
			}
			catch (Exception innerException)
			{
				throw new ZipException(string.Format("Could not read {0} as a zip file", fileName), innerException);
			}
		}

		public ZipFile(string fileName, Encoding encoding)
		{
			try
			{
				this.AlternateEncoding = encoding;
				this.AlternateEncodingUsage = ZipOption.Always;
				this._InitInstance(fileName, null);
			}
			catch (Exception innerException)
			{
				throw new ZipException(string.Format("{0} is not a valid zip file", fileName), innerException);
			}
		}

		public ZipFile()
		{
			this._InitInstance(null, null);
		}

		public ZipFile(Encoding encoding)
		{
			this.AlternateEncoding = encoding;
			this.AlternateEncodingUsage = ZipOption.Always;
			this._InitInstance(null, null);
		}

		public ZipFile(string fileName, TextWriter statusMessageWriter)
		{
			try
			{
				this._InitInstance(fileName, statusMessageWriter);
			}
			catch (Exception innerException)
			{
				throw new ZipException(string.Format("{0} is not a valid zip file", fileName), innerException);
			}
		}

		public ZipFile(string fileName, TextWriter statusMessageWriter, Encoding encoding)
		{
			try
			{
				this.AlternateEncoding = encoding;
				this.AlternateEncodingUsage = ZipOption.Always;
				this._InitInstance(fileName, statusMessageWriter);
			}
			catch (Exception innerException)
			{
				throw new ZipException(string.Format("{0} is not a valid zip file", fileName), innerException);
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		public ZipEntry AddItem(string fileOrDirectoryName)
		{
			return this.AddItem(fileOrDirectoryName, null);
		}

		public ZipEntry AddItem(string fileOrDirectoryName, string directoryPathInArchive)
		{
			if (File.Exists(fileOrDirectoryName))
			{
				return this.AddFile(fileOrDirectoryName, directoryPathInArchive);
			}
			if (Directory.Exists(fileOrDirectoryName))
			{
				return this.AddDirectory(fileOrDirectoryName, directoryPathInArchive);
			}
			throw new FileNotFoundException(string.Format("That file or directory ({0}) does not exist!", fileOrDirectoryName));
		}

		public ZipEntry AddFile(string fileName)
		{
			return this.AddFile(fileName, null);
		}

		public ZipEntry AddFile(string fileName, string directoryPathInArchive)
		{
			string nameInArchive = ZipEntry.NameInArchive(fileName, directoryPathInArchive);
			ZipEntry ze = ZipEntry.CreateFromFile(fileName, nameInArchive);
			if (this.Verbose)
			{
				this.StatusMessageTextWriter.WriteLine("adding {0}...", fileName);
			}
			return this._InternalAddEntry(ze);
		}

		public void RemoveEntries(ICollection<ZipEntry> entriesToRemove)
		{
			if (entriesToRemove == null)
			{
				throw new ArgumentNullException("entriesToRemove");
			}
			foreach (ZipEntry current in entriesToRemove)
			{
				this.RemoveEntry(current);
			}
		}

		public void RemoveEntries(ICollection<string> entriesToRemove)
		{
			if (entriesToRemove == null)
			{
				throw new ArgumentNullException("entriesToRemove");
			}
			foreach (string current in entriesToRemove)
			{
				this.RemoveEntry(current);
			}
		}

		public void AddFiles(IEnumerable<string> fileNames)
		{
			this.AddFiles(fileNames, null);
		}

		public void UpdateFiles(IEnumerable<string> fileNames)
		{
			this.UpdateFiles(fileNames, null);
		}

		public void AddFiles(IEnumerable<string> fileNames, string directoryPathInArchive)
		{
			this.AddFiles(fileNames, false, directoryPathInArchive);
		}

		public void AddFiles(IEnumerable<string> fileNames, bool preserveDirHierarchy, string directoryPathInArchive)
		{
			if (fileNames == null)
			{
				throw new ArgumentNullException("fileNames");
			}
			this._addOperationCanceled = false;
			this.OnAddStarted();
			if (preserveDirHierarchy)
			{
				foreach (string current in fileNames)
				{
					if (this._addOperationCanceled)
					{
						break;
					}
					if (directoryPathInArchive != null)
					{
						string fullPath = Path.GetFullPath(Path.Combine(directoryPathInArchive, Path.GetDirectoryName(current)));
						this.AddFile(current, fullPath);
					}
					else
					{
						this.AddFile(current, null);
					}
				}
			}
			else
			{
				foreach (string current2 in fileNames)
				{
					if (this._addOperationCanceled)
					{
						break;
					}
					this.AddFile(current2, directoryPathInArchive);
				}
			}
			if (!this._addOperationCanceled)
			{
				this.OnAddCompleted();
			}
		}

		public void UpdateFiles(IEnumerable<string> fileNames, string directoryPathInArchive)
		{
			if (fileNames == null)
			{
				throw new ArgumentNullException("fileNames");
			}
			this.OnAddStarted();
			foreach (string current in fileNames)
			{
				this.UpdateFile(current, directoryPathInArchive);
			}
			this.OnAddCompleted();
		}

		public ZipEntry UpdateFile(string fileName)
		{
			return this.UpdateFile(fileName, null);
		}

		public ZipEntry UpdateFile(string fileName, string directoryPathInArchive)
		{
			string fileName2 = ZipEntry.NameInArchive(fileName, directoryPathInArchive);
			if (this[fileName2] != null)
			{
				this.RemoveEntry(fileName2);
			}
			return this.AddFile(fileName, directoryPathInArchive);
		}

		public ZipEntry UpdateDirectory(string directoryName)
		{
			return this.UpdateDirectory(directoryName, null);
		}

		public ZipEntry UpdateDirectory(string directoryName, string directoryPathInArchive)
		{
			return this.AddOrUpdateDirectoryImpl(directoryName, directoryPathInArchive, AddOrUpdateAction.AddOrUpdate);
		}

		public void UpdateItem(string itemName)
		{
			this.UpdateItem(itemName, null);
		}

		public void UpdateItem(string itemName, string directoryPathInArchive)
		{
			if (File.Exists(itemName))
			{
				this.UpdateFile(itemName, directoryPathInArchive);
			}
			else
			{
				if (!Directory.Exists(itemName))
				{
					throw new FileNotFoundException(string.Format("That file or directory ({0}) does not exist!", itemName));
				}
				this.UpdateDirectory(itemName, directoryPathInArchive);
			}
		}

		public ZipEntry AddEntry(string entryName, string content)
		{
			return this.AddEntry(entryName, content, Encoding.UTF8);
		}

		public ZipEntry AddEntry(string entryName, string content, Encoding encoding)
		{
			MemoryStream memoryStream = new MemoryStream();
			StreamWriter streamWriter = new StreamWriter(memoryStream, encoding);
			streamWriter.Write(content);
			streamWriter.Flush();
			memoryStream.Seek(0L, SeekOrigin.Begin);
			return this.AddEntry(entryName, memoryStream);
		}

		public ZipEntry AddEntry(string entryName, Stream stream)
		{
			ZipEntry zipEntry = ZipEntry.CreateForStream(entryName, stream);
			zipEntry.SetEntryTimes(DateTime.Now, DateTime.Now, DateTime.Now);
			if (this.Verbose)
			{
				this.StatusMessageTextWriter.WriteLine("adding {0}...", entryName);
			}
			return this._InternalAddEntry(zipEntry);
		}

		public ZipEntry AddEntry(string entryName, WriteDelegate writer)
		{
			ZipEntry ze = ZipEntry.CreateForWriter(entryName, writer);
			if (this.Verbose)
			{
				this.StatusMessageTextWriter.WriteLine("adding {0}...", entryName);
			}
			return this._InternalAddEntry(ze);
		}

		public ZipEntry AddEntry(string entryName, OpenDelegate opener, CloseDelegate closer)
		{
			ZipEntry zipEntry = ZipEntry.CreateForJitStreamProvider(entryName, opener, closer);
			zipEntry.SetEntryTimes(DateTime.Now, DateTime.Now, DateTime.Now);
			if (this.Verbose)
			{
				this.StatusMessageTextWriter.WriteLine("adding {0}...", entryName);
			}
			return this._InternalAddEntry(zipEntry);
		}

		private ZipEntry _InternalAddEntry(ZipEntry ze)
		{
			ze._container = new ZipContainer(this);
			ze.CompressionMethod = this.CompressionMethod;
			ze.CompressionLevel = this.CompressionLevel;
			ze.ExtractExistingFile = this.ExtractExistingFile;
			ze.ZipErrorAction = this.ZipErrorAction;
			ze.SetCompression = this.SetCompression;
			ze.AlternateEncoding = this.AlternateEncoding;
			ze.AlternateEncodingUsage = this.AlternateEncodingUsage;
			ze.Password = this._Password;
			ze.Encryption = this.Encryption;
			ze.EmitTimesInWindowsFormatWhenSaving = this._emitNtfsTimes;
			ze.EmitTimesInUnixFormatWhenSaving = this._emitUnixTimes;
			this.InternalAddEntry(ze.FileName, ze);
			this.AfterAddEntry(ze);
			return ze;
		}

		public ZipEntry UpdateEntry(string entryName, string content)
		{
			return this.UpdateEntry(entryName, content, Encoding.UTF8);
		}

		public ZipEntry UpdateEntry(string entryName, string content, Encoding encoding)
		{
			this.RemoveEntryForUpdate(entryName);
			return this.AddEntry(entryName, content, encoding);
		}

		public ZipEntry UpdateEntry(string entryName, WriteDelegate writer)
		{
			this.RemoveEntryForUpdate(entryName);
			return this.AddEntry(entryName, writer);
		}

		public ZipEntry UpdateEntry(string entryName, OpenDelegate opener, CloseDelegate closer)
		{
			this.RemoveEntryForUpdate(entryName);
			return this.AddEntry(entryName, opener, closer);
		}

		public ZipEntry UpdateEntry(string entryName, Stream stream)
		{
			this.RemoveEntryForUpdate(entryName);
			return this.AddEntry(entryName, stream);
		}

		private void RemoveEntryForUpdate(string entryName)
		{
			if (string.IsNullOrEmpty(entryName))
			{
				throw new ArgumentNullException("entryName");
			}
			string directoryPathInArchive = null;
			if (entryName.IndexOf('\\') != -1)
			{
				directoryPathInArchive = Path.GetDirectoryName(entryName);
				entryName = Path.GetFileName(entryName);
			}
			string fileName = ZipEntry.NameInArchive(entryName, directoryPathInArchive);
			if (this[fileName] != null)
			{
				this.RemoveEntry(fileName);
			}
		}

		public ZipEntry AddEntry(string entryName, byte[] byteContent)
		{
			if (byteContent == null)
			{
				throw new ArgumentException("bad argument", "byteContent");
			}
			MemoryStream stream = new MemoryStream(byteContent);
			return this.AddEntry(entryName, stream);
		}

		public ZipEntry UpdateEntry(string entryName, byte[] byteContent)
		{
			this.RemoveEntryForUpdate(entryName);
			return this.AddEntry(entryName, byteContent);
		}

		public ZipEntry AddDirectory(string directoryName)
		{
			return this.AddDirectory(directoryName, null);
		}

		public ZipEntry AddDirectory(string directoryName, string directoryPathInArchive)
		{
			return this.AddOrUpdateDirectoryImpl(directoryName, directoryPathInArchive, AddOrUpdateAction.AddOnly);
		}

		public ZipEntry AddDirectoryByName(string directoryNameInArchive)
		{
			ZipEntry zipEntry = ZipEntry.CreateFromNothing(directoryNameInArchive);
			zipEntry._container = new ZipContainer(this);
			zipEntry.MarkAsDirectory();
			zipEntry.AlternateEncoding = this.AlternateEncoding;
			zipEntry.AlternateEncodingUsage = this.AlternateEncodingUsage;
			zipEntry.SetEntryTimes(DateTime.Now, DateTime.Now, DateTime.Now);
			zipEntry.EmitTimesInWindowsFormatWhenSaving = this._emitNtfsTimes;
			zipEntry.EmitTimesInUnixFormatWhenSaving = this._emitUnixTimes;
			zipEntry._Source = ZipEntrySource.Stream;
			this.InternalAddEntry(zipEntry.FileName, zipEntry);
			this.AfterAddEntry(zipEntry);
			return zipEntry;
		}

		private ZipEntry AddOrUpdateDirectoryImpl(string directoryName, string rootDirectoryPathInArchive, AddOrUpdateAction action)
		{
			if (rootDirectoryPathInArchive == null)
			{
				rootDirectoryPathInArchive = string.Empty;
			}
			return this.AddOrUpdateDirectoryImpl(directoryName, rootDirectoryPathInArchive, action, true, 0);
		}

		internal void InternalAddEntry(string name, ZipEntry entry)
		{
			this._entries.Add(name, entry);
			this._zipEntriesAsList = null;
			this._contentsChanged = true;
		}

		private ZipEntry AddOrUpdateDirectoryImpl(string directoryName, string rootDirectoryPathInArchive, AddOrUpdateAction action, bool recurse, int level)
		{
			if (this.Verbose)
			{
				this.StatusMessageTextWriter.WriteLine("{0} {1}...", (action != AddOrUpdateAction.AddOnly) ? "Adding or updating" : "adding", directoryName);
			}
			if (level == 0)
			{
				this._addOperationCanceled = false;
				this.OnAddStarted();
			}
			if (this._addOperationCanceled)
			{
				return null;
			}
			string text = rootDirectoryPathInArchive;
			ZipEntry zipEntry = null;
			if (level > 0)
			{
				int num = directoryName.Length;
				for (int i = level; i > 0; i--)
				{
					num = directoryName.LastIndexOfAny("/\\".ToCharArray(), num - 1, num - 1);
				}
				text = directoryName.Substring(num + 1);
				text = Path.Combine(rootDirectoryPathInArchive, text);
			}
			if (level > 0 || rootDirectoryPathInArchive != string.Empty)
			{
				zipEntry = ZipEntry.CreateFromFile(directoryName, text);
				zipEntry._container = new ZipContainer(this);
				zipEntry.AlternateEncoding = this.AlternateEncoding;
				zipEntry.AlternateEncodingUsage = this.AlternateEncodingUsage;
				zipEntry.MarkAsDirectory();
				zipEntry.EmitTimesInWindowsFormatWhenSaving = this._emitNtfsTimes;
				zipEntry.EmitTimesInUnixFormatWhenSaving = this._emitUnixTimes;
				if (!this._entries.ContainsKey(zipEntry.FileName))
				{
					this.InternalAddEntry(zipEntry.FileName, zipEntry);
					this.AfterAddEntry(zipEntry);
				}
				text = zipEntry.FileName;
			}
			if (!this._addOperationCanceled)
			{
				string[] files = Directory.GetFiles(directoryName);
				if (recurse)
				{
					string[] array = files;
					for (int j = 0; j < array.Length; j++)
					{
						string fileName = array[j];
						if (this._addOperationCanceled)
						{
							break;
						}
						if (action == AddOrUpdateAction.AddOnly)
						{
							this.AddFile(fileName, text);
						}
						else
						{
							this.UpdateFile(fileName, text);
						}
					}
					if (!this._addOperationCanceled)
					{
						string[] directories = Directory.GetDirectories(directoryName);
						string[] array2 = directories;
						for (int k = 0; k < array2.Length; k++)
						{
							string directoryName2 = array2[k];
							if (this.AddDirectoryWillTraverseReparsePoints)
							{
								this.AddOrUpdateDirectoryImpl(directoryName2, rootDirectoryPathInArchive, action, recurse, level + 1);
							}
						}
					}
				}
			}
			if (level == 0)
			{
				this.OnAddCompleted();
			}
			return zipEntry;
		}

		public static bool CheckZip(string zipFileName)
		{
			return ZipFile.CheckZip(zipFileName, false, null);
		}

		public static bool CheckZip(string zipFileName, bool fixIfNecessary, TextWriter writer)
		{
			ZipFile zipFile = null;
			ZipFile zipFile2 = null;
			bool flag = true;
			try
			{
				zipFile = new ZipFile();
				zipFile.FullScan = true;
				zipFile.Initialize(zipFileName);
				zipFile2 = ZipFile.Read(zipFileName);
				foreach (ZipEntry current in zipFile)
				{
					foreach (ZipEntry current2 in zipFile2)
					{
						if (current.FileName == current2.FileName)
						{
							if (current._RelativeOffsetOfLocalHeader != current2._RelativeOffsetOfLocalHeader)
							{
								flag = false;
								if (writer != null)
								{
									writer.WriteLine("{0}: mismatch in RelativeOffsetOfLocalHeader  (0x{1:X16} != 0x{2:X16})", current.FileName, current._RelativeOffsetOfLocalHeader, current2._RelativeOffsetOfLocalHeader);
								}
							}
							if (current._CompressedSize != current2._CompressedSize)
							{
								flag = false;
								if (writer != null)
								{
									writer.WriteLine("{0}: mismatch in CompressedSize  (0x{1:X16} != 0x{2:X16})", current.FileName, current._CompressedSize, current2._CompressedSize);
								}
							}
							if (current._UncompressedSize != current2._UncompressedSize)
							{
								flag = false;
								if (writer != null)
								{
									writer.WriteLine("{0}: mismatch in UncompressedSize  (0x{1:X16} != 0x{2:X16})", current.FileName, current._UncompressedSize, current2._UncompressedSize);
								}
							}
							if (current.CompressionMethod != current2.CompressionMethod)
							{
								flag = false;
								if (writer != null)
								{
									writer.WriteLine("{0}: mismatch in CompressionMethod  (0x{1:X4} != 0x{2:X4})", current.FileName, current.CompressionMethod, current2.CompressionMethod);
								}
							}
							if (current.Crc != current2.Crc)
							{
								flag = false;
								if (writer != null)
								{
									writer.WriteLine("{0}: mismatch in Crc32  (0x{1:X4} != 0x{2:X4})", current.FileName, current.Crc, current2.Crc);
								}
							}
							break;
						}
					}
				}
				zipFile2.Dispose();
				zipFile2 = null;
				if (!flag && fixIfNecessary)
				{
					string text = Path.GetFileNameWithoutExtension(zipFileName);
					text = string.Format("{0}_fixed.zip", text);
					zipFile.Save(text);
				}
			}
			finally
			{
				if (zipFile != null)
				{
					zipFile.Dispose();
				}
				if (zipFile2 != null)
				{
					zipFile2.Dispose();
				}
			}
			return flag;
		}

		public static void FixZipDirectory(string zipFileName)
		{
			using (ZipFile zipFile = new ZipFile())
			{
				zipFile.FullScan = true;
				zipFile.Initialize(zipFileName);
				zipFile.Save(zipFileName);
			}
		}

		public static bool CheckZipPassword(string zipFileName, string password)
		{
			bool result = false;
			try
			{
				using (ZipFile zipFile = ZipFile.Read(zipFileName))
				{
					foreach (ZipEntry current in zipFile)
					{
						if (!current.IsDirectory && current.UsesEncryption)
						{
							current.ExtractWithPassword(Stream.Null, password);
						}
					}
				}
				result = true;
			}
			catch (BadPasswordException)
			{
			}
			return result;
		}

		internal bool OnSaveBlock(ZipEntry entry, long bytesXferred, long totalBytesToXfer)
		{
			EventHandler<SaveProgressEventArgs> saveProgress = this.SaveProgress;
			if (saveProgress != null)
			{
				SaveProgressEventArgs saveProgressEventArgs = SaveProgressEventArgs.ByteUpdate(this.ArchiveNameForEvent, entry, bytesXferred, totalBytesToXfer);
				saveProgress(this, saveProgressEventArgs);
				if (saveProgressEventArgs.Cancel)
				{
					this._saveOperationCanceled = true;
				}
			}
			return this._saveOperationCanceled;
		}

		private void OnSaveEntry(int current, ZipEntry entry, bool before)
		{
			EventHandler<SaveProgressEventArgs> saveProgress = this.SaveProgress;
			if (saveProgress != null)
			{
				SaveProgressEventArgs saveProgressEventArgs = new SaveProgressEventArgs(this.ArchiveNameForEvent, before, this._entries.Count, current, entry);
				saveProgress(this, saveProgressEventArgs);
				if (saveProgressEventArgs.Cancel)
				{
					this._saveOperationCanceled = true;
				}
			}
		}

		private void OnSaveEvent(ZipProgressEventType eventFlavor)
		{
			EventHandler<SaveProgressEventArgs> saveProgress = this.SaveProgress;
			if (saveProgress != null)
			{
				SaveProgressEventArgs saveProgressEventArgs = new SaveProgressEventArgs(this.ArchiveNameForEvent, eventFlavor);
				saveProgress(this, saveProgressEventArgs);
				if (saveProgressEventArgs.Cancel)
				{
					this._saveOperationCanceled = true;
				}
			}
		}

		private void OnSaveStarted()
		{
			EventHandler<SaveProgressEventArgs> saveProgress = this.SaveProgress;
			if (saveProgress != null)
			{
				SaveProgressEventArgs saveProgressEventArgs = SaveProgressEventArgs.Started(this.ArchiveNameForEvent);
				saveProgress(this, saveProgressEventArgs);
				if (saveProgressEventArgs.Cancel)
				{
					this._saveOperationCanceled = true;
				}
			}
		}

		private void OnSaveCompleted()
		{
			EventHandler<SaveProgressEventArgs> saveProgress = this.SaveProgress;
			if (saveProgress != null)
			{
				SaveProgressEventArgs e = SaveProgressEventArgs.Completed(this.ArchiveNameForEvent);
				saveProgress(this, e);
			}
		}

		private void OnReadStarted()
		{
			EventHandler<ReadProgressEventArgs> readProgress = this.ReadProgress;
			if (readProgress != null)
			{
				ReadProgressEventArgs e = ReadProgressEventArgs.Started(this.ArchiveNameForEvent);
				readProgress(this, e);
			}
		}

		private void OnReadCompleted()
		{
			EventHandler<ReadProgressEventArgs> readProgress = this.ReadProgress;
			if (readProgress != null)
			{
				ReadProgressEventArgs e = ReadProgressEventArgs.Completed(this.ArchiveNameForEvent);
				readProgress(this, e);
			}
		}

		internal void OnReadBytes(ZipEntry entry)
		{
			EventHandler<ReadProgressEventArgs> readProgress = this.ReadProgress;
			if (readProgress != null)
			{
				ReadProgressEventArgs e = ReadProgressEventArgs.ByteUpdate(this.ArchiveNameForEvent, entry, this.ReadStream.Position, this.LengthOfReadStream);
				readProgress(this, e);
			}
		}

		internal void OnReadEntry(bool before, ZipEntry entry)
		{
			EventHandler<ReadProgressEventArgs> readProgress = this.ReadProgress;
			if (readProgress != null)
			{
				ReadProgressEventArgs e = (!before) ? ReadProgressEventArgs.After(this.ArchiveNameForEvent, entry, this._entries.Count) : ReadProgressEventArgs.Before(this.ArchiveNameForEvent, this._entries.Count);
				readProgress(this, e);
			}
		}

		private void OnExtractEntry(int current, bool before, ZipEntry currentEntry, string path)
		{
			EventHandler<ExtractProgressEventArgs> extractProgress = this.ExtractProgress;
			if (extractProgress != null)
			{
				ExtractProgressEventArgs extractProgressEventArgs = new ExtractProgressEventArgs(this.ArchiveNameForEvent, before, this._entries.Count, current, currentEntry, path);
				extractProgress(this, extractProgressEventArgs);
				if (extractProgressEventArgs.Cancel)
				{
					this._extractOperationCanceled = true;
				}
			}
		}

		internal bool OnExtractBlock(ZipEntry entry, long bytesWritten, long totalBytesToWrite)
		{
			EventHandler<ExtractProgressEventArgs> extractProgress = this.ExtractProgress;
			if (extractProgress != null)
			{
				ExtractProgressEventArgs extractProgressEventArgs = ExtractProgressEventArgs.ByteUpdate(this.ArchiveNameForEvent, entry, bytesWritten, totalBytesToWrite);
				extractProgress(this, extractProgressEventArgs);
				if (extractProgressEventArgs.Cancel)
				{
					this._extractOperationCanceled = true;
				}
			}
			return this._extractOperationCanceled;
		}

		internal bool OnSingleEntryExtract(ZipEntry entry, string path, bool before)
		{
			EventHandler<ExtractProgressEventArgs> extractProgress = this.ExtractProgress;
			if (extractProgress != null)
			{
				ExtractProgressEventArgs extractProgressEventArgs = (!before) ? ExtractProgressEventArgs.AfterExtractEntry(this.ArchiveNameForEvent, entry, path) : ExtractProgressEventArgs.BeforeExtractEntry(this.ArchiveNameForEvent, entry, path);
				extractProgress(this, extractProgressEventArgs);
				if (extractProgressEventArgs.Cancel)
				{
					this._extractOperationCanceled = true;
				}
			}
			return this._extractOperationCanceled;
		}

		internal bool OnExtractExisting(ZipEntry entry, string path)
		{
			EventHandler<ExtractProgressEventArgs> extractProgress = this.ExtractProgress;
			if (extractProgress != null)
			{
				ExtractProgressEventArgs extractProgressEventArgs = ExtractProgressEventArgs.ExtractExisting(this.ArchiveNameForEvent, entry, path);
				extractProgress(this, extractProgressEventArgs);
				if (extractProgressEventArgs.Cancel)
				{
					this._extractOperationCanceled = true;
				}
			}
			return this._extractOperationCanceled;
		}

		private void OnExtractAllCompleted(string path)
		{
			EventHandler<ExtractProgressEventArgs> extractProgress = this.ExtractProgress;
			if (extractProgress != null)
			{
				ExtractProgressEventArgs e = ExtractProgressEventArgs.ExtractAllCompleted(this.ArchiveNameForEvent, path);
				extractProgress(this, e);
			}
		}

		private void OnExtractAllStarted(string path)
		{
			EventHandler<ExtractProgressEventArgs> extractProgress = this.ExtractProgress;
			if (extractProgress != null)
			{
				ExtractProgressEventArgs e = ExtractProgressEventArgs.ExtractAllStarted(this.ArchiveNameForEvent, path);
				extractProgress(this, e);
			}
		}

		private void OnAddStarted()
		{
			EventHandler<AddProgressEventArgs> addProgress = this.AddProgress;
			if (addProgress != null)
			{
				AddProgressEventArgs addProgressEventArgs = AddProgressEventArgs.Started(this.ArchiveNameForEvent);
				addProgress(this, addProgressEventArgs);
				if (addProgressEventArgs.Cancel)
				{
					this._addOperationCanceled = true;
				}
			}
		}

		private void OnAddCompleted()
		{
			EventHandler<AddProgressEventArgs> addProgress = this.AddProgress;
			if (addProgress != null)
			{
				AddProgressEventArgs e = AddProgressEventArgs.Completed(this.ArchiveNameForEvent);
				addProgress(this, e);
			}
		}

		internal void AfterAddEntry(ZipEntry entry)
		{
			EventHandler<AddProgressEventArgs> addProgress = this.AddProgress;
			if (addProgress != null)
			{
				AddProgressEventArgs addProgressEventArgs = AddProgressEventArgs.AfterEntry(this.ArchiveNameForEvent, entry, this._entries.Count);
				addProgress(this, addProgressEventArgs);
				if (addProgressEventArgs.Cancel)
				{
					this._addOperationCanceled = true;
				}
			}
		}

		internal bool OnZipErrorSaving(ZipEntry entry, Exception exc)
		{
			if (this.ZipError != null)
			{
				object lOCK = this.LOCK;
				lock (lOCK)
				{
					ZipErrorEventArgs zipErrorEventArgs = ZipErrorEventArgs.Saving(this.Name, entry, exc);
					this.ZipError(this, zipErrorEventArgs);
					if (zipErrorEventArgs.Cancel)
					{
						this._saveOperationCanceled = true;
					}
				}
			}
			return this._saveOperationCanceled;
		}

		public void ExtractAll(string path)
		{
			this._InternalExtractAll(path, true);
		}

		public void ExtractAll(string path, ExtractExistingFileAction extractExistingFile)
		{
			this.ExtractExistingFile = extractExistingFile;
			this._InternalExtractAll(path, true);
		}

		private void _InternalExtractAll(string path, bool overrideExtractExistingProperty)
		{
			bool flag = this.Verbose;
			this._inExtractAll = true;
			try
			{
				this.OnExtractAllStarted(path);
				int num = 0;
				foreach (ZipEntry current in this._entries.Values)
				{
					if (flag)
					{
						this.StatusMessageTextWriter.WriteLine("\n{1,-22} {2,-8} {3,4}   {4,-8}  {0}", new object[]
						{
							"Name",
							"Modified",
							"Size",
							"Ratio",
							"Packed"
						});
						this.StatusMessageTextWriter.WriteLine(new string('-', 72));
						flag = false;
					}
					if (this.Verbose)
					{
						this.StatusMessageTextWriter.WriteLine("{1,-22} {2,-8} {3,4:F0}%   {4,-8} {0}", new object[]
						{
							current.FileName,
							current.LastModified.ToString("yyyy-MM-dd HH:mm:ss"),
							current.UncompressedSize,
							current.CompressionRatio,
							current.CompressedSize
						});
						if (!string.IsNullOrEmpty(current.Comment))
						{
							this.StatusMessageTextWriter.WriteLine("  Comment: {0}", current.Comment);
						}
					}
					current.Password = this._Password;
					this.OnExtractEntry(num, true, current, path);
					if (overrideExtractExistingProperty)
					{
						current.ExtractExistingFile = this.ExtractExistingFile;
					}
					current.Extract(path);
					num++;
					this.OnExtractEntry(num, false, current, path);
					if (this._extractOperationCanceled)
					{
						break;
					}
				}
				if (!this._extractOperationCanceled)
				{
					foreach (ZipEntry current2 in this._entries.Values)
					{
						if (current2.IsDirectory || current2.FileName.EndsWith("/"))
						{
							string fileOrDirectory = (!current2.FileName.StartsWith("/")) ? Path.Combine(path, current2.FileName) : Path.Combine(path, current2.FileName.Substring(1));
							current2._SetTimes(fileOrDirectory, false);
						}
					}
					this.OnExtractAllCompleted(path);
				}
			}
			finally
			{
				this._inExtractAll = false;
			}
		}

		public static ZipFile Read(string fileName)
		{
			return ZipFile.Read(fileName, null, null, null);
		}

		public static ZipFile Read(string fileName, ReadOptions options)
		{
			if (options == null)
			{
				throw new ArgumentNullException("options");
			}
			return ZipFile.Read(fileName, options.StatusMessageWriter, options.Encoding, options.ReadProgress);
		}

		private static ZipFile Read(string fileName, TextWriter statusMessageWriter, Encoding encoding, EventHandler<ReadProgressEventArgs> readProgress)
		{
			ZipFile zipFile = new ZipFile();
			zipFile.AlternateEncoding = (encoding ?? ZipFile.DefaultEncoding);
			zipFile.AlternateEncodingUsage = ZipOption.Always;
			zipFile._StatusMessageTextWriter = statusMessageWriter;
			zipFile._name = fileName;
			if (readProgress != null)
			{
				zipFile.ReadProgress = readProgress;
			}
			if (zipFile.Verbose)
			{
				zipFile._StatusMessageTextWriter.WriteLine("reading from {0}...", fileName);
			}
			ZipFile.ReadIntoInstance(zipFile);
			zipFile._fileAlreadyExists = true;
			return zipFile;
		}

		public static ZipFile Read(Stream zipStream)
		{
			return ZipFile.Read(zipStream, null, null, null);
		}

		public static ZipFile Read(Stream zipStream, ReadOptions options)
		{
			if (options == null)
			{
				throw new ArgumentNullException("options");
			}
			return ZipFile.Read(zipStream, options.StatusMessageWriter, options.Encoding, options.ReadProgress);
		}

		private static ZipFile Read(Stream zipStream, TextWriter statusMessageWriter, Encoding encoding, EventHandler<ReadProgressEventArgs> readProgress)
		{
			if (zipStream == null)
			{
				throw new ArgumentNullException("zipStream");
			}
			ZipFile zipFile = new ZipFile();
			zipFile._StatusMessageTextWriter = statusMessageWriter;
			zipFile._alternateEncoding = (encoding ?? ZipFile.DefaultEncoding);
			zipFile._alternateEncodingUsage = ZipOption.Always;
			if (readProgress != null)
			{
				zipFile.ReadProgress += readProgress;
			}
			zipFile._readstream = ((zipStream.Position != 0L) ? new OffsetStream(zipStream) : zipStream);
			zipFile._ReadStreamIsOurs = false;
			if (zipFile.Verbose)
			{
				zipFile._StatusMessageTextWriter.WriteLine("reading from stream...");
			}
			ZipFile.ReadIntoInstance(zipFile);
			return zipFile;
		}

		private static void ReadIntoInstance(ZipFile zf)
		{
			Stream readStream = zf.ReadStream;
			try
			{
				zf._readName = zf._name;
				if (!readStream.CanSeek)
				{
					ZipFile.ReadIntoInstance_Orig(zf);
					return;
				}
				zf.OnReadStarted();
				uint num = ZipFile.ReadFirstFourBytes(readStream);
				if (num == 101010256u)
				{
					return;
				}
				int num2 = 0;
				bool flag = false;
				long num3 = readStream.Length - 64L;
				long num4 = Math.Max(readStream.Length - 16384L, 10L);
				do
				{
					if (num3 < 0L)
					{
						num3 = 0L;
					}
					readStream.Seek(num3, SeekOrigin.Begin);
					long num5 = SharedUtilities.FindSignature(readStream, 101010256);
					if (num5 != -1L)
					{
						flag = true;
					}
					else
					{
						if (num3 == 0L)
						{
							break;
						}
						num2++;
						num3 -= (long)(32 * (num2 + 1) * num2);
					}
				}
				while (!flag && num3 > num4);
				if (flag)
				{
					zf._locEndOfCDS = readStream.Position - 4L;
					byte[] array = new byte[16];
					readStream.Read(array, 0, array.Length);
					zf._diskNumberWithCd = (uint)BitConverter.ToUInt16(array, 2);
					if (zf._diskNumberWithCd == 65535u)
					{
						throw new ZipException("Spanned archives with more than 65534 segments are not supported at this time.");
					}
					zf._diskNumberWithCd += 1u;
					int startIndex = 12;
					uint num6 = BitConverter.ToUInt32(array, startIndex);
					if (num6 == 4294967295u)
					{
						ZipFile.Zip64SeekToCentralDirectory(zf);
					}
					else
					{
						zf._OffsetOfCentralDirectory = num6;
						readStream.Seek((long)((ulong)num6), SeekOrigin.Begin);
					}
					ZipFile.ReadCentralDirectory(zf);
				}
				else
				{
					readStream.Seek(0L, SeekOrigin.Begin);
					ZipFile.ReadIntoInstance_Orig(zf);
				}
			}
			catch (Exception innerException)
			{
				if (zf._ReadStreamIsOurs && zf._readstream != null)
				{
					try
					{
						zf._readstream.Dispose();
						zf._readstream = null;
					}
					finally
					{
					}
				}
				throw new ZipException("Cannot read that as a ZipFile", innerException);
			}
			zf._contentsChanged = false;
		}

		private static void Zip64SeekToCentralDirectory(ZipFile zf)
		{
			Stream readStream = zf.ReadStream;
			byte[] array = new byte[16];
			readStream.Seek(-40L, SeekOrigin.Current);
			readStream.Read(array, 0, 16);
			long num = BitConverter.ToInt64(array, 8);
			zf._OffsetOfCentralDirectory = 4294967295u;
			zf._OffsetOfCentralDirectory64 = num;
			readStream.Seek(num, SeekOrigin.Begin);
			uint num2 = (uint)SharedUtilities.ReadInt(readStream);
			if (num2 != 101075792u)
			{
				throw new BadReadException(string.Format("  Bad signature (0x{0:X8}) looking for ZIP64 EoCD Record at position 0x{1:X8}", num2, readStream.Position));
			}
			readStream.Read(array, 0, 8);
			long num3 = BitConverter.ToInt64(array, 0);
			array = new byte[num3];
			readStream.Read(array, 0, array.Length);
			num = BitConverter.ToInt64(array, 36);
			readStream.Seek(num, SeekOrigin.Begin);
		}

		private static uint ReadFirstFourBytes(Stream s)
		{
			return (uint)SharedUtilities.ReadInt(s);
		}

		private static void ReadCentralDirectory(ZipFile zf)
		{
			bool flag = false;
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			ZipEntry zipEntry;
			while ((zipEntry = ZipEntry.ReadDirEntry(zf, dictionary)) != null)
			{
				zipEntry.ResetDirEntry();
				zf.OnReadEntry(true, null);
				if (zf.Verbose)
				{
					zf.StatusMessageTextWriter.WriteLine("entry {0}", zipEntry.FileName);
				}
				zf._entries.Add(zipEntry.FileName, zipEntry);
				if (zipEntry._InputUsesZip64)
				{
					flag = true;
				}
				dictionary.Add(zipEntry.FileName, null);
			}
			if (flag)
			{
				zf.UseZip64WhenSaving = Zip64Option.Always;
			}
			if (zf._locEndOfCDS > 0L)
			{
				zf.ReadStream.Seek(zf._locEndOfCDS, SeekOrigin.Begin);
			}
			ZipFile.ReadCentralDirectoryFooter(zf);
			if (zf.Verbose && !string.IsNullOrEmpty(zf.Comment))
			{
				zf.StatusMessageTextWriter.WriteLine("Zip file Comment: {0}", zf.Comment);
			}
			if (zf.Verbose)
			{
				zf.StatusMessageTextWriter.WriteLine("read in {0} entries.", zf._entries.Count);
			}
			zf.OnReadCompleted();
		}

		private static void ReadIntoInstance_Orig(ZipFile zf)
		{
			zf.OnReadStarted();
			zf._entries = new Dictionary<string, ZipEntry>();
			if (zf.Verbose)
			{
				if (zf.Name == null)
				{
					zf.StatusMessageTextWriter.WriteLine("Reading zip from stream...");
				}
				else
				{
					zf.StatusMessageTextWriter.WriteLine("Reading zip {0}...", zf.Name);
				}
			}
			bool first = true;
			ZipContainer zc = new ZipContainer(zf);
			ZipEntry zipEntry;
			while ((zipEntry = ZipEntry.ReadEntry(zc, first)) != null)
			{
				if (zf.Verbose)
				{
					zf.StatusMessageTextWriter.WriteLine("  {0}", zipEntry.FileName);
				}
				zf._entries.Add(zipEntry.FileName, zipEntry);
				first = false;
			}
			try
			{
				Dictionary<string, object> dictionary = new Dictionary<string, object>();
				ZipEntry zipEntry2;
				while ((zipEntry2 = ZipEntry.ReadDirEntry(zf, dictionary)) != null)
				{
					ZipEntry zipEntry3 = zf._entries[zipEntry2.FileName];
					if (zipEntry3 != null)
					{
						zipEntry3._Comment = zipEntry2.Comment;
						if (zipEntry2.IsDirectory)
						{
							zipEntry3.MarkAsDirectory();
						}
					}
					dictionary.Add(zipEntry2.FileName, null);
				}
				if (zf._locEndOfCDS > 0L)
				{
					zf.ReadStream.Seek(zf._locEndOfCDS, SeekOrigin.Begin);
				}
				ZipFile.ReadCentralDirectoryFooter(zf);
				if (zf.Verbose && !string.IsNullOrEmpty(zf.Comment))
				{
					zf.StatusMessageTextWriter.WriteLine("Zip file Comment: {0}", zf.Comment);
				}
			}
			catch (ZipException)
			{
			}
			catch (IOException)
			{
			}
			zf.OnReadCompleted();
		}

		private static void ReadCentralDirectoryFooter(ZipFile zf)
		{
			Stream readStream = zf.ReadStream;
			int num = SharedUtilities.ReadSignature(readStream);
			int num2 = 0;
			byte[] array;
			if ((long)num == 101075792L)
			{
				array = new byte[52];
				readStream.Read(array, 0, array.Length);
				long num3 = BitConverter.ToInt64(array, 0);
				if (num3 < 44L)
				{
					throw new ZipException("Bad size in the ZIP64 Central Directory.");
				}
				zf._versionMadeBy = BitConverter.ToUInt16(array, num2);
				num2 += 2;
				zf._versionNeededToExtract = BitConverter.ToUInt16(array, num2);
				num2 += 2;
				zf._diskNumberWithCd = BitConverter.ToUInt32(array, num2);
				num2 += 2;
				array = new byte[num3 - 44L];
				readStream.Read(array, 0, array.Length);
				num = SharedUtilities.ReadSignature(readStream);
				if ((long)num != 117853008L)
				{
					throw new ZipException("Inconsistent metadata in the ZIP64 Central Directory.");
				}
				array = new byte[16];
				readStream.Read(array, 0, array.Length);
				num = SharedUtilities.ReadSignature(readStream);
			}
			if ((long)num != 101010256L)
			{
				readStream.Seek(-4L, SeekOrigin.Current);
				throw new BadReadException(string.Format("Bad signature ({0:X8}) at position 0x{1:X8}", num, readStream.Position));
			}
			array = new byte[16];
			zf.ReadStream.Read(array, 0, array.Length);
			if (zf._diskNumberWithCd == 0u)
			{
				zf._diskNumberWithCd = (uint)BitConverter.ToUInt16(array, 2);
			}
			ZipFile.ReadZipFileComment(zf);
		}

		private static void ReadZipFileComment(ZipFile zf)
		{
			byte[] array = new byte[2];
			zf.ReadStream.Read(array, 0, array.Length);
			short num = (short)((int)array[0] + (int)array[1] * 256);
			if (num > 0)
			{
				array = new byte[(int)num];
				zf.ReadStream.Read(array, 0, array.Length);
				string @string = zf.AlternateEncoding.GetString(array, 0, array.Length);
				zf.Comment = @string;
			}
		}

		public static bool IsZipFile(string fileName)
		{
			return ZipFile.IsZipFile(fileName, false);
		}

		public static bool IsZipFile(string fileName, bool testExtract)
		{
			bool result = false;
			try
			{
				if (!File.Exists(fileName))
				{
					return false;
				}
				using (FileStream fileStream = File.Open(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
				{
					result = ZipFile.IsZipFile(fileStream, testExtract);
				}
			}
			catch (IOException)
			{
			}
			catch (ZipException)
			{
			}
			return result;
		}

		public static bool IsZipFile(Stream stream, bool testExtract)
		{
			if (stream == null)
			{
				throw new ArgumentNullException("stream");
			}
			bool result = false;
			try
			{
				if (!stream.CanRead)
				{
					return false;
				}
				Stream @null = Stream.Null;
				using (ZipFile zipFile = ZipFile.Read(stream, null, null, null))
				{
					if (testExtract)
					{
						foreach (ZipEntry current in zipFile)
						{
							if (!current.IsDirectory)
							{
								current.Extract(@null);
							}
						}
					}
				}
				result = true;
			}
			catch (IOException)
			{
			}
			catch (ZipException)
			{
			}
			return result;
		}

		private void DeleteFileWithRetry(string filename)
		{
			bool flag = false;
			int num = 3;
			int num2 = 0;
			while (num2 < num && !flag)
			{
				try
				{
					File.Delete(filename);
					flag = true;
				}
				catch (UnauthorizedAccessException)
				{
					Console.WriteLine("************************************************** Retry delete.");
					Thread.Sleep(200 + num2 * 200);
				}
				num2++;
			}
		}

		public void Save()
		{
			try
			{
				bool flag = false;
				this._saveOperationCanceled = false;
				this._numberOfSegmentsForMostRecentSave = 0u;
				this.OnSaveStarted();
				if (this.WriteStream == null)
				{
					throw new BadStateException("You haven't specified where to save the zip.");
				}
				if (this._name != null && this._name.EndsWith(".exe") && !this._SavingSfx)
				{
					throw new BadStateException("You specified an EXE for a plain zip file.");
				}
				if (!this._contentsChanged)
				{
					this.OnSaveCompleted();
					if (this.Verbose)
					{
						this.StatusMessageTextWriter.WriteLine("No save is necessary....");
					}
					return;
				}
				this.Reset(true);
				if (this.Verbose)
				{
					this.StatusMessageTextWriter.WriteLine("saving....");
				}
				if (this._entries.Count >= 65535 && this._zip64 == Zip64Option.Default)
				{
					throw new ZipException("The number of entries is 65535 or greater. Consider setting the UseZip64WhenSaving property on the ZipFile instance.");
				}
				int num = 0;
				ICollection<ZipEntry> arg_100_0;
				if (this.SortEntriesBeforeSaving)
				{
					ICollection<ZipEntry> entriesSorted = this.EntriesSorted;
					arg_100_0 = entriesSorted;
				}
				else
				{
					arg_100_0 = this.Entries;
				}
				ICollection<ZipEntry> collection = arg_100_0;
				foreach (ZipEntry current in collection)
				{
					this.OnSaveEntry(num, current, true);
					current.Write(this.WriteStream);
					if (this._saveOperationCanceled)
					{
						break;
					}
					num++;
					this.OnSaveEntry(num, current, false);
					if (this._saveOperationCanceled)
					{
						break;
					}
					if (current.IncludedInMostRecentSave)
					{
						flag |= current.OutputUsedZip64.Value;
					}
				}
				if (this._saveOperationCanceled)
				{
					return;
				}
				ZipSegmentedStream zipSegmentedStream = this.WriteStream as ZipSegmentedStream;
				this._numberOfSegmentsForMostRecentSave = ((zipSegmentedStream == null) ? 1u : zipSegmentedStream.CurrentSegment);
				bool flag2 = ZipOutput.WriteCentralDirectoryStructure(this.WriteStream, collection, this._numberOfSegmentsForMostRecentSave, this._zip64, this.Comment, new ZipContainer(this));
				this.OnSaveEvent(ZipProgressEventType.Saving_AfterSaveTempArchive);
				this._hasBeenSaved = true;
				this._contentsChanged = false;
				flag |= flag2;
				this._OutputUsesZip64 = new bool?(flag);
				if (this._name != null && (this._temporaryFileName != null || zipSegmentedStream != null))
				{
					this.WriteStream.Dispose();
					if (this._saveOperationCanceled)
					{
						return;
					}
					if (this._fileAlreadyExists && this._readstream != null)
					{
						this._readstream.Close();
						this._readstream = null;
						foreach (ZipEntry current2 in collection)
						{
							ZipSegmentedStream zipSegmentedStream2 = current2._archiveStream as ZipSegmentedStream;
							if (zipSegmentedStream2 != null)
							{
								zipSegmentedStream2.Dispose();
							}
							current2._archiveStream = null;
						}
					}
					string text = null;
					if (File.Exists(this._name))
					{
						text = this._name + "." + SharedUtilities.GenerateRandomStringImpl(8, 0) + ".tmp";
						if (File.Exists(text))
						{
							this.DeleteFileWithRetry(text);
						}
						File.Move(this._name, text);
					}
					this.OnSaveEvent(ZipProgressEventType.Saving_BeforeRenameTempArchive);
					File.Move((zipSegmentedStream == null) ? this._temporaryFileName : zipSegmentedStream.CurrentTempName, this._name);
					this.OnSaveEvent(ZipProgressEventType.Saving_AfterRenameTempArchive);
					if (text != null)
					{
						try
						{
							if (File.Exists(text))
							{
								File.Delete(text);
							}
						}
						catch
						{
						}
					}
					this._fileAlreadyExists = true;
				}
				ZipFile.NotifyEntriesSaveComplete(collection);
				this.OnSaveCompleted();
				this._JustSaved = true;
			}
			finally
			{
				this.CleanupAfterSaveOperation();
			}
		}

		private static void NotifyEntriesSaveComplete(ICollection<ZipEntry> c)
		{
			foreach (ZipEntry current in c)
			{
				current.NotifySaveComplete();
			}
		}

		private void RemoveTempFile()
		{
			try
			{
				if (File.Exists(this._temporaryFileName))
				{
					File.Delete(this._temporaryFileName);
				}
			}
			catch (IOException ex)
			{
				if (this.Verbose)
				{
					this.StatusMessageTextWriter.WriteLine("ZipFile::Save: could not delete temp file: {0}.", ex.Message);
				}
			}
		}

		private void CleanupAfterSaveOperation()
		{
			if (this._name != null)
			{
				if (this._writestream != null)
				{
					try
					{
						this._writestream.Dispose();
					}
					catch (IOException)
					{
					}
				}
				this._writestream = null;
				if (this._temporaryFileName != null)
				{
					this.RemoveTempFile();
					this._temporaryFileName = null;
				}
			}
		}

		public void Save(string fileName)
		{
			if (this._name == null)
			{
				this._writestream = null;
			}
			else
			{
				this._readName = this._name;
			}
			this._name = fileName;
			if (Directory.Exists(this._name))
			{
				throw new ZipException("Bad Directory", new ArgumentException("That name specifies an existing directory. Please specify a filename.", "fileName"));
			}
			this._contentsChanged = true;
			this._fileAlreadyExists = File.Exists(this._name);
			this.Save();
		}

		public void Save(Stream outputStream)
		{
			if (outputStream == null)
			{
				throw new ArgumentNullException("outputStream");
			}
			if (!outputStream.CanWrite)
			{
				throw new ArgumentException("Must be a writable stream.", "outputStream");
			}
			this._name = null;
			this._writestream = new CountingStream(outputStream);
			this._contentsChanged = true;
			this._fileAlreadyExists = false;
			this.Save();
		}

		public void AddSelectedFiles(string selectionCriteria)
		{
			this.AddSelectedFiles(selectionCriteria, ".", null, false);
		}

		public void AddSelectedFiles(string selectionCriteria, bool recurseDirectories)
		{
			this.AddSelectedFiles(selectionCriteria, ".", null, recurseDirectories);
		}

		public void AddSelectedFiles(string selectionCriteria, string directoryOnDisk)
		{
			this.AddSelectedFiles(selectionCriteria, directoryOnDisk, null, false);
		}

		public void AddSelectedFiles(string selectionCriteria, string directoryOnDisk, bool recurseDirectories)
		{
			this.AddSelectedFiles(selectionCriteria, directoryOnDisk, null, recurseDirectories);
		}

		public void AddSelectedFiles(string selectionCriteria, string directoryOnDisk, string directoryPathInArchive)
		{
			this.AddSelectedFiles(selectionCriteria, directoryOnDisk, directoryPathInArchive, false);
		}

		public void AddSelectedFiles(string selectionCriteria, string directoryOnDisk, string directoryPathInArchive, bool recurseDirectories)
		{
			this._AddOrUpdateSelectedFiles(selectionCriteria, directoryOnDisk, directoryPathInArchive, recurseDirectories, false);
		}

		public void UpdateSelectedFiles(string selectionCriteria, string directoryOnDisk, string directoryPathInArchive, bool recurseDirectories)
		{
			this._AddOrUpdateSelectedFiles(selectionCriteria, directoryOnDisk, directoryPathInArchive, recurseDirectories, true);
		}

		private string EnsureendInSlash(string s)
		{
			if (s.EndsWith("\\"))
			{
				return s;
			}
			return s + "\\";
		}

		private void _AddOrUpdateSelectedFiles(string selectionCriteria, string directoryOnDisk, string directoryPathInArchive, bool recurseDirectories, bool wantUpdate)
		{
			if (directoryOnDisk == null && Directory.Exists(selectionCriteria))
			{
				directoryOnDisk = selectionCriteria;
				selectionCriteria = "*.*";
			}
			else if (string.IsNullOrEmpty(directoryOnDisk))
			{
				directoryOnDisk = ".";
			}
			while (directoryOnDisk.EndsWith("\\"))
			{
				directoryOnDisk = directoryOnDisk.Substring(0, directoryOnDisk.Length - 1);
			}
			if (this.Verbose)
			{
				this.StatusMessageTextWriter.WriteLine("adding selection '{0}' from dir '{1}'...", selectionCriteria, directoryOnDisk);
			}
			FileSelector fileSelector = new FileSelector(selectionCriteria, this.AddDirectoryWillTraverseReparsePoints);
			ReadOnlyCollection<string> readOnlyCollection = fileSelector.SelectFiles(directoryOnDisk, recurseDirectories);
			if (this.Verbose)
			{
				this.StatusMessageTextWriter.WriteLine("found {0} files...", readOnlyCollection.Count);
			}
			this.OnAddStarted();
			AddOrUpdateAction action = (!wantUpdate) ? AddOrUpdateAction.AddOnly : AddOrUpdateAction.AddOrUpdate;
			foreach (string current in readOnlyCollection)
			{
				string text = (directoryPathInArchive != null) ? ZipFile.ReplaceLeadingDirectory(Path.GetDirectoryName(current), directoryOnDisk, directoryPathInArchive) : null;
				if (File.Exists(current))
				{
					if (wantUpdate)
					{
						this.UpdateFile(current, text);
					}
					else
					{
						this.AddFile(current, text);
					}
				}
				else
				{
					this.AddOrUpdateDirectoryImpl(current, text, action, false, 0);
				}
			}
			this.OnAddCompleted();
		}

		private static string ReplaceLeadingDirectory(string original, string pattern, string replacement)
		{
			string text = original.ToUpper();
			string text2 = pattern.ToUpper();
			int num = text.IndexOf(text2);
			if (num != 0)
			{
				return original;
			}
			return replacement + original.Substring(text2.Length);
		}

		public ICollection<ZipEntry> SelectEntries(string selectionCriteria)
		{
			FileSelector fileSelector = new FileSelector(selectionCriteria, this.AddDirectoryWillTraverseReparsePoints);
			return fileSelector.SelectEntries(this);
		}

		public ICollection<ZipEntry> SelectEntries(string selectionCriteria, string directoryPathInArchive)
		{
			FileSelector fileSelector = new FileSelector(selectionCriteria, this.AddDirectoryWillTraverseReparsePoints);
			return fileSelector.SelectEntries(this, directoryPathInArchive);
		}

		public int RemoveSelectedEntries(string selectionCriteria)
		{
			ICollection<ZipEntry> collection = this.SelectEntries(selectionCriteria);
			this.RemoveEntries(collection);
			return collection.Count;
		}

		public int RemoveSelectedEntries(string selectionCriteria, string directoryPathInArchive)
		{
			ICollection<ZipEntry> collection = this.SelectEntries(selectionCriteria, directoryPathInArchive);
			this.RemoveEntries(collection);
			return collection.Count;
		}

		public void ExtractSelectedEntries(string selectionCriteria)
		{
			foreach (ZipEntry current in this.SelectEntries(selectionCriteria))
			{
				current.Password = this._Password;
				current.Extract();
			}
		}

		public void ExtractSelectedEntries(string selectionCriteria, ExtractExistingFileAction extractExistingFile)
		{
			foreach (ZipEntry current in this.SelectEntries(selectionCriteria))
			{
				current.Password = this._Password;
				current.Extract(extractExistingFile);
			}
		}

		public void ExtractSelectedEntries(string selectionCriteria, string directoryPathInArchive)
		{
			foreach (ZipEntry current in this.SelectEntries(selectionCriteria, directoryPathInArchive))
			{
				current.Password = this._Password;
				current.Extract();
			}
		}

		public void ExtractSelectedEntries(string selectionCriteria, string directoryInArchive, string extractDirectory)
		{
			foreach (ZipEntry current in this.SelectEntries(selectionCriteria, directoryInArchive))
			{
				current.Password = this._Password;
				current.Extract(extractDirectory);
			}
		}

		public void ExtractSelectedEntries(string selectionCriteria, string directoryPathInArchive, string extractDirectory, ExtractExistingFileAction extractExistingFile)
		{
			foreach (ZipEntry current in this.SelectEntries(selectionCriteria, directoryPathInArchive))
			{
				current.Password = this._Password;
				current.Extract(extractDirectory, extractExistingFile);
			}
		}

		public bool ContainsEntry(string name)
		{
			return this._entries.ContainsKey(SharedUtilities.NormalizePathForUseInZipFile(name));
		}

		public override string ToString()
		{
			return string.Format("ZipFile::{0}", this.Name);
		}

		internal void NotifyEntryChanged()
		{
			this._contentsChanged = true;
		}

		internal Stream StreamForDiskNumber(uint diskNumber)
		{
			if (diskNumber + 1u == this._diskNumberWithCd || (diskNumber == 0u && this._diskNumberWithCd == 0u))
			{
				return this.ReadStream;
			}
			return ZipSegmentedStream.ForReading(this._readName ?? this._name, diskNumber, this._diskNumberWithCd);
		}

		internal void Reset(bool whileSaving)
		{
			if (this._JustSaved)
			{
				using (ZipFile zipFile = new ZipFile())
				{
					zipFile._readName = (zipFile._name = ((!whileSaving) ? this._name : (this._readName ?? this._name)));
					zipFile.AlternateEncoding = this.AlternateEncoding;
					zipFile.AlternateEncodingUsage = this.AlternateEncodingUsage;
					ZipFile.ReadIntoInstance(zipFile);
					foreach (ZipEntry current in zipFile)
					{
						foreach (ZipEntry current2 in this)
						{
							if (current.FileName == current2.FileName)
							{
								current2.CopyMetaData(current);
								break;
							}
						}
					}
				}
				this._JustSaved = false;
			}
		}

		public void Initialize(string fileName)
		{
			try
			{
				this._InitInstance(fileName, null);
			}
			catch (Exception innerException)
			{
				throw new ZipException(string.Format("{0} is not a valid zip file", fileName), innerException);
			}
		}

		private void _initEntriesDictionary()
		{
			StringComparer comparer = (!this.CaseSensitiveRetrieval) ? StringComparer.OrdinalIgnoreCase : StringComparer.Ordinal;
			this._entries = ((this._entries != null) ? new Dictionary<string, ZipEntry>(this._entries, comparer) : new Dictionary<string, ZipEntry>(comparer));
		}

		private void _InitInstance(string zipFileName, TextWriter statusMessageWriter)
		{
			this._name = zipFileName;
			this._StatusMessageTextWriter = statusMessageWriter;
			this._contentsChanged = true;
			this.AddDirectoryWillTraverseReparsePoints = true;
			this.CompressionLevel = CompressionLevel.Default;
			this.ParallelDeflateThreshold = 524288L;
			this._initEntriesDictionary();
			if (zipFileName != null && File.Exists(this._name))
			{
				if (this.FullScan)
				{
					ZipFile.ReadIntoInstance_Orig(this);
				}
				else
				{
					ZipFile.ReadIntoInstance(this);
				}
				this._fileAlreadyExists = true;
			}
		}

		public void RemoveEntry(ZipEntry entry)
		{
			if (entry == null)
			{
				throw new ArgumentNullException("entry");
			}
			this._entries.Remove(SharedUtilities.NormalizePathForUseInZipFile(entry.FileName));
			this._zipEntriesAsList = null;
			this._contentsChanged = true;
		}

		public void RemoveEntry(string fileName)
		{
			string fileName2 = ZipEntry.NameInArchive(fileName, null);
			ZipEntry zipEntry = this[fileName2];
			if (zipEntry == null)
			{
				throw new ArgumentException("The entry you specified was not found in the zip archive.");
			}
			this.RemoveEntry(zipEntry);
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposeManagedResources)
		{
			if (!this._disposed)
			{
				if (disposeManagedResources)
				{
					if (this._ReadStreamIsOurs && this._readstream != null)
					{
						this._readstream.Dispose();
						this._readstream = null;
					}
					if (this._temporaryFileName != null && this._name != null && this._writestream != null)
					{
						this._writestream.Dispose();
						this._writestream = null;
					}
					if (this.ParallelDeflater != null)
					{
						this.ParallelDeflater.Dispose();
						this.ParallelDeflater = null;
					}
				}
				this._disposed = true;
			}
		}

		[DebuggerHidden]
		public IEnumerator<ZipEntry> GetEnumerator()
		{
			ZipFile.<GetEnumerator>c__Iterator0 <GetEnumerator>c__Iterator = new ZipFile.<GetEnumerator>c__Iterator0();
			<GetEnumerator>c__Iterator.<>f__this = this;
			return <GetEnumerator>c__Iterator;
		}

		[DispId(-4)]
		public IEnumerator GetNewEnum()
		{
			return this.GetEnumerator();
		}
	}
}

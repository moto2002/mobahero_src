using System;

namespace Pathfinding.Ionic.Zip
{
	public class ExtractProgressEventArgs : ZipProgressEventArgs
	{
		private int _entriesExtracted;

		private string _target;

		public int EntriesExtracted
		{
			get
			{
				return this._entriesExtracted;
			}
		}

		public string ExtractLocation
		{
			get
			{
				return this._target;
			}
		}

		internal ExtractProgressEventArgs(string archiveName, bool before, int entriesTotal, int entriesExtracted, ZipEntry entry, string extractLocation) : base(archiveName, (!before) ? ZipProgressEventType.Extracting_AfterExtractEntry : ZipProgressEventType.Extracting_BeforeExtractEntry)
		{
			base.EntriesTotal = entriesTotal;
			base.CurrentEntry = entry;
			this._entriesExtracted = entriesExtracted;
			this._target = extractLocation;
		}

		internal ExtractProgressEventArgs(string archiveName, ZipProgressEventType flavor) : base(archiveName, flavor)
		{
		}

		internal ExtractProgressEventArgs()
		{
		}

		internal static ExtractProgressEventArgs BeforeExtractEntry(string archiveName, ZipEntry entry, string extractLocation)
		{
			return new ExtractProgressEventArgs
			{
				ArchiveName = archiveName,
				EventType = ZipProgressEventType.Extracting_BeforeExtractEntry,
				CurrentEntry = entry,
				_target = extractLocation
			};
		}

		internal static ExtractProgressEventArgs ExtractExisting(string archiveName, ZipEntry entry, string extractLocation)
		{
			return new ExtractProgressEventArgs
			{
				ArchiveName = archiveName,
				EventType = ZipProgressEventType.Extracting_ExtractEntryWouldOverwrite,
				CurrentEntry = entry,
				_target = extractLocation
			};
		}

		internal static ExtractProgressEventArgs AfterExtractEntry(string archiveName, ZipEntry entry, string extractLocation)
		{
			return new ExtractProgressEventArgs
			{
				ArchiveName = archiveName,
				EventType = ZipProgressEventType.Extracting_AfterExtractEntry,
				CurrentEntry = entry,
				_target = extractLocation
			};
		}

		internal static ExtractProgressEventArgs ExtractAllStarted(string archiveName, string extractLocation)
		{
			return new ExtractProgressEventArgs(archiveName, ZipProgressEventType.Extracting_BeforeExtractAll)
			{
				_target = extractLocation
			};
		}

		internal static ExtractProgressEventArgs ExtractAllCompleted(string archiveName, string extractLocation)
		{
			return new ExtractProgressEventArgs(archiveName, ZipProgressEventType.Extracting_AfterExtractAll)
			{
				_target = extractLocation
			};
		}

		internal static ExtractProgressEventArgs ByteUpdate(string archiveName, ZipEntry entry, long bytesWritten, long totalBytes)
		{
			return new ExtractProgressEventArgs(archiveName, ZipProgressEventType.Extracting_EntryBytesWritten)
			{
				ArchiveName = archiveName,
				CurrentEntry = entry,
				BytesTransferred = bytesWritten,
				TotalBytesToTransfer = totalBytes
			};
		}
	}
}

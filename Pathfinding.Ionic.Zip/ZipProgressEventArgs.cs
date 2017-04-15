using System;

namespace Pathfinding.Ionic.Zip
{
	public class ZipProgressEventArgs : EventArgs
	{
		private int _entriesTotal;

		private bool _cancel;

		private ZipEntry _latestEntry;

		private ZipProgressEventType _flavor;

		private string _archiveName;

		private long _bytesTransferred;

		private long _totalBytesToTransfer;

		public int EntriesTotal
		{
			get
			{
				return this._entriesTotal;
			}
			set
			{
				this._entriesTotal = value;
			}
		}

		public ZipEntry CurrentEntry
		{
			get
			{
				return this._latestEntry;
			}
			set
			{
				this._latestEntry = value;
			}
		}

		public bool Cancel
		{
			get
			{
				return this._cancel;
			}
			set
			{
				this._cancel = (this._cancel || value);
			}
		}

		public ZipProgressEventType EventType
		{
			get
			{
				return this._flavor;
			}
			set
			{
				this._flavor = value;
			}
		}

		public string ArchiveName
		{
			get
			{
				return this._archiveName;
			}
			set
			{
				this._archiveName = value;
			}
		}

		public long BytesTransferred
		{
			get
			{
				return this._bytesTransferred;
			}
			set
			{
				this._bytesTransferred = value;
			}
		}

		public long TotalBytesToTransfer
		{
			get
			{
				return this._totalBytesToTransfer;
			}
			set
			{
				this._totalBytesToTransfer = value;
			}
		}

		internal ZipProgressEventArgs()
		{
		}

		internal ZipProgressEventArgs(string archiveName, ZipProgressEventType flavor)
		{
			this._archiveName = archiveName;
			this._flavor = flavor;
		}
	}
}

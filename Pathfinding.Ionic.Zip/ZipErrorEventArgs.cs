using System;

namespace Pathfinding.Ionic.Zip
{
	public class ZipErrorEventArgs : ZipProgressEventArgs
	{
		private Exception _exc;

		public Exception Exception
		{
			get
			{
				return this._exc;
			}
		}

		public string FileName
		{
			get
			{
				return base.CurrentEntry.LocalFileName;
			}
		}

		private ZipErrorEventArgs()
		{
		}

		internal static ZipErrorEventArgs Saving(string archiveName, ZipEntry entry, Exception exception)
		{
			return new ZipErrorEventArgs
			{
				EventType = ZipProgressEventType.Error_Saving,
				ArchiveName = archiveName,
				CurrentEntry = entry,
				_exc = exception
			};
		}
	}
}

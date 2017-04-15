using System;
using System.Collections.Generic;

namespace Pathfinding.Serialization.JsonFx
{
	public class DataReaderProvider : IDataReaderProvider
	{
		private readonly IDictionary<string, IDataReader> ReadersByMime = new Dictionary<string, IDataReader>(StringComparer.OrdinalIgnoreCase);

		public DataReaderProvider(IEnumerable<IDataReader> readers)
		{
			if (readers != null)
			{
				foreach (IDataReader current in readers)
				{
					if (!string.IsNullOrEmpty(current.ContentType))
					{
						this.ReadersByMime[current.ContentType] = current;
					}
				}
			}
		}

		public IDataReader Find(string contentTypeHeader)
		{
			string key = DataWriterProvider.ParseMediaType(contentTypeHeader);
			if (this.ReadersByMime.ContainsKey(key))
			{
				return this.ReadersByMime[key];
			}
			return null;
		}
	}
}

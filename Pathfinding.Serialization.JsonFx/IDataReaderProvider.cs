using System;

namespace Pathfinding.Serialization.JsonFx
{
	public interface IDataReaderProvider
	{
		IDataReader Find(string contentTypeHeader);
	}
}

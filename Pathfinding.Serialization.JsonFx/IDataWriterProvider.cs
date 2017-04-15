using System;

namespace Pathfinding.Serialization.JsonFx
{
	public interface IDataWriterProvider
	{
		IDataWriter DefaultDataWriter
		{
			get;
		}

		IDataWriter Find(string extension);

		IDataWriter Find(string acceptHeader, string contentTypeHeader);
	}
}

using System;
using System.IO;

namespace Pathfinding.Serialization.JsonFx
{
	public interface IDataReader
	{
		string ContentType
		{
			get;
		}

		object Deserialize(TextReader input, Type data);
	}
}

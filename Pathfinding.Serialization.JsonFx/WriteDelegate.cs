using System;

namespace Pathfinding.Serialization.JsonFx
{
	public delegate void WriteDelegate<T>(JsonWriter writer, T value);
}

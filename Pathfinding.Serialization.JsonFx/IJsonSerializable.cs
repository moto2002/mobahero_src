using System;

namespace Pathfinding.Serialization.JsonFx
{
	public interface IJsonSerializable
	{
		void ReadJson(JsonReader reader);

		void WriteJson(JsonWriter writer);
	}
}

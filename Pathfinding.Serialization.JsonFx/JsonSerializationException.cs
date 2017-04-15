using System;

namespace Pathfinding.Serialization.JsonFx
{
	public class JsonSerializationException : InvalidOperationException
	{
		public JsonSerializationException()
		{
		}

		public JsonSerializationException(string message) : base(message)
		{
		}

		public JsonSerializationException(string message, Exception innerException) : base(message, innerException)
		{
		}
	}
}

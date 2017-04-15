using System;
using System.Collections.Generic;

namespace Pathfinding.Serialization.JsonFx
{
	public abstract class JsonConverter
	{
		public abstract bool CanConvert(Type t);

		public void Write(JsonWriter writer, Type type, object value)
		{
			Dictionary<string, object> value2 = this.WriteJson(type, value);
			writer.Write(value2);
		}

		public object Read(JsonReader reader, Type type, Dictionary<string, object> value)
		{
			return this.ReadJson(type, value);
		}

		public float CastFloat(object o)
		{
			if (o == null)
			{
				return 0f;
			}
			float result;
			try
			{
				result = Convert.ToSingle(o);
			}
			catch (Exception innerException)
			{
				throw new JsonDeserializationException("Cannot cast object to float. Expected float, got " + o.GetType(), innerException, 0);
			}
			return result;
		}

		public double CastDouble(object o)
		{
			if (o == null)
			{
				return 0.0;
			}
			double result;
			try
			{
				result = Convert.ToDouble(o);
			}
			catch (Exception innerException)
			{
				throw new JsonDeserializationException("Cannot cast object to double. Expected double, got " + o.GetType(), innerException, 0);
			}
			return result;
		}

		public abstract Dictionary<string, object> WriteJson(Type type, object value);

		public abstract object ReadJson(Type type, Dictionary<string, object> value);
	}
}

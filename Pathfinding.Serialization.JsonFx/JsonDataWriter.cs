using System;
using System.IO;
using System.Text;

namespace Pathfinding.Serialization.JsonFx
{
	public class JsonDataWriter : IDataWriter
	{
		public const string JsonMimeType = "application/json";

		public const string JsonFileExtension = ".json";

		private readonly JsonWriterSettings Settings;

		public Encoding ContentEncoding
		{
			get
			{
				return Encoding.UTF8;
			}
		}

		public string ContentType
		{
			get
			{
				return "application/json";
			}
		}

		public string FileExtension
		{
			get
			{
				return ".json";
			}
		}

		public JsonDataWriter(JsonWriterSettings settings)
		{
			if (settings == null)
			{
				throw new ArgumentNullException("settings");
			}
			this.Settings = settings;
		}

		public void Serialize(TextWriter output, object data)
		{
			new JsonWriter(output, this.Settings).Write(data);
		}

		public static JsonWriterSettings CreateSettings(bool prettyPrint)
		{
			return new JsonWriterSettings
			{
				PrettyPrint = prettyPrint
			};
		}
	}
}

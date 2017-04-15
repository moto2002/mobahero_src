using System;
using System.IO;
using System.Text;

namespace Pathfinding.Serialization.JsonFx
{
	public interface IDataWriter
	{
		Encoding ContentEncoding
		{
			get;
		}

		string ContentType
		{
			get;
		}

		string FileExtension
		{
			get;
		}

		void Serialize(TextWriter output, object data);
	}
}

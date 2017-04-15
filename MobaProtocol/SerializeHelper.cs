using ProtoBuf;
using System;
using System.IO;

namespace MobaProtocol
{
	public class SerializeHelper
	{
		public static bool usePool = false;

		private static readonly SimpleObjectPool<MemoryStream> _streamsPool = new SimpleObjectPool<MemoryStream>(10, delegate(MemoryStream stream)
		{
			stream.Position = 0L;
			stream.SetLength(0L);
		}, null, "");

		private static MemoryStream Get()
		{
			MemoryStream result;
			if (!SerializeHelper.usePool)
			{
				result = new MemoryStream();
			}
			else
			{
				result = SerializeHelper._streamsPool.Get();
			}
			return result;
		}

		private static void Release(MemoryStream stream)
		{
			if (SerializeHelper.usePool && stream != null)
			{
				SerializeHelper._streamsPool.Release(stream);
			}
		}

		public static byte[] Serialize<T>(T data)
		{
			byte[] array = null;
			MemoryStream memoryStream = SerializeHelper.Get();
			try
			{
				Serializer.Serialize<T>(memoryStream, data);
				memoryStream.Position = 0L;
				int count = (int)memoryStream.Length;
				array = new byte[memoryStream.Length];
				memoryStream.Read(array, 0, count);
			}
			finally
			{
				SerializeHelper.Release(memoryStream);
			}
			return array;
		}

		public static byte[] Serialize(object data)
		{
			byte[] array = null;
			MemoryStream memoryStream = SerializeHelper.Get();
			try
			{
				Serializer.Serialize<object>(memoryStream, data);
				memoryStream.Position = 0L;
				int count = (int)memoryStream.Length;
				array = new byte[memoryStream.Length];
				memoryStream.Read(array, 0, count);
			}
			finally
			{
				SerializeHelper.Release(memoryStream);
			}
			return array;
		}

		public static T Deserialize<T>(byte[] buffer)
		{
			T result;
			using (MemoryStream memoryStream = new MemoryStream(buffer))
			{
				result = Serializer.Deserialize<T>(memoryStream);
			}
			return result;
		}

		public static object DeserializeUseType(byte[] buffer, Type type)
		{
			object result;
			using (MemoryStream memoryStream = new MemoryStream(buffer))
			{
				result = Serializer.NonGeneric.Deserialize(type, memoryStream);
			}
			return result;
		}
	}
}

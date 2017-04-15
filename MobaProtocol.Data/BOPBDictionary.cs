using ProtoBuf;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class BOPBDictionary
	{
		[ProtoMember(1)]
		public List<byte> keys
		{
			get;
			set;
		}

		[ProtoMember(2)]
		public byte[] valueBytes
		{
			get;
			set;
		}

		public Dictionary<byte, object> GetDic()
		{
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			Dictionary<byte, object> result;
			if (this.keys == null)
			{
				result = dictionary;
			}
			else
			{
				ByteBuffer byteBuffer = new ByteBuffer(this.valueBytes, 0, this.valueBytes.Length);
				ArrayList arrayList = byteBuffer.ReadArray();
				if (this.keys.Count != arrayList.Count)
				{
					throw new ArgumentException(string.Concat(new object[]
					{
						"BODic.ToDic read error,count error:",
						this.keys.Count,
						"!=",
						arrayList.Count
					}));
				}
				for (int i = 0; i < this.keys.Count; i++)
				{
					dictionary[this.keys[i]] = arrayList[i];
				}
				result = dictionary;
			}
			return result;
		}

		public void SetDic(Dictionary<byte, object> val)
		{
			if (val == null)
			{
				this.keys = null;
			}
			else
			{
				this.keys = val.Keys.ToList<byte>();
				ByteBuffer byteBuffer = new ByteBuffer(true);
				ArrayList obj = new ArrayList(val.Values.ToArray<object>());
				byteBuffer.Write(obj);
				this.valueBytes = byteBuffer.ToBytes();
			}
		}

		public void SetDic(Dictionary<object, object> val)
		{
			if (val == null)
			{
				this.keys = null;
			}
			else
			{
				this.keys = (from o in val.Keys
				select (byte)o).ToList<byte>();
				ByteBuffer byteBuffer = new ByteBuffer(true);
				ArrayList obj = new ArrayList(val.Values.ToArray<object>());
				byteBuffer.Write(obj);
				this.valueBytes = byteBuffer.ToBytes();
			}
		}
	}
}

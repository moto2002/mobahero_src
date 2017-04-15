using System;
using System.Collections;
using System.IO;
using System.Text;

namespace MobaProtocol
{
	public class ByteBuffer
	{
		private MemoryStream stream = null;

		private BinaryWriter writer = null;

		private BinaryReader reader = null;

		private bool m_isWriteType = false;

		public ByteBuffer(bool writeType = false)
		{
			this.m_isWriteType = writeType;
			this.stream = new MemoryStream();
			this.writer = new BinaryWriter(this.stream);
		}

		public ByteBuffer(byte[] data, int offset, int count)
		{
			if (data != null)
			{
				this.stream = new MemoryStream(data, offset, count);
				this.reader = new BinaryReader(this.stream);
			}
			else
			{
				this.stream = new MemoryStream();
				this.writer = new BinaryWriter(this.stream);
			}
		}

		public void Reset(byte[] data, int offset, int count)
		{
			if (data != null)
			{
				this.stream.SetLength(0L);
				this.stream.Position = 0L;
				this.writer.Write(data, offset, count);
			}
			else
			{
				this.stream.SetLength(0L);
				this.stream.Position = 0L;
			}
		}

		public void Close()
		{
			if (this.writer != null)
			{
				this.writer.Close();
			}
			if (this.reader != null)
			{
				this.reader.Close();
			}
			this.stream.Close();
			this.writer = null;
			this.reader = null;
			this.stream = null;
		}

		public void WriteWithType(object obj)
		{
			if (obj != null)
			{
				this.Write((byte)ByteBuffer.GetNetObjectType(obj.GetType()));
				this.Write(obj);
			}
			else
			{
				this.Write(16);
			}
		}

		public void Write(object obj)
		{
			switch (ByteBuffer.GetNetObjectType(obj.GetType()))
			{
			case NetObjetType.TARRAY:
				this.WriteArrayList(obj as ArrayList);
				return;
			case NetObjetType.TBool:
				this.WriteBool((bool)obj);
				return;
			case NetObjetType.TByte:
				this.WriteByte((byte)obj);
				return;
			case NetObjetType.TByteList:
				this.WriteByteList((byte[])obj);
				return;
			case NetObjetType.TInt:
				this.WriteInt((int)obj);
				return;
			case NetObjetType.TLong:
				this.WriteLong((long)obj);
				return;
			case NetObjetType.TShort:
				this.WriteShort((ushort)obj);
				return;
			case NetObjetType.TFloat:
				this.WriteFloat((float)obj);
				return;
			case NetObjetType.TDouble:
				this.WriteDouble((double)obj);
				return;
			case NetObjetType.TString:
				this.WriteString((string)obj);
				return;
			case NetObjetType.TNull:
				return;
			}
			throw new Exception(string.Concat(new object[]
			{
				"write unknow value:",
				obj.ToString(),
				"  ",
				obj
			}));
		}

		public static NetObjetType GetNetObjectType(Type type)
		{
			NetObjetType result;
			if (type.Equals(typeof(byte)))
			{
				result = NetObjetType.TByte;
			}
			else if (type.Equals(typeof(bool)))
			{
				result = NetObjetType.TBool;
			}
			else if (type.Equals(typeof(int)))
			{
				result = NetObjetType.TInt;
			}
			else if (type.Equals(typeof(long)))
			{
				result = NetObjetType.TLong;
			}
			else if (type.Equals(typeof(ushort)))
			{
				result = NetObjetType.TShort;
			}
			else if (type.Equals(typeof(float)))
			{
				result = NetObjetType.TFloat;
			}
			else if (type.Equals(typeof(double)))
			{
				result = NetObjetType.TDouble;
			}
			else if (type.Equals(typeof(string)))
			{
				result = NetObjetType.TString;
			}
			else if (type.Equals(typeof(byte[])))
			{
				result = NetObjetType.TByteList;
			}
			else if (type.Equals(typeof(ArrayList)))
			{
				result = NetObjetType.TARRAY;
			}
			else
			{
				result = NetObjetType.TUnknow;
			}
			return result;
		}

		public void WriteBool(bool val)
		{
			this.writer.Write(val);
		}

		public void WriteByte(byte v)
		{
			this.writer.Write(v);
		}

		public void WriteByteList(byte[] bt)
		{
			this.WriteShort((ushort)bt.Length);
			this.writer.Write(bt);
		}

		public void WriteInt(int v)
		{
			this.writer.Write(v);
		}

		public void WriteShort(ushort v)
		{
			this.writer.Write(v);
		}

		public void WriteLong(long v)
		{
			this.writer.Write(v);
		}

		public void WriteFloat(float v)
		{
			byte[] bytes = BitConverter.GetBytes(v);
			Array.Reverse(bytes);
			this.writer.Write(BitConverter.ToSingle(bytes, 0));
		}

		public void WriteDouble(double v)
		{
			byte[] bytes = BitConverter.GetBytes(v);
			Array.Reverse(bytes);
			this.writer.Write(BitConverter.ToDouble(bytes, 0));
		}

		public void WriteString(string v)
		{
			byte[] bytes = Encoding.UTF8.GetBytes(v);
			this.writer.Write((ushort)bytes.Length);
			this.writer.Write(bytes);
		}

		public void WriteArrayList(ArrayList arr)
		{
			this.writer.Write((ushort)arr.Count);
			foreach (object current in arr)
			{
				this.WriteWithType(current);
			}
		}

		public object ReadWithType()
		{
			NetObjetType totype = (NetObjetType)this.ReadByte();
			return this.Read(totype);
		}

		public object Read(NetObjetType totype)
		{
			object result;
			switch (totype)
			{
			case NetObjetType.TARRAY:
				result = this.ReadArray();
				return result;
			case NetObjetType.TBool:
				result = this.ReadBool();
				return result;
			case NetObjetType.TByte:
				result = this.ReadByte();
				return result;
			case NetObjetType.TByteList:
				result = this.ReadByteList();
				return result;
			case NetObjetType.TInt:
				result = this.ReadInt();
				return result;
			case NetObjetType.TLong:
				result = this.ReadLong();
				return result;
			case NetObjetType.TShort:
				result = this.ReadShort();
				return result;
			case NetObjetType.TFloat:
				result = this.ReadFloat();
				return result;
			case NetObjetType.TDouble:
				result = this.ReadDouble();
				return result;
			case NetObjetType.TString:
				result = this.ReadString();
				return result;
			case NetObjetType.TNull:
				result = null;
				return result;
			}
			result = 0;
			return result;
		}

		public byte ReadByte()
		{
			return this.reader.ReadByte();
		}

		public bool ReadBool()
		{
			return this.reader.ReadBoolean();
		}

		public byte[] ReadByteList()
		{
			ushort count = this.ReadShort();
			return this.reader.ReadBytes((int)count);
		}

		public int ReadInt()
		{
			return this.reader.ReadInt32();
		}

		public ushort ReadShort()
		{
			return (ushort)this.reader.ReadInt16();
		}

		public long ReadLong()
		{
			return this.reader.ReadInt64();
		}

		public float ReadFloat()
		{
			byte[] bytes = BitConverter.GetBytes(this.reader.ReadSingle());
			Array.Reverse(bytes);
			return BitConverter.ToSingle(bytes, 0);
		}

		public double ReadDouble()
		{
			byte[] bytes = BitConverter.GetBytes(this.reader.ReadDouble());
			Array.Reverse(bytes);
			return BitConverter.ToDouble(bytes, 0);
		}

		public string ReadString()
		{
			ushort num = this.ReadShort();
			byte[] bytes = new byte[(int)num];
			bytes = this.reader.ReadBytes((int)num);
			return Encoding.UTF8.GetString(bytes);
		}

		public ArrayList ReadArray()
		{
			ArrayList arrayList = new ArrayList();
			ushort num = this.ReadShort();
			for (int i = 0; i < (int)num; i++)
			{
				arrayList.Add(this.ReadWithType());
			}
			return arrayList;
		}

		public byte[] ToBytes()
		{
			this.writer.Flush();
			return this.stream.ToArray();
		}

		public void Flush()
		{
			this.writer.Flush();
		}
	}
}

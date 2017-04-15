using System;
using System.IO;
using System.Text;
using UnityEngine;

public class SByteBuffer
{
	private MemoryStream stream;

	private BinaryWriter writer;

	private BinaryReader reader;

	public SByteBuffer()
	{
		this.stream = new MemoryStream();
		this.writer = new BinaryWriter(this.stream);
	}

	public SByteBuffer(byte[] data)
	{
		if (data != null)
		{
			this.stream = new MemoryStream(data);
			this.reader = new BinaryReader(this.stream);
		}
		else
		{
			this.stream = new MemoryStream();
			this.writer = new BinaryWriter(this.stream);
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

	public void WriteByte(byte v)
	{
		this.writer.Write(v);
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
		this.writer.Write(v);
	}

	public void WriteBool(bool v)
	{
		this.writer.Write(v);
	}

	public void WriteColor(Color v)
	{
		this.WriteFloat(v.r);
		this.WriteFloat(v.g);
		this.WriteFloat(v.b);
		this.WriteFloat(v.a);
	}

	public void WriteVector3(Vector3 v)
	{
		this.WriteFloat(v.x);
		this.WriteFloat(v.y);
		this.WriteFloat(v.z);
	}

	public void WriteVector3_2(Vector3? v)
	{
		if (v.HasValue)
		{
			Vector3 value = v.Value;
			this.WriteFloat(value.x);
			this.WriteFloat(value.y);
			this.WriteFloat(value.z);
		}
	}

	public void WriteDouble(double v)
	{
		this.writer.Write(v);
	}

	public void WriteString(string v)
	{
		byte[] bytes = Encoding.UTF8.GetBytes(v);
		this.writer.Write((ushort)bytes.Length);
		this.writer.Write(bytes);
	}

	public void WriteBytes(byte[] v)
	{
		this.writer.Write((ushort)v.Length);
		this.writer.Write(v);
	}

	public void Write(Type tp, object obj)
	{
		if (tp == null)
		{
			return;
		}
		if (tp == typeof(byte))
		{
			this.WriteByte((byte)obj);
		}
		else if (tp == typeof(int))
		{
			this.WriteInt((int)obj);
		}
		else if (tp == typeof(ushort) || tp == typeof(short))
		{
			this.WriteShort((ushort)obj);
		}
		else if (tp == typeof(long) || tp == typeof(long))
		{
			this.WriteLong((long)obj);
		}
		else if (tp == typeof(float))
		{
			this.WriteFloat((float)obj);
		}
		else if (tp == typeof(string))
		{
			this.WriteString((string)obj);
		}
		else if (tp == typeof(double))
		{
			this.WriteDouble((double)obj);
		}
		else if (tp == typeof(Vector3))
		{
			this.WriteVector3((Vector3)obj);
		}
		else if (tp == typeof(Vector3?))
		{
			this.WriteVector3_2((Vector3?)obj);
		}
		else if (tp == typeof(bool))
		{
			this.WriteBool((bool)obj);
		}
		else if (tp == typeof(Color))
		{
			this.WriteColor((Color)obj);
		}
		else
		{
			if (tp != null)
			{
				throw new Exception("unknow type write:" + tp.Name);
			}
			throw new Exception("unknow type write: tp is null !!");
		}
	}

	public object Read(Type tp)
	{
		if (tp == typeof(byte))
		{
			return this.ReadByte();
		}
		if (tp == typeof(int))
		{
			return this.ReadInt();
		}
		if (tp == typeof(ushort) || tp == typeof(short))
		{
			return this.ReadShort();
		}
		if (tp == typeof(long) || tp == typeof(long))
		{
			return this.ReadInt64();
		}
		if (tp == typeof(float))
		{
			return this.ReadFloat();
		}
		if (tp == typeof(double))
		{
			return this.ReadDouble();
		}
		if (tp == typeof(string))
		{
			return this.ReadString();
		}
		if (tp == typeof(Vector3))
		{
			return this.ReadVector3();
		}
		if (tp == typeof(Color))
		{
			return this.ReadColor();
		}
		return null;
	}

	public byte ReadByte()
	{
		return this.reader.ReadByte();
	}

	public int ReadInt()
	{
		return this.reader.ReadInt32();
	}

	public ushort ReadShort()
	{
		return (ushort)this.reader.ReadInt16();
	}

	public long ReadInt64()
	{
		return this.reader.ReadInt64();
	}

	public byte[] ReadBytes()
	{
		ushort num = this.ReadShort();
		byte[] array = new byte[(int)num];
		return this.reader.ReadBytes((int)num);
	}

	public float ReadFloat()
	{
		return this.reader.ReadSingle();
	}

	public Vector3 ReadVector3()
	{
		float x = this.ReadFloat();
		float y = this.ReadFloat();
		float z = this.ReadFloat();
		Vector3 result = new Vector3(x, y, z);
		return result;
	}

	public Color ReadColor()
	{
		float r = this.ReadFloat();
		float g = this.ReadFloat();
		float b = this.ReadFloat();
		float a = this.ReadFloat();
		Color result = new Color(r, g, b, a);
		return result;
	}

	public double ReadDouble()
	{
		return this.reader.ReadDouble();
	}

	public string ReadString()
	{
		ushort num = this.ReadShort();
		byte[] bytes = new byte[(int)num];
		bytes = this.reader.ReadBytes((int)num);
		return Encoding.UTF8.GetString(bytes);
	}

	public byte[] ToBytes()
	{
		this.writer.Flush();
		return this.stream.ToArray();
	}

	public byte[] ToCompressionBytes()
	{
		return this.ToBytes();
	}

	public void Flush()
	{
		this.writer.Flush();
	}
}

using System;

public class ByteBuffer
{
	private byte[] buf;

	private int readIndex;

	private int writeIndex;

	private int markReadIndex;

	private int markWirteIndex;

	private int capacity;

	private ByteBuffer(int capacity)
	{
		this.buf = new byte[capacity];
		this.capacity = capacity;
	}

	private ByteBuffer(byte[] bytes)
	{
		this.buf = bytes;
		this.capacity = bytes.Length;
	}

	public static ByteBuffer Allocate(int capacity)
	{
		return new ByteBuffer(capacity);
	}

	public static ByteBuffer Allocate(byte[] bytes)
	{
		return new ByteBuffer(bytes);
	}

	private int FixLength(int length)
	{
		int num = 2;
		int i = 2;
		while (i < length)
		{
			i = 2 << num;
			num++;
		}
		return i;
	}

	private byte[] flip(byte[] bytes)
	{
		if (BitConverter.IsLittleEndian)
		{
			Array.Reverse(bytes);
		}
		return bytes;
	}

	private int FixSizeAndReset(int currLen, int futureLen)
	{
		if (futureLen > currLen)
		{
			int num = this.FixLength(currLen) * 2;
			if (futureLen > num)
			{
				num = this.FixLength(futureLen) * 2;
			}
			byte[] array = new byte[num];
			Array.Copy(this.buf, 0, array, 0, currLen);
			this.buf = array;
			this.capacity = array.Length;
		}
		return futureLen;
	}

	public void WriteBytes(byte[] bytes, int startIndex, int length)
	{
		lock (this)
		{
			int num = length - startIndex;
			if (num > 0)
			{
				int num2 = num + this.writeIndex;
				int currLen = this.buf.Length;
				this.FixSizeAndReset(currLen, num2);
				int i = this.writeIndex;
				int num3 = startIndex;
				while (i < num2)
				{
					this.buf[i] = bytes[num3];
					i++;
					num3++;
				}
				this.writeIndex = num2;
			}
		}
	}

	public void WriteBytes(byte[] bytes, int length)
	{
		this.WriteBytes(bytes, 0, length);
	}

	public void WriteBytes(byte[] bytes)
	{
		this.WriteBytes(bytes, bytes.Length);
	}

	public void Write(ByteBuffer buffer)
	{
		if (buffer == null)
		{
			return;
		}
		if (buffer.ReadableBytes() <= 0)
		{
			return;
		}
		this.WriteBytes(buffer.ToArray());
	}

	public void WriteShort(short value)
	{
		this.WriteBytes(this.flip(BitConverter.GetBytes(value)));
	}

	public void WriteUshort(ushort value)
	{
		this.WriteBytes(this.flip(BitConverter.GetBytes(value)));
	}

	public void WriteInt(int value)
	{
		this.WriteBytes(this.flip(BitConverter.GetBytes(value)));
	}

	public void WriteUint(uint value)
	{
		this.WriteBytes(this.flip(BitConverter.GetBytes(value)));
	}

	public void WriteLong(long value)
	{
		this.WriteBytes(this.flip(BitConverter.GetBytes(value)));
	}

	public void WriteUlong(ulong value)
	{
		this.WriteBytes(this.flip(BitConverter.GetBytes(value)));
	}

	public void WriteFloat(float value)
	{
		this.WriteBytes(this.flip(BitConverter.GetBytes(value)));
	}

	public void WriteByte(byte value)
	{
		lock (this)
		{
			int futureLen = this.writeIndex + 1;
			int currLen = this.buf.Length;
			this.FixSizeAndReset(currLen, futureLen);
			this.buf[this.writeIndex] = value;
			this.writeIndex = futureLen;
		}
	}

	public void WriteDouble(double value)
	{
		this.WriteBytes(this.flip(BitConverter.GetBytes(value)));
	}

	public byte ReadByte()
	{
		byte result = this.buf[this.readIndex];
		this.readIndex++;
		return result;
	}

	private byte[] Read(int len)
	{
		byte[] array = new byte[len];
		Array.Copy(this.buf, this.readIndex, array, 0, len);
		if (BitConverter.IsLittleEndian)
		{
			Array.Reverse(array);
		}
		this.readIndex += len;
		return array;
	}

	public ushort ReadUshort()
	{
		return BitConverter.ToUInt16(this.Read(2), 0);
	}

	public short ReadShort()
	{
		return BitConverter.ToInt16(this.Read(2), 0);
	}

	public uint ReadUint()
	{
		return BitConverter.ToUInt32(this.Read(4), 0);
	}

	public int ReadInt()
	{
		return BitConverter.ToInt32(this.Read(4), 0);
	}

	public ulong ReadUlong()
	{
		return BitConverter.ToUInt64(this.Read(8), 0);
	}

	public long ReadLong()
	{
		return BitConverter.ToInt64(this.Read(8), 0);
	}

	public float ReadFloat()
	{
		return BitConverter.ToSingle(this.Read(4), 0);
	}

	public double ReadDouble()
	{
		return BitConverter.ToDouble(this.Read(8), 0);
	}

	public string ReadString()
	{
		return BitConverter.ToString(this.Read(8), 0);
	}

	public void ReadBytes(byte[] disbytes, int disstart, int len)
	{
		int num = disstart + len;
		for (int i = disstart; i < num; i++)
		{
			disbytes[i] = this.ReadByte();
		}
	}

	public void DiscardReadBytes()
	{
		if (this.readIndex <= 0)
		{
			return;
		}
		int num = this.buf.Length - this.readIndex;
		byte[] destinationArray = new byte[num];
		Array.Copy(this.buf, this.readIndex, destinationArray, 0, num);
		this.buf = destinationArray;
		this.writeIndex -= this.readIndex;
		this.markReadIndex -= this.readIndex;
		if (this.markReadIndex < 0)
		{
			this.markReadIndex = this.readIndex;
		}
		this.markWirteIndex -= this.readIndex;
		if (this.markWirteIndex < 0 || this.markWirteIndex < this.readIndex || this.markWirteIndex < this.markReadIndex)
		{
			this.markWirteIndex = this.writeIndex;
		}
		this.readIndex = 0;
	}

	public void Clear()
	{
		this.buf = new byte[this.buf.Length];
		this.readIndex = 0;
		this.writeIndex = 0;
		this.markReadIndex = 0;
		this.markWirteIndex = 0;
	}

	public void SetReaderIndex(int index)
	{
		if (index < 0)
		{
			return;
		}
		this.readIndex = index;
	}

	public void MarkReaderIndex()
	{
		this.markReadIndex = this.readIndex;
	}

	public void MarkWriterIndex()
	{
		this.markWirteIndex = this.writeIndex;
	}

	public void ResetReaderIndex()
	{
		this.readIndex = this.markReadIndex;
	}

	public void ResetWriterIndex()
	{
		this.writeIndex = this.markWirteIndex;
	}

	public int ReadableBytes()
	{
		return this.writeIndex - this.readIndex;
	}

	public byte[] ToArray()
	{
		byte[] array = new byte[this.writeIndex];
		Array.Copy(this.buf, 0, array, 0, array.Length);
		return array;
	}

	public int GetCapacity()
	{
		return this.capacity;
	}
}

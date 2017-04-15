using SevenZip;
using SevenZip.Compression.LZMA;
using System;
using System.IO;
using UnityEngine;

public class Util
{
	private static CoderPropID[] propIDs = new CoderPropID[]
	{
		CoderPropID.DictionarySize,
		CoderPropID.PosStateBits,
		CoderPropID.LitContextBits,
		CoderPropID.LitPosBits,
		CoderPropID.Algorithm,
		CoderPropID.NumFastBytes,
		CoderPropID.MatchFinder,
		CoderPropID.EndMarker
	};

	public static CompressMode mode = CompressMode.LZMA;

	public static string StorePath
	{
		get
		{
			return "jar:file://" + Application.dataPath + "!/assets//";
		}
	}

	public static byte[] Compress(byte[] data)
	{
		switch (Util.mode)
		{
		case CompressMode.GZip:
			return Util.CompressZip(data);
		case CompressMode.LZMA:
			return Util.CompressLzma(data);
		case CompressMode.LZMALow:
			return Util.CompressLzmaLow(data);
		default:
			return null;
		}
	}

	public static byte[] Decompress(byte[] data)
	{
		switch (Util.mode)
		{
		case CompressMode.GZip:
			return Util.DecompressZip(data);
		case CompressMode.LZMA:
			return Util.DecompressLzma(data);
		case CompressMode.LZMALow:
			return Util.DecompressLzma(data);
		default:
			return null;
		}
	}

	public static byte[] CompressZip(byte[] data)
	{
		return data;
	}

	public static byte[] DecompressZip(byte[] input)
	{
		return input;
	}

	public static byte[] CompressLzma(byte[] data)
	{
		object[] properties = new object[]
		{
			2097152,
			2,
			3,
			2,
			1,
			128,
			"bt4",
			true
		};
		Encoder encoder = new Encoder();
		encoder.SetCoderProperties(Util.propIDs, properties);
		MemoryStream memoryStream = new MemoryStream(data);
		MemoryStream memoryStream2 = new MemoryStream();
		encoder.WriteCoderProperties(memoryStream2);
		long length = memoryStream.Length;
		encoder.Code(memoryStream, memoryStream2, -1L, -1L, null);
		return memoryStream2.ToArray();
	}

	public static byte[] CompressLzmaLow(byte[] data)
	{
		object[] properties = new object[]
		{
			6,
			2,
			3,
			2,
			1,
			128,
			"bt4",
			true
		};
		Encoder encoder = new Encoder();
		encoder.SetCoderProperties(Util.propIDs, properties);
		MemoryStream memoryStream = new MemoryStream(data);
		MemoryStream memoryStream2 = new MemoryStream();
		encoder.WriteCoderProperties(memoryStream2);
		long length = memoryStream.Length;
		encoder.Code(memoryStream, memoryStream2, -1L, -1L, null);
		return memoryStream2.ToArray();
	}

	public static byte[] DecompressLzma(byte[] data)
	{
		Decoder decoder = new Decoder();
		byte[] array = new byte[5];
		Array.Copy(data, array, 5);
		decoder.SetDecoderProperties(array);
		MemoryStream memoryStream = new MemoryStream(data);
		memoryStream.Seek(5L, SeekOrigin.Current);
		MemoryStream memoryStream2 = new MemoryStream();
		decoder.Code(memoryStream, memoryStream2, -1L, -1L, null);
		return memoryStream2.ToArray();
	}
}

using System;

public class TimeUtils
{
	public static long BinaryStamp()
	{
		return DateTime.UtcNow.ToBinary();
	}
}

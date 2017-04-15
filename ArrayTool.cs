using System;

public class ArrayTool
{
	public static bool isNullOrEmpty(object[] arr)
	{
		return arr == null || arr.Length == 0;
	}

	public static int getSize(object[] arr)
	{
		return ArrayTool.isNullOrEmpty(arr) ? 0 : arr.Length;
	}

	public static int getTotalSize(params object[] arrs)
	{
		int num = 0;
		for (int i = 0; i < arrs.Length; i++)
		{
			object[] arr = (object[])arrs[i];
			num += ArrayTool.getSize(arr);
		}
		return num;
	}
}

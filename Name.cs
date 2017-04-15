using System;
using System.Collections.Generic;

public class Name
{
	private static List<string> StringList = new List<string>();

	private static Dictionary<string, int> StringIDs = new Dictionary<string, int>();

	public static void reset()
	{
		if (Name.StringList != null)
		{
			Name.StringList.Clear();
		}
		else
		{
			Name.StringList = new List<string>();
		}
		if (Name.StringIDs != null)
		{
			Name.StringIDs.Clear();
		}
		else
		{
			Name.StringIDs = new Dictionary<string, int>();
		}
	}

	public static int add(string inStr)
	{
		int num = 0;
		int result;
		if (Name.StringIDs.TryGetValue(inStr, out num))
		{
			result = num;
		}
		else
		{
			num = Name.StringList.Count;
			Name.StringIDs.Add(inStr, num);
			Name.StringList.Add(inStr);
			result = num;
		}
		return result;
	}

	public static int getId(string inStr)
	{
		int result = 0;
		Name.StringIDs.TryGetValue(inStr, out result);
		return result;
	}

	public static string getString(int inId)
	{
		string result;
		if (inId < Name.StringList.Count && inId >= 0)
		{
			result = Name.StringList[inId];
		}
		else
		{
			result = "";
		}
		return result;
	}
}

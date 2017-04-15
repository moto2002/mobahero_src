using System;
using UnityEngine;

public class DirectoryTool
{
	public static string extractFileFromPath(string path)
	{
		int num = path.LastIndexOf("/");
		return path.Substring(num + 1, path.Length - num - 1);
	}

	public static string extractPath(string path)
	{
		int num = path.LastIndexOf("/");
		string result;
		if (num <= 0)
		{
			result = "";
		}
		else
		{
			result = path.Substring(0, num);
		}
		return result;
	}

	public static string extractResPathFromAssetPath(string assetPath)
	{
		string result;
		if (!assetPath.Contains("Assets/Resources/"))
		{
			Debug.LogError("asset is not in Resources folder! inappropriate call ");
			result = "";
		}
		else
		{
			int length = "Assets/Resources/".Length;
			int num;
			if (assetPath.Contains("."))
			{
				num = assetPath.LastIndexOf('.');
			}
			else
			{
				num = assetPath.Length - 1;
			}
			result = assetPath.Substring(length, num - length);
		}
		return result;
	}
}

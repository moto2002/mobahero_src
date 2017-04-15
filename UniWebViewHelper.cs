using System;
using UnityEngine;

public class UniWebViewHelper
{
	public static int screenHeight
	{
		get
		{
			return Screen.height;
		}
	}

	public static int screenWidth
	{
		get
		{
			return Screen.width;
		}
	}

	public static int screenScale
	{
		get
		{
			return 1;
		}
	}

	public static string streamingAssetURLForPath(string path)
	{
		return "file:///android_asset/" + path;
	}
}

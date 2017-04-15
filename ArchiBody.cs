using System;
using UnityEngine;

public class ArchiBody
{
	public static void ReMagnify(GameObject g)
	{
		float magnify = ArchiBody.GetMagnify(g.name);
		g.transform.localScale = g.transform.localScale * magnify;
	}

	private static float GetMagnify(string modelName)
	{
		if (modelName.Length < 7)
		{
			return 1f;
		}
		string text = modelName.Substring(0, modelName.Length - 7);
		string text2 = text;
		switch (text2)
		{
		case "Huonv":
			return 1.05f;
		case "Lanmao":
			return 1.1f;
		case "Xiaolu":
			return 1.13f;
		case "Zhousi":
			return 1.2f;
		case "Demaershi":
			return 1.08f;
		case "DK":
			return 1.09f;
		case "Xiaoxiao":
			return 1.1f;
		case "Jiansheng":
			return 1.05f;
		case "Meidusha":
			return 1.1f;
		case "Emowushi":
			return 1.1f;
		}
		return 1f;
	}
}

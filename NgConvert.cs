using System;
using System.Collections;
using UnityEngine;

public class NgConvert
{
	public static string GetTabSpace(int nTab)
	{
		string str = "    ";
		string text = string.Empty;
		for (int i = 0; i < nTab; i++)
		{
			text += str;
		}
		return text;
	}

	public static string[] GetIntStrings(int start, int count)
	{
		string[] array = new string[count];
		for (int i = start; i < count; i++)
		{
			array[i] = i.ToString();
		}
		return array;
	}

	public static int[] GetIntegers(int start, int count)
	{
		int[] array = new int[count];
		for (int i = start; i < count; i++)
		{
			array[i] = i;
		}
		return array;
	}

	public static ArrayList ToArrayList<TT>(TT[] data)
	{
		ArrayList arrayList = new ArrayList();
		for (int i = 0; i < data.Length; i++)
		{
			arrayList.Add(data[i]);
		}
		return arrayList;
	}

	public static TT[] ToArray<TT>(ArrayList data)
	{
		TT[] array = new TT[data.Count];
		int num = 0;
		IEnumerator enumerator = data.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				TT tT = (TT)((object)enumerator.Current);
				if (tT != null)
				{
					array[num] = tT;
				}
				num++;
			}
		}
		finally
		{
			IDisposable disposable = enumerator as IDisposable;
			if (disposable != null)
			{
				disposable.Dispose();
			}
		}
		return array;
	}

	public static TT[] ResizeArray<TT>(TT[] src, int nResize)
	{
		TT[] array = new TT[nResize];
		int num = 0;
		while (num < src.Length && num < nResize)
		{
			array[num] = src[num];
			num++;
		}
		return array;
	}

	public static TT[] ResizeArray<TT>(TT[] src, int nResize, TT defaultValue)
	{
		TT[] array = new TT[nResize];
		int i = 0;
		while (i < src.Length && i < nResize)
		{
			array[i] = src[i];
			i++;
		}
		while (i < array.Length)
		{
			array[i] = defaultValue;
			i++;
		}
		return array;
	}

	public static string[] ResizeArray(string[] src, int nResize)
	{
		string[] array = new string[nResize];
		int num = 0;
		while (num < src.Length && num < nResize)
		{
			array[num] = src[num];
			num++;
		}
		return array;
	}

	public static GameObject[] ResizeArray(GameObject[] src, int nResize)
	{
		GameObject[] array = new GameObject[nResize];
		int num = 0;
		while (num < src.Length && num < nResize)
		{
			array[num] = src[num];
			num++;
		}
		return array;
	}

	public static GUIContent[] ResizeArray(GUIContent[] src, int nResize)
	{
		GUIContent[] array = new GUIContent[nResize];
		int num = 0;
		while (num < src.Length && num < nResize)
		{
			array[num] = src[num];
			num++;
		}
		return array;
	}

	public static GUIContent[] StringsToContents(string[] strings)
	{
		if (strings == null)
		{
			return null;
		}
		GUIContent[] array = new GUIContent[strings.Length];
		for (int i = 0; i < strings.Length; i++)
		{
			array[i] = new GUIContent(strings[i], strings[i]);
		}
		return array;
	}

	public static string[] ContentsToStrings(GUIContent[] contents)
	{
		if (contents == null)
		{
			return null;
		}
		string[] array = new string[contents.Length];
		for (int i = 0; i < contents.Length; i++)
		{
			array[i] = contents[i].text;
		}
		return array;
	}

	public static uint ToUint(string value, uint nDefaultValue)
	{
		value = value.Trim();
		if (value == string.Empty)
		{
			value = "0";
		}
		uint result;
		if (uint.TryParse(value, out result))
		{
			return result;
		}
		return nDefaultValue;
	}

	public static int ToInt(string value, int nDefaultValue)
	{
		value = value.Trim();
		if (value == string.Empty)
		{
			value = "0";
		}
		int result;
		if (int.TryParse(value, out result))
		{
			return result;
		}
		return nDefaultValue;
	}

	public static float ToFloat(string value, float fDefaultValue)
	{
		value = value.Trim();
		if (value == string.Empty)
		{
			value = "0";
		}
		float result;
		if (float.TryParse(value, out result))
		{
			return result;
		}
		return fDefaultValue;
	}

	public static string GetVaildFloatString(string strInput, ref float fCompleteValue)
	{
		int i = 0;
		int num = 0;
		string text = "0123456789";
		strInput = strInput.Trim();
		while (i < strInput.Length)
		{
			if (text.Contains(strInput[i].ToString()))
			{
				i++;
			}
			else if (strInput[i] == '+' || strInput[i] == '-')
			{
				if (i == 0)
				{
					i++;
				}
				else
				{
					strInput = strInput.Remove(i, 1);
				}
			}
			else if (strInput[i] == '.')
			{
				num++;
				i++;
				if (num != 1)
				{
					strInput = strInput.Remove(i - 1, 1);
				}
			}
			else
			{
				i++;
			}
		}
		float num2;
		if (strInput == string.Empty || !float.TryParse(strInput, out num2))
		{
			return strInput;
		}
		if (strInput[strInput.Length - 1] == '.')
		{
			return strInput;
		}
		fCompleteValue = num2;
		return null;
	}
}

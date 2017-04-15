using Com.Game.Data;
using Com.Game.Manager;
using Com.Game.Module;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class GetRandomStr
{
	public static string[] sSurnames;

	public static string[] sFirstNames;

	public static string CreateCheckCode()
	{
		char[] array = new char[]
		{
			'A',
			'B',
			'C',
			'D',
			'E',
			'F',
			'G',
			'H',
			'J',
			'K',
			'L',
			'M',
			'N',
			'O',
			'P',
			'Q',
			'R',
			'S',
			'T',
			'U',
			'V',
			'W',
			'X',
			'Y',
			'Z',
			'1',
			'2',
			'3',
			'4',
			'5',
			'6',
			'7',
			'8',
			'9'
		};
		string text = string.Empty;
		System.Random random = new System.Random();
		for (int i = 0; i < 5; i++)
		{
			text += array[random.Next(array.Length)];
		}
		return text;
	}

	public static string CreateCheckNumber()
	{
		char[] array = new char[]
		{
			'1',
			'2',
			'3',
			'4',
			'5',
			'6',
			'7',
			'8',
			'9'
		};
		string text = string.Empty;
		System.Random random = new System.Random();
		for (int i = 0; i < 9; i++)
		{
			text += array[random.Next(array.Length)];
		}
		return text;
	}

	public static string[] Name(int num)
	{
		if (GetRandomStr.sSurnames == null || GetRandomStr.sFirstNames == null)
		{
			GetRandomStr.IniName();
		}
		if (GetRandomStr.sSurnames == null || GetRandomStr.sFirstNames == null)
		{
			Debug.LogError("随机昵称出错");
			return null;
		}
		string[] array = new string[num];
		System.Random random = new System.Random();
		for (int i = 0; i < num; i++)
		{
			array[i] = GetRandomStr.sSurnames[random.Next(0, GetRandomStr.sSurnames.Length - 1)] + GetRandomStr.sFirstNames[random.Next(0, GetRandomStr.sFirstNames.Length - 1)];
		}
		return array;
	}

	private static void IniName()
	{
		Dictionary<string, object> dicByType = BaseDataMgr.instance.GetDicByType<SysSummonersNameVo>();
		if (dicByType == null || dicByType.Count == 0)
		{
			Singleton<TipView>.Instance.ShowViewSetText("配置表还没有加载完全==Name==SysSummonersNameVo", 1f);
			return;
		}
		List<SysSummonersNameVo> list = dicByType.Values.OfType<SysSummonersNameVo>().ToList<SysSummonersNameVo>();
		List<string> fistName = new List<string>();
		List<string> second = new List<string>();
		list.ForEach(delegate(SysSummonersNameVo obj)
		{
			fistName.Add(obj.adjectives);
			second.Add(obj.noun);
		});
		GetRandomStr.sSurnames = fistName.ToArray();
		GetRandomStr.sFirstNames = second.ToArray();
	}
}

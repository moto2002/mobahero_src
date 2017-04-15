using Com.Game.Data;
using Com.Game.Manager;
using MobaProtocol.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class Tools_ParsePrice
{
	public static List<float> ParsePrice(string price)
	{
		if (price == null)
		{
			return null;
		}
		string[] array = price.Split(new char[]
		{
			','
		});
		if (array != null || "[]" != array[0])
		{
			return Tools_ParsePrice.ParseTypes(array.Length, array);
		}
		return null;
	}

	private static List<float> ParseTypes(int num, string[] details)
	{
		List<float> list = new List<float>();
		if (num != 1)
		{
			if (num == 2)
			{
				list = Tools_ParsePrice.ParseDetails(details[0], details[1]);
				list.Add(2f);
			}
		}
		else
		{
			list = Tools_ParsePrice.ParseDetails(details[0], null);
			list.Add(1f);
		}
		return list;
	}

	private static List<float> ParseDetails(string details, string details_another = null)
	{
		List<float> list = new List<float>();
		if (details_another == null)
		{
			string[] array = details.Split(new char[]
			{
				'|'
			});
			if (array.Length > 1)
			{
				if (Tools_ParsePrice.ParseDiscount(float.Parse(array[2])) == 1)
				{
					list.Add((float)Tools_ParsePrice.ParseDiscount(float.Parse(array[2])));
					list.Add(float.Parse(array[2]));
					list.Add((float)int.Parse(array[0]));
					list.Add(float.Parse(array[2]) * (float)int.Parse(array[1]));
				}
				else
				{
					list.Add((float)Tools_ParsePrice.ParseDiscount(float.Parse(array[2])));
					list.Add((float)int.Parse(array[0]));
					list.Add(float.Parse(array[1]));
				}
			}
		}
		else
		{
			string[] array2 = details.Split(new char[]
			{
				'|'
			});
			string[] array3 = details_another.Split(new char[]
			{
				'|'
			});
			if (array2.Length > 1 && array3.Length > 1)
			{
				if (Tools_ParsePrice.ParseDiscount(float.Parse(array2[2])) == 1 || Tools_ParsePrice.ParseDiscount(float.Parse(array3[2])) == 1)
				{
					if (Tools_ParsePrice.ParseDiscount(float.Parse(array2[2])) == 1 && Tools_ParsePrice.ParseDiscount(float.Parse(array3[2])) == 1)
					{
						list.Add(1f);
						list.Add((float.Parse(array2[0]) != 2f) ? ((float.Parse(array3[0]) != 2f) ? float.Parse(array2[2]) : float.Parse(array3[2])) : float.Parse(array2[2]));
						list.Add((float.Parse(array2[0]) != 2f) ? ((float.Parse(array3[0]) != 2f) ? float.Parse(array2[0]) : float.Parse(array3[0])) : float.Parse(array2[0]));
						list.Add((float)((float.Parse(array2[0]) != 2f) ? ((float.Parse(array3[0]) != 2f) ? (int.Parse(array2[1]) * Tools_ParsePrice.ParseDiscount(float.Parse(array2[2]))) : (int.Parse(array3[1]) * Tools_ParsePrice.ParseDiscount(float.Parse(array3[2])))) : (int.Parse(array2[1]) * Tools_ParsePrice.ParseDiscount(float.Parse(array2[2])))));
					}
					else
					{
						list.Add((float)((Tools_ParsePrice.ParseDiscount(float.Parse(array2[2])) != 1) ? Tools_ParsePrice.ParseDiscount(float.Parse(array3[2])) : Tools_ParsePrice.ParseDiscount(float.Parse(array2[2]))));
						list.Add((Tools_ParsePrice.ParseDiscount(float.Parse(array2[2])) != 1) ? float.Parse(array3[2]) : float.Parse(array2[2]));
						list.Add((float)((Tools_ParsePrice.ParseDiscount(float.Parse(array2[2])) != 1) ? int.Parse(array3[0]) : int.Parse(array2[0])));
						list.Add((float)((Tools_ParsePrice.ParseDiscount(float.Parse(array2[2])) != 1) ? (int.Parse(array3[1]) * Tools_ParsePrice.ParseDiscount(float.Parse(array3[2]))) : (int.Parse(array2[1]) * Tools_ParsePrice.ParseDiscount(float.Parse(array2[2])))));
					}
				}
				else
				{
					list.Add((float)Tools_ParsePrice.ParseDiscount(float.Parse(array2[2])));
					list.Add((float)int.Parse(array2[0]));
					list.Add((float)int.Parse(array3[0]));
					list.Add((float)int.Parse(array2[1]));
					list.Add((float)int.Parse(array3[1]));
				}
			}
		}
		return list;
	}

	private static int ParseDiscount(float discount)
	{
		if (discount == 1f)
		{
			return 0;
		}
		return 1;
	}

	public static string StringPrice(List<GoodsData> goodsData)
	{
		if (goodsData.Count == 1)
		{
			return goodsData[0].Price;
		}
		List<float> info = Tools_ParsePrice.ParsePrice(goodsData[0].Price);
		List<float> info_another = Tools_ParsePrice.ParsePrice(goodsData[1].Price);
		int index = Tools_ParsePrice.CompareItems(info, info_another);
		return goodsData[index].Price;
	}

	private static int CompareItems(List<float> info, List<float> info_another)
	{
		int result;
		if (info[0] == 1f || info_another[0] == 1f)
		{
			result = ((info[0] != 1f) ? 1 : 0);
		}
		else if (info[1] != info_another[1])
		{
			if (info[1] == 9f || info_another[1] == 9f)
			{
				result = ((info[1] != 9f) ? 0 : 1);
			}
			else if (info[1] > info_another[1])
			{
				result = 0;
			}
			else
			{
				result = 1;
			}
		}
		else if (info[1] == 9f || info_another[1] == 9f)
		{
			result = ((info[1] != 9f) ? 0 : 1);
		}
		else if (info[1] > info_another[1])
		{
			result = 0;
		}
		else
		{
			result = 1;
		}
		return result;
	}

	public static long BottleMaxExp(int level)
	{
		long result = 0L;
		Dictionary<string, SysMagicbottleExpVo> typeDicByType = BaseDataMgr.instance.GetTypeDicByType<SysMagicbottleExpVo>();
		if (typeDicByType != null)
		{
			result = (long)typeDicByType[Tools_ParsePrice.Level_Check<SysMagicbottleExpVo>(level, typeDicByType, false).ToString()].exp;
		}
		return result;
	}

	public static string BottleLevelParse(int level)
	{
		Dictionary<string, SysMagicbottleExpVo> typeDicByType = BaseDataMgr.instance.GetTypeDicByType<SysMagicbottleExpVo>();
		int num = Tools_ParsePrice.Level_Check<SysMagicbottleExpVo>(level, typeDicByType, false);
		if (typeDicByType != null)
		{
			return typeDicByType[num.ToString()].largeIcon;
		}
		return null;
	}

	public static string BottleRewardParse(int level)
	{
		Dictionary<string, SysMagicbottleRankrewardsVo> typeDicByType = BaseDataMgr.instance.GetTypeDicByType<SysMagicbottleRankrewardsVo>();
		int num = Tools_ParsePrice.Level_Check<SysMagicbottleRankrewardsVo>(level, typeDicByType, false);
		if (typeDicByType != null)
		{
			return num.ToString();
		}
		return null;
	}

	public static int Level_Check<T>(int level, Dictionary<string, T> x_info, bool isDown = false)
	{
		int result = 0;
		List<string> list = new List<string>();
		Dictionary<string, T>.KeyCollection keys = x_info.Keys;
		foreach (string current in keys)
		{
			list.Add(current);
		}
		for (int i = list.Count - 1; i >= 0; i--)
		{
			if (level >= int.Parse(list[i]))
			{
				if (isDown)
				{
					if (i != list.Count - 1)
					{
						result = int.Parse(list[i + 1]);
					}
				}
				else
				{
					result = int.Parse(list[i]);
				}
				break;
			}
			result = int.Parse(list[0]);
		}
		return result;
	}

	public static int MaxLevelCheck<T>(Dictionary<string, T> x_info, bool isFirst = false)
	{
		List<string> list = new List<string>();
		Dictionary<string, T>.KeyCollection keys = x_info.Keys;
		int result = int.Parse(keys.Last<string>());
		if (isFirst)
		{
			result = int.Parse(keys.First<string>());
		}
		return result;
	}

	public static int ParseCollectRange(int level, bool isDown = false)
	{
		int result = 0;
		Dictionary<string, SysMagicbottleLevelVo> dictionary = new Dictionary<string, SysMagicbottleLevelVo>();
		dictionary = BaseDataMgr.instance.GetTypeDicByType<SysMagicbottleLevelVo>();
		if (dictionary != null)
		{
			result = Tools_ParsePrice.Level_Check<SysMagicbottleLevelVo>(level, dictionary, isDown);
		}
		return result;
	}

	public static bool IsCollectorBottleUp(int levelL, int levelC)
	{
		Dictionary<string, SysMagicbottleLevelVo> typeDicByType = BaseDataMgr.instance.GetTypeDicByType<SysMagicbottleLevelVo>();
		int num = Tools_ParsePrice.MaxLevelCheck<SysMagicbottleLevelVo>(typeDicByType, false);
		if (levelL >= num && levelC >= num)
		{
			SysMagicbottleLevelVo dataById = BaseDataMgr.instance.GetDataById<SysMagicbottleLevelVo>(num.ToString());
			int lastLevelDefference = dataById.lastLevelDefference;
			return (levelC - num) % lastLevelDefference == 0;
		}
		if (levelC == Tools_ParsePrice.MaxLevelCheck<SysMagicbottleLevelVo>(typeDicByType, true))
		{
			return true;
		}
		int num2 = Tools_ParsePrice.Level_Check<SysMagicbottleLevelVo>(levelL, typeDicByType, false);
		int num3 = Tools_ParsePrice.Level_Check<SysMagicbottleLevelVo>(levelC, typeDicByType, false);
		return num2 != num3;
	}

	public static List<SysGameItemsVo> ParseGameItemData(int type, Dictionary<string, SysGameItemsVo> xInfo)
	{
		List<SysGameItemsVo> list = new List<SysGameItemsVo>();
		foreach (KeyValuePair<string, SysGameItemsVo> current in xInfo)
		{
			if (current.Value.type == type)
			{
				list.Add(current.Value);
			}
		}
		return list;
	}

	public static Color ParseColorLevel(int quality)
	{
		Color result = default(Color);
		switch (quality)
		{
		}
		return result;
	}
}

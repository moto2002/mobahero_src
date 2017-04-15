using MobaHeros;
using System;
using System.Collections.Generic;
using UnityEngine;

public class TagManager
{
	public const string TAG_MONSTER = "Monster";

	public const string TAG_PLAYER = "Player";

	public const string TAG_HERO = "Hero";

	public const string TAG_TOWER = "Building";

	public const string TAG_HOME = "Home";

	public const string TAG_MAP = "Map";

	public const string TAG_ITEM = "Item";

	public const string TAG_BUFF = "BuffItem";

	public const string TAG_DangBan = "DangBan";

	public const string OBJ_SCREENPOINT = "Target";

	public static bool CheckTag(Units target, TargetTag targetTag)
	{
		if (target != null)
		{
			int dataInt = target.data.GetDataInt(DataType.ItemType);
			switch (targetTag)
			{
			case TargetTag.Tower:
				if (dataInt == 2)
				{
					return true;
				}
				return false;
			case TargetTag.Creeps:
				if (dataInt == 3 || dataInt == 7 || dataInt == 9)
				{
					return true;
				}
				return false;
			case TargetTag.Minions:
				if (dataInt == 1)
				{
					return true;
				}
				return false;
			case TargetTag.CreepsAndMinions:
				if (dataInt == 1 || dataInt == 3 || dataInt == 7 || dataInt == 9)
				{
					return true;
				}
				return false;
			case TargetTag.TowerAndHome:
				if (dataInt == 2 || dataInt == 4)
				{
					return true;
				}
				return false;
			case TargetTag.EyeUnit:
				if (dataInt == 8)
				{
					return true;
				}
				return false;
			case TargetTag.Pet:
				if (dataInt == 10)
				{
					return true;
				}
				return false;
			case TargetTag.Labisi:
				if (dataInt == 11)
				{
					return true;
				}
				return false;
			}
			if (TagManager.CheckTagType(target.tag, targetTag))
			{
				return true;
			}
		}
		return false;
	}

	public static bool CheckTag(GameObject target, TargetTag targetTag)
	{
		return target != null && TagManager.CheckTagType(target.tag, targetTag);
	}

	private static bool CheckTagType(string tag, TargetTag tagType)
	{
		switch (tagType)
		{
		case TargetTag.All:
			return true;
		case TargetTag.Hero:
			return tag.Equals("Hero") || tag.Equals("Player");
		case TargetTag.Monster:
			return tag.Equals("Monster");
		case TargetTag.Tower:
			return tag.Equals("Building");
		case TargetTag.Home:
			return tag.Equals("Home");
		case TargetTag.BuffItem:
			return tag.Equals("BuffItem");
		case TargetTag.Item:
			return tag.Equals("Item");
		case TargetTag.HeroAndMonster:
			return tag.Equals("Hero") || tag.Equals("Player") || tag.Equals("Monster");
		case TargetTag.Building:
			return tag.Equals("Home") || tag.Equals("Building");
		case TargetTag.Player:
			return tag.Equals("Player");
		case TargetTag.Creeps:
			return tag.Equals("Monster");
		case TargetTag.Minions:
			return tag.Equals("Monster");
		case TargetTag.CreepsAndMinions:
			return tag.Equals("Monster");
		default:
			return false;
		}
	}

	public static List<int> GetTagType(string tag)
	{
		List<int> list = new List<int>();
		switch (tag)
		{
		case "Hero":
			list.Add(2);
			list.Add(8);
			list.Add(1);
			break;
		case "Player":
			list.Add(10);
			list.Add(2);
			list.Add(8);
			list.Add(1);
			break;
		case "Home":
			list.Add(5);
			list.Add(9);
			list.Add(1);
			break;
		case "Building":
			list.Add(4);
			list.Add(9);
			list.Add(1);
			break;
		case "Monster":
			list.Add(3);
			list.Add(8);
			list.Add(1);
			list.Add(11);
			list.Add(13);
			list.Add(12);
			break;
		case "BuffItem":
			list.Add(6);
			list.Add(1);
			break;
		case "Item":
			list.Add(7);
			list.Add(1);
			break;
		}
		return list;
	}

	public static int GetTagCount(Dictionary<string, List<int>> list, int tagType)
	{
		int num = 0;
		switch (tagType)
		{
		case 1:
		{
			Dictionary<string, List<int>>.Enumerator enumerator = list.GetEnumerator();
			while (enumerator.MoveNext())
			{
				int arg_1DC_0 = num;
				KeyValuePair<string, List<int>> current = enumerator.Current;
				num = arg_1DC_0 + current.Value.Count;
			}
			break;
		}
		case 2:
			if (list.ContainsKey("Hero"))
			{
				num += list["Hero"].Count;
			}
			break;
		case 3:
			if (list.ContainsKey("Monster"))
			{
				num += list["Monster"].Count;
			}
			break;
		case 4:
			if (list.ContainsKey("Building"))
			{
				num += list["Building"].Count;
			}
			break;
		case 5:
			if (list.ContainsKey("Home"))
			{
				num += list["Home"].Count;
			}
			break;
		case 6:
			if (list.ContainsKey("BuffItem"))
			{
				num += list["BuffItem"].Count;
			}
			break;
		case 7:
			if (list.ContainsKey("Item"))
			{
				num += list["Item"].Count;
			}
			break;
		case 8:
			if (list.ContainsKey("Hero"))
			{
				num += list["Hero"].Count;
			}
			if (list.ContainsKey("Monster"))
			{
				num += list["Monster"].Count;
			}
			break;
		case 9:
			if (list.ContainsKey("Home"))
			{
				num += list["Home"].Count;
			}
			if (list.ContainsKey("Building"))
			{
				num += list["Building"].Count;
			}
			break;
		}
		return num;
	}

	public static bool IsCharacterTarget(GameObject obj)
	{
		return obj != null && (obj.CompareTag("Monster") || obj.CompareTag("Player") || obj.CompareTag("Hero"));
	}

	public static bool IsAttackTarget(GameObject obj)
	{
		return obj != null && (obj.CompareTag("Monster") || obj.CompareTag("Player") || obj.CompareTag("Hero") || obj.CompareTag("Home") || obj.CompareTag("Building"));
	}

	public static bool IsColliderTarget(GameObject obj)
	{
		return obj != null && (obj.CompareTag("Monster") || obj.CompareTag("Player") || obj.CompareTag("Hero") || obj.CompareTag("Home") || obj.CompareTag("Building") || obj.CompareTag("Item") || obj.CompareTag("BuffItem"));
	}

	public static bool IsTargetObject(GameObject hit)
	{
		return hit != null && (hit.CompareTag("Monster") || hit.CompareTag("Player") || hit.CompareTag("Hero") || hit.CompareTag("Home") || hit.CompareTag("Building"));
	}

	public static bool CompareTag(Units owner, Units target)
	{
		string text = owner.tag;
		if (text.Equals("Player"))
		{
			text = "Hero";
		}
		string text2 = target.tag;
		if (text2.Equals("Player"))
		{
			text2 = "Hero";
		}
		return text.Equals(text2);
	}
}

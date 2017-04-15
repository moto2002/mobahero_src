using Assets.Scripts.GUILogic.View.HomeChatView;
using Com.Game.Module;
using MobaServer;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

public static class Tools_StringCheck
{
	private static List<EmotionData> listEmotionData = new List<EmotionData>();

	public static bool IsChineseCharacter(this ToolsFacade facade, char c)
	{
		return c > '\u007f';
	}

	public static int WordsCounter(this ToolsFacade facade, string _input)
	{
		int num = 0;
		char[] array = _input.ToCharArray();
		for (int i = 0; i < array.Length; i++)
		{
			if (facade.IsChineseCharacter(array[i]))
			{
				num += 2;
			}
			else
			{
				num++;
			}
		}
		return num;
	}

	public static bool IsLegalString(this ToolsFacade facade, ref string _str)
	{
		_str = FilterWorder.Instance.ReplaceKeyword(_str);
		bool result = true;
		if (string.IsNullOrEmpty(_str))
		{
			result = false;
		}
		byte[] bytes = Encoding.Default.GetBytes(_str);
		string @string = Encoding.UTF8.GetString(bytes);
		if (string.IsNullOrEmpty(@string))
		{
			result = false;
			Singleton<TipView>.Instance.ShowViewSetText("含有非法字符！！！", 1f);
		}
		string value = _str.Trim();
		if (string.IsNullOrEmpty(value))
		{
			result = false;
			Singleton<TipView>.Instance.ShowViewSetText("不能发送全为空格的信息！！！", 1f);
		}
		return result;
	}

	public static string CheckEmotion(string str, UIAtlas atlas)
	{
		Tools_StringCheck.listEmotionData.Clear();
		int length = str.Length;
		int num = -6;
		BetterList<string> listOfSprites = atlas.GetListOfSprites();
		for (int num2 = 0; num2 != length; num2++)
		{
			if (str[num2] == '#' && num2 + 4 < length && str[num2 + 1] == 'e')
			{
				EmotionData item = default(EmotionData);
				item.serialNumber = str.Substring(num2 + 2, 3);
				if (listOfSprites.Contains(item.serialNumber))
				{
					if (Tools_StringCheck.IsShowVIPStr(str, num2))
					{
						int num3 = (num != 0) ? (num + 6) : (num + 7);
						item.strContent = str.Substring(num3, num2 + 5 - num3).Replace(str.Substring(num2, 5), (num2 != 0) ? "      " : "       ");
						item.position = num2;
						item.row = 1;
						item.totalConSpace = item.strContent.Length;
						item.totalNonSpace = (float)(str.Substring(num3, num2 + 5 - num3).Length - 5) + 1.5f;
						Tools_StringCheck.listEmotionData.Add(item);
						num = num2;
						Regex regex = new Regex(str.Substring(num2, 5));
						str = regex.Replace(str, (num2 != 0) ? "      " : "       ", 1);
						length = str.Length;
					}
				}
			}
		}
		string text = string.Empty;
		if (Tools_StringCheck.listEmotionData != null && Tools_StringCheck.listEmotionData.Count != 0)
		{
			if (Tools_StringCheck.listEmotionData[Tools_StringCheck.listEmotionData.Count - 1].position == 0)
			{
				if (str.Length != 7)
				{
					text = str.Substring(Tools_StringCheck.listEmotionData[Tools_StringCheck.listEmotionData.Count - 1].position + 7);
				}
			}
			else
			{
				text = str.Substring(Tools_StringCheck.listEmotionData[Tools_StringCheck.listEmotionData.Count - 1].position + 6);
			}
		}
		else
		{
			text = str;
		}
		string text2 = null;
		for (int num4 = 0; num4 != Tools_StringCheck.listEmotionData.Count; num4++)
		{
			text2 += Tools_StringCheck.listEmotionData[num4].strContent;
		}
		if (!string.IsNullOrEmpty(text))
		{
			text2 += text;
		}
		return text2;
	}

	public static void GenerateEmotion(UILabel chatContent, UIAtlas atlas, Transform emojiHitch)
	{
		BetterList<Vector3> betterList = new BetterList<Vector3>();
		BetterList<int> indices = new BetterList<int>();
		List<GameObject> list = new List<GameObject>();
		chatContent.pivot = UIWidget.Pivot.Left;
		chatContent.UpdateNGUIText();
		NGUIText.PrintCharacterPositions(chatContent.text, betterList, indices);
		int num = 0;
		for (int num2 = 0; num2 != Tools_StringCheck.listEmotionData.Count; num2++)
		{
			GameObject gameObject = new GameObject();
			UISprite uISprite = gameObject.AddComponent<UISprite>();
			if (null != emojiHitch)
			{
				uISprite.transform.parent = emojiHitch;
			}
			uISprite.depth = chatContent.depth + 1;
			uISprite.transform.localScale = new Vector3(1f, 1f, 1f);
			uISprite.name = "emotion" + num2;
			uISprite.atlas = atlas;
			uISprite.spriteName = Tools_StringCheck.listEmotionData[num2].serialNumber;
			uISprite.SetDimensions(38, 38);
			if (Tools_StringCheck.listEmotionData != null && Tools_StringCheck.listEmotionData.Count > 1)
			{
				if (Tools_StringCheck.listEmotionData[0].position == 0 && num2 != 0)
				{
					num = -8;
				}
				else
				{
					num = 0;
				}
			}
			int i = (2 * Tools_StringCheck.listEmotionData[num2].position != 0) ? (2 * Tools_StringCheck.listEmotionData[num2].position - 1) : 0;
			uISprite.transform.localPosition = new Vector3(chatContent.transform.localPosition.x, 0f, chatContent.transform.localPosition.z) + betterList[i] + new Vector3((float)(uISprite.width / 2 + -(float)chatContent.spacingX + num), (float)(-(float)chatContent.spacingY / 2 + 21), 0f);
		}
	}

	private static bool IsShowVIPStr(string str, int i)
	{
		string text = str.Substring(i, 5);
		return i == 0 || text.CompareTo("#e062") != 0;
	}

	public static string GetMillionsSuffix(this ToolsFacade facade, int _arg)
	{
		if (_arg < 1000000)
		{
			return _arg.ToString();
		}
		return ((float)_arg / 1000000f).ToString("F2") + "M";
	}

	public static string GetThousandsSuffix(this ToolsFacade facade, int _arg)
	{
		if (_arg < 1000)
		{
			return _arg.ToString();
		}
		return ((float)_arg / 1000f).ToString("F1") + "K";
	}

	public static string GetMathSuffix(this ToolsFacade facade, int _arg)
	{
		if (_arg < 1000)
		{
			return _arg.ToString();
		}
		if (_arg < 1000000)
		{
			return facade.GetThousandsSuffix(_arg);
		}
		return facade.GetMillionsSuffix(_arg);
	}
}

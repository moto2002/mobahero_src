using System;
using UnityEngine;

public class SkillShowItem : MonoBehaviour
{
	[SerializeField]
	private UITexture S_Texture;

	[SerializeField]
	private UILabel S_Name;

	[SerializeField]
	private UILabel S_Type;

	[SerializeField]
	private UILabel S_Introduction;

	[SerializeField]
	private UILabel S_Lock;

	[SerializeField]
	private UILabel S_CD;

	public void Init(Texture textue, string name, string type, string str, bool isLock, string cd)
	{
		this.S_Texture.mainTexture = textue;
		this.S_Name.text = LanguageManager.Instance.GetStringById(name);
		this.S_Type.text = type;
		this.S_Introduction.text = str;
		if (isLock)
		{
			this.S_Lock.text = "未解锁";
		}
		else
		{
			this.S_Lock.text = "已解锁";
		}
		if (cd == "0")
		{
			this.S_CD.text = string.Empty;
		}
		else
		{
			this.S_CD.text = "CD " + cd + "s";
		}
	}

	private string FixStr(string str, int lineLength = 11)
	{
		string a = string.Empty;
		string text = string.Empty;
		char[] array = str.ToCharArray();
		NGUIText.WrapText(str, out str);
		string[] array2 = str.Split(new char[]
		{
			'\n'
		});
		Debug.Log("lines = " + array2.Length);
		for (int i = 0; i < array2.Length; i++)
		{
			a = array2[i].PadLeft(1);
			if (a == "，" || a == "。" || a == "！" || a == "）" || a == "？" || a == "’" || a == ">" || a == "》" || a == "+" || a == "-")
			{
				text = text.Insert(text.Length - 2, array2[i].PadLeft(1)) + array2[i].Remove(0, 1);
			}
			else
			{
				text += array2[i];
			}
		}
		return text;
	}
}

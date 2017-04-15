using Com.Game.Data;
using Com.Game.Manager;
using Com.Game.Utils;
using System;
using System.Text.RegularExpressions;
using UnityEngine;

public class LanguageManager
{
	private SysLanguageVo tampLanguageVo;

	private string txt = string.Empty;

	private static LanguageManager _instance;

	public bool getDataReady;

	private Font MiddleBlack;

	private Font SpecialBlack;

	public static LanguageManager Instance
	{
		get
		{
			if (LanguageManager._instance == null)
			{
				LanguageManager._instance = new LanguageManager();
			}
			return LanguageManager._instance;
		}
	}

	public string GetStringById(string id)
	{
		this.txt = string.Empty;
		if (this.getDataReady)
		{
			this.tampLanguageVo = BaseDataMgr.instance.GetLanguageData(id);
			if (this.tampLanguageVo == null)
			{
				ClientLogger.Error("语言包中找不到语言id为* " + id + " *的文本内容，详情请咨询相关策划在检查Language之后");
				Match match = Regex.Match(id, "[\\u4e00-\\u9fa5]");
				if (match.Success)
				{
					this.txt = id;
				}
				else
				{
					this.txt = string.Empty;
				}
			}
			else
			{
				this.txt = this.tampLanguageVo.content;
			}
		}
		this.FixBrokenWord(this.txt, 1);
		return this.txt;
	}

	public string GetStringById(string id, string defaultStr)
	{
		string stringById = this.GetStringById(id);
		return (!string.IsNullOrEmpty(stringById)) ? stringById : defaultStr;
	}

	public string FormatString(string id, params object[] args)
	{
		string stringById = this.GetStringById(id);
		string result = stringById;
		try
		{
			if (stringById != null)
			{
				result = string.Format(stringById, args);
			}
		}
		catch (Exception var_2_1D)
		{
			ClientLogger.Error("FormatString failed for #" + id);
		}
		return result;
	}

	public void FixBrokenWord(string chineseTxt, int fontType = 1)
	{
		if (fontType == 1)
		{
			if (this.MiddleBlack == null)
			{
				this.MiddleBlack = (Resources.Load("Font/Lanting-MiddleBlack") as Font);
			}
			this.MiddleBlack.RequestCharactersInTexture(chineseTxt);
		}
		else
		{
			if (this.SpecialBlack == null)
			{
				this.SpecialBlack = (Resources.Load("Font/Lanting-SpecialBlack") as Font);
			}
			this.SpecialBlack.RequestCharactersInTexture(chineseTxt);
		}
	}
}

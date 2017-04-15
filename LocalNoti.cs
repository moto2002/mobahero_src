using Com.Game.Data;
using Com.Game.Manager;
using System;
using UnityEngine;

public class LocalNoti
{
	private static string isOpenKey = "IsOpenKey";

	public static string StrById(string strId)
	{
		SysLanguageVo languageData = BaseDataMgr.instance.GetLanguageData(strId);
		if (languageData != null)
		{
			return languageData.content;
		}
		return string.Empty;
	}

	public static void SetOpenState(bool isOpen)
	{
		if (!isOpen)
		{
			APush.StopAndriodPush();
		}
		else
		{
			APush.ResumeAndriodPush();
		}
		int value = (!isOpen) ? 0 : 1;
		PlayerPrefs.SetInt(LocalNoti.isOpenKey, value);
	}

	public static bool CheckOpen()
	{
		if (!PlayerPrefs.HasKey(LocalNoti.isOpenKey))
		{
			return true;
		}
		int @int = PlayerPrefs.GetInt(LocalNoti.isOpenKey);
		return @int == 1;
	}
}

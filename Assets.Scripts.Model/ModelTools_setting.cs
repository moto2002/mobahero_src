using Com.Game.Module;
using Newbie;
using System;
using UnityEngine;

namespace Assets.Scripts.Model
{
	public static class ModelTools_setting
	{
		public static SettingModelData Get_SettingData(this ModelManager mmng)
		{
			SettingModelData result = null;
			if (mmng != null)
			{
				if (NewbieManager.Instance.IsNewbieSysSetting())
				{
					result = new SettingModelData();
				}
				else
				{
					result = mmng.GetData<SettingModelData>(EModelType.Model_setting);
				}
			}
			return result;
		}

		public static void Initialize_SettingData(this ModelManager mmng)
		{
			SettingModelData settingModelData = mmng.Get_SettingData();
			if (settingModelData != null)
			{
				settingModelData.Load();
			}
		}

		public static void Save_SettingData(this ModelManager mmng)
		{
			SettingModelData settingModelData = mmng.Get_SettingData();
			if (settingModelData != null)
			{
				settingModelData.Record();
			}
		}

		public static void Apply_SettingDataInBattle(this ModelManager mmng)
		{
			SettingModelData settingModelData = mmng.Get_SettingData();
			Units player = PlayerControlMgr.Instance.GetPlayer();
			if (settingModelData != null && player != null)
			{
				if (settingModelData.cameraHigh)
				{
					BattleCameraMgr.Instance.ChangeCamera(2);
				}
				else
				{
					BattleCameraMgr.Instance.ChangeCamera(1);
				}
				GlobalSettings.Instance.isShowFPS = settingModelData.showFPS;
				GlobalSettings.Instance.isHighFPS = settingModelData.highFPS;
				if (settingModelData.crazyCastingSkill)
				{
					PlayerPrefs.SetInt("IsCrazyMode", 1);
					GlobalSettings.Instance.isCrazyMode = true;
					player.SetCrazyMode();
				}
				else
				{
					PlayerPrefs.SetInt("IsCrazyMode", 0);
					GlobalSettings.Instance.isCrazyMode = false;
					player.SetNormalMode();
				}
				if (settingModelData.selectDyingHero)
				{
					GlobalSettings.Instance.AttackSortType = SortType.Blood;
				}
				else
				{
					GlobalSettings.Instance.AttackSortType = SortType.Distance;
				}
				GlobalSettings.Instance.ChaOutlineLevel = (int)settingModelData.characterOutlineList;
				PlayerPrefs.SetInt("ChaOutlineLevel", (int)settingModelData.characterOutlineList);
				Singleton<HUDModuleManager>.Instance.skillPanelPivot = (SkillPanelPivot)settingModelData.skillPanelPivot;
				PlayerPrefs.Save();
			}
		}

		public static void Apply_SettingData(this ModelManager mmng)
		{
			SettingModelData settingModelData = mmng.Get_SettingData();
			if (settingModelData != null)
			{
				mmng.Set_Setting_QualityLevel(settingModelData.qualityLevel);
			}
		}

		public static void Set_Setting_QualityLevel(this ModelManager mmng, MobaQualityLevel level)
		{
			SettingModelData settingModelData = mmng.Get_SettingData();
			if (settingModelData != null)
			{
				if (level == settingModelData.qualityLevel)
				{
					return;
				}
				settingModelData.qualityLevel = level;
				PlayerPrefs.SetInt("MobaQualityLevel", (int)level);
				ModelManager.Instance.Get_SettingData().qualityLevel = level;
				GlobalSettings.Instance.QualitySetting.SetLevel(settingModelData.qualityLevel);
				MobaMessage message = MobaMessageManager.GetMessage((ClientMsg)23078, level, 0f);
				MobaMessageManager.ExecuteMsg(message);
				if (!settingModelData.temp_ShadowOn || settingModelData.qualityLevel != MobaQualityLevel.P1080)
				{
				}
			}
		}

		public static void Apply_QualityLevel(this ModelManager mmng)
		{
			SettingModelData settingModelData = mmng.Get_SettingData();
			if (settingModelData != null)
			{
				GlobalSettings.Instance.QualitySetting.SetLevelEx(settingModelData.qualityLevel);
			}
		}

		public static void Set_Setting_TempShadowOn(this ModelManager mmng)
		{
			SettingModelData settingModelData = mmng.Get_SettingData();
			settingModelData.temp_ShadowOn = true;
			if (settingModelData != null && settingModelData.qualityLevel != MobaQualityLevel.P1080 && settingModelData.qualityLevel != MobaQualityLevel.Original)
			{
				QualitySettings.SetQualityLevel(6);
			}
		}

		public static void Set_Setting_TempShadowOff(this ModelManager mmng)
		{
			SettingModelData settingModelData = mmng.Get_SettingData();
			settingModelData.temp_ShadowOn = false;
			if (settingModelData != null && settingModelData.qualityLevel != MobaQualityLevel.P1080 && settingModelData.qualityLevel != MobaQualityLevel.Original)
			{
				QualitySettings.SetQualityLevel(7);
				GlobalSettings.Instance.QualitySetting.SetLevel(settingModelData.qualityLevel);
			}
		}
	}
}

using System;
using UnityEngine;

[Serializable]
public class MobaQualitySetting
{
	public int OldLevel = -1;

	private float oldAspect = -1f;

	public void SetLevel(MobaQualityLevel _level)
	{
		if (this.OldLevel != (int)_level)
		{
			this.OldLevel = (int)_level;
			this.SetLevelEx(_level);
		}
	}

	public void SetLevelEx(MobaQualityLevel _level)
	{
		if (this.oldAspect == -1f)
		{
			this.oldAspect = (float)Screen.width / (float)Screen.height;
		}
		if (_level == MobaQualityLevel.Original)
		{
			this.SetOriginalLevel();
		}
		else if (_level == MobaQualityLevel.P1080)
		{
			this.SetHighLevel();
		}
		else
		{
			this.SetLowLevel();
		}
	}

	private void SetOriginalLevel()
	{
		QualitySettings.SetQualityLevel(7);
		this.SetCameraAspect(true);
		float num = 0f;
		float num2 = 0f;
		if (!Application.isEditor)
		{
			num = (float)Screen.width;
			num2 = (float)Screen.height;
			if (num < 1920f)
			{
				num2 = 1920f * num2 / num;
				num = 1920f;
			}
		}
		Screen.SetResolution((int)num, (int)num2, true);
		GlobalSettings.SetFogMode(0);
	}

	private void SetHighLevel()
	{
		QualitySettings.SetQualityLevel(7);
		this.SetCameraAspect(true);
		int num = 1920;
		int height;
		if (GlobalSettings.ResWidth < 1920)
		{
			num = GlobalSettings.ResWidth;
			height = GlobalSettings.ResHeight;
		}
		else
		{
			height = num * GlobalSettings.ResHeight / GlobalSettings.ResWidth;
		}
		Screen.SetResolution(num, height, true);
		GlobalSettings.SetFogMode(0);
	}

	private void SetLowLevel()
	{
		QualitySettings.SetQualityLevel(6);
		this.SetCameraAspect(false);
		int num = 1280;
		int height;
		if (GlobalSettings.ResWidth < 1280)
		{
			num = GlobalSettings.ResWidth;
			height = GlobalSettings.ResHeight;
		}
		else
		{
			height = num * GlobalSettings.ResHeight / GlobalSettings.ResWidth;
		}
		Screen.SetResolution(num, height, true);
		GlobalSettings.SetFogMode(0);
	}

	private void SetCameraAspect(bool highLevel)
	{
	}

	private void InitLevel(MobaQualityLevel mobaQualityLevel)
	{
		this.InitShadow(mobaQualityLevel);
		this.InitResolution(mobaQualityLevel);
	}

	private void InitFog(MobaQualityLevel mobaQualityLevel)
	{
		if (mobaQualityLevel == MobaQualityLevel.P1080)
		{
			GlobalSettings.SetFogMode(0);
		}
		else
		{
			GlobalSettings.SetFogMode(0);
		}
	}

	private void InitShadow(MobaQualityLevel mobaQualityLevel)
	{
		if (mobaQualityLevel == MobaQualityLevel.P1080 || mobaQualityLevel == MobaQualityLevel.SuperHd)
		{
			QualitySettings.SetQualityLevel(6);
		}
		else
		{
			QualitySettings.SetQualityLevel(7);
		}
	}

	private void InitResolution(MobaQualityLevel mobaQualityLevel)
	{
		switch (mobaQualityLevel)
		{
		case MobaQualityLevel.Fast:
			Screen.SetResolution(640, Screen.height * 640 / Screen.width, true);
			break;
		case MobaQualityLevel.Hd:
			Screen.SetResolution(960, Screen.height * 960 / Screen.width, true);
			break;
		case MobaQualityLevel.SuperHd:
			Screen.SetResolution(1280, Screen.height * 1280 / Screen.width, true);
			break;
		case MobaQualityLevel.P1080:
			Screen.SetResolution(1920, Screen.height * 1920 / Screen.width, true);
			break;
		case MobaQualityLevel.Original:
			if (Screen.width > 1920)
			{
				float num = (float)Screen.width;
				float num2 = (float)Screen.height;
				if (num < 1920f)
				{
					num2 = 1920f * num2 / num;
					Screen.SetResolution(Screen.width, Screen.height, true);
				}
				else
				{
					Screen.SetResolution(Screen.width, Screen.height, true);
				}
			}
			else
			{
				Screen.SetResolution(1920, Screen.height * 1920 / Screen.width, true);
			}
			break;
		}
	}
}

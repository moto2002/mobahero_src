using System;
using UnityEngine;

namespace Assets.Scripts.Model
{
	public class SettingModelData
	{
		private const byte recordNum = 13;

		public MobaQualityLevel qualityLevel;

		public bool characterOutline;

		public bool showFPS;

		public bool highFPS;

		public bool cameraHigh;

		public bool recommendOn;

		public bool crazyCastingSkill;

		public bool selectDyingHero;

		public byte bgm;

		public byte sfx;

		public byte voice;

		public bool detailedShop;

		public byte skillPanelPivot;

		public byte characterOutlineList;

		public bool temp_ShadowOn;

		public bool IsHighQualitySetting
		{
			get
			{
				return this.qualityLevel == MobaQualityLevel.P1080 || this.qualityLevel == MobaQualityLevel.SuperHd || this.qualityLevel == MobaQualityLevel.Original;
			}
		}

		public SettingModelData()
		{
			this.SetDefault();
		}

		public void SetDefault()
		{
			this.qualityLevel = MobaQualityLevel.P1080;
			this.characterOutlineList = 2;
			this.showFPS = true;
			this.cameraHigh = true;
			this.recommendOn = true;
			this.crazyCastingSkill = true;
			this.selectDyingHero = false;
			this.bgm = (byte)AudioMgr.getVolumeBG();
			this.sfx = (byte)AudioMgr.getVolumeEff();
			this.voice = (byte)AudioMgr.getVolumeVoice();
			this.detailedShop = false;
			this.skillPanelPivot = 0;
			this.highFPS = false;
		}

		public void Record()
		{
			string value = string.Empty;
			string arg_13E_0 = "{0}|{1}|{2}|{3}|{4}|{5}|{6}|{7}|{8}|{9}|{10}|{11}|{12}";
			object[] expr_12 = new object[13];
			int arg_22_1 = 0;
			int num = (int)this.qualityLevel;
			expr_12[arg_22_1] = num.ToString();
			int arg_33_1 = 1;
			int num2 = (int)this.characterOutlineList;
			expr_12[arg_33_1] = num2.ToString();
			expr_12[2] = ((!this.showFPS) ? "0" : "1");
			expr_12[3] = ((!this.cameraHigh) ? "0" : "1");
			expr_12[4] = ((!this.crazyCastingSkill) ? "0" : "1");
			expr_12[5] = ((!this.selectDyingHero) ? "0" : "1");
			expr_12[6] = this.bgm.ToString();
			expr_12[7] = this.sfx.ToString();
			expr_12[8] = this.voice.ToString();
			expr_12[9] = ((!this.recommendOn) ? "0" : "1");
			expr_12[10] = ((!this.detailedShop) ? "0" : "1");
			int arg_11F_1 = 11;
			int num3 = (int)this.skillPanelPivot;
			expr_12[arg_11F_1] = num3.ToString();
			expr_12[12] = ((!this.highFPS) ? "0" : "1");
			value = string.Format(arg_13E_0, expr_12);
			PlayerPrefs.SetString("MySetting", value);
			PlayerPrefs.Save();
		}

		public void Load()
		{
			this.SetDefault();
			string @string = PlayerPrefs.GetString("MySetting");
			if (string.IsNullOrEmpty(@string))
			{
				this.Record();
				@string = PlayerPrefs.GetString("MySetting");
			}
			string[] array = @string.Split(new char[]
			{
				'|'
			});
			if (array.Length != 13)
			{
				return;
			}
			int num = 3;
			int.TryParse(array[0], out num);
			this.qualityLevel = (MobaQualityLevel)num;
			this.characterOutlineList = byte.Parse(array[1]);
			this.showFPS = (array[2] == "1");
			this.cameraHigh = (array[3] == "1");
			this.crazyCastingSkill = (array[4] == "1");
			this.selectDyingHero = (array[5] == "1");
			this.bgm = byte.Parse(array[6]);
			this.sfx = byte.Parse(array[7]);
			this.voice = byte.Parse(array[8]);
			this.recommendOn = (array[9] == "1");
			this.detailedShop = (array[10] == "1");
			this.skillPanelPivot = byte.Parse(array[11]);
			this.highFPS = (array[12] == "1");
		}
	}
}

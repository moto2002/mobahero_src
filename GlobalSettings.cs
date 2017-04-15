using Com.Game.Utils;
using System;
using UnityEngine;

public class GlobalSettings : MonoBehaviour
{
	public Transform tempNode;

	public bool UIOpt = true;

	public VersionConfig versionConfig;

	public NetConfig netConfig;

	public DataConfig dataConfig;

	public LanguageConfig languageConfig;

	public TestSwitch testSwitch;

	public PvpSetting PvpSetting;

	public AutoTestSetting autoTestSetting;

	public LoggerTags LogTags;

	public MobaQualitySetting QualitySetting;

	public bool IsPvp;

	public TestCGConfig testCGConfigInst;

	public bool isMenuViewPlayAnima;

	public bool isArenaModeViewPlayAnim;

	public bool isOpenNewbieGuide = true;

	public static int FogMode = 3;

	public static bool meshanim;

	public static bool dynamicdraw = true;

	public static bool fakeshadow = true;

	public bool isShowFPS = true;

	public bool isHighFPS;

	public static int ResWidth;

	public static int ResHeight;

	public bool isLockView = true;

	public static GlobalSettings Instance
	{
		get;
		private set;
	}

	public static bool IsMasterGizmosShown
	{
		get
		{
			return !(GlobalSettings.Instance == null) && GlobalSettings.Instance.PvpSetting.IsMasterGizmosShown;
		}
	}

	public static bool TestCreep
	{
		get
		{
			return !(GlobalSettings.Instance == null) && GlobalSettings.Instance.PvpSetting.TestCreep;
		}
	}

	public static bool NoMonster
	{
		get
		{
			return !(GlobalSettings.Instance == null) && GlobalSettings.Instance.PvpSetting.NoMonster;
		}
	}

	public static bool NoDamage
	{
		get
		{
			return !(GlobalSettings.Instance == null) && GlobalSettings.Instance.PvpSetting.isNoDamage;
		}
	}

	public static bool NoCost
	{
		get;
		set;
	}

	public static bool NoReConnnect
	{
		get
		{
			return !(GlobalSettings.Instance == null) && GlobalSettings.Instance.PvpSetting.isNoReConnect;
		}
	}

	public static bool isSkillUnlock
	{
		get
		{
			return !(GlobalSettings.Instance == null) && GlobalSettings.Instance.PvpSetting.isSkillUnlock;
		}
	}

	public static bool DebugData
	{
		get
		{
			return !(GlobalSettings.Instance == null) && GlobalSettings.Instance.PvpSetting.isDebugData;
		}
	}

	public static bool IsNoSkillCD
	{
		get;
		set;
	}

	public static bool IsTowerDurationWound
	{
		get
		{
			return !(GlobalSettings.Instance == null) && GlobalSettings.Instance.PvpSetting.isTowerDurationWound;
		}
	}

	public static bool useLocalData
	{
		get
		{
			return GlobalSettings.Instance == null || GlobalSettings.Instance.dataConfig.isLocalBinData || GlobalSettings.Instance.dataConfig.isLocalXml;
		}
	}

	public static bool useLocalXML
	{
		get
		{
			return GlobalSettings.Instance == null || GlobalSettings.Instance.dataConfig.isLocalXml;
		}
	}

	public static bool UseLZMADecoder
	{
		get
		{
			return GlobalSettings.Instance == null || GlobalSettings.Instance.dataConfig.isUseLZMADecoder;
		}
	}

	public static bool useBundle
	{
		get
		{
			return GlobalSettings.Instance != null && GlobalSettings.Instance.dataConfig.useBundle;
		}
	}

	public static bool TestBattleRefresh
	{
		get
		{
			return GlobalSettings.Instance != null && GlobalSettings.Instance.testSwitch.testBattleRefresh;
		}
	}

	public static bool TestDisableAI
	{
		get
		{
			return GlobalSettings.Instance != null && GlobalSettings.Instance.testSwitch.testDisableAI;
		}
	}

	public static bool isTestModeOpen
	{
		get
		{
			return GlobalSettings.Instance != null && GlobalSettings.Instance.testSwitch.testMode;
		}
	}

	public string MasterIpForce
	{
		get
		{
			return (!(null == GlobalSettings.Instance)) ? GlobalSettings.Instance.netConfig.masterIpForce : string.Empty;
		}
	}

	public static int MasterIpVersion
	{
		get
		{
			return (!(null == GlobalSettings.Instance)) ? GlobalSettings.Instance.netConfig.ipVersion : 0;
		}
	}

	public static string AppVersion
	{
		get
		{
			return (!(null == GlobalSettings.Instance)) ? GlobalSettings.Instance.versionConfig.appVersion : string.Empty;
		}
	}

	public static LanguageType languageType
	{
		get
		{
			return (!(null == GlobalSettings.Instance)) ? GlobalSettings.Instance.languageConfig.languageType : LanguageType.Chinese;
		}
	}

	public static string DataVersion
	{
		get
		{
			return (!(null == GlobalSettings.Instance)) ? GlobalSettings.Instance.versionConfig.dataVersion : string.Empty;
		}
		set
		{
			if (null != GlobalSettings.Instance)
			{
				GlobalSettings.Instance.versionConfig.dataVersion = value;
			}
		}
	}

	public static bool isLoginByHoolaiSDK
	{
		get
		{
			return !(null == GlobalSettings.Instance) && (GlobalSettings.Instance.versionConfig.isLoginByHoolaiSDK && Application.platform == RuntimePlatform.Android) && !Application.isEditor;
		}
	}

	public static bool isLoginByAnySDK
	{
		get
		{
			return !(null == GlobalSettings.Instance) && (GlobalSettings.Instance.versionConfig.isLoginByAnySDK && Application.platform == RuntimePlatform.IPhonePlayer) && !Application.isEditor;
		}
	}

	public static bool isLoginByLDSDK
	{
		get
		{
			return !(null == GlobalSettings.Instance) && (GlobalSettings.Instance.versionConfig.isLoginByLDSDK && Application.platform == RuntimePlatform.IPhonePlayer) && !Application.isEditor;
		}
	}

	public static bool needThirdLogin
	{
		get
		{
			return !(null == GlobalSettings.Instance) && GlobalSettings.Instance.versionConfig.needThirdLogin;
		}
	}

	public static bool isIOSInAppPurchase
	{
		get
		{
			return !(null == GlobalSettings.Instance) && GlobalSettings.Instance.versionConfig.isIOSInAppPurchase;
		}
	}

	public static bool CheckVersion
	{
		get
		{
			return !(null == GlobalSettings.Instance) && GlobalSettings.Instance.versionConfig.checkVersion;
		}
	}

	public SortType AttackSortType
	{
		get;
		set;
	}

	public bool isCrazyMode
	{
		get;
		set;
	}

	public bool isEnableChaOutline
	{
		get;
		set;
	}

	public int ChaOutlineLevel
	{
		get;
		set;
	}

	public bool ClientGoAhead
	{
		get;
		set;
	}

	public static void setDynamicdraw(bool draw)
	{
	}

	public static void setfakeshadow(bool fake)
	{
		GlobalSettings.fakeshadow = fake;
		RecieverObjCtrl.usefakeshadow = fake;
	}

	public static void SetFogMode(int state)
	{
		GlobalSettings.FogMode = state;
		FOWSystem.effectVisible = true;
		if (state <= 2)
		{
			FOWSystem.effectVisible = false;
		}
	}

	private void Awake()
	{
		GlobalSettings.Instance = this;
		GlobalSettings.SetFogMode(0);
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		if (this.LogTags)
		{
			this.LogTags.Apply();
		}
		this.AttackSortType = (SortType)PlayerPrefs.GetInt("AttackPriority", 3);
		this.isCrazyMode = (PlayerPrefs.GetInt("IsCrazyMode", 1) == 1);
		this.isEnableChaOutline = (PlayerPrefs.GetInt("EnableChaOutline", 1) == 1);
		this.ChaOutlineLevel = PlayerPrefs.GetInt("ChaOutlineLevel", 2);
		this.ClientGoAhead = false;
		GlobalSettings.ResetQuality();
	}

	public static void ResetQuality()
	{
		if (GlobalSettings.ResWidth == 0)
		{
			GlobalSettings.ResWidth = Screen.currentResolution.width;
		}
		if (GlobalSettings.ResHeight == 0)
		{
			GlobalSettings.ResHeight = Screen.currentResolution.height;
		}
		int num = 3;
		if (PlayerPrefs.HasKey("MobaQualityLevel"))
		{
			num = PlayerPrefs.GetInt("MobaQualityLevel");
		}
		switch (num)
		{
		case 0:
		case 1:
			GlobalSettings.Instance.QualitySetting.SetLevel(MobaQualityLevel.Hd);
			break;
		case 2:
		case 3:
			GlobalSettings.Instance.QualitySetting.SetLevel(MobaQualityLevel.P1080);
			break;
		case 4:
			GlobalSettings.Instance.QualitySetting.SetLevel(MobaQualityLevel.Original);
			break;
		}
		Light light = UnityEngine.Object.FindObjectOfType<Light>();
		if (light != null)
		{
			light.shadows = ((num <= 0) ? LightShadows.None : LightShadows.Hard);
		}
	}
}

using Com.Game.Utils;
using System;
using System.IO;
using UnityEngine;

[AddComponentMenu("Wwise/AkInitializer"), RequireComponent(typeof(AkTerminator))]
public class AkInitializer : MonoBehaviour
{
	private const string c_Language = "Chinese(PRC)";

	public const int c_DefaultPoolSize = 4096;

	public const int c_LowerPoolSize = 2048;

	public const int c_StreamingPoolSize = 1024;

	public const float c_MemoryCutoffThreshold = 0.95f;

	public const bool c_EngineLogging = true;

	public static readonly string c_DefaultBasePath = Path.Combine("Audio", "GeneratedSoundBanks");

	public string basePath = AkInitializer.c_DefaultBasePath;

	public string language = "Chinese(PRC)";

	public int defaultPoolSize = 4096;

	public int lowerPoolSize = 2048;

	public int streamingPoolSize = 1024;

	public float memoryCutoffThreshold = 0.95f;

	public bool engineLogging = true;

	private static AkInitializer ms_Instance;

	public static string GetBasePath()
	{
		return AkInitializer.ms_Instance.basePath;
	}

	public static string GetCurrentLanguage()
	{
		return AkInitializer.ms_Instance.language;
	}

	private void Awake()
	{
		this.Initialize();
	}

	public static AkInitializer ins()
	{
		return AkInitializer.ms_Instance;
	}

	public void Initialize()
	{
		if (AkInitializer.ms_Instance != null)
		{
			if (AkInitializer.ms_Instance != this)
			{
				UnityEngine.Object.DestroyImmediate(base.gameObject);
			}
			return;
		}
		AkMemSettings akMemSettings = new AkMemSettings();
		akMemSettings.uMaxNumPools = 40u;
		AkDeviceSettings akDeviceSettings = new AkDeviceSettings();
		AkSoundEngine.GetDefaultDeviceSettings(akDeviceSettings);
		AkStreamMgrSettings akStreamMgrSettings = new AkStreamMgrSettings();
		akStreamMgrSettings.uMemorySize = (uint)(this.streamingPoolSize * 1024);
		AkInitSettings akInitSettings = new AkInitSettings();
		AkSoundEngine.GetDefaultInitSettings(akInitSettings);
		akInitSettings.uDefaultPoolSize = (uint)(this.defaultPoolSize * 1024);
		AkPlatformInitSettings akPlatformInitSettings = new AkPlatformInitSettings();
		AkSoundEngine.GetDefaultPlatformInitSettings(akPlatformInitSettings);
		akPlatformInitSettings.uLEngineDefaultPoolSize = (uint)(this.lowerPoolSize * 1024);
		akPlatformInitSettings.fLEngineDefaultPoolRatioThreshold = this.memoryCutoffThreshold;
		AkMusicSettings akMusicSettings = new AkMusicSettings();
		AkSoundEngine.GetDefaultMusicSettings(akMusicSettings);
		AKRESULT aKRESULT = AkSoundEngine.Init(akMemSettings, akStreamMgrSettings, akDeviceSettings, akInitSettings, akPlatformInitSettings, akMusicSettings);
		if (aKRESULT != AKRESULT.AK_Success)
		{
			Debug.LogError("WwiseUnity: Failed to initialize the sound engine. Abort.");
			return;
		}
		AkInitializer.ms_Instance = this;
		string validBasePath = AkBasePathGetter.GetValidBasePath();
		if (string.IsNullOrEmpty(validBasePath))
		{
			return;
		}
		aKRESULT = AkSoundEngine.SetBasePath(validBasePath);
		if (aKRESULT != AKRESULT.AK_Success)
		{
			return;
		}
		AkSoundEngine.SetCurrentLanguage("Chinese(PRC)");
		aKRESULT = AkCallbackManager.Init();
		if (aKRESULT != AKRESULT.AK_Success)
		{
			ClientLogger.Error("WwiseUnity: Failed to initialize Callback Manager. Terminate sound engine.");
			AkSoundEngine.Term();
			AkInitializer.ms_Instance = null;
			return;
		}
		AkBankManager.Reset();
		UnityEngine.Object.DontDestroyOnLoad(this);
	}

	public void unloadLanguageSoundBank(string filename)
	{
		string text = string.Concat(new string[]
		{
			Application.streamingAssetsPath,
			"/Audio/GeneratedSoundBanks/",
			AkBasePathGetter.GetPlatformName(),
			"/",
			this.language
		});
		AkBankManager.UnloadBank(filename + ".bnk", true, 0);
	}

	public void loadLanguageSoundBank(string filename)
	{
		string text = string.Concat(new string[]
		{
			Application.streamingAssetsPath,
			"/Audio/GeneratedSoundBanks/",
			AkBasePathGetter.GetPlatformName(),
			"/",
			this.language
		});
		AkBankManager.LoadBank(filename + ".bnk", 0);
	}

	public void loadAllGameSoundBank(string lang = "")
	{
		string text = string.Concat(new string[]
		{
			Application.streamingAssetsPath,
			"/Audio/GeneratedSoundBanks/",
			AkBasePathGetter.GetPlatformName(),
			"/",
			lang
		});
		if (!Directory.Exists(text))
		{
			Debug.LogError(text + " does not exist!");
			return;
		}
		string[] files = Directory.GetFiles(text);
		string[] array = files;
		for (int i = 0; i < array.Length; i++)
		{
			string text2 = array[i];
			if (text2.EndsWith(".bnk"))
			{
				int num = text2.LastIndexOf("/");
				string text3 = text2.Substring(num + 1, text2.Length - num - 1);
				string name = text3;
				AkBankManager.LoadBank(name, 0);
			}
		}
	}

	private void OnDestroy()
	{
		if (AkInitializer.ms_Instance == this)
		{
			AkCallbackManager.SetMonitoringCallback((ErrorLevel)0, null);
			AkInitializer.ms_Instance = null;
		}
	}

	private void OnEnable()
	{
		if (AkInitializer.ms_Instance == null && AkSoundEngine.IsInitialized())
		{
			AkInitializer.ms_Instance = this;
		}
	}

	private void LateUpdate()
	{
		if (AkInitializer.ms_Instance != null)
		{
			AkCallbackManager.PostCallbacks();
			AkBankManager.DoUnloadBanks();
			AkSoundEngine.RenderAudio();
		}
	}

	private void OnApplicationPause(bool pauseStatus)
	{
		if (AkInitializer.ms_Instance != null)
		{
			if (!pauseStatus)
			{
				AkSoundEngine.WakeupFromSuspend();
			}
			else
			{
				AkSoundEngine.Suspend();
			}
			AkSoundEngine.RenderAudio();
		}
	}

	private void OnApplicationFocus(bool focus)
	{
		if (AkInitializer.ms_Instance != null)
		{
			if (focus)
			{
				AkSoundEngine.WakeupFromSuspend();
			}
			else
			{
				AkSoundEngine.Suspend();
			}
			AkSoundEngine.RenderAudio();
		}
	}
}

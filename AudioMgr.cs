using Com.Game.Data;
using Com.Game.Manager;
using System;
using System.Collections.Generic;
using UnityEngine;

public class AudioMgr : MonoBehaviour
{
	public class eventObj
	{
		public string eventstr = string.Empty;

		public GameObject eventobj;

		public eventObj(string str, GameObject obj)
		{
			this.eventstr = str;
			this.eventobj = obj;
		}
	}

	private bool usingwwise = true;

	private static Dictionary<string, AudioClip> m_audioClipDict = new Dictionary<string, AudioClip>();

	private float m_fCurVolume;

	public static float m_bgVol = 1f;

	public static float m_effVol = 1f;

	public static float m_voiceVol = 1f;

	private static bool m_bgMute = false;

	private static bool m_effMute = false;

	private static bool m_voiceMute = false;

	private static bool awaked = false;

	[Range(0f, 1f), SerializeField]
	private float m_volume = 0.5f;

	[SerializeField]
	private BgmPlayer m_bgm;

	[SerializeField]
	private AudioClip m_defaultButtonClip;

	private WaitForEndOfFrame m_waitForEndOfFrame = new WaitForEndOfFrame();

	private static AudioMgr m_instance;

	public GameObject exSoundObj;

	public List<AudioMgr.eventObj> eventObjList = new List<AudioMgr.eventObj>();

	public static uint[] playingids = new uint[3];

	private GameObject ggg;

	private GameObject ggg1;

	public static int outerlistenidx = 0;

	private static bool enblis = false;

	public bool muteSound
	{
		get
		{
			return AudioMgr.m_effMute;
		}
	}

	public static AudioMgr Instance
	{
		get
		{
			if (AudioMgr.m_instance == null)
			{
				Debug.LogWarning("AudioMgr script is missing");
			}
			return AudioMgr.m_instance;
		}
	}

	public static BgmPlayer BGM
	{
		get
		{
			return AudioMgr.m_instance.m_bgm;
		}
	}

	public bool isUsingWWise()
	{
		return this.usingwwise;
	}

	private void Awake()
	{
		AudioMgr.m_instance = this;
		this.exSoundObj = new GameObject("ExSoundObj");
		this.exSoundObj.transform.parent = base.transform;
		if (AudioMgr.awaked)
		{
			return;
		}
		if (PlayerPrefs.GetInt("BGM") == 0)
		{
			AudioMgr.m_bgMute = false;
		}
		else
		{
			AudioMgr.m_bgMute = true;
		}
		if (PlayerPrefs.GetInt("SoundEffect") == 0)
		{
			AudioMgr.m_effMute = false;
		}
		else
		{
			AudioMgr.m_effMute = true;
		}
		if (PlayerPrefs.GetInt("VoiceEff") == 0)
		{
			AudioMgr.m_voiceMute = false;
		}
		else
		{
			AudioMgr.m_voiceMute = true;
		}
		AudioMgr.m_bgVol = PlayerPrefs.GetFloat("BGM_V");
		AudioMgr.m_effVol = PlayerPrefs.GetFloat("EFF_V");
		AudioMgr.m_voiceVol = 1f;
		int @int = PlayerPrefs.GetInt("FIRSTGAME");
		if (this.usingwwise)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(Resources.Load("WwiseGlobal") as GameObject) as GameObject;
			gameObject.name = "WwiseGlobal";
		}
		AudioMgr.loadSoundBank_UI();
		if (@int == 0)
		{
			PlayerPrefs.SetInt("FIRSTGAME", 1);
			PlayerPrefs.Save();
			AudioMgr.setVolumeBG(0.75f, true);
			AudioMgr.setVolumeEff(0.75f);
			AudioMgr.setVolumeVoice(1f);
		}
		AudioMgr.setVolumeBG(AudioMgr.m_bgVol, true);
		AudioMgr.setVolumeEff(AudioMgr.m_effVol);
		AudioMgr.setVolumeVoice(AudioMgr.m_voiceVol);
		if (AudioMgr.m_bgMute)
		{
			this.MuteBg();
		}
		if (AudioMgr.m_effMute)
		{
			this.MuteEff();
		}
		if (AudioMgr.m_voiceMute)
		{
			this.MuteVoice();
		}
		AudioMgr.awaked = true;
	}

	private void Start()
	{
		MobaMessageManager.RegistMessage((ClientMsg)25010, new MobaMessageFunc(this.ReleaseAudioClips));
		AudioMgr.setVolumeBG(AudioMgr.m_bgVol, true);
		AudioMgr.setVolumeEff(AudioMgr.m_effVol);
		AudioMgr.setVolumeVoice(AudioMgr.m_voiceVol);
	}

	public static string getPlayingEvent()
	{
		if (AudioMgr.playingids == null || AudioMgr.playingids[0] != 0u)
		{
			uint eventIDFromPlayingID = AkSoundEngine.GetEventIDFromPlayingID(AudioMgr.playingids[0]);
		}
		return string.Empty;
	}

	public static bool isPlaying(GameObject emmit)
	{
		uint num = 3u;
		AkSoundEngine.GetPlayingIDsFromGameObject(emmit, ref num, AudioMgr.playingids);
		return AudioMgr.playingids != null && num > 0u;
	}

	public static void setVolumeBG(float v, bool value = true)
	{
		if (!value)
		{
			AudioMgr.setRTPC("Music_Bus_Vol", v * 100f);
			return;
		}
		AudioMgr.m_bgVol = v;
		AudioMgr.setRTPC("Music_Bus_Vol", v * 100f);
		PlayerPrefs.SetFloat("BGM_V", v);
		PlayerPrefs.Save();
	}

	public static float getVolumeBG()
	{
		if (AudioMgr.m_bgMute)
		{
			return 0f;
		}
		return AudioMgr.m_bgVol;
	}

	public static void setVolumeEff(float v)
	{
		AudioMgr.m_effVol = v;
		AudioMgr.setRTPC("SFX_Bus_Vol", v * 100f);
		PlayerPrefs.SetFloat("EFF_V", v);
		PlayerPrefs.Save();
	}

	public static float getVolumeEff()
	{
		if (AudioMgr.m_effMute)
		{
			return 0f;
		}
		return AudioMgr.m_effVol;
	}

	public static float getVolumeVoice()
	{
		if (AudioMgr.m_voiceMute)
		{
			return 0f;
		}
		return AudioMgr.m_voiceVol;
	}

	public static void setVolumeVoice(float v)
	{
		AudioMgr.m_voiceVol = v;
		AudioMgr.setRTPC("Voice_Bus_Vol", v * 100f);
		PlayerPrefs.SetFloat("VOICE_V", v);
		PlayerPrefs.Save();
	}

	public static void setVolume(GameObject emmit, float v)
	{
	}

	public static float getVolume(GameObject emmit)
	{
		return 0f;
	}

	public static void Pause(string name)
	{
		if (string.IsNullOrEmpty(name))
		{
			return;
		}
		AkSoundEngine.ExecuteActionOnEvent(name, AkActionOnEventType.AkActionOnEventType_Pause);
	}

	public static void Continue(string name)
	{
		if (string.IsNullOrEmpty(name))
		{
			return;
		}
		AkSoundEngine.ExecuteActionOnEvent(name, AkActionOnEventType.AkActionOnEventType_Resume);
	}

	private void Update()
	{
		if (Input.GetKeyUp(KeyCode.B))
		{
			if (this.ggg == null)
			{
				this.ggg = new GameObject("testaudio.......");
				this.ggg1 = new GameObject("111111111111testaudio.......");
			}
			AkBankManager.LoadBank("SLiaonida", 0);
			AudioMgr.Play("Play_Liaonida_conj_1_Buff", null, false, false);
		}
		else if (Input.GetKeyUp(KeyCode.H))
		{
			if (this.ggg != null)
			{
				AudioMgr.Stop(this.ggg);
				AudioMgr.Stop(this.ggg1);
			}
		}
		else if (Input.GetKeyUp(KeyCode.S))
		{
			AudioMgr.setState(BgmPlayer.Instance.gameObject, "BGM", "Level03");
			AudioMgr.trigger(BgmPlayer.Instance.gameObject, "Stingers");
		}
		else if (Input.GetKeyUp(KeyCode.A))
		{
			AudioMgr.Play("Play_Coin_Throw", BgmPlayer.Instance.gameObject, false, false);
		}
		if (this.eventObjList.Count > 0)
		{
			string eventstr = this.eventObjList[0].eventstr;
			GameObject eventobj = this.eventObjList[0].eventobj;
			this.eventObjList.RemoveAt(0);
		}
	}

	public static void Stop(GameObject emitter = null)
	{
		AkSoundEngine.StopAll(emitter);
	}

	public static void SetLisenerPos_NoDir(Transform tr, Vector3 offset, int idx)
	{
		AkSoundEngine.SetListenerPosition(0f, 0f, 0f, 0f, 0f, 0f, tr.position.x + offset.x, tr.position.y + offset.y, tr.position.z + offset.z, (uint)idx);
	}

	public static void closeOuterListen()
	{
		AudioMgr.outerlistenidx = -1;
	}

	public static void SetLisenerPos(Transform tr, Vector3 offset, int idx)
	{
		AudioMgr.outerlistenidx = idx;
		AkSoundEngine.SetListenerPosition(tr.forward.x, tr.forward.y, tr.forward.z, tr.up.x, tr.up.y, tr.up.z, tr.position.x + offset.x, tr.position.y + offset.y, tr.position.z + offset.z, (uint)idx);
	}

	public static void enableListner(GameObject gm, bool enb)
	{
		if (AudioMgr.enblis == enb)
		{
			return;
		}
		AudioMgr.enblis = enb;
		AkAudioListener component = gm.GetComponent<AkAudioListener>();
		if (component != null)
		{
			component.enabled = enb;
		}
	}

	public static void AddLisener(GameObject gm)
	{
		AudioMgr.enblis = true;
		AkAudioListener component = gm.GetComponent<AkAudioListener>();
		if (component == null)
		{
			gm.AddComponent<AkAudioListener>();
		}
	}

	public static void setRTPC(string name, float value)
	{
		AkSoundEngine.SetRTPCValue(name, value);
	}

	public static void trigger(GameObject gm, string name)
	{
		AkSoundEngine.PostTrigger(name, gm);
	}

	public static void setState(GameObject gm, string grp, string state)
	{
		AkSoundEngine.SetState(grp, state);
	}

	public static void RemoveLisener(GameObject gm)
	{
		AkAudioListener component = gm.GetComponent<AkAudioListener>();
		if (component != null)
		{
			UnityEngine.Object.DestroyImmediate(component);
		}
	}

	public static void loadSoundBank_UI()
	{
		AkBankManager.LoadBank("Init.bnk", 0);
		AkBankManager.LoadBank("UI.bnk", 0);
		AkBankManager.LoadBank("Interface_Music.bnk", 0);
		AkBankManager.LoadBank("VoiceOver.bnk", 0);
		AkBankManager.LoadBank("Jingle.bnk", 0);
	}

	public static void unloadSoundBank_UI()
	{
		AkBankManager.UnloadBank("Interface_Music.bnk", true, 0);
	}

	public static void loadSoundBank_INGAME()
	{
		AkBankManager.LoadBank("VoiceOver.bnk", 0);
		AkBankManager.LoadBank("AMB.bnk", 0);
		AkBankManager.LoadBank("ThreeVThree.bnk", 0);
		AkBankManager.LoadBank("Summoner.bnk", 0);
		AkBankManager.LoadBank("Item.bnk", 0);
		AkBankManager.LoadBank("Businessman.bnk", 0);
	}

	public static void loadSoundBank3V3V3()
	{
		AkBankManager.LoadBank("ThreeVThreeVThree.bnk", 0);
	}

	public static void unloadSoundBank_3V3V3()
	{
		AkBankManager.UnloadBank("ThreeVThreeVThree.bnk", false, 0);
	}

	public static void unloadSoundBank_INGAME()
	{
		AkBankManager.UnloadBank("VoiceOver.bnk", true, 0);
		AkBankManager.UnloadBank("AMB.bnk", true, 0);
		AkBankManager.UnloadBank("ThreeVThree.bnk", true, 0);
		AudioMgr.unloadSoundBank_AllHero();
	}

	public static void loadSoundBank_Skill(string heroname, bool nofix = false, int skin = 0)
	{
		if (nofix)
		{
			if (!AkBankManager.LoadBank(heroname + ".bnk", skin))
			{
				AkBankManager.LoadBank(heroname + ".bnk", 0);
			}
		}
		else if (!AkBankManager.LoadBank("S" + heroname + ".bnk", skin))
		{
			AkBankManager.LoadBank("S" + heroname + ".bnk", 0);
		}
	}

	public static void unloadSoundBank_AllHero()
	{
		Dictionary<string, object> data = BaseDataMgr.instance.getData(typeof(SysHeroMainVo));
		if (data == null)
		{
			return;
		}
		foreach (string current in data.Keys)
		{
			for (int i = 0; i <= 10; i++)
			{
				AkBankManager.UnloadBank(current + ".bnk", true, i);
				AkBankManager.UnloadBank("S" + current + ".bnk", true, i);
			}
		}
		AkBankManager.UnloadBank("Summoner.bnk", true, 0);
	}

	public static void unloadLanguageSoundBank(string str, int skin = 0)
	{
		if (AkInitializer.ins() == null)
		{
			return;
		}
		AkInitializer.ins().unloadLanguageSoundBank(str);
	}

	public static void loadLanguageSoundBank(string name, int skin = 0)
	{
		AkInitializer.ins().loadLanguageSoundBank(name);
	}

	public static void LoadBnk(string nam)
	{
		AkBankManager.LoadBank(nam, 0);
	}

	public static void UnLoadBnk(string nam, bool realy = false)
	{
		AkBankManager.UnloadBank(nam, realy, 0);
	}

	public static void RealyUnLoadBnk()
	{
		AkBankManager.DoUnloadBanks();
	}

	public static void PlayPromptVoice(string voiceId)
	{
		if (AudioMgr.Instance.isVoiceMute())
		{
			return;
		}
		if (string.IsNullOrEmpty(voiceId) || voiceId.Equals("[]"))
		{
			return;
		}
		AudioMgr.Play(voiceId, null, false, false);
	}

	public static void Play(int id, bool enmey = false)
	{
		if (AudioMgr.Instance.isVoiceMute())
		{
			return;
		}
		SysPvpPromptVo dataById = BaseDataMgr.instance.GetDataById<SysPvpPromptVo>(id.ToString());
		if (dataById == null)
		{
			return;
		}
		if (!enmey)
		{
			AudioMgr.Play(dataById.sound, null, false, false);
		}
	}

	public static void PlayVoice(string eventstr, GameObject emitter = null, bool stop = false, bool notbreakprev = false)
	{
		if (AudioMgr.Instance.isVoiceMute())
		{
			return;
		}
		AudioMgr.Play(eventstr, emitter, stop, notbreakprev);
	}

	public static void PlayEff(string eventstr, GameObject emitter = null, bool stop = false, bool notbreakprev = false)
	{
		if (AudioMgr.Instance.isEffMute())
		{
			return;
		}
		AudioMgr.Play(eventstr, emitter, stop, notbreakprev);
	}

	public static void PlayUI(string eventstr, GameObject emitter = null, bool stop = false, bool notbreakprev = false)
	{
		if (AudioMgr.Instance.isEffMute())
		{
			return;
		}
		AudioMgr.Play(eventstr, emitter, stop, notbreakprev);
	}

	public static void Play(string eventstr, GameObject emitter = null, bool stop = false, bool notbreakprev = false)
	{
		if (string.IsNullOrEmpty(eventstr))
		{
			return;
		}
		if (emitter == null && AudioMgr.Instance != null)
		{
			emitter = AudioMgr.Instance.gameObject;
		}
		if (stop && emitter != null)
		{
			AkSoundEngine.StopAll(emitter);
		}
		if (notbreakprev && emitter != null && AudioMgr.isPlaying(emitter))
		{
			return;
		}
		AkSoundEngine.PostEvent(eventstr, emitter);
	}

	public static void PlayBG(string eventstr, GameObject emitter = null, bool stop = false, bool notbreakprev = false)
	{
		if (AudioMgr.m_instance.isBgMute())
		{
			return;
		}
		if (string.IsNullOrEmpty(eventstr))
		{
			return;
		}
		if (emitter == null)
		{
			emitter = AudioMgr.Instance.gameObject;
		}
		if (stop && emitter != null)
		{
			AkSoundEngine.StopAll(emitter);
		}
		if (notbreakprev && emitter != null && AudioMgr.isPlaying(emitter))
		{
			return;
		}
		AkSoundEngine.PostEvent(eventstr, emitter);
	}

	public static AudioSourceControl Play(AudioClipInfo clipInfo, Transform targetTrans = null)
	{
		return null;
	}

	public static AudioSourceControl PlayBGOld(AudioClipInfo clipInfo, Transform targetTrans = null)
	{
		return null;
	}

	public static AudioSourceControl Play_UI(AudioClip clip, float vol, float pitch, Transform targetTrans = null)
	{
		return null;
	}

	public void LoadAlleffectAudioItem(AudioClip clip, bool needPreload)
	{
		if (needPreload && !AudioMgr.m_audioClipDict.ContainsKey(clip.name))
		{
			AudioMgr.m_audioClipDict.Add(clip.name, clip);
		}
	}

	public void ReleaseAudioClips(MobaMessage msg)
	{
		foreach (KeyValuePair<string, AudioClip> current in AudioMgr.m_audioClipDict)
		{
			AudioClip value = current.Value;
		}
		AudioMgr.m_audioClipDict.Clear();
		PoolMgr.Instance.GetAudioPoolByType(eAudioSourceType.World).ReleaseAll();
		PoolMgr.Instance.GetAudioPoolByType(eAudioSourceType.World_Loop).ReleaseAll();
		PoolMgr.Instance.GetAudioPoolByType(eAudioSourceType.Voice).ReleaseAll();
		PoolMgr.Instance.ReleaseAudioPools();
		Resources.UnloadUnusedAssets();
	}

	public bool isEffMute()
	{
		return AudioMgr.m_effMute;
	}

	public bool isBgMute()
	{
		return AudioMgr.m_bgMute;
	}

	public bool isVoiceMute()
	{
		return AudioMgr.m_voiceMute;
	}

	public bool isMuteAll()
	{
		return AudioMgr.m_effMute && AudioMgr.m_bgMute && AudioMgr.m_voiceMute;
	}

	public void MuteEff()
	{
		PlayerPrefs.SetInt("SoundEffect", 1);
		PlayerPrefs.Save();
		AudioMgr.m_effMute = true;
	}

	public void UnMuteEff()
	{
		PlayerPrefs.SetInt("SoundEffect", 0);
		PlayerPrefs.Save();
		AudioMgr.m_effMute = false;
	}

	public void MuteVoice()
	{
		PlayerPrefs.SetInt("VoiceEff", 1);
		PlayerPrefs.Save();
		AudioMgr.m_voiceMute = true;
	}

	public void UnMuteVoice()
	{
		PlayerPrefs.SetInt("VoiceEff", 0);
		PlayerPrefs.Save();
		AudioMgr.m_voiceMute = false;
	}

	public void MuteBg()
	{
		BgmPlayer.Instance.StopBG();
		PlayerPrefs.SetInt("BGM", 1);
		PlayerPrefs.Save();
		AudioMgr.m_bgMute = true;
	}

	public void UnMuteBg()
	{
		PlayerPrefs.SetInt("BGM", 0);
		PlayerPrefs.Save();
		AudioMgr.m_bgMute = false;
		BgmPlayer.Instance.PlayBG();
	}

	public void MuteAll()
	{
		this.MuteEff();
		this.MuteBg();
		new ToggleSoundMsg(false);
	}

	public void UnMuteAll()
	{
		this.UnMuteEff();
		this.UnMuteBg();
		new ToggleSoundMsg(true);
	}

	public void PauseAll()
	{
		for (int i = 0; i < 5; i++)
		{
			if (i != 2)
			{
				List<AudioSourceControl> usingAudioSourceControlList = PoolMgr.Instance.GetAudioPoolByType((eAudioSourceType)i).GetUsingAudioSourceControlList();
				for (int j = 0; j < usingAudioSourceControlList.Count; j++)
				{
					usingAudioSourceControlList[j].Pause(true);
				}
			}
		}
	}

	public void ResumeAll()
	{
		for (int i = 0; i < 5; i++)
		{
			if (i != 2)
			{
				List<AudioSourceControl> usingAudioSourceControlList = PoolMgr.Instance.GetAudioPoolByType((eAudioSourceType)i).GetUsingAudioSourceControlList();
				for (int j = 0; j < usingAudioSourceControlList.Count; j++)
				{
					usingAudioSourceControlList[j].Pause(false);
				}
			}
		}
	}

	public void SetVolume(float volume)
	{
		volume = Mathf.Clamp01(volume);
		AudioListener.volume = volume;
		this.m_volume = volume;
	}
}

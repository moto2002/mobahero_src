using Com.Game.Module;
using MobaHeros.Pvp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class HeroVoicePlayer : VoicePlayer
{
	public delegate void playfunc();

	public const string MSG_PLAYSOUND = "playSound";

	private const float MOVE_SOUND_INTERVAL = 15f;

	private string _name;

	private static bool _debugSound;

	[SerializeField]
	private Hero _hero;

	[SerializeField]
	private float _volume = 2f;

	[SerializeField]
	private AudioClip[] _bornClips;

	[SerializeField]
	private AudioClip[] _movingClips;

	[SerializeField]
	private AudioClip[] _killClips;

	[SerializeField]
	private AudioClip[] _dieClips;

	[SerializeField]
	private AudioClip[] _rebornClips;

	private Dictionary<string, AudioClip> _atkClips = new Dictionary<string, AudioClip>();

	private string born_event;

	private string moving_event;

	private string kill_event;

	private string die_event;

	private string reborn_event;

	private string attackstart_event;

	private string attackoral_event;

	private string heroselected_event;

	private string bubing_event;

	private string levelup_event;

	private string kill3_event;

	public static bool editorVoiceEnable = true;

	private Dictionary<string, HeroVoicePlayer.playfunc> playfuncs = new Dictionary<string, HeroVoicePlayer.playfunc>();

	private float _lastMoveSoundTiming;

	public static bool debugSound
	{
		get
		{
			return HeroVoicePlayer._debugSound;
		}
		set
		{
			HeroVoicePlayer._debugSound = value;
		}
	}

	public Hero hero
	{
		get
		{
			return this._hero;
		}
		set
		{
			this._hero = value;
		}
	}

	public float volume
	{
		get
		{
			return this._volume;
		}
		set
		{
			this._volume = value;
		}
	}

	public AudioClip[] bornClips
	{
		get
		{
			return this._bornClips;
		}
	}

	public AudioClip[] movingClips
	{
		get
		{
			return this._movingClips;
		}
	}

	public AudioClip[] killClips
	{
		get
		{
			return this._killClips;
		}
	}

	public AudioClip[] dieClips
	{
		get
		{
			return this._dieClips;
		}
	}

	public AudioClip[] rebornClips
	{
		get
		{
			return this._rebornClips;
		}
	}

	private bool _speakEnabled
	{
		get
		{
			bool isObserver = Singleton<PvpManager>.Instance.IsObserver;
			Units obserserUnit = PvpObserveMgr.GetObserserUnit();
			return (isObserver && obserserUnit != null && this._hero != null && obserserUnit.unique_id == this._hero.unique_id) || (this._hero != null && this._hero.isPlayer);
		}
	}

	private void addNoNullClip(List<AudioClip> list, AudioClip clip)
	{
		if (clip == null)
		{
			return;
		}
		list.Add(clip);
	}

	public static void playHeroVoice(string heroid, bool stop = false, GameObject gm = null)
	{
		if (AudioMgr.Instance.isVoiceMute())
		{
			return;
		}
		if (string.IsNullOrEmpty(heroid))
		{
			return;
		}
		AudioGameDataLoader.instance.Load();
		List<AudioGameDataLoader.audioBindstruct> list = null;
		if (AudioGameDataLoader.instance._heroVoice.ContainsKey(heroid))
		{
			list = AudioGameDataLoader.instance._heroVoice[heroid];
		}
		if (list == null)
		{
			return;
		}
		AudioGameDataLoader.audioBindstruct audioBindstruct = list.Find((AudioGameDataLoader.audioBindstruct x) => x.desc == "Select");
		if (audioBindstruct != null)
		{
			AudioMgr.Play(audioBindstruct.eventstr, gm, stop, false);
		}
	}

	public void PlayVoice(string voice)
	{
		if (AudioMgr.Instance.isVoiceMute())
		{
			return;
		}
		if (this.playfuncs.ContainsKey(voice) && this.playfuncs[voice] != null)
		{
			this.playfuncs[voice]();
		}
	}

	public void init(string name)
	{
		AudioGameDataLoader.instance.Load();
		if (this._hero == null)
		{
			this._hero = base.gameObject.GetComponent<Hero>();
		}
		this.playfuncs.Clear();
		this.playfuncs.Add("onReborn", new HeroVoicePlayer.playfunc(this.onReborn));
		this.playfuncs.Add("onDie", new HeroVoicePlayer.playfunc(this.onDie));
		this.playfuncs.Add("onNormalAttack", new HeroVoicePlayer.playfunc(this.onNormalAttack));
		this.playfuncs.Add("onUpdateMoveTarget", new HeroVoicePlayer.playfunc(this.onUpdateMoveTarget));
		this.playfuncs.Add("onSkillAttack", new HeroVoicePlayer.playfunc(this.onSkillAttack));
		this.playfuncs.Add("onHeroSelected", new HeroVoicePlayer.playfunc(this.onHeroSelected));
		this.playfuncs.Add("onlevelup", new HeroVoicePlayer.playfunc(this.onlevelup));
		this.playfuncs.Add("onbubing", new HeroVoicePlayer.playfunc(this.onbubing));
		this.playfuncs.Add("onKill", new HeroVoicePlayer.playfunc(this.onKill));
		this.playfuncs.Add("onkill3", new HeroVoicePlayer.playfunc(this.onKill3));
		this._name = name;
		this.loadSoundData(this._name);
	}

	public void loadSoundData(string heroname)
	{
		if (string.IsNullOrEmpty(heroname))
		{
			return;
		}
		List<AudioGameDataLoader.audioBindstruct> list = null;
		if (AudioGameDataLoader.instance._heroVoice.ContainsKey(heroname))
		{
			list = AudioGameDataLoader.instance._heroVoice[heroname];
		}
		if (list == null)
		{
			return;
		}
		AudioGameDataLoader.audioBindstruct audioBindstruct = list.Find((AudioGameDataLoader.audioBindstruct x) => x.desc == "born");
		AudioGameDataLoader.audioBindstruct audioBindstruct2 = list.Find((AudioGameDataLoader.audioBindstruct x) => x.desc == "move");
		AudioGameDataLoader.audioBindstruct audioBindstruct3 = list.Find((AudioGameDataLoader.audioBindstruct x) => x.desc == "die");
		AudioGameDataLoader.audioBindstruct audioBindstruct4 = list.Find((AudioGameDataLoader.audioBindstruct x) => x.desc == "reborn");
		AudioGameDataLoader.audioBindstruct audioBindstruct5 = list.Find((AudioGameDataLoader.audioBindstruct x) => x.desc == "Attack");
		AudioGameDataLoader.audioBindstruct audioBindstruct6 = list.Find((AudioGameDataLoader.audioBindstruct x) => x.desc == "LastKill");
		AudioGameDataLoader.audioBindstruct audioBindstruct7 = list.Find((AudioGameDataLoader.audioBindstruct x) => x.desc == "Kill");
		AudioGameDataLoader.audioBindstruct audioBindstruct8 = list.Find((AudioGameDataLoader.audioBindstruct x) => x.desc == "LevelUp");
		AudioGameDataLoader.audioBindstruct audioBindstruct9 = list.Find((AudioGameDataLoader.audioBindstruct x) => x.desc == "OrgAttack");
		AudioGameDataLoader.audioBindstruct audioBindstruct10 = list.Find((AudioGameDataLoader.audioBindstruct x) => x.desc == "Select");
		if (audioBindstruct != null)
		{
			this.born_event = audioBindstruct.eventstr;
		}
		if (audioBindstruct2 != null)
		{
			this.moving_event = audioBindstruct2.eventstr;
		}
		if (audioBindstruct3 != null)
		{
			this.die_event = audioBindstruct3.eventstr;
		}
		if (audioBindstruct4 != null)
		{
			this.reborn_event = audioBindstruct4.eventstr;
		}
		if (audioBindstruct5 != null)
		{
			this.attackstart_event = audioBindstruct5.eventstr;
		}
		if (audioBindstruct9 != null)
		{
			this.attackoral_event = audioBindstruct9.eventstr;
		}
		if (audioBindstruct10 != null)
		{
			this.heroselected_event = audioBindstruct10.eventstr;
		}
		if (audioBindstruct6 != null)
		{
			this.bubing_event = audioBindstruct6.eventstr;
		}
		if (audioBindstruct8 != null)
		{
			this.levelup_event = audioBindstruct8.eventstr;
		}
		if (audioBindstruct7 != null)
		{
			this.kill3_event = audioBindstruct7.eventstr;
		}
	}

	private void playSound(string strParam, bool stop = false, bool notbreakprev = false)
	{
		if (!HeroVoicePlayer.editorVoiceEnable)
		{
			return;
		}
		if (AudioMgr.Instance.isVoiceMute())
		{
			return;
		}
		if (AudioMgr.Instance.isUsingWWise())
		{
			if (!string.IsNullOrEmpty(strParam))
			{
				AudioMgr.Play(strParam, base.gameObject, stop, notbreakprev);
			}
			return;
		}
	}

	protected override void Start()
	{
		if (HeroVoicePlayer.debugSound)
		{
			return;
		}
		this._lastMoveSoundTiming = Time.time + 15f;
		base.Start();
		if (this._hero == null)
		{
			this._hero = base.gameObject.GetComponent<Hero>();
		}
		base.addMsgLs(typeof(StartFightMsg), new Action<GameMessage>(this.onStartFight));
		base.addMsgLs(typeof(ExitBattleMsg), new Action<GameMessage>(this.onExitBattle));
		base.setInitVol(this._volume);
	}

	private void findHero()
	{
		Transform transform = base.trans;
		while (transform != null)
		{
			this._hero = transform.GetComponent<Hero>();
			if (this._hero != null)
			{
				break;
			}
			transform = base.trans.parent;
		}
	}

	private void onStartFight(GameMessage gameMsg)
	{
		if (HeroVoicePlayer.debugSound)
		{
			return;
		}
		if (!this._speakEnabled)
		{
			return;
		}
		if (AudioMgr.Instance.isUsingWWise())
		{
			this.playSound(this.born_event, false, false);
			return;
		}
		if (this._speakEnabled)
		{
			this.pickOneAndPlay(this._bornClips);
		}
	}

	private void onExitBattle(GameMessage gameMsg)
	{
		this.unloadAudios(this._killClips);
		this.unloadAudios(this._bornClips);
		this.unloadAudios(this._dieClips);
		this.unloadAudios(this._movingClips);
		this.unloadAudios(this._rebornClips);
	}

	private void unloadAudios(AudioClip[] acs)
	{
		for (int i = 0; i < acs.Length; i++)
		{
			AudioClip assetToUnload = acs[i];
			acs[i] = null;
			Resources.UnloadAsset(assetToUnload);
		}
	}

	private void onKill()
	{
		if (HeroVoicePlayer.debugSound)
		{
			return;
		}
		if (AudioMgr.Instance.isUsingWWise())
		{
			this.playSound(this.kill_event, false, false);
			return;
		}
		if (this._speakEnabled)
		{
			this.pickOneAndPlay(this._killClips);
		}
	}

	public void onKill3()
	{
		if (HeroVoicePlayer.debugSound)
		{
			return;
		}
		if (AudioMgr.Instance.isUsingWWise())
		{
			this.playSound(this.kill3_event, false, false);
			return;
		}
	}

	public void onSkillAttack()
	{
		if (!this._speakEnabled)
		{
			return;
		}
		if (AudioMgr.Instance.isUsingWWise())
		{
			this.playSound(this.attackstart_event, false, false);
			return;
		}
	}

	public void onNormalAttack()
	{
		if (!this._speakEnabled)
		{
			return;
		}
		if (AudioMgr.Instance.isUsingWWise())
		{
			this.playSound(this.attackoral_event, false, true);
			return;
		}
	}

	public void onHeroSelected()
	{
		if (AudioMgr.Instance.isUsingWWise())
		{
			this.playSound(this.heroselected_event, false, false);
			return;
		}
	}

	private void onbubing()
	{
		if (!this._speakEnabled)
		{
			return;
		}
		if (AudioMgr.Instance.isUsingWWise())
		{
			this.playSound(this.bubing_event, false, false);
			return;
		}
	}

	public void onlevelup()
	{
		if (!this._speakEnabled)
		{
			return;
		}
		if (AudioMgr.Instance.isUsingWWise())
		{
			this.playSound(this.levelup_event, false, false);
			return;
		}
	}

	public void onkill3()
	{
		if (!this._speakEnabled)
		{
			return;
		}
		if (AudioMgr.Instance.isUsingWWise())
		{
			this.playSound(this.kill3_event, false, false);
			return;
		}
	}

	public void onDie()
	{
		if (HeroVoicePlayer.debugSound)
		{
			return;
		}
		if (AudioMgr.Instance.isUsingWWise())
		{
			if (this._speakEnabled)
			{
				AudioMgr.Stop(base.gameObject);
				this.playSound(this.die_event, true, false);
				AudioMgr.setState(base.gameObject, "PlayerLife", "Dead");
			}
			return;
		}
	}

	public void onReborn()
	{
		if (HeroVoicePlayer.debugSound)
		{
			return;
		}
		if (AudioMgr.Instance.isUsingWWise())
		{
			if (this._speakEnabled)
			{
				this.playSound(this.reborn_event, false, false);
				AudioMgr.setState(base.gameObject, "PlayerLife", "Alive");
			}
			return;
		}
	}

	public void onUpdateMoveTarget()
	{
		if (HeroVoicePlayer.debugSound)
		{
			return;
		}
		if (AudioMgr.Instance.isUsingWWise())
		{
			if (this._speakEnabled)
			{
				this.playSound(this.moving_event, false, true);
			}
			return;
		}
		if (this._speakEnabled)
		{
		}
	}

	private void pickOneAndPlay(AudioClip[] clips)
	{
	}

	private AudioClip getRandomClip(AudioClip[] clips)
	{
		if (ArrayTool.isNullOrEmpty(clips))
		{
			return null;
		}
		int num = UnityEngine.Random.Range(0, clips.Length);
		return clips[num];
	}

	[Conditional("UNITY_EDITOR")]
	public void loadAll(string heroName = "")
	{
	}
}

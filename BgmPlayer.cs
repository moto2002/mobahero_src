using Com.Game.Data;
using Com.Game.Manager;
using System;
using UnityEngine;

public class BgmPlayer : MonoBehaviour
{
	private struct BgmInfo
	{
		public string bgmName;

		public float volume;

		public float fadeInSpeed;

		public float fadeOutSpeed;
	}

	private AudioSourceControl m_audioSourceControl;

	[SerializeField]
	private AudioClip m_nextClip;

	private BgmPlayer.BgmInfo m_currentBgmInfo;

	public SceneType curSceneType = SceneType.Login;

	public static BgmPlayer instance;

	public static GameObject exbgmobject1;

	private static string currplayingBg = string.Empty;

	public static BgmPlayer Instance
	{
		get
		{
			return BgmPlayer.instance;
		}
	}

	private void Awake()
	{
		BgmPlayer.instance = this;
	}

	private void OnDestroy()
	{
		if (BgmPlayer.exbgmobject1 != null)
		{
			UnityEngine.Object.DestroyImmediate(BgmPlayer.exbgmobject1);
			BgmPlayer.exbgmobject1 = null;
		}
	}

	private void Start()
	{
		if (BgmPlayer.exbgmobject1 == null)
		{
			BgmPlayer.exbgmobject1 = new GameObject();
			BgmPlayer.exbgmobject1.transform.parent = AudioMgr.Instance.gameObject.transform;
			UnityEngine.Object.DontDestroyOnLoad(BgmPlayer.exbgmobject1);
			BgmPlayer.exbgmobject1.transform.position = base.gameObject.transform.position;
		}
		MobaMessageManager.RegistMessage((ClientMsg)25009, new MobaMessageFunc(this.LoadSceneComplete));
		MobaMessageManager.RegistMessage((ClientMsg)25010, new MobaMessageFunc(this.ResetListenerTrans));
	}

	public void StopBG()
	{
		AudioMgr.setVolumeBG(0f, false);
	}

	public void StopExBG()
	{
		if (BgmPlayer.exbgmobject1 != null)
		{
			AudioMgr.Stop(BgmPlayer.exbgmobject1);
		}
	}

	public void PlayBG()
	{
		AudioMgr.setVolumeBG(AudioMgr.getVolumeBG(), false);
		this.Play();
	}

	public void StopBGM()
	{
		if (base.gameObject != null)
		{
			AudioMgr.Stop(base.gameObject);
		}
	}

	public void Play()
	{
		AudioMgr.setVolumeBG(AudioMgr.getVolumeBG(), false);
		SceneType sceneType = this.curSceneType;
		if (sceneType != SceneType.Login)
		{
			if (sceneType != SceneType.Home)
			{
				SysBattleSceneVo dataById = BaseDataMgr.instance.GetDataById<SysBattleSceneVo>(LevelManager.CurLevelId);
				if (dataById.belonged_battletype == 5)
				{
					if (BgmPlayer.exbgmobject1 != null)
					{
						AudioMgr.Stop(BgmPlayer.exbgmobject1);
					}
					AudioMgr.PlayBG("Play_3V3V3", base.gameObject, true, false);
					BgmPlayer.currplayingBg = "Play_3V3V3";
				}
				else
				{
					string text = dataById.scene_map_id;
					string[] array = new string[]
					{
						"Play_Amb_1v1_map16",
						"Play_Amb_2v2_map11",
						"Play_Amb_2v2_map13",
						"Play_Amb_2v2_map14",
						"Play_Amb_5v5_map17"
					};
					text = text.ToLower();
					for (int i = 0; i < array.Length; i++)
					{
						if (array[i].Contains(text))
						{
							if (array[i].Contains("1v1"))
							{
								text = "1v1";
							}
							else if (array[i].Contains("5v5"))
							{
								text = "3v3";
							}
							break;
						}
					}
					if (text.Contains("1v1"))
					{
						AudioMgr.setState(base.gameObject, "BGM", "OneVOneLevel01");
					}
					else if (text.Contains("3v3"))
					{
						AudioMgr.setState(base.gameObject, "BGM", "Level01");
					}
					else
					{
						AudioMgr.setState(base.gameObject, "BGM", "Level02");
					}
					AudioMgr.PlayBG("Play_3V3_Switch_Container", base.gameObject, true, false);
					BgmPlayer.currplayingBg = "Play_3V3_Switch_Container";
				}
			}
			else
			{
				if (BgmPlayer.exbgmobject1 != null)
				{
					AudioMgr.Stop(BgmPlayer.exbgmobject1);
				}
				AudioMgr.setState(null, "Interface_Music_State", "browse");
				AudioMgr.PlayBG("Play_Interface_Music", base.gameObject, true, false);
				BgmPlayer.currplayingBg = "Play_Interface_Music";
			}
		}
		else if (ToolsFacade.Instance.IsInXmasTime(DateTime.Now))
		{
			AudioMgr.setState(null, "Interface_Music_State", "Login_xiaolu");
			AudioMgr.PlayBG("Play_Interface_Music", base.gameObject, true, false);
			BgmPlayer.currplayingBg = "Play_Interface_Music";
		}
		else if (ToolsFacade.Instance.IsInNewYearTime(DateTime.Now))
		{
			AudioMgr.setState(null, "Interface_Music_State", "Login_SpringFestival");
			AudioMgr.PlayBG("Play_Interface_Music", base.gameObject, true, false);
			BgmPlayer.currplayingBg = "Play_Interface_Music";
		}
		else
		{
			AudioMgr.setState(null, "Interface_Music_State", "Login_zuolun");
			AudioMgr.PlayBG("Play_Interface_Music", base.gameObject, true, false);
			BgmPlayer.currplayingBg = "Play_Interface_Music";
		}
	}

	public static void PlayWinBG()
	{
		if (!AudioMgr.Instance.isBgMute())
		{
			AudioMgr.PlayBG("Play_Jingle_Win", BgmPlayer.Instance.gameObject, true, false);
			BgmPlayer.currplayingBg = "Play_Jingle_Win";
		}
	}

	public static void PlayLoseBG()
	{
		if (!AudioMgr.Instance.isBgMute())
		{
			AudioMgr.PlayBG("Play_Jingle_Fail", BgmPlayer.Instance.gameObject, true, false);
			BgmPlayer.currplayingBg = "Play_Jingle_Fail";
		}
	}

	public static void OnFstOr2ndTowerBePullDown()
	{
		AudioMgr.setState(BgmPlayer.instance.gameObject, "BGM", "Level03");
		AudioMgr.trigger(BgmPlayer.instance.gameObject, "Stingers");
	}

	public static void OnFirstTowerBePullbyUS()
	{
		AudioMgr.setState(BgmPlayer.instance.gameObject, "BGM", "Level02");
		AudioMgr.trigger(BgmPlayer.instance.gameObject, "Stingers");
	}

	private void ResetListenerTrans(MobaMessage msg)
	{
		this.ChangeListenerParentTrans(base.transform);
	}

	private void LoadSceneComplete(MobaMessage msg)
	{
		if (msg == null || msg.Param == null)
		{
			return;
		}
		int num = (int)msg.Param;
		this.curSceneType = (SceneType)num;
		this.Play();
	}

	public void Play(string clipName, float volume, float fadeInSpeed, float fadeOutSpeed)
	{
	}

	public void ChangeListenerParentTrans(Transform parent)
	{
	}
}

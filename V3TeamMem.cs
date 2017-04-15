using Assets.Scripts.Model;
using Com.Game.Data;
using Com.Game.Manager;
using Com.Game.Module;
using Com.Game.Utils;
using MobaHeros.Pvp;
using MobaMessageData;
using MobaProtocol.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class V3TeamMem : MonoBehaviour
{
	[SerializeField]
	private UITexture sp_summonerSkill;

	[SerializeField]
	private UITexture sp_heroIcon;

	[SerializeField]
	private UILabel lb_heroName;

	[SerializeField]
	private UILabel lb_summonerName;

	[SerializeField]
	private UILabel lb_progress;

	[SerializeField]
	private UILabel lb_net;

	[SerializeField]
	private UISprite loadingFrame;

	private ArrayList msgs;

	private bool isMainPlayer;

	private int displayProgress;

	private int targetProgress;

	private bool finishFlag;

	private Queue<int> qKeyPoint;

	public ReadyPlayerSampleInfo MemInfo
	{
		get;
		set;
	}

	public Action<V3TeamMem> OnLoadFinish
	{
		get;
		set;
	}

	private void Awake()
	{
		this.msgs = new ArrayList
		{
			ClientC2C.SceneManagerLoadComplete
		};
		this.qKeyPoint = new Queue<int>();
	}

	private void Start()
	{
	}

	private void Update()
	{
		if (this.finishFlag)
		{
			return;
		}
		HeroExtraInRoom heroExtraByUserId = Singleton<PvpManager>.Instance.RoomInfo.GetHeroExtraByUserId(this.MemInfo.newUid);
		if (heroExtraByUserId != null && this.targetProgress < heroExtraByUserId.LoadProgress)
		{
			this.targetProgress = heroExtraByUserId.LoadProgress;
		}
		if (this.displayProgress != 100)
		{
			if (this.displayProgress < this.targetProgress)
			{
				if (this.targetProgress > 100)
				{
					this.displayProgress = 100;
				}
				else if (this.targetProgress == 100)
				{
					this.displayProgress = Math.Min(100, this.displayProgress + 10);
				}
				else
				{
					this.displayProgress = Math.Min(this.targetProgress, this.displayProgress + 1);
				}
				this.RefreshUI_progress();
				if (this.isMainPlayer)
				{
					while (this.qKeyPoint.Count > 0)
					{
						int num = this.qKeyPoint.Peek();
						if (this.displayProgress < num)
						{
							break;
						}
						Debug.Log("Self send progress:" + num);
						PvpEvent.SendLoadingProcessEvent((byte)this.qKeyPoint.Dequeue());
					}
				}
			}
		}
		else
		{
			this.finishFlag = true;
			if (this.OnLoadFinish != null)
			{
				this.OnLoadFinish(this);
			}
		}
	}

	private void OnDestroy()
	{
		this.Unregister();
	}

	private void Register()
	{
		MobaMessageManagerTools.RegistMsg(this, this.msgs, true, "OnMsg_");
	}

	private void Unregister()
	{
		MobaMessageManagerTools.RegistMsg(this, this.msgs, false, "OnMsg_");
	}

	public void UpdateCom()
	{
		if (this.MemInfo == null)
		{
			return;
		}
		this.Init();
		this.RefreshUI();
	}

	private void Init()
	{
		this.displayProgress = 0;
		this.targetProgress = 0;
		this.finishFlag = false;
		int myLobbyUserId = Singleton<PvpManager>.Instance.MyLobbyUserId;
		this.isMainPlayer = (this.MemInfo.newUid == myLobbyUserId);
		if (this.isMainPlayer)
		{
			this.msgs.Add(ClientC2V.LoadView_setProgress);
		}
		this.Register();
	}

	private void RefreshUI()
	{
		this.RefreshUI_progress();
		this.RefreshUI_summonerSkill();
		this.RefreshUI_heroIcon();
		this.RefreshUI_heroName();
		this.RefreshUI_summonerName();
		this.RefreshUI_loadingFrame();
		this.RefreshUI_net();
	}

	private void RefreshUI_loadingFrame()
	{
		ReadyPlayerSampleInfo memInfo = this.MemInfo;
		if (this.MemInfo.GetTeam() == Singleton<PvpManager>.Instance.SelfTeamType)
		{
			ToolsFacade.Instance.GetLoadingFrame(memInfo.RankFrame, this.loadingFrame);
		}
	}

	private void RefreshUI_summonerSkill()
	{
		ReadyPlayerSampleInfo memInfo = this.MemInfo;
		this.sp_summonerSkill.gameObject.SetActive(false);
		if (!string.IsNullOrEmpty(memInfo.selfDefSkillId))
		{
			SysSummonersSkillVo dataById = BaseDataMgr.instance.GetDataById<SysSummonersSkillVo>(memInfo.selfDefSkillId);
			if (dataById != null)
			{
				SysSkillMainVo skillMainData = BaseDataMgr.instance.GetSkillMainData(dataById.skill_id);
				if (skillMainData != null)
				{
					this.sp_summonerSkill.mainTexture = ResourceManager.Load<Texture>(skillMainData.skill_icon, true, true, null, 0, false);
					this.sp_summonerSkill.gameObject.SetActive(true);
				}
			}
		}
	}

	private void RefreshUI_heroIcon()
	{
		ReadyPlayerSampleInfo memInfo = this.MemInfo;
		SysHeroMainVo heroMainData = BaseDataMgr.instance.GetHeroMainData(memInfo.GetHeroId());
		if (heroMainData == null)
		{
			ClientLogger.Error("找不到heroMain表信息，heroID=" + memInfo.GetHeroId());
			return;
		}
		string text = string.Empty;
		if (memInfo.heroSkinId != "0" && memInfo.heroSkinId != string.Empty)
		{
			Dictionary<string, object> dicByType = BaseDataMgr.instance.GetDicByType<SysHeroSkinVo>();
			if (dicByType != null && dicByType.ContainsKey(memInfo.heroSkinId))
			{
				SysHeroSkinVo sysHeroSkinVo = dicByType[memInfo.heroSkinId] as SysHeroSkinVo;
				if (sysHeroSkinVo != null)
				{
					text = sysHeroSkinVo.avatar_icon;
				}
				else
				{
					ClientLogger.Error("找不到SysHeroSkinVo表信息，skinID=" + memInfo.heroSkinId);
				}
			}
			else
			{
				ClientLogger.Error("找不到SysHeroSkinVo表信息2，skinID=" + memInfo.heroSkinId);
			}
		}
		else
		{
			text = heroMainData.avatar_icon;
		}
		if (!string.IsNullOrEmpty(text))
		{
			this.sp_heroIcon.mainTexture = ResourceManager.Load<Texture>(text, true, true, null, 0, false);
		}
	}

	private void RefreshUI_heroName()
	{
		SysHeroMainVo heroMainData = BaseDataMgr.instance.GetHeroMainData(this.MemInfo.GetHeroId());
		if (heroMainData != null)
		{
			this.lb_heroName.text = LanguageManager.Instance.GetStringById(heroMainData.name);
		}
		else
		{
			this.lb_heroName.text = string.Empty;
		}
	}

	private void RefreshUI_summonerName()
	{
		this.lb_summonerName.text = this.MemInfo.userName;
	}

	private void RefreshUI_progress()
	{
		this.lb_progress.text = this.displayProgress.ToString() + "%";
	}

	private void RefreshUI_net()
	{
		this.lb_net.gameObject.SetActive(this.isMainPlayer);
	}

	private void OnMsg_LoadView_setProgress(MobaMessage msg)
	{
		int num = 0;
		MsgData_LoadView_setProgress msgData_LoadView_setProgress = msg.Param as MsgData_LoadView_setProgress;
		if (msgData_LoadView_setProgress != null)
		{
			if (!Singleton<PvpManager>.Instance.IsObserver)
			{
				if (msgData_LoadView_setProgress.AddType == MsgData_LoadView_setProgress.SetType.addNum)
				{
					num = this.SetComProgress(msgData_LoadView_setProgress.Num, true);
				}
				else if (msgData_LoadView_setProgress.AddType == MsgData_LoadView_setProgress.SetType.targetNum)
				{
					num = this.SetComProgress(msgData_LoadView_setProgress.Num, false);
				}
				HeroExtraInRoom heroExtraByUserId = Singleton<PvpManager>.Instance.RoomInfo.GetHeroExtraByUserId(this.MemInfo.newUid);
				if (heroExtraByUserId != null)
				{
					heroExtraByUserId.LoadProgress = num;
				}
				this.qKeyPoint.Enqueue(num);
			}
		}
	}

	private void OnMsg_SceneManagerLoadComplete(MobaMessage msg)
	{
		ModelManager.Instance.Apply_SettingDataInBattle();
		this.targetProgress = 100;
	}

	private int SetComProgress(int num, bool add = false)
	{
		if (add)
		{
			num += this.targetProgress;
		}
		if (this.targetProgress < num)
		{
			this.targetProgress = num;
		}
		return this.targetProgress;
	}
}

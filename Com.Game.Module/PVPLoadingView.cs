using Assets.Scripts.Model;
using Com.Game.Data;
using Com.Game.Manager;
using Com.Game.Utils;
using GUIFramework;
using MobaHeros.Pvp;
using MobaMessageData;
using MobaProtocol.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;

namespace Com.Game.Module
{
	public class PVPLoadingView : BaseView<PVPLoadingView>
	{
		private class ProgressInfo
		{
			public int target;

			public int display;
		}

		private const float interval = 0.5f;

		private UIGrid grid_up;

		private UIGrid grid_down;

		private UILabel Center_Label;

		private UISprite bg;

		private PVPHeroCard template_heroUp;

		private PVPHeroCard template_heroDown;

		private List<ReadyPlayerSampleInfo> list_LMPlayer;

		private List<ReadyPlayerSampleInfo> list_BLPlayer;

		private Dictionary<int, PVPHeroCard> dic_com;

		private List<int> list_finishLoadList;

		private Task task_checkProgress;

		private TeamType playerTeamType;

		public PVPLoadingView()
		{
			this.WinResCfg = new WinResurceCfg(false, false, "Prefab/UI/PVPLoading/PVPLoadingView");
		}

		public override void Init()
		{
			base.Init();
			this.grid_up = this.transform.Find("TopAnchor/Grid").GetComponent<UIGrid>();
			this.grid_down = this.transform.Find("BottomAnchor/Grid").GetComponent<UIGrid>();
			this.Center_Label = this.transform.Find("CenterAnchor/Prompt").GetComponent<UILabel>();
			this.bg = this.transform.Find("CenterAnchor/BG").GetComponent<UISprite>();
		}

		public override void RegisterUpdateHandler()
		{
			MobaMessageManager.RegistMessage((ClientMsg)23008, new MobaMessageFunc(this.OnMsg_LoadView_setProgress));
			MobaMessageManager.RegistMessage((ClientMsg)25013, new MobaMessageFunc(this.OnMsg_SceneManagerLoadComplete));
		}

		public override void CancelUpdateHandler()
		{
			MobaMessageManager.UnRegistMessage((ClientMsg)23008, new MobaMessageFunc(this.OnMsg_LoadView_setProgress));
			MobaMessageManager.UnRegistMessage((ClientMsg)25013, new MobaMessageFunc(this.OnMsg_SceneManagerLoadComplete));
		}

		public override void HandleAfterOpenView()
		{
			base.HandleAfterOpenView();
			this.InitData();
			this.LoadHeroCardPrefab();
			this.RefreshUI_grid();
			this.StartCheck();
		}

		public override void HandleBeforeCloseView()
		{
			this.ClearPvpLoadViewResources();
		}

		public override void RefreshUI()
		{
		}

		public override void Destroy()
		{
			base.Destroy();
		}

		private void InitData()
		{
			this.list_finishLoadList = new List<int>();
			this.list_LMPlayer = Singleton<PvpManager>.Instance.GetPlayersByTeam(TeamType.LM);
			this.list_BLPlayer = Singleton<PvpManager>.Instance.GetPlayersByTeam(TeamType.BL);
			this.dic_com = new Dictionary<int, PVPHeroCard>();
			this.playerTeamType = Singleton<PvpManager>.Instance.SelfTeamType;
		}

		private void OnMsg_LoadView_setProgress(MobaMessage msg)
		{
			int num = 0;
			MsgData_LoadView_setProgress msgData_LoadView_setProgress = msg.Param as MsgData_LoadView_setProgress;
			if (msgData_LoadView_setProgress != null)
			{
				if (!Singleton<PvpManager>.Instance.IsObserver)
				{
					int myLobbyUserId = Singleton<PvpManager>.Instance.MyLobbyUserId;
					if (msgData_LoadView_setProgress.AddType == MsgData_LoadView_setProgress.SetType.addNum)
					{
						num = this.SetComProgress(myLobbyUserId, msgData_LoadView_setProgress.Num, true);
					}
					else if (msgData_LoadView_setProgress.AddType == MsgData_LoadView_setProgress.SetType.targetNum)
					{
						num = this.SetComProgress(myLobbyUserId, msgData_LoadView_setProgress.Num, false);
					}
					HeroExtraInRoom heroExtraByUserId = Singleton<PvpManager>.Instance.RoomInfo.GetHeroExtraByUserId(myLobbyUserId);
					if (heroExtraByUserId != null)
					{
						heroExtraByUserId.LoadProgress = num;
					}
					PvpEvent.SendLoadingProcessEvent((byte)num);
				}
			}
		}

		private void OnMsg_SceneManagerLoadComplete(MobaMessage msg)
		{
			ModelManager.Instance.Apply_SettingDataInBattle();
			foreach (KeyValuePair<int, PVPHeroCard> current in this.dic_com)
			{
				current.Value.Progress = 100;
			}
		}

		private void OnLoadFinish(PVPHeroCard com)
		{
			int userID = com.InitInfo.userID;
			if (!this.list_finishLoadList.Contains(userID) && this.dic_com.Keys.Contains(userID))
			{
				this.list_finishLoadList.Add(userID);
				if (this.list_finishLoadList.Count == this.dic_com.Count)
				{
					if (this.task_checkProgress != null)
					{
						this.task_checkProgress.Stop();
					}
					MobaMessageManager.DispatchMsg((ClientMsg)21008, null, 0f);
				}
			}
		}

		private int SetComProgress(int heroID, int num, bool add = false)
		{
			if (this.dic_com.ContainsKey(heroID))
			{
				if (add)
				{
					num += this.dic_com[heroID].Progress;
				}
				this.dic_com[heroID].Progress = num;
			}
			return num;
		}

		private void RefreshUI_grid()
		{
			this.RefreshUI_initGrid(this.grid_up, this.template_heroUp, this.list_LMPlayer);
			this.RefreshUI_initGrid(this.grid_down, this.template_heroDown, this.list_BLPlayer);
		}

		private void RefreshUI_initGrid(UIGrid grid, PVPHeroCard template, List<ReadyPlayerSampleInfo> list)
		{
			GridHelper.FillGrid<PVPHeroCard>(grid, template, (list == null) ? 0 : list.Count, delegate(int idx, PVPHeroCard comp)
			{
				PVPCardInfo pVPCardInfo = this.CreatePVPCardInfo(list[idx]);
				comp.name = pVPCardInfo.SummonerName;
				comp.InitInfo = pVPCardInfo;
				if (pVPCardInfo.userID == Singleton<PvpManager>.Instance.MyLobbyUserId)
				{
					comp.ShowDelay();
				}
				this.dic_com[list[idx].newUid] = comp;
			});
			grid.Reposition();
		}

		private void StartCheck()
		{
			this.task_checkProgress = new Task(this.Coroutine_UpdateProgress(), false);
			this.task_checkProgress.Finished += delegate(bool m)
			{
			};
			this.task_checkProgress.Start();
		}

		[DebuggerHidden]
		private IEnumerator Coroutine_UpdateProgress()
		{
			PVPLoadingView.<Coroutine_UpdateProgress>c__Iterator168 <Coroutine_UpdateProgress>c__Iterator = new PVPLoadingView.<Coroutine_UpdateProgress>c__Iterator168();
			<Coroutine_UpdateProgress>c__Iterator.<>f__this = this;
			return <Coroutine_UpdateProgress>c__Iterator;
		}

		private PVPCardInfo CreatePVPCardInfo(ReadyPlayerSampleInfo readyInfo)
		{
			PVPCardInfo pVPCardInfo = new PVPCardInfo();
			pVPCardInfo.userID = readyInfo.newUid;
			pVPCardInfo.color = ((readyInfo.GetTeam() != TeamType.LM) ? Color.red : Color.blue);
			SysHeroMainVo heroMainData = BaseDataMgr.instance.GetHeroMainData(readyInfo.GetHeroId());
			if (readyInfo.heroSkinId != "0" && readyInfo.heroSkinId != string.Empty)
			{
				SkinInfo skinInfo = new SkinInfo(int.Parse(readyInfo.heroSkinId), 0);
				pVPCardInfo.texture = skinInfo.tex;
			}
			else
			{
				pVPCardInfo.texture = ResourceManager.Load<Texture>(heroMainData.Loading_icon, true, true, null, 0, false);
			}
			pVPCardInfo.HeroName = LanguageManager.Instance.GetStringById(heroMainData.name);
			pVPCardInfo.SummonerName = readyInfo.userName;
			pVPCardInfo.SkillID = readyInfo.selfDefSkillId;
			pVPCardInfo.OnLoadFinish = new Action<PVPHeroCard>(this.OnLoadFinish);
			pVPCardInfo.RankFrame = ((readyInfo.GetTeam() != this.playerTeamType) ? 0 : readyInfo.RankFrame);
			pVPCardInfo.lastCharmRank = readyInfo.CharmRankvalue;
			ClientLogger.Assert(!string.IsNullOrEmpty(readyInfo.userName), null);
			return pVPCardInfo;
		}

		private void LoadHeroCardPrefab()
		{
			this.template_heroUp = Resources.Load<PVPHeroCard>("Prefab/UI/PVPLoading/PVPHeroCard");
			this.template_heroDown = Resources.Load<PVPHeroCard>("Prefab/UI/PVPLoading/PVPHeroCardRed");
		}

		private void UnloadHeroCardPrefab()
		{
			if (this.template_heroUp != null)
			{
				this.template_heroUp.ClearResources();
				this.template_heroUp = null;
			}
			if (this.template_heroDown != null)
			{
				this.template_heroDown.ClearResources();
				this.template_heroDown = null;
			}
		}

		private void DestroyAllChildren(GameObject inGo)
		{
			if (inGo == null)
			{
				return;
			}
			Transform transform = inGo.transform;
			int childCount = transform.childCount;
			for (int i = 0; i < childCount; i++)
			{
				Transform child = transform.GetChild(i);
				if (child != null)
				{
					UnityEngine.Object.Destroy(child.gameObject);
				}
			}
		}

		private void ClearPvpLoadViewResources()
		{
			CachedRes.ClearResources();
			this.UnloadHeroCardPrefab();
			if (this.dic_com != null)
			{
				Dictionary<int, PVPHeroCard>.Enumerator enumerator = this.dic_com.GetEnumerator();
				while (enumerator.MoveNext())
				{
					KeyValuePair<int, PVPHeroCard> current = enumerator.Current;
					PVPHeroCard value = current.Value;
					if (value != null)
					{
						value.ClearResources();
					}
				}
				this.dic_com.Clear();
			}
			if (this.bg != null && this.bg.atlas != null && this.bg.atlas.spriteMaterial != null)
			{
				Texture texture = this.bg.atlas.spriteMaterial.GetTexture("_MainTex");
				if (texture != null)
				{
					Resources.UnloadAsset(texture);
				}
				texture = this.bg.atlas.spriteMaterial.GetTexture("_DetailTex");
				if (texture != null)
				{
					Resources.UnloadAsset(texture);
				}
			}
		}
	}
}

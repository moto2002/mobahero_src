using Assets.Scripts.Model;
using Com.Game.Data;
using Com.Game.Manager;
using Com.Game.Utils;
using ExitGames.Client.Photon;
using GUIFramework;
using MobaProtocol;
using MobaProtocol.Data;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Com.Game.Module
{
	public class RankView : BaseView<RankView>
	{
		public Transform LadderBtn;

		public Transform MagicBtn;

		public Transform CharmingBtn;

		private Transform TeamBtn;

		private Transform ChampionshipBtn;

		private Transform Ladder;

		private Transform MagicPanel;

		private Transform Charming;

		private Transform Team;

		private Transform Championship;

		private Transform EmptyShow;

		private UITexture ladderBannerBgPic;

		private UILabel SeasonPeriod;

		private UILabel RemainingTime;

		private UILabel MyRank;

		private Transform LastSeason;

		private UITexture magicBannerBgPic;

		private UITexture magicBannerTexture;

		private UILabel NowMyRank;

		private UILabel TodayExp;

		private UILabel Reward;

		private Transform CapShop;

		private Transform MagicBottleView;

		private UILabel myCharmRank;

		private UILabel myCharmCount;

		private UILabel mySeason;

		private UIGrid LadderList;

		private List<SummonerLadderRankData> summonerLadderRankDataList = new List<SummonerLadderRankData>();

		private UIGrid MagicList;

		private UIGrid CharmList;

		private List<CharmRankData> charmingRankDataList = new List<CharmRankData>();

		public string targetId;

		private List<RankItem> rankItemList;

		private UIScrollView heroScrollview;

		private UIScrollView magicScrollview;

		private int _chooseType;

		public string selectBtn;

		private RankItem _rankItem;

		private Dictionary<string, object> rankStageDic = new Dictionary<string, object>();

		private CoroutineManager coroutineManager = new CoroutineManager();

		private TweenAlpha m_AlphaController;

		public RankView()
		{
			this.WinResCfg = new WinResurceCfg(true, true, "RankView");
			this.WindowTitle = "排行榜";
		}

		public override void Init()
		{
			base.Init();
			this.EmptyShow = this.transform.Find("Empty");
			this.LadderBtn = this.transform.Find("Left/Grid/Ladder");
			this.MagicBtn = this.transform.Find("Left/Grid/MagicBottle");
			this.CharmingBtn = this.transform.Find("Left/Grid/Charming");
			this.TeamBtn = this.transform.Find("Left/Grid/Team");
			this.ChampionshipBtn = this.transform.Find("Left/Grid/Championship");
			this.Ladder = this.transform.Find("Right/Ladder");
			this.MagicPanel = this.transform.Find("Right/Magic");
			this.Charming = this.transform.Find("Right/Charming");
			this.TeamBtn = this.transform.Find("Left/Grid/Team");
			this.ChampionshipBtn = this.transform.Find("Left/Grid/Championship");
			this.ladderBannerBgPic = this.transform.Find("Right/Ladder/Scroll View/Anchor/Banner").GetComponent<UITexture>();
			this.SeasonPeriod = this.transform.Find("Right/Ladder/Scroll View/Anchor/Banner/SeasonPeriod").GetComponent<UILabel>();
			this.RemainingTime = this.transform.Find("Right/Ladder/Scroll View/Anchor/Banner/RemainingTime").GetComponent<UILabel>();
			this.MyRank = this.transform.Find("Right/Ladder/Scroll View/Anchor/Banner/MyRank").GetComponent<UILabel>();
			this.LastSeason = this.transform.Find("Right/Ladder/Scroll View/Anchor/Banner/LastSeason");
			this.LadderList = this.Ladder.Find("Scroll View/Anchor/Grid").GetComponent<UIGrid>();
			this.MagicList = this.MagicPanel.Find("Scroll View/Anchor/Grid").GetComponent<UIGrid>();
			this.CharmList = this.Charming.Find("Scroll View/Anchor/Grid").GetComponent<UIGrid>();
			this.myCharmRank = this.Charming.Find("Scroll View/Anchor/Banner/MyInfo/MyRank").GetComponent<UILabel>();
			this.myCharmCount = this.Charming.Find("Scroll View/Anchor/Banner/MyInfo/MyExp").GetComponent<UILabel>();
			this.mySeason = this.Charming.Find("Scroll View/Anchor/Banner/Tlite/Season").GetComponent<UILabel>();
			this.magicBannerBgPic = this.transform.Find("Right/Magic/Scroll View/Anchor/Banner").GetComponent<UITexture>();
			this.magicBannerTexture = this.transform.Find("Right/Magic/Scroll View/Anchor/Banner/Texture").GetComponent<UITexture>();
			this.NowMyRank = this.transform.Find("Right/Magic/Scroll View/Anchor/Banner/MyInfo/MyRank").GetComponent<UILabel>();
			this.TodayExp = this.transform.Find("Right/Magic/Scroll View/Anchor/Banner/MyInfo/MyExp").GetComponent<UILabel>();
			this.Reward = this.transform.Find("Right/Magic/Scroll View/Anchor/Banner/MyInfo/Gain").GetComponent<UILabel>();
			this.CapShop = this.transform.Find("Right/Magic/Scroll View/Anchor/Banner/CapShop");
			this.MagicBottleView = this.transform.Find("Right/Magic/Scroll View/Anchor/Banner/MagicBottle");
			UIEventListener expr_2DF = UIEventListener.Get(this.LadderBtn.gameObject);
			expr_2DF.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(expr_2DF.onClick, new UIEventListener.VoidDelegate(this.OnClickLeftBtns));
			UIEventListener expr_310 = UIEventListener.Get(this.MagicBtn.gameObject);
			expr_310.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(expr_310.onClick, new UIEventListener.VoidDelegate(this.OnClickLeftBtns));
			UIEventListener expr_341 = UIEventListener.Get(this.CharmingBtn.gameObject);
			expr_341.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(expr_341.onClick, new UIEventListener.VoidDelegate(this.OnClickLeftBtns));
			UIEventListener expr_372 = UIEventListener.Get(this.TeamBtn.gameObject);
			expr_372.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(expr_372.onClick, new UIEventListener.VoidDelegate(this.OnClickLeftBtns));
			UIEventListener expr_3A3 = UIEventListener.Get(this.ChampionshipBtn.gameObject);
			expr_3A3.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(expr_3A3.onClick, new UIEventListener.VoidDelegate(this.OnClickLeftBtns));
			UIEventListener expr_3D4 = UIEventListener.Get(this.CapShop.gameObject);
			expr_3D4.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(expr_3D4.onClick, new UIEventListener.VoidDelegate(this.OnClickCapShop));
			UIEventListener expr_405 = UIEventListener.Get(this.MagicBottleView.gameObject);
			expr_405.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(expr_405.onClick, new UIEventListener.VoidDelegate(this.OnClickMagic));
			this.m_AlphaController = this.transform.GetComponent<TweenAlpha>();
		}

		public override void HandleAfterOpenView()
		{
			if (this.ladderBannerBgPic != null && this.ladderBannerBgPic.mainTexture == null)
			{
				this.ladderBannerBgPic.mainTexture = Resources.Load<Texture>("Texture/Rank/Ranking_title_bg_01");
			}
			if (this.magicBannerBgPic != null && this.magicBannerBgPic.mainTexture == null)
			{
				this.magicBannerBgPic.mainTexture = Resources.Load<Texture>("Texture/Rank/Ranking_title_bg_02");
			}
			if (this.magicBannerTexture != null && this.magicBannerTexture.mainTexture == null)
			{
				this.magicBannerTexture.mainTexture = Resources.Load<Texture>("Texture/MagicBottleHD/Magic_bottle_12");
			}
			this._rankItem = Resources.Load<RankItem>("Prefab/UI/Rank/RankItem");
			this.m_AlphaController.Begin();
		}

		public override void HandleBeforeCloseView()
		{
			this.ClearResources();
		}

		private void ClearResources()
		{
			if (this.ladderBannerBgPic != null && this.ladderBannerBgPic.mainTexture != null)
			{
				this.ladderBannerBgPic.mainTexture = null;
			}
			if (this.magicBannerBgPic != null && this.magicBannerBgPic.mainTexture != null)
			{
				this.magicBannerBgPic.mainTexture = null;
			}
			if (this.magicBannerTexture != null && this.magicBannerTexture.mainTexture != null)
			{
				this.magicBannerTexture.mainTexture = null;
			}
			if (this._rankItem != null)
			{
				this._rankItem.ClearResources();
				this._rankItem = null;
			}
			if (this.LadderList != null)
			{
				this.ClearRankItemResources(this.LadderList.transform);
			}
			if (this.MagicList != null)
			{
				this.ClearRankItemResources(this.MagicList.transform);
			}
		}

		private void ClearRankItemResources(Transform inParentTrans)
		{
			if (inParentTrans == null)
			{
				return;
			}
			int childCount = inParentTrans.childCount;
			for (int i = 0; i < childCount; i++)
			{
				Transform child = inParentTrans.GetChild(i);
				RankItem component = child.GetComponent<RankItem>();
				if (component != null)
				{
					component.ClearResources();
				}
			}
		}

		public override void RegisterUpdateHandler()
		{
			MVC_MessageManager.AddListener_view(MobaGameCode.GetSummonerLadderRankList, new MobaMessageFunc(this.OnGetMsg_GetSummonerLadderRankList));
			MVC_MessageManager.AddListener_view(MobaGameCode.GetMagicBottleRankList, new MobaMessageFunc(this.OnGetMsg_GetMagicBottleRankList));
			MVC_MessageManager.AddListener_view(MobaGameCode.GetPlayerData, new MobaMessageFunc(this.OnGetMsg_GetPlayerData));
			MVC_MessageManager.AddListener_view(MobaGameCode.GetCharmRankList, new MobaMessageFunc(this.OnGetMsg_GetCharmData));
			if (this.selectBtn == null)
			{
				this.selectBtn = this.LadderBtn.gameObject.name;
			}
			this.UpdateRankPage(this.selectBtn);
			this.rankStageDic = BaseDataMgr.instance.GetDicByType<SysRankStageVo>();
		}

		public override void CancelUpdateHandler()
		{
			MVC_MessageManager.RemoveListener_view(MobaGameCode.GetSummonerLadderRankList, new MobaMessageFunc(this.OnGetMsg_GetSummonerLadderRankList));
			MVC_MessageManager.RemoveListener_view(MobaGameCode.GetMagicBottleRankList, new MobaMessageFunc(this.OnGetMsg_GetMagicBottleRankList));
			MVC_MessageManager.RemoveListener_view(MobaGameCode.GetPlayerData, new MobaMessageFunc(this.OnGetMsg_GetPlayerData));
			MVC_MessageManager.RemoveListener_view(MobaGameCode.GetCharmRankList, new MobaMessageFunc(this.OnGetMsg_GetCharmData));
			this.Ladder.gameObject.SetActive(false);
			this.MagicPanel.gameObject.SetActive(false);
			this.Charming.gameObject.SetActive(false);
			this.MagicBtn.transform.GetComponent<UIToggle>().value = false;
			this.CharmingBtn.transform.GetComponent<UIToggle>().value = false;
			this.LadderBtn.transform.GetComponent<UIToggle>().value = false;
		}

		public override void RefreshUI()
		{
		}

		public void OnClickLeftBtns(GameObject go)
		{
			this.UpdateRankPage(go.name);
			this.selectBtn = go.name;
		}

		private void UpdateRankPage(string name)
		{
			switch (name)
			{
			case "Ladder":
			{
				SendMsgManager.SendMsgParam param = new SendMsgManager.SendMsgParam(true, "正在获得排位赛列表", true, 15f);
				SendMsgManager.Instance.SendMsg(MobaGameCode.GetSummonerLadderRankList, param, new object[0]);
				break;
			}
			case "MagicBottle":
			{
				SendMsgManager.SendMsgParam param2 = new SendMsgManager.SendMsgParam(true, "正在获得小魔瓶经验列表", true, 15f);
				SendMsgManager.Instance.SendMsg(MobaGameCode.GetMagicBottleRankList, param2, new object[0]);
				break;
			}
			case "Charming":
			{
				SendMsgManager.SendMsgParam param3 = new SendMsgManager.SendMsgParam(true, "正在获得魅力值列表", true, 15f);
				SendMsgManager.Instance.SendMsg(MobaGameCode.GetCharmRankList, param3, new object[0]);
				break;
			}
			case "Team":
				Singleton<TipView>.Instance.ShowViewSetText("暂未开放", 1f);
				break;
			case "Championship":
				Singleton<TipView>.Instance.ShowViewSetText("暂未开放", 1f);
				break;
			}
		}

		private void OnClickCapShop(GameObject go)
		{
			Singleton<ShopView>.Instance.ThroughShop = ETypicalShop.CapShop;
			CtrlManager.OpenWindow(WindowID.ShopViewNew, null);
		}

		private void OnClickMagic(GameObject go)
		{
			MobaMessageManagerTools.SendClientMsg(ClientV2C.bottleSystemOpenView, null, false);
		}

		private void UpdateLadderBanner()
		{
			MatchTimeData matchTimeData = ModelManager.Instance.Get_GetSummonerLadderRankList_X().matchTimeData;
			if (matchTimeData == null)
			{
				return;
			}
			if (matchTimeData.BeginTime.Year == 1)
			{
				this.SeasonPeriod.gameObject.SetActive(false);
				this.RemainingTime.text = LanguageManager.Instance.GetStringById("RankListUI_Seasonover");
			}
			else
			{
				this.SeasonPeriod.gameObject.SetActive(true);
				this.SeasonPeriod.text = matchTimeData.BeginTime.ToString("yyyy.M.d") + "~" + matchTimeData.EndTime.ToString("yyyy.M.d");
				DateTime serverCurrentTime = ToolsFacade.ServerCurrentTime;
				if (serverCurrentTime.DayOfYear - matchTimeData.BeginTime.DayOfYear < 0)
				{
					this.RemainingTime.text = LanguageManager.Instance.GetStringById("RankListUI_Seasonawait");
				}
				else
				{
					this.RemainingTime.text = LanguageManager.Instance.GetStringById("RankListUI_Seasontime").Replace("*", (matchTimeData.EndTime.DayOfYear - serverCurrentTime.DayOfYear).ToString());
				}
			}
			for (int i = this.summonerLadderRankDataList.Count - 1; i >= 0; i--)
			{
				if (this.summonerLadderRankDataList[i].UserId == ModelManager.Instance.Get_userData_X().UserId)
				{
					this.MyRank.text = this.summonerLadderRankDataList[i].Rank.ToString();
					ModelManager.Instance.Get_userData_X().LastDayLadderRank = this.summonerLadderRankDataList[i].Rank;
				}
			}
			foreach (MatchRankInfo current in matchTimeData.list)
			{
				if (current.Rk == 0)
				{
					this.LastSeason.gameObject.SetActive(false);
				}
				else
				{
					this.LastSeason.gameObject.SetActive(true);
					this.LastSeason.Find(current.Rk.ToString()).GetComponent<UILabel>().text = current.Rk + "." + current.SumName;
					this.LastSeason.Find(current.Rk.ToString() + "/Label").GetComponent<UILabel>().text = current.Points.ToString();
				}
			}
		}

		private string setMagicText(MagicBottleRankList magicRankDataList)
		{
			if (magicRankDataList.myrank == 0)
			{
				return "0";
			}
			string rank_reward = BaseDataMgr.instance.GetDataById<SysMagicbottleRankrewardsVo>(Tools_ParsePrice.BottleRewardParse(magicRankDataList.myrank)).rank_reward;
			string drop_items = BaseDataMgr.instance.GetDataById<SysDropRewardsVo>(rank_reward).drop_items;
			string[] array = BaseDataMgr.instance.GetDataById<SysDropItemsVo>(drop_items).rewards.Split(new char[]
			{
				'|'
			});
			string text = string.Empty;
			string text2 = array[1];
			switch (text2)
			{
			case "1":
				text = LanguageManager.Instance.GetStringById("Currency_Gold");
				break;
			case "2":
				text = LanguageManager.Instance.GetStringById("Currency_Diamond");
				break;
			case "3":
				text = LanguageManager.Instance.GetStringById("Currency_MagicBottle");
				break;
			}
			return array[2];
		}

		private void OnGetMsg_GetMagicBottleRankList(MobaMessage msg)
		{
			if (msg == null)
			{
				return;
			}
			OperationResponse operationResponse = msg.Param as OperationResponse;
			if (operationResponse == null)
			{
				return;
			}
			int num = (int)operationResponse.Parameters[1];
			MobaErrorCode mobaErrorCode = (MobaErrorCode)num;
			if (mobaErrorCode == MobaErrorCode.Ok)
			{
				this.MagicBtn.transform.GetComponent<UIToggle>().value = true;
				this.Ladder.gameObject.SetActive(false);
				this.MagicPanel.gameObject.SetActive(true);
				this.Charming.gameObject.SetActive(false);
				MagicBottleRankList magicBottleRankList = ModelManager.Instance.Get_GetMagicBottleRankList_X();
				if (magicBottleRankList == null || magicBottleRankList.list == null || magicBottleRankList.list.Count == 0)
				{
					this.EmptyShow.gameObject.SetActive(true);
					return;
				}
				this.EmptyShow.gameObject.SetActive(false);
				this.NowMyRank.text = magicBottleRankList.myrank.ToString();
				this.TodayExp.text = magicBottleRankList.todayexp.ToString();
				this.Reward.text = this.setMagicText(magicBottleRankList);
				magicBottleRankList.list.Sort((MagicBottleRankData x, MagicBottleRankData y) => (x.rank >= y.rank) ? 0 : -1);
				GridHelper.FillGrid<RankItem>(this.MagicList, this._rankItem, magicBottleRankList.list.Count, delegate(int idx, RankItem comp)
				{
					comp.SetAndUpdateType("MagicBottle", idx, this.rankStageDic);
				});
				this.MagicList.Reposition();
			}
		}

		private void OnGetMsg_GetPlayerData(MobaMessage msg)
		{
			if (msg == null)
			{
				return;
			}
			OperationResponse operationResponse = msg.Param as OperationResponse;
			if (operationResponse == null)
			{
				return;
			}
			int num = (int)operationResponse.Parameters[1];
			MobaErrorCode mobaErrorCode = (MobaErrorCode)num;
			if (mobaErrorCode == MobaErrorCode.Ok)
			{
				if (this.targetId == null)
				{
					ClientLogger.Error(" targetId is null @shaohe");
				}
				else
				{
					this.ShowDetailsInfo(this.targetId);
				}
			}
		}

		private void OnGetMsg_GetCharmData(MobaMessage msg)
		{
			if (msg == null)
			{
				return;
			}
			OperationResponse operationResponse = msg.Param as OperationResponse;
			if (operationResponse == null)
			{
				return;
			}
			int num = (int)operationResponse.Parameters[1];
			MobaErrorCode mobaErrorCode = (MobaErrorCode)num;
			if (mobaErrorCode == MobaErrorCode.Ok)
			{
				this.CharmingBtn.transform.GetComponent<UIToggle>().value = true;
				this.Ladder.gameObject.SetActive(false);
				this.MagicPanel.gameObject.SetActive(false);
				this.Charming.gameObject.SetActive(true);
				this.charmingRankDataList = ModelManager.Instance.Get_GetRankList_X().CharmRankList;
				this.mySeason.text = this.RemainingTime.text;
				if (this.charmingRankDataList == null || this.charmingRankDataList.Count == 0)
				{
					this.EmptyShow.gameObject.SetActive(true);
					return;
				}
				this.EmptyShow.gameObject.SetActive(false);
				for (int i = this.charmingRankDataList.Count - 1; i >= 0; i--)
				{
					if (this.charmingRankDataList[i].UserId == ModelManager.Instance.Get_userData_X().UserId)
					{
						this.myCharmRank.text = this.charmingRankDataList[i].Rank.ToString();
						this.myCharmCount.text = this.charmingRankDataList[i].Charm.ToString();
					}
				}
				this.charmingRankDataList.Sort((CharmRankData x, CharmRankData y) => (x.Rank >= y.Rank) ? 0 : -1);
				GridHelper.FillGrid<RankItem>(this.CharmList, this._rankItem, (this.charmingRankDataList.Count >= 50) ? 50 : this.charmingRankDataList.Count, delegate(int idx, RankItem comp)
				{
					comp.SetAndUpdateType("Charming", idx, this.rankStageDic);
				});
				this.CharmList.Reposition();
			}
		}

		private void OnGetMsg_GetSummonerLadderRankList(MobaMessage msg)
		{
			if (msg == null)
			{
				return;
			}
			OperationResponse operationResponse = msg.Param as OperationResponse;
			if (operationResponse == null)
			{
				return;
			}
			int num = (int)operationResponse.Parameters[1];
			MobaErrorCode mobaErrorCode = (MobaErrorCode)num;
			if (mobaErrorCode == MobaErrorCode.Ok)
			{
				this.LadderBtn.transform.GetComponent<UIToggle>().value = true;
				this.summonerLadderRankDataList = ModelManager.Instance.Get_GetSummonerLadderRankList_X().rankList;
				this.Ladder.gameObject.SetActive(true);
				this.MagicPanel.gameObject.SetActive(false);
				this.Charming.gameObject.SetActive(false);
				this.UpdateLadderBanner();
				if (this.summonerLadderRankDataList == null || this.summonerLadderRankDataList.Count == 0)
				{
					this.EmptyShow.gameObject.SetActive(true);
					return;
				}
				this.EmptyShow.gameObject.SetActive(false);
				GridHelper.FillGrid<RankItem>(this.LadderList, this._rankItem, (this.summonerLadderRankDataList.Count >= 50) ? 50 : this.summonerLadderRankDataList.Count, delegate(int idx, RankItem comp)
				{
					comp.SetAndUpdateType("Ladder", idx, this.rankStageDic);
				});
				this.LadderList.Reposition();
			}
		}

		public override void Destroy()
		{
			base.Destroy();
		}

		private void ShowDetailsInfo(string userinfo)
		{
			Singleton<DetailsInfo>.Instance.ShowDetailsInfo(userinfo);
		}
	}
}

using Assets.Scripts.Model;
using Com.Game.Data;
using Com.Game.Manager;
using Com.Game.Utils;
using GUIFramework;
using MobaHeros;
using MobaHeros.Pvp;
using MobaProtocol;
using MobaProtocol.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace Com.Game.Module
{
	public class ArenaModeView : BaseView<ArenaModeView>
	{
		private Transform _center;

		private Transform _bg;

		private Transform _allItem;

		private Transform _kh;

		private Transform _khBack;

		private UILabel _khMatchTime;

		private UILabel _khMatchLabel;

		private Transform _dp;

		private Transform _dpBack;

		private UILabel _dpMatchTime;

		private UILabel _dpMatchLabel;

		private Transform _daluandou;

		private Transform _daluandouBack;

		private UILabel _daluandouMatchTime;

		private UILabel _daluandouMatchLabel;

		private Transform _jbs;

		private Transform _jbsBack;

		private UILabel _jbsMatchTime;

		private UILabel _jbsMatchLabel;

		private Transform _cg;

		private Transform _cgBack;

		private UILabel _cgMatchTime;

		private UILabel _cgMatchLabel;

		private CoroutineManager _coroutineManager = new CoroutineManager();

		private CoroutineManager _timeLockCMgr = new CoroutineManager();

		private Task _task_timeLock;

		private string _selectBattleId;

		private MatchType _recordType;

		private AudioClipInfo _startLoadClipInfo;

		private bool _recordBool;

		private bool isRankClose;

		private bool isLeagueClose;

		private bool clickCd;

		private SysBattleSceneVo _rankSceneVo;

		private SysBattleSceneVo _leagueSceneVo;

		public ArenaModeView()
		{
			this.WinResCfg = new WinResurceCfg(true, true, "ArenaModeView");
			this.WindowTitle = LanguageManager.Instance.GetStringById("PlayUI_Title_ChooseMode");
		}

		public override void Init()
		{
			base.Init();
			this._center = this.transform.Find("CenterAnchor");
			this._bg = this._center.Find("BG");
			this._allItem = this._center.Find("AllItem");
			this._kh = this.transform.Find("CenterAnchor/AllItem/KH");
			this._khBack = this._kh.Find("Bg/BackBtn");
			this._khMatchTime = this._kh.Find("Bg/MatchTime").GetComponent<UILabel>();
			this._khMatchLabel = this._kh.Find("Bg/MatchLabel").GetComponent<UILabel>();
			this._dp = this.transform.Find("CenterAnchor/AllItem/DP");
			this._dpBack = this._dp.Find("Bg/BackBtn");
			this._dpMatchTime = this._dp.Find("Bg/MatchTime").GetComponent<UILabel>();
			this._dpMatchLabel = this._dp.Find("Bg/MatchLabel").GetComponent<UILabel>();
			this._daluandou = this.transform.Find("CenterAnchor/AllItem/DLD");
			this._daluandouBack = this._daluandou.Find("Bg/BackBtn");
			this._daluandouMatchTime = this._daluandou.Find("Bg/MatchTime").GetComponent<UILabel>();
			this._daluandouMatchLabel = this._daluandou.Find("Bg/MatchLabel").GetComponent<UILabel>();
			this._jbs = this.transform.TryFindChild("CenterAnchor/AllItem/JBS");
			this._cg = this.transform.Find("CenterAnchor/AllItem/CG");
			this._cgBack = this._cg.Find("Bg/BackBtn");
			this._cgMatchTime = this._cg.Find("Bg/MatchTime").GetComponent<UILabel>();
			this._cgMatchLabel = this._cg.Find("Bg/MatchLabel").GetComponent<UILabel>();
			this._jbsBack = this._jbs.Find("Bg/BackBtn");
			this._jbsMatchLabel = this._jbs.Find("Bg/MatchLabel").GetComponent<UILabel>();
			this._jbsMatchTime = this._jbs.Find("Bg/MatchTime").GetComponent<UILabel>();
			UIEventListener.Get(this._kh.gameObject).onClick = new UIEventListener.VoidDelegate(this.ClickBtn);
			UIEventListener.Get(this._dp.gameObject).onClick = new UIEventListener.VoidDelegate(this.ClickBtn);
			UIEventListener.Get(this._cg.gameObject).onClick = new UIEventListener.VoidDelegate(this.ClickBtn);
			UIEventListener.Get(this._cgBack.gameObject).onClick = new UIEventListener.VoidDelegate(this.ClickBtn);
			UIEventListener.Get(this._khBack.gameObject).onClick = new UIEventListener.VoidDelegate(this.ClickBtn);
			UIEventListener.Get(this._dpBack.gameObject).onClick = new UIEventListener.VoidDelegate(this.ClickBtn);
			UIEventListener.Get(this._jbsBack.gameObject).onClick = new UIEventListener.VoidDelegate(this.ClickBtn);
			UIEventListener.Get(this._daluandou.gameObject).onClick = new UIEventListener.VoidDelegate(this.ClickBtn);
			UIEventListener.Get(this._daluandouBack.gameObject).onClick = new UIEventListener.VoidDelegate(this.ClickBtn);
			UIEventListener.Get(this._jbs.gameObject).onClick = new UIEventListener.VoidDelegate(this.ClickJbs);
			UIEventListener.Get(this._dp.gameObject).onPress = new UIEventListener.BoolDelegate(this.ShowPressMask);
			UIEventListener.Get(this._jbs.gameObject).onPress = new UIEventListener.BoolDelegate(this.ShowPressMask);
			UIEventListener.Get(this._cg.gameObject).onPress = new UIEventListener.BoolDelegate(this.ShowPressMask);
			UIEventListener.Get(this._daluandou.gameObject).onPress = new UIEventListener.BoolDelegate(this.ShowPressMask);
			UIEventListener.Get(this._kh.gameObject).onPress = new UIEventListener.BoolDelegate(this.ShowPressMask);
			string stringById = LanguageManager.Instance.GetStringById("MainUI_Tips_NeedSummonerLevel");
			SysBattleSceneVo dataById = BaseDataMgr.instance.GetDataById<SysBattleSceneVo>("80006");
			if (stringById != null && dataById != null)
			{
				this._daluandou.FindChild("Lock/Label").GetComponent<UILabel>().text = stringById.Replace("*", dataById.scene_limit_level.ToString());
			}
			SysBattleSceneVo dataById2 = BaseDataMgr.instance.GetDataById<SysBattleSceneVo>("80021");
			if (stringById != null && dataById != null)
			{
				this._jbs.FindChild("Lock/Label").GetComponent<UILabel>().text = stringById.Replace("*", dataById2.scene_limit_level.ToString());
			}
			this.transform.GetComponent<UITweenHelper>().NextPlayDelay = 0.5f;
		}

		public override void HandleAfterOpenView()
		{
			this.clickCd = false;
			SendMsgManager.Instance.SendMsg(MobaGameCode.DealGameReport, null, new object[0]);
			if (Singleton<MenuBottomBarView>.Instance.IsOpen)
			{
				CtrlManager.CloseWindow(WindowID.MenuBottomBarView);
			}
			if (PvpMatchMgr.State == PvpMatchState.Matching)
			{
				this.ShowMatchingFrame();
			}
			else if (PvpMatchMgr.State == PvpMatchState.Matched)
			{
				this.ShowMatchingFrame();
			}
			PvpLevelStorage.DispatchChooseGameMsg();
			bool flag = CharacterDataMgr.instance.GetUserLevel(ModelManager.Instance.Get_userData_X().Exp) >= BaseDataMgr.instance.GetDataById<SysBattleSceneVo>("80006").scene_limit_level;
			this._daluandou.FindChild("Lock").gameObject.SetActive(!flag);
			this._daluandou.GetComponent<BoxCollider>().enabled = flag;
			if (flag)
			{
				this._task_timeLock = this._timeLockCMgr.StartCoroutine(this.RankTimeLock(), true);
			}
			this._timeLockCMgr.StartCoroutine(this.RankSeasonLock(), true);
			bool flag2 = CharacterDataMgr.instance.GetUserLevel(ModelManager.Instance.Get_userData_X().Exp) >= BaseDataMgr.instance.GetDataById<SysBattleSceneVo>("80021").scene_limit_level;
			this._jbs.FindChild("Lock").gameObject.SetActive(!flag2);
			this._jbs.GetComponent<BoxCollider>().enabled = flag2;
			if (flag2)
			{
				this._timeLockCMgr.StartCoroutine(this.LeagueTimeLock(), true);
			}
			AutoTestController.InvokeTestLogic(AutoTestTag.EnterPvp, delegate
			{
				this.ClickFightWithRobot(null);
			}, 1f);
		}

		public override void HandleBeforeCloseView()
		{
			this._timeLockCMgr.StopAllCoroutine();
			this._task_timeLock = null;
		}

		public override void RegisterUpdateHandler()
		{
			this.RegistEvent();
			this.RefreshUI();
			Singleton<MenuTopBarView>.Instance.DoOpenOtherViewAction(0.25f);
			this.transform.GetComponent<UITweenHelper>().Play();
		}

		[DebuggerHidden]
		private IEnumerator PlayOpenAnim()
		{
			ArenaModeView.<PlayOpenAnim>c__Iterator12E <PlayOpenAnim>c__Iterator12E = new ArenaModeView.<PlayOpenAnim>c__Iterator12E();
			<PlayOpenAnim>c__Iterator12E.<>f__this = this;
			return <PlayOpenAnim>c__Iterator12E;
		}

		public override void CancelUpdateHandler()
		{
			MobaMessageManager.UnRegistMessage((ClientMsg)26002, new MobaMessageFunc(this.OnPvpMapChosen));
			MobaMessageManager.UnRegistMessage((ClientMsg)20009, new MobaMessageFunc(this.OnReconnected));
			Singleton<MenuBottomBarView>.Instance.CheckNeedOpen();
		}

		public override void RefreshUI()
		{
			this.UpdateModeView();
		}

		private void RegistEvent()
		{
			MobaMessageManager.RegistMessage((ClientMsg)26002, new MobaMessageFunc(this.OnPvpMapChosen));
			MobaMessageManager.RegistMessage((ClientMsg)20009, new MobaMessageFunc(this.OnReconnected));
		}

		public void ShowConfirmingFrame()
		{
			this._coroutineManager.StopAllCoroutine();
			this._coroutineManager.StartCoroutine(this.WaitConfirmFrame_Coroutine(), true);
			this.ShowItem(false);
			this._bg.gameObject.SetActive(false);
		}

		public void ShowSelectHeroView()
		{
			this.ShowLoadingPanel(false);
			this._coroutineManager.StopAllCoroutine();
			CtrlManager.OpenWindow(WindowID.PvpSelectHeroView, null);
			if (PvpMatchMgr.Instance.TryClearWaitSelectHeroFlag())
			{
				PvpMatchMgr.Instance.OnWaitSelectHeroFlagCleared();
			}
		}

		private void JoinGameAsSingle(string battleId, string hintText = "排队中")
		{
			Singleton<PvpManager>.Instance.ChooseSingleGame(battleId, hintText);
		}

		private void OnReconnected(MobaMessage msg)
		{
			SendMsgManager.Instance.SendMsg(MobaGameCode.DealGameReport, null, new object[0]);
		}

		private void OnPvpMapChosen(MobaMessage msg)
		{
			object[] array = (object[])msg.Param;
			int num = (int)array[0];
			this._recordType = (MatchType)((int)array[1]);
			this._selectBattleId = num.ToString();
			PvpLevelStorage.SetLevel(this._recordType, this._selectBattleId);
			if (this._recordType == MatchType.KH)
			{
				Singleton<PvpRoomView>.Instance.SetMatchInfo(this._selectBattleId, true, FriendOrTeamType.friendType, PvpRoomType.KaiHei);
				CtrlManager.OpenWindow(WindowID.PvpRoomView, null);
			}
			else if (this._recordType == MatchType.DP)
			{
				this.JoinGameAsSingle(this._selectBattleId, "排队中");
			}
			else if (this._recordType == MatchType.FightWithRobot)
			{
				this.HandleFightWithRobotUIProcess();
			}
			else if (this._recordType == MatchType.JBS)
			{
				this.JoinGameAsSingle(this._selectBattleId, "排队中");
			}
			Singleton<UIPvpEntranceCtrl>.Instance.DelayCloseView();
		}

		private void HandleFightWithRobotUIProcess()
		{
			SysBattleSceneVo dataById = BaseDataMgr.instance.GetDataById<SysBattleSceneVo>(this._selectBattleId);
			if (dataById != null)
			{
				if (dataById.hero1_number_cap > 1)
				{
					this.GoFightWithRobotBuildTeam();
				}
				else
				{
					this.GoFightWithRobotWithoutBuildTeam();
				}
			}
		}

		private void GoFightWithRobotBuildTeam()
		{
			Singleton<PvpRoomView>.Instance.SetMatchInfo(this._selectBattleId, true, FriendOrTeamType.friendType, PvpRoomType.KaiHei);
			CtrlManager.OpenWindow(WindowID.PvpRoomView, null);
		}

		private void GoFightWithRobotWithoutBuildTeam()
		{
			this.JoinGameAsSingle(this._selectBattleId, "等待服务器响应");
		}

		private void UpdateModeView()
		{
			this.ShowLoadingPanel(false);
			this._center.gameObject.SetActive(true);
			this._bg.gameObject.SetActive(false);
			this.ShowItem(false);
		}

		public void ShowMatchingFrame()
		{
			this._recordType = MatchType.KH;
			if (Singleton<PvpManager>.Instance.JoinType == PvpJoinType.Single)
			{
				this._recordType = MatchType.DP;
			}
			this._selectBattleId = Singleton<PvpManager>.Instance.BattleId.ToString();
			MatchType recordType = this._recordType;
			if (recordType != MatchType.DP)
			{
				if (recordType == MatchType.KH)
				{
					if (LevelManager.m_CurLevel.IsRank(this._selectBattleId))
					{
						this._coroutineManager.StartCoroutine(this.WaitMatchFrame_Coroutine(this._daluandouMatchTime, this._daluandouMatchLabel, this._daluandou.gameObject), true);
					}
					else if (LevelManager.m_CurLevel.IsFightWithRobot(this._selectBattleId))
					{
						this._coroutineManager.StartCoroutine(this.WaitMatchFrame_Coroutine(this._cgMatchTime, this._cgMatchLabel, this._cg.gameObject), true);
					}
					else
					{
						this._coroutineManager.StartCoroutine(this.WaitMatchFrame_Coroutine(this._khMatchTime, this._khMatchLabel, this._kh.gameObject), true);
					}
				}
			}
			else if (LevelManager.m_CurLevel.IsRank(this._selectBattleId))
			{
				this._coroutineManager.StartCoroutine(this.WaitMatchFrame_Coroutine(this._daluandouMatchTime, this._daluandouMatchLabel, this._daluandou.gameObject), true);
			}
			else if (LevelManager.m_CurLevel.IsFightWithRobot(this._selectBattleId))
			{
				this._coroutineManager.StartCoroutine(this.WaitMatchFrame_Coroutine(this._cgMatchTime, this._cgMatchLabel, this._cg.gameObject), true);
			}
			else if (LevelManager.m_CurLevel.IsLeague(this._selectBattleId))
			{
				this._coroutineManager.StartCoroutine(this.WaitMatchFrame_Coroutine(this._jbsMatchTime, this._jbsMatchLabel, this._jbs.gameObject), true);
			}
			else
			{
				this._coroutineManager.StartCoroutine(this.WaitMatchFrame_Coroutine(this._dpMatchTime, this._dpMatchLabel, this._dp.gameObject), true);
			}
		}

		private void ShowItem(bool isShow)
		{
			foreach (Transform transform in this._allItem)
			{
				if (transform.Find("Bg"))
				{
					transform.Find("Bg").gameObject.SetActive(isShow);
				}
			}
		}

		private void PrepareConfirmFrame()
		{
			this.ShowLoadingPanel(true);
			if (this._startLoadClipInfo.clipName != "sd_battle_win_star")
			{
				this._startLoadClipInfo = default(AudioClipInfo);
				this._startLoadClipInfo.clipName = "sd_battle_win_star";
				this._startLoadClipInfo.audioSourceType = eAudioSourceType.UI;
				this._startLoadClipInfo.audioPriority = 128;
				this._startLoadClipInfo.volume = 1.3f;
			}
			AudioMgr.Play(this._startLoadClipInfo, null);
		}

		[DebuggerHidden]
		private IEnumerator WaitConfirmFrame_Coroutine()
		{
			ArenaModeView.<WaitConfirmFrame_Coroutine>c__Iterator12F <WaitConfirmFrame_Coroutine>c__Iterator12F = new ArenaModeView.<WaitConfirmFrame_Coroutine>c__Iterator12F();
			<WaitConfirmFrame_Coroutine>c__Iterator12F.<>f__this = this;
			return <WaitConfirmFrame_Coroutine>c__Iterator12F;
		}

		[DebuggerHidden]
		private IEnumerator WaitMatchFrame_Coroutine(UILabel label, UILabel label2, GameObject obj)
		{
			ArenaModeView.<WaitMatchFrame_Coroutine>c__Iterator130 <WaitMatchFrame_Coroutine>c__Iterator = new ArenaModeView.<WaitMatchFrame_Coroutine>c__Iterator130();
			<WaitMatchFrame_Coroutine>c__Iterator.obj = obj;
			<WaitMatchFrame_Coroutine>c__Iterator.label = label;
			<WaitMatchFrame_Coroutine>c__Iterator.label2 = label2;
			<WaitMatchFrame_Coroutine>c__Iterator.<$>obj = obj;
			<WaitMatchFrame_Coroutine>c__Iterator.<$>label = label;
			<WaitMatchFrame_Coroutine>c__Iterator.<$>label2 = label2;
			<WaitMatchFrame_Coroutine>c__Iterator.<>f__this = this;
			return <WaitMatchFrame_Coroutine>c__Iterator;
		}

		private string ShowMatchLabel(int number)
		{
			return LanguageManager.Instance.GetStringById("PlayUI_FightingMacth") + ".......";
		}

		public void TravalClick(int type)
		{
			if (PvpMatchMgr.State != PvpMatchState.None)
			{
				return;
			}
			switch (type)
			{
			case 2:
				this.ClickBtn(this._dp.gameObject);
				break;
			case 3:
				this.ClickBtn(this._daluandou.gameObject);
				break;
			case 4:
				this.ClickBtn(this._jbs.gameObject);
				break;
			case 5:
				this.ClickBtn(this._kh.gameObject);
				break;
			case 6:
				this.ClickBtn(this._cg.gameObject);
				break;
			}
		}

		private void ShowPressMask(GameObject obj, bool isPress)
		{
			if (this.clickCd)
			{
				return;
			}
			if (obj == null)
			{
				return;
			}
			GameObject gameObject = obj.transform.FindChild("PressMask").gameObject;
			if (gameObject == null)
			{
				return;
			}
			gameObject.SetActive(isPress);
		}

		private void ClickBtn(GameObject obj)
		{
			if (this.clickCd)
			{
				return;
			}
			if (obj == this._dpBack.gameObject || obj == this._khBack.gameObject || obj == this._daluandouBack.gameObject || obj == this._cgBack.gameObject || obj == this._jbsBack.gameObject)
			{
				this.OnClickCloseButton();
			}
			else
			{
				if (PvpMatchMgr.State == PvpMatchState.Matched)
				{
					return;
				}
				if (obj == this._kh.gameObject)
				{
					this.ClickKH(obj);
				}
				else if (obj == this._dp.gameObject)
				{
					this.ClickDP(obj);
				}
				else if (obj == this._daluandou.gameObject)
				{
					this.ClickRank(obj);
				}
				else if (obj == this._jbs.gameObject)
				{
					this.ClickJbs(obj);
				}
				else if (obj == this._cg.gameObject && !GlobalSettings.Instance.testCGConfigInst.isEnableTestCG)
				{
					this.ClickFightWithRobot(obj);
				}
			}
			this.clickCd = true;
			this._timeLockCMgr.StartCoroutine(this.ClickCDTimer(), true);
		}

		private void ClickKH(GameObject object1 = null)
		{
			if (ModelManager.Instance.IsInPunishmentTime())
			{
				string content = LanguageManager.Instance.GetStringById("Tips_Hung_Punish_Content1").Replace("*", ModelManager.Instance.Get_PunishmentEndTime().ToString("yyyy.MM.dd HH:mm"));
				CtrlManager.ShowMsgBox(LanguageManager.Instance.GetStringById("Tips_Hung_Punish1"), content, delegate
				{
				}, PopViewType.PopOneButton, "确定", "取消", null);
				return;
			}
			Singleton<UIPvpEntranceCtrl>.Instance.matchType = MatchType.KH;
			CtrlManager.OpenWindow(WindowID.PvpEntranceView, null);
		}

		private void ClickZPW(GameObject object1 = null)
		{
			if (LevelManager.Instance.CheckSceneIsUnLock(3))
			{
				CtrlManager.OpenWindow(WindowID.CardView, null);
			}
		}

		private void ClickQX(GameObject object1 = null)
		{
			if (LevelManager.Instance.CheckSceneIsUnLock(4))
			{
				CtrlManager.OpenWindow(WindowID.XLDView, null);
			}
		}

		private void ClickDP(GameObject object1 = null)
		{
			if (ModelManager.Instance.IsInPunishmentTime())
			{
				string content = LanguageManager.Instance.GetStringById("Tips_Hung_Punish_Content1").Replace("*", ModelManager.Instance.Get_PunishmentEndTime().ToString("yyyy.MM.dd HH:mm"));
				CtrlManager.ShowMsgBox(LanguageManager.Instance.GetStringById("Tips_Hung_Punish1"), content, delegate
				{
				}, PopViewType.PopOneButton, "确定", "取消", null);
				return;
			}
			Singleton<UIPvpEntranceCtrl>.Instance.matchType = MatchType.DP;
			CtrlManager.OpenWindow(WindowID.PvpEntranceView, null);
		}

		private void ClickRank(GameObject obj = null)
		{
			if (this._daluandou.FindChild("TimeLock").gameObject.activeInHierarchy)
			{
				return;
			}
			if (ModelManager.Instance.IsInPunishmentTime())
			{
				string content = LanguageManager.Instance.GetStringById("Tips_Hung_Punish_Content1").Replace("*", ModelManager.Instance.Get_PunishmentEndTime().ToString("yyyy.MM.dd HH:mm"));
				CtrlManager.ShowMsgBox(LanguageManager.Instance.GetStringById("Tips_Hung_Punish1"), content, delegate
				{
				}, PopViewType.PopOneButton, "确定", "取消", null);
				return;
			}
			Singleton<UIPvpEntranceCtrl>.Instance.matchType = MatchType.PWS;
			CtrlManager.OpenWindow(WindowID.PvpEntranceView, null);
		}

		[DebuggerHidden]
		private IEnumerator ClickCDTimer()
		{
			ArenaModeView.<ClickCDTimer>c__Iterator131 <ClickCDTimer>c__Iterator = new ArenaModeView.<ClickCDTimer>c__Iterator131();
			<ClickCDTimer>c__Iterator.<>f__this = this;
			return <ClickCDTimer>c__Iterator;
		}

		[DebuggerHidden]
		private IEnumerator RankSeasonLock()
		{
			ArenaModeView.<RankSeasonLock>c__Iterator132 <RankSeasonLock>c__Iterator = new ArenaModeView.<RankSeasonLock>c__Iterator132();
			<RankSeasonLock>c__Iterator.<>f__this = this;
			return <RankSeasonLock>c__Iterator;
		}

		private bool IsRankLockTime()
		{
			if (this._rankSceneVo == null)
			{
				this._rankSceneVo = BaseDataMgr.instance.GetDataById<SysBattleSceneVo>("80006");
			}
			bool flag;
			return !ToolsFacade.Instance.IsInTimeInterval(this._rankSceneVo.scene_open_time, out flag);
		}

		[DebuggerHidden]
		private IEnumerator RankTimeLock()
		{
			ArenaModeView.<RankTimeLock>c__Iterator133 <RankTimeLock>c__Iterator = new ArenaModeView.<RankTimeLock>c__Iterator133();
			<RankTimeLock>c__Iterator.<>f__this = this;
			return <RankTimeLock>c__Iterator;
		}

		private bool IsLeagueLockTime()
		{
			if (this._leagueSceneVo == null)
			{
				this._leagueSceneVo = BaseDataMgr.instance.GetDataById<SysBattleSceneVo>("80007");
			}
			bool flag;
			return !ToolsFacade.Instance.IsInTimeInterval(this._leagueSceneVo.scene_open_time, out flag);
		}

		[DebuggerHidden]
		private IEnumerator LeagueTimeLock()
		{
			ArenaModeView.<LeagueTimeLock>c__Iterator134 <LeagueTimeLock>c__Iterator = new ArenaModeView.<LeagueTimeLock>c__Iterator134();
			<LeagueTimeLock>c__Iterator.<>f__this = this;
			return <LeagueTimeLock>c__Iterator;
		}

		private void ClickJbs(GameObject obj = null)
		{
			if (ModelManager.Instance.IsInPunishmentTime())
			{
				string content = LanguageManager.Instance.GetStringById("Tips_Hung_Punish_Content1").Replace("*", ModelManager.Instance.Get_PunishmentEndTime().ToString("yyyy.MM.dd HH:mm"));
				CtrlManager.ShowMsgBox(LanguageManager.Instance.GetStringById("Tips_Hung_Punish1"), content, delegate
				{
				}, PopViewType.PopOneButton, "确定", "取消", null);
				return;
			}
			Singleton<UIPvpEntranceCtrl>.Instance.matchType = MatchType.JBS;
			CtrlManager.OpenWindow(WindowID.PvpEntranceView, null);
		}

		private bool GetDaLuanDouBattleId(out int outDaLuanDouBattleId)
		{
			outDaLuanDouBattleId = 0;
			Dictionary<string, object> dicByType = BaseDataMgr.instance.GetDicByType<SysBattleSceneVo>();
			foreach (KeyValuePair<string, object> current in dicByType)
			{
				SysBattleSceneVo sysBattleSceneVo = current.Value as SysBattleSceneVo;
				if (sysBattleSceneVo != null && sysBattleSceneVo.belonged_battletype == 3 && int.TryParse(sysBattleSceneVo.scene_id, out outDaLuanDouBattleId))
				{
					return true;
				}
			}
			return false;
		}

		private void ClickZDS(GameObject object1 = null)
		{
			Singleton<TipView>.Instance.ShowViewSetText("未开放", 1f);
		}

		private void ClickFightWithRobot(GameObject inObj = null)
		{
			Singleton<UIPvpEntranceCtrl>.Instance.matchType = MatchType.FightWithRobot;
			CtrlManager.OpenWindow(WindowID.PvpEntranceView, null);
		}

		private void OnClickCloseButton()
		{
			PvpMatchMgr.Instance.LeaveQueue();
		}

		private void ClickChoiceHero(GameObject obj = null)
		{
			Singleton<PvpManager>.Instance.ConfirmReady(true);
		}

		private void HideWaitFrame()
		{
			this._coroutineManager.StopAllCoroutine();
			this.ShowItem(false);
			this._bg.gameObject.SetActive(false);
		}

		private void HideConfirmFrame()
		{
			this._coroutineManager.StopAllCoroutine();
			this.ShowLoadingPanel(false);
		}

		private void ShowLoadingPanel(bool type)
		{
			if (type)
			{
			}
		}

		public static void ShowMatchingState(bool shown)
		{
			if (shown)
			{
				if (Singleton<PvpRoomView>.Instance.GetRoomType() != PvpRoomType.ZiDingYi)
				{
					CtrlManager.CloseWindow(WindowID.PvpRoomView);
					CtrlManager.OpenWindow(WindowID.ArenaModeView, null);
					Singleton<ArenaModeView>.Instance.ShowMatchingFrame();
				}
			}
			else if (Singleton<ArenaModeView>.Instance.IsOpened)
			{
				Singleton<ArenaModeView>.Instance.HideWaitFrame();
			}
		}
	}
}

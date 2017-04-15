using Assets.Scripts.Model;
using Com.Game.Module;
using MobaHeros.Pvp;
using MobaProtocol;
using MobaProtocol.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEngine;

namespace Assets.Scripts.GUILogic.View.BattleSettlement
{
	public class Settlement_Common : MonoBehaviour
	{
		private bool initFlag;

		private GameObject mTop;

		private BoxCollider mBGCollider;

		private UISprite mTitle;

		private GameObject mBattleInfo;

		private Transform mBtns;

		private UIButton mBackBtn;

		private UIButton mAgainBtn;

		private UIButton mShareBtn;

		private UIButton mDetailBtn;

		private UIButton mPicBtn;

		private UIButton mRecordBtn;

		private UIToggle mReportBtn;

		private Transform mTipLabel;

		private GameObject mEffObject;

		private List<UIToggle> mReportItems;

		private UIPanel mReportSendPanel;

		private UIPanel mReportCallbackPanel;

		private GameObject mReportSubmitBtn;

		private GameObject mReportCancelBtn;

		private GameObject mReportConfirmBtn;

		private GameObject Fx_BattleWin;

		private GameObject Fx_BattleLose;

		private CoroutineManager cMgr = new CoroutineManager();

		private GameObject SettleEffect;

		private bool isVictory;

		private VictPlayerData reportTargetPlayer;

		private bool reportTargetIsFriend;

		private bool clickFlag = true;

		private Task clickRecordTask;

		private Queue<int> reportItemRecord = new Queue<int>();

		private void Awake()
		{
			if (!this.initFlag)
			{
				this.Init();
			}
			this.Fx_BattleWin = ResourceManager.LoadPath<GameObject>("Prefab/Effects/UIEffect/Fx_BattleWin", null, 0);
			this.Fx_BattleLose = ResourceManager.LoadPath<GameObject>("Prefab/Effects/UIEffect/Fx_BattleLose", null, 0);
			MobaMessageManager.RegistMessage((ClientMsg)23029, new MobaMessageFunc(this.onMsg_showCommon));
			MobaMessageManager.RegistMessage((ClientMsg)23035, new MobaMessageFunc(this.onMsg_showRank));
			MobaMessageManager.RegistMessage((ClientMsg)23030, new MobaMessageFunc(this.onMsg_showSummoner));
			MobaMessageManager.RegistMessage((ClientMsg)23031, new MobaMessageFunc(this.onMsg_showPve));
			MobaMessageManager.RegistMessage((ClientMsg)23032, new MobaMessageFunc(this.onMsg_showNormal));
			MobaMessageManager.RegistMessage((ClientMsg)23033, new MobaMessageFunc(this.onMsg_showInfo));
			MobaMessageManager.RegistMessage((ClientMsg)23034, new MobaMessageFunc(this.onMsg_showSurprise));
			MobaMessageManager.RegistMessage((ClientMsg)23036, new MobaMessageFunc(this.onMsg_showChaosInfo));
			MobaMessageManager.RegistMessage((ClientMsg)23037, new MobaMessageFunc(this.onMsg_showAchievement));
			MobaMessageManager.RegistMessage((ClientMsg)21032, new MobaMessageFunc(this.onMsg_forceContinue));
			MobaMessageManager.RegistMessage((ClientMsg)21030, new MobaMessageFunc(this.onMsg_showReportPanel));
		}

		private void OnDestroy()
		{
			this.cMgr.StopAllCoroutine();
			this.SettleEffect = null;
			this.Fx_BattleLose = null;
			this.Fx_BattleWin = null;
			MobaMessageManager.UnRegistMessage((ClientMsg)23029, new MobaMessageFunc(this.onMsg_showCommon));
			MobaMessageManager.UnRegistMessage((ClientMsg)23035, new MobaMessageFunc(this.onMsg_showRank));
			MobaMessageManager.UnRegistMessage((ClientMsg)23030, new MobaMessageFunc(this.onMsg_showSummoner));
			MobaMessageManager.UnRegistMessage((ClientMsg)23031, new MobaMessageFunc(this.onMsg_showPve));
			MobaMessageManager.UnRegistMessage((ClientMsg)23032, new MobaMessageFunc(this.onMsg_showNormal));
			MobaMessageManager.UnRegistMessage((ClientMsg)23033, new MobaMessageFunc(this.onMsg_showInfo));
			MobaMessageManager.UnRegistMessage((ClientMsg)23034, new MobaMessageFunc(this.onMsg_showSurprise));
			MobaMessageManager.UnRegistMessage((ClientMsg)23036, new MobaMessageFunc(this.onMsg_showChaosInfo));
			MobaMessageManager.UnRegistMessage((ClientMsg)23037, new MobaMessageFunc(this.onMsg_showAchievement));
			MobaMessageManager.UnRegistMessage((ClientMsg)21032, new MobaMessageFunc(this.onMsg_forceContinue));
			MobaMessageManager.UnRegistMessage((ClientMsg)21030, new MobaMessageFunc(this.onMsg_showReportPanel));
		}

		private void Init()
		{
			this.mTop = base.transform.Find("Top").gameObject;
			this.mBGCollider = base.transform.Find("BG").GetComponent<BoxCollider>();
			this.mTitle = this.mTop.transform.FindChild("title").GetComponent<UISprite>();
			this.mBattleInfo = this.mTop.transform.FindChild("BattleInfo").gameObject;
			this.mBtns = base.transform.Find("btns");
			this.mBackBtn = this.mBtns.FindChild("backBtn").GetComponent<UIButton>();
			this.mAgainBtn = this.mBtns.FindChild("againBtn").GetComponent<UIButton>();
			this.mShareBtn = this.mBtns.FindChild("shareBtn").GetComponent<UIButton>();
			this.mDetailBtn = this.mBtns.FindChild("detailBtn").GetComponent<UIButton>();
			this.mPicBtn = this.mBtns.FindChild("PicBtn").GetComponent<UIButton>();
			this.mRecordBtn = this.mBtns.FindChild("recordBtn").GetComponent<UIButton>();
			this.mReportBtn = this.mBtns.FindChild("reportBtn").GetComponent<UIToggle>();
			this.mTipLabel = base.transform.Find("TipLabel");
			this.mEffObject = base.transform.Find("EffObject").gameObject;
			this.mReportSendPanel = base.transform.Find("ReportSender").GetComponent<UIPanel>();
			this.mReportCallbackPanel = base.transform.Find("ReportCallback").GetComponent<UIPanel>();
			this.mReportCancelBtn = base.transform.Find("ReportSender/cancel").gameObject;
			this.mReportConfirmBtn = base.transform.Find("ReportCallback/confirm").gameObject;
			this.mReportSubmitBtn = base.transform.Find("ReportSender/submit").gameObject;
			UIEventListener.Get(this.mBGCollider.gameObject).onClick = new UIEventListener.VoidDelegate(this.onClickContinue);
			UIEventListener.Get(this.mBackBtn.gameObject).onClick = new UIEventListener.VoidDelegate(this.onClickBack);
			UIEventListener.Get(this.mAgainBtn.gameObject).onClick = new UIEventListener.VoidDelegate(this.onClickAgian);
			UIEventListener.Get(this.mShareBtn.gameObject).onClick = new UIEventListener.VoidDelegate(this.onClickShare);
			UIEventListener.Get(this.mDetailBtn.gameObject).onClick = new UIEventListener.VoidDelegate(this.onClickInfo);
			UIEventListener.Get(this.mRecordBtn.gameObject).onClick = new UIEventListener.VoidDelegate(this.onClickRecord);
			UIEventListener.Get(this.mReportBtn.gameObject).onClick = new UIEventListener.VoidDelegate(this.onClickReport);
			UIEventListener.Get(this.mReportCancelBtn).onClick = new UIEventListener.VoidDelegate(this.onClickReportCancel);
			UIEventListener.Get(this.mReportConfirmBtn).onClick = new UIEventListener.VoidDelegate(this.onClickReportConfirm);
			UIEventListener.Get(this.mReportSubmitBtn).onClick = new UIEventListener.VoidDelegate(this.onClickReportSubmit);
			this.mReportItems = new List<UIToggle>();
			Transform transform = base.transform.FindChild("ReportSender/Grid");
			for (int i = 0; i < transform.childCount; i++)
			{
				UIToggle component = transform.GetChild(i).FindChild("toggle").GetComponent<UIToggle>();
				if (component != null)
				{
					this.mReportItems.Add(component);
					UIEventListener.Get(component.gameObject).onClick = new UIEventListener.VoidDelegate(this.onClickReportItem);
				}
			}
			this.mAgainBtn.isEnabled = PvpLevelStorage.CanFightAgain;
			this.mDetailBtn.gameObject.SetActive(false);
			this.mBackBtn.transform.FindChild("Label").GetComponent<UILabel>().text = LanguageManager.Instance.GetStringById("BattleSettlement_Return");
			this.mAgainBtn.transform.FindChild("Label").GetComponent<UILabel>().text = LanguageManager.Instance.GetStringById("BattleSettlement_OneMoreTime");
			this.mDetailBtn.transform.FindChild("Label").GetComponent<UILabel>().text = LanguageManager.Instance.GetStringById("BattleSettlement_DataDetail");
			this.mShareBtn.transform.FindChild("Label").GetComponent<UILabel>().text = LanguageManager.Instance.GetStringById("BattleSettlement_Share");
			this.mShareBtn.transform.FindChild("tipLabel").GetComponent<UILabel>().text = LanguageManager.Instance.GetStringById("BattleSettlement_ShareDesc");
			this.initFlag = true;
		}

		private void InitData()
		{
			if (Singleton<PvpManager>.Instance.IsObserver)
			{
				return;
			}
			this.mBattleInfo.transform.FindChild("BattleName").GetComponent<UILabel>().text = ModelManager.Instance.Get_Settle_MapName();
			this.mBattleInfo.transform.FindChild("BattleTime").GetComponent<UILabel>().text = ModelManager.Instance.Get_Settle_Time();
			this.mBattleInfo.transform.FindChild("BattleMode").GetComponent<UILabel>().text = ModelManager.Instance.Get_Settle_BattleTypeText();
			this.mBattleInfo.transform.FindChild("BattlePlayer").GetComponent<UILabel>().text = ModelManager.Instance.Get_Settle_BattlePlayersText();
		}

		[DebuggerHidden]
		private IEnumerator TimeDelay(float t, Callback del)
		{
			Settlement_Common.<TimeDelay>c__Iterator105 <TimeDelay>c__Iterator = new Settlement_Common.<TimeDelay>c__Iterator105();
			<TimeDelay>c__Iterator.t = t;
			<TimeDelay>c__Iterator.del = del;
			<TimeDelay>c__Iterator.<$>t = t;
			<TimeDelay>c__Iterator.<$>del = del;
			return <TimeDelay>c__Iterator;
		}

		private void TipLabel_SetActive_true()
		{
			this.mBGCollider.enabled = true;
			this.mTipLabel.gameObject.SetActive(true);
		}

		private void TipLabel_SetActive_false()
		{
			this.mBGCollider.enabled = false;
			this.mTipLabel.gameObject.SetActive(false);
		}

		private void ShowEffect()
		{
			this.SettleEffect = NGUITools.AddChild(this.mEffObject, (!this.isVictory) ? this.Fx_BattleLose : this.Fx_BattleWin);
		}

		private void PlayChaosWinVoice()
		{
			switch (Singleton<PvpManager>.Instance.RoomInfo.WinTeam.Value)
			{
			case TeamType.LM:
				AudioMgr.Play("Play_Businessman_BattleOver03", null, false, false);
				break;
			case TeamType.BL:
				AudioMgr.Play("Play_Businessman_BattleOver02", null, false, false);
				break;
			case TeamType.Team_3:
				AudioMgr.Play("Play_Businessman_BattleOver01", null, false, false);
				break;
			}
		}

		private void PlayVictorySound()
		{
			AudioMgr.PlayUI("Play_Jingle_Win_SFX", null, false, false);
		}

		private void PlayDefeatSound()
		{
			AudioMgr.PlayUI("Play_Jingle_Fail_SFX", null, false, false);
		}

		private void onMsg_showCommon(MobaMessage msg)
		{
			if (LevelManager.m_CurLevel.IsFightWithRobot() || LevelManager.m_CurLevel.IsBattleNewbieGuide() || LevelManager.m_CurLevel.IsSolo(LevelManager.CurBattleId) || ModelManager.Instance.Get_Settle_IsSelfDefine())
			{
				this.mReportBtn.gameObject.SetActive(false);
			}
			this.mReportSendPanel.depth = Singleton<BattleSettlementView>.Instance.transform.GetComponent<UIPanel>().depth + 5;
			this.mReportCallbackPanel.depth = this.mReportSendPanel.depth + 1;
			this.clickFlag = true;
			this.cMgr.StartCoroutine(this.clickCD(), true);
			this.InitData();
			this.isVictory = ((bool)msg.Param || Singleton<PvpManager>.Instance.IsObserver);
			this.cMgr.StartCoroutine(this.TimeDelay(1f, new Callback(this.TipLabel_SetActive_true)), true);
			CtrlManager.CloseWindow(WindowID.SettlementMaskView);
			if (this.isVictory)
			{
				this.cMgr.StartCoroutine(this.TimeDelay(0.05f, new Callback(this.PlayVictorySound)), true);
			}
			else
			{
				this.cMgr.StartCoroutine(this.TimeDelay(0.05f, new Callback(this.PlayDefeatSound)), true);
			}
			if (LevelManager.m_CurLevel.Is3V3V3())
			{
				this.cMgr.StartCoroutine(this.TimeDelay(2f, new Callback(this.PlayChaosWinVoice)), true);
			}
			this.cMgr.StartCoroutine(this.TimeDelay(0.05f, new Callback(this.ShowEffect)), true);
		}

		private void onMsg_showRank(MobaMessage msg)
		{
			if (!this.mBattleInfo.gameObject.activeInHierarchy)
			{
				this.mBattleInfo.gameObject.SetActive(true);
			}
			if (!this.mTop.gameObject.activeInHierarchy)
			{
				this.mTop.SetActive(true);
			}
			this.mTitle.spriteName = "Settlement_images_qualifying";
			this.mTitle.MakePixelPerfect();
		}

		private void onMsg_showSummoner(MobaMessage msg)
		{
			if (!this.mBattleInfo.gameObject.activeInHierarchy)
			{
				this.mBattleInfo.gameObject.SetActive(true);
			}
			if (!this.mTop.gameObject.activeInHierarchy)
			{
				this.mTop.SetActive(true);
			}
			this.mTitle.spriteName = "Settlement_images_summoner_level";
			this.mTitle.MakePixelPerfect();
		}

		private void onMsg_showPve(MobaMessage msg)
		{
			if (!this.mTop.gameObject.activeInHierarchy)
			{
				this.mTop.SetActive(true);
			}
			if (this.mBattleInfo.gameObject.activeInHierarchy)
			{
				this.mBattleInfo.SetActive(false);
			}
			this.mTitle.spriteName = "Settlement_images_pingxing";
			this.mTitle.MakePixelPerfect();
		}

		private void onMsg_showNormal(MobaMessage msg)
		{
			if (!this.mTop.gameObject.activeInHierarchy)
			{
				this.mTop.SetActive(true);
			}
			if (!this.mBattleInfo.gameObject.activeInHierarchy)
			{
				this.mBattleInfo.SetActive(true);
			}
			bool flag = (bool)msg.Param;
			this.mTitle.spriteName = ((!flag) ? "Settlement_images_lose" : "Settlement_images_win");
			this.mTitle.MakePixelPerfect();
		}

		private void onMsg_showInfo(MobaMessage msg)
		{
			if (!this.mTop.gameObject.activeInHierarchy)
			{
				this.mTop.SetActive(true);
			}
			if (Singleton<PvpManager>.Instance.IsObserver)
			{
				this.mBattleInfo.SetActive(false);
				this.mTitle.spriteName = "Settlement_images_win";
				this.mTitle.MakePixelPerfect();
				this.mBtns.gameObject.SetActive(false);
				this.TipLabel_SetActive_true();
			}
			else
			{
				this.mTitle.spriteName = ((!this.isVictory) ? "Settlement_images_lose" : "Settlement_images_win");
				this.mTitle.MakePixelPerfect();
				this.mBtns.gameObject.SetActive(true);
				this.TipLabel_SetActive_false();
			}
		}

		private void onMsg_showChaosInfo(MobaMessage msg)
		{
			if (this.mTop.gameObject.activeInHierarchy)
			{
				this.mTop.SetActive(false);
			}
			if (Singleton<PvpManager>.Instance.IsObserver)
			{
				this.mBtns.gameObject.SetActive(false);
				this.TipLabel_SetActive_true();
			}
			else
			{
				this.mBtns.gameObject.SetActive(true);
				this.mReportBtn.gameObject.SetActive(false);
				this.TipLabel_SetActive_false();
			}
		}

		private void onMsg_showAchievement(MobaMessage msg)
		{
			if (!this.mTop.gameObject.activeInHierarchy)
			{
				this.mTop.SetActive(true);
			}
			this.mTitle.spriteName = "Settlement_images_summoner_achievement";
			this.mTitle.MakePixelPerfect();
		}

		private void onMsg_showSurprise(MobaMessage msg)
		{
			if (GameManager.IsGameNone())
			{
				base.gameObject.SetActive(false);
			}
			else
			{
				this.mTop.SetActive(false);
				this.mBtns.gameObject.SetActive(false);
			}
		}

		private void onMsg_showReportPanel(MobaMessage msg)
		{
			if (ModelManager.Instance.Get_userData_X().ReportCount <= 0)
			{
				Singleton<TipView>.Instance.ShowViewSetText(LanguageManager.Instance.GetStringById("Whistleblower_Hint_010", "本周举报次数已用完，感谢您对游戏环境做出的不懈努力！"), 1f);
				return;
			}
			object[] array = msg.Param as object[];
			if (array == null || array.Length != 2)
			{
				return;
			}
			this.reportTargetPlayer = (array[0] as VictPlayerData);
			this.reportTargetIsFriend = (bool)array[1];
			string text = LanguageManager.Instance.GetStringById("Whistleblower_Hint_002").Replace("*", "[F9C356]" + this.reportTargetPlayer.SummonerName + "[-]");
			this.mReportSendPanel.transform.FindChild("textLabel").GetComponent<UILabel>().text = text;
			this.mReportSendPanel.transform.FindChild("count").GetComponent<UILabel>().text = LanguageManager.Instance.GetStringById("Whistleblower_Hint_011", "（剩余举报次数：[E10620]*[-]次）").Replace("*", ModelManager.Instance.Get_userData_X().ReportCount.ToString());
			this.mReportSendPanel.gameObject.SetActive(true);
			for (int i = 0; i < this.mReportItems.Count; i++)
			{
				this.mReportItems[i].value = false;
			}
		}

		private void onMsg_forceContinue(MobaMessage msg)
		{
			this.cMgr.StopAllCoroutine();
			this.clickFlag = true;
			this.cMgr.StartCoroutine(this.clickCD(), true);
		}

		[DebuggerHidden]
		private IEnumerator clickCD()
		{
			Settlement_Common.<clickCD>c__Iterator106 <clickCD>c__Iterator = new Settlement_Common.<clickCD>c__Iterator106();
			<clickCD>c__Iterator.<>f__this = this;
			return <clickCD>c__Iterator;
		}

		private void onClickContinue(GameObject obj = null)
		{
			if (this.clickFlag)
			{
				return;
			}
			this.clickFlag = true;
			this.cMgr.StartCoroutine(this.clickCD(), true);
			if (this.SettleEffect != null)
			{
				UnityEngine.Object.Destroy(this.SettleEffect);
			}
			MobaMessage message = MobaMessageManager.GetMessage((ClientMsg)21025, null, 0f);
			MobaMessageManager.ExecuteMsg(message);
		}

		private void onClickBack(GameObject obj = null)
		{
			if (this.clickFlag)
			{
				return;
			}
			MobaMessage message = MobaMessageManager.GetMessage((ClientMsg)21026, null, 0f);
			MobaMessageManager.ExecuteMsg(message);
		}

		private void onClickAgian(GameObject obj = null)
		{
			if (this.clickFlag)
			{
				return;
			}
			if (this.mAgainBtn.isEnabled)
			{
				MobaMessage message = MobaMessageManager.GetMessage((ClientMsg)21027, null, 0f);
				MobaMessageManager.ExecuteMsg(message);
			}
		}

		private void onClickShare(GameObject obj = null)
		{
			GameManager.Instance.DoShareSDK(3, new Rect(0f, (float)Screen.height * 0.14f, (float)Screen.width, (float)Screen.height * 0.86f), null);
		}

		private void onClickInfo(GameObject obj = null)
		{
			Singleton<StatisticView>.Instance.SetModel(false);
			CtrlManager.OpenWindow(WindowID.StatisticView, null);
			Singleton<StatisticView>.Instance.gameObject.layer = LayerMask.NameToLayer("UIEffect");
		}

		private void onClickPic(GameObject obj = null)
		{
			Singleton<CaptureScreenView>.Instance.HideObject(base.gameObject);
			Singleton<CaptureScreenView>.Instance.SetRoomId(Singleton<PvpManager>.Instance.RoomInfo.RoomId.ToString());
			CtrlManager.OpenWindow(WindowID.CaptureScreenView, null);
		}

		private void onClickRecord(GameObject obj = null)
		{
			this.clickRecordTask = this.cMgr.StartCoroutine(this.RecordReplay(), true);
		}

		private void onClickReport(GameObject obj = null)
		{
			bool value = this.mReportBtn.value;
			MobaMessage message = MobaMessageManager.GetMessage((ClientMsg)21029, value, 0f);
			MobaMessageManager.ExecuteMsg(message);
		}

		private void onClickReportItem(GameObject obj = null)
		{
			int num = int.Parse(obj.transform.parent.name);
			if (obj.GetComponent<UIToggle>().value)
			{
				this.reportItemRecord.Enqueue(num);
			}
			else if (this.reportItemRecord.Contains(num))
			{
				Queue<int> queue = new Queue<int>();
				foreach (int current in this.reportItemRecord)
				{
					if (current != num)
					{
						queue.Enqueue(current);
					}
				}
				this.reportItemRecord = queue;
			}
			if (this.reportItemRecord.Count > 1)
			{
				this.reportItemRecord.Dequeue();
			}
			foreach (UIToggle current2 in this.mReportItems)
			{
				if (this.reportItemRecord.Contains(int.Parse(current2.transform.parent.name)))
				{
					current2.value = true;
				}
				else
				{
					current2.value = false;
				}
			}
		}

		private void onClickReportCancel(GameObject obj = null)
		{
			this.reportItemRecord.Clear();
			this.mReportSendPanel.gameObject.SetActive(false);
			MobaMessage message = MobaMessageManager.GetMessage((ClientMsg)21031, false, 0f);
			MobaMessageManager.ExecuteMsg(message);
		}

		private void onClickReportConfirm(GameObject obj = null)
		{
			this.mReportCallbackPanel.gameObject.SetActive(false);
		}

		private void onClickReportSubmit(GameObject obj = null)
		{
			if (this.reportTargetPlayer != null && this.reportItemRecord.Count > 0)
			{
				GameReportData gameReportData = new GameReportData();
				gameReportData.battleid = int.Parse(LevelManager.CurBattleId);
				gameReportData.reporter = ModelManager.Instance.Get_userData_filed_X("UserId");
				gameReportData.reportype = (ReportType)this.reportItemRecord.Dequeue();
				gameReportData.userid = this.reportTargetPlayer.UserID;
				gameReportData.roomgid = Singleton<PvpManager>.Instance.RoomGid;
				gameReportData.allytype = ((!this.reportTargetIsFriend) ? 1 : 0);
				SendMsgManager.Instance.SendMsg(MobaGameCode.GameReport, null, new object[]
				{
					gameReportData
				});
				ModelManager.Instance.Get_userData_X().ReportCount--;
			}
			else if (this.reportItemRecord.Count == 0)
			{
				Singleton<TipView>.Instance.ShowViewSetText(LanguageManager.Instance.GetStringById("Whistleblower_Hint_009", "举报理由不可为空，请重新选择。"), 1f);
				return;
			}
			this.mReportSendPanel.gameObject.SetActive(false);
			this.mReportCallbackPanel.gameObject.SetActive(true);
			this.reportItemRecord.Clear();
			MobaMessage message = MobaMessageManager.GetMessage((ClientMsg)21031, true, 0f);
			MobaMessageManager.ExecuteMsg(message);
		}

		private bool IsFreeMemoryEnough()
		{
			return true;
		}

		[DebuggerHidden]
		private IEnumerator RecordReplay()
		{
			Settlement_Common.<RecordReplay>c__Iterator107 <RecordReplay>c__Iterator = new Settlement_Common.<RecordReplay>c__Iterator107();
			<RecordReplay>c__Iterator.<>f__this = this;
			return <RecordReplay>c__Iterator;
		}

		private void UpdateProgressBar(UISprite bar, UISprite light, UILabel label, int percentNum)
		{
			float num = (float)percentNum / 100f;
			bar.width = (int)(340f * num);
			light.fillAmount = num;
			label.text = LanguageManager.Instance.GetStringById("LiveorPlayback_PlaybackHint001").Replace("*", percentNum.ToString());
		}

		private string GetRecordInfo()
		{
			Units player = PlayerControlMgr.Instance.GetPlayer();
			if (player == null)
			{
				return string.Empty;
			}
			PvpStatisticMgr.HeroData heroData = Singleton<PvpManager>.Instance.StatisticMgr.GetHeroData(player.unique_id);
			if (heroData == null)
			{
				heroData = new PvpStatisticMgr.HeroData(0);
			}
			int heroKill = heroData.HeroKill;
			int death = heroData.Death;
			int assist = heroData.Assist;
			string text = ModelManager.Instance.Get_Settle_BattleTypeText();
			string npc_id = player.npc_id;
			string text2 = ToolsFacade.ServerCurrentTime.ToString("yyyy.MM.dd HH:mm");
			string text3 = (!ModelManager.Instance.Get_Settle_ImMvp()) ? "0" : "1";
			return string.Format("{0}|{1}|{2}|{3}|{4}|{5}|{6}", new object[]
			{
				heroKill,
				death,
				assist,
				text,
				npc_id,
				text2,
				text3
			});
		}

		private void DoOnRecordingFinish(Exception exception)
		{
			this.mRecordBtn.transform.FindChild("ProgressBar").gameObject.SetActive(false);
			if (exception == null)
			{
				if (this.clickRecordTask != null)
				{
					this.cMgr.StopCoroutine(this.clickRecordTask);
				}
				this.clickRecordTask = null;
				Singleton<TipView>.Instance.ShowViewSetText(LanguageManager.Instance.GetStringById("LiveorPlayback_Playback007", "保存成功"), 1f);
				this.mRecordBtn.isEnabled = false;
				return;
			}
			if (exception is IOException)
			{
				this.TipOnMemoryDeficiency();
				return;
			}
			Singleton<TipView>.Instance.ShowViewSetText(LanguageManager.Instance.GetStringById("LiveorPlayback_Playback005", "保存失败") + exception.ToString(), 1f);
			UnityEngine.Debug.LogError("Replay Saving Error: " + exception.ToString());
		}

		private void TipOnMemoryDeficiency()
		{
			if (GameManager.Instance.ReplayController.GetReplayMetaInfo().ReplayEntryList.Count < 1)
			{
				Singleton<TipView>.Instance.ShowViewSetText("抱歉，磁盘空间不足，无法保存回放", 1f);
				return;
			}
			CtrlManager.ShowMsgBox(LanguageManager.Instance.GetStringById("LiveorPlayback_Playback005", "保存失败"), LanguageManager.Instance.GetStringById("LiveorPlayback_Playback006", "磁盘空间不足，是否删除本地保存最早的战斗数据？"), delegate(bool isConfirm)
			{
				if (!isConfirm)
				{
					return;
				}
				this.DeleteFirstRecord();
			}, PopViewType.PopTwoButton, "确定", "取消", null);
		}

		private void DeleteFirstRecord()
		{
			if (GameManager.Instance.ReplayController.DeleteFirstRecord())
			{
				Singleton<TipView>.Instance.ShowViewSetText(LanguageManager.Instance.GetStringById("LiveorPlayback_Playback009", "删除完成"), 1f);
			}
			else
			{
				Singleton<TipView>.Instance.ShowViewSetText(LanguageManager.Instance.GetStringById("LiveorPlayback_Playback010", "删除失败"), 1f);
			}
		}

		public void NewbieDestroyResultEffect()
		{
			if (this.SettleEffect != null)
			{
				UnityEngine.Object.Destroy(this.SettleEffect);
				this.SettleEffect = null;
			}
		}
	}
}

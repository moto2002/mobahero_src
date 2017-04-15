using Assets.Scripts.GUILogic.View.HomeChatView;
using Assets.Scripts.Model;
using Com.Game.Data;
using Com.Game.Manager;
using Com.Game.Utils;
using ExitGames.Client.Photon;
using GUIFramework;
using MobaHeros.Pvp;
using MobaHeros.Pvp.State;
using MobaProtocol;
using MobaProtocol.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace Com.Game.Module
{
	public class MenuBottomBarView : BaseView<MenuBottomBarView>
	{
		private Transform _task;

		private Transform _eachDayTask;

		private Transform _mail;

		private Transform _heroSacrificial;

		private Transform _chat;

		private Transform _playBtn;

		private Transform _friend;

		private Transform messageBox;

		private Transform hitchChat;

		private Transform emojiHitch;

		private Transform _taskBtn;

		private Transform _eachDayTaskBtn;

		private Transform _mailBtn;

		private Transform _heroSacrificialBtn;

		private Transform _friendBtn;

		private Transform _taskTip;

		private EffectDelayActive _taskTipEffect;

		private UILabel chatContent;

		public UILabel testLabel;

		private UIPanel vipScroll;

		private object[] mgs;

		private Coroutine coroutine;

		private Task task_showVipMessage;

		private TweenPosition tween_ChatPosition;

		private Vector3 originPosition;

		private UIAtlas atlas;

		private UILabel friendNum;

		private SimpleToggle _playBtnStates;

		private TweenPosition _tPos;

		private float fromY;

		private float toY;

		private UIAnchor anchor;

		private bool _isInit;

		private Task _updatePlayBtnTask;

		private readonly CoroutineManager _coroutineManager = new CoroutineManager();

		private readonly Dictionary<int, List<string>> _newsShowDic = new Dictionary<int, List<string>>();

		private int oldFriendNum;

		private bool isSetFriendNew;

		public MenuBottomBarView()
		{
			this.WinResCfg = new WinResurceCfg(true, true, "MenuBottomBarView");
		}

		public override void Init()
		{
			base.Init();
			this.mgs = new object[]
			{
				MobaChatCode.Chat_Recv
			};
			this.messageBox = this.transform.Find("BottomAnchor/Button/MsgBox");
			this._task = this.transform.Find("BottomAnchor/Button/Task");
			this._eachDayTask = this.transform.Find("BottomAnchor/Button/EachDayTask");
			this._mail = this.transform.Find("BottomAnchor/Button/Mail");
			this._heroSacrificial = this.transform.Find("BottomAnchor/Button/HeroSacrificial");
			this._chat = this.transform.Find("BottomAnchor/Button/MsgBox/category");
			this.hitchChat = this.messageBox.Find("ScrollView/Hitch");
			this.chatContent = this.messageBox.Find("ScrollView/Hitch/content").GetComponent<UILabel>();
			this.testLabel = this.transform.Find("TestLabel").GetComponent<UILabel>();
			this.vipScroll = this.messageBox.Find("ScrollView").GetComponent<UIPanel>();
			this.tween_ChatPosition = this.hitchChat.GetComponent<TweenPosition>();
			this.originPosition = this.hitchChat.localPosition;
			this.atlas = this.transform.GetComponent<HomeChatContent>().atlas;
			this.emojiHitch = this.hitchChat.Find("EmojiManager");
			this._friend = this.transform.Find("BottomAnchor/Button/Friend");
			this._taskBtn = this.transform.Find("BottomAnchor/Button/Task/Task");
			this._eachDayTaskBtn = this.transform.Find("BottomAnchor/Button/EachDayTask/light");
			this._mailBtn = this.transform.Find("BottomAnchor/Button/Mail/light");
			this._heroSacrificialBtn = this.transform.Find("BottomAnchor/Button/HeroSacrificial/light");
			this._friendBtn = this.transform.Find("BottomAnchor/Button/Friend/light");
			this._taskTip = this.transform.FindChild("TaskTips");
			this._taskTipEffect = this._taskTip.FindChild("Fx_ui_daytask").GetComponent<EffectDelayActive>();
			this._playBtn = this.transform.Find("BottomAnchor/Button/PlayBtn");
			this.friendNum = this._friend.Find("Friend/Num").GetComponent<UILabel>();
			this._playBtnStates = this._playBtn.GetComponent<SimpleToggle>();
			this._tPos = this.transform.Find("BottomAnchor").GetComponent<TweenPosition>();
			this.anchor = this.transform.Find("BottomAnchor").GetComponent<UIAnchor>();
			UIEventListener.Get(this._taskBtn.gameObject).onClick = new UIEventListener.VoidDelegate(this.ClickTask);
			UIEventListener.Get(this._eachDayTaskBtn.gameObject).onClick = new UIEventListener.VoidDelegate(this.ClickEachDay);
			UIEventListener.Get(this._mailBtn.gameObject).onClick = new UIEventListener.VoidDelegate(this.ClickMail);
			UIEventListener.Get(this._heroSacrificialBtn.gameObject).onClick = new UIEventListener.VoidDelegate(this.ClickHeroSacrificial);
			UIEventListener.Get(this._chat.gameObject).onClick = new UIEventListener.VoidDelegate(this.ClickChat);
			UIEventListener.Get(this._playBtn.gameObject).onClick = new UIEventListener.VoidDelegate(this.ClickPlayBtn);
			UIEventListener.Get(this._friendBtn.gameObject).onClick = new UIEventListener.VoidDelegate(this.ClickFriend);
		}

		public override void RegisterUpdateHandler()
		{
			if (this.transform == null)
			{
				return;
			}
			if (!this._isInit)
			{
				this._isInit = true;
				MVC_MessageManager.AddListener_view(MobaFriendCode.Friend_GetFriendList, new MobaMessageFunc(this.OnGetMsg_GetFriendList));
				MVC_MessageManager.AddListener_view(MobaFriendCode.Friend_ModifyFriendStatus, new MobaMessageFunc(this.OnGetMsg_ModifyFriendStatus));
				MVC_MessageManager.AddListener_view(MobaGameCode.GetFriendMessages, new MobaMessageFunc(this.OnGetMsg_GetFriendMessages));
				PvpStateManager.Instance.OnStateChanged += new PvpStateManager.StateChanged(this.OnPvpStateChange);
			}
			if (this.transform.GetComponent<UIPanel>() != null)
			{
				this.transform.GetComponent<UIPanel>().alpha = 0f;
			}
			if (this._coroutineManager != null)
			{
				this._coroutineManager.StartCoroutine(this.WaitForDoAction(), true);
			}
			if (this._coroutineManager != null)
			{
				this._coroutineManager.StopCoroutine(this.task_showVipMessage);
			}
			this.task_showVipMessage = this._coroutineManager.StartCoroutine(this.CheckVipMessages(), true);
			this._friend.Find("Message").gameObject.SetActive(this.isSetFriendNew);
		}

		public override void Destroy()
		{
			base.Destroy();
			this._coroutineManager.StopAllCoroutine();
			this.task_showVipMessage = null;
		}

		private void OnPvpStateChange(PvpStateCode oldState, PvpStateCode newState)
		{
			if (newState == PvpStateCode.Home)
			{
				this.RefreshPlayBtn();
			}
		}

		[DebuggerHidden]
		public IEnumerator WaitForDoAction()
		{
			MenuBottomBarView.<WaitForDoAction>c__Iterator13B <WaitForDoAction>c__Iterator13B = new MenuBottomBarView.<WaitForDoAction>c__Iterator13B();
			<WaitForDoAction>c__Iterator13B.<>f__this = this;
			return <WaitForDoAction>c__Iterator13B;
		}

		public override void CancelUpdateHandler()
		{
			if (this._isInit)
			{
				this._isInit = false;
				MVC_MessageManager.RemoveListener_view(MobaFriendCode.Friend_GetFriendList, new MobaMessageFunc(this.OnGetMsg_GetFriendList));
				MVC_MessageManager.RemoveListener_view(MobaFriendCode.Friend_ModifyFriendStatus, new MobaMessageFunc(this.OnGetMsg_ModifyFriendStatus));
				MVC_MessageManager.RemoveListener_view(MobaGameCode.GetFriendMessages, new MobaMessageFunc(this.OnGetMsg_GetFriendMessages));
				PvpStateManager.Instance.OnStateChanged -= new PvpStateManager.StateChanged(this.OnPvpStateChange);
				this._playBtn.Find("Nomal/Fx_Text_play_liuguang").gameObject.SetActive(false);
				this._tPos.PlayReverse();
			}
			if (this._taskTipEffect.activeInHierarchy)
			{
				this._taskTipEffect.SetActive(false, 0f);
			}
			this._taskTip.gameObject.SetActive(false);
		}

		public void HidePlayBtnEffect()
		{
			if (this._playBtn != null)
			{
				this._playBtn.Find("Nomal/Fx_Text_play_liuguang").gameObject.SetActive(false);
			}
		}

		private void OnGetMsg_ModifyFriendStatus(MobaMessage msg)
		{
			SendMsgManager.Instance.SendMsg(MobaFriendCode.Friend_GetFriendList, null, new object[0]);
		}

		private void OnGetMsg_GetFriendList(MobaMessage msg)
		{
			Singleton<MenuBottomBarView>.Instance.UpdateFriendNum();
		}

		private void OnGetMsg_GetFriendMessages(MobaMessage msg)
		{
		}

		public void SetFriendNew(bool isShow)
		{
			if (this._friend != null)
			{
				this.isSetFriendNew = isShow;
				this._friend.Find("Message").gameObject.SetActive(isShow);
			}
		}

		[DebuggerHidden]
		private IEnumerator CheckStates()
		{
			MenuBottomBarView.<CheckStates>c__Iterator13C <CheckStates>c__Iterator13C = new MenuBottomBarView.<CheckStates>c__Iterator13C();
			<CheckStates>c__Iterator13C.<>f__this = this;
			return <CheckStates>c__Iterator13C;
		}

		[DebuggerHidden]
		private IEnumerator CheckNeedOpenDelay()
		{
			return new MenuBottomBarView.<CheckNeedOpenDelay>c__Iterator13D();
		}

		public void CheckNeedOpen()
		{
			this._coroutineManager.StartCoroutine(this.CheckNeedOpenDelay(), true);
		}

		public override void HandleAfterOpenView()
		{
			this._coroutineManager.StartCoroutine(this.CheckStates(), true);
			this.RefreshView();
			if (null != this.chatContent)
			{
				this.chatContent.text = "点击可查看综合聊天";
			}
		}

		public override void HandleBeforeCloseView()
		{
			if (this._playBtnStates.ActiveIndex == 0)
			{
				this._playBtnStates.transform.FindChild("Nomal/Fx_PlayBth_loop").GetComponent<EffectDelayActive>().SetActive(false, 0f);
			}
		}

		private void RefreshView()
		{
			this.RefreshPlayBtn();
		}

		public void RefreshPlayBtn()
		{
			if (this._playBtnStates == null)
			{
				return;
			}
			if (PvpMatchMgr.State == PvpMatchState.None)
			{
				this._playBtnStates.ActiveIndex = 0;
				this._playBtnStates.transform.FindChild("Nomal/Fx_PlayBth_loop").GetComponent<EffectDelayActive>().SetActive(true, 0f);
			}
			else if (PvpMatchMgr.State == PvpMatchState.Matching)
			{
				this._playBtnStates.ActiveIndex = 1;
				Task.Clear(ref this._updatePlayBtnTask);
				this._updatePlayBtnTask = new Task(this.UpdatePlayBtn_Coroutine(), true);
			}
			else
			{
				Singleton<MenuTopBarView>.Instance.TryShowTimeTips();
				this._playBtnStates.ActiveIndex = 2;
			}
		}

		[DebuggerHidden]
		private IEnumerator UpdatePlayBtn_Coroutine()
		{
			MenuBottomBarView.<UpdatePlayBtn_Coroutine>c__Iterator13E <UpdatePlayBtn_Coroutine>c__Iterator13E = new MenuBottomBarView.<UpdatePlayBtn_Coroutine>c__Iterator13E();
			<UpdatePlayBtn_Coroutine>c__Iterator13E.<>f__this = this;
			return <UpdatePlayBtn_Coroutine>c__Iterator13E;
		}

		private void ClickChat(GameObject obj = null)
		{
			if (null != Singleton<HomeChatview>.Instance.transform)
			{
				if (Singleton<HomeChatview>.Instance.gameObject.activeInHierarchy)
				{
					CtrlManager.CloseWindow(WindowID.HomeChatview);
				}
				else
				{
					CtrlManager.OpenWindow(WindowID.HomeChatview, null);
					MobaMessageManagerTools.SendClientMsg(ClientV2C.chatviewOpenView, (Singleton<PvpManager>.Instance.JoinType != PvpJoinType.SefDefineGame && Singleton<PvpManager>.Instance.JoinType != PvpJoinType.Team) ? ChitchatType.Hall : ChitchatType.Lobby, false);
					MobaMessageManagerTools.SendClientMsg(ClientV2C.chatviewFillChatHistory, false, false);
				}
			}
			else
			{
				CtrlManager.OpenWindow(WindowID.HomeChatview, null);
				MobaMessageManagerTools.SendClientMsg(ClientV2C.chatviewOpenView, (Singleton<PvpManager>.Instance.JoinType != PvpJoinType.SefDefineGame && Singleton<PvpManager>.Instance.JoinType != PvpJoinType.Team) ? ChitchatType.Hall : ChitchatType.Lobby, false);
				MobaMessageManagerTools.SendClientMsg(ClientV2C.chatviewFillChatHistory, true, false);
			}
		}

		private void ClickFriend(GameObject objct1 = null)
		{
			this.HideNews(14);
			CtrlManager.OpenWindow(WindowID.FriendView, null);
			CtrlManager.CloseWindow(WindowID.MenuBottomBarView);
		}

		private void ClickPlayBtn(GameObject objct1 = null)
		{
			Singleton<MenuBottomBarView>.Instance.uiWindow.DataCfg.IsDelayClose = true;
			CtrlManager.OpenWindow(WindowID.ArenaModeView, null);
			CtrlManager.CloseWindow(WindowID.MenuBottomBarView);
			Singleton<MenuBottomBarView>.Instance.uiWindow.DataCfg.IsDelayClose = false;
		}

		public void NewbieClickPlayBtn()
		{
			this.ClickPlayBtn(null);
		}

		private void ClickTask(GameObject objct1 = null)
		{
			this.HideNews(2);
			CtrlManager.OpenWindow(WindowID.TaskView, null);
			CtrlManager.CloseWindow(WindowID.MenuBottomBarView);
		}

		public void NewbieClickTask()
		{
			this.ClickTask(null);
		}

		public void NewbieClickDaily()
		{
			this.ClickEachDay(null);
		}

		private void ClickEachDay(GameObject objct1 = null)
		{
			this.HideNews(1);
			CtrlManager.OpenWindow(WindowID.DailyView, null);
			CtrlManager.CloseWindow(WindowID.MenuBottomBarView);
		}

		private void ClickHeroSacrificial(GameObject objct1 = null)
		{
			this.HideNews(3);
			MobaMessageManagerTools.SendClientMsg(ClientC2V.OpenSacrificial, null, false);
		}

		private void ClickMail(GameObject object1 = null)
		{
			if (LevelManager.Instance.CheckSceneIsUnLock(16))
			{
				this.HideNews(10);
				CtrlManager.OpenWindow(WindowID.MailView, null);
				CtrlManager.CloseWindow(WindowID.MenuBottomBarView);
			}
		}

		[DebuggerHidden]
		private IEnumerator CheckVipMessages()
		{
			MenuBottomBarView.<CheckVipMessages>c__Iterator13F <CheckVipMessages>c__Iterator13F = new MenuBottomBarView.<CheckVipMessages>c__Iterator13F();
			<CheckVipMessages>c__Iterator13F.<>f__this = this;
			return <CheckVipMessages>c__Iterator13F;
		}

		private int NumModelData()
		{
			List<ChatMessageNew> list = ModelManager.Instance.Get_Hall_Chat_VIP_DataX();
			int result;
			if (list == null || list.Count == 0)
			{
				result = 0;
			}
			else
			{
				result = list.Count;
			}
			return result;
		}

		private void DealWithMessage()
		{
			if (this.hitchChat.gameObject.activeInHierarchy)
			{
				ChatMessageNew chatMessageNew = ModelManager.Instance.Get_Hall_Chat_VIP_DataX(0);
				string text = string.Empty;
				string str = string.Empty;
				if (chatMessageNew != null)
				{
					text = ModelManager.Instance.Get_Hall_Chat_VIP_DataX(0).Message;
					AgentBaseInfo client = chatMessageNew.Client;
					if (client != null)
					{
						str = client.NickName;
					}
				}
				if (!string.IsNullOrEmpty(text))
				{
					text = "#e062" + str + ": " + text;
					text = Tools_StringCheck.CheckEmotion(text, this.atlas);
					this.chatContent.text = text;
					if (null != this.emojiHitch && this.emojiHitch.transform.childCount > 0)
					{
						UnityEngine.Object.Destroy(this.emojiHitch.gameObject);
						GameObject gameObject = new GameObject("EmojiManager");
						this.emojiHitch = gameObject.transform;
						this.emojiHitch.parent = this.chatContent.transform.parent;
						this.emojiHitch.localPosition = Vector3.zero;
						this.emojiHitch.localScale = Vector3.one;
					}
					else if (null == this.emojiHitch)
					{
						GameObject gameObject2 = new GameObject("EmojiManager");
						this.emojiHitch = gameObject2.transform;
						this.emojiHitch.parent = this.chatContent.transform.parent;
						this.emojiHitch.localPosition = Vector3.zero;
						this.emojiHitch.localScale = Vector3.one;
					}
					Tools_StringCheck.GenerateEmotion(this.chatContent, this.atlas, this.emojiHitch);
				}
				this.DisplayVipMessage();
			}
			ModelManager.Instance.Set_RemoveVipChatView();
		}

		private void DisplayVipMessage()
		{
			Vector2 viewSize = this.vipScroll.GetViewSize();
			Vector2 vector = NGUIText.CalculatePrintedSize(this.chatContent.text);
			if (vector.x < viewSize.x)
			{
				this.tween_ChatPosition.from = this.originPosition + new Vector3(this.chatContent.transform.localPosition.x - 20f, 0f, 0f) + new Vector3(viewSize.x, 0f, this.originPosition.z);
				this.tween_ChatPosition.to = this.originPosition + new Vector3(this.chatContent.transform.localPosition.x - 50f, 0f, 0f) - new Vector3(vector.x, 0f, this.originPosition.z);
				this.tween_ChatPosition.duration = 5f;
			}
			else
			{
				this.tween_ChatPosition.from = this.originPosition + new Vector3(this.chatContent.transform.localPosition.x - 20f, 0f, 0f) + new Vector3(viewSize.x, 0f, this.originPosition.z);
				this.tween_ChatPosition.to = this.originPosition + new Vector3(this.chatContent.transform.localPosition.x - 50f, 0f, 0f) - new Vector3(vector.x, 0f, this.originPosition.z);
				this.tween_ChatPosition.duration = 5f;
			}
			this.tween_ChatPosition.ResetToBeginning();
			this.tween_ChatPosition.PlayForward();
		}

		private void NewsManagerInit()
		{
			this.ShowNews();
		}

		public void SetNews(int newType, string id)
		{
			if (!this._newsShowDic.ContainsKey(newType))
			{
				this._newsShowDic.Add(newType, new List<string>());
				this._newsShowDic[newType].Add(id);
				if (Singleton<MenuBottomBarView>.Instance.gameObject != null)
				{
					this.ChangeShow(newType, true);
				}
			}
			else
			{
				if (!this._newsShowDic[newType].Contains(id))
				{
					this._newsShowDic[newType].Add(id);
				}
				if (this._newsShowDic[newType].Count == 1 && Singleton<MenuBottomBarView>.Instance.gameObject != null)
				{
					this.ChangeShow(newType, true);
				}
			}
		}

		public void CheckHeroState()
		{
			List<string> allCanCallHeros = CharacterDataMgr.instance.AllCanCallHeros;
			if (allCanCallHeros != null && allCanCallHeros.Count > 0)
			{
				this.ChangeShow(3, true);
				return;
			}
			this.ChangeShow(3, false);
		}

		public void CheckTaskNewsState()
		{
			AchieveAll achieveAll = ModelManager.Instance.Get_AchieveAll_X();
			if (achieveAll == null)
			{
				return;
			}
			if (achieveAll.dailyTaskList.Find((DailyTaskData obj) => obj.isComplete && !obj.isGetAward) != null)
			{
				this._taskTip.gameObject.SetActive(true);
				this._taskTipEffect.transform.localPosition = this._taskTip.FindChild("Sprite").localPosition;
				this._taskTipEffect.SetActive(true, 0f);
				this._taskTip.GetComponent<UILabel>().text = LanguageManager.Instance.GetStringById("Daily_Tips_Rewards");
			}
			else if (achieveAll.dailyTaskList.Count == achieveAll.dailyTaskList.FindAll((DailyTaskData obj) => obj.isComplete && obj.isGetAward).Count)
			{
				this._taskTipEffect.SetActive(false, 0f);
				this._taskTip.gameObject.SetActive(false);
			}
			else
			{
				List<DailyTaskData> list = achieveAll.dailyTaskList.FindAll((DailyTaskData obj) => !obj.isComplete);
				list.Sort((DailyTaskData x, DailyTaskData y) => (x.taskid >= y.taskid) ? 0 : -1);
				for (int i = 0; i < list.Count; i++)
				{
					SysAchievementDailyVo dataById = BaseDataMgr.instance.GetDataById<SysAchievementDailyVo>(list[i].taskid.ToString());
					if (dataById == null)
					{
						UnityEngine.Debug.LogError("从Task表中读取id" + list[i].taskid.ToString() + "  结果为空");
					}
					else
					{
						if (dataById.tips != null && dataById.tips != "[]")
						{
							this._taskTip.gameObject.SetActive(true);
							this._taskTipEffect.transform.localPosition = this._taskTip.FindChild("Sprite").localPosition;
							this._taskTipEffect.SetActive(true, 0f);
							this._taskTip.GetComponent<UILabel>().text = LanguageManager.Instance.GetStringById(dataById.tips);
							break;
						}
						this._taskTipEffect.SetActive(false, 0f);
						this._taskTip.gameObject.SetActive(false);
					}
				}
			}
			for (int j = 0; j < achieveAll.dailyTaskList.Count; j++)
			{
				if (achieveAll.dailyTaskList[j].isComplete && !achieveAll.dailyTaskList[j].isGetAward)
				{
					this.SetNews(1, achieveAll.dailyTaskList[j].taskid.ToString());
				}
			}
			if (this._task.Find("Message").gameObject.activeSelf)
			{
				if (achieveAll.detailAchieveDataList.Find((DetailAchieveData obj) => obj.isComplete && !obj.isGetAward) == null)
				{
					this.ClearNews(2);
				}
			}
			for (int k = 0; k < achieveAll.detailAchieveDataList.Count; k++)
			{
				if (achieveAll.detailAchieveDataList[k].isComplete && !achieveAll.detailAchieveDataList[k].isGetAward)
				{
					this.SetNews(2, achieveAll.detailAchieveDataList[k].taskid.ToString());
					Singleton<TaskView>.Instance.ShowPoint(achieveAll.detailAchieveDataList[k].achieveid.ToString(), true);
				}
			}
		}

		public void CheckEmailNewsState()
		{
			if (ModelManager.Instance.Get_mailDataList_X() == null)
			{
				return;
			}
			List<MailData> list = ModelManager.Instance.Get_mailDataList_X();
			for (int i = 0; i < list.Count; i++)
			{
				if (!list[i].IsRead)
				{
					this.SetNews(10, list[i].Id.ToString());
				}
			}
		}

		public void CheckMailState()
		{
			if (!this._isInit)
			{
				this.RegisterUpdateHandler();
			}
			SendMsgManager.Instance.SendMsg(MobaGameCode.GetMailList, null, new object[0]);
		}

		public void CheckUnionState()
		{
			int num = ModelManager.Instance.Get_userData_filed_X("UnionId");
			if (num > 0)
			{
				SendMsgManager.SendMsgParam param = new SendMsgManager.SendMsgParam(true, "等待服务器响应...", true, 15f);
				SendMsgManager.Instance.SendMsg(MobaGameCode.GetUnionRequestList, param, new object[0]);
			}
		}

		private void OnGetUnionRequestList(MobaMessage msg)
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
			if (mobaErrorCode != MobaErrorCode.Ok)
			{
				this.OnGetUnionRequestList(num, operationResponse.DebugMessage, null);
			}
			else
			{
				byte[] buffer = operationResponse.Parameters[144] as byte[];
				List<UnionMemberData> unionMemberDataList = SerializeHelper.Deserialize<List<UnionMemberData>>(buffer);
				this.OnGetUnionRequestList(num, operationResponse.DebugMessage, unionMemberDataList);
			}
		}

		private void OnGetUnionRequestList(int arg1, string arg2, List<UnionMemberData> unionMemberDataList)
		{
			if (unionMemberDataList != null && unionMemberDataList.Count > 0)
			{
				this.SetNews(12, "0");
			}
		}

		public void HideNews(int newType)
		{
			if (this._newsShowDic.ContainsKey(newType) && this._newsShowDic[newType].Count == 0)
			{
				this._newsShowDic.Remove(newType);
				this.ChangeShow(newType, false);
			}
		}

		public void RemoveNews(int newType, string id)
		{
			if (this._newsShowDic.ContainsKey(newType))
			{
				if (this._newsShowDic[newType].Contains(id))
				{
					this._newsShowDic[newType].Remove(id);
					this.HideNews(newType);
				}
			}
			else
			{
				this.HideNews(newType);
			}
		}

		public void ClearNews(int newType)
		{
			this._newsShowDic[newType].Clear();
			this.HideNews(newType);
		}

		private void ShowNews()
		{
			foreach (int current in this._newsShowDic.Keys)
			{
				if (this._newsShowDic[current].Count > 0)
				{
					this.ChangeShow(current, true);
				}
			}
		}

		public void ChangeShow(int newType, bool isShow = true)
		{
			if (Singleton<MenuBottomBarView>.Instance.gameObject == null)
			{
				return;
			}
			switch (newType)
			{
			case 10:
				this._mail.Find("Message").gameObject.SetActive(isShow);
				return;
			case 11:
			case 13:
				IL_35:
				switch (newType)
				{
				case 0:
					return;
				case 1:
					this._eachDayTask.Find("Message").gameObject.SetActive(isShow);
					return;
				case 2:
					this._task.Find("Message").gameObject.SetActive(isShow);
					return;
				case 3:
					return;
				default:
					return;
				}
				break;
			case 12:
				return;
			case 14:
				this._friend.Find("Message").gameObject.SetActive(isShow);
				return;
			}
			goto IL_35;
		}

		public void SetFriendNum(int num)
		{
			if (this._friend == null && this.transform != null)
			{
				this._friend = this.transform.Find("BottomAnchor/Button/Friend");
			}
			if (this._friend == null)
			{
				return;
			}
			if (this.friendNum == null)
			{
				this.friendNum = this._friend.Find("Num").GetComponent<UILabel>();
			}
			if (this.friendNum == null)
			{
				ClientLogger.Error("logic error-----friendNum is null @linken");
				return;
			}
			this.oldFriendNum = num;
			if (num == 0)
			{
				this.friendNum.color = new Color(0.07450981f, 0.6627451f, 0.09019608f);
			}
			else
			{
				this.friendNum.color = new Color(0f, 0.917647064f, 0.08235294f);
			}
			this.friendNum.text = "(" + num + ")";
		}

		public void UpdateFriendNum()
		{
			if (this.transform == null)
			{
				return;
			}
			List<FriendData> list = ModelManager.Instance.Get_FriendDataList_X();
			if (list != null && list.Count > 0)
			{
				List<FriendData> list2 = list.FindAll((FriendData obj) => (int)obj.Status == 1 && (int)obj.GameStatus != 0);
				if (list2 != null && list2.Count > 0)
				{
					Singleton<MenuBottomBarView>.Instance.SetFriendNum(list2.Count);
				}
				else
				{
					Singleton<MenuBottomBarView>.Instance.SetFriendNum(0);
				}
			}
			else
			{
				Singleton<MenuBottomBarView>.Instance.SetFriendNum(0);
			}
		}
	}
}

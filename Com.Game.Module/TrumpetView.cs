using Assets.Scripts.GUILogic.View.HomeChatView;
using Assets.Scripts.Model;
using Com.Game.Utils;
using GUIFramework;
using MobaProtocol;
using MobaProtocol.Data;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Com.Game.Module
{
	public class TrumpetView : BaseView<TrumpetView>
	{
		private Transform closeBtn;

		private Transform confirmSend;

		private Transform emojiList;

		private Transform trumpet;

		private UIInput content;

		private UISprite emojiBtn;

		private UILabel numLimit;

		private UILabel countLabel;

		private UIGrid emoGrid;

		private EmojiItem emojiItem;

		public TrumpetView()
		{
			this.WinResCfg = new WinResurceCfg(false, false, "Prefab/UI/HomeChat/TrumpetView");
		}

		public override void Init()
		{
			base.Init();
			this.closeBtn = this.transform.Find("BackBtn");
			this.confirmSend = this.transform.Find("ConfirmSend");
			this.emojiList = this.transform.Find("EmojiLst");
			this.trumpet = this.transform.Find("Trumpets");
			this.emoGrid = this.emojiList.Find("Scroll View/Grid").GetComponent<UIGrid>();
			this.content = this.transform.Find("InputField").GetComponent<UIInput>();
			this.emojiBtn = this.transform.Find("Emoji").GetComponent<UISprite>();
			this.numLimit = this.transform.Find("LimitNum").GetComponent<UILabel>();
			this.countLabel = this.trumpet.Find("Count").GetComponent<UILabel>();
			this.emojiItem = Resources.Load<EmojiItem>("Prefab/UI/HomeChat/emojiItem");
			UIEventListener.Get(this.closeBtn.gameObject).onClick = new UIEventListener.VoidDelegate(this.CloseView);
			UIEventListener.Get(this.confirmSend.gameObject).onClick = new UIEventListener.VoidDelegate(this.ConfirmSendMessage);
			UIEventListener.Get(this.emojiBtn.gameObject).onClick = new UIEventListener.VoidDelegate(this.ShowEmotionPanel);
			this.content.defaultText = "点此输入文字";
		}

		public override void RegisterUpdateHandler()
		{
		}

		public override void CancelUpdateHandler()
		{
		}

		public override void HandleAfterOpenView()
		{
			HomeChatCtrl.GetInstance().sendState = SendState.Nothing;
			UserData userData = ModelManager.Instance.Get_userData_X();
			if (userData != null)
			{
				this.countLabel.text = "x" + userData.Speaker;
			}
		}

		public override void HandleBeforeCloseView()
		{
		}

		private void CloseView(GameObject obj)
		{
			if (null != obj)
			{
				CtrlManager.CloseWindow(WindowID.TrumpetView);
			}
		}

		public void ConfirmSendMessage(GameObject obj)
		{
			if (null != obj)
			{
				if (HomeChatCtrl.GetInstance().sendState == SendState.Sending)
				{
					return;
				}
				UserData userData = ModelManager.Instance.Get_userData_X();
				if (userData.Speaker <= 0)
				{
					CtrlManager.OpenWindow(WindowID.PurchasePopupView, null);
					Singleton<PurchasePopupView>.Instance.onSuccess.Add(new Callback(this.AfterBuying));
					Singleton<PurchasePopupView>.Instance.Show(GoodsSubject.Currency, "11", 1, false);
					return;
				}
				if (this.content.value.Length > 28)
				{
					Singleton<TipView>.Instance.ShowViewSetText("最多发送28个字！！！", 1f);
					return;
				}
				HomeChatCtrl.GetInstance().sendState = SendState.Sending;
				this.GetSetEmojiPanelState(false);
				string nickName = ModelManager.Instance.Get_userData_X().NickName;
				long userId = long.Parse(ModelManager.Instance.Get_userData_X().UserId);
				int userLevel = CharacterDataMgr.instance.GetUserLevel(ModelManager.Instance.Get_userData_X().Exp);
				int ladder = ToolsFacade.Instance.ToInt32(ModelManager.Instance.Get_userData_X().LadderScore);
				int botLevel = ModelManager.Instance.Get_BottleData_Level();
				int head = ModelManager.Instance.Get_userData_filed_X("Avatar");
				int headFrame = ModelManager.Instance.Get_userData_filed_X("PictureFrame");
				int charmRankvalue = ModelManager.Instance.Get_userData_filed_X("CharmRankValue");
				string value = this.content.value;
				AgentBaseInfo client = new AgentBaseInfo
				{
					NickName = nickName,
					UserId = userId,
					head = head,
					headFrame = headFrame,
					Level = userLevel,
					Ladder = ladder,
					BotLevel = botLevel,
					CharmRankvalue = charmRankvalue
				};
				if (!ToolsFacade.Instance.IsLegalString(ref value))
				{
					Debug.LogError("return");
					HomeChatCtrl.GetInstance().sendState = SendState.Nothing;
					return;
				}
				ChatMessageNew data = new ChatMessageNew
				{
					Client = client,
					ChatType = 8,
					Message = value,
					TargetId = null,
					TimeTick = ToolsFacade.ServerCurrentTime.Ticks
				};
				Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
				dictionary.Add(100, SerializeHelper.Serialize<ChatMessageNew>(data));
				SendMsgManager.SendMsgParam sendMsgParam = new SendMsgManager.SendMsgParam(true, "正在发送...", false, 15f);
				NetWorkHelper.Instance.client.SendSessionChannelMessage(2, MobaChannel.Chat, dictionary);
				this.content.defaultText = "点此输入文字";
			}
		}

		private void AfterBuying()
		{
			HomeChatCtrl.GetInstance().sendState = SendState.Nothing;
			UserData userData = ModelManager.Instance.Get_userData_X();
			if (userData != null)
			{
				this.countLabel.text = "x" + userData.Speaker;
			}
		}

		private void ShowEmotionPanel(GameObject obj)
		{
			if (null != obj)
			{
				if (this.emojiList.gameObject.activeInHierarchy)
				{
					this.GetSetEmojiPanelState(false);
				}
				else
				{
					this.GetSetEmojiPanelState(true);
					if (this.emoGrid.transform.childCount > 0)
					{
						this.emoGrid.Reposition();
						return;
					}
					BetterList<string> emojiNameList = this.emojiItem.Sprite.atlas.GetListOfSprites();
					GridHelper.FillGrid<EmojiItem>(this.emoGrid, this.emojiItem, emojiNameList.size - 1, delegate(int idx, EmojiItem comp)
					{
						comp.Init(emojiNameList[idx]);
						comp.ClickCallBack = new Callback<GameObject, EmojiItem>(this.FillEmojiToField);
					});
					this.emoGrid.Reposition();
				}
			}
		}

		private void GetSetEmojiPanelState(bool isOn)
		{
			if (null == this.emojiBtn || null == this.emojiList)
			{
				return;
			}
			this.emojiBtn.spriteName = ((!isOn) ? "Home_chatting_icons_emotion" : "Home_chatting_icons_emotion_02");
			this.emojiList.gameObject.SetActive(isOn);
			this.emojiList.gameObject.SetActive(isOn);
		}

		private void FillEmojiToField(GameObject obj, EmojiItem ei)
		{
			if (null != obj)
			{
				UIInput expr_12 = this.content;
				expr_12.value += ei.EmojiName;
				this.GetSetEmojiPanelState(false);
			}
		}

		private bool AntiIllegalStr(string str)
		{
			bool result = true;
			if (string.IsNullOrEmpty(str))
			{
				result = false;
			}
			byte[] bytes = Encoding.Default.GetBytes(str);
			string @string = Encoding.UTF8.GetString(bytes);
			if (string.IsNullOrEmpty(@string))
			{
				result = false;
				Singleton<TipView>.Instance.ShowViewSetText("含有非法字符！！！", 1f);
			}
			string value = str.Trim();
			if (string.IsNullOrEmpty(value))
			{
				result = false;
				Singleton<TipView>.Instance.ShowViewSetText("不能发送全为空格的信息！！！", 1f);
			}
			return result;
		}

		public void ClearContent()
		{
			this.content.value = string.Empty;
		}
	}
}

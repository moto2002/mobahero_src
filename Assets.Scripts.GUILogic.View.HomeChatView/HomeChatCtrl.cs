using MobaProtocol;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.GUILogic.View.HomeChatView
{
	public class HomeChatCtrl
	{
		private static HomeChatCtrl instance;

		private static object obj_lock = new object();

		public SendState state;

		private object[] mgs;

		public SendState sendState
		{
			get
			{
				return this.state;
			}
			set
			{
				this.state = value;
			}
		}

		private HomeChatCtrl()
		{
		}

		public static HomeChatCtrl GetInstance()
		{
			if (HomeChatCtrl.instance == null)
			{
				object obj = HomeChatCtrl.obj_lock;
				lock (obj)
				{
					if (HomeChatCtrl.instance == null)
					{
						HomeChatCtrl.instance = new HomeChatCtrl();
						return HomeChatCtrl.instance;
					}
				}
			}
			return HomeChatCtrl.instance;
		}

		public void Init()
		{
			this.mgs = new object[]
			{
				ClientV2C.chatviewOpenView,
				ClientV2C.chatviewSendChatToServer
			};
			HomeChatCtrl.GetInstance().sendState = SendState.Nothing;
			this.Register();
		}

		public void UnInit()
		{
			this.UnRegister();
			HomeChatCtrl.GetInstance().sendState = SendState.Nothing;
		}

		private void Register()
		{
			MobaMessageManagerTools.RegistMsg(this, this.mgs, true, "OnMsg_");
		}

		private void UnRegister()
		{
			MobaMessageManagerTools.RegistMsg(this, this.mgs, false, "OnMsg_");
		}

		private void OnMsg_chatviewOpenView(MobaMessage msg)
		{
			if (msg.Param != null)
			{
				ChitchatType chitchatType = (ChitchatType)((int)msg.Param);
				MobaMessageManagerTools.SendClientMsg(ClientV2C.chatviewInitRoom, chitchatType, false);
			}
		}

		private void OnMsg_chatviewSendChatToServer(MobaMessage msg)
		{
			if (msg.Param != null)
			{
				Dictionary<byte, object> args = new Dictionary<byte, object>();
				args = (Dictionary<byte, object>)msg.Param;
				NetWorkHelper.Instance.client.SendSessionChannelMessage(2, MobaChannel.Chat, args);
			}
		}
	}
}

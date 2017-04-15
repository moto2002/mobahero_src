using ExitGames.Client.Photon;
using MobaClient;
using MobaProtocol;
using MobaProtocol.Data;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.Model
{
	internal class Model_chatMessage : ModelBase<ChatMessageData>
	{
		public Model_chatMessage()
		{
			base.Init(EModelType.Model_chatMessage);
			base.Data = new ChatMessageData();
		}

		public override void RegisterMsgHandler()
		{
			MVC_MessageManager.AddListener_model(MobaGameCode.GetFriendMessages, new MobaMessageFunc(this.OnGetMsg));
		}

		public override void UnRegisterMsgHandler()
		{
			MVC_MessageManager.RemoveListener_model(MobaGameCode.GetFriendMessages, new MobaMessageFunc(this.OnGetMsg));
		}

		protected override void OnGetMsg(MobaMessage msg)
		{
		}

		private void ReviceMessageResponse(OperationResponse operationResponse)
		{
			Log.debug("==> MobaChatClientPeer : LoginUserResponse " + operationResponse.ReturnCode);
			int num = (int)operationResponse.Parameters[1];
			ChatMessageData chatMessageData = base.Data as ChatMessageData;
			MobaErrorCode mobaErrorCode = (MobaErrorCode)num;
			if (mobaErrorCode == MobaErrorCode.Ok)
			{
				Log.debug(" MobaClient : 获取聊天信息成功!!");
				byte[] buffer = operationResponse.Parameters[174] as byte[];
				chatMessageData.chatMessage = SerializeHelper.Deserialize<ChatMessage>(buffer);
				if (chatMessageData.chatMessage != null)
				{
					if (chatMessageData.CahtMessageList == null)
					{
						chatMessageData.CahtMessageList = new List<ChatMessage>();
					}
					if (chatMessageData.CahtMessageList.Count > 30)
					{
						chatMessageData.CahtMessageList.RemoveRange(30, chatMessageData.CahtMessageList.Count - 30);
					}
					chatMessageData.CahtMessageList.Add(chatMessageData.chatMessage);
					base.Data = chatMessageData;
				}
			}
		}
	}
}

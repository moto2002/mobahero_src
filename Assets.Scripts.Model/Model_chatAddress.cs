using MobaProtocol;
using System;

namespace Assets.Scripts.Model
{
	internal class Model_chatAddress : ModelBase<string>
	{
		public Model_chatAddress()
		{
			base.Init(EModelType.Model_chatAddress);
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
			base.DebugMessage = ((base.LastError != 0) ? "===>聊天服务器IP地址获取失败" : "===>聊天服务器IP地址获取成功");
			base.TriggerListners();
		}
	}
}

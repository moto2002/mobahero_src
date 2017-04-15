using MobaProtocol.Data;
using System;

namespace Assets.Scripts.Model
{
	internal class Model_chatClientInfo : ModelBase<ChatClientInfo>
	{
		public Model_chatClientInfo()
		{
			base.Init(EModelType.Model_chatClientInfo);
		}

		public override void RegisterMsgHandler()
		{
		}

		public override void UnRegisterMsgHandler()
		{
		}

		protected override void OnGetMsg(MobaMessage msg)
		{
		}

		public ChatClientInfo ToChatClientInfo(object serObj)
		{
			return null;
		}
	}
}

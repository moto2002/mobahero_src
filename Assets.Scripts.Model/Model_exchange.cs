using MobaProtocol;
using MobaProtocol.Data;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.Model
{
	internal class Model_exchange : ModelBase<List<ExchangeData>>
	{
		public Model_exchange()
		{
			base.Init(EModelType.Model_exchange);
		}

		public override void RegisterMsgHandler()
		{
			MVC_MessageManager.AddListener_model(MobaGameCode.ExchangeByDimond, new MobaMessageFunc(this.OnGetMsg));
		}

		public override void UnRegisterMsgHandler()
		{
			MVC_MessageManager.RemoveListener_model(MobaGameCode.ExchangeByDimond, new MobaMessageFunc(this.OnGetMsg));
		}

		protected override void OnGetMsg(MobaMessage msg)
		{
			base.Deserialize(136, msg);
			base.DebugMessage = ((base.LastError != 0) ? "===>金币体力兑换失败" : "===>金币体力兑换成功");
			base.TriggerListners();
		}
	}
}

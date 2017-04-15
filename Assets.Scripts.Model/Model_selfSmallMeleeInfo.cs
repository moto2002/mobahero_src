using MobaProtocol;
using MobaProtocol.Data;
using System;

namespace Assets.Scripts.Model
{
	internal class Model_selfSmallMeleeInfo : ModelBase<SmallMeleeData>
	{
		public Model_selfSmallMeleeInfo()
		{
			base.Init(EModelType.Model_selfSmallMeleeInfo);
		}

		public override void RegisterMsgHandler()
		{
			MVC_MessageManager.AddListener_model(MobaGameCode.GetSmallMeleeInfo, new MobaMessageFunc(this.OnGetMsg));
		}

		public override void UnRegisterMsgHandler()
		{
			MVC_MessageManager.RemoveListener_model(MobaGameCode.GetSmallMeleeInfo, new MobaMessageFunc(this.OnGetMsg));
		}

		protected override void OnGetMsg(MobaMessage msg)
		{
			base.Deserialize(162, msg);
			base.DebugMessage = ((base.LastError != 0) ? "===>任务小乱斗敌方数据失败" : "===>任务小乱斗敌方数据成功");
			base.TriggerListners();
		}
	}
}

using MobaProtocol;
using MobaProtocol.Data;
using System;

namespace Assets.Scripts.Model
{
	internal class Model_barrage : ModelBase<BarrageModelData>
	{
		private BarrageModelData data;

		public Model_barrage()
		{
			base.Init(EModelType.Model_barrage);
			this.data = new BarrageModelData();
			base.Data = this.data;
		}

		public override void RegisterMsgHandler()
		{
			MobaMessageManager.RegistMessage(PvpCode.C2P_Caption, new MobaMessageFunc(this.OnGetMsg));
		}

		public override void UnRegisterMsgHandler()
		{
			MobaMessageManager.UnRegistMessage(PvpCode.C2P_Caption, new MobaMessageFunc(this.OnGetMsg));
		}

		protected override void OnGetMsg(MobaMessage msg)
		{
			P2CCaption probufMsg = msg.GetProbufMsg<P2CCaption>();
			if (probufMsg != null)
			{
				string captionStr = probufMsg.captionStr;
				this.data.mQueue.Enqueue(captionStr);
			}
		}
	}
}

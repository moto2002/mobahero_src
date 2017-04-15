using ExitGames.Client.Photon;
using MobaProtocol;
using System;

namespace Assets.Scripts.Model
{
	internal class Model_Punishment : ModelBase<DateTime>
	{
		public Model_Punishment()
		{
			base.Init(EModelType.Model_Punishment);
			base.Data = default(DateTime);
		}

		public override void RegisterMsgHandler()
		{
			MobaMessageManager.RegistMessage(MobaGameCode.DealGameReport, new MobaMessageFunc(this.OnGetMsg));
		}

		public override void UnRegisterMsgHandler()
		{
			MobaMessageManager.UnRegistMessage(MobaGameCode.DealGameReport, new MobaMessageFunc(this.OnGetMsg));
		}

		protected override void OnGetMsg(MobaMessage msg)
		{
			if (msg != null)
			{
				OperationResponse operationResponse = msg.Param as OperationResponse;
				if (operationResponse != null)
				{
					if (operationResponse.Parameters.ContainsKey(84))
					{
						long ticks = (long)operationResponse.Parameters[84];
						base.Data = new DateTime(ticks);
					}
				}
			}
		}
	}
}

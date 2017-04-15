using ExitGames.Client.Photon;
using MobaProtocol;
using MobaProtocol.Data;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Assets.Scripts.Model
{
	internal class Model_DoubleExpGold : ModelBase<List<DoubleCardData>>
	{
		private List<MobaGameCode> listCode = new List<MobaGameCode>();

		public Model_DoubleExpGold()
		{
			base.Init(EModelType.Model_DoubleExpGold);
			base.Data = new List<DoubleCardData>();
		}

		public override void RegisterMsgHandler()
		{
			this.listCode = new List<MobaGameCode>
			{
				MobaGameCode.GetDoubleCard
			};
			foreach (MobaGameCode current in this.listCode)
			{
				MVC_MessageManager.AddListener_model(current, new MobaMessageFunc(this.OnGetMsg));
			}
		}

		public override void UnRegisterMsgHandler()
		{
			foreach (MobaGameCode current in this.listCode)
			{
				MVC_MessageManager.RemoveListener_model(current, new MobaMessageFunc(this.OnGetMsg));
			}
		}

		protected override void OnGetMsg(MobaMessage msg)
		{
			int num = 0;
			MobaMessageType mobaMessageType = MVC_MessageManager.ClientMsg_to_RawCode((ClientMsg)msg.ID, out num);
			if (this.listCode.Contains((MobaGameCode)num))
			{
				string name = "OnGetMsg_" + mobaMessageType.ToString() + "_" + ((MobaGameCode)num).ToString();
				MethodInfo method = base.GetType().GetMethod(name, BindingFlags.Instance | BindingFlags.NonPublic);
				if (method != null)
				{
					object[] parameters = new object[]
					{
						msg
					};
					method.Invoke(this, parameters);
					base.TriggerListners();
				}
			}
		}

		private void OnGetMsg_GameCode_GetDoubleCard(MobaMessage msg)
		{
			if (msg != null)
			{
				OperationResponse operationResponse = msg.Param as OperationResponse;
				if (operationResponse != null)
				{
					int num = (int)operationResponse.Parameters[1];
					MobaErrorCode mobaErrorCode = (MobaErrorCode)num;
					if (mobaErrorCode == MobaErrorCode.Ok)
					{
						List<DoubleCardData> list = SerializeHelper.Deserialize<List<DoubleCardData>>(operationResponse.Parameters[127] as byte[]);
						if (list != null)
						{
							base.Data = list;
						}
						MobaMessageManagerTools.SendClientMsg(ClientC2V.GetDoubleCard, null, false);
					}
				}
			}
		}
	}
}

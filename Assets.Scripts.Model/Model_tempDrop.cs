using ExitGames.Client.Photon;
using MobaProtocol;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.Model
{
	internal class Model_tempDrop : ModelBase<List<int>>
	{
		public Model_tempDrop()
		{
			base.Init(EModelType.Model_tempDrop);
		}

		public override void RegisterMsgHandler()
		{
			MVC_MessageManager.AddListener_model(MobaGameCode.GetEquipmentDrop, new MobaMessageFunc(this.OnGetMsg));
		}

		public override void UnRegisterMsgHandler()
		{
			MVC_MessageManager.RemoveListener_model(MobaGameCode.GetEquipmentDrop, new MobaMessageFunc(this.OnGetMsg));
		}

		protected override void OnGetMsg(MobaMessage msg)
		{
			base.LastError = 505;
			OperationResponse operationResponse = msg.Param as OperationResponse;
			int num = (int)operationResponse.Parameters[1];
			if (base.LastError == 0)
			{
				string text = operationResponse.Parameters[91] as string;
				string[] array = text.Split(new char[]
				{
					'|'
				});
				base.Data = new List<int>();
				string[] array2 = array;
				for (int i = 0; i < array2.Length; i++)
				{
					string text2 = array2[i];
					if (!string.IsNullOrEmpty(text2))
					{
						((List<int>)base.Data).Add(int.Parse(text2));
					}
				}
			}
			else
			{
				base.DebugMessage = "====>GetEquipmentDrop" + operationResponse.OperationCode;
			}
			base.Valid = (base.LastError == 0);
		}
	}
}

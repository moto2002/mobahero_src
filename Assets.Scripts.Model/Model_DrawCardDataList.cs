using ExitGames.Client.Photon;
using MobaProtocol;
using MobaProtocol.Data;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Assets.Scripts.Model
{
	internal class Model_DrawCardDataList : ModelBase<DrawCardModelData>
	{
		internal static int Exp_Max;

		private List<string> list_key = new List<string>();

		private Dictionary<MobaMessageType, List<int>> Dic_Bottle;

		private DrawCardModelData data;

		public Model_DrawCardDataList()
		{
			base.Init(EModelType.Model_DrawCardDataList);
			this.Dic_Bottle = new Dictionary<MobaMessageType, List<int>>();
			this.Dic_Bottle.Add(MobaMessageType.GameCode, new List<int>());
			this.data = new DrawCardModelData();
			base.Data = this.data;
		}

		public override void RegisterMsgHandler()
		{
			List<MobaGameCode> list = new List<MobaGameCode>
			{
				MobaGameCode.DrawMagicBottleAward
			};
			foreach (MobaGameCode current in list)
			{
				this.Dic_Bottle[MobaMessageType.GameCode].Add((int)current);
				MVC_MessageManager.AddListener_model(current, new MobaMessageFunc(this.OnGetMsg));
			}
		}

		public override void UnRegisterMsgHandler()
		{
			foreach (MobaMessageType current in this.Dic_Bottle.Keys)
			{
				List<int> list = this.Dic_Bottle[current];
				foreach (int current2 in list)
				{
					MobaMessageType mobaMessageType = current;
					if (mobaMessageType == MobaMessageType.GameCode)
					{
						MVC_MessageManager.RemoveListener_model((MobaGameCode)current2, new MobaMessageFunc(this.OnGetMsg));
					}
				}
			}
		}

		protected override void OnGetMsg(MobaMessage msg)
		{
			int num = 0;
			MobaMessageType mobaMessageType = MVC_MessageManager.ClientMsg_to_RawCode((ClientMsg)msg.ID, out num);
			if (this.Dic_Bottle.ContainsKey(mobaMessageType) && this.Dic_Bottle[mobaMessageType].Contains(num))
			{
				string text = "OnGetMsg_" + mobaMessageType.ToString() + "_";
				MobaMessageType mobaMessageType2 = mobaMessageType;
				if (mobaMessageType2 == MobaMessageType.GameCode)
				{
					text += ((MobaGameCode)num).ToString();
				}
				MethodInfo method = base.GetType().GetMethod(text, BindingFlags.Instance | BindingFlags.NonPublic);
				if (method != null)
				{
					object[] parameters = new object[]
					{
						msg
					};
					method.Invoke(this, parameters);
				}
			}
		}

		private bool PreHandel(MobaMessage msg, out OperationResponse res)
		{
			res = null;
			res = (msg.Param as OperationResponse);
			return res != null;
		}

		private void OnGetMsg_GameCode_DrawMagicBottleAward(MobaMessage msg)
		{
			base.LastError = 505;
			OperationResponse operationResponse;
			if (this.PreHandel(msg, out operationResponse))
			{
				base.LastError = (int)operationResponse.Parameters[1];
				if (base.LastError == 0)
				{
					List<DropItemData> list = new List<DropItemData>();
					List<EquipmentInfoData> list2 = new List<EquipmentInfoData>();
					List<HeroInfoData> list3 = new List<HeroInfoData>();
					if (operationResponse.Parameters.ContainsKey(240))
					{
						list = SerializeHelper.Deserialize<List<DropItemData>>(operationResponse.Parameters[240] as byte[]);
					}
					if (operationResponse.Parameters.ContainsKey(202))
					{
						list2 = SerializeHelper.Deserialize<List<EquipmentInfoData>>(operationResponse.Parameters[202] as byte[]);
					}
					if (operationResponse.Parameters.ContainsKey(88))
					{
						list3 = SerializeHelper.Deserialize<List<HeroInfoData>>(operationResponse.Parameters[88] as byte[]);
					}
					int num = (int)operationResponse.Parameters[241];
					if (list != null)
					{
						this.data.normalDrop = list;
					}
					if (list2 != null)
					{
						this.data.equipList = list2;
					}
					if (list3 != null)
					{
						this.data.heroinfoList = list3;
					}
					List<DropItemData> list4 = SerializeHelper.Deserialize<List<DropItemData>>(operationResponse.Parameters[146] as byte[]);
					if (list4 != null && list4.Count != 0)
					{
						this.data.repeatDrop = list4;
					}
					MobaMessage message = MobaMessageManager.GetMessage((ClientMsg)23051, new List<object>
					{
						num,
						list
					}, 0f);
					MobaMessageManager.ExecuteMsg(message);
				}
			}
		}
	}
}

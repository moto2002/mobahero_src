using Com.Game.Module;
using ExitGames.Client.Photon;
using MobaHeros.Pvp;
using MobaProtocol;
using MobaProtocol.Data;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.Model
{
	internal class Model_settlement : ModelBase<SettlementModelData>
	{
		private SettlementModelData data;

		public Model_settlement()
		{
			base.Init(EModelType.Model_settlement);
			this.data = new SettlementModelData();
			base.Data = this.data;
		}

		public override void RegisterMsgHandler()
		{
			MobaMessageManager.RegistMessage(MobaGameCode.GetPvpFightResult, new MobaMessageFunc(this.OnGetMsg));
			MobaMessageManager.RegistMessage(MobaGameCode.UploadFightResult, new MobaMessageFunc(this.OnGetMsg));
		}

		public override void UnRegisterMsgHandler()
		{
			MobaMessageManager.UnRegistMessage(MobaGameCode.GetPvpFightResult, new MobaMessageFunc(this.OnGetMsg));
			MobaMessageManager.UnRegistMessage(MobaGameCode.UploadFightResult, new MobaMessageFunc(this.OnGetMsg));
		}

		protected override void OnGetMsg(MobaMessage msg)
		{
			base.DebugMessage = ((base.LastError != 0) ? "===>战斗结算数据获取失败" : "===>战斗结算数据获取成功");
			if (msg != null)
			{
				OperationResponse operationResponse = msg.Param as OperationResponse;
				if (operationResponse != null)
				{
					if (operationResponse.OperationCode == 208)
					{
						this.OnGetMsg_Pve(operationResponse);
					}
					else if (operationResponse.OperationCode == 94)
					{
						this.OnGetMsg_Pvp(operationResponse);
					}
				}
			}
		}

		private void OnGetMsg_Pve(OperationResponse operationResponse)
		{
			if (operationResponse.Parameters.ContainsKey(202))
			{
				byte[] buffer = operationResponse.Parameters[202] as byte[];
				this.data.equipRtnData = SerializeHelper.Deserialize<List<EquipmentInfoData>>(buffer);
			}
			if (operationResponse.Parameters.ContainsKey(88))
			{
				byte[] buffer2 = operationResponse.Parameters[88] as byte[];
				this.data.heroRtnData = SerializeHelper.Deserialize<List<HeroInfoData>>(buffer2);
			}
			if (operationResponse.Parameters.ContainsKey(246))
			{
				byte[] buffer3 = operationResponse.Parameters[246] as byte[];
				this.data.dropRtnData = SerializeHelper.Deserialize<List<DropItemData>>(buffer3);
			}
			if (operationResponse.Parameters.ContainsKey(113))
			{
				byte[] buffer4 = operationResponse.Parameters[113] as byte[];
				this.data.pve_battlesInfo = SerializeHelper.Deserialize<List<BattlesModel>>(buffer4);
			}
			ModelManager.Instance.Set_Settle_DeltaExp();
			if (!Singleton<PvpManager>.Instance.IsObserver)
			{
				ModelManager.Instance.Set_Settle_Proficiency();
			}
			this.data.isGetMsg = true;
		}

		private void OnGetMsg_Pvp(OperationResponse operationResponse)
		{
			if (operationResponse.Parameters.ContainsKey(202))
			{
				byte[] buffer = operationResponse.Parameters[202] as byte[];
				this.data.equipRtnData = SerializeHelper.Deserialize<List<EquipmentInfoData>>(buffer);
			}
			if (operationResponse.Parameters.ContainsKey(88))
			{
				byte[] buffer2 = operationResponse.Parameters[88] as byte[];
				this.data.heroRtnData = SerializeHelper.Deserialize<List<HeroInfoData>>(buffer2);
			}
			if (operationResponse.Parameters.ContainsKey(246))
			{
				byte[] buffer3 = operationResponse.Parameters[246] as byte[];
				this.data.dropRtnData = SerializeHelper.Deserialize<List<DropItemData>>(buffer3);
			}
			if (operationResponse.Parameters.ContainsKey(220))
			{
				byte[] buffer4 = operationResponse.Parameters[220] as byte[];
				this.data.pvp_teamInfo = SerializeHelper.Deserialize<PvpTeamInfo>(buffer4);
			}
			if (operationResponse.Parameters.ContainsKey(146))
			{
				byte[] buffer5 = operationResponse.Parameters[146] as byte[];
				this.data.repeatRtnData = SerializeHelper.Deserialize<List<DropItemData>>(buffer5);
			}
			ModelManager.Instance.Set_Settle_DeltaExp();
			if (!Singleton<PvpManager>.Instance.IsObserver)
			{
				ModelManager.Instance.Set_Settle_Proficiency();
			}
			this.data.isGetMsg = true;
		}
	}
}

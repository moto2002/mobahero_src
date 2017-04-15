using ExitGames.Client.Photon;
using MobaProtocol;
using MobaProtocol.Data;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.Model
{
	internal class Model_battleList : ModelBase<List<BattlesModel>>
	{
		public Model_battleList()
		{
			base.Init(EModelType.Model_battleList);
		}

		public override void RegisterMsgHandler()
		{
			MVC_MessageManager.AddListener_model(MobaGameCode.GetBattles, new MobaMessageFunc(this.OnGetMsg));
			MVC_MessageManager.AddListener_model(MobaGameCode.SweepBattle, new MobaMessageFunc(this.OnGetMsg));
			MVC_MessageManager.AddListener_model(MobaGameCode.RestTodayBattlesCount, new MobaMessageFunc(this.OnGetMsg));
			MVC_MessageManager.AddListener_model(MobaGameCode.UploadFightResult, new MobaMessageFunc(this.OnGetMsg));
		}

		public override void UnRegisterMsgHandler()
		{
			MVC_MessageManager.RemoveListener_model(MobaGameCode.GetBattles, new MobaMessageFunc(this.OnGetMsg));
			MVC_MessageManager.RemoveListener_model(MobaGameCode.SweepBattle, new MobaMessageFunc(this.OnGetMsg));
			MVC_MessageManager.RemoveListener_model(MobaGameCode.RestTodayBattlesCount, new MobaMessageFunc(this.OnGetMsg));
			MVC_MessageManager.RemoveListener_model(MobaGameCode.UploadFightResult, new MobaMessageFunc(this.OnGetMsg));
		}

		protected override void OnGetMsg(MobaMessage msg)
		{
			base.LastError = 505;
			OperationResponse operationResponse = msg.Param as OperationResponse;
			if (operationResponse != null)
			{
				MobaGameCode mobaGameCode = MVC_MessageManager.NotifyModel_to_Game((ClientMsg)msg.ID);
				MobaGameCode mobaGameCode2 = mobaGameCode;
				if (mobaGameCode2 != MobaGameCode.SweepBattle)
				{
					if (mobaGameCode2 != MobaGameCode.RestTodayBattlesCount)
					{
						if (mobaGameCode2 != MobaGameCode.UploadFightResult)
						{
							if (mobaGameCode2 == MobaGameCode.GetBattles)
							{
								this.OnGetMsg_GetBattles(operationResponse);
							}
						}
						else
						{
							this.OnGetMsg_UploadFightResult(operationResponse);
						}
					}
					else
					{
						this.OnGetMsg_RestTodayBattlesCount(operationResponse);
					}
				}
				else
				{
					this.OnGetMsg_SweepBattle(operationResponse);
				}
			}
			base.TriggerListners();
		}

		private void OnGetMsg_GetBattles(OperationResponse res)
		{
			base.LastError = (int)res.Parameters[1];
			if (base.LastError == 0)
			{
				byte[] buffer = res.Parameters[113] as byte[];
				List<BattlesModel> collection = SerializeHelper.Deserialize<List<BattlesModel>>(buffer);
				base.Data = new List<BattlesModel>(collection);
				base.DebugMessage = "====>OK " + res.OperationCode;
			}
			else
			{
				base.DebugMessage = "====>GetBattles" + res.OperationCode;
			}
			base.Valid = (base.LastError == 0);
		}

		private void OnGetMsg_SweepBattle(OperationResponse res)
		{
			base.LastError = (int)res.Parameters[1];
			if (base.LastError == 0)
			{
				int num = (int)res.Parameters[101];
				string battleId = res.Parameters[157] as string;
				string sceneId = res.Parameters[94] as string;
				BattlesModel battlesModel = ((List<BattlesModel>)base.Data).Find((BattlesModel obj) => obj.BattleId == int.Parse(battleId));
				BattleSceneModel battleSceneModel = battlesModel.List.Find((BattleSceneModel obj) => obj.SceneId == (long)int.Parse(sceneId));
				if (battleSceneModel != null)
				{
					battleSceneModel.DayCount += num;
				}
				base.DebugMessage = "====>OK " + res.OperationCode;
			}
			else
			{
				base.DebugMessage = "====>SweepBattle" + res.OperationCode;
			}
			base.Valid = (base.LastError == 0);
		}

		private void OnGetMsg_RestTodayBattlesCount(OperationResponse res)
		{
			base.LastError = (int)res.Parameters[1];
			if (base.LastError == 0)
			{
				List<BattlesModel> list = base.Data as List<BattlesModel>;
				string sceneId = res.Parameters[94] as string;
				string battleId = res.Parameters[157] as string;
				BattlesModel battlesModel = list.Find((BattlesModel obj) => obj.BattleId == int.Parse(battleId));
				BattleSceneModel battleSceneModel = battlesModel.List.Find((BattleSceneModel obj) => obj.SceneId == (long)int.Parse(sceneId));
				if (battleSceneModel != null)
				{
					battleSceneModel.DayCount = 0;
					battleSceneModel.DayRestCount++;
				}
				base.DebugMessage = "====>OK " + res.OperationCode;
			}
			else
			{
				base.DebugMessage = "====>RestTodayBattlesCount" + res.OperationCode;
			}
			base.Valid = (base.LastError == 0);
		}

		private void OnGetMsg_UploadFightResult(OperationResponse res)
		{
			base.LastError = (int)res.Parameters[1];
			if (base.LastError == 0)
			{
				byte[] buffer = res.Parameters[113] as byte[];
				List<BattlesModel> collection = SerializeHelper.Deserialize<List<BattlesModel>>(buffer);
				((List<BattlesModel>)base.Data).Clear();
				base.Data = new List<BattlesModel>(collection);
				base.DebugMessage = "====>OK " + res.OperationCode;
			}
			else
			{
				base.DebugMessage = "====>UploadFightResult" + res.OperationCode;
			}
			base.Valid = (base.LastError == 0);
		}
	}
}

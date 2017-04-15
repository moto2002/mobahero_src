using ExitGames.Client.Photon;
using MobaClient;
using MobaProtocol;
using MobaProtocol.Data;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.Model
{
	internal class Model_getMyAchievementData : ModelBase<MyAchievementInfo>
	{
		public Model_getMyAchievementData()
		{
			base.Init(EModelType.Model_getMyAchievementData);
			base.Data = new MyAchievementInfo();
		}

		public override void RegisterMsgHandler()
		{
			MVC_MessageManager.AddListener_model(MobaGameCode.GetTotalRecord, new MobaMessageFunc(this.OnGetMsg));
			MVC_MessageManager.AddListener_model(MobaGameCode.GetHistoryRecord, new MobaMessageFunc(this.OnGetMsg));
			MVC_MessageManager.AddListener_model(MobaGameCode.GetKdaMyHeroData, new MobaMessageFunc(this.OnGetMsg));
			MVC_MessageManager.AddListener_model(MobaGameCode.GetMyFightAbility, new MobaMessageFunc(this.OnGetMsg));
			MVC_MessageManager.AddListener_model(MobaGameCode.GetUserHonorPic, new MobaMessageFunc(this.OnGetMsg));
			MVC_MessageManager.AddListener_model(MobaGameCode.GetHomeTotalRecord, new MobaMessageFunc(this.OnGetMsg));
			MVC_MessageManager.AddListener_model(MobaGameCode.GetHeroRecordInfo, new MobaMessageFunc(this.OnGetMsg));
		}

		public override void UnRegisterMsgHandler()
		{
			MVC_MessageManager.RemoveListener_model(MobaGameCode.GetTotalRecord, new MobaMessageFunc(this.OnGetMsg));
			MVC_MessageManager.RemoveListener_model(MobaGameCode.GetHistoryRecord, new MobaMessageFunc(this.OnGetMsg));
			MVC_MessageManager.RemoveListener_model(MobaGameCode.GetKdaMyHeroData, new MobaMessageFunc(this.OnGetMsg));
			MVC_MessageManager.RemoveListener_model(MobaGameCode.GetMyFightAbility, new MobaMessageFunc(this.OnGetMsg));
			MVC_MessageManager.RemoveListener_model(MobaGameCode.GetUserHonorPic, new MobaMessageFunc(this.OnGetMsg));
			MVC_MessageManager.RemoveListener_model(MobaGameCode.GetHomeTotalRecord, new MobaMessageFunc(this.OnGetMsg));
			MVC_MessageManager.RemoveListener_model(MobaGameCode.GetHeroRecordInfo, new MobaMessageFunc(this.OnGetMsg));
		}

		protected override void OnGetMsg(MobaMessage msg)
		{
			base.DebugMessage = ((base.LastError != 0) ? "===>成就系统信息获取失败" : "===>成就系统信息获取成功");
			if (msg != null)
			{
				OperationResponse operationResponse = msg.Param as OperationResponse;
				if (operationResponse != null)
				{
					MobaGameCode mobaGameCode = MVC_MessageManager.NotifyModel_to_Game((ClientMsg)msg.ID);
					base.LastMsgType = (int)mobaGameCode;
					MobaGameCode mobaGameCode2 = mobaGameCode;
					switch (mobaGameCode2)
					{
					case MobaGameCode.GetTotalRecord:
						this.GetKdaData(operationResponse);
						break;
					case MobaGameCode.GetHistoryRecord:
						this.GetHistoryData(operationResponse);
						break;
					case MobaGameCode.GetHomeTotalRecord:
						this.GetHomeKDAData(operationResponse);
						break;
					case MobaGameCode.GetHeroRecordInfo:
						this.GetOnePageBattleData(operationResponse);
						break;
					default:
						if (mobaGameCode2 != MobaGameCode.GetKdaMyHeroData)
						{
							if (mobaGameCode2 != MobaGameCode.GetMyFightAbility)
							{
								if (mobaGameCode2 == MobaGameCode.GetUserHonorPic)
								{
									this.GetMyHonorData(operationResponse);
								}
							}
							else
							{
								this.GetMyAbilityData(operationResponse);
							}
						}
						else
						{
							this.GetAchievementData(operationResponse);
						}
						break;
					}
				}
			}
			base.TriggerListners();
		}

		private void GetKdaData(OperationResponse operationResponse)
		{
			Log.debug(" MobaClient : 响应更新 " + operationResponse.ReturnCode);
			int num = (int)operationResponse.Parameters[1];
			MyAchievementInfo myAchievementInfo = base.Data as MyAchievementInfo;
			base.Valid = true;
			MobaErrorCode mobaErrorCode = (MobaErrorCode)num;
			if (mobaErrorCode != MobaErrorCode.Ok)
			{
				if (mobaErrorCode != MobaErrorCode.ItemExist)
				{
				}
			}
			else
			{
				byte[] buffer = operationResponse.Parameters[248] as byte[];
				if (operationResponse.Parameters.ContainsKey(181))
				{
					myAchievementInfo.totalpvpLogId = (long)Convert.ToInt32(operationResponse.Parameters[181]);
				}
				else
				{
					myAchievementInfo.totalpvpLogId = 0L;
				}
				myAchievementInfo.kdaData = SerializeHelper.Deserialize<KDAData>(buffer);
				base.Data = myAchievementInfo;
			}
		}

		private void GetHistoryData(OperationResponse operationResponse)
		{
			Log.debug(" MobaClient : 响应更新 " + operationResponse.ReturnCode);
			int num = (int)operationResponse.Parameters[1];
			MyAchievementInfo myAchievementInfo = base.Data as MyAchievementInfo;
			base.Valid = true;
			MobaErrorCode mobaErrorCode = (MobaErrorCode)num;
			if (mobaErrorCode == MobaErrorCode.Ok)
			{
				object obj = null;
				if (!operationResponse.Parameters.TryGetValue(249, out obj))
				{
					myAchievementInfo.historyBattle = new HistoryBattleData();
					return;
				}
				byte[] buffer = obj as byte[];
				myAchievementInfo.historyBattle = SerializeHelper.Deserialize<HistoryBattleData>(buffer);
				if (operationResponse.Parameters.ContainsKey(181))
				{
					myAchievementInfo.battleLogId = (long)Convert.ToInt32(operationResponse.Parameters[181]);
				}
				else
				{
					myAchievementInfo.battleLogId = 0L;
				}
				if (!myAchievementInfo.HistoryBattleDataDic.ContainsKey(myAchievementInfo.battleLogId))
				{
					myAchievementInfo.HistoryBattleDataDic.Add(myAchievementInfo.battleLogId, myAchievementInfo.historyBattle);
				}
				base.Data = myAchievementInfo;
			}
		}

		private void GetAchievementData(OperationResponse operationResponse)
		{
			Log.debug(" MobaClient : 响应更新 " + operationResponse.ReturnCode);
			int num = (int)operationResponse.Parameters[1];
			MyAchievementInfo myAchievementInfo = base.Data as MyAchievementInfo;
			base.Valid = true;
			MobaErrorCode mobaErrorCode = (MobaErrorCode)num;
			if (mobaErrorCode == MobaErrorCode.Ok)
			{
				byte[] buffer = operationResponse.Parameters[250] as byte[];
				if (operationResponse.Parameters.ContainsKey(181))
				{
					myAchievementInfo.heropvpLogId = (long)Convert.ToInt32(operationResponse.Parameters[181]);
				}
				else
				{
					myAchievementInfo.heropvpLogId = 0L;
				}
				myAchievementInfo.myHero = SerializeHelper.Deserialize<List<KdaMyHeroData>>(buffer);
				base.Data = myAchievementInfo;
			}
		}

		private void GetMyAbilityData(OperationResponse operationResponse)
		{
			Log.debug(" MobaClient : 响应更新 " + operationResponse.ReturnCode);
			int num = (int)operationResponse.Parameters[1];
			MyAchievementInfo myAchievementInfo = base.Data as MyAchievementInfo;
			base.Valid = true;
			MobaErrorCode mobaErrorCode = (MobaErrorCode)num;
			if (mobaErrorCode == MobaErrorCode.Ok)
			{
				byte[] buffer = operationResponse.Parameters[251] as byte[];
				byte[] buffer2 = operationResponse.Parameters[101] as byte[];
				myAchievementInfo.abilityGraph = SerializeHelper.Deserialize<KdaAbilityGraph>(buffer2);
				if (operationResponse.Parameters.ContainsKey(181))
				{
					myAchievementInfo.abilitypvpLogId = (long)Convert.ToInt32(operationResponse.Parameters[181]);
				}
				else
				{
					myAchievementInfo.abilitypvpLogId = 0L;
				}
				myAchievementInfo.myTopData = SerializeHelper.Deserialize<KdaUserTopData>(buffer);
				base.Data = myAchievementInfo;
			}
		}

		private void GetMyHonorData(OperationResponse operationResponse)
		{
		}

		private void GetHomeKDAData(OperationResponse operationResponse)
		{
			Log.debug(" MobaClient : 响应更新 " + operationResponse.ReturnCode);
			int num = (int)operationResponse.Parameters[1];
			MyAchievementInfo myAchievementInfo = base.Data as MyAchievementInfo;
			base.Valid = true;
			MobaErrorCode mobaErrorCode = (MobaErrorCode)num;
			if (mobaErrorCode == MobaErrorCode.Ok)
			{
				byte[] buffer = operationResponse.Parameters[248] as byte[];
				myAchievementInfo.myHomeKDA = SerializeHelper.Deserialize<HomeKDAData>(buffer);
				base.Data = myAchievementInfo;
			}
		}

		private void GetOnePageBattleData(OperationResponse operationResponse)
		{
			Log.debug(" MobaClient : 响应更新 " + operationResponse.ReturnCode);
			int num = (int)operationResponse.Parameters[1];
			MyAchievementInfo myAchievementInfo = base.Data as MyAchievementInfo;
			base.Valid = true;
			MobaErrorCode mobaErrorCode = (MobaErrorCode)num;
			if (mobaErrorCode == MobaErrorCode.Ok)
			{
				byte[] buffer = operationResponse.Parameters[248] as byte[];
				myAchievementInfo.myHeroRecordList = SerializeHelper.Deserialize<List<heroRecordInfo>>(buffer);
				myAchievementInfo.myHeroRecord.Clear();
				if (myAchievementInfo.myHeroRecordList != null && myAchievementInfo.myHeroRecordList.Count > 0)
				{
					foreach (heroRecordInfo current in myAchievementInfo.myHeroRecordList)
					{
						if (!myAchievementInfo.myHeroRecord.ContainsKey(current.pvpLogId))
						{
							myAchievementInfo.myHeroRecord.Add(current.pvpLogId, current);
						}
					}
				}
				base.Data = myAchievementInfo;
			}
		}
	}
}

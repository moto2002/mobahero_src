using ExitGames.Client.Photon;
using MobaProtocol;
using MobaProtocol.Data;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.Model
{
	internal class Model_taskInfoList : ModelBase<AchieveAll>
	{
		private AchieveAll achieveAll;

		public Model_taskInfoList()
		{
			base.Init(EModelType.Model_taskInfoList);
		}

		public override void RegisterMsgHandler()
		{
			MVC_MessageManager.AddListener_model(MobaGameCode.GetTaskList, new MobaMessageFunc(this.OnGetMsg));
			MVC_MessageManager.AddListener_model(MobaGameCode.GetAchieveTaskAward, new MobaMessageFunc(this.OnGetMsg));
			MVC_MessageManager.AddListener_model(MobaGameCode.ShowDailyTask, new MobaMessageFunc(this.OnGetMsg));
			MVC_MessageManager.AddListener_model(MobaGameCode.GetDailyTaskAward, new MobaMessageFunc(this.OnGetMsg));
		}

		public override void UnRegisterMsgHandler()
		{
			MVC_MessageManager.RemoveListener_model(MobaGameCode.GetTaskList, new MobaMessageFunc(this.OnGetMsg));
			MVC_MessageManager.RemoveListener_model(MobaGameCode.GetAchieveTaskAward, new MobaMessageFunc(this.OnGetMsg));
			MVC_MessageManager.RemoveListener_model(MobaGameCode.ShowDailyTask, new MobaMessageFunc(this.OnGetMsg));
			MVC_MessageManager.RemoveListener_model(MobaGameCode.GetDailyTaskAward, new MobaMessageFunc(this.OnGetMsg));
		}

		protected override void OnGetMsg(MobaMessage msg)
		{
			base.LastError = 505;
			OperationResponse operationResponse = msg.Param as OperationResponse;
			if (operationResponse != null)
			{
				MobaGameCode mobaGameCode = MVC_MessageManager.NotifyModel_to_Game((ClientMsg)msg.ID);
				MobaGameCode mobaGameCode2 = mobaGameCode;
				switch (mobaGameCode2)
				{
				case MobaGameCode.ShowDailyTask:
					this.OnGetMsg_GetTaskList(operationResponse);
					goto IL_9E;
				case MobaGameCode.GetAchieveTaskAward:
					this.OnGetMsg_GetReward(operationResponse);
					goto IL_9E;
				case MobaGameCode.GetFriendGameServer:
				case MobaGameCode.GetGameServer:
					IL_48:
					switch (mobaGameCode2)
					{
					case MobaGameCode.GetTaskList:
						this.OnGetMsg_GetTaskList(operationResponse);
						goto IL_9E;
					case MobaGameCode.CompleteTask:
						goto IL_9E;
					case MobaGameCode.CompleteTaskMessage:
						this.OnGetMsg_CompleteTaskMessage(operationResponse);
						goto IL_9E;
					default:
						goto IL_9E;
					}
					break;
				case MobaGameCode.GetDailyTaskAward:
					this.OnGetMsg_GetReward(operationResponse);
					goto IL_9E;
				}
				goto IL_48;
			}
			IL_9E:
			base.TriggerListners();
		}

		private void OnGetMsg_GetTaskList(OperationResponse res)
		{
			base.LastError = (int)res.Parameters[1];
			if (base.LastError == 0)
			{
				if (this.achieveAll == null)
				{
					this.achieveAll = new AchieveAll();
				}
				if (res.Parameters.ContainsKey(253))
				{
					byte[] buffer = res.Parameters[253] as byte[];
					List<TotalAchieveData> totalAchieveDataList = SerializeHelper.Deserialize<List<TotalAchieveData>>(buffer);
					this.achieveAll.totalAchieveDataList = totalAchieveDataList;
				}
				if (res.Parameters.ContainsKey(254))
				{
					byte[] buffer2 = res.Parameters[254] as byte[];
					List<DetailAchieveData> taskList2 = SerializeHelper.Deserialize<List<DetailAchieveData>>(buffer2);
					if (!res.Parameters.ContainsKey(253))
					{
						if (taskList2.Count > 0)
						{
							this.achieveAll.detailAchieveDataList.RemoveAll((DetailAchieveData obj) => obj.achieveid == taskList2[0].achieveid);
							this.achieveAll.detailAchieveDataList.AddRange(taskList2);
						}
					}
					else
					{
						this.achieveAll.detailAchieveDataList = taskList2;
					}
				}
				if (res.Parameters.ContainsKey(137))
				{
					byte[] buffer3 = res.Parameters[137] as byte[];
					List<DailyTaskData> dailyTaskList = SerializeHelper.Deserialize<List<DailyTaskData>>(buffer3);
					this.achieveAll.dailyTaskList = dailyTaskList;
				}
				base.Data = this.achieveAll;
			}
			else
			{
				base.DebugMessage = "===>MobaGameClientPeer:GetTaskListResponse" + res.OperationCode;
			}
			base.Valid = (base.LastError == 0);
		}

		private void OnGetMsg_GetReward(OperationResponse res)
		{
			base.LastError = (int)res.Parameters[1];
			if (base.LastError == 0)
			{
				int num = (int)res.Parameters[123];
				byte[] buffer = res.Parameters[202] as byte[];
				List<EquipmentInfoData> equipmentList = SerializeHelper.Deserialize<List<EquipmentInfoData>>(buffer);
				byte[] buffer2 = res.Parameters[88] as byte[];
				List<HeroInfoData> heroList = SerializeHelper.Deserialize<List<HeroInfoData>>(buffer2);
				byte[] buffer3 = res.Parameters[246] as byte[];
				List<DropItemData> itemList = SerializeHelper.Deserialize<List<DropItemData>>(buffer3);
				byte[] buffer4 = res.Parameters[146] as byte[];
				List<DropItemData> repeatList = SerializeHelper.Deserialize<List<DropItemData>>(buffer4);
				this.achieveAll.taskAward = new TaskAward
				{
					equipmentList = equipmentList,
					heroList = heroList,
					itemList = itemList,
					repeatList = repeatList
				};
				base.Data = this.achieveAll;
			}
			else
			{
				base.DebugMessage = "===>MobaGameClientPeer:OnGetMsg_GetReward" + res.OperationCode;
			}
			base.Valid = (base.LastError == 0);
		}

		private void OnGetMsg_CompleteTaskMessage(OperationResponse res)
		{
			base.LastError = (int)res.Parameters[1];
			if (base.LastError == 0)
			{
				int num = (int)res.Parameters[123];
				int num2 = (int)res.Parameters[252];
				int num3 = (int)res.Parameters[241];
			}
			else
			{
				base.DebugMessage = "===>MobaGameClientPeer:CompleteTaskMessageResponse" + res.OperationCode;
			}
			base.Valid = (base.LastError == 0);
		}
	}
}

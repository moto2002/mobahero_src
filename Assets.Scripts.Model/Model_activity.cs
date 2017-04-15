using Com.Game.Data;
using Com.Game.Manager;
using Com.Game.Module;
using Com.Game.Utils;
using ExitGames.Client.Photon;
using MobaProtocol;
using MobaProtocol.Data;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.Model
{
	internal class Model_activity : ModelBase<ActivityData>
	{
		private object[] msgs;

		private bool init;

		public Model_activity()
		{
			base.Init(EModelType.Model_activity);
			this.msgs = new object[]
			{
				ClientV2V.Activity_openView,
				ClientV2C.activity_readActivity,
				ClientV2C.activity_readNotice,
				MobaGameCode.GetActivityTask,
				MobaGameCode.GetActivityAward,
				MobaGameCode.GetNoticeBoard,
				MobaGameCode.CompleteTaskMessage
			};
			base.Data = new ActivityData();
			this.init = false;
		}

		public override void RegisterMsgHandler()
		{
			MobaMessageManagerTools.RegistMsg(this, this.msgs, true, "OnMsg_");
		}

		public override void UnRegisterMsgHandler()
		{
			MobaMessageManagerTools.RegistMsg(this, this.msgs, false, "OnMsg_");
		}

		protected override void OnGetMsg(MobaMessage msg)
		{
		}

		private void OnMsg_Activity_openView(MobaMessage msg)
		{
			this.GetActivityTask();
			this.GetNoticeData();
		}

		private void OnMsg_GetActivityTask(MobaMessage msg)
		{
			ActivityData activityData = base.Data as ActivityData;
			OperationResponse operationResponse = msg.Param as OperationResponse;
			if (operationResponse != null)
			{
				base.LastError = (int)operationResponse.Parameters[1];
				if (base.LastError == 0)
				{
					byte[] buffer = operationResponse.Parameters[124] as byte[];
					activityData.listTask = SerializeHelper.Deserialize<List<ActivityTaskData>>(buffer);
					buffer = (operationResponse.Parameters[125] as byte[]);
					activityData.listHasReadActivity = SerializeHelper.Deserialize<List<int>>(buffer);
					this.UpdateActivityNewState();
					MobaMessageManagerTools.SendClientMsg(ClientC2V.Receive_GetActivityTask, activityData, false);
				}
				else
				{
					ClientLogger.Error("OnMsg_GetActivityTask:mobaOpKey=" + base.LastError);
				}
			}
			else
			{
				ClientLogger.Error("OnMsg_GetActivityTask:,msg.Param=错误数据");
			}
			base.Valid = (base.LastError == 0 && null != activityData);
		}

		private void OnMsg_GetActivityAward(MobaMessage msg)
		{
			ActivityData activityData = base.Data as ActivityData;
			OperationResponse operationResponse = msg.Param as OperationResponse;
			if (operationResponse != null)
			{
				base.LastError = (int)operationResponse.Parameters[1];
				if (base.LastError == 0)
				{
					int num = int.Parse((string)operationResponse.Parameters[123]);
					int taskstate = (int)operationResponse.Parameters[125];
					byte[] buffer = operationResponse.Parameters[246] as byte[];
					activityData.rewards.listDropItem = SerializeHelper.Deserialize<List<DropItemData>>(buffer);
					buffer = (operationResponse.Parameters[88] as byte[]);
					activityData.rewards.listHero = SerializeHelper.Deserialize<List<HeroInfoData>>(buffer);
					buffer = (operationResponse.Parameters[202] as byte[]);
					activityData.rewards.listEquip = SerializeHelper.Deserialize<List<EquipmentInfoData>>(buffer);
					buffer = (operationResponse.Parameters[146] as byte[]);
					activityData.rewards.listRepeatItem = SerializeHelper.Deserialize<List<DropItemData>>(buffer);
					ActivityTaskData activityTaskData = null;
					if (activityData.listTask != null)
					{
						for (int i = 0; i < activityData.listTask.Count; i++)
						{
							if (activityData.listTask[i].taskid == num)
							{
								activityTaskData = activityData.listTask[i];
								activityData.listTask[i].taskstate = taskstate;
								break;
							}
						}
						if (activityTaskData != null)
						{
							this.UpdateActivityNewState_onGetRewards(activityTaskData);
							MobaMessageManagerTools.SendClientMsg(ClientC2V.Receive_GetTaskReward, activityTaskData, false);
						}
					}
				}
				else
				{
					ClientLogger.Error("OnMsg_GetActivityAward:mobaOpKey=" + base.LastError);
				}
			}
			else
			{
				ClientLogger.Error("OnMsg_GetActivityAward:,msg.Param=错误数据");
			}
			base.Valid = (base.LastError == 0 && null != activityData);
		}

		private void OnMsg_GetNoticeBoard(MobaMessage msg)
		{
			ActivityData activityData = base.Data as ActivityData;
			OperationResponse operationResponse = msg.Param as OperationResponse;
			if (operationResponse != null)
			{
				base.LastError = (int)operationResponse.Parameters[1];
				if (base.LastError == 0)
				{
					byte[] buffer = operationResponse.Parameters[124] as byte[];
					activityData.listNotice = SerializeHelper.Deserialize<List<NoticeBoardData>>(buffer);
					MobaMessageManagerTools.SendClientMsg(ClientC2V.Receive_GetNoticeBoard, activityData, false);
				}
				else
				{
					ClientLogger.Error("OnMsg_GetActivityTask:mobaOpKey=" + base.LastError);
				}
			}
			else
			{
				ClientLogger.Error("OnMsg_GetActivityTask:,msg.Param=错误数据");
			}
			base.Valid = (base.LastError == 0 && null != activityData);
		}

		private void OnMsg_activity_readActivity(MobaMessage msg)
		{
			int num = (int)msg.Param;
			ActivityData activityData = base.Data as ActivityData;
			if (activityData.dicActivityNewState.ContainsKey(num))
			{
				ActivityData.ActivityNewState activityNewState = activityData.dicActivityNewState[num];
				if (activityNewState.notRead)
				{
					activityNewState.notRead = false;
					this.SendActivity_hasRead(num);
					MobaMessageManagerTools.SendClientMsg(ClientC2V.activity_updateActivityState, activityNewState, true);
				}
			}
		}

		private void OnMsg_activity_readNotice(MobaMessage msg)
		{
			int noticeID = (int)msg.Param;
			ActivityData activityData = base.Data as ActivityData;
			if (activityData.listNotice != null)
			{
				NoticeBoardData noticeBoardData = activityData.listNotice.Find((NoticeBoardData obj) => obj.noticeid == noticeID);
				if (noticeBoardData != null && !noticeBoardData.hasRead)
				{
					noticeBoardData.hasRead = true;
					MobaMessageManagerTools.SendClientMsg(ClientC2V.activity_updateNoticeState, noticeID, true);
					this.SendNotice_hasRead(noticeID);
				}
			}
		}

		private void OnMsg_CompleteTaskMessage(MobaMessage msg)
		{
			OperationResponse operationResponse = msg.Param as OperationResponse;
			if (operationResponse != null)
			{
				int num = (int)operationResponse.Parameters[1];
				MobaErrorCode mobaErrorCode = (MobaErrorCode)num;
				if (mobaErrorCode == MobaErrorCode.Ok)
				{
					int taskId = (int)operationResponse.Parameters[123];
					int num2 = (int)operationResponse.Parameters[252];
					int num3 = (int)operationResponse.Parameters[241];
					if (num3 == 3)
					{
						ActivityData activityData = base.Data as ActivityData;
						if (activityData.listTask != null)
						{
							ActivityTaskData activityTaskData = activityData.listTask.Find((ActivityTaskData obj) => obj.taskid == taskId);
							if (activityTaskData != null)
							{
								activityTaskData.taskstate = 1;
								this.UpdateActivityNewState_onCompleteTask(taskId);
								Singleton<MenuView>.Instance.CheckActivityState();
							}
						}
					}
				}
			}
		}

		private void GetActivityTask()
		{
			SendMsgManager.SendMsgParam param = new SendMsgManager.SendMsgParam(true, "获取活动信息...", true, 15f);
			SendMsgManager.Instance.SendMsg(MobaGameCode.GetActivityTask, param, new object[0]);
		}

		private void SendActivity_hasRead(int activityID)
		{
			SendMsgManager.Instance.SendMsg(MobaGameCode.GetActivityTask, null, new object[]
			{
				activityID
			});
		}

		private void GetNoticeData()
		{
			SendMsgManager.SendMsgParam param = new SendMsgManager.SendMsgParam(true, "获取游戏公告...", true, 15f);
			SendMsgManager.Instance.SendMsg(MobaGameCode.GetNoticeBoard, param, new object[0]);
		}

		private void SendNotice_hasRead(int noticeID)
		{
			SendMsgManager.Instance.SendMsg(MobaGameCode.GetNoticeBoard, null, new object[]
			{
				noticeID
			});
		}

		private void SendTask_stateChange(ActivityData.ActivityNewState newState)
		{
			MobaMessageManagerTools.SendClientMsg(ClientC2V.activity_updateTaskState, newState, false);
		}

		private void InitActivityNewState()
		{
			ActivityData activityData = base.Data as ActivityData;
			if (activityData.dicActivityNewState == null)
			{
				activityData.dicActivityNewState = new Dictionary<int, ActivityData.ActivityNewState>();
			}
			Dictionary<string, object> dicByType = BaseDataMgr.instance.GetDicByType<SysActivityVo>();
			DateTime serverCurrentTime = ToolsFacade.ServerCurrentTime;
			foreach (KeyValuePair<string, object> current in dicByType)
			{
				SysActivityVo sysActivityVo = current.Value as SysActivityVo;
				if (sysActivityVo != null)
				{
					DateTime dateTime = ActivityTools.GetDateTime(sysActivityVo.show_start_time, true);
					DateTime dateTime2 = ActivityTools.GetDateTime(sysActivityVo.show_end_time, false);
					if (string.IsNullOrEmpty(sysActivityVo.show_start_time) || !(serverCurrentTime < dateTime))
					{
						if (string.IsNullOrEmpty(sysActivityVo.show_end_time) || !(serverCurrentTime > dateTime2))
						{
							int key = int.Parse(current.Key);
							if (!activityData.dicActivityNewState.ContainsKey(key))
							{
								ActivityData.ActivityNewState activityNewState = new ActivityData.ActivityNewState
								{
									activityVo = sysActivityVo,
									type = sysActivityVo.activity_type_id,
									dicTaskState = new Dictionary<int, ActivityData.TaskState>()
								};
								activityData.dicActivityNewState.Add(key, activityNewState);
								string[] array = sysActivityVo.module.Split(new char[]
								{
									','
								});
								for (int i = 0; i < array.Length; i++)
								{
									SysActivityModuleVo dataById = BaseDataMgr.instance.GetDataById<SysActivityModuleVo>(array[i]);
									if (dataById != null && dataById.type == 4)
									{
										if (string.IsNullOrEmpty(dataById.content_first))
										{
											ClientLogger.Error("配置错误， 模块ID=" + dataById.id);
										}
										else
										{
											string[] array2 = dataById.content_first.Split(new char[]
											{
												','
											});
											for (int j = 0; j < array2.Length; j++)
											{
												SysActivityTaskVo dataById2 = BaseDataMgr.instance.GetDataById<SysActivityTaskVo>(array2[j]);
												if (dataById2 == null)
												{
													ClientLogger.Error("配置错误，找不到SysActivityTaskVo id=" + array2[j]);
												}
												else
												{
													ActivityData.TaskState value = new ActivityData.TaskState
													{
														taskID = dataById2.id,
														taskVo = dataById2
													};
													activityNewState.dicTaskState.Add(dataById2.id, value);
												}
											}
										}
									}
								}
							}
						}
					}
				}
			}
			this.init = true;
		}

		private void UpdateActivityNewState()
		{
			if (!this.init)
			{
				this.InitActivityNewState();
			}
			ActivityData activityData = base.Data as ActivityData;
			List<ActivityTaskData> listTask = activityData.listTask;
			List<int> listHasReadActivity = activityData.listHasReadActivity;
			foreach (KeyValuePair<int, ActivityData.ActivityNewState> current in activityData.dicActivityNewState)
			{
				bool flag = current.Value.notRead || current.Value.hasRewards;
				current.Value.notRead = (listHasReadActivity == null || !listHasReadActivity.Contains(current.Key));
				current.Value.hasRewards = false;
				foreach (KeyValuePair<int, ActivityData.TaskState> it2 in current.Value.dicTaskState)
				{
					ActivityTaskData activityTaskData = listTask.Find((ActivityTaskData task) => task.taskid == it2.Key);
					if (activityTaskData != null)
					{
						it2.Value.taskState = activityTaskData.taskstate;
						current.Value.hasRewards = (current.Value.hasRewards || it2.Value.taskState == 1);
					}
				}
				if (flag != (current.Value.notRead || current.Value.hasRewards))
				{
					this.SendTask_stateChange(current.Value);
				}
			}
		}

		private void UpdateActivityNewState_onGetRewards(ActivityTaskData newTaskState)
		{
			ActivityData activityData = base.Data as ActivityData;
			foreach (KeyValuePair<int, ActivityData.ActivityNewState> current in activityData.dicActivityNewState)
			{
				if (current.Value.dicTaskState.ContainsKey(newTaskState.taskid))
				{
					current.Value.hasRewards = false;
					foreach (KeyValuePair<int, ActivityData.TaskState> current2 in current.Value.dicTaskState)
					{
						if (current2.Key == newTaskState.taskid)
						{
							current2.Value.taskState = newTaskState.taskstate;
						}
						current.Value.hasRewards = (current.Value.hasRewards || current2.Value.taskState == 1);
					}
					this.SendTask_stateChange(current.Value);
				}
			}
		}

		private void UpdateActivityNewState_onCompleteTask(int taskId)
		{
			ActivityData activityData = base.Data as ActivityData;
			foreach (KeyValuePair<int, ActivityData.ActivityNewState> current in activityData.dicActivityNewState)
			{
				if (current.Value.dicTaskState.ContainsKey(taskId))
				{
					current.Value.hasRewards = true;
					this.SendTask_stateChange(current.Value);
				}
			}
		}
	}
}

using ExitGames.Client.Photon;
using MobaClient;
using MobaProtocol;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.Model
{
	internal class MsgPreHandlerMng
	{
		private static MsgPreHandlerMng mInstance;

		public static MsgPreHandlerMng Instance
		{
			get
			{
				if (MsgPreHandlerMng.mInstance == null)
				{
					MsgPreHandlerMng.mInstance = new MsgPreHandlerMng();
				}
				return MsgPreHandlerMng.mInstance;
			}
		}

		private MsgPreHandlerMng()
		{
			this.Init();
		}

		private void Init()
		{
			MVC_MessageManager.AddListener_preHandler(MobaGameCode.Attendance, new MobaMessageFunc(this.PreHandler_GameCode_Attendance));
			MVC_MessageManager.AddListener_preHandler(MobaGameCode.VipAttendance, new MobaMessageFunc(this.PreHandler_GameCode_VipAttendance));
			MVC_MessageManager.AddListener_preHandler(MobaGameCode.CompleteTask, new MobaMessageFunc(this.PreHandler_GameCode_CompleteTask));
		}

		private void PreHandler_GameCode_Attendance(MobaMessage msg)
		{
			OperationResponse operationResponse = msg.Param as OperationResponse;
			if (operationResponse == null)
			{
				return;
			}
			Log.debug("===>TryBuyGoodsResponse" + operationResponse.OperationCode);
			int num = (int)operationResponse.Parameters[1];
			MobaErrorCode mobaErrorCode = (MobaErrorCode)num;
			if (mobaErrorCode == MobaErrorCode.Ok)
			{
				string text = operationResponse.Parameters[91] as string;
				if (!string.IsNullOrEmpty(text))
				{
					string[] array = text.Split(new char[]
					{
						'|'
					});
					string text2 = array[0];
					if (text2 != null)
					{
						if (MsgPreHandlerMng.<>f__switch$map31 == null)
						{
							MsgPreHandlerMng.<>f__switch$map31 = new Dictionary<string, int>(2)
							{
								{
									"1",
									0
								},
								{
									"3",
									0
								}
							};
						}
						int num2;
						if (MsgPreHandlerMng.<>f__switch$map31.TryGetValue(text2, out num2))
						{
							if (num2 == 0)
							{
								SendMsgManager.SendMsgParam param = new SendMsgManager.SendMsgParam(true, "等待服务器响应...", true, 15f);
								SendMsgManager.Instance.SendMsg(MobaGameCode.GetEquipmentList, param, new object[0]);
								SendMsgManager.Instance.SendMsg(MobaGameCode.GetHeroList, param, new object[]
								{
									ModelManager.Instance.Get_userData_filed_X("SummonerId").ToString()
								});
							}
						}
					}
				}
			}
		}

		private void PreHandler_GameCode_VipAttendance(MobaMessage msg)
		{
			OperationResponse operationResponse = msg.Param as OperationResponse;
			if (operationResponse == null)
			{
				return;
			}
			Log.debug("===>TryBuyGoodsResponse" + operationResponse.OperationCode);
			int num = (int)operationResponse.Parameters[1];
			MobaErrorCode mobaErrorCode = (MobaErrorCode)num;
			if (mobaErrorCode == MobaErrorCode.Ok)
			{
				string text = operationResponse.Parameters[91] as string;
				if (!string.IsNullOrEmpty(text))
				{
					string[] array = text.Split(new char[]
					{
						'|'
					});
					string text2 = array[0];
					if (text2 != null)
					{
						if (MsgPreHandlerMng.<>f__switch$map32 == null)
						{
							MsgPreHandlerMng.<>f__switch$map32 = new Dictionary<string, int>(2)
							{
								{
									"1",
									0
								},
								{
									"3",
									0
								}
							};
						}
						int num2;
						if (MsgPreHandlerMng.<>f__switch$map32.TryGetValue(text2, out num2))
						{
							if (num2 == 0)
							{
								SendMsgManager.SendMsgParam param = new SendMsgManager.SendMsgParam(true, "等待服务器响应...", true, 15f);
								SendMsgManager.Instance.SendMsg(MobaGameCode.GetEquipmentList, param, new object[0]);
								SendMsgManager.Instance.SendMsg(MobaGameCode.GetHeroList, param, new object[]
								{
									ModelManager.Instance.Get_userData_filed_X("SummonerId").ToString()
								});
							}
						}
					}
				}
			}
		}

		private void PreHandler_GameCode_BuyShopGoods(MobaMessage msg)
		{
			OperationResponse operationResponse = msg.Param as OperationResponse;
			if (operationResponse == null)
			{
				return;
			}
			Log.debug("===>TryBuyGoodsResponse" + operationResponse.OperationCode);
			if ((int)operationResponse.Parameters[1] == 0)
			{
				SendMsgManager.SendMsgParam param = new SendMsgManager.SendMsgParam(true, "刷新商店数据", true, 15f);
				SendMsgManager.Instance.SendMsg(MobaGameCode.GetEquipmentList, param, new object[0]);
			}
		}

		private void PreHandler_GameCode_CompleteTask(MobaMessage msg)
		{
			OperationResponse operationResponse = msg.Param as OperationResponse;
			if (operationResponse == null)
			{
				return;
			}
			int num = (int)operationResponse.Parameters[1];
			bool flag = false;
			if (num == 0)
			{
				string text = operationResponse.Parameters[91] as string;
				if (!string.IsNullOrEmpty(text))
				{
					string[] array = text.Split(new char[]
					{
						','
					});
					string[] array2 = array;
					for (int i = 0; i < array2.Length; i++)
					{
						string text2 = array2[i];
						string[] array3 = text2.Split(new char[]
						{
							'|'
						});
						if (array3.Length > 0 && array3[0] == "4")
						{
							flag = true;
							break;
						}
					}
				}
			}
			if (flag)
			{
				SendMsgManager.SendMsgParam param = new SendMsgManager.SendMsgParam(true, "等待服务器响应...", true, 15f);
				SendMsgManager.Instance.SendMsg(MobaGameCode.GetEquipmentList, param, new object[0]);
			}
		}
	}
}

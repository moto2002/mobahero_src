using ExitGames.Client.Photon;
using MobaClient;
using MobaProtocol;
using MobaProtocol.Data;
using MobaProtocol.Helper;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Model
{
	internal class Model_serverlist : ModelBase<ServerListModelData>
	{
		public Model_serverlist()
		{
			base.Init(EModelType.Model_serverlist);
			base.Data = new ServerListModelData();
		}

		public override void RegisterMsgHandler()
		{
			MVC_MessageManager.AddListener_model(MobaMasterCode.GetAllGameServers, new MobaMessageFunc(this.OnGetMsg));
			MVC_MessageManager.AddListener_model(MobaMasterCode.SelectGameArea, new MobaMessageFunc(this.OnGetMsg));
			MVC_MessageManager.AddListener_model(MobaGameCode.Login, new MobaMessageFunc(this.OnGetMsg));
		}

		public override void UnRegisterMsgHandler()
		{
			MVC_MessageManager.RemoveListener_model(MobaMasterCode.GetAllGameServers, new MobaMessageFunc(this.OnGetMsg));
			MVC_MessageManager.RemoveListener_model(MobaMasterCode.SelectGameArea, new MobaMessageFunc(this.OnGetMsg));
			MVC_MessageManager.RemoveListener_model(MobaGameCode.Login, new MobaMessageFunc(this.OnGetMsg));
		}

		protected override void OnGetMsg(MobaMessage msg)
		{
			base.DebugMessage = ((base.LastError != 0) ? "===>服务器列表信息获取失败" : "===>服务器列表信息获取成功");
			if (msg != null)
			{
				OperationResponse operationResponse = msg.Param as OperationResponse;
				if (operationResponse != null)
				{
					base.LastMsgType = (int)msg.MessageType;
					base.LastMsgID = msg.ID;
					int num;
					MobaMessageType mobaMessageType = MVC_MessageManager.ClientMsg_to_RawCode((ClientMsg)msg.ID, out num);
					MobaMessageType mobaMessageType2 = mobaMessageType;
					if (mobaMessageType2 != MobaMessageType.MasterCode)
					{
						if (mobaMessageType2 == MobaMessageType.GameCode)
						{
							MobaGameCode mobaGameCode = (MobaGameCode)num;
							if (mobaGameCode == MobaGameCode.Login)
							{
								this.OnGetMsg_GameCode_Login(operationResponse);
							}
						}
					}
					else
					{
						MobaMasterCode mobaMasterCode = (MobaMasterCode)num;
						if (mobaMasterCode != MobaMasterCode.SelectGameArea)
						{
							if (mobaMasterCode == MobaMasterCode.GetAllGameServers)
							{
								this.OnGetMsg_MasterCode_GetAllGameServers(operationResponse);
							}
						}
						else
						{
							this.OnGetMsg_MasterCode_SelectGameArea(operationResponse);
						}
					}
				}
			}
			base.TriggerListners();
		}

		private void OnGetMsg_MasterCode_SelectGameArea(OperationResponse res)
		{
			base.LastError = (int)res.ReturnCode;
			ServerListModelData serverListModelData = base.Data as ServerListModelData;
			if (base.LastError == 0)
			{
				serverListModelData.m_GatePeeradressIP = (res.Parameters[51] as string);
				serverListModelData.m_GatePeerPort = (int)res.Parameters[52];
			}
			PlayerPrefs.SetInt("LastLoginAreaId", serverListModelData.serverlist[serverListModelData.currLoginServerIndex].areaId);
			PlayerPrefs.Save();
			MobaMessageManagerTools.SendClientMsg(ClientV2C.AreaViewNew_enterGame, null, false);
		}

		private void OnGetMsg_MasterCode_GetAllGameServers(OperationResponse res)
		{
			base.LastError = (int)res.ReturnCode;
			ServerListModelData serverListModelData = base.Data as ServerListModelData;
			if (serverListModelData == null)
			{
				serverListModelData = new ServerListModelData();
				base.Data = serverListModelData;
			}
			MobaErrorCode lastError = (MobaErrorCode)base.LastError;
			if (lastError != MobaErrorCode.Ok)
			{
				base.DebugMessage = "获取服务器列表失败：" + res.DebugMessage;
			}
			else
			{
				Dictionary<byte, object> listObj = res.Parameters[49] as Dictionary<byte, object>;
				List<ServerInfo> serverlist = DataHelper.ToServerList(listObj);
				serverListModelData.serverlist = serverlist;
				string recommendServerId = res.Parameters[50] as string;
				serverListModelData.RecommendServerId = recommendServerId;
				serverListModelData.m_TokenKey = (res.Parameters[15] as string);
				base.DebugMessage = "获取服务器列表成功";
			}
			base.Valid = (base.LastError == 0 && null != base.Data);
		}

		private void OnGetMsg_GameCode_Login(OperationResponse res)
		{
			Log.debug("==> MobaGameClientPeer : LoginUserResponse " + res.ReturnCode);
			base.LastError = (int)res.Parameters[1];
			ServerListModelData serverListModelData = base.Data as ServerListModelData;
			if (serverListModelData == null)
			{
				serverListModelData = new ServerListModelData();
				base.Data = serverListModelData;
			}
			MobaErrorCode lastError = (MobaErrorCode)base.LastError;
			if (lastError != MobaErrorCode.Ok)
			{
				base.DebugMessage = "登录获得OnlyServerKey失败：" + res.DebugMessage;
			}
			else
			{
				serverListModelData.OnlyServerKey = (res.Parameters[182] as string);
				base.DebugMessage = "登录获得OnlyServerKey成功";
			}
			base.Valid = ((base.LastError == 0 || base.LastError == 10103) && null != base.Data);
		}
	}
}

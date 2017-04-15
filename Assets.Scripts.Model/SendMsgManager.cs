using Com.Game.Module;
using Com.Game.Utils;
using MobaClient;
using MobaHeros.Pvp;
using MobaProtocol;
using MobaProtocol.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Assets.Scripts.Model
{
	public class SendMsgManager
	{
		public class SendMsgParam
		{
			private bool _openWaitingView;

			private string _waitingViewText;

			private bool _bNormal;

			private float _waitTime;

			public bool OpenWaitingView
			{
				get
				{
					return this._openWaitingView;
				}
			}

			public string WaitingViewText
			{
				get
				{
					return this._waitingViewText;
				}
			}

			public bool BNormal
			{
				get
				{
					return this._bNormal;
				}
			}

			public float WaitTime
			{
				get
				{
					return this._waitTime;
				}
			}

			public SendMsgParam(bool openWaitingView, string waitingViewText, bool bNormal = true, float waitTime = 15f)
			{
				this._openWaitingView = openWaitingView;
				this._waitingViewText = waitingViewText;
				this._bNormal = bNormal;
				this._waitTime = waitTime;
			}
		}

		public static SendMsgManager mInstance;

		public static SendMsgManager Instance
		{
			get
			{
				if (SendMsgManager.mInstance == null)
				{
					SendMsgManager.mInstance = new SendMsgManager();
				}
				return SendMsgManager.mInstance;
			}
		}

		public bool ClientConnectToMaster()
		{
			string serverName = ModelManager.Instance.Get_masterIp() + ":" + ModelManager.Instance.Get_masterPort();
			string appName = ModelManager.Instance.Get_appName();
			return NetWorkHelper.Instance.client.MainPeerConnect(MobaPeerType.C2Master, serverName, appName);
		}

		public bool ClientConnectToLogin()
		{
			string serverName = ModelManager.Instance.Get_masterIp() + ":" + ModelManager.Instance.Get_masterPort();
			string appName = ModelManager.Instance.Get_appName();
			return NetWorkHelper.Instance.client.MainPeerConnect(MobaPeerType.C2Master, serverName, appName);
		}

		public bool ClientConnectToGate()
		{
			Log.debug("==> MobaClient：ClientConnectToGate ...");
			string appName = "MobaServer.GateServer";
			ModelManager.Instance.Get_GamePeerIP_X();
			ServerListModelData serverListModelData = ModelManager.Instance.Get_serverInfo();
			if (serverListModelData == null)
			{
				return false;
			}
			string serverName = serverListModelData.m_GatePeeradressIP + ":" + serverListModelData.m_GatePeerPort;
			return NetWorkHelper.Instance.client.MainPeerConnect(MobaPeerType.C2GateServer, serverName, appName);
		}

		public bool ClientConnectToPVP(string serverIP, int serverPort, string appName)
		{
			return NetWorkHelper.Instance.client.MainPeerConnect(MobaPeerType.C2PvpServer, serverIP + ":" + serverPort, appName);
		}

		[DebuggerHidden]
		public IEnumerator GetMasterIP2()
		{
			return new SendMsgManager.<GetMasterIP2>c__Iterator18F();
		}

		public bool SendMsg(MobaMasterCode code, SendMsgManager.SendMsgParam param, params object[] rawParam)
		{
			return this.SendMsg(MobaPeerType.C2Master, (byte)code, param, rawParam);
		}

		public bool SendMsg(MobaGameCode code, SendMsgManager.SendMsgParam param, params object[] rawParam)
		{
			float realtimeSinceStartup = Time.realtimeSinceStartup;
			if (!MenuView.dicTimes.ContainsKey(code))
			{
				MenuView.dicTimes.Add(code, realtimeSinceStartup);
			}
			else
			{
				MenuView.dicTimes[code] = realtimeSinceStartup;
			}
			return this.SendGameChannelMessage((byte)code, param, rawParam);
		}

		public bool SendMsg(MobaFriendCode code, SendMsgManager.SendMsgParam param, params object[] rawParam)
		{
			return this.SendChannelMsg(MobaChannel.Friend, (byte)code, param, rawParam);
		}

		public bool SendMsg(MobaTeamRoomCode code, SendMsgManager.SendMsgParam param, params object[] rawParam)
		{
			return this.SendChannelMsg(MobaChannel.Team, (byte)code, param, rawParam);
		}

		public bool SendChannelMsg(MobaChannel channel, MobaGameCode code, SendMsgManager.SendMsgParam param, params object[] rawParam)
		{
			return this.SendChannelMsg(channel, (byte)code, param, rawParam);
		}

		public bool SendChannelMsg(MobaChannel channel, byte code, SendMsgManager.SendMsgParam param, params object[] rawParam)
		{
			return this.SendMessageByGateChannel(channel, code, param, rawParam);
		}

		public bool SendMsg(MobaGateCode code, SendMsgManager.SendMsgParam param, params object[] rawParam)
		{
			return this.SendMsg(MobaPeerType.C2GateServer, (byte)code, param, rawParam);
		}

		private bool SendMsg(MobaPeerType peerType, byte code, SendMsgManager.SendMsgParam param, object[] rawParam)
		{
			bool flag = false;
			try
			{
				string methodNameString = this.GetMethodNameString(peerType, code);
				Dictionary<byte, object> item;
				Dictionary<string, object> dictionary;
				if (!this.MakeMsgParam(methodNameString, rawParam, out item, out dictionary))
				{
					throw new Exception(string.Format("methodName {0} 构造参数错误", methodNameString));
				}
				MobaPeer mobaPeer = NetWorkHelper.Instance.client.GetMobaPeer(peerType);
				if (mobaPeer == null)
				{
					throw new Exception(peerType.ToString() + "MobaPeer==null");
				}
				List<object> list = new List<object>();
				List<Type> list2 = new List<Type>();
				list.Add(code);
				list2.Add(typeof(byte));
				list.Add(item);
				list2.Add(typeof(Dictionary<byte, object>));
				list.Add(true);
				list2.Add(typeof(bool));
				if (dictionary != null && dictionary.ContainsKey("channelId"))
				{
					list.Add(dictionary["channelId"]);
					list2.Add(typeof(byte));
					if (dictionary.ContainsKey("encrypt"))
					{
						list.Add(dictionary["encrypt"]);
						list2.Add(typeof(bool));
					}
				}
				Type typeFromHandle = typeof(MobaPeer);
				MethodInfo method = typeFromHandle.GetMethod("OpCustom", list2.ToArray());
				if (method == null)
				{
					throw new Exception("消息参数错误");
				}
				if (param != null && param.OpenWaitingView)
				{
					WaitingViewMng.Instance.Waiting(this.GetMobaMessageType(peerType), (int)code, param.WaitingViewText, param.BNormal, param.WaitTime, true);
				}
				flag = (bool)method.Invoke(mobaPeer, list.ToArray());
			}
			catch (Exception ex)
			{
				flag = false;
				UnityEngine.Debug.LogError(string.Format("SendMsg code = {0}   Error : {1}", code.ToString(), ex.Message));
			}
			if (flag)
			{
			}
			return flag;
		}

		private bool MakeMsgParam(string methodName, object[] rawParam, out Dictionary<byte, object> baseParam, out Dictionary<string, object> appendParam)
		{
			bool result = false;
			baseParam = null;
			appendParam = null;
			if (!string.IsNullOrEmpty(methodName))
			{
				List<object> list = new List<object>(rawParam);
				List<Type> list2 = new List<Type>();
				for (int i = 0; i < rawParam.Length; i++)
				{
					object obj = rawParam[i];
					if (obj == null)
					{
						UnityEngine.Debug.LogError("MakeMsgParam：参数错误");
						break;
					}
					list2.Add(obj.GetType());
				}
				if (list2.Count == list.Count)
				{
					list.Add(baseParam);
					list2.Add(typeof(Dictionary<byte, object>).MakeByRefType());
					list.Add(appendParam);
					list2.Add(typeof(Dictionary<string, object>).MakeByRefType());
					Type type = base.GetType();
					MethodInfo method = type.GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic, null, list2.ToArray(), null);
					if (method == null)
					{
						UnityEngine.Debug.LogError("MakeMsgParam：找不到函数:" + methodName);
					}
					else
					{
						object[] array = list.ToArray();
						result = (bool)method.Invoke(this, array);
						baseParam = (Dictionary<byte, object>)array[array.Length - 2];
						appendParam = (Dictionary<string, object>)array[array.Length - 1];
					}
				}
			}
			return result;
		}

		private string GetMethodNameString(MobaPeerType peerType, byte code)
		{
			string result = string.Empty;
			switch (peerType)
			{
			case MobaPeerType.C2Master:
				result = "Master_" + ((MobaMasterCode)code).ToString();
				break;
			case MobaPeerType.C2GateServer:
				result = "Gate_" + ((MobaGateCode)code).ToString();
				break;
			}
			return result;
		}

		private string GetMethodNameStringByChannel(MobaChannel channel, byte code)
		{
			string result = string.Empty;
			switch (channel)
			{
			case MobaChannel.Friend:
				result = "Game_" + (MobaFriendCode)code;
				break;
			case MobaChannel.Team:
				if (code == 16)
				{
					result = "MobaTeamRoomCode_Room_RefuseJoinInvite";
				}
				else if (code != 14)
				{
					result = "TeamRoom_Common";
				}
				else
				{
					result = "TeamRoom_InviteJoinRoom";
				}
				break;
			}
			return result;
		}

		private MobaMessageType GetMobaMessageType(MobaPeerType peerType)
		{
			MobaMessageType result = MobaMessageType.Client;
			switch (peerType)
			{
			case MobaPeerType.C2Master:
				result = MobaMessageType.MasterCode;
				break;
			case MobaPeerType.C2GateServer:
				result = MobaMessageType.GameCode;
				break;
			}
			return result;
		}

		private bool Master_GuestUpgrade(out Dictionary<byte, object> baseParam, out Dictionary<string, object> appendParam)
		{
			bool result = true;
			baseParam = null;
			appendParam = null;
			return result;
		}

		private bool Master_Register(out Dictionary<byte, object> baseParam, out Dictionary<string, object> appendParam)
		{
			bool result = true;
			baseParam = null;
			appendParam = null;
			Log.debug("==> PhotonClient ： 注册用户 ");
			baseParam = new Dictionary<byte, object>();
			baseParam[85] = SerializeHelper.Serialize<AccountData>(ModelManager.Instance.Get_accountData_X());
			return result;
		}

		private bool Master_GetPhoneCode(string PhoneNumber, out Dictionary<byte, object> baseParam, out Dictionary<string, object> appendParam)
		{
			Log.debug("==> PhotonClient ： 注册用户 ");
			appendParam = null;
			baseParam = new Dictionary<byte, object>();
			baseParam[81] = PhoneNumber;
			return true;
		}

		private bool Master_CheckPhoneAndCode(string PhoneNumber, string PhoneCheckCode, out Dictionary<byte, object> baseParam, out Dictionary<string, object> appendParam)
		{
			Log.debug("==> PhotonClient ： 注册用户 ");
			appendParam = null;
			baseParam = new Dictionary<byte, object>();
			baseParam[81] = PhoneNumber;
			baseParam[82] = PhoneCheckCode;
			return true;
		}

		private bool Master_FindMyAccountPasswd(string PhoneNumber, out Dictionary<byte, object> baseParam, out Dictionary<string, object> appendParam)
		{
			Log.debug("==> PhotonClient ： 找回密码");
			appendParam = null;
			baseParam = new Dictionary<byte, object>();
			baseParam[81] = PhoneNumber;
			return true;
		}

		private bool Master_ModifyAccountPasswd(string PhoneNumber, string PassWord, out Dictionary<byte, object> baseParam, out Dictionary<string, object> appendParam)
		{
			Log.debug("==> PhotonClient ： 修改密码 ");
			appendParam = null;
			baseParam = new Dictionary<byte, object>();
			baseParam[81] = PhoneNumber;
			baseParam[74] = PassWord;
			return true;
		}

		private bool Master_Register(AccountData userdata, out Dictionary<byte, object> baseParam, out Dictionary<string, object> appendParam)
		{
			Log.debug("==> PhotonClient ： 注册用户 ");
			appendParam = null;
			userdata.DeviceToken = ((SystemInfo.deviceUniqueIdentifier != null) ? SystemInfo.deviceUniqueIdentifier : "Unknow");
			baseParam = new Dictionary<byte, object>();
			baseParam[85] = SerializeHelper.Serialize<AccountData>(userdata);
			return true;
		}

		private bool Master_Login(out Dictionary<byte, object> baseParam, out Dictionary<string, object> appendParam)
		{
			bool result = true;
			appendParam = null;
			AccountData accountData = ModelManager.Instance.Get_accountData_X();
			if (accountData == null)
			{
				throw new NullReferenceException();
			}
			Log.debug(" MobaClient ： 用户登录（使用上一次用户信息） ");
			baseParam = new Dictionary<byte, object>();
			baseParam[85] = SerializeHelper.Serialize<AccountData>(accountData);
			return result;
		}

		private bool Master_Login(AccountData accData, out Dictionary<byte, object> baseParam, out Dictionary<string, object> appendParam)
		{
			bool result = true;
			appendParam = null;
			Log.debug(" MobaClient ： 用户登录（使用上一次用户信息） ");
			baseParam = new Dictionary<byte, object>();
			baseParam[85] = SerializeHelper.Serialize<AccountData>(accData);
			return result;
		}

		private bool Master_UpgradeUrl(out Dictionary<byte, object> baseParam, out Dictionary<string, object> appendParam)
		{
			bool result = true;
			Log.debug(" MobaClient ： CheckUpgrade...");
			appendParam = null;
			baseParam = new Dictionary<byte, object>();
			byte[] value = SerializeHelper.Serialize<ClientData>(new ClientData
			{
				AppVersion = GlobalSettings.AppVersion,
				AppResVersion = "1.0.0",
				DeviceType = 2
			});
			baseParam[84] = value;
			baseParam[101] = GlobalSettings.MasterIpVersion;
			return result;
		}

		private bool Master_GetAllGameServers(out Dictionary<byte, object> baseParam, out Dictionary<string, object> appendParam)
		{
			Log.debug(" PhotonClient ： 获取服务器列表... ");
			appendParam = null;
			baseParam = new Dictionary<byte, object>();
			baseParam[72] = ModelManager.Instance.Get_accountData_filed_X("UserName");
			return true;
		}

		private bool Master_SelectGameArea(int _areaId, out Dictionary<byte, object> baseParam, out Dictionary<string, object> appendParam)
		{
			appendParam = null;
			baseParam = new Dictionary<byte, object>();
			baseParam[245] = _areaId;
			return true;
		}

		private bool Master_LoginByPlatformUid(int uid, string channel, string channelUid, string accessToken, int productId, bool isBindPhone, bool isBindEmail, out Dictionary<byte, object> baseParam, out Dictionary<string, object> appendParam)
		{
			Log.debug(" PhotonClient ： 渠道方客户端登陆... ");
			appendParam = null;
			baseParam = new Dictionary<byte, object>();
			baseParam[222] = uid;
			baseParam[223] = channel;
			baseParam[224] = channelUid;
			baseParam[225] = accessToken;
			baseParam[226] = productId;
			baseParam[227] = isBindPhone;
			baseParam[228] = isBindEmail;
			return true;
		}

		private bool Master_LoginByChannelId(string channelId, string userName, out Dictionary<byte, object> baseParam, out Dictionary<string, object> appendParam)
		{
			Log.debug(" PhotonClient ： 客户端QQ WeChat登陆... ");
			appendParam = null;
			baseParam = new Dictionary<byte, object>();
			baseParam[78] = channelId;
			baseParam[72] = userName;
			return true;
		}

		private bool Master_LoginOut(out Dictionary<byte, object> baseParam, out Dictionary<string, object> appendParam)
		{
			Log.debug(" PhotonClient ： 渠道方客户端账户登出... ");
			appendParam = null;
			baseParam = new Dictionary<byte, object>();
			baseParam[57] = ModelManager.Instance.Get_accountData_X().AccountId;
			return true;
		}

		private bool Gate_SelectGameServer(string serverId, string lobbyId, string sessionId, out Dictionary<byte, object> baseParam, out Dictionary<string, object> appendParam)
		{
			Log.debug(" PhotonClient ： 指定网关对应的服务器... ");
			appendParam = null;
			ServerListModelData serverListModelData = ModelManager.Instance.Get_serverInfo();
			baseParam = new Dictionary<byte, object>();
			baseParam[0] = serverId;
			baseParam[2] = lobbyId;
			baseParam[1] = sessionId;
			return true;
		}

		private bool Gate_VerificationKey(string accountId, string TokenKey, out Dictionary<byte, object> baseParam, out Dictionary<string, object> appendParam)
		{
			Log.debug(" PhotonClient ： 验证会话秘钥... ");
			appendParam = null;
			baseParam = new Dictionary<byte, object>();
			baseParam[71] = accountId;
			baseParam[5] = TokenKey;
			return true;
		}

		private bool Game_GetDiscountCard(out Dictionary<byte, object> baseParam, out Dictionary<string, object> appendParam)
		{
			bool result = true;
			appendParam = null;
			baseParam = new Dictionary<byte, object>();
			return result;
		}

		private bool Game_ReconnectToGameServer(out Dictionary<byte, object> baseParam, out Dictionary<string, object> appendParam)
		{
			bool result = true;
			appendParam = null;
			baseParam = new Dictionary<byte, object>();
			baseParam[57] = ModelManager.Instance.Get_userData_filed_X("UserId");
			baseParam[182] = ModelManager.Instance.Get_OnlyServerKey_X();
			baseParam[218] = GlobalSettings.Instance.versionConfig.appVersion;
			baseParam[229] = ResourceManager.GetBindataMD5();
			return result;
		}

		private bool Game_Register(string nickname, out Dictionary<byte, object> baseParam, out Dictionary<string, object> appendParam)
		{
			bool result = true;
			appendParam = null;
			string text = ModelManager.Instance.Get_accountData_filed_X("AccountId");
			Log.debug("==> PhotonClient ： 注册游戏账户... nickname = " + nickname + ", accountid = " + text);
			baseParam = new Dictionary<byte, object>();
			baseParam[59] = nickname;
			baseParam[71] = text;
			baseParam[218] = GlobalSettings.Instance.versionConfig.appVersion;
			baseParam[229] = ResourceManager.GetBindataMD5();
			return result;
		}

		private bool Game_Login(out Dictionary<byte, object> baseParam, out Dictionary<string, object> appendParam)
		{
			bool result = true;
			appendParam = null;
			baseParam = null;
			AccountData accountData = ModelManager.Instance.Get_accountData_X();
			string text = ModelManager.Instance.Get_accountData_filed_X("AccountId");
			if (accountData != null && !string.IsNullOrEmpty(text))
			{
				Log.debug("==> PhotonClient ： 登录已有的游戏账户 ... AccountId = " + text);
				baseParam = new Dictionary<byte, object>();
				baseParam[57] = text;
				baseParam[213] = APush.regId;
				baseParam[229] = ResourceManager.GetBindataMD5();
				baseParam[218] = GlobalSettings.Instance.versionConfig.appVersion;
			}
			else
			{
				result = false;
			}
			return result;
		}

		private bool Game_QueryRoomState(out Dictionary<byte, object> baseParam, out Dictionary<string, object> appendParam)
		{
			bool result = true;
			appendParam = null;
			baseParam = new Dictionary<byte, object>();
			return result;
		}

		private bool Game_GetHeroList(string summonerId, out Dictionary<byte, object> baseParam, out Dictionary<string, object> appendParam)
		{
			bool result = true;
			Log.debug(" MobaClient ： 获取我的英雄列表... summonerId = " + summonerId);
			appendParam = null;
			baseParam = new Dictionary<byte, object>();
			baseParam[83] = summonerId;
			return result;
		}

		private bool Game_GetEquipmentList(out Dictionary<byte, object> baseParam, out Dictionary<string, object> appendParam)
		{
			bool result = true;
			appendParam = null;
			UserData userData = ModelManager.Instance.Get_userData_X();
			if (userData == null)
			{
				throw new Exception("userData 无效");
			}
			Log.debug(" MobaClient ： 获取背包道具列表... UserId = " + userData.UserId);
			baseParam = new Dictionary<byte, object>();
			return result;
		}

		private bool Game_CompleteNewerGuide(int groupId, out Dictionary<byte, object> baseParam, out Dictionary<string, object> appendParam)
		{
			bool result = true;
			string value = ModelManager.Instance.Get_userData_filed_X("UserId");
			appendParam = null;
			baseParam = new Dictionary<byte, object>();
			baseParam[57] = value;
			baseParam[212] = groupId;
			return result;
		}

		private bool Game_UsingEquipment(string equipId, string ePosition, string heroId, out Dictionary<byte, object> baseParam, out Dictionary<string, object> appendParam)
		{
			bool result = true;
			Log.debug(string.Concat(new string[]
			{
				" MobaClient ： 穿戴装备... heroId = ",
				heroId,
				",装备ID：",
				equipId,
				",穿戴位置：",
				ePosition
			}));
			appendParam = null;
			baseParam = new Dictionary<byte, object>();
			baseParam[57] = ModelManager.Instance.Get_userData_filed_X("UserId");
			baseParam[93] = equipId;
			baseParam[92] = ePosition;
			baseParam[89] = heroId;
			long eid = Convert.ToInt64(equipId);
			if ((from obj in ModelManager.Instance.Get_equipmentList_X()
			where obj.EquipmentId == eid
			select obj).FirstOrDefault<EquipmentInfoData>() == null)
			{
				throw new Exception("===>要装备到道具不存在于背包列表中！equipID: " + equipId);
			}
			long hid = Convert.ToInt64(heroId);
			List<HeroInfoData> source = ModelManager.Instance.Get_heroInfo_list_X();
			HeroInfoData heroInfoData = (from obj in source
			where obj.HeroId == hid
			select obj).FirstOrDefault<HeroInfoData>();
			if (heroInfoData != null)
			{
				switch (int.Parse(ePosition))
				{
				case 1:
					if (heroInfoData.Ep_1 == 0)
					{
						heroInfoData.Ep_1 = int.Parse(equipId);
					}
					break;
				case 2:
					if (heroInfoData.Ep_2 == 0)
					{
						heroInfoData.Ep_2 = int.Parse(equipId);
					}
					break;
				case 3:
					if (heroInfoData.Ep_3 == 0)
					{
						heroInfoData.Ep_3 = int.Parse(equipId);
					}
					break;
				case 4:
					if (heroInfoData.Ep_4 == 0)
					{
						heroInfoData.Ep_4 = int.Parse(equipId);
					}
					break;
				case 5:
					if (heroInfoData.Ep_5 == 0)
					{
						heroInfoData.Ep_5 = int.Parse(equipId);
					}
					break;
				case 6:
					if (heroInfoData.Ep_6 == 0)
					{
						heroInfoData.Ep_6 = int.Parse(equipId);
					}
					break;
				default:
					throw new Exception("===>传入的装备位置错误 ePosition：" + ePosition);
				}
			}
			return result;
		}

		private bool Game_DischargeRune(string modelid, string ePosition, string heroId, int type, out Dictionary<byte, object> baseParam, out Dictionary<string, object> appendParam)
		{
			bool result = true;
			Log.debug(" RunesDemount ： 卸下符文... heroId = " + heroId);
			appendParam = null;
			baseParam = new Dictionary<byte, object>();
			baseParam[57] = ModelManager.Instance.Get_userData_filed_X("UserId");
			baseParam[247] = int.Parse(modelid);
			baseParam[92] = byte.Parse(ePosition);
			baseParam[89] = long.Parse(heroId);
			baseParam[109] = type;
			return result;
		}

		private bool Game_ChangeSkin(string heroid, string skinid, out Dictionary<byte, object> baseParam, out Dictionary<string, object> appendParam)
		{
			bool result = true;
			Log.debug("chaning skin...heroid=" + heroid);
			appendParam = null;
			baseParam = new Dictionary<byte, object>();
			baseParam[83] = ModelManager.Instance.Get_userData_filed_X("SummonerId");
			baseParam[89] = long.Parse(heroid);
			baseParam[208] = int.Parse(skinid);
			return result;
		}

		private bool Game_HeroAdvance(string heroId, out Dictionary<byte, object> baseParam, out Dictionary<string, object> appendParam)
		{
			bool result = true;
			Log.debug(" MobaClient ： 英雄进阶... heroId = " + heroId);
			appendParam = null;
			baseParam = new Dictionary<byte, object>();
			baseParam[89] = heroId;
			baseParam[57] = ModelManager.Instance.Get_userData_filed_X("UserId");
			return result;
		}

		private bool Game_GetEquipmentDrop(string battleId, string sceneId, string[] fightHeros, out Dictionary<byte, object> baseParam, out Dictionary<string, object> appendParam)
		{
			bool result = true;
			Log.debug(" MobaClient ： 获取掉落信息... sceneId = " + sceneId);
			appendParam = null;
			baseParam = new Dictionary<byte, object>();
			baseParam[113] = battleId;
			baseParam[94] = sceneId;
			baseParam[57] = ModelManager.Instance.Get_userData_filed_X("UserId");
			baseParam[112] = fightHeros;
			return result;
		}

		private bool Game_UploadFightResult(bool isWin, byte star, string battleId, string sceneId, string[] heroList, out Dictionary<byte, object> baseParam, out Dictionary<string, object> appendParam)
		{
			bool result = true;
			Log.debug(" MobaClient ： 上报战斗结果是否胜利... isWin = " + isWin);
			appendParam = null;
			baseParam = new Dictionary<byte, object>();
			baseParam[95] = isWin;
			baseParam[57] = ModelManager.Instance.Get_userData_filed_X("UserId");
			baseParam[111] = star;
			baseParam[113] = battleId;
			baseParam[94] = sceneId;
			baseParam[112] = heroList;
			return result;
		}

		private bool Game_GetTalent(out Dictionary<byte, object> baseParam, out Dictionary<string, object> appendParam)
		{
			bool result = true;
			Log.debug(" 获取我的天赋信息... summoneId = " + ModelManager.Instance.Get_userData_filed_X("SummonerId"));
			appendParam = null;
			baseParam = new Dictionary<byte, object>();
			baseParam[83] = ModelManager.Instance.Get_userData_filed_X("SummonerId");
			return result;
		}

		private bool Game_ChangeTalent(List<TalentModel> data, int talentPag, out Dictionary<byte, object> baseParam, out Dictionary<string, object> appendParam)
		{
			bool result = true;
			appendParam = null;
			Log.debug(" MobaClient ： 修改我的天赋... talentPag = " + talentPag);
			baseParam = new Dictionary<byte, object>();
			baseParam[97] = talentPag;
			baseParam[83] = ModelManager.Instance.Get_userData_filed_X("SummonerId");
			baseParam[57] = ModelManager.Instance.Get_userData_filed_X("UserId");
			baseParam[96] = SerializeHelper.Serialize<List<TalentModel>>(data);
			return result;
		}

		private bool Game_BuyTalentPag(out Dictionary<byte, object> baseParam, out Dictionary<string, object> appendParam)
		{
			bool result = true;
			appendParam = null;
			Log.debug(" MobaClient ： 购买新的天赋页... ");
			baseParam = new Dictionary<byte, object>();
			baseParam[83] = ModelManager.Instance.Get_userData_filed_X("SummonerId");
			baseParam[57] = ModelManager.Instance.Get_userData_filed_X("UserId");
			return result;
		}

		private bool Game_ModfiyTalentPag(int pagId, out Dictionary<byte, object> baseParam, out Dictionary<string, object> appendParam)
		{
			bool result = true;
			appendParam = null;
			Log.debug(" MobaClient ： 更换当前使用天赋页... ");
			baseParam = new Dictionary<byte, object>();
			baseParam[97] = pagId;
			baseParam[83] = ModelManager.Instance.Get_userData_filed_X("SummonerId");
			return result;
		}

		private bool Game_RestTalentPag(int pagId, out Dictionary<byte, object> baseParam, out Dictionary<string, object> appendParam)
		{
			bool result = true;
			appendParam = null;
			Log.debug(" MobaClient ： 重置指定天赋页... ");
			baseParam = new Dictionary<byte, object>();
			baseParam[97] = pagId;
			baseParam[57] = ModelManager.Instance.Get_userData_filed_X("UserId");
			baseParam[83] = ModelManager.Instance.Get_userData_filed_X("SummonerId");
			return result;
		}

		private bool Game_UseSoulstone(int soulId, out Dictionary<byte, object> baseParam, out Dictionary<string, object> appendParam)
		{
			bool result = true;
			appendParam = null;
			Log.debug(" MobaClient ：使用灵魂石... ");
			baseParam = new Dictionary<byte, object>();
			baseParam[100] = soulId;
			baseParam[57] = ModelManager.Instance.Get_userData_filed_X("UserId");
			baseParam[83] = ModelManager.Instance.Get_userData_filed_X("SummonerId");
			return result;
		}

		private bool Game_UseProps(long equipId, int count, string targetId, out Dictionary<byte, object> baseParam, out Dictionary<string, object> appendParam)
		{
			bool result = true;
			appendParam = null;
			Log.debug(" MobaClient ：使用道具... ");
			baseParam = new Dictionary<byte, object>();
			baseParam[93] = equipId;
			baseParam[101] = count;
			baseParam[102] = targetId;
			baseParam[57] = ModelManager.Instance.Get_userData_filed_X("UserId");
			baseParam[83] = ModelManager.Instance.Get_userData_filed_X("SummonerId");
			return result;
		}

		private bool Game_GetRune(out Dictionary<byte, object> baseParam, out Dictionary<string, object> appendParam)
		{
			bool result = true;
			appendParam = null;
			Log.debug(" MobaClient ：获取我的符文列表... SummonerId：" + ModelManager.Instance.Get_userData_filed_X("SummonerId"));
			baseParam = new Dictionary<byte, object>();
			baseParam[83] = ModelManager.Instance.Get_userData_filed_X("SummonerId");
			return result;
		}

		private bool Game_ChangeRune(RuneModel data, int runePag, bool isPutOn, out Dictionary<byte, object> baseParam, out Dictionary<string, object> appendParam)
		{
			bool result = true;
			appendParam = null;
			Log.debug(" TryChangeRune ： 修改我的符文... runePag = " + runePag);
			baseParam = new Dictionary<byte, object>();
			baseParam[105] = runePag;
			baseParam[83] = ModelManager.Instance.Get_userData_filed_X("SummonerId");
			baseParam[104] = SerializeHelper.Serialize<RuneModel>(data);
			baseParam[106] = isPutOn;
			baseParam[57] = ModelManager.Instance.Get_userData_filed_X("UserId");
			return result;
		}

		private bool Game_BuyRunePag(out Dictionary<byte, object> baseParam, out Dictionary<string, object> appendParam)
		{
			bool result = true;
			appendParam = null;
			Log.debug(" MobaClient ： 购买新的符文页... ");
			baseParam = new Dictionary<byte, object>();
			baseParam[83] = ModelManager.Instance.Get_userData_filed_X("SummonerId");
			return result;
		}

		private bool Game_ModfiyRunePag(int pagId, out Dictionary<byte, object> baseParam, out Dictionary<string, object> appendParam)
		{
			bool result = true;
			appendParam = null;
			Log.debug(" MobaClient ： 更换当前使用符文页...pagId: " + pagId);
			baseParam = new Dictionary<byte, object>();
			baseParam[105] = pagId;
			baseParam[83] = ModelManager.Instance.Get_userData_filed_X("SummonerId");
			return result;
		}

		private bool Game_Coalesce(int equipId, int count1, int count2, out Dictionary<byte, object> baseParam, out Dictionary<string, object> appendParam)
		{
			bool result = true;
			appendParam = null;
			Log.debug(" MobaClient ： 道具合成...equipId: " + equipId);
			baseParam = new Dictionary<byte, object>();
			baseParam[93] = equipId.ToString();
			baseParam[57] = ModelManager.Instance.Get_userData_filed_X("UserId");
			baseParam[56] = count1;
			baseParam[71] = count2;
			baseParam[61] = (int)ModelManager.Instance.Get_userData_X().Money;
			return result;
		}

		private bool Game_GetBattles(out Dictionary<byte, object> baseParam, out Dictionary<string, object> appendParam)
		{
			bool result = true;
			appendParam = null;
			Log.debug(" MobaClient ： 获取战役信息...");
			baseParam = new Dictionary<byte, object>();
			baseParam[57] = ModelManager.Instance.Get_userData_filed_X("UserId");
			return result;
		}

		private bool Game_BuySkillPoint(out Dictionary<byte, object> baseParam, out Dictionary<string, object> appendParam)
		{
			bool result = true;
			appendParam = null;
			Log.debug(" MobaClient ： 购买技能点...");
			baseParam = new Dictionary<byte, object>();
			baseParam[57] = ModelManager.Instance.Get_userData_filed_X("UserId");
			return result;
		}

		private bool Game_UsingSkillPoint(string heroId, int skillIndex, int point, out Dictionary<byte, object> baseParam, out Dictionary<string, object> appendParam)
		{
			bool result = true;
			appendParam = null;
			Log.debug(" MobaClient ： 使用技能点...");
			baseParam = new Dictionary<byte, object>();
			baseParam[57] = ModelManager.Instance.Get_userData_filed_X("UserId");
			baseParam[89] = heroId;
			baseParam[114] = skillIndex;
			baseParam[101] = point;
			return result;
		}

		private bool Game_CheckUnlockBattle(string battleId, out Dictionary<byte, object> baseParam, out Dictionary<string, object> appendParam)
		{
			bool result = true;
			appendParam = null;
			Log.debug(" MobaClient ： 根据战役ID检查战役是否解锁...");
			baseParam = new Dictionary<byte, object>();
			baseParam[113] = battleId;
			baseParam[57] = ModelManager.Instance.Get_userData_filed_X("UserId");
			return result;
		}

		private bool Game_UpdateDefFight(out Dictionary<byte, object> baseParam, out Dictionary<string, object> appendParam)
		{
			bool result = true;
			appendParam = null;
			Log.debug(" MobaClient ： 更新远征战队信息(防守阵型)..." + ModelManager.Instance.Get_ServerList_X().Count + "->");
			baseParam = new Dictionary<byte, object>();
			baseParam[57] = ModelManager.Instance.Get_userData_filed_X("UserId");
			baseParam[83] = ModelManager.Instance.Get_userData_filed_X("SummonerId");
			List<ServerInfo> list = ModelManager.Instance.Get_ServerList_X();
			int num = ModelManager.Instance.Get_currLoginServerIndex_X();
			if (list == null || num < 0 || num >= list.Count || list[num] == null)
			{
				throw new Exception("服务器列表数据错误");
			}
			baseParam[50] = list[num].serverid;
			return result;
		}

		private bool Game_GetTBCEnemyInfo(out Dictionary<byte, object> baseParam, out Dictionary<string, object> appendParam)
		{
			bool result = true;
			appendParam = null;
			Log.debug(" MobaClient ： 获取燃烧的远征敌方信息...");
			baseParam = new Dictionary<byte, object>();
			baseParam[57] = ModelManager.Instance.Get_userData_filed_X("UserId");
			baseParam[71] = ModelManager.Instance.Get_accountData_filed_X("AccountId");
			return result;
		}

		private bool Game_RestTBCEnemyInfo(out Dictionary<byte, object> baseParam, out Dictionary<string, object> appendParam)
		{
			bool result = true;
			appendParam = null;
			Log.debug(" MobaClient ： 重置燃烧的远征信息...");
			baseParam = new Dictionary<byte, object>();
			baseParam[57] = ModelManager.Instance.Get_userData_filed_X("UserId");
			baseParam[71] = ModelManager.Instance.Get_accountData_filed_X("AccountId");
			return result;
		}

		private bool Game_GetSummonersData(out Dictionary<byte, object> baseParam, out Dictionary<string, object> appendParam)
		{
			bool result = true;
			appendParam = null;
			baseParam = new Dictionary<byte, object>();
			baseParam[102] = ModelManager.Instance.Get_userData_filed_X("UserId");
			return result;
		}

		private bool Game_SaveTBCEnemyInfo(bool isWin, long battleId, List<TBCHeroStateInfo> myHeroStateInfo, List<TBCHeroStateInfo> targetHeroStateInfo, int levelID, out Dictionary<byte, object> baseParam, out Dictionary<string, object> appendParam)
		{
			bool result = true;
			appendParam = null;
			Log.debug(" MobaClient ： 保存燃烧的远征记录...");
			baseParam = new Dictionary<byte, object>();
			byte[] value = SerializeHelper.Serialize<List<TBCHeroStateInfo>>(myHeroStateInfo);
			byte[] value2 = SerializeHelper.Serialize<List<TBCHeroStateInfo>>(targetHeroStateInfo);
			baseParam[117] = value;
			baseParam[118] = value2;
			baseParam[57] = ModelManager.Instance.Get_userData_filed_X("UserId");
			baseParam[71] = ModelManager.Instance.Get_accountData_filed_X("AccountId");
			baseParam[83] = ModelManager.Instance.Get_userData_filed_X("SummonerId");
			baseParam[113] = battleId;
			baseParam[95] = isWin;
			baseParam[94] = levelID;
			return result;
		}

		private bool Game_TBCMyHeroStateInfo(out Dictionary<byte, object> baseParam, out Dictionary<string, object> appendParam)
		{
			bool result = true;
			appendParam = null;
			Log.debug(" MobaClient ： 获取燃烧远征我方英雄状态信息...");
			baseParam = new Dictionary<byte, object>();
			baseParam[57] = ModelManager.Instance.Get_userData_filed_X("UserId");
			baseParam[71] = ModelManager.Instance.Get_accountData_filed_X("AccountId");
			baseParam[83] = ModelManager.Instance.Get_userData_filed_X("SummonerId");
			return result;
		}

		private bool Game_ReceiveTBCReward(long battleId, int levelID, out Dictionary<byte, object> baseParam, out Dictionary<string, object> appendParam)
		{
			bool result = true;
			appendParam = null;
			Log.debug(" MobaClient ： 领取远征奖励...");
			baseParam = new Dictionary<byte, object>();
			baseParam[57] = ModelManager.Instance.Get_userData_filed_X("UserId");
			baseParam[71] = ModelManager.Instance.Get_accountData_filed_X("AccountId");
			baseParam[83] = ModelManager.Instance.Get_userData_filed_X("SummonerId");
			baseParam[113] = battleId;
			baseParam[94] = levelID;
			return result;
		}

		private bool Game_UpdateArenaDefTeam(string[] heroArr, int power, out Dictionary<byte, object> baseParam, out Dictionary<string, object> appendParam)
		{
			bool result = true;
			appendParam = null;
			Log.debug(" MobaClient ： 更新竞技场防守阵型...");
			baseParam = new Dictionary<byte, object>();
			baseParam[57] = ModelManager.Instance.Get_userData_filed_X("UserId");
			baseParam[71] = ModelManager.Instance.Get_accountData_filed_X("AccountId");
			baseParam[83] = ModelManager.Instance.Get_userData_filed_X("SummonerId");
			baseParam[88] = heroArr;
			baseParam[166] = power;
			return result;
		}

		private bool Game_GetArenaDefTeam(out Dictionary<byte, object> baseParam, out Dictionary<string, object> appendParam)
		{
			bool result = true;
			appendParam = null;
			Log.debug(" MobaClient ： 获取竞技场己方防守阵型...");
			baseParam = new Dictionary<byte, object>();
			baseParam[57] = ModelManager.Instance.Get_userData_filed_X("UserId");
			baseParam[71] = ModelManager.Instance.Get_accountData_filed_X("AccountId");
			baseParam[83] = ModelManager.Instance.Get_userData_filed_X("SummonerId");
			return result;
		}

		private bool Game_GetArenaEnemyInfo(out Dictionary<byte, object> baseParam, out Dictionary<string, object> appendParam)
		{
			bool result = true;
			appendParam = null;
			Log.debug(" MobaClient ： 获取竞技场敌人信息...");
			baseParam = new Dictionary<byte, object>();
			baseParam[57] = ModelManager.Instance.Get_userData_filed_X("UserId");
			baseParam[71] = ModelManager.Instance.Get_accountData_filed_X("AccountId");
			baseParam[83] = ModelManager.Instance.Get_userData_filed_X("SummonerId");
			return result;
		}

		private bool Game_ArenaAtcCheck(string targetId, out Dictionary<byte, object> baseParam, out Dictionary<string, object> appendParam)
		{
			bool result = true;
			appendParam = null;
			Log.debug(" MobaClient ： 发起竞技场战斗...");
			baseParam = new Dictionary<byte, object>();
			baseParam[57] = ModelManager.Instance.Get_userData_filed_X("UserId");
			baseParam[102] = targetId;
			baseParam[71] = ModelManager.Instance.Get_accountData_filed_X("AccountId");
			baseParam[83] = ModelManager.Instance.Get_userData_filed_X("SummonerId");
			return result;
		}

		private bool Game_UploadArenaAtc(string targetId, bool isWin, string[] heroArr, ArenaData arenaData, string targetHeroInfo, string myHeroInfo, bool isRevenge, long arenaLogId, out Dictionary<byte, object> baseParam, out Dictionary<string, object> appendParam)
		{
			bool result = true;
			appendParam = null;
			Log.debug(" MobaClient ： 上传竞技场战斗结果...");
			baseParam = new Dictionary<byte, object>();
			if (arenaData == null)
			{
				return false;
			}
			byte[] value = SerializeHelper.Serialize<ArenaData>(arenaData);
			baseParam[57] = ModelManager.Instance.Get_userData_filed_X("UserId");
			baseParam[95] = isWin;
			baseParam[119] = value;
			baseParam[88] = heroArr;
			baseParam[198] = targetHeroInfo;
			baseParam[199] = myHeroInfo;
			baseParam[200] = isRevenge;
			baseParam[201] = arenaLogId;
			return result;
		}

		private bool Game_ArenaFightLog(out Dictionary<byte, object> baseParam, out Dictionary<string, object> appendParam)
		{
			bool result = true;
			appendParam = null;
			Log.debug(" MobaClient ： 获取战斗记录...");
			baseParam = new Dictionary<byte, object>();
			baseParam[57] = ModelManager.Instance.Get_userData_filed_X("UserId");
			return result;
		}

		private bool Game_GetArenaState(out Dictionary<byte, object> baseParam, out Dictionary<string, object> appendParam)
		{
			bool result = true;
			appendParam = null;
			Log.debug(" MobaClient ： 获取竞技场挑战次数和时间等相关信息...");
			baseParam = new Dictionary<byte, object>();
			baseParam[57] = ModelManager.Instance.Get_userData_filed_X("UserId");
			return result;
		}

		private bool Game_RestArenaCount(out Dictionary<byte, object> baseParam, out Dictionary<string, object> appendParam)
		{
			bool result = true;
			appendParam = null;
			Log.debug(" MobaClient ： 重置竞技场挑战次数...");
			baseParam = new Dictionary<byte, object>();
			baseParam[57] = ModelManager.Instance.Get_userData_filed_X("UserId");
			return result;
		}

		private bool Game_RestArenaCD(out Dictionary<byte, object> baseParam, out Dictionary<string, object> appendParam)
		{
			bool result = true;
			appendParam = null;
			Log.debug(" MobaClient ： 重置竞技场挑战CD时间...");
			baseParam = new Dictionary<byte, object>();
			baseParam[57] = ModelManager.Instance.Get_userData_filed_X("UserId");
			return result;
		}

		private bool Game_ArenaRank(out Dictionary<byte, object> baseParam, out Dictionary<string, object> appendParam)
		{
			bool result = true;
			appendParam = null;
			Log.debug(" MobaClient ：获取竞技场排行榜 ...");
			baseParam = new Dictionary<byte, object>();
			baseParam[57] = ModelManager.Instance.Get_userData_filed_X("UserId");
			return result;
		}

		private bool Game_LuckyDrawState(out Dictionary<byte, object> baseParam, out Dictionary<string, object> appendParam)
		{
			bool result = true;
			appendParam = null;
			Log.debug(" MobaClient ：获取抽奖状态 ...");
			baseParam = new Dictionary<byte, object>();
			baseParam[57] = ModelManager.Instance.Get_userData_filed_X("UserId");
			return result;
		}

		private bool Game_LuckyDraw(byte drawtype, byte count, bool isFree, out Dictionary<byte, object> baseParam, out Dictionary<string, object> appendParam)
		{
			bool result = true;
			appendParam = null;
			Log.debug(" MobaClient ：幸运抽奖 ...");
			baseParam = new Dictionary<byte, object>();
			baseParam[57] = ModelManager.Instance.Get_userData_filed_X("UserId");
			baseParam[101] = count;
			baseParam[10] = drawtype;
			baseParam[122] = isFree;
			return result;
		}

		private bool Game_GetShopInfo(byte type, out Dictionary<byte, object> baseParam, out Dictionary<string, object> appendParam)
		{
			bool result = true;
			appendParam = null;
			Log.debug(" MobaClient ：根据商店类型获取商店货物信息 ...");
			baseParam = new Dictionary<byte, object>();
			baseParam[57] = ModelManager.Instance.Get_userData_filed_X("UserId");
			baseParam[10] = type;
			return result;
		}

		private bool Game_GetShopNew(out Dictionary<byte, object> baseParam, out Dictionary<string, object> appendParam)
		{
			bool result = true;
			appendParam = null;
			Log.debug(" MobaClient ：根据商店类型获取商店货物信息 ...");
			baseParam = new Dictionary<byte, object>();
			baseParam[10] = null;
			return result;
		}

		private bool Game_GetShopVersion(out Dictionary<byte, object> baseParam, out Dictionary<string, object> appendParam)
		{
			appendParam = null;
			baseParam = null;
			return true;
		}

		private bool Game_BuyShopGoods(int storegoods, byte costtype, int count, out Dictionary<byte, object> baseParam, out Dictionary<string, object> appendParam)
		{
			bool result = true;
			appendParam = null;
			Log.debug(" MobaClient ：指定商店类型购买指定货物 ...");
			baseParam = new Dictionary<byte, object>();
			baseParam[57] = ModelManager.Instance.Get_userData_filed_X("UserId");
			baseParam[126] = storegoods;
			baseParam[234] = costtype;
			baseParam[101] = count;
			return result;
		}

		private bool Game_BuyShopGoodsNew(byte moneyType, int id, int count, long sumId, long heroId, out Dictionary<byte, object> baseParam, out Dictionary<string, object> appendParam)
		{
			bool result = true;
			appendParam = null;
			Log.debug(" MobaClient ：指定商店类型购买指定货物 ...");
			baseParam = new Dictionary<byte, object>();
			baseParam[57] = ModelManager.Instance.Get_userData_filed_X("UserId");
			baseParam[126] = id;
			baseParam[234] = moneyType;
			baseParam[101] = count;
			baseParam[83] = sumId;
			baseParam[89] = heroId;
			return result;
		}

		private bool Game_GameReport(GameReportData _data, out Dictionary<byte, object> baseParam, out Dictionary<string, object> appendParam)
		{
			bool result = true;
			appendParam = null;
			baseParam = new Dictionary<byte, object>();
			baseParam[84] = SerializeHelper.Serialize<GameReportData>(_data);
			return result;
		}

		private bool Game_DealGameReport(out Dictionary<byte, object> baseParam, out Dictionary<string, object> appendParam)
		{
			baseParam = null;
			appendParam = null;
			return true;
		}

		private bool Game_GetSummSkinList(long sumId, out Dictionary<byte, object> baseParam, out Dictionary<string, object> appendParam)
		{
			bool result = true;
			appendParam = null;
			baseParam = new Dictionary<byte, object>();
			baseParam[83] = sumId;
			return result;
		}

		private bool Game_BuySkin(long sumId, long heroId, short skinId, out Dictionary<byte, object> baseParam, out Dictionary<string, object> appendParam)
		{
			bool result = true;
			appendParam = null;
			Log.debug(" MobaClient ：指定商店类型购买指定货物 ...");
			baseParam = new Dictionary<byte, object>();
			baseParam[57] = ModelManager.Instance.Get_userData_filed_X("UserId");
			baseParam[83] = sumId;
			baseParam[89] = heroId;
			baseParam[208] = skinId;
			return result;
		}

		private bool Game_AppStoreCharge(string data, out Dictionary<byte, object> baseParam, out Dictionary<string, object> appendParam)
		{
			bool result = true;
			appendParam = null;
			Log.debug(" MobaClient ：IAP回执验证 ...");
			baseParam = new Dictionary<byte, object>();
			baseParam[84] = data;
			return result;
		}

		private bool Game_RestShop(byte type, out Dictionary<byte, object> baseParam, out Dictionary<string, object> appendParam)
		{
			bool result = true;
			appendParam = null;
			Log.debug(" MobaClient ：重置指定类型商店 ...");
			baseParam = new Dictionary<byte, object>();
			baseParam[57] = ModelManager.Instance.Get_userData_filed_X("UserId");
			baseParam[10] = type;
			return result;
		}

		private bool Game_GetTaskList(int AchieveId, out Dictionary<byte, object> baseParam, out Dictionary<string, object> appendParam)
		{
			bool result = true;
			appendParam = null;
			Log.debug(" MobaClient ：获取任务列表 ...");
			baseParam = new Dictionary<byte, object>();
			baseParam[252] = AchieveId;
			return result;
		}

		private bool Game_ShowDailyTask(out Dictionary<byte, object> baseParam, out Dictionary<string, object> appendParam)
		{
			bool result = true;
			appendParam = null;
			Log.debug(" MobaClient ：获取任务列表 ...");
			baseParam = new Dictionary<byte, object>();
			return result;
		}

		private bool Game_CompleteTask(int taskId, out Dictionary<byte, object> baseParam, out Dictionary<string, object> appendParam)
		{
			bool result = true;
			appendParam = null;
			Log.debug(" MobaClient ：完成任务（进度型任务每增加一个进度都需要调用一次该方法） ...");
			baseParam = new Dictionary<byte, object>();
			baseParam[57] = ModelManager.Instance.Get_userData_filed_X("UserId");
			baseParam[123] = taskId;
			return result;
		}

		private bool Game_GetAchieveTaskAward(int taskId, out Dictionary<byte, object> baseParam, out Dictionary<string, object> appendParam)
		{
			bool result = true;
			appendParam = null;
			baseParam = new Dictionary<byte, object>();
			baseParam[123] = taskId;
			return result;
		}

		private bool Game_GetDailyTaskAward(int taskId, out Dictionary<byte, object> baseParam, out Dictionary<string, object> appendParam)
		{
			bool result = true;
			appendParam = null;
			baseParam = new Dictionary<byte, object>();
			baseParam[123] = taskId;
			return result;
		}

		private bool Game_GetSignDay(out Dictionary<byte, object> baseParam, out Dictionary<string, object> appendParam)
		{
			bool result = true;
			appendParam = null;
			Log.debug(" MobaClient ：签到 ...");
			baseParam = new Dictionary<byte, object>();
			return result;
		}

		private bool Game_SignDay(out Dictionary<byte, object> baseParam, out Dictionary<string, object> appendParam)
		{
			bool result = true;
			appendParam = null;
			Log.debug(" MobaClient ：签到 ...");
			baseParam = new Dictionary<byte, object>();
			baseParam[57] = ModelManager.Instance.Get_userData_filed_X("UserId");
			return result;
		}

		private bool Game_TeachingGuide(int dataType, int dataValue, out Dictionary<byte, object> baseParam, out Dictionary<string, object> appendParam)
		{
			bool result = true;
			appendParam = null;
			baseParam = new Dictionary<byte, object>();
			baseParam[1] = dataValue;
			baseParam[0] = dataType;
			return result;
		}

		private bool Game_GetActivityAward(string taskID, out Dictionary<byte, object> baseParam, out Dictionary<string, object> appendParam)
		{
			bool result = true;
			appendParam = null;
			baseParam = new Dictionary<byte, object>();
			baseParam[91] = taskID;
			return result;
		}

		private bool Game_GetActivityTask(out Dictionary<byte, object> baseParam, out Dictionary<string, object> appendParam)
		{
			bool result = true;
			appendParam = null;
			baseParam = null;
			return result;
		}

		private bool Game_GetNoticeBoard(out Dictionary<byte, object> baseParam, out Dictionary<string, object> appendParam)
		{
			bool result = true;
			appendParam = null;
			baseParam = null;
			return result;
		}

		private bool Game_GetActivityTask(int activityID, out Dictionary<byte, object> baseParam, out Dictionary<string, object> appendParam)
		{
			bool result = true;
			appendParam = null;
			baseParam = new Dictionary<byte, object>();
			baseParam[125] = activityID;
			return result;
		}

		private bool Game_GetNoticeBoard(int noticeID, out Dictionary<byte, object> baseParam, out Dictionary<string, object> appendParam)
		{
			bool result = true;
			appendParam = null;
			baseParam = new Dictionary<byte, object>();
			baseParam[125] = noticeID;
			return result;
		}

		private bool Game_VipAttendance(out Dictionary<byte, object> baseParam, out Dictionary<string, object> appendParam)
		{
			bool result = true;
			appendParam = null;
			Log.debug(" MobaClient ：签到 ...");
			baseParam = new Dictionary<byte, object>();
			baseParam[57] = ModelManager.Instance.Get_userData_filed_X("UserId");
			return result;
		}

		private bool Game_ModfiyNickName(string nickName, out Dictionary<byte, object> baseParam, out Dictionary<string, object> appendParam)
		{
			bool result = true;
			appendParam = null;
			Log.debug(" MobaClient ：修改召唤师昵称 ...");
			baseParam = new Dictionary<byte, object>();
			baseParam[57] = ModelManager.Instance.Get_userData_filed_X("UserId");
			baseParam[59] = nickName;
			return result;
		}

		private bool Game_ModfiyAvatar(int iconId, byte type, out Dictionary<byte, object> baseParam, out Dictionary<string, object> appendParam)
		{
			bool result = true;
			appendParam = null;
			Log.debug(" MobaClient ：修改召唤师头像/头像框 ...");
			baseParam = new Dictionary<byte, object>();
			baseParam[57] = ModelManager.Instance.Get_userData_filed_X("UserId");
			baseParam[58] = iconId;
			baseParam[10] = type;
			return result;
		}

		private bool Game_ShowIconFrame(int iconId, byte type, out Dictionary<byte, object> baseParam, out Dictionary<string, object> appendParam)
		{
			bool result = true;
			appendParam = null;
			Log.debug(" MobaClient ：修改召唤师头像/头像框的新获得的标记 ...");
			baseParam = new Dictionary<byte, object>();
			baseParam[58] = iconId;
			baseParam[10] = type;
			return result;
		}

		private bool Game_SellProps(List<SellProsData> saleList, out Dictionary<byte, object> baseParam, out Dictionary<string, object> appendParam)
		{
			bool result = true;
			appendParam = null;
			Log.debug(" MobaClient ：出售道具 ...");
			baseParam = new Dictionary<byte, object>();
			byte[] value = SerializeHelper.Serialize<List<SellProsData>>(saleList);
			baseParam[57] = ModelManager.Instance.Get_userData_filed_X("UserId");
			baseParam[133] = value;
			return result;
		}

		private bool Game_ExchangeByDimond(byte type, int count, out Dictionary<byte, object> baseParam, out Dictionary<string, object> appendParam)
		{
			bool result = true;
			appendParam = null;
			Log.debug(" MobaClient ：购买金币/体力...");
			baseParam = new Dictionary<byte, object>();
			baseParam[57] = ModelManager.Instance.Get_userData_filed_X("UserId");
			baseParam[10] = type;
			baseParam[101] = count;
			return result;
		}

		private bool Game_GetMailList(out Dictionary<byte, object> baseParam, out Dictionary<string, object> appendParam)
		{
			bool result = true;
			appendParam = null;
			Log.debug(" MobaClient ：获取邮件列表...");
			baseParam = new Dictionary<byte, object>();
			baseParam[57] = ModelManager.Instance.Get_userData_filed_X("UserId");
			return result;
		}

		private bool Game_ReceiveMailAttachment(long mailId, out Dictionary<byte, object> baseParam, out Dictionary<string, object> appendParam)
		{
			bool result = true;
			appendParam = null;
			Log.debug(" MobaClient ： 领取邮件附件...");
			baseParam = new Dictionary<byte, object>();
			baseParam[57] = ModelManager.Instance.Get_userData_filed_X("UserId");
			baseParam[73] = mailId;
			return result;
		}

		private bool Game_CreateUnion(string unionName, int unionType, int joinlevel, int iconid, int iconborderid, out Dictionary<byte, object> baseParam, out Dictionary<string, object> appendParam)
		{
			bool result = true;
			appendParam = null;
			Log.debug(" MobaClient ： 创建工会...");
			baseParam = new Dictionary<byte, object>();
			baseParam[57] = ModelManager.Instance.Get_userData_filed_X("UserId");
			baseParam[138] = unionType;
			baseParam[139] = joinlevel;
			baseParam[140] = iconid;
			baseParam[141] = iconborderid;
			return result;
		}

		private bool Game_GetUnionInfo(out Dictionary<byte, object> baseParam, out Dictionary<string, object> appendParam)
		{
			bool result = true;
			appendParam = null;
			Log.debug(" MobaClient ： 获取工会信息...");
			baseParam = new Dictionary<byte, object>();
			baseParam[57] = ModelManager.Instance.Get_userData_filed_X("UserId");
			baseParam[142] = ModelManager.Instance.Get_userData_filed_X("UnionId");
			return result;
		}

		private bool Game_DissolveUnion(out Dictionary<byte, object> baseParam, out Dictionary<string, object> appendParam)
		{
			bool result = true;
			appendParam = null;
			Log.debug(" MobaClient ： 解散工会...");
			baseParam = new Dictionary<byte, object>();
			baseParam[57] = ModelManager.Instance.Get_userData_filed_X("UserId");
			baseParam[142] = ModelManager.Instance.Get_userData_filed_X("UnionId");
			return result;
		}

		private bool Game_SearchUnion(int unionId, out Dictionary<byte, object> baseParam, out Dictionary<string, object> appendParam)
		{
			bool result = true;
			appendParam = null;
			Log.debug(" MobaClient ： 查找工会...");
			baseParam = new Dictionary<byte, object>();
			baseParam[57] = ModelManager.Instance.Get_userData_filed_X("UserId");
			baseParam[142] = unionId;
			return result;
		}

		private bool Game_JoinUnion(int unionId, out Dictionary<byte, object> baseParam, out Dictionary<string, object> appendParam)
		{
			bool result = true;
			appendParam = null;
			Log.debug(" MobaClient ： 申请/加入工会...");
			baseParam = new Dictionary<byte, object>();
			baseParam[57] = ModelManager.Instance.Get_userData_filed_X("UserId");
			baseParam[142] = unionId;
			return result;
		}

		private bool Game_KickUnion(string targetId, out Dictionary<byte, object> baseParam, out Dictionary<string, object> appendParam)
		{
			bool result = true;
			appendParam = null;
			Log.debug(" MobaClient ： 踢出工会...");
			baseParam = new Dictionary<byte, object>();
			baseParam[57] = ModelManager.Instance.Get_userData_filed_X("UserId");
			baseParam[142] = ModelManager.Instance.Get_userData_filed_X("UnionId");
			baseParam[102] = targetId;
			return result;
		}

		private bool Game_LeaveUnion(out Dictionary<byte, object> baseParam, out Dictionary<string, object> appendParam)
		{
			bool result = true;
			appendParam = null;
			Log.debug(" MobaClient ： 退出工会...");
			baseParam = new Dictionary<byte, object>();
			baseParam[57] = ModelManager.Instance.Get_userData_filed_X("UserId");
			baseParam[142] = ModelManager.Instance.Get_userData_filed_X("UnionId");
			return result;
		}

		private bool Game_ModifyUnionSetting(int unionType, int joinlevel, int iconid, int iconborderid, out Dictionary<byte, object> baseParam, out Dictionary<string, object> appendParam)
		{
			bool result = true;
			appendParam = null;
			Log.debug(" MobaClient ： 修改工会设置...");
			baseParam = new Dictionary<byte, object>();
			baseParam[57] = ModelManager.Instance.Get_userData_filed_X("UserId");
			baseParam[142] = ModelManager.Instance.Get_userData_filed_X("UnionId");
			baseParam[138] = unionType;
			baseParam[139] = joinlevel;
			baseParam[140] = iconid;
			baseParam[141] = iconborderid;
			return result;
		}

		private bool Game_GetUnionList(out Dictionary<byte, object> baseParam, out Dictionary<string, object> appendParam)
		{
			bool result = true;
			appendParam = null;
			Log.debug(" MobaClient ： 获取工会列表...");
			baseParam = new Dictionary<byte, object>();
			baseParam[57] = ModelManager.Instance.Get_userData_filed_X("UserId");
			return result;
		}

		private bool Game_GetUnionLogs(out Dictionary<byte, object> baseParam, out Dictionary<string, object> appendParam)
		{
			bool result = true;
			appendParam = null;
			Log.debug(" MobaClient ： 获取工会日志...");
			baseParam = new Dictionary<byte, object>();
			baseParam[57] = ModelManager.Instance.Get_userData_filed_X("UserId");
			baseParam[142] = ModelManager.Instance.Get_userData_filed_X("UnionId");
			return result;
		}

		private bool Game_GetMemberList(out Dictionary<byte, object> baseParam, out Dictionary<string, object> appendParam)
		{
			bool result = true;
			appendParam = null;
			Log.debug(" MobaClient ： 获取成员列表...");
			baseParam = new Dictionary<byte, object>();
			baseParam[57] = ModelManager.Instance.Get_userData_filed_X("UserId");
			baseParam[142] = ModelManager.Instance.Get_userData_filed_X("UnionId");
			return result;
		}

		private bool Game_UpgradeMaster(string targetId, out Dictionary<byte, object> baseParam, out Dictionary<string, object> appendParam)
		{
			bool result = true;
			appendParam = null;
			Log.debug(" MobaClient ： 提升会长...");
			baseParam = new Dictionary<byte, object>();
			baseParam[57] = ModelManager.Instance.Get_userData_filed_X("UserId");
			baseParam[142] = ModelManager.Instance.Get_userData_filed_X("UnionId");
			baseParam[145] = targetId;
			return result;
		}

		private bool Game_AppointElder(string targetId, out Dictionary<byte, object> baseParam, out Dictionary<string, object> appendParam)
		{
			bool result = true;
			appendParam = null;
			Log.debug(" MobaClient ： 提升长老...");
			baseParam = new Dictionary<byte, object>();
			baseParam[57] = ModelManager.Instance.Get_userData_filed_X("UserId");
			baseParam[142] = ModelManager.Instance.Get_userData_filed_X("UnionId");
			baseParam[145] = targetId;
			return result;
		}

		private bool Game_UnAppointElder(string targetId, out Dictionary<byte, object> baseParam, out Dictionary<string, object> appendParam)
		{
			bool result = true;
			appendParam = null;
			Log.debug(" MobaClient ： 撤销长老...");
			baseParam = new Dictionary<byte, object>();
			baseParam[57] = ModelManager.Instance.Get_userData_filed_X("UserId");
			baseParam[142] = ModelManager.Instance.Get_userData_filed_X("UnionId");
			baseParam[145] = targetId;
			return result;
		}

		private bool Game_GetUnionRequestList(out Dictionary<byte, object> baseParam, out Dictionary<string, object> appendParam)
		{
			bool result = true;
			appendParam = null;
			Log.debug(" MobaClient ： 获取申请列表...");
			baseParam = new Dictionary<byte, object>();
			baseParam[57] = ModelManager.Instance.Get_userData_filed_X("UserId");
			baseParam[142] = ModelManager.Instance.Get_userData_filed_X("UnionId");
			return result;
		}

		private bool Game_DisposeUnionRequest(string targetId, bool isPass, out Dictionary<byte, object> baseParam, out Dictionary<string, object> appendParam)
		{
			bool result = true;
			appendParam = null;
			Log.debug(" MobaClient ： 处理工会申请...");
			baseParam = new Dictionary<byte, object>();
			baseParam[57] = ModelManager.Instance.Get_userData_filed_X("UserId");
			baseParam[142] = ModelManager.Instance.Get_userData_filed_X("UnionId");
			baseParam[102] = targetId;
			baseParam[150] = isPass;
			return result;
		}

		private bool Game_Enchant(long heroId, byte postion, List<EnchantData> data, out Dictionary<byte, object> baseParam, out Dictionary<string, object> appendParam)
		{
			bool result = true;
			appendParam = null;
			Log.debug(" MobaClient ： 装备附魔...");
			baseParam = new Dictionary<byte, object>();
			baseParam[57] = ModelManager.Instance.Get_userData_filed_X("UserId");
			baseParam[89] = heroId;
			baseParam[92] = postion;
			byte[] value = SerializeHelper.Serialize<List<EnchantData>>(data);
			baseParam[152] = value;
			return result;
		}

		private bool Game_SweepBattle(int count, string battleId, string sceneId, out Dictionary<byte, object> baseParam, out Dictionary<string, object> appendParam)
		{
			bool result = true;
			appendParam = null;
			Log.debug(" MobaClient ： 副本扫荡...");
			baseParam = new Dictionary<byte, object>();
			baseParam[57] = ModelManager.Instance.Get_userData_filed_X("UserId");
			baseParam[113] = battleId;
			baseParam[94] = sceneId;
			baseParam[101] = count;
			return result;
		}

		private bool Game_ModifyEmailState(long emailId, out Dictionary<byte, object> baseParam, out Dictionary<string, object> appendParam)
		{
			bool result = true;
			appendParam = null;
			Log.debug(" MobaClient ： 修改邮件读取状态...");
			baseParam = new Dictionary<byte, object>();
			baseParam[57] = ModelManager.Instance.Get_userData_filed_X("UserId");
			baseParam[73] = emailId;
			return result;
		}

		private bool Game_UpdateSmallMeleeTeam(string[] heros, out Dictionary<byte, object> baseParam, out Dictionary<string, object> appendParam)
		{
			bool result = true;
			appendParam = null;
			Log.debug(" MobaClient ： 更新小乱斗防御阵型...");
			if (heros == null || heros.Length < 1)
			{
				throw new Exception(" MobaClient ： 传入阵型不符合规范！");
			}
			baseParam = new Dictionary<byte, object>();
			baseParam[57] = ModelManager.Instance.Get_userData_filed_X("UserId");
			baseParam[88] = heros;
			return result;
		}

		private bool Game_GetSmallMeleeInfo(out Dictionary<byte, object> baseParam, out Dictionary<string, object> appendParam)
		{
			bool result = true;
			appendParam = null;
			Log.debug(" MobaClient ： 获取小乱斗个人信息...");
			baseParam = new Dictionary<byte, object>();
			baseParam[57] = ModelManager.Instance.Get_userData_filed_X("UserId");
			return result;
		}

		private bool Game_GetTalentInfoByUid(string userid, out Dictionary<byte, object> baseParam, out Dictionary<string, object> appendParam)
		{
			bool result = true;
			appendParam = null;
			Log.debug(" MobaClient ： 获取天赋信息...");
			baseParam = new Dictionary<byte, object>();
			baseParam[57] = userid;
			return result;
		}

		private bool Game_ChangeKillTitanTeam(List<KillTitanModel> heros, out Dictionary<byte, object> baseParam, out Dictionary<string, object> appendParam)
		{
			bool result = true;
			appendParam = null;
			Log.debug(" MobaClient ： 修改挑战大魔王阵型...");
			baseParam = new Dictionary<byte, object>();
			baseParam[57] = ModelManager.Instance.Get_userData_filed_X("UserId");
			byte[] value = SerializeHelper.Serialize<List<KillTitanModel>>(heros);
			baseParam[88] = value;
			return result;
		}

		private bool Game_GetKillTitanData(out Dictionary<byte, object> baseParam, out Dictionary<string, object> appendParam)
		{
			bool result = true;
			appendParam = null;
			Log.debug(" MobaClient ： 获取挑战大魔王基本信息...");
			baseParam = new Dictionary<byte, object>();
			baseParam[57] = ModelManager.Instance.Get_userData_filed_X("UserId");
			return result;
		}

		private bool Game_ReceiveKillTitanReward(out Dictionary<byte, object> baseParam, out Dictionary<string, object> appendParam)
		{
			bool result = true;
			appendParam = null;
			Log.debug(" MobaClient ： 领取挑战大魔王的奖励...");
			baseParam = new Dictionary<byte, object>();
			baseParam[57] = ModelManager.Instance.Get_userData_filed_X("UserId");
			return result;
		}

		private bool Game_AddKillTitanSnapshot(string modelid, int position, out Dictionary<byte, object> baseParam, out Dictionary<string, object> appendParam)
		{
			bool result = true;
			appendParam = null;
			Log.debug(" MobaClient ： 添加挑战大魔王英雄...");
			baseParam = new Dictionary<byte, object>();
			baseParam[57] = ModelManager.Instance.Get_userData_filed_X("UserId");
			baseParam[88] = modelid;
			baseParam[92] = position;
			return result;
		}

		private bool Game_GetKillTitanSnapshot(out Dictionary<byte, object> baseParam, out Dictionary<string, object> appendParam)
		{
			bool result = true;
			appendParam = null;
			Log.debug(" MobaClient ： 重构后的获取挑战大魔王信息（如果有快照则包含最近的一个快照）...");
			baseParam = new Dictionary<byte, object>();
			baseParam[57] = ModelManager.Instance.Get_userData_filed_X("UserId");
			return result;
		}

		private bool Game_ReceiveSnapshotAward(out Dictionary<byte, object> baseParam, out Dictionary<string, object> appendParam)
		{
			bool result = true;
			appendParam = null;
			Log.debug(" MobaClient ： 重构后的挑战大魔王奖励领取...");
			baseParam = new Dictionary<byte, object>();
			baseParam[57] = ModelManager.Instance.Get_userData_filed_X("UserId");
			return result;
		}

		private bool Game_BuySpecialShopOwn(byte shopType, out Dictionary<byte, object> baseParam, out Dictionary<string, object> appendParam)
		{
			bool result = true;
			appendParam = null;
			Log.debug(" MobaClient ： 购买特殊商店常驻...");
			baseParam = new Dictionary<byte, object>();
			baseParam[57] = ModelManager.Instance.Get_userData_filed_X("UserId");
			baseParam[10] = shopType;
			return result;
		}

		private bool Game_RestTodayBattlesCount(string battleId, string sceneId, out Dictionary<byte, object> baseParam, out Dictionary<string, object> appendParam)
		{
			bool result = true;
			appendParam = null;
			Log.debug(" MobaClient ： 重置关卡可挑战次数...");
			baseParam = new Dictionary<byte, object>();
			baseParam[57] = ModelManager.Instance.Get_userData_filed_X("UserId");
			baseParam[94] = sceneId;
			baseParam[157] = battleId;
			return result;
		}

		private bool Game_Friend_ApplyAddFriend(long targetSummid, out Dictionary<byte, object> baseParam, out Dictionary<string, object> appendParam)
		{
			bool result = true;
			appendParam = null;
			Log.debug(" MobaClient ： 申请添加好友...");
			baseParam = new Dictionary<byte, object>();
			baseParam[83] = ModelManager.Instance.Get_userData_filed_X("SummonerId");
			baseParam[102] = targetSummid;
			return result;
		}

		private bool Game_SayGoodToSomeOne(string userId, out Dictionary<byte, object> baseParam, out Dictionary<string, object> appendParam)
		{
			bool result = true;
			appendParam = null;
			Log.debug(" MobaClient ： 点赞...");
			baseParam = new Dictionary<byte, object>();
			baseParam[102] = userId;
			return result;
		}

		private bool Game_Friend_GetFriendList(out Dictionary<byte, object> baseParam, out Dictionary<string, object> appendParam)
		{
			bool result = true;
			appendParam = null;
			Log.debug(" MobaClient ： 获取好友列表...");
			baseParam = new Dictionary<byte, object>();
			baseParam[83] = ModelManager.Instance.Get_userData_filed_X("SummonerId");
			return result;
		}

		private bool Game_Friend_RaderFindFriend(out Dictionary<byte, object> baseParam, out Dictionary<string, object> appendParam)
		{
			bool result = true;
			appendParam = null;
			baseParam = new Dictionary<byte, object>();
			return result;
		}

		private bool Game_SetPushState(bool isOpen, out Dictionary<byte, object> baseParam, out Dictionary<string, object> appendParam)
		{
			bool result = true;
			appendParam = null;
			baseParam = new Dictionary<byte, object>();
			baseParam[57] = ModelManager.Instance.Get_userData_X().UserId;
			baseParam[215] = isOpen;
			return result;
		}

		private bool Game_Friend_ModifyFriendStatus(long targetSummid, byte operatType, out Dictionary<byte, object> baseParam, out Dictionary<string, object> appendParam)
		{
			bool result = true;
			appendParam = null;
			Log.debug(" MobaClient ： 修改好友状态...");
			baseParam = new Dictionary<byte, object>();
			baseParam[83] = ModelManager.Instance.Get_userData_filed_X("SummonerId");
			baseParam[102] = targetSummid;
			baseParam[10] = operatType;
			if (operatType == 5)
			{
				ModelManager.Instance.Get_FriendDataList_X().RemoveAll((FriendData obj) => obj.TargetId == targetSummid);
			}
			return result;
		}

		private bool Game_GetFriendMessages(out Dictionary<byte, object> baseParam, out Dictionary<string, object> appendParam)
		{
			bool result = true;
			appendParam = null;
			Log.debug(" MobaClient ： 获取好友留言消息...");
			baseParam = new Dictionary<byte, object>();
			baseParam[83] = ModelManager.Instance.Get_userData_filed_X("SummonerId");
			return result;
		}

		private bool Game_GetUserInfoBySummId(long targetSummId, out Dictionary<byte, object> baseParam, out Dictionary<string, object> appendParam)
		{
			bool result = true;
			appendParam = null;
			Log.debug(" MobaClient ： 获取好友留言消息...");
			baseParam = new Dictionary<byte, object>();
			baseParam[102] = targetSummId;
			return result;
		}

		private bool Game_RoomsManager(MobaTeamRoomCode code, byte roomType, string targetId, string roomId, FriendGameType frienGameType, string mapId, int peopleMax, string targetId2, out Dictionary<byte, object> baseParam, out Dictionary<string, object> appendParam)
		{
			bool result = true;
			appendParam = null;
			Log.debug(" MobaClient ： 开黑自定义房间管理...");
			baseParam = new Dictionary<byte, object>();
			baseParam[57] = ModelManager.Instance.Get_userData_filed_X("UserId");
			baseParam[102] = targetId;
			baseParam[183] = targetId2;
			baseParam[177] = roomId;
			baseParam[176] = roomType;
			baseParam[175] = (byte)code;
			baseParam[178] = (byte)frienGameType;
			baseParam[179] = mapId;
			baseParam[180] = peopleMax;
			return result;
		}

		private bool TeamRoom_Common(byte roomType, string targetId, string roomId, FriendGameType frienGameType, string mapId, int peopleMax, string targetId2, out Dictionary<byte, object> baseParam, out Dictionary<string, object> appendParam)
		{
			bool result = true;
			appendParam = null;
			Log.debug(" MobaClient ： 开黑自定义房间管理...");
			baseParam = new Dictionary<byte, object>();
			baseParam[57] = ModelManager.Instance.Get_userData_filed_X("UserId");
			baseParam[102] = targetId;
			baseParam[183] = targetId2;
			baseParam[177] = roomId;
			baseParam[176] = roomType;
			baseParam[178] = (byte)frienGameType;
			baseParam[179] = mapId;
			baseParam[180] = peopleMax;
			return result;
		}

		private bool TeamRoom_InviteJoinRoom(long targetSummId, string roomId, out Dictionary<byte, object> baseParam, out Dictionary<string, object> appendParam)
		{
			bool result = true;
			appendParam = null;
			Log.debug(" MobaClient ： 开黑自定义房间管理...");
			baseParam = new Dictionary<byte, object>();
			baseParam[57] = ModelManager.Instance.Get_userData_filed_X("UserId");
			baseParam[102] = targetSummId;
			baseParam[177] = roomId;
			return result;
		}

		private bool MobaTeamRoomCode_Room_RefuseJoinInvite(long targetSummId, string roomId, out Dictionary<byte, object> baseParam, out Dictionary<string, object> appendParam)
		{
			bool result = true;
			appendParam = null;
			Log.debug(" MobaClient ： MobaTeamRoomCode_Room_RefuseJoinInvite...");
			baseParam = new Dictionary<byte, object>();
			baseParam[83] = targetSummId;
			baseParam[177] = roomId;
			return result;
		}

		private bool Game_GetMedalData(out Dictionary<byte, object> baseParam, out Dictionary<string, object> appendParam)
		{
			bool result = true;
			appendParam = null;
			Log.debug(" MobaClient ：获取勋章信息 ...");
			baseParam = new Dictionary<byte, object>();
			baseParam[57] = ModelManager.Instance.Get_userData_filed_X("UserId");
			return result;
		}

		private bool Game_MedalNotice(out Dictionary<byte, object> baseParam, out Dictionary<string, object> appendParam)
		{
			bool result = true;
			appendParam = null;
			baseParam = new Dictionary<byte, object>();
			baseParam[57] = string.Empty;
			return result;
		}

		private bool Game_GetMyArenaRank(out Dictionary<byte, object> baseParam, out Dictionary<string, object> appendParam)
		{
			bool result = true;
			appendParam = null;
			baseParam = new Dictionary<byte, object>();
			baseParam[57] = ModelManager.Instance.Get_userData_filed_X("UserId");
			return result;
		}

		private bool Game_GetArenaRankList(out Dictionary<byte, object> baseParam, out Dictionary<string, object> appendParam)
		{
			bool result = true;
			appendParam = null;
			baseParam = new Dictionary<byte, object>();
			return result;
		}

		private bool Game_GetSummonerLadderRankList(out Dictionary<byte, object> baseParam, out Dictionary<string, object> appendParam)
		{
			bool result = true;
			appendParam = null;
			baseParam = new Dictionary<byte, object>();
			return result;
		}

		private bool Game_GetMagicBottleRankList(out Dictionary<byte, object> baseParam, out Dictionary<string, object> appendParam)
		{
			UserData userData = ModelManager.Instance.Get_userData_X();
			bool result = true;
			appendParam = null;
			baseParam = new Dictionary<byte, object>();
			baseParam[57] = ModelManager.Instance.Get_userData_X().UserId;
			baseParam[59] = userData.NickName;
			return result;
		}

		private bool Game_GetCharmRankList(out Dictionary<byte, object> baseParam, out Dictionary<string, object> appendParam)
		{
			bool result = true;
			appendParam = null;
			baseParam = new Dictionary<byte, object>();
			return result;
		}

		private bool Game_GetPlayerData(string targetUserId, out Dictionary<byte, object> baseParam, out Dictionary<string, object> appendParam)
		{
			bool result = true;
			appendParam = null;
			baseParam = new Dictionary<byte, object>();
			baseParam[102] = targetUserId;
			return result;
		}

		private bool Game_ChangCurrentTips(long summonerId, byte isOpen, out Dictionary<byte, object> baseParam, out Dictionary<string, object> appendParam)
		{
			bool result = true;
			appendParam = null;
			baseParam = new Dictionary<byte, object>();
			baseParam[83] = summonerId;
			baseParam[207] = isOpen;
			return result;
		}

		private bool Game_ChangeSummonerSKill(long summId, string surrSkills, out Dictionary<byte, object> baseParam, out Dictionary<string, object> appendParam)
		{
			bool result = true;
			appendParam = null;
			baseParam = new Dictionary<byte, object>();
			baseParam[216] = surrSkills;
			baseParam[83] = summId;
			return result;
		}

		private bool Game_RecordSpeechTime(int talkTime, out Dictionary<byte, object> baseParam, out Dictionary<string, object> appendParam)
		{
			bool result = true;
			appendParam = null;
			baseParam = new Dictionary<byte, object>();
			baseParam[101] = talkTime;
			return result;
		}

		private bool Game_ModifyMyState(byte userStatus, out Dictionary<byte, object> baseParam, out Dictionary<string, object> appendParam)
		{
			bool result = true;
			appendParam = null;
			Log.debug(" MobaClient ： 改变当前召唤师的游戏状态...");
			baseParam = new Dictionary<byte, object>();
			baseParam[57] = ModelManager.Instance.Get_userData_filed_X("UserId");
			baseParam[65] = userStatus;
			return result;
		}

		private bool Game_GetArenaEnemyInfoByUserId(string userId, out Dictionary<byte, object> baseParam, out Dictionary<string, object> appendParam)
		{
			bool result = true;
			appendParam = null;
			Log.debug(" MobaClient ： 通过useid获得敌方的奇袭召唤师...");
			baseParam = new Dictionary<byte, object>();
			baseParam[57] = userId;
			return result;
		}

		private bool Game_InviteManger(string inviterId, string roomId, out Dictionary<byte, object> baseParam, out Dictionary<string, object> appendParam)
		{
			bool result = true;
			appendParam = null;
			Log.debug(" MobaClient ： 拒绝其他的召唤师开黑邀请.");
			baseParam = new Dictionary<byte, object>();
			baseParam[57] = ModelManager.Instance.Get_userData_X().UserId;
			baseParam[16] = inviterId;
			baseParam[177] = roomId;
			return result;
		}

		private bool Game_ExchangeChip(string chipCode, out Dictionary<byte, object> baseParam, out Dictionary<string, object> appendParam)
		{
			bool result = true;
			appendParam = null;
			Log.debug(" MobaClient ： 拒绝其他的召唤师开黑邀请.");
			baseParam = new Dictionary<byte, object>();
			baseParam[57] = ModelManager.Instance.Get_userData_X().UserId;
			baseParam[221] = chipCode;
			return result;
		}

		private bool Game_GetHomeTotalRecord(out Dictionary<byte, object> baseParam, out Dictionary<string, object> appendParam)
		{
			bool result = true;
			appendParam = null;
			baseParam = new Dictionary<byte, object>();
			baseParam[57] = ModelManager.Instance.Get_userData_X().UserId;
			return result;
		}

		private bool Game_GetTotalRecord(out Dictionary<byte, object> baseParam, out Dictionary<string, object> appendParam)
		{
			bool result = true;
			appendParam = null;
			baseParam = new Dictionary<byte, object>();
			baseParam[57] = ModelManager.Instance.Get_userData_X().UserId;
			baseParam[181] = ModelManager.Instance.Get_GetMyAchievementData_X().totalpvpLogId;
			return result;
		}

		private bool Game_GetHeroRecordInfo(string battleId, int PageNum, out Dictionary<byte, object> baseParam, out Dictionary<string, object> appendParam)
		{
			bool result = true;
			appendParam = null;
			baseParam = new Dictionary<byte, object>();
			baseParam[57] = ModelManager.Instance.Get_userData_X().UserId;
			baseParam[10] = PageNum;
			baseParam[157] = battleId;
			return result;
		}

		private bool Game_GetHistoryRecord(long _logId, out Dictionary<byte, object> baseParam, out Dictionary<string, object> appendParam)
		{
			bool result = true;
			appendParam = null;
			baseParam = new Dictionary<byte, object>();
			baseParam[57] = ModelManager.Instance.Get_userData_X().UserId;
			baseParam[181] = _logId;
			return result;
		}

		private bool Game_SaveUserHonorPic(KdaUserHonorData data, out Dictionary<byte, object> baseParam, out Dictionary<string, object> appendParam)
		{
			bool result = true;
			appendParam = null;
			baseParam = new Dictionary<byte, object>();
			baseParam[251] = data;
			return result;
		}

		private bool Game_GetUserHonorPic(out Dictionary<byte, object> baseParam, out Dictionary<string, object> appendParam)
		{
			bool result = true;
			appendParam = null;
			baseParam = new Dictionary<byte, object>();
			return result;
		}

		private bool Game_GetPvpFightResult(out Dictionary<byte, object> baseParam, out Dictionary<string, object> appendParam)
		{
			bool result = true;
			appendParam = null;
			baseParam = new Dictionary<byte, object>();
			baseParam[57] = ModelManager.Instance.Get_userData_X().UserId;
			baseParam[233] = Singleton<PvpManager>.Instance.RoomGid;
			return result;
		}

		private bool Game_GetKdaMyHeroData(out Dictionary<byte, object> baseParam, out Dictionary<string, object> appendParam)
		{
			bool result = true;
			appendParam = null;
			baseParam = new Dictionary<byte, object>();
			baseParam[181] = ModelManager.Instance.Get_GetMyAchievementData_X().heropvpLogId;
			return result;
		}

		private bool Game_GetMyFightAbility(out Dictionary<byte, object> baseParam, out Dictionary<string, object> appendParam)
		{
			bool result = true;
			appendParam = null;
			baseParam = new Dictionary<byte, object>();
			baseParam[181] = ModelManager.Instance.Get_GetMyAchievementData_X().abilitypvpLogId;
			return result;
		}

		private bool Game_GetMagicBottleInfo(out Dictionary<byte, object> baseParam, out Dictionary<string, object> appendParam)
		{
			bool result = true;
			appendParam = null;
			baseParam = new Dictionary<byte, object>();
			baseParam[235] = ModelManager.Instance.Get_userData_X().magicbottleid;
			return result;
		}

		private bool Game_DrawMagicBottleAward(int type, out Dictionary<byte, object> baseParam, out Dictionary<string, object> appendParam)
		{
			bool result = true;
			appendParam = null;
			baseParam = new Dictionary<byte, object>();
			baseParam[235] = ModelManager.Instance.Get_userData_X().magicbottleid;
			baseParam[241] = type;
			return result;
		}

		private bool Game_GetRedPacketsInfo(out Dictionary<byte, object> baseParam, out Dictionary<string, object> appendParam)
		{
			bool result = true;
			appendParam = null;
			baseParam = new Dictionary<byte, object>();
			return result;
		}

		private bool Game_GetRedPackets(int id, int type, long time, int timepast, out Dictionary<byte, object> baseParam, out Dictionary<string, object> appendParam)
		{
			bool result = true;
			appendParam = null;
			baseParam = new Dictionary<byte, object>();
			baseParam[10] = type;
			baseParam[168] = time;
			baseParam[101] = timepast;
			baseParam[11] = id;
			return result;
		}

		private bool Game_ClientReportOnlineTime(long time, int timepast, out Dictionary<byte, object> baseParam, out Dictionary<string, object> appendParam)
		{
			bool result = true;
			appendParam = null;
			baseParam = new Dictionary<byte, object>();
			baseParam[168] = time;
			baseParam[101] = timepast;
			return result;
		}

		private bool Game_RichManGiftMgr(int type, out Dictionary<byte, object> baseParam, out Dictionary<string, object> appendParam)
		{
			bool result = true;
			appendParam = null;
			baseParam = new Dictionary<byte, object>();
			baseParam[57] = ModelManager.Instance.Get_userData_X().UserId;
			baseParam[234] = type;
			return result;
		}

		private bool Game_GetDoubleCard(out Dictionary<byte, object> baseParam, out Dictionary<string, object> appendParam)
		{
			bool result = true;
			appendParam = null;
			baseParam = new Dictionary<byte, object>();
			return result;
		}

		private bool Game_WearPrivateEffect(long heroId, int modelid, int type, out Dictionary<byte, object> baseParam, out Dictionary<string, object> appendParam)
		{
			bool result = true;
			appendParam = null;
			baseParam = new Dictionary<byte, object>();
			baseParam[57] = ModelManager.Instance.Get_userData_X().UserId;
			baseParam[89] = heroId;
			baseParam[93] = modelid;
			baseParam[10] = type;
			return result;
		}

		private bool Game_OneKeyCompose(out Dictionary<byte, object> baseParam, out Dictionary<string, object> appendParam)
		{
			bool result = true;
			appendParam = null;
			baseParam = new Dictionary<byte, object>();
			baseParam[57] = ModelManager.Instance.Get_userData_X().UserId;
			baseParam[61] = (int)ModelManager.Instance.Get_userData_X().Money;
			return result;
		}

		private bool Game_MagicBottleItem(string nickName, out Dictionary<byte, object> baseParam, out Dictionary<string, object> appendParam)
		{
			bool result = true;
			appendParam = null;
			List<EquipmentInfoData> list = ModelManager.Instance.Get_equipmentList_X();
			long num;
			if (list == null)
			{
				num = 0L;
			}
			else
			{
				EquipmentInfoData equipmentInfoData = list.Find((EquipmentInfoData data) => data.ModelId == 6666);
				if (list.Find((EquipmentInfoData data) => data.ModelId == 6666) == null)
				{
					num = 0L;
				}
				else
				{
					num = equipmentInfoData.EquipmentId;
				}
			}
			baseParam = new Dictionary<byte, object>();
			baseParam[57] = ModelManager.Instance.Get_userData_X().UserId;
			baseParam[59] = nickName;
			baseParam[232] = num.ToString() + ",6666";
			return result;
		}

		public bool SendGateSelfChannelMessage(byte code, SendMsgManager.SendMsgParam param, params object[] args)
		{
			string methodNameString = this.GetMethodNameString(MobaPeerType.C2GateServer, code);
			Dictionary<byte, object> args2;
			Dictionary<string, object> dictionary;
			if (!this.MakeMsgParam(methodNameString, args, out args2, out dictionary))
			{
				throw new Exception(string.Format("methodName {0} 构造参数错误", methodNameString));
			}
			bool flag = NetWorkHelper.Instance.client.SendGateChannelMessage(MobaPeerType.C2GateServer, MobaChannel.Default, code, args2);
			if (!flag)
			{
				ClientLogger.Warn("SendGameMsg failed for " + code);
			}
			return flag;
		}

		public bool SendGameChannelMessage(byte code, SendMsgManager.SendMsgParam param, object[] args)
		{
			string text = "Game_" + ((MobaGameCode)code).ToString();
			Dictionary<byte, object> args2;
			Dictionary<string, object> dictionary;
			if (!this.MakeMsgParam(text, args, out args2, out dictionary))
			{
				UnityEngine.Debug.LogError(string.Format("methodName {0} 构造参数错误", text));
				throw new Exception(string.Format("methodName {0} 构造参数错误", text));
			}
			bool flag = NetWorkHelper.Instance.client.SendGateChannelMessage(MobaPeerType.C2GateServer, MobaChannel.Game, code, args2);
			if (!flag)
			{
				ClientLogger.Warn("SendGameMsg failed for " + code);
			}
			else if (param != null && param.OpenWaitingView)
			{
				WaitingViewMng.Instance.Waiting(MobaMessageType.GameCode, (int)code, param.WaitingViewText, param.BNormal, 15f, true);
			}
			return flag;
		}

		public bool SendTeamChannelMessage(byte code, SendMsgManager.SendMsgParam param, params object[] args)
		{
			return this.SendMessageByGateChannel(MobaChannel.Team, code, param, args);
		}

		private bool SendMessageByGateChannel(MobaChannel channel, byte code, SendMsgManager.SendMsgParam param, object[] args)
		{
			string methodNameStringByChannel = this.GetMethodNameStringByChannel(channel, code);
			Dictionary<byte, object> customOpParameters;
			Dictionary<string, object> dictionary;
			if (!this.MakeMsgParam(methodNameStringByChannel, args, out customOpParameters, out dictionary))
			{
				throw new Exception(string.Format("methodName {0} 构造参数错误", methodNameStringByChannel));
			}
			bool flag = false;
			MobaPeer mobaPeer = NetWorkHelper.Instance.client.GetMobaPeer(MobaPeerType.C2GateServer);
			if (mobaPeer == null)
			{
				ClientLogger.Warn(string.Concat(new object[]
				{
					"SendGateChannelMessage: failed for ",
					code,
					"@",
					channel,
					", gatePeer == null"
				}));
			}
			else
			{
				flag = mobaPeer.OpCustom(code, customOpParameters, true, (byte)channel);
				if (!flag)
				{
					ClientLogger.Warn(string.Concat(new object[]
					{
						"SendGateChannelMessage: failed for ",
						code,
						"@",
						channel
					}));
				}
				else if (param != null && param.OpenWaitingView)
				{
					MobaMessageType type;
					switch (channel)
					{
					case MobaChannel.Friend:
						type = MobaMessageType.FriendCode;
						goto IL_11B;
					case MobaChannel.Team:
						type = MobaMessageType.TeamRoomCode;
						goto IL_11B;
					}
					type = MobaMessageType.GateCode;
					IL_11B:
					WaitingViewMng.Instance.Waiting(type, (int)code, param.WaitingViewText, param.BNormal, 15f, true);
				}
			}
			return flag;
		}

		private bool Game_GetCurrencyCount(int type, out Dictionary<byte, object> baseParam, out Dictionary<string, object> appendParam)
		{
			bool result = true;
			appendParam = null;
			baseParam = new Dictionary<byte, object>();
			baseParam[10] = type;
			return result;
		}

		private Dictionary<byte, object> BuildParams(object[] args)
		{
			if (args == null || args.Length <= 0)
			{
				return null;
			}
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			byte b = 0;
			while ((int)b < args.Length)
			{
				dictionary[b] = args[(int)b];
				b += 1;
			}
			return dictionary;
		}

		public bool SendLobbyMsg(PvpCode code, params object[] args)
		{
			bool result = false;
			try
			{
				Dictionary<byte, object> customOpParameters = this.BuildParams(args);
				MobaPeer mobaPeer = NetWorkHelper.Instance.client.GetMobaPeer(MobaPeerType.C2Lobby);
				result = mobaPeer.OpCustom((byte)code, customOpParameters, true, 1, mobaPeer.IsEncryptionAvailable);
			}
			catch (Exception ex)
			{
				result = false;
				ClientLogger.Error("SendLobbyMsg Error : " + ex.Message);
			}
			return result;
		}

		public bool SendGateLobbyMessage(LobbyCode code, SendMsgManager.SendMsgParam param, params object[] args)
		{
			if (param != null && param.OpenWaitingView)
			{
				WaitingViewMng.Instance.Waiting(MobaMessageType.LobbyCode, (int)code, param.WaitingViewText, param.BNormal, 15f, true);
			}
			if (NetWorkHelper.Instance == null || NetWorkHelper.Instance.client == null)
			{
				ClientLogger.Warn("networkhelper not available");
				return false;
			}
			bool flag = NetWorkHelper.Instance.client.SendLobbyChannelMessage(code, args);
			if (!flag)
			{
				ClientLogger.Warn("SendLobbyChannelMessage failed for " + code);
			}
			return flag;
		}

		public bool SendPvpMsg(PvpCode code, byte[] args = null)
		{
			if (code != PvpCode.C2P_Ping)
			{
				if (code == PvpCode.C2P_UnitsSnap)
				{
					Singleton<NetStatistic>.Instance.Log(NetEventType.eMoveSend);
				}
			}
			else
			{
				Singleton<NetStatistic>.Instance.Log(NetEventType.ePingSend);
			}
			if (NetWorkHelper.Instance == null || NetWorkHelper.Instance.client == null)
			{
				ClientLogger.Error("networkhelper not available");
				return false;
			}
			if (NetWorkHelper.Instance.PvpServerFlag)
			{
				bool flag = NetWorkHelper.Instance.client.SendPvpServerMsg(code, new object[]
				{
					args
				});
				if (!flag)
				{
					ClientLogger.Error("SendPvpServerMsg failed for " + code);
				}
				return flag;
			}
			return true;
		}

		public bool SendPvpMsgBase<T>(PvpCode code, T probufObj) where T : class
		{
			byte[] array = SerializeHelper.Serialize<T>(probufObj);
			if (NetWorkHelper.Instance == null || NetWorkHelper.Instance.client == null)
			{
				ClientLogger.Error("networkhelper not available");
				return false;
			}
			if (NetWorkHelper.Instance.PvpServerFlag)
			{
				bool flag = NetWorkHelper.Instance.client.SendPvpServerMsg(code, new object[]
				{
					array
				});
				if (!flag)
				{
					ClientLogger.Error("SendPvpServerMsg failed for " + code);
				}
				return flag;
			}
			return true;
		}

		public bool SendPvpLoginMsgBase<T>(PvpCode code, T probufObj, int roomId) where T : class
		{
			byte[] array = SerializeHelper.Serialize<T>(probufObj);
			if (NetWorkHelper.Instance == null || NetWorkHelper.Instance.client == null)
			{
				ClientLogger.Error("networkhelper not available");
				return false;
			}
			if (NetWorkHelper.Instance.PvpServerFlag)
			{
				bool flag = NetWorkHelper.Instance.client.SendPvpServerMsg(code, new object[]
				{
					array,
					roomId
				});
				if (!flag)
				{
					ClientLogger.Error("SendPvpServerMsg failed for " + code);
				}
				return flag;
			}
			return true;
		}
	}
}

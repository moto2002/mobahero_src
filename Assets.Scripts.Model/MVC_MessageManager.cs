using Assets.Scripts.Server;
using Com.Game.Module;
using MobaProtocol;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.Model
{
	public class MVC_MessageManager : IGlobalComServer
	{
		private static MVC_MessageManager mInstance;

		private static Dictionary<MobaGameCode, List<MobaMessageFunc>> dicMabaGamePreHandler = new Dictionary<MobaGameCode, List<MobaMessageFunc>>();

		private static Dictionary<MobaMasterCode, List<MobaMessageFunc>> dicMabaMasterPreHandler = new Dictionary<MobaMasterCode, List<MobaMessageFunc>>();

		private static Dictionary<MobaGateCode, List<MobaMessageFunc>> dicMabaGatePreHandler = new Dictionary<MobaGateCode, List<MobaMessageFunc>>();

		private static Dictionary<MobaFriendCode, List<MobaMessageFunc>> dicMabaFriendPreHandler = new Dictionary<MobaFriendCode, List<MobaMessageFunc>>();

		private static Dictionary<MobaChatCode, List<MobaMessageFunc>> dicMabaChatPreHandler = new Dictionary<MobaChatCode, List<MobaMessageFunc>>();

		private static Dictionary<MobaTeamRoomCode, List<MobaMessageFunc>> dicMabaTeamRoomPreHandler = new Dictionary<MobaTeamRoomCode, List<MobaMessageFunc>>();

		private static Dictionary<MobaUserDataCode, List<MobaMessageFunc>> dicMobaUserDataCodePreHandler = new Dictionary<MobaUserDataCode, List<MobaMessageFunc>>();

		private static Dictionary<MobaMessageType, List<int>> dicWaitingMsg = new Dictionary<MobaMessageType, List<int>>();

		private IGlobalComServer m_waitViewMsg;

		private IGlobalComServer m_popViewMng;

		public void OnAwake()
		{
			Array values = Enum.GetValues(typeof(MobaMasterCode));
			foreach (object current in values)
			{
				MobaMessageManager.RegistMessage((MobaMasterCode)((byte)current), new MobaMessageFunc(MVC_MessageManager.OnGetMsg));
			}
			Array values2 = Enum.GetValues(typeof(MobaGameCode));
			foreach (object current2 in values2)
			{
				MobaMessageManager.RegistMessage((MobaGameCode)((byte)current2), new MobaMessageFunc(MVC_MessageManager.OnGetMsg));
			}
			Array values3 = Enum.GetValues(typeof(MobaGateCode));
			foreach (object current3 in values3)
			{
				MobaMessageManager.RegistMessage((MobaGateCode)((byte)current3), new MobaMessageFunc(MVC_MessageManager.OnGetMsg));
			}
			Array values4 = Enum.GetValues(typeof(MobaFriendCode));
			foreach (object current4 in values4)
			{
				MobaMessageManager.RegistMessage((MobaFriendCode)((byte)current4), new MobaMessageFunc(MVC_MessageManager.OnGetMsg));
			}
			Array values5 = Enum.GetValues(typeof(MobaTeamRoomCode));
			foreach (object current5 in values5)
			{
				MobaMessageManager.RegistMessage((MobaTeamRoomCode)((byte)current5), new MobaMessageFunc(MVC_MessageManager.OnGetMsg));
			}
			Array values6 = Enum.GetValues(typeof(MobaUserDataCode));
			foreach (object current6 in values5)
			{
				MobaMessageManager.RegistMessage((MobaUserDataCode)((byte)current6), new MobaMessageFunc(MVC_MessageManager.OnGetMsg));
			}
			MsgPreHandlerMng instance = MsgPreHandlerMng.Instance;
			this.m_waitViewMsg = new WaitingViewMng();
			this.m_waitViewMsg.OnAwake();
			this.m_waitViewMsg.Enable(true);
			this.m_popViewMng = new PopViewMng();
			this.m_popViewMng.OnAwake();
			this.m_popViewMng.Enable(true);
		}

		public void OnStart()
		{
			this.m_waitViewMsg.OnStart();
			this.m_popViewMng.OnStart();
		}

		public void OnUpdate()
		{
			this.m_waitViewMsg.OnUpdate();
			this.m_popViewMng.OnUpdate();
		}

		public void OnDestroy()
		{
			this.m_waitViewMsg.OnDestroy();
			this.m_popViewMng.OnDestroy();
		}

		public void Enable(bool b)
		{
			this.m_waitViewMsg.Enable(b);
			this.m_popViewMng.Enable(b);
		}

		public void OnRestart()
		{
			this.m_waitViewMsg.OnRestart();
			this.m_popViewMng.OnRestart();
		}

		public void OnApplicationQuit()
		{
			this.m_waitViewMsg.OnApplicationQuit();
			this.m_popViewMng.OnApplicationQuit();
		}

		public void OnApplicationFocus(bool isFocus)
		{
		}

		public void OnApplicationPause(bool isPause)
		{
		}

		private static void OnGetMsg(MobaMessage msg)
		{
			if (msg == null)
			{
				return;
			}
			ClientMsg msgID = ClientMsg.PeerConnectTimeOut;
			ClientMsg msgID2 = ClientMsg.PeerConnectTimeOut;
			switch (msg.MessageType)
			{
			case MobaMessageType.MasterCode:
				MVC_MessageManager.PreHandler((MobaMasterCode)msg.ID, msg);
				msgID = MVC_MessageManager.Master_to_NotifyModel((MobaMasterCode)msg.ID);
				msgID2 = MVC_MessageManager.Master_to_NotifyView((MobaMasterCode)msg.ID);
				break;
			case MobaMessageType.GameCode:
				if (MenuView.dicTimes.ContainsKey((MobaGameCode)msg.ID))
				{
				}
				MVC_MessageManager.PreHandler((MobaGameCode)msg.ID, msg);
				msgID = MVC_MessageManager.Game_to_NotifyModel((MobaGameCode)msg.ID);
				msgID2 = MVC_MessageManager.Game_to_NotifyView((MobaGameCode)msg.ID);
				break;
			case MobaMessageType.ChatCode:
				MVC_MessageManager.PreHandler((MobaChatCode)msg.ID, msg);
				msgID = MVC_MessageManager.Chat_to_NotifyModel((MobaChatCode)msg.ID);
				msgID2 = MVC_MessageManager.Chat_to_NotifyView((MobaChatCode)msg.ID);
				break;
			case MobaMessageType.FriendCode:
				MVC_MessageManager.PreHandler((MobaFriendCode)msg.ID, msg);
				msgID = MVC_MessageManager.Friend_to_NotifyModel((MobaFriendCode)msg.ID);
				msgID2 = MVC_MessageManager.Friend_to_NotifyView((MobaFriendCode)msg.ID);
				break;
			case MobaMessageType.TeamRoomCode:
				MVC_MessageManager.PreHandler((MobaTeamRoomCode)msg.ID, msg);
				msgID = MVC_MessageManager.TeamRoom_to_NotifyModel((MobaTeamRoomCode)msg.ID);
				msgID2 = MVC_MessageManager.TeamRoom_to_NotifyView((MobaTeamRoomCode)msg.ID);
				break;
			case MobaMessageType.UserDataCode:
				MVC_MessageManager.PreHandler((MobaUserDataCode)msg.ID, msg);
				msgID = MVC_MessageManager.UserData_to_NotifyModel((MobaUserDataCode)msg.ID);
				msgID2 = MVC_MessageManager.UserData_to_NotifyView((MobaUserDataCode)msg.ID);
				break;
			case MobaMessageType.GateCode:
				MVC_MessageManager.PreHandler((MobaGameCode)msg.ID, msg);
				msgID2 = MVC_MessageManager.Gate_to_NotifyView((MobaGateCode)msg.ID);
				break;
			}
			MobaMessage message = MobaMessageManager.GetMessage(msgID, msg.Param, 0f);
			MobaMessageManager.ExecuteMsg(message);
			MobaMessage message2 = MobaMessageManager.GetMessage(msgID2, msg.Param, 0f);
			MobaMessageManager.ExecuteMsg(message2);
		}

		private static void PreHandler(MobaGameCode code, MobaMessage msg)
		{
			if (MVC_MessageManager.dicMabaGamePreHandler.ContainsKey(code))
			{
				foreach (MobaMessageFunc current in MVC_MessageManager.dicMabaGamePreHandler[code])
				{
					current(msg);
				}
			}
		}

		private static void PreHandler(MobaMasterCode code, MobaMessage msg)
		{
			if (MVC_MessageManager.dicMabaMasterPreHandler.ContainsKey(code))
			{
				foreach (MobaMessageFunc current in MVC_MessageManager.dicMabaMasterPreHandler[code])
				{
					current(msg);
				}
			}
		}

		private static void PreHandler(MobaGateCode code, MobaMessage msg)
		{
			if (MVC_MessageManager.dicMabaGatePreHandler.ContainsKey(code))
			{
				foreach (MobaMessageFunc current in MVC_MessageManager.dicMabaGatePreHandler[code])
				{
					current(msg);
				}
			}
		}

		private static void PreHandler(MobaFriendCode code, MobaMessage msg)
		{
			if (MVC_MessageManager.dicMabaFriendPreHandler.ContainsKey(code))
			{
				foreach (MobaMessageFunc current in MVC_MessageManager.dicMabaFriendPreHandler[code])
				{
					current(msg);
				}
			}
		}

		private static void PreHandler(MobaChatCode code, MobaMessage msg)
		{
			if (MVC_MessageManager.dicMabaChatPreHandler.ContainsKey(code))
			{
				foreach (MobaMessageFunc current in MVC_MessageManager.dicMabaChatPreHandler[code])
				{
					current(msg);
				}
			}
		}

		private static void PreHandler(MobaTeamRoomCode code, MobaMessage msg)
		{
			if (MVC_MessageManager.dicMabaTeamRoomPreHandler.ContainsKey(code))
			{
				foreach (MobaMessageFunc current in MVC_MessageManager.dicMabaTeamRoomPreHandler[code])
				{
					current(msg);
				}
			}
		}

		private static void PreHandler(MobaUserDataCode code, MobaMessage msg)
		{
			if (MVC_MessageManager.dicMobaUserDataCodePreHandler.ContainsKey(code))
			{
				foreach (MobaMessageFunc current in MVC_MessageManager.dicMobaUserDataCodePreHandler[code])
				{
					current(msg);
				}
			}
		}

		public static void AddListener_preHandler(MobaGameCode code, MobaMessageFunc func)
		{
			if (func == null)
			{
				return;
			}
			if (!MVC_MessageManager.dicMabaGamePreHandler.ContainsKey(code))
			{
				MVC_MessageManager.dicMabaGamePreHandler.Add(code, new List<MobaMessageFunc>());
			}
			if (!MVC_MessageManager.dicMabaGamePreHandler[code].Contains(func))
			{
				MVC_MessageManager.dicMabaGamePreHandler[code].Add(func);
			}
		}

		public static void AddListener_preHandler(MobaMasterCode code, MobaMessageFunc func)
		{
			if (func == null)
			{
				return;
			}
			if (!MVC_MessageManager.dicMabaMasterPreHandler.ContainsKey(code))
			{
				MVC_MessageManager.dicMabaMasterPreHandler.Add(code, new List<MobaMessageFunc>());
			}
			if (!MVC_MessageManager.dicMabaMasterPreHandler[code].Contains(func))
			{
				MVC_MessageManager.dicMabaMasterPreHandler[code].Add(func);
			}
		}

		public static void AddListener_model(MobaMasterCode code, MobaMessageFunc func)
		{
			MobaMessageManager.RegistMessage(MVC_MessageManager.Master_to_NotifyModel(code), func);
		}

		public static void AddListener_model(MobaGameCode code, MobaMessageFunc func)
		{
			MobaMessageManager.RegistMessage(MVC_MessageManager.Game_to_NotifyModel(code), func);
		}

		public static void AddListener_model(MobaFriendCode code, MobaMessageFunc func)
		{
			MobaMessageManager.RegistMessage(MVC_MessageManager.Friend_to_NotifyModel(code), func);
		}

		public static void AddListener_model(MobaChatCode code, MobaMessageFunc func)
		{
			MobaMessageManager.RegistMessage(MVC_MessageManager.Chat_to_NotifyModel(code), func);
		}

		public static void AddListener_model(MobaTeamRoomCode code, MobaMessageFunc func)
		{
			MobaMessageManager.RegistMessage(MVC_MessageManager.TeamRoom_to_NotifyModel(code), func);
		}

		public static void AddListener_model(MobaUserDataCode code, MobaMessageFunc func)
		{
			MobaMessageManager.RegistMessage(MVC_MessageManager.UserData_to_NotifyModel(code), func);
		}

		public static void AddListener_view(MobaMasterCode code, MobaMessageFunc func)
		{
			MobaMessageManager.RegistMessage(MVC_MessageManager.Master_to_NotifyView(code), func);
		}

		public static void AddListener_view(MobaGameCode code, MobaMessageFunc func)
		{
			MobaMessageManager.RegistMessage(MVC_MessageManager.Game_to_NotifyView(code), func);
		}

		public static void AddListener_view(MobaFriendCode code, MobaMessageFunc func)
		{
			MobaMessageManager.RegistMessage(MVC_MessageManager.Friend_to_NotifyView(code), func);
		}

		public static void AddListener_view(MobaChatCode code, MobaMessageFunc func)
		{
			MobaMessageManager.RegistMessage(MVC_MessageManager.Chat_to_NotifyView(code), func);
		}

		public static void AddListener_view(MobaTeamRoomCode code, MobaMessageFunc func)
		{
			MobaMessageManager.RegistMessage(MVC_MessageManager.TeamRoom_to_NotifyView(code), func);
		}

		public static void AddListener_view(MobaGateCode code, MobaMessageFunc func)
		{
			MobaMessageManager.RegistMessage(MVC_MessageManager.Gate_to_NotifyView(code), func);
		}

		public static void AddListener_view(MobaUserDataCode code, MobaMessageFunc func)
		{
			MobaMessageManager.RegistMessage(MVC_MessageManager.UserData_to_NotifyView(code), func);
		}

		public static void RemoveListener_preHandler(MobaGameCode code, MobaMessageFunc func)
		{
			if (MVC_MessageManager.dicMabaGamePreHandler.ContainsKey(code) && func != null && MVC_MessageManager.dicMabaGamePreHandler[code] != null && MVC_MessageManager.dicMabaGamePreHandler[code].Contains(func))
			{
				MVC_MessageManager.dicMabaGamePreHandler[code].Remove(func);
			}
		}

		public static void RemoveListener_preHandler(MobaMasterCode code, MobaMessageFunc func)
		{
			if (MVC_MessageManager.dicMabaMasterPreHandler.ContainsKey(code) && func != null && MVC_MessageManager.dicMabaMasterPreHandler[code] != null && MVC_MessageManager.dicMabaMasterPreHandler[code].Contains(func))
			{
				MVC_MessageManager.dicMabaMasterPreHandler[code].Remove(func);
			}
		}

		public static void RemoveListener_model(MobaMasterCode code, MobaMessageFunc func)
		{
			MobaMessageManager.UnRegistMessage(MVC_MessageManager.Master_to_NotifyModel(code), func);
		}

		public static void RemoveListener_model(MobaGameCode code, MobaMessageFunc func)
		{
			MobaMessageManager.UnRegistMessage(MVC_MessageManager.Game_to_NotifyModel(code), func);
		}

		public static void RemoveListener_model(MobaFriendCode code, MobaMessageFunc func)
		{
			MobaMessageManager.UnRegistMessage(MVC_MessageManager.Friend_to_NotifyModel(code), func);
		}

		public static void RemoveListener_model(MobaChatCode code, MobaMessageFunc func)
		{
			MobaMessageManager.UnRegistMessage(MVC_MessageManager.Chat_to_NotifyModel(code), func);
		}

		public static void RemoveListener_view(MobaMasterCode code, MobaMessageFunc func)
		{
			MobaMessageManager.UnRegistMessage(MVC_MessageManager.Master_to_NotifyView(code), func);
		}

		public static void RemoveListener_view(MobaGameCode code, MobaMessageFunc func)
		{
			MobaMessageManager.UnRegistMessage(MVC_MessageManager.Game_to_NotifyView(code), func);
		}

		public static void RemoveListener_view(MobaFriendCode code, MobaMessageFunc func)
		{
			MobaMessageManager.UnRegistMessage(MVC_MessageManager.Friend_to_NotifyView(code), func);
		}

		public static void RemoveListener_view(MobaChatCode code, MobaMessageFunc func)
		{
			MobaMessageManager.UnRegistMessage(MVC_MessageManager.Chat_to_NotifyView(code), func);
		}

		public static void RemoveListener_view(MobaTeamRoomCode code, MobaMessageFunc func)
		{
			MobaMessageManager.UnRegistMessage(MVC_MessageManager.TeamRoom_to_NotifyView(code), func);
		}

		public static void RemoveListener_view(MobaGateCode code, MobaMessageFunc func)
		{
			MobaMessageManager.UnRegistMessage(MVC_MessageManager.Gate_to_NotifyView(code), func);
		}

		public static void RemoveListener_view(MobaUserDataCode code, MobaMessageFunc func)
		{
			MobaMessageManager.UnRegistMessage(MVC_MessageManager.UserData_to_NotifyView(code), func);
		}

		public static MobaMessageType ClientMsg_to_RawCode(ClientMsg code, out int retCode)
		{
			MobaMessageType result = MobaMessageType.Client;
			retCode = 0;
			if (code >= ClientMsg.NotifyModel_master_begin && code < ClientMsg.NotifyModel_master_end)
			{
				retCode = (int)MVC_MessageManager.NotifyModel_to_Master(code);
				result = MobaMessageType.MasterCode;
			}
			else if (code >= ClientMsg.NotifyModel_game_begin && code < ClientMsg.NotifyModel_game_end)
			{
				retCode = (int)MVC_MessageManager.NotifyModel_to_Game(code);
				result = MobaMessageType.GameCode;
			}
			else if (code >= ClientMsg.NotifyModel_friend_begin && code < ClientMsg.NotifyModel_friend_end)
			{
				retCode = (int)MVC_MessageManager.NotifyModel_to_Friend(code);
				result = MobaMessageType.FriendCode;
			}
			else if (code >= ClientMsg.NotifyModel_chat_begin && code < ClientMsg.NotifyModel_chat_end)
			{
				retCode = (int)MVC_MessageManager.NotifyModel_to_Chat(code);
				result = MobaMessageType.ChatCode;
			}
			else if (code >= ClientMsg.NotifyModel_teamroom_begin && code < ClientMsg.NotifyModel_teamroom_end)
			{
				retCode = (int)MVC_MessageManager.NotifyModel_to_TeamRoom(code);
				result = MobaMessageType.FriendCode;
			}
			else if (code >= ClientMsg.NotifyModel_userdata_begin && code < ClientMsg.NotifyModel_userdata_end)
			{
				retCode = (int)MVC_MessageManager.NotifyModel_to_UserData(code);
				result = MobaMessageType.UserDataCode;
			}
			else if (code >= ClientMsg.NotifyView_master_begin && code < ClientMsg.NotifyView_game_begin)
			{
				retCode = (int)MVC_MessageManager.NotifyView_to_Master(code);
				result = MobaMessageType.MasterCode;
			}
			else if (code >= ClientMsg.NotifyView_game_begin && code < ClientMsg.NotifyView_game_end)
			{
				retCode = (int)MVC_MessageManager.NotifyView_to_Game(code);
				result = MobaMessageType.GameCode;
			}
			else if (code >= ClientMsg.NotifyView_friend_begin && code < ClientMsg.NotifyView_friend_end)
			{
				retCode = (int)MVC_MessageManager.NotifyView_to_Friend(code);
				result = MobaMessageType.FriendCode;
			}
			else if (code >= ClientMsg.NotifyView_chat_begin && code < ClientMsg.NotifyView_chat_end)
			{
				retCode = (int)MVC_MessageManager.NotifyView_to_Chat(code);
				result = MobaMessageType.ChatCode;
			}
			else if (code >= ClientMsg.NotifyView_teamroom_begin && code < ClientMsg.NotifyView_teamroom_end)
			{
				retCode = (int)MVC_MessageManager.NotifyView_to_TeamRoom(code);
				result = MobaMessageType.FriendCode;
			}
			else if (code >= ClientMsg.NotifyView_userdata_begin && code < ClientMsg.NotifyView_userdata_end)
			{
				retCode = (int)MVC_MessageManager.NotifyView_to_UserData(code);
				result = MobaMessageType.UserDataCode;
			}
			return result;
		}

		public static MobaMasterCode NotifyModel_to_Master(ClientMsg code)
		{
			return (MobaMasterCode)(code - ClientMsg.NotifyModel_master_begin);
		}

		public static MobaGameCode NotifyModel_to_Game(ClientMsg code)
		{
			return (MobaGameCode)(code - ClientMsg.NotifyModel_game_begin);
		}

		public static MobaFriendCode NotifyModel_to_Friend(ClientMsg code)
		{
			return (MobaFriendCode)(code - ClientMsg.NotifyModel_friend_begin);
		}

		public static MobaChatCode NotifyModel_to_Chat(ClientMsg code)
		{
			return (MobaChatCode)(code - ClientMsg.NotifyModel_chat_begin);
		}

		public static MobaTeamRoomCode NotifyModel_to_TeamRoom(ClientMsg code)
		{
			return (MobaTeamRoomCode)(code - ClientMsg.NotifyModel_teamroom_begin);
		}

		public static MobaUserDataCode NotifyModel_to_UserData(ClientMsg code)
		{
			return (MobaUserDataCode)(code - ClientMsg.NotifyModel_userdata_begin);
		}

		public static MobaMasterCode NotifyView_to_Master(ClientMsg code)
		{
			return (MobaMasterCode)(code - ClientMsg.NotifyView_master_begin);
		}

		public static MobaGameCode NotifyView_to_Game(ClientMsg code)
		{
			return (MobaGameCode)(code - ClientMsg.NotifyView_game_begin);
		}

		public static MobaFriendCode NotifyView_to_Friend(ClientMsg code)
		{
			return (MobaFriendCode)(code - ClientMsg.NotifyView_friend_begin);
		}

		public static MobaChatCode NotifyView_to_Chat(ClientMsg code)
		{
			return (MobaChatCode)(code - ClientMsg.NotifyView_chat_begin);
		}

		public static MobaTeamRoomCode NotifyView_to_TeamRoom(ClientMsg code)
		{
			return (MobaTeamRoomCode)(code - ClientMsg.NotifyView_teamroom_begin);
		}

		public static MobaUserDataCode NotifyView_to_UserData(ClientMsg code)
		{
			return (MobaUserDataCode)(code - ClientMsg.NotifyView_userdata_begin);
		}

		public static ClientMsg Master_to_NotifyModel(MobaMasterCode code)
		{
			return ClientMsg.NotifyModel_master_begin + (int)code;
		}

		public static ClientMsg Game_to_NotifyModel(MobaGameCode code)
		{
			return ClientMsg.NotifyModel_game_begin + (int)code;
		}

		public static ClientMsg Friend_to_NotifyModel(MobaFriendCode code)
		{
			return ClientMsg.NotifyModel_friend_begin + (int)code;
		}

		public static ClientMsg Chat_to_NotifyModel(MobaChatCode code)
		{
			return ClientMsg.NotifyModel_chat_begin + (int)code;
		}

		public static ClientMsg TeamRoom_to_NotifyModel(MobaTeamRoomCode code)
		{
			return ClientMsg.NotifyModel_teamroom_begin + (int)code;
		}

		public static ClientMsg UserData_to_NotifyModel(MobaUserDataCode code)
		{
			return ClientMsg.NotifyModel_userdata_begin + (int)code;
		}

		public static ClientMsg Master_to_NotifyView(MobaMasterCode code)
		{
			return ClientMsg.NotifyView_master_begin + (int)code;
		}

		public static ClientMsg Game_to_NotifyView(MobaGameCode code)
		{
			return ClientMsg.NotifyView_game_begin + (int)code;
		}

		public static ClientMsg Friend_to_NotifyView(MobaFriendCode code)
		{
			return ClientMsg.NotifyView_game_begin + (int)code;
		}

		public static ClientMsg Chat_to_NotifyView(MobaChatCode code)
		{
			return ClientMsg.NotifyView_chat_begin + (int)code;
		}

		public static ClientMsg Gate_to_NotifyView(MobaGateCode code)
		{
			return ClientMsg.NotifyView_gate_begin + (int)code;
		}

		public static ClientMsg TeamRoom_to_NotifyView(MobaTeamRoomCode code)
		{
			return ClientMsg.NotifyView_teamroom_begin + (int)code;
		}

		public static ClientMsg UserData_to_NotifyView(MobaUserDataCode code)
		{
			return ClientMsg.NotifyView_userdata_begin + (int)code;
		}
	}
}

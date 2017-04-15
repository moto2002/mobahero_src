using Assets.Scripts.Server;
using Com.Game.Utils;
using MobaClient;
using MobaProtocol;
using System;
using System.Collections.Generic;
using System.Diagnostics;

public class MobaMessageManager : IGlobalComServer
{
	private static Dictionary<int, MobaMessageFunc>[] mMessageFuncList = new Dictionary<int, MobaMessageFunc>[11];

	private static readonly Queue<MobaMessage> mMessageQueue = new Queue<MobaMessage>(30);

	public void OnAwake()
	{
	}

	public void OnStart()
	{
	}

	public void OnDestroy()
	{
	}

	public void Enable(bool b)
	{
	}

	public void OnRestart()
	{
		MobaMessageManager.ClearAllMessage();
	}

	public void OnApplicationQuit()
	{
	}

	public void OnApplicationFocus(bool isFocus)
	{
	}

	public void OnApplicationPause(bool isPause)
	{
	}

	public static void RegistMessage(Photon2ClientMsg msgID, MobaMessageFunc msgFunc)
	{
		MobaMessageManager.RegistMessage(MobaMessageType.Phonto2Client, (int)msgID, msgFunc);
	}

	public static void UnRegistMessage(Photon2ClientMsg msgID, MobaMessageFunc msgFunc)
	{
		MobaMessageManager.UnRegistMessage(MobaMessageType.Phonto2Client, (int)msgID, msgFunc);
	}

	public static void RegistMessage(MobaMasterCode msgID, MobaMessageFunc msgFunc)
	{
		MobaMessageManager.RegistMessage(MobaMessageType.MasterCode, (int)msgID, msgFunc);
	}

	public static void UnRegistMessage(MobaMasterCode msgID, MobaMessageFunc msgFunc)
	{
		MobaMessageManager.UnRegistMessage(MobaMessageType.MasterCode, (int)msgID, msgFunc);
	}

	public static void RegistMessage(MobaGameCode msgID, MobaMessageFunc msgFunc)
	{
		MobaMessageManager.RegistMessage(MobaMessageType.GameCode, (int)msgID, msgFunc);
	}

	public static void UnRegistMessage(MobaGameCode msgID, MobaMessageFunc msgFunc)
	{
		MobaMessageManager.UnRegistMessage(MobaMessageType.GameCode, (int)msgID, msgFunc);
	}

	public static void RegistMessage(PvpCode msgID, MobaMessageFunc msgFunc)
	{
		MobaMessageManager.RegistMessage(MobaMessageType.PvpCode, (int)msgID, msgFunc);
	}

	public static void UnRegistMessage(PvpCode msgID, MobaMessageFunc msgFunc)
	{
		MobaMessageManager.UnRegistMessage(MobaMessageType.PvpCode, (int)msgID, msgFunc);
	}

	public static void RegistMessage(MobaChatCode msgID, MobaMessageFunc msgFunc)
	{
		MobaMessageManager.RegistMessage(MobaMessageType.ChatCode, (int)msgID, msgFunc);
	}

	public static void UnRegistMessage(MobaChatCode msgID, MobaMessageFunc msgFunc)
	{
		MobaMessageManager.UnRegistMessage(MobaMessageType.ChatCode, (int)msgID, msgFunc);
	}

	public static void RegistMessage(LobbyCode msgID, MobaMessageFunc msgFunc)
	{
		MobaMessageManager.RegistMessage(MobaMessageType.LobbyCode, (int)msgID, msgFunc);
	}

	public static void UnRegistMessage(LobbyCode msgID, MobaMessageFunc msgFunc)
	{
		MobaMessageManager.UnRegistMessage(MobaMessageType.LobbyCode, (int)msgID, msgFunc);
	}

	public static void RegistMessage(ClientMsg msgID, MobaMessageFunc msgFunc)
	{
		MobaMessageManager.RegistMessage(MobaMessageType.Client, (int)msgID, msgFunc);
	}

	public static void UnRegistMessage(ClientMsg msgID, MobaMessageFunc msgFunc)
	{
		MobaMessageManager.UnRegistMessage(MobaMessageType.Client, (int)msgID, msgFunc);
	}

	public static void RegistMessage(ClientNet msgID, MobaMessageFunc msgFunc)
	{
		MobaMessageManager.RegistMessage((ClientMsg)msgID, msgFunc);
	}

	public static void UnRegistMessage(ClientNet msgID, MobaMessageFunc msgFunc)
	{
		MobaMessageManager.UnRegistMessage((ClientMsg)msgID, msgFunc);
	}

	public static void RegistMessage(ClientC2V msgID, MobaMessageFunc msgFunc)
	{
		MobaMessageManager.RegistMessage((ClientMsg)msgID, msgFunc);
	}

	public static void UnRegistMessage(ClientC2V msgID, MobaMessageFunc msgFunc)
	{
		MobaMessageManager.UnRegistMessage((ClientMsg)msgID, msgFunc);
	}

	public static void RegistMessage(ClientV2C msgID, MobaMessageFunc msgFunc)
	{
		MobaMessageManager.RegistMessage((ClientMsg)msgID, msgFunc);
	}

	public static void UnRegistMessage(ClientV2C msgID, MobaMessageFunc msgFunc)
	{
		MobaMessageManager.UnRegistMessage((ClientMsg)msgID, msgFunc);
	}

	public static void RegistMessage(ClientC2C msgID, MobaMessageFunc msgFunc)
	{
		MobaMessageManager.RegistMessage((ClientMsg)msgID, msgFunc);
	}

	public static void UnRegistMessage(ClientC2C msgID, MobaMessageFunc msgFunc)
	{
		MobaMessageManager.UnRegistMessage((ClientMsg)msgID, msgFunc);
	}

	public static void RegistMessage(ClientV2V msgID, MobaMessageFunc msgFunc)
	{
		MobaMessageManager.RegistMessage((ClientMsg)msgID, msgFunc);
	}

	public static void UnRegistMessage(ClientV2V msgID, MobaMessageFunc msgFunc)
	{
		MobaMessageManager.UnRegistMessage((ClientMsg)msgID, msgFunc);
	}

	public static void RegistMessage(MobaGateCode msgID, MobaMessageFunc msgFunc)
	{
		MobaMessageManager.RegistMessage(MobaMessageType.GateCode, (int)msgID, msgFunc);
	}

	public static void UnRegistMessage(MobaGateCode msgID, MobaMessageFunc msgFunc)
	{
		MobaMessageManager.UnRegistMessage(MobaMessageType.GateCode, (int)msgID, msgFunc);
	}

	public static void RegistMessage(MobaFriendCode msgID, MobaMessageFunc msgFunc)
	{
		MobaMessageManager.RegistMessage(MobaMessageType.FriendCode, (int)msgID, msgFunc);
	}

	public static void UnRegistMessage(MobaFriendCode msgID, MobaMessageFunc msgFunc)
	{
		MobaMessageManager.UnRegistMessage(MobaMessageType.FriendCode, (int)msgID, msgFunc);
	}

	public static void RegistMessage(MobaTeamRoomCode msgID, MobaMessageFunc msgFunc)
	{
		MobaMessageManager.RegistMessage(MobaMessageType.TeamRoomCode, (int)msgID, msgFunc);
	}

	public static void UnRegistMessage(MobaTeamRoomCode msgID, MobaMessageFunc msgFunc)
	{
		MobaMessageManager.UnRegistMessage(MobaMessageType.TeamRoomCode, (int)msgID, msgFunc);
	}

	public static void RegistMessage(MobaUserDataCode msgID, MobaMessageFunc msgFunc)
	{
		MobaMessageManager.RegistMessage(MobaMessageType.UserDataCode, (int)msgID, msgFunc);
	}

	public static void UnRegistMessage(MobaUserDataCode msgID, MobaMessageFunc msgFunc)
	{
		MobaMessageManager.UnRegistMessage(MobaMessageType.UserDataCode, (int)msgID, msgFunc);
	}

	public static void RegistMessage(MobaMessageType type, int msgID, MobaMessageFunc msgFunc)
	{
		if (type < (MobaMessageType)MobaMessageManager.mMessageFuncList.Length)
		{
			if (MobaMessageManager.mMessageFuncList[(int)type] == null)
			{
				MobaMessageManager.mMessageFuncList[(int)type] = new Dictionary<int, MobaMessageFunc>();
			}
			Dictionary<int, MobaMessageFunc> dictionary = MobaMessageManager.mMessageFuncList[(int)type];
			if (dictionary.ContainsKey(msgID))
			{
				Dictionary<int, MobaMessageFunc> dictionary2;
				Dictionary<int, MobaMessageFunc> expr_3C = dictionary2 = dictionary;
				MobaMessageFunc a = dictionary2[msgID];
				expr_3C[msgID] = (MobaMessageFunc)Delegate.Combine(a, msgFunc);
			}
			else
			{
				dictionary[msgID] = msgFunc;
			}
		}
	}

	public static void UnRegistMessage(MobaMessageType msgType, int msgID, MobaMessageFunc msgFunc)
	{
		if (msgType < (MobaMessageType)MobaMessageManager.mMessageFuncList.Length)
		{
			Dictionary<int, MobaMessageFunc> dictionary = MobaMessageManager.mMessageFuncList[(int)msgType];
			if (dictionary != null && dictionary.ContainsKey(msgID))
			{
				Dictionary<int, MobaMessageFunc> dictionary2;
				Dictionary<int, MobaMessageFunc> expr_2A = dictionary2 = dictionary;
				MobaMessageFunc source = dictionary2[msgID];
				expr_2A[msgID] = (MobaMessageFunc)Delegate.Remove(source, msgFunc);
			}
		}
	}

	public static void ClearAllMessage()
	{
		for (int i = 0; i < MobaMessageManager.mMessageFuncList.Length; i++)
		{
			if (MobaMessageManager.mMessageFuncList[i] != null)
			{
				MobaMessageManager.mMessageFuncList[i].Clear();
				MobaMessageManager.mMessageFuncList[i] = null;
			}
		}
	}

	public static void DispatchMsg(MobaMessage msg)
	{
		MobaMessageManager.mMessageQueue.Enqueue(msg);
	}

	public static void ExecuteMsg(MobaMessage msg)
	{
		if (msg.MessageType < (MobaMessageType)MobaMessageManager.mMessageFuncList.Length)
		{
			if (msg.MessageType == MobaMessageType.PvpCode)
			{
			}
			Dictionary<int, MobaMessageFunc> dictionary = MobaMessageManager.mMessageFuncList[(int)msg.MessageType];
			MobaMessageFunc mobaMessageFunc;
			if (dictionary != null && dictionary.TryGetValue(msg.ID, out mobaMessageFunc))
			{
				if (mobaMessageFunc == null)
				{
					return;
				}
				if (msg.MessageType == MobaMessageType.PvpCode)
				{
				}
				try
				{
					Stopwatch stopwatch = new Stopwatch();
					if (msg.MessageType == MobaMessageType.PvpCode)
					{
						stopwatch.Start();
					}
					mobaMessageFunc(msg);
					if (msg.MessageType == MobaMessageType.PvpCode)
					{
						stopwatch.Stop();
					}
				}
				catch (Exception e)
				{
					ClientLogger.LogException(e);
				}
				if (msg.MessageType == MobaMessageType.PvpCode)
				{
				}
			}
		}
	}

	public static bool IsHandlerExists(MobaMessage msg)
	{
		if (msg == null)
		{
			throw new ArgumentNullException("msg");
		}
		if (msg.MessageType < (MobaMessageType)MobaMessageManager.mMessageFuncList.Length)
		{
			Dictionary<int, MobaMessageFunc> dictionary = MobaMessageManager.mMessageFuncList[(int)msg.MessageType];
			MobaMessageFunc mobaMessageFunc;
			if (dictionary != null && dictionary.TryGetValue(msg.ID, out mobaMessageFunc))
			{
				return mobaMessageFunc != null;
			}
		}
		return false;
	}

	public static MobaMessage GetMessage(Photon2ClientMsg msgID, object msgParam, float delayTime = 0f)
	{
		return new MobaMessage(MobaMessageType.Phonto2Client, (int)msgID, msgParam, delayTime, null);
	}

	public static MobaMessage GetMessage(MobaGateCode msgID, object msgParam, float delayTime = 0f)
	{
		return new MobaMessage(MobaMessageType.GateCode, (int)msgID, msgParam, delayTime, null);
	}

	public static MobaMessage GetMessage(LobbyCode msgID, object msgParam, float delayTime = 0f)
	{
		return new MobaMessage(MobaMessageType.LobbyCode, (int)msgID, msgParam, delayTime, null);
	}

	public static MobaMessage GetMessage(MobaMasterCode msgID, object msgParam, float delayTime = 0f)
	{
		return new MobaMessage(MobaMessageType.MasterCode, (int)msgID, msgParam, delayTime, null);
	}

	public static MobaMessage GetMessage(MobaGameCode msgID, object msgParam, float delayTime = 0f)
	{
		return new MobaMessage(MobaMessageType.GameCode, (int)msgID, msgParam, delayTime, null);
	}

	public static MobaMessage GetMessage(PvpCode msgID, object msgParam, float delayTime = 0f, object protoObj = null)
	{
		return new MobaMessage(MobaMessageType.PvpCode, (int)msgID, msgParam, delayTime, protoObj);
	}

	public static MobaMessage GetMessage(MobaChatCode msgID, object msgParam, float delayTime = 0f)
	{
		return new MobaMessage(MobaMessageType.ChatCode, (int)msgID, msgParam, delayTime, null);
	}

	public static MobaMessage GetMessage(MobaFriendCode msgID, object msgParam, float delayTime = 0f)
	{
		return new MobaMessage(MobaMessageType.FriendCode, (int)msgID, msgParam, delayTime, null);
	}

	public static MobaMessage GetMessage(MobaTeamRoomCode msgID, object msgParam, float delayTime = 0f)
	{
		return new MobaMessage(MobaMessageType.TeamRoomCode, (int)msgID, msgParam, delayTime, null);
	}

	public static MobaMessage GetMessage(MobaUserDataCode msgID, object msgParam, float delayTime = 0f)
	{
		return new MobaMessage(MobaMessageType.UserDataCode, (int)msgID, msgParam, delayTime, null);
	}

	public static MobaMessage GetMessage(ClientMsg msgID, object msgParam, float delayTime = 0f)
	{
		return new MobaMessage(MobaMessageType.Client, (int)msgID, msgParam, delayTime, null);
	}

	public static void ExecuteMsg(ClientMsg msgId, object msgParam, float delay = 0f)
	{
		MobaMessage message = MobaMessageManager.GetMessage(msgId, msgParam, delay);
		MobaMessageManager.ExecuteMsg(message);
	}

	public static void ExecuteMsg(ClientC2C msgId, object msgParam, float delay = 0f)
	{
		MobaMessage message = MobaMessageManager.GetMessage((ClientMsg)msgId, msgParam, delay);
		MobaMessageManager.ExecuteMsg(message);
	}

	public static void DispatchMsg(ClientMsg msgId, object msgParam, float delay = 0f)
	{
		MobaMessage message = MobaMessageManager.GetMessage(msgId, msgParam, delay);
		MobaMessageManager.DispatchMsg(message);
	}

	public static void DispatchMsg(PvpCode msgId, object msgParam, float delay = 0f)
	{
		MobaMessage message = MobaMessageManager.GetMessage(msgId, msgParam, delay, null);
		MobaMessageManager.DispatchMsg(message);
	}

	public static void DispatchMsg(ClientC2C msgId, object msgParam)
	{
		MobaMessageManager.DispatchMsg((ClientMsg)msgId, msgParam, 0f);
	}

	public void OnUpdate()
	{
		int count = MobaMessageManager.mMessageQueue.Count;
		for (int i = 0; i < count; i++)
		{
			MobaMessage mobaMessage = MobaMessageManager.mMessageQueue.Dequeue();
			if (mobaMessage.IsDelayExec())
			{
				MobaMessageManager.mMessageQueue.Enqueue(mobaMessage);
			}
			else
			{
				MobaMessageManager.ExecuteMsg(mobaMessage);
			}
		}
	}
}

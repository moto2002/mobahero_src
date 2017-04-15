using MobaClient;
using MobaProtocol.Data;
using System;

namespace Assets.Scripts.Model
{
	public static class ModelTools_chatClientInfo
	{
		public static ChatClientInfo GetChatClientInfo(this ModelManager mmng)
		{
			MobaClient.User user = NetWorkHelper.Instance.client.m_user;
			user.ChatClientInfo = new ChatClientInfo
			{
				IsRoom = false,
				NickName = mmng.Get_userData_filed_X("NickName"),
				RoomId = 0,
				ServerId = NetWorkHelper.Instance.client.GetMobaPeer(MobaPeerType.C2GateServer).ServerName,
				UnionId = ModelManager.Instance.Get_userData_filed_X("UnionId"),
				UserId = ModelManager.Instance.Get_userData_filed_X("UserId")
			};
			return user.ChatClientInfo;
		}
	}
}

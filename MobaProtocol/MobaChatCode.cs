using System;

namespace MobaProtocol
{
	public enum MobaChatCode : byte
	{
		Chat_Login,
		Chat_Logout,
		Chat_Send,
		Chat_Recv,
		Chat_PullHistory,
		Chat_ScanPrivateNoRead,
		Chat_NoticePrivateNewMsg,
		Chat_ListenPrivate,
		Chat_GetPlayerInfo,
		Chat_GetGlobleMsg
	}
}

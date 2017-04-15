using Com.Game.Module;
using System;
using UnityEngine;

public class SendChatMessage : MonoBehaviour
{
	public void ChatInputOnSubmit()
	{
		Singleton<FriendView>.Instance.OnSendMessageBtn(null);
	}

	public void SearchOnSubmit()
	{
		Singleton<FriendView>.Instance.OnFindButton(null);
	}
}

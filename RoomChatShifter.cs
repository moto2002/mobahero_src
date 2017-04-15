using Assets.Scripts.GUILogic.View.HomeChatView;
using Com.Game.Module;
using MobaProtocol.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class RoomChatShifter : MonoBehaviour
{
	public GameObject mClickListener;

	public UILabel mContent;

	private Queue<ChatMessageNew> mMsgQueue = new Queue<ChatMessageNew>();

	private CoroutineManager cMgr;

	private void Awake()
	{
		this.cMgr = new CoroutineManager();
		this.mContent.text = "   点击此处打开聊天窗口";
		this.cMgr.StartCoroutine(this.SetData_IEnumerator(), true);
		UIEventListener.Get(this.mClickListener.gameObject).onClick = new UIEventListener.VoidDelegate(this.OnClickRoomChatOpenView);
		MobaMessageManager.RegistMessage((ClientMsg)23062, new MobaMessageFunc(this.OnMsg_ReceiveRoomChatMessage));
	}

	private void OnDestroy()
	{
		if (this.cMgr != null)
		{
			this.cMgr.StopAllCoroutine();
			this.cMgr = null;
		}
		MobaMessageManager.UnRegistMessage((ClientMsg)23062, new MobaMessageFunc(this.OnMsg_ReceiveRoomChatMessage));
	}

	private void OnMsg_ReceiveRoomChatMessage(MobaMessage msg)
	{
		if (msg.Param != null)
		{
			ChatMessageNew item = (ChatMessageNew)msg.Param;
			this.mMsgQueue.Enqueue(item);
		}
	}

	private void OnClickRoomChatOpenView(GameObject obj = null)
	{
		CtrlManager.OpenWindow(WindowID.HomeChatview, null);
		MobaMessageManagerTools.SendClientMsg(ClientV2C.chatviewOpenView, ChitchatType.Lobby, false);
	}

	private void SetData(ChatMessageNew _msgData)
	{
		if (_msgData != null)
		{
			string text = _msgData.Client.NickName + ":" + _msgData.Message;
			this.mContent.text = text;
		}
	}

	[DebuggerHidden]
	private IEnumerator SetData_IEnumerator()
	{
		RoomChatShifter.<SetData_IEnumerator>c__Iterator158 <SetData_IEnumerator>c__Iterator = new RoomChatShifter.<SetData_IEnumerator>c__Iterator158();
		<SetData_IEnumerator>c__Iterator.<>f__this = this;
		return <SetData_IEnumerator>c__Iterator;
	}

	public void SetDefault()
	{
		this.mContent.text = "   点击此处打开聊天窗口";
		this.cMgr.StartCoroutine(this.SetData_IEnumerator(), true);
	}
}

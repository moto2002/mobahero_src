using Assets.Scripts.GUILogic.View.HomeChatView;
using Com.Game.Module;
using System;
using UnityEngine;

public class TestCheckOpenChatView : MonoBehaviour
{
	private void Start()
	{
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Backslash))
		{
			if (null != Singleton<HomeChatview>.Instance.transform)
			{
				if (Singleton<HomeChatview>.Instance.gameObject.activeInHierarchy)
				{
					CtrlManager.CloseWindow(WindowID.HomeChatview);
				}
				else
				{
					CtrlManager.OpenWindow(WindowID.HomeChatview, null);
					MobaMessageManagerTools.SendClientMsg(ClientV2C.chatviewOpenView, ChitchatType.Hall, false);
				}
			}
			else
			{
				CtrlManager.OpenWindow(WindowID.HomeChatview, null);
				MobaMessageManagerTools.SendClientMsg(ClientV2C.chatviewOpenView, ChitchatType.Hall, false);
			}
		}
	}
}

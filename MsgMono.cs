using System;
using System.Collections.Generic;
using UnityEngine;

public class MsgMono : NewMono
{
	private MsgMono[] _monoChildren = null;

	private Dictionary<Type, Action<GameMessage>> _msgLs;

	public void broadCastMsg(string msg, bool refreshMonoChildren = false)
	{
		if (this._monoChildren == null || refreshMonoChildren)
		{
			this._monoChildren = base.GetComponentsInChildren<MsgMono>();
		}
		if (!ArrayTool.isNullOrEmpty(this._monoChildren))
		{
			MsgMono[] monoChildren = this._monoChildren;
			for (int i = 0; i < monoChildren.Length; i++)
			{
				MsgMono msgMono = monoChildren[i];
				if (!(msgMono == null))
				{
					msgMono.SendMessage(msg, SendMessageOptions.DontRequireReceiver);
				}
			}
		}
	}

	protected void addMsgLs(Type type, Action<GameMessage> action)
	{
		if (this._msgLs == null)
		{
			this._msgLs = new Dictionary<Type, Action<GameMessage>>();
		}
		if (this._msgLs.ContainsKey(type))
		{
			Debug.LogError("add duplicated ls!");
		}
		else
		{
			this._msgLs.Add(type, action);
			MessageManager.addMsgLs(type, action);
		}
	}

	protected void dropMsgs()
	{
		if (this._msgLs != null)
		{
			foreach (Action<GameMessage> current in this._msgLs.Values)
			{
				MessageManager.dropLs(current);
			}
		}
	}
}

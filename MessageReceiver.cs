using System;
using UnityEngine;

public class MessageReceiver
{
	private string _func;

	private GameObject _receiver;

	public MessageReceiver(GameObject receiver, string func)
	{
		this._receiver = receiver;
		this._func = func;
	}

	public void send()
	{
		if (this._receiver && !string.IsNullOrEmpty(this._func))
		{
			this._receiver.SendMessage(this._func, SendMessageOptions.DontRequireReceiver);
		}
	}
}

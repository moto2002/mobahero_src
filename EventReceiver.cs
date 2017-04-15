using System;
using UnityEngine;

public class EventReceiver
{
	private GameObject _receiver;

	private string _func;

	public EventReceiver(GameObject receiver, string func)
	{
		this._receiver = receiver;
		this._func = func;
	}

	public void send(GameObject sender)
	{
		if (this._receiver && !string.IsNullOrEmpty(this._func))
		{
			this._receiver.SendMessage(this._func, sender, SendMessageOptions.DontRequireReceiver);
		}
	}
}

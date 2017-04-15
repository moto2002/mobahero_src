using System;
using UnityEngine;

public class HitObjDiedMsg : GameMessage
{
	private GameObject _go;

	public GameObject go
	{
		get
		{
			return this._go;
		}
	}

	public HitObjDiedMsg(GameObject go)
	{
		this._go = go;
		MessageManager.dispatch(this);
	}
}

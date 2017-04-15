using System;
using UnityEngine;

public class HitObjInstantiatedMsg : GameMessage
{
	private GameObject _go;

	public GameObject go
	{
		get
		{
			return this._go;
		}
	}

	public HitObjInstantiatedMsg(GameObject go)
	{
		this._go = go;
		MessageManager.dispatch(this);
	}
}
